using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarshipCodeGenerator.CodeGenerators

{
    /// <summary>
    /// desc：
    /// author：yjq 2019/8/19 10:45:11
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string TableDescription { get; set; }

        /// <summary>
        /// 列信息列表
        /// </summary>
        public IEnumerable<TableColumnInfo> ColumnList { get; set; }

        public TableColumnInfo GetKeyColumn()
        {
            if (ColumnList == null) return null;
            return ColumnList.Where(m => m.IsKey == 1).OrderBy(m => m.ColumnPosition).FirstOrDefault();
        }

        public string GetBizName(string ignoreFirstPrefix)
        {
            var tableSplitNames = TableName.Split('_');

            if (tableSplitNames[0].Equals(ignoreFirstPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return tableSplitNames[1].ToUpperFirst();
            }
            else if (tableSplitNames.Length > 1)
            {
                if (!tableSplitNames[0].Equals(ignoreFirstPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return tableSplitNames[0].ToUpperFirst();
                }
            }
            return string.Empty;
        }

        public string GetApplicationName(string ignoreFirstPrefix)
        {
            return $"{GetDomainNameWithoutInfo(ignoreFirstPrefix)}Application";
        }

        public string GetIApplicationName(string ignoreFirstPrefix)
        {
            return $"I{GetDomainNameWithoutInfo(ignoreFirstPrefix)}Application";
        }

        public string GetDomainServiceName(string ignoreFirstPrefix)
        {
            return $"{GetDomainNameWithoutInfo(ignoreFirstPrefix)}DomainService";
        }

        public string GetIDomainServiceName(string ignoreFirstPrefix)
        {
            return $"I{GetDomainNameWithoutInfo(ignoreFirstPrefix)}DomainService";
        }

        public string GetRepositoryName(string ignoreFirstPrefix)
        {
            return $"{GetDomainNameWithoutInfo(ignoreFirstPrefix)}Repository";
        }

        public string GetIRepositoryName(string ignoreFirstPrefix)
        {
            return $"I{GetDomainNameWithoutInfo(ignoreFirstPrefix)}Repository";
        }

        public string GetDomainNameWithoutInfo(string ignoreFirstPrefix)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var splits = TableName.Split('_');
            if (splits.Length == 0)
            {
                stringBuilder.Append(TableName.ToUpperFirst());
            }
            else
            {
                for (int i = 0; i < splits.Length; i++)
                {
                    if (i == 0)
                    {
                        if (!ignoreFirstPrefix.Equals(splits[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            stringBuilder.Append(splits[i].ToUpperFirst());
                        }
                    }
                    else
                    {
                        stringBuilder.Append(splits[i].ToUpperFirst());
                    }
                }
            }
            return stringBuilder.ToString();
        }

        public string GetDomainNameWithInfo(string ignoreFirstPrefix)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var splits = TableName.Split('_');
            if (splits.Length == 0)
            {
                stringBuilder.Append(TableName.ToUpperFirst()).Append("Info");
            }
            else
            {
                for (int i = 0; i < splits.Length; i++)
                {
                    if (i == 0)
                    {
                        if (!ignoreFirstPrefix.Equals(splits[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            stringBuilder.Append(splits[i].ToUpperFirst());
                        }
                    }
                    else
                    {
                        stringBuilder.Append(splits[i].ToUpperFirst());
                    }
                }
                stringBuilder.Append("Info");
            }
            return stringBuilder.ToString();
        }

        public string GetTableConstantName()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Table_");
            var splits = TableName.Split('_');
            foreach (var item in splits)
            {
                stringBuilder.Append(item.ToUpperFirst()).Append('_');
            }
            return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
        }

        public string GetTableConstantFileName()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var splits = TableName.Split('_');
            foreach (var item in splits)
            {
                stringBuilder.Append(item.ToUpperFirst()).Append('_');
            }
            return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
        }
    }
}