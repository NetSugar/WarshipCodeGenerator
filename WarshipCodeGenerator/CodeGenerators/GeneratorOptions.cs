using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 10:40:36
    /// </summary>
    public class GeneratorOptions
    {
        public string Connection { get; set; }

        public string Schema { get; set; } = "public";

        public string DatabaseType { get; set; }

        public string TablePrefix { get; set; }

        public string IgnorePrefix { get; set; }

        public string TargetFramwork { get; set; } = "netcoreapp2.2";

        public string WarshipVersion { get; set; } = "1.0.0.0";

        public bool IsPublished { get; set; } = true;

        public string BaseNameSpace { get; set; }

        public bool IsOverride { get; set; } = false;

        public string BaseProjectPath { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Connection))
            {
                throw new ArgumentNullException(nameof(Connection));
            }
            if (string.IsNullOrWhiteSpace(BaseNameSpace)) throw new ArgumentNullException(nameof(BaseNameSpace));
            if (string.IsNullOrWhiteSpace(TargetFramwork)) throw new ArgumentNullException(nameof(TargetFramwork));
            if (Schema != null && Schema.IndexOfAny(new char[] { '/', '*', '-' }) >= 0)
            {
                throw new NotSupportedException();
            }
            if (TablePrefix != null && TablePrefix.IndexOfAny(new char[] { '/', '*', '-' }) >= 0)
            {
                throw new NotSupportedException();
            }
            if (IgnorePrefix != null && IgnorePrefix.IndexOfAny(new char[] { '/', '*', '-' }) >= 0)
            {
                throw new NotSupportedException();
            }
            if (DatabaseType == "Npgsql")
            {
                if (string.IsNullOrWhiteSpace(Schema)) throw new ArgumentNullException(nameof(Schema));
                if (Schema.IndexOfAny(new char[] { '/', '*', '-' }) >= 0)
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
