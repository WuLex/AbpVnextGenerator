using EntityCreater.AddDatabase.MySql.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase.MySql.BLL
{
    public static class MySqlExtend
    {
        //通过对CreateDefaultBuilder扩展，连接数据库，获取数据信息
        public static IEntityBuild AddMySql(this CreateDefaultBuilder builder, string connectionString)
        {
            //连接数据库
            var database = new DAL.MySql(connectionString);
            //获取表信息
            var tableInfos = database.GetTableInfo();
            //返回构建实体模型的类
            return new MySqlEntityBuild(tableInfos);
        }
    }
}