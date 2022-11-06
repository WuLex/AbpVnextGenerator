using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EntityCreater.Utility
{
    public interface ICreateEntityFile
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="folderPath">文件路径</param>
        void Create(string folderPath);
    }

    public class CreateEntityFile : ICreateEntityFile
    {
        private Dictionary<string, string> FileContent { get; set; }

        public CreateEntityFile(Dictionary<string, string> fileContent)
        {
            FileContent = fileContent;
        }

        /// <summary>
        /// 创建实体模型
        /// </summary>
        /// <param name="folderPath"></param>
        public void Create(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                //创建文件夹
                Directory.CreateDirectory(folderPath);
            }

            foreach (var item in FileContent)
            {
                string filePath = folderPath + item.Key + ".cs";
                File.WriteAllText(filePath, item.Value);
                Console.WriteLine(item.Key + " 创建完成");
            }
        }
    }
}