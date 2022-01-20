using CodeGenerator.DB;
using CodeGenerator.Helper;
using CodeGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;

namespace CodeGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataDbContext _dataDbContext;

        public HomeController(ILogger<HomeController> logger, DataDbContext dataDbContext)
        {
            _logger = logger;
            _dataDbContext = dataDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            var tables = new List<Table>();
            if (file == null)
            {
                return View();
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var name = reader.ReadLine();
                    if (name != null)
                    {
                        tables.Add(new Table()
                        {
                            Name = name.ToString().Replace("\"", ""),
                            GenerateFile = true
                        });
                    }
                }
            }
            TempData["Tables"] = JsonConvert.SerializeObject(tables.OrderBy(x => x.Name).ToList());
            return RedirectToAction("NamespaceAndTableSelection");
        }

        public IActionResult NamespaceAndTableSelection()
        {
            var tablesSerialized = TempData["Tables"];
            if (tablesSerialized != null)
            {
                var tables = JsonConvert.DeserializeObject<List<Table>>(tablesSerialized.ToString());
                var tableViewModel = new TableViewModel() { Tables = tables };
                return View(tableViewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult NamespaceAndTableSelection(TableViewModel tableViewModel)
        {
            //项目路径
            string filesPath = Directory.GetCurrentDirectory() + "\\Example\\";
            //string path = MyServiceProvider.ServiceProvider.GetRequiredService<IHostEnvironment>().ContentRootPath;

            var pathZip = Directory.GetCurrentDirectory() + @"\CreatedZipFile";
            var pathToDownload = Directory.GetCurrentDirectory() + @"\DownloadFiles";

            if (tableViewModel == null || tableViewModel.Tables == null)
            {
                return RedirectToAction("Index");
            }

            var filesNameToGenerate = Directory.GetFiles(filesPath).ToList();
            FileHelper.DeleteOlderFiles(filesNameToGenerate, pathToDownload);

            foreach (var table in tableViewModel.Tables)
            {
                foreach (var templateFilePath in filesNameToGenerate)
                {
                    if (table.GenerateFile)
                    {
                        FileHelper.CreateFiles(templateFilePath, tableViewModel.Namespace, table.Name);
                    }
                }
            }
            var directoryInfo = new DirectoryInfo(pathZip);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            var zipName = "Code" + tableViewModel.Namespace + ".zip";
            ZipFile.CreateFromDirectory(pathToDownload, pathZip + @"\" + zipName, CompressionLevel.Fastest, false);
            FileHelper.DeleteOlderFiles(filesNameToGenerate, pathToDownload);

            byte[] bytes = System.IO.File.ReadAllBytes(pathZip + @"\" + zipName);

            return File(bytes, "application/octet-stream", zipName);
        }

        [HttpPost]
        public IActionResult LoadTable([FromBody] string connectStr)
        {
            var tables = new List<Table>();

            //Data Source=.;Initial Catalog=BlogDB;Persist Security Info=True;User ID=sa;Password=******
           
            if (string.IsNullOrEmpty(connectStr))
            {
                return NoContent();
            }
            else
            {
                ConstHelper.Connstr = connectStr;
            }
            _dataDbContext.Database.GetDbConnection().ConnectionString = ConstHelper.Connstr;
            var tableList = _dataDbContext.Database.SqlQuery<DbTable>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME NOT LIKE '%Migrations%';").ToList();

            for (int i = 0; i < tableList.Count; i++)
            {
                tables.Add(new Table()
                {
                    Name = tableList[i].TABLE_NAME?.ToString()?.Replace("\"", ""),
                    GenerateFile = true
                });
            }

            TempData["Tables"] = JsonConvert.SerializeObject(tables.OrderBy(x => x.Name).ToList());
            return Json(new {Action="NamespaceAndTableSelection"});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}