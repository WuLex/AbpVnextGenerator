using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase.MySql.Model
{
    public class ColumnInfo
    {
        public string ColumnOrder { get; set; }
        public string ColumnName { get; set; }
        public string TypeName { get; set; }
        public string Length { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool Nullable { get; set; }
        public string DefaultVal { get; set; }
        public string Description { get; set; }
    }
}