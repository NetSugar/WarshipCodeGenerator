using System.Collections.Generic;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:11:48
    /// </summary>
    public class GeneralConstantsUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, string dataBaseName, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.Constants", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.Constants.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion, isAutoIncludeWarshap: true);
            if (tables != null)
            {
                var tablesDirectory = FileUtil.CreateDirectory(projectDirectory, "Tables");
                foreach (var tableInfo in tables)
                {
                    if (tableInfo.GetKeyColumn() == null)
                    {
                        continue;
                    }
                    string baseFilePath = tablesDirectory;
                    var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
                    if (!string.IsNullOrWhiteSpace(bizName))
                    {
                        baseFilePath = FileUtil.CreateDirectory(baseFilePath, bizName);
                    }
                    CreateTableConstants(baseNameSpace, ignoreFirstPrefix, baseFilePath, tableInfo);
                }
            }
            //创建ConfigName
            CreateConfigName(projectDirectory, baseNameSpace, dataBaseName, generatorOptions);
        }

        private static void CreateConfigName(string baseTableFilePath, string baseNameSpace, string databaseName, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseTableFilePath, $"ConfigName.{databaseName.ToUpperFirst()}.cs", GetConfigNameStr(baseNameSpace, databaseName));
            }
            else if (!FileUtil.IsExistsFile(baseTableFilePath, $"ConfigName.{databaseName.ToUpperFirst()}.cs"))
            {
                FileUtil.CreateFile(baseTableFilePath, $"ConfigName.{databaseName.ToUpperFirst()}.cs", GetConfigNameStr(baseNameSpace, databaseName));
            }
        }

        private static string GetConfigNameStr(string baseNameSpace, string databaseName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("namespace {0}.Constants", baseNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\tpublic partial class ConfigNameConstants")
                         .AppendLine("\t{")
                         .AppendFormat("\t\tpublic const string Config_{0} = \"{1}\";", databaseName.Replace(".", "_").ToUpperFirst(), databaseName).AppendLine()
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }

        private static void CreateTableConstants(string baseNameSpace, string ignoreFirstPrefix, string baseTableFilePath, TableInfo tableInfo)
        {
            if (!FileUtil.IsExistsFile(baseTableFilePath, $"{tableInfo.GetTableConstantFileName()}.cs"))
            {
                FileUtil.CreateFile(baseTableFilePath, $"{tableInfo.GetTableConstantFileName()}.cs", GetTableConstantsStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetTableConstantsStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            var keyColumnInfo = tableInfo.GetKeyColumn();
            StringBuilder stringBuilder = new StringBuilder();
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            stringBuilder.AppendLine("using Warship.DataAccess;")
                         .AppendLine()
                         .AppendFormat("namespace {0}.Constants.Tables{1}", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\tpublic partial class TableConstants")
                         .AppendLine("\t{")
                         .AppendLine("\t\t/// <summary>")
                         .AppendFormat("\t\t/// {0}", tableInfo.TableDescription).AppendLine()
                         .AppendLine("\t\t/// </summary>")
                         .AppendFormat("\t\tpublic static readonly TableKeyInfo {0} = new TableKeyInfo(\"{1}\", \"{2}\", {3});", tableInfo.GetTableConstantName(), tableInfo.TableName, keyColumnInfo?.ColumnName, (keyColumnInfo != null && keyColumnInfo.IsIdentity == 1).ToString().ToLower()).AppendLine()
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }
    }
}