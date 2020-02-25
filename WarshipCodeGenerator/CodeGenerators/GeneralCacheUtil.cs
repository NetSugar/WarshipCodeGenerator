using System.Collections.Generic;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2020/2/25 21:36:17
    /// </summary>
    public class GeneralCacheUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, string dataBaseName, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.Cache", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.Cache.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion, isAutoIncludeWarshap: true, $"{baseNameSpace}.Constants", $"{baseNameSpace}.Repository");
        }
    }
}