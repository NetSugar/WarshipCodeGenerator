using System;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 16:40:39
    /// </summary>
    public static class SqlUtil
    {
        public static string BuildSelectSysTableSql(string databaseName, string schema, string tablePrefix, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    return BuildSelectMySqlSysTableSql(databaseName, tablePrefix);

                case DatabaseType.MSSQLServer:
                    return BuildSelectSqlServerSysTableSql(tablePrefix);

                case DatabaseType.PostgreSqlClient:
                    return BuildSelectPostgreSqlSysTableSql(schema, tablePrefix);

                case DatabaseType.Oracle:
                default:
                    throw new NotSupportedException(databaseType.ToString());
            }
        }

        private static string BuildSelectPostgreSqlSysTableSql(string schema, string tablePrefix)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT a.relname AS TableName, b.description AS TableDescription FROM pg_class a LEFT OUTER JOIN pg_description b ON b.objsubid=0 AND a.oid = b.objoid where 1=1");
            if (!string.IsNullOrWhiteSpace(schema))
            {
                sqlBuilder.Append($" and a.relnamespace = (SELECT oid FROM pg_namespace WHERE nspname='{schema}')");
            }
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                sqlBuilder.Append($" and a.relname LIKE CONCAT('{tablePrefix}','%')");
            }
            sqlBuilder.Append(" order by a.relname;");
            return sqlBuilder.ToString();
        }

        private static string BuildSelectMySqlSysTableSql(string databaseName, string tablePrefix)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT TABLE_NAME TableName,TABLE_COMMENT TableDescription FROM INFORMATION_SCHEMA.TABLES where 1=1");
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                sqlBuilder.Append($" and TABLE_SCHEMA='{databaseName}'");
            }
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                sqlBuilder.Append($" and TABLE_NAME LIKE CONCAT('{tablePrefix}','%')'");
            }
            sqlBuilder.Append(" order by CREATE_TIME;");
            return sqlBuilder.ToString();
        }

        private static string BuildSelectSqlServerSysTableSql(string tablePrefix)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("ELECT D.NAME TableName,F.VALUE TableDescription FROM  SYSCOLUMNS A INNER JOIN SYSOBJECTS D  ON A.ID=D.ID  AND D.XTYPE='U' AND  D.NAME<>'DTPROPERTIES' LEFT JOIN sys.extended_properties f ON D.ID=F.major_id AND F.minor_id=0 where a.colorder=1");
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                sqlBuilder.Append($" and D.NAMEE LIKE '{tablePrefix}%'");
            }
            sqlBuilder.Append(" order by D.NAME;");
            return sqlBuilder.ToString();
        }

        public static string BuildSelectSysTableColumnSql(string databaseName, string schema, string tablePrefix, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    return BuildSelectMySqlSysTableColumnSql(databaseName, tablePrefix);

                case DatabaseType.MSSQLServer:
                    return BuildSelectSqlServerSysTableColumnSql(tablePrefix);

                case DatabaseType.PostgreSqlClient:
                    return BuildSelectPostgreSqlSysTableColumnSql(schema, tablePrefix);

                case DatabaseType.Oracle:
                default:
                    throw new NotSupportedException(databaseType.ToString());
            }
        }

        private static string BuildSelectPostgreSqlSysTableColumnSql(string schema, string tablePrefix)
        {
            StringBuilder stringBuilder = new StringBuilder();

            const string baseSelectSql = @"SELECT cc.table_name as TableName ,cc.column_name as ColumnName,t.typname as DataType,a.attlen as MaxLength,b.description as ColumnDescription,cc.column_default as ColumnDefaultValue,CASE cc.is_nullable WHEN 'NO' THEN 0 ELSE 1 END IsNullable,
			CASE (SELECT 1 FROM  pg_index,  pg_class,  pg_attribute  WHERE  pg_class.oid = c.relname :: regclass AND pg_index.indrelid = pg_class.oid AND pg_attribute.attrelid = pg_class.oid  AND pg_attribute.attnum = ANY (pg_index.indkey) AND pg_attribute.attname=a.attname) WHEN 1 THEN 1 else 0 END IsKey,
			 CASE  cc.is_identity WHEN 'NO' THEN 0 ELSE 1 END as IsIdentity,
				cc.ordinal_position as ColumnPosition
  FROM pg_class c INNER JOIN pg_attribute a on a.attrelid = c.oid
       LEFT  JOIN pg_description b ON a.attrelid=b.objoid AND a.attnum = b.objsubid
       INNER JOIN pg_type t on a.atttypid = t.oid
			 LEFT JOIN information_schema.COLUMNS CC ON CC.TABLE_NAME=c.relname AND CC.COLUMN_NAME=a.attname where 1=1 ";
            stringBuilder.Append(baseSelectSql);
            if (!string.IsNullOrWhiteSpace(schema))
            {
                stringBuilder.Append($" and CC.table_schema='{schema}'");
            }
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                stringBuilder.Append($" and cc.table_name LIKE CONCAT('{tablePrefix}','%')");
            }

            stringBuilder.Append(" order by cc.table_name,cc.ordinal_position;");
            return stringBuilder.ToString();
        }

        private static string BuildSelectMySqlSysTableColumnSql(string databaseName, string tablePrefix)
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT a.TABLE_NAME AS TableName,b.COLUMN_NAME AS ColumnName,b.DATA_TYPE AS DataType,b.CHARACTER_MAXIMUM_LENGTH AS MaxLength,b.COLUMN_COMMENT AS ColumnDescription,b.COLUMN_DEFAULT AS ColumnDefaultValue,case b.IS_NULLABLE when  'NO' THEN 0 ELSE 1  END IsNullable,CASE b.COLUMN_KEY WHEN 'PRI' THEN 1  ELSE 0 END IsKey, CASE b.EXTRA WHEN 'auto_increment' THEN 1 ELSE 0 END IsIdentity,B.ORDINAL_POSITION AS ColumnPosition  FROM information_schema.TABLES a INNER JOIN information_schema.COLUMNS b ON a.table_name = b.TABLE_NAME AND b.table_schema=a.table_schema where 1=1");
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                stringBuilder.Append($" and a.table_schema='{databaseName}'");
            }
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                stringBuilder.Append($" and a.TABLE_NAME LIKE CONCAT('{tablePrefix}','%')'");
            }
            stringBuilder.Append(" order by a.table_name,B.ORDINAL_POSITION;");
            return stringBuilder.ToString();
        }

        private static string BuildSelectSqlServerSysTableColumnSql(string tablePrefix)
        {
            const string baseSql = @"SELECT  Sysobjects.name AS TableName ,
								syscolumns.name AS ColumnName ,
								systypes.name AS DataType ,
								syscolumns.length AS MaxLength ,
								sys.extended_properties.[value] AS ColumnDescription ,
								syscomments.text AS ColumnDefaultValue ,
								syscolumns.isnullable AS IsNullable,
                                (case when exists(SELECT 1 FROM sysobjects where xtype= 'PK' and name in (
                                SELECT name FROM sysindexes WHERE indid in(
                                SELECT indid FROM sysindexkeys WHERE id = syscolumns.id AND colid=syscolumns.colid
                                ))) then 1 else 0 end) as IsKey,
                                COLUMNPROPERTY(syscolumns.id, syscolumns.name, 'IsIdentity') IsIdentity,
                                syscolumns.colorder as ColumnPosition
								FROM    syscolumns
								INNER JOIN systypes ON syscolumns.xtype = systypes.xtype
								LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
								LEFT OUTER JOIN sys.extended_properties ON ( sys.extended_properties.minor_id = syscolumns.colid
																			 AND sys.extended_properties.major_id = syscolumns.id
																		   )
								LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id
								WHERE   syscolumns.id IN ( SELECT   id
												   FROM     SYSOBJECTS
												   WHERE    xtype='U' )
								AND  systypes.name not IN('hierarchyid','sysname','geometry','geography') where 1=1";
            StringBuilder stringBuilder = new StringBuilder(baseSql);
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                stringBuilder.Append($" and Sysobjects.name LIKE '{tablePrefix}%'");
            }
            stringBuilder.Append(" order by Sysobjects.name,syscolumns.colorder;");
            return stringBuilder.ToString();
        }

        public static string ChangeDBTypeToCSharpType(string typeName, long length, bool isNullable = true)
        {
            string reval;
            switch (typeName.ToLower())
            {
                case "int":
                    reval = "int" + (isNullable ? "?" : string.Empty);
                    break;

                case "int8":
                case "bigint":
                    reval = "long" + (isNullable ? "?" : string.Empty);
                    break;

                case "binary":
                    reval = "object";
                    break;

                case "bit":
                    reval = "bool" + (isNullable ? "?" : string.Empty);
                    break;

                case "datetime":
                case "date":
                case "time":
                case "datetime2":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "datetimeoffset":
                    reval = "DateTimeOffset" + (isNullable ? "?" : string.Empty);
                    break;

                case "single":
                case "decimal":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "float":
                    reval = "double" + (isNullable ? "?" : string.Empty);
                    break;

                case "image":
                    reval = "byte[]";
                    break;

                case "money":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "text":
                case "longtext":
                case "char":
                case "nvarchar":
                case "nchar":
                case "ntext":
                    reval = "string";
                    break;

                case "numeric":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "real":
                    reval = "float" + (isNullable ? "?" : string.Empty);
                    break;

                case "smalldatetime":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "smallint":
                    reval = "short" + (isNullable ? "?" : string.Empty);
                    break;

                case "smallmoney":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "timestamp":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "tinyint":
                    if (length == 1) reval = "bool" + (isNullable ? "?" : string.Empty);
                    else reval = "byte" + (isNullable ? "?" : string.Empty);
                    break;

                case "uniqueidentifier":
                    reval = "Guid" + (isNullable ? "?" : string.Empty);
                    break;

                case "varbinary":
                    reval = "byte[]";
                    break;

                case "varchar":
                    reval = "string";
                    break;

                case "sql_variant":
                case "variant":
                    reval = "object";
                    break;

                default:
                    reval = "other";
                    break;
            }
            return reval;
        }

        /// <summary>
        /// 将SqlType转成C#Type
        /// </summary>
        /// <param name="typeName">数据库类型</param>
        /// <param name="isNullable">是否可空</param>
        /// <returns></returns>
        public static string ChangeDBTypeToCSharpType(string typeName, bool isNullable = true)
        {
            string reval;
            switch (typeName.ToLower())
            {
                case "int":
                    reval = "int" + (isNullable ? "?" : string.Empty);
                    break;

                case "int8":
                case "bigint":
                    reval = "long" + (isNullable ? "?" : string.Empty);
                    break;

                case "binary":
                    reval = "object";
                    break;

                case "bit":
                    reval = "bool" + (isNullable ? "?" : string.Empty);
                    break;

                case "datetime":
                case "date":
                case "time":
                case "datetime2":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "datetimeoffset":
                    reval = "DateTimeOffset" + (isNullable ? "?" : string.Empty);
                    break;

                case "single":
                case "decimal":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "float":
                    reval = "double" + (isNullable ? "?" : string.Empty);
                    break;

                case "image":
                    reval = "byte[]";
                    break;

                case "money":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "text":
                case "char":
                case "nvarchar":
                case "nchar":
                case "ntext":
                    reval = "string";
                    break;

                case "numeric":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "real":
                    reval = "float" + (isNullable ? "?" : string.Empty);
                    break;

                case "smalldatetime":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "smallint":
                    reval = "short" + (isNullable ? "?" : string.Empty);
                    break;

                case "smallmoney":
                    reval = "decimal" + (isNullable ? "?" : string.Empty);
                    break;

                case "timestamp":
                    reval = "DateTime" + (isNullable ? "?" : string.Empty);
                    break;

                case "tinyint":
                    reval = "byte" + (isNullable ? "?" : string.Empty);
                    break;

                case "uniqueidentifier":
                    reval = "guid" + (isNullable ? "?" : string.Empty);
                    break;

                case "varbinary":
                    reval = "byte[]";
                    break;

                case "varchar":
                    reval = "string";
                    break;

                case "sql_variant":
                case "variant":
                    reval = "object";
                    break;

                default:
                    reval = "other";
                    break;
            }
            return reval;
        }
    }
}