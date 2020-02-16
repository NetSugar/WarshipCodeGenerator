using EnvDTE;
using System.IO;
using System.Linq;

namespace WarshipCodeGenerator
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 10:23:34
    /// </summary>
    public class ProjectUtil
    {
        public static string GetBaseProjectDirPath(Project topProject, ProjectItem selectProjectItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var fileBasePath = topProject.FullName.Substring(0, topProject.FullName.LastIndexOf(Path.DirectorySeparatorChar));
            //先判断当前文件加是否包含.sln文件，没有则从上级查找
            if (IsContainFile(fileBasePath, "*.sln"))
            {
                return fileBasePath;
            }
            else
            {
                var prevFilePath = fileBasePath.Substring(0, fileBasePath.LastIndexOf(Path.DirectorySeparatorChar));
                if (IsContainFile(prevFilePath, "*.sln"))
                {
                    return prevFilePath;
                }
                var sencondFilePath = prevFilePath.Substring(0, prevFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                if (IsContainFile(sencondFilePath, ".sln"))
                {
                    return sencondFilePath;
                }
                var thirdFilePath = sencondFilePath.Substring(0, sencondFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                if (IsContainFile(thirdFilePath, ".sln"))
                {
                    return thirdFilePath;
                }
            }
            return fileBasePath;
        }

        private static bool IsContainFile(string baseDirectory, string fileFilter)
        {
            DirectoryInfo theFolder = new DirectoryInfo(baseDirectory);

            FileInfo[] thefileInfos = theFolder.GetFiles(fileFilter, SearchOption.TopDirectoryOnly);
            if (thefileInfos != null && thefileInfos.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前所选文件去除项目目录后的文件夹结构
        /// </summary>
        /// <param name="selectProjectItem"></param>
        /// <returns></returns>
        public static string GetSelectFileDirPath(Project topProject, ProjectItem selectProjectItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string dirPath = "";
            if (selectProjectItem != null)
            {
                //所选文件对应的路径
                string fileNames = selectProjectItem.FileNames[0];
                string selectedFullName = fileNames.Substring(0, fileNames.LastIndexOf('\\'));

                //所选文件所在的项目
                if (topProject != null)
                {
                    //项目目录
                    string projectFullName = topProject.FullName.Substring(0, topProject.FullName.LastIndexOf('\\'));

                    //当前所选文件去除项目目录后的文件夹结构
                    dirPath = selectedFullName.Replace(projectFullName, "");
                }
            }

            return dirPath.Substring(1);
        }

        public static CodeClass GetClass(CodeElements codeElements)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var elements = codeElements.Cast<CodeElement>().ToList();
            var result = elements.FirstOrDefault(codeElement => codeElement.Kind == vsCMElement.vsCMElementClass) as CodeClass;
            if (result != null)
            {
                return result;
            }
            foreach (var codeElement in elements)
            {
                result = GetClass(codeElement.Children);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}