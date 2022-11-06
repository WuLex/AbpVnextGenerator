using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase.SqlServer.BLL
{
    public static class SqlServerExtend
    {
        //通过对CreateDefaultBuilder扩展，连接数据库，获取数据信息
        public static IEntityBuild AddSqlServer(this CreateDefaultBuilder builder, string connectionString)
        {
            //连接数据库
            var database = new DAL.SqlServer(connectionString);
            //获取表信息
            var tableInfos = database.GetTableInfo();
            //返回构建实体模型的类
            return new SqlServerEntityBuild(tableInfos);
        }
    }
}