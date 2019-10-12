namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/8/19 10:45:53
    /// </summary>
    public class TableColumnInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名字
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// 列说明
        /// </summary>
        public string ColumnDescription { get; set; }

        /// <summary>
        /// 列默认值
        /// </summary>
        public string ColumnDefaultValue { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public int IsNullable { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public int IsKey { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public int IsIdentity { get; set; }

        /// <summary>
        /// 字段排序
        /// </summary>
        public int ColumnPosition { get; set; }
    }
}