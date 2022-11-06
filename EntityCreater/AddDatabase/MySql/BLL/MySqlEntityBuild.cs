using EntityCreater.AddDatabase.MySql.Model;
using EntityCreater.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase.MySql.BLL
{
    //构建实体模型的类
    public class MySqlEntityBuild : IEntityBuild
    {
        private List<TableInfo> TableInfo { get; set; }

        public MySqlEntityBuild(List<TableInfo> tableInfo)
        {
            TableInfo = tableInfo;
        }

        /// <summary>
        /// 构建实体模型方法
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ICreateEntityFile EntityBuild(Action<EntityBuildModel> options)
        {
            //构建实体模型需要的相关参数
            EntityBuildModel model = new EntityBuildModel();
            //委托赋值
            options(model);
            Dictionary<string, string> fileContent = new Dictionary<string, string>();
            //历遍数据库中表信息
            foreach (var item in TableInfo)
            {
                //使用委托自定义类名规则
                if (model.CustomClassName != null)
                {
                    item.TableName = model.CustomClassName(item.TableName);
                }

                //根据数据库表名，字段信息，构建实体模型
                string content = CreatModel(item.TableName, item.ColumnInfos, model.NamespaceName, model.Using);
                //将表名，内容加入字典
                fileContent.Add(item.TableName, content);
            }

            //返回创建模型文件的类
            return new CreateEntityFile(fileContent);
        }

        /// <summary>
        /// 构建实体模型,把相关信息按模型格式拼接成字符串
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnInfos"></param>
        /// <param name="namespaceName"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        private string CreatModel(string tableName, List<ColumnInfo> columnInfos, string namespaceName,
            List<string> reference = null)
        {
            StringBuilder attribute = new StringBuilder();
            foreach (ColumnInfo item in columnInfos)
            {
                attribute.AppendSpaceLine("/// <summary>", 8);
                attribute.AppendSpaceLine($"/// {item.Description}", 8);
                attribute.AppendSpaceLine("/// </summary>", 8);
                attribute.AppendSpaceLine($"public {TypeCast(item.TypeName)} {item.ColumnName}", 8);
                attribute.AppendLine(" { get; set; }");
            }

            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine("using System;", 0, 0);
            strclass.AppendSpaceLine("using System.Collections.Generic;");
            strclass.AppendSpaceLine("using System.Text;");

            if (reference != null)
            {
                reference.ForEach(o => { strclass.AppendSpaceLine($"using {o};"); });
            }

            strclass.AppendSpaceLine($"namespace {namespaceName}", 0, 2);
            strclass.AppendSpaceLine("{");
            strclass.AppendSpaceLine($"public class {tableName} : AuditedAggregateRoot<Guid>,IDeletionAuditedObject",
                4);
            strclass.AppendSpaceLine("{", 4);
            //封装属性
            strclass.AppendSpaceLine(attribute.ToString());
            strclass.AppendSpaceLine("}", 4);
            strclass.AppendSpaceLine("}");
            return strclass.ToString();
        }

        /// <summary>
        /// 数据库字段类型转换为C#属性类型
        /// </summary>
        /// <param name="TypeName">数据库字段类型</param>
        /// <returns></returns>
        private string TypeCast(string TypeName)
        {
            switch (TypeName.ToLower())
            {
                case "bigint":
                    TypeName = "Int64?";
                    break;
                case "binary":
                    TypeName = "byte[]";
                    break;
                case "bit":
                    TypeName = "Boolean?";
                    break;
                case "char":
                    TypeName = "string";
                    break;
                case "date":
                    TypeName = "DateTime?";
                    break;
                case "datetime":
                    TypeName = "DateTime?";
                    break;
                case "decimal":
                    TypeName = "decimal?";
                    break;
                case "double":
                    TypeName = "double?";
                    break;
                case "float":
                    TypeName = "float?";
                    break;
                case "int":
                    TypeName = "int?";
                    break;
                case "text":
                    TypeName = "string";
                    break;
                case "time":
                    TypeName = "DateTime?";
                    break;
                case "tinyint":
                    TypeName = "int?";
                    break;
                case "varbinary":
                    TypeName = "byte[]";
                    break;
                case "varchar":
                    TypeName = "string";
                    break;
                case "smallint":
                    TypeName = "int?";
                    break;
                case "mediumint":
                    TypeName = "int?";
                    break;
                case "longtext":
                    TypeName = "string";
                    break;
                default:
                    TypeName = TypeName.ToLower();
                    break;
            }

            return TypeName;
        }
    }
}