using EntityCreater.AddDatabase.SqlServer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace EntityCreater.AddDatabase.SqlServer.DAL
{
    public class SqlServer
    {
        private SqlConnection conn { get; set; }

        public SqlServer(string ConnectionString)
        {
            conn = new SqlConnection(ConnectionString);
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
                    TableInfo tableInfo = new TableInfo();
                    string tableName = dr["table_name"].ToString();
                    var GetColumnInfo = GetFileds(tableName);
                    var Description = GetDescription(tableName);
                    GetColumnInfo.ForEach(o =>
                    {
                        o.Description = Description.Where(p => p.ColumnName.Equals(o.ColumnName))
                            .Select(p => p.Description).FirstOrDefault();
                    });
                    tables.Add(new TableInfo
                    {
                        TableName = tableName,
                        ColumnInfos = GetColumnInfo
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
            DataTable dt = conn.GetSchema(SqlClientMetaDataCollectionNames.Columns, restrictionValues);
            foreach (DataRow dr in dt.Rows)
            {
                ColumnInfo field = new ColumnInfo();
                field.ColumnName = dr["column_name"].ToString();
                field.TypeName = dr["data_type"].ToString();
                _Fields.Add(field);
            }

            return _Fields;
        }

        private List<ColumnInfo> GetDescription(string tableName)
        {
            List<ColumnInfo> list = new List<ColumnInfo>();
            string sql = string.Format(
                "SELECT  a.name AS ColumnName, isnull(g.[value],'') AS Description FROM  sys.columns a left join sys.extended_properties g  on (a.object_id = g.major_id AND g.minor_id = a.column_id) WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name = '{0}') and g.[value] is not null",
                tableName);
            SqlCommand com = new SqlCommand(sql, conn);
            SqlDataReader dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    list.Add(new ColumnInfo
                    {
                        ColumnName = dr["ColumnName"].ToString(),
                        Description = dr["Description"].ToString()
                    });
                }
            }

            dr.Close();
            return list;
        }
    }
}