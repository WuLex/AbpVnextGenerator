using CodeGenerator.DB;
using CodeGenerator.Helper;
using CodeGenerator.Models;
using EntityCreater;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using EntityCreater.AddDatabase.MySql.BLL;
using EntityCreater.AddDatabase.SqlServer.BLL;

namespace CodeGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly CreateDefaultBuilder _createDefaultBuilder;
        private readonly DataDbContext _dataDbContext;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache,
            CreateDefaultBuilder createDefaultBuilder, DataDbContext dataDbContext)
        {
            _logger = logger;
            _dataDbContext = dataDbContext;
            _memoryCache = memoryCache;
            _createDefaultBuilder = createDefaultBuilder;
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

        /// <summary>
        /// 生成并下载zip
        /// </summary>
        /// <param name="tableViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NamespaceAndTableSelection(TableViewModel tableViewModel)
        {
            #region 验证

            string connectionString = string.Empty;
            if (!_memoryCache.TryGetValue<string>("ConnectStr", out connectionString))
            {
                return RedirectToAction("Index");
            }

            if (tableViewModel == null || tableViewModel.Tables == null)
            {
                return RedirectToAction("Index");
            }

            #endregion

            //项目模板路径
            string templateFilesPath = Directory.GetCurrentDirectory() + "\\Example\\";
            //string path = MyServiceProvider.ServiceProvider.GetRequiredService<IHostEnvironment>().ContentRootPath;

            //zip包下载路径
            var zipPath = Directory.GetCurrentDirectory() + @"\CreatedZipPack";

            //用生成zip包里临时文件路径
            var zipFilesPath = Directory.GetCurrentDirectory() + @"\ZipContainFiles";

            var toGenerateFileFullPathList = Directory.GetFiles(templateFilesPath).ToList();

            //检查并删除旧临时文件
            //FileHelper.DeleteOlderFiles(toGenerateFileFullPathList, zipFilesPath);

            #region 生成实体类

            _createDefaultBuilder //创建默认构建器
                .AddSqlServer(connectionString) //添加SqlServer数据库
                //.AddMySql(connectionString)  //添加MySql数据库
                .EntityBuild(options => //实体类构建
                {
                    //命名空间名（必填）
                    options.NamespaceName = tableViewModel.Namespace ?? "Model";
                    //引用程序集（选填）
                    options.Using = new List<string>
                    {
                        "System.IO",
                        "Volo.Abp.Auditing",
                        "Volo.Abp.Domain.Entities.Auditing",
                    };
                    //使用委托自定义类名规则（默认类名与表名一致，选填）
                    //options.CustomClassName = fileName => fileName + "Entity";
                })
                .Create(zipFilesPath + @"\Domain\"); //创建文件

            #endregion

            #region 遍历表信息,根据模板生成表对应的类临时文件

            foreach (var table in tableViewModel.Tables)
            {
                foreach (var templateFilePath in toGenerateFileFullPathList)
                {
                    if (table.GenerateFile)
                    {
                        //根据模板生成临时文件
                        FileHelper.CreateFiles(templateFilePath, tableViewModel.Namespace, table.Name);
                    }
                }
            }

            #endregion

            #region 遍历删除所有旧zip包

            var directoryInfo = new DirectoryInfo(zipPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            #endregion

            #region 生成新zip包

            var zipName = "Code" + tableViewModel.Namespace + ".zip";

            ZipFile.CreateFromDirectory(zipFilesPath, zipPath + @"\" + zipName, CompressionLevel.Fastest, false);

            //删除本次用来生成zip包的临时文件
            FileHelper.DeleteOlderFiles(toGenerateFileFullPathList, zipFilesPath);

            #endregion

            byte[] bytes = System.IO.File.ReadAllBytes(zipPath + @"\" + zipName);

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

            //保存连接字符串
            _memoryCache.Set<string>("ConnectStr", connectStr);

            #region 查询并遍历表名

            _dataDbContext.Database.GetDbConnection().ConnectionString = ConstHelper.Connstr;
            var tableList = _dataDbContext.Database
                .SqlQuery<DbTable>(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME NOT LIKE '%Migrations%';")
                .ToList();

            for (int i = 0; i < tableList.Count; i++)
            {
                tables.Add(new Table()
                {
                    Name = tableList[i].TABLE_NAME?.ToString()?.Replace("\"", ""),
                    GenerateFile = true
                });
            }

            #endregion

            TempData["Tables"] = JsonConvert.SerializeObject(tables.OrderBy(x => x.Name).ToList());
            return Json(new { Action = "NamespaceAndTableSelection" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}