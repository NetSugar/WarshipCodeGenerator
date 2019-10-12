using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:15:02
    /// </summary>
    public class GeneralDomainUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.Domain", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.Domain.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion);
            if (tables != null)
            {
                foreach (var tableInfo in tables.Where(m => m.ColumnList != null && m.ColumnList.Count() > 0))
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
                    CreateDomain(baseNameSpace, baseFilePath, ignoreFirstPrefix, tableInfo, generatorOptions);
                }
            }
        }

        private static void CreateDomain(string baseNameSpace, string baseFilePath, string ignoreFirstPrefix, TableInfo tableInfo, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetDomainNameWithInfo(ignoreFirstPrefix)}.cs", GetDomainStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
            else if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetDomainNameWithInfo(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetDomainNameWithInfo(ignoreFirstPrefix)}.cs", GetDomainStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetDomainStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;").AppendLine()
                         .AppendFormat("namespace {0}.Domain{1}", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic class {0}", tableInfo.GetDomainNameWithInfo(ignoreFirstPrefix)).AppendLine()
                         .AppendLine("\t{");
            foreach (var columnInfo in tableInfo.ColumnList.OrderBy(m => m.ColumnPosition))
            {
                stringBuilder.AppendLine("\t\t/// <summary>");
                stringBuilder.AppendLine("\t\t/// " + columnInfo.ColumnDescription + "");
                stringBuilder.AppendLine("\t\t/// </summary>");
                stringBuilder.AppendLine(string.Format("\t\tpublic {0} {1} {{ get; set; }}", SqlUtil.ChangeDBTypeToCSharpType(columnInfo.DataType, columnInfo.MaxLength, columnInfo.IsNullable == 1), columnInfo.ColumnName));
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("\t}");
            stringBuilder.Append("}").AppendLine();
            return stringBuilder.ToString();
        }
    }
}