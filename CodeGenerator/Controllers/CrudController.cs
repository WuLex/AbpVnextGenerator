using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;
 namespace CodeGenerator.Controllers
{
    /// <summary>
    /// 代码生成器实现方式二
    /// </summary>
    public class CrudController : Controller
    {
        private readonly string _connectionString;
        //项目模板路径
        private readonly string _templatePath;

        public CrudController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _templatePath = Directory.GetCurrentDirectory() + "\\Example\\CrudTemplate.txt";
        }

        public async Task<IActionResult> GenerateAsync(string tbName="")
        {
            // Read table information from database
            var columns = ReadColumnsFromTable(tbName);
            var primaryKey = ReadPrimaryKeyFromTable(tbName);

            // Read template from file
            var template = System.IO.File.ReadAllText(_templatePath);

            // Replace placeholders with actual values
            var code = template.Replace("[TableName]", tbName)
                               .Replace("[PrimaryKey]", primaryKey)
                               .Replace("[Columns]", string.Join(", ", columns.Select(c => $"\"{c}\"")));

            #region MyRegion
            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Integrated Security=true;";
            var tableName = "Books";
            var @namespace = "MyProject.Books";
            var entityName = "Book";
            var dtoName = "BookDto";
            var idTypeName = "int";
            var idName = "Id";
            var createInputName = "CreateBookInput";
            var updateInputName = "UpdateBookInput";
            var getListInputName = "GetBookListInput";
            var baseAppServiceName = "CrudAppService";

            var columnsQuery = @$"
                SELECT COLUMN_NAME, DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = '{tableName}'
            ";

            var columnInfos = new List<(string Name, string DataType)>();
            using (SqlConnection connection=new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(columnsQuery, connection);
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var columnName = (string)reader[0];
                    var dataType = (string)reader[1];
                    columnInfos.Add((columnName, dataType));
                }
            }
           
            var properties = string.Join(Environment.NewLine, columnInfos.Select(c => @$"public {c.DataType} {c.Name} {{ get; set; }}"));

            var content = System.IO.File.ReadAllText("CrudTemplate.txt")
                .Replace("{NAMESPACE}", @namespace)
                .Replace("{ENTITY}", entityName)
                .Replace("{DTO}", dtoName)
                .Replace("{ID}", idName)
                .Replace("{IDTYPE}", idTypeName)
                .Replace("{CREATEINPUT}", createInputName)
                .Replace("{UPDATEINPUT}", updateInputName)
                .Replace("{GETLISTINPUT}", getListInputName)
                .Replace("{BASEAPP}", baseAppServiceName)
                .Replace("{PROPERTIES}", properties);

            var outputFileName = $"{entityName}AppService.cs";
            System.IO.File.WriteAllText(outputFileName, content);
            #endregion



            // Write generated code to file
            var filePath = $"{tableName}Crud.cs";
            System.IO.File.WriteAllText(filePath, code, Encoding.UTF8);

            return Content("CRUD code generated successfully.");
        }

        private string[] ReadColumnsFromTable(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var commandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";

            using var command = new SqlCommand(commandText, connection);

            using var reader = command.ExecuteReader();

            return Enumerable.Range(0, reader.FieldCount)
                             .Select(i => reader.GetName(i))
                             .ToArray();
        }

        private string ReadPrimaryKeyFromTable(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var commandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}'";

            using var command = new SqlCommand(commandText, connection);

            using var reader = command.ExecuteReader();

            return reader.Read() ? reader.GetString(0) : null;
        }
    }
}
