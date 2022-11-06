using EntityCreater.AddDatabase.MySql.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EntityCreater.AddDatabase.MySql.DAL
{
    public class MySql
    {
        private MySqlConnection conn { get; set; }

        public MySql(string ConnectionString)
        {
            conn = new MySqlConnection(ConnectionString);
            conn.Open();
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetTableInfo()
        {
            List<TableInfo> tables = new List<TableInfo>();
            using (DataTable dt = conn.GetSchema("Tables"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string tableName = dr["table_name"].ToString();
                    tables.Add(new TableInfo
                    {
                        TableName = tableName,
                        ColumnInfos = GetFileds(tableName)
                    });
                }
            }

            return tables;
        }

        /// <summary>
        /// 获取表中字段信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<ColumnInfo> GetFileds(string tableName)
        {
            List<ColumnInfo> _Fields = new List<ColumnInfo>();
            string[] restrictionValues = new string[4];
            restrictionValues[0] = null; // Catalog
            restrictionValues[1] = null; // Owner
            restrictionValues[2] = tableName; // Table
            restrictionValues[3] = null; // Column
            DataTable dt = conn.GetSchema("Columns", restrictionValues);
            foreach (DataRow dr in dt.Rows)
            {
                ColumnInfo field = new ColumnInfo();
                field.ColumnName = dr["column_name"].ToString();
                field.TypeName = dr["data_type"].ToString();
                field.Description = dr["COLUMN_COMMENT"].ToString();
                _Fields.Add(field);
            }

            return _Fields;
        }
    }
}