using System;
using System.IO;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 16:09:46
    /// </summary>
    public static class FileUtil
    {
        public static string CreateTempFileNmae(string extensions)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString("N") + "." + extensions;
        }

        public static bool IsExistsDirectory(string directory)
        {
            return Directory.Exists(directory);
        }

        public static string CreateDirectory(params string[] directoryNames)
        {
            var path = Path.Combine(directoryNames);
            CreateDirectory(path);
            return path;
        }

        public static string CreateDirectory(string baseDirctory, string directoryName)
        {
            var path = Path.Combine(baseDirctory, directoryName);
            CreateDirectory(path);
            return path;
        }

        public static void CreateDirectory(string directory)
        {
            if (!IsExistsDirectory(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void CreateFile(string baseDirectory, string fileName, string content)
        {
            var path = Path.Combine(baseDirectory, fileName);
            CreateFile(path, content);
        }

        public static void CreateFile(string baseDirectory, string fileName, string content, Encoding encoding)
        {
            var path = Path.Combine(baseDirectory, fileName);
            CreateFile(path, content, encoding);
        }

        public static void CreateFile(string filePath, string content)
        {
            FileInfo file = new FileInfo(filePath);
            using (FileStream stream = file.Create())
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        public static void CreateFile(string filePath, string content, Encoding encoding)
        {
            FileInfo file = new FileInfo(filePath);
            using (FileStream stream = file.Create())
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        public static bool IsExistsFile(string baseFilePath, string fileName)
        {
            var path = Path.Combine(baseFilePath, fileName);
            return IsExistsFile(path);
        }

        /// <summary>
        /// 判断文件是否存在本地目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 文件是否不存在本地目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsNotExistsFile(string filePath)
        {
            return !IsExistsFile(filePath);
        }

        /// <summary>
        /// 删除指定路径的文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistsFile(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}