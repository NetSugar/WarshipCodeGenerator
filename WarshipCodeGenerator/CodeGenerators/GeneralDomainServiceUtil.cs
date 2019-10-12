using System;
using System.Collections.Generic;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:16:22
    /// </summary>
    public class GeneralDomainServiceUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.DomainService", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.DomainService.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion, isAutoIncludeWarshap: true, $"{baseNameSpace}.Domain", $"{baseNameSpace}.Repository");
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
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIDomainServiceName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
            else if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetIDomainServiceName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIDomainServiceName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetInterfaceStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            stringBuilder.AppendFormat("namespace {0}.DomainService{1}", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic interface I{0}DomainService", tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix)).AppendLine()
                         .AppendLine("\t{")
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }

        private static void CreateImplement(string baseFilePath, string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetDomainServiceName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetDomainServiceName(ignoreFirstPrefix)}.cs", GetImplementStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetImplementStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var domainWithoutInfo = tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix);
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            stringBuilder.AppendFormat("namespace {0}.DomainService{1}.Implement", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic class {0}DomainService :I{0}DomainService", domainWithoutInfo).AppendLine()
                         .AppendLine("\t{")
                         .AppendLine("\t}")
                         .AppendLine("}")
                         ;
            return stringBuilder.ToString();
        }
    }
}