using System.Collections.Generic;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2020/2/25 21:07:22
    /// </summary>
    public class GeneralUnitOfWorkUtil
    {
        public static void Start(string targetFramwork, string baseNameSpace, string ignoreFirstPrefix, string dataBaseName, IEnumerable<TableInfo> tables, GeneratorOptions generatorOptions)
        {
            var projectDirectory = InternalUtil.CreateProjectDirectory($"{baseNameSpace}.UnitOfWork", generatorOptions.BaseProjectPath);
            _ = InternalUtil.CreateProjectFile(targetFramwork, projectDirectory, $"{baseNameSpace}.UnitOfWork.csproj", generatorOptions.IsPublished, generatorOptions.WarshipVersion,isAutoIncludeWarshap:true, $"{baseNameSpace}.Constants");
            CreateIUnitOfWork(projectDirectory, baseNameSpace, dataBaseName, generatorOptions);
            var baseFilePath = FileUtil.CreateDirectory(projectDirectory, "Implement");
            CreateUnitOfWork(baseFilePath, baseNameSpace, dataBaseName, generatorOptions);
        }

        private static void CreateIUnitOfWork(string baseTableFilePath, string baseNameSpace, string databaseName, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseTableFilePath, $"I{databaseName.ToUpperFirst()}UnitOfWork.cs", GetIUnitOfWorkStr(baseNameSpace, databaseName));
            }
            else if (!FileUtil.IsExistsFile(baseTableFilePath, $"I{databaseName.ToUpperFirst()}UnitOfWork.cs"))
            {
                FileUtil.CreateFile(baseTableFilePath, $"I{databaseName.ToUpperFirst()}UnitOfWork.cs", GetIUnitOfWorkStr(baseNameSpace, databaseName));
            }
        }

        private static string GetIUnitOfWorkStr(string baseNameSpace, string databaseName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("using Warship.DataAccess;").AppendLine()
                         .AppendLine()
                         .AppendFormat("namespace {0}.UnitOfWork", baseNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine($"\tpublic interface I{databaseName.ToUpperFirst()}UnitOfWork : IBaseUnitOfWork")
                         .AppendLine("\t{")
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }

        private static void CreateUnitOfWork(string baseTableFilePath, string baseNameSpace, string databaseName, GeneratorOptions generatorOptions)
        {
            if (generatorOptions.IsOverride)
            {
                FileUtil.CreateFile(baseTableFilePath, $"{databaseName.ToUpperFirst()}UnitOfWork.cs", GetUnitOfWorkStr(baseNameSpace, databaseName));
            }
            else if (!FileUtil.IsExistsFile(baseTableFilePath, $"{databaseName.ToUpperFirst()}UnitOfWork.cs"))
            {
                FileUtil.CreateFile(baseTableFilePath, $"{databaseName.ToUpperFirst()}UnitOfWork.cs", GetUnitOfWorkStr(baseNameSpace, databaseName));
            }
        }

        private static string GetUnitOfWorkStr(string baseNameSpace, string databaseName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat($"using {baseNameSpace}.Constants;").AppendLine()
                         .AppendFormat("using Warship.DataAccess;").AppendLine()
                         .AppendLine()
                         .AppendFormat("namespace {0}.UnitOfWork.Implement", baseNameSpace).AppendLine()
                         .AppendLine("{")
                         .AppendLine($"\tpublic class {databaseName.ToUpperFirst()}UnitOfWork : DefaultBaseUnitOfWork , I{databaseName.ToUpperFirst()}UnitOfWork")
                         .AppendLine("\t{")
                         .AppendLine($"\t\tpublic {databaseName.ToUpperFirst()}UnitOfWork(IDataAccessFactory dataAccessFactory) : base(dataAccessFactory, ConfigNameConstants.Config_{databaseName.ToUpperFirst()})")
                         .AppendLine("\t\t{")
                         .AppendLine("\t\t}")
                         .AppendLine("\t}")
                         .AppendLine("}");
            return stringBuilder.ToString();
        }
    }
}