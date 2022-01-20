using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Models
{
    public class TableViewModel
    {
        public TableViewModel()
        {
        }

        public TableViewModel(List<Table>? tables, IFormFile? file, string name)
        {
            Tables = tables;
            File = file;
            Namespace = name;
        }
        public string Namespace { get; set; }

        public List<Table>? Tables { get; set; }

        public IFormFile? File{ get; set; }
    }
}
