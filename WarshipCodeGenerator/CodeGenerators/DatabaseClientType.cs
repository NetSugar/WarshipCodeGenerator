namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:41:20
    /// </summary>
    public class DatabaseClientType
    {
        public const string Database_Client_Mssql = "System.Data.SqlClient";

        public const string Database_Client_Oracle = "Oracle.ManagedDataAccess.Client";

        public const string Database_Client_Mysql = "MySql.Data.MySqlClient";

        public const string Database_Client_PostgreSql = "Npgsql";
    }
}