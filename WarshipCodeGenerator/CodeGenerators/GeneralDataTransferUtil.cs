using System.Collections.Generic;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:13:46
    /// </summary>
    public class GeneralDataTransferUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.DataTransfer", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.DataTransfer.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion);
            if (tables != null)
            {
                foreach (var tableInfo in tables)
                {
                    if (tableInfo.GetKeyColumn() == null)
                    {
                        continue;
                    }
                    string baseFilePath = projectDirectory;
                    var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
                    if (!string.IsNullOrWhiteSpace(bizName))
                    {
                        baseFilePath = FileUtil.CreateDirectory(baseFilePath, bizName);
                    }
                }
            }
        }
    }
}