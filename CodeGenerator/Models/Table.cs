namespace CodeGenerator.Models
{
    public class Table
    {
        public Table()
        {
        }

        public Table(string name, bool generateFile)
        {
            Name = name;
            GenerateFile = generateFile;
        }

        public string Name { get; set; }
        public bool GenerateFile { get; set; }

    }
}
