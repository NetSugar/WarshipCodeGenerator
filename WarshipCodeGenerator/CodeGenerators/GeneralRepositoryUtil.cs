using System;
using System.Collections.Generic;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:17:58
    /// </summary>
    public class GeneralRepositoryUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, string databaseName, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.Repository", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.Repository.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion, isAutoIncludeWarshap: true, $"{baseNameSpace}.Domain", $"{baseNameSpace}.Constants");
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
                    CreateImplement(baseFilePath, baseNameSpace, ignoreFirstPrefix, databaseName, tableInfo);
                }
            }
        }

        private static void CreateInterface(string baseFilePath, string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIRepositoryName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
            else if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetIRepositoryName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetIRepositoryName(ignoreFirstPrefix)}.cs", GetInterfaceStr(baseNameSpace, ignoreFirstPrefix, tableInfo));
            }
        }

        private static string GetInterfaceStr(string baseNameSpace, string ignoreFirstPrefix, TableInfo tableInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var keyColumnInfo = tableInfo.GetKeyColumn();
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            stringBuilder.AppendLine("using System;")
                         .AppendLine("using Warship.DataAccess;")
                         .AppendFormat("using {0}.Domain{1};", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine()
                         .AppendFormat("namespace {0}.Repository{1}", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic interface I{0}Repository : IDataRepository<{0}Info, {1}>", tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix), SqlUtil.ChangeDBTypeToCSharpType(keyColumnInfo.DataType, keyColumnInfo.MaxLength, keyColumnInfo.IsNullable == 1)).AppendLine()
                         .AppendLine("\t{")
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }

        public static void CreateImplement(string baseFilePath, string baseNameSpace, string ignoreFirstPrefix, string databaseName, TableInfo tableInfo)
        {
            if (!FileUtil.IsExistsFile(baseFilePath, $"{tableInfo.GetRepositoryName(ignoreFirstPrefix)}.cs"))
            {
                FileUtil.CreateFile(baseFilePath, $"{tableInfo.GetRepositoryName(ignoreFirstPrefix)}.cs", GetImplementStr(baseNameSpace, ignoreFirstPrefix, databaseName, tableInfo));
            }
        }

        public static string GetImplementStr(string baseNameSpace, string ignoreFirstPrefix, string databaseName, TableInfo tableInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var keyColumnInfo = tableInfo.GetKeyColumn();
            var domainWithoutInfo = tableInfo.GetDomainNameWithoutInfo(ignoreFirstPrefix);
            var keyStr = SqlUtil.ChangeDBTypeToCSharpType(keyColumnInfo.DataType, keyColumnInfo.MaxLength, keyColumnInfo.IsNullable == 1);
            var bizName = tableInfo.GetBizName(ignoreFirstPrefix);
            var bizNameSpace = string.IsNullOrWhiteSpace(bizName) ? string.Empty : "." + bizName;
            stringBuilder.AppendLine("using System;")
                         .AppendLine("using Warship.DataAccess;")
                         .AppendFormat("using {0}.Domain{1};", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendFormat("using {0}.Constants;", baseNameSpace).AppendLine()
                         .AppendFormat("using {0}.Constants.Tables{1};", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine()
                         .AppendFormat("namespace {0}.Repository{1}.Implement", baseNameSpace, bizNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine("\t/// <summary>")
                         .AppendFormat("\t/// desc：{0}", tableInfo.TableDescription).AppendLine()
                         .AppendFormat("\t/// table：{0}", tableInfo.TableName).AppendLine()
                         .AppendFormat("\t/// author：template {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
                         .AppendLine("\t/// </summary>")
                         .AppendFormat("\tpublic class {0}Repository : BaseDataRepository<{0}Info, {1}>, I{0}Repository", domainWithoutInfo, keyStr).AppendLine()
                         .AppendLine("\t{")
                         .AppendFormat("\t\tpublic {0}Repository(IDataAccessFactory dataAccessFactory) : base(dataAccessFactory, ConfigNameConstants.Config_{1}, TableConstants.{2})", domainWithoutInfo, databaseName.Replace(".", "_").ToUpperFirst(), tableInfo.GetTableConstantName()).AppendLine()
                         .AppendLine("\t\t{")
                         .AppendLine("\t\t}")
                         .AppendLine()
                         .AppendFormat("\t\tprotected override Action<{0}Info, {1}> SetKey => (info,key) => info.{2} = key ;", domainWithoutInfo, keyStr, keyColumnInfo.ColumnName).AppendLine()
                         .AppendLine("\t}")
                         .AppendLine("}")
                         ;
            return stringBuilder.ToString();
        }
    }
}