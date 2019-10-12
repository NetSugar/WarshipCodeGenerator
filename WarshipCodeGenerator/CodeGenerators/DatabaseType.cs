using System;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 16:41:17
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// MSSQLServer
        /// </summary>
        MSSQLServer = 1,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 2,

        /// <summary>
        /// MySql
        /// </summary>
        MySql = 3,

        /// <summary>
        /// PostgreSqlClient
        /// </summary>
        PostgreSqlClient = 4
    }

    public static class DatabaseTypeExtensions
    {
        public static DatabaseType ToDatabaseType(this string clientType)
        {
            switch (clientType)
            {
                case "System.Data.SqlClient":
                    return DatabaseType.MSSQLServer;

                case "Oracle.ManagedDataAccess.Client":
                    return DatabaseType.Oracle;

                case "MySql.Data.MySqlClient":
                    return DatabaseType.MySql;

                case "Npgsql":
                    return DatabaseType.PostgreSqlClient;

                default:
                    throw new NotSupportedException("clientType");
            }
        }
    }
}