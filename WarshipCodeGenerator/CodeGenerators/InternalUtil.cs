using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 16:07:54
    /// </summary>
    public static class InternalUtil
    {
        public static string ToLowwerFirst(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            if (str.Length == 1)
            {
                return str.Substring(0, 1).ToLower();
            }
            if (str.Length >= 2)
            {
                return str.Substring(0, 1).ToLower() + str.Substring(1);
            }
            return string.Empty;
        }

        public static string ToUpperFirst(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            if (str.Length == 1)
            {
                return str.Substring(0, 1).ToUpper();
            }
            if (str.Length >= 2)
            {
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            return string.Empty;
        }

        public static string CreateProjectDirectory(string projectDirectory, string projectBaseDirectory)
        {
            string directory = $"{projectBaseDirectory}{Path.DirectorySeparatorChar}{projectDirectory}";
            FileUtil.CreateDirectory(directory);
            return directory;
        }

        public static string CreateProjectFile(string targetFramwork, string projectDirectory, string projectFileName, bool isPublished, string warshipVersion, bool isAutoIncludeWarshap = true, params string[] includeProjects)
        {
            var projectFilePath = Path.Combine(projectDirectory, projectFileName);
            if (FileUtil.IsNotExistsFile(projectFilePath))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">").AppendLine()
                             .AppendLine("\t<PropertyGroup>")
                             .AppendFormat("\t\t<TargetFramework>{0}</TargetFramework>", targetFramwork).AppendLine()
                             .AppendLine("\t\t</PropertyGroup>").AppendLine();
                if (!isPublished)
                {
                    stringBuilder.AppendLine("\t<ItemGroup>");
                    if (isAutoIncludeWarshap)
                    {
                        stringBuilder.AppendFormat("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\" />", "Warship").AppendLine();
                    }
                    if (includeProjects != null)
                    {
                        foreach (var includeProject in includeProjects)
                        {
                            stringBuilder.AppendFormat("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\" />", includeProject).AppendLine();
                        }
                    }
                    stringBuilder.AppendLine("\t</ItemGroup>");
                }
                else
                {
                    if (includeProjects != null)
                    {
                        stringBuilder.AppendLine("\t<ItemGroup>");
                        foreach (var includeProject in includeProjects)
                        {
                            stringBuilder.AppendFormat("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\" />", includeProject).AppendLine();
                        }
                        stringBuilder.AppendLine("\t</ItemGroup>");
                    }
                }
                if (isPublished && isAutoIncludeWarshap)
                {
                    stringBuilder.AppendLine("\t<ItemGroup>");
                    stringBuilder.AppendFormat("\t\t<PackageReference Include=\"Warship\" Version=\"{0}\" />", warshipVersion).AppendLine();
                    stringBuilder.AppendLine("\t</ItemGroup>");
                }

                stringBuilder.AppendLine("</Project>");
                FileUtil.CreateFile(projectFilePath, stringBuilder.ToString());
            }
            return projectFilePath;
        }

        public static void IncludeProject(string baseNameSpace, string projectBaseDirectory)
        {
            try
            {
                string[] projectNames = new string[] {
                $"{baseNameSpace}.Application",
                $"{baseNameSpace}.Constants",
                $"{baseNameSpace}.Domain",
                $"{baseNameSpace}.Cache",
                $"{baseNameSpace}.UnitOfWork",
                $"{baseNameSpace}.Repository",
                $"{baseNameSpace}.DomainService",
                $"{baseNameSpace}.DataTransfer",
                };
                var slnFilePath = Directory.GetFiles(projectBaseDirectory, "*.sln").FirstOrDefault();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var project in projectNames)
                {
                    stringBuilder.AppendFormat(" {0}{1}{2}{1}{2}.csproj", projectBaseDirectory, Path.DirectorySeparatorChar, project);
                }
                var comdStr = $"sln {slnFilePath} add {stringBuilder.ToString()}";
                var psi = new ProcessStartInfo("dotnet", comdStr) { RedirectStandardOutput = true, UseShellExecute = false };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
            }
        }

        public static (IEnumerable<TableInfo>, string) GetTables(string clientType, string connection, string schema, string tablePrefix)
        {
            var databaseType = clientType.ToDatabaseType();
            var dbConntction = Create(databaseType, connection);
            string databaseName = dbConntction.Database;
            var sqlquery = SqlUtil.BuildSelectSysTableSql(databaseName, schema, tablePrefix, databaseType);
            var tables = dbConntction.Query<TableInfo>(sqlquery);
            var tableColumnSqlQuery = SqlUtil.BuildSelectSysTableColumnSql(databaseName, schema, tablePrefix, databaseType);
            var columns = dbConntction.Query<TableColumnInfo>(tableColumnSqlQuery);
            if (tables != null && columns != null)
            {
                foreach (var table in tables)
                {
                    table.ColumnList = columns.Where(m => m.TableName == table.TableName);
                }
            }
            return (tables, databaseName);
        }

        public static IDbConnection Create(DatabaseType databaseType, string connectionStr)
        {
            IDbConnection connection;
            switch (databaseType)
            {
                case DatabaseType.MSSQLServer:
                    connection = new SqlConnection(connectionStr);
                    break;

                case DatabaseType.MySql:
                    connection = new MySqlConnection(connectionStr);
                    break;

                case DatabaseType.Oracle:
                    connection = new OracleConnection(connectionStr);
                    break;

                case DatabaseType.PostgreSqlClient:
                    connection = new NpgsqlConnection(connectionStr);
                    break;

                default:
                    throw new System.NotSupportedException(databaseType.ToString());
            }

            connection.Open();
            return connection;
        }
    }
}