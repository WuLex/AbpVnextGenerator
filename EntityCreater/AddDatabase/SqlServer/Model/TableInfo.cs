using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase.SqlServer.Model
{
    public class TableInfo
    {
        public string TableName { get; set; }
        public List<ColumnInfo> ColumnInfos { get; set; }
    }
}