using System;
using System.Collections.Generic;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:09:23
    /// </summary>
    public class GeneralApplicationUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.Application", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.Application.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion, isAutoIncludeWarshap: true, $"{baseNameSpace}.Domain", $"{baseNameSpace}.Repository", $"{baseNameSpace}.DomainService");
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
                    CreateInterface(baseFilePath, baseNameSpace, ignoreFirstPrefix, tableInfo, generatorOptions);
                    baseFilePath = FileUtil.CreateDirectory(baseFilePath, "Implement");
                    CreateImplement(baseFilePath, baseNameSpace, ignoreFirstPrefix, tableInfo);
                }
            }
        }

        private static void CreateInterface(string baseFilePath, string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIApplicationName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
            else if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetIApplicationName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIApplicationName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetInterfaceStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Warship.Results;").AppendLine()
                         .AppendFormat("namespace {0}.Application{1}", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic interface I{0}Application", tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix)).AppendLine()
                         .AppendLine("\t{")
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }

        private static void CreateImplement(string baseFilePath, string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetApplicationName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetApplicationName(ignoreFirstPrefix)}.cs", GetImplementStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetImplementStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            StringBuilder stringBuilder = new StringBuilder();
            var domainWithoutInfo = tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix);
            stringBuilder.AppendLine("using Warship.Results;")
                         .AppendFormat("using {0}.Repository{1};", baseNameSpace, bizNameSpace).AppendLine().AppendLine()
                         .AppendFormat("namespace {0}.Application{1}.Implement", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic class {0}Application :I{0}Application", domainWithoutInfo).AppendLine()
                         .AppendLine("\t{")
                         .AppendFormat("\t\tprivate readonly I{0}Repository _{1}Repository;", domainWithoutInfo, domainWithoutInfo.ToLowwerFirst()).AppendLine()
                         .AppendLine()
                         .AppendFormat("\t\tpublic {0}Application(I{0}Repository {1}Repository)", domainWithoutInfo, domainWithoutInfo.ToLowwerFirst()).AppendLine()
                         .AppendLine("\t\t{")
                         .AppendFormat("\t\t\t_{0}Repository = {0}Repository;", domainWithoutInfo.ToLowwerFirst()).AppendLine()
                         .AppendLine("\t\t}")
                         .AppendLine("\t}")
                         .AppendLine("}")
                         ;
            return stringBuilder.ToString();
        }
    }
}