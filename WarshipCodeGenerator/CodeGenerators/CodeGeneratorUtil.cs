using System;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 17:19:15
    /// </summary>
    public class CodeGeneratorUtil
    {
        public static void Generate(GeneratorOptions generatorOptions)
        {
            if (generatorOptions == null) throw new ArgumentNullException(nameof(generatorOptions));
            generatorOptions.Check();
            var result = InternalUtil.GetTables(generatorOptions.DatabaseType, generatorOptions.Connection, generatorOptions.Schema, generatorOptions.TablePrefix);
            if (result.Item1 != null)
            {
                GeneralConstantsUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item2, result.Item1, generatorOptions);
                GeneralDomainUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item1, generatorOptions);
                GeneralRepositoryUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item2, result.Item1, generatorOptions);
                GeneralDomainServiceUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item1, generatorOptions);
                GeneralApplicationUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item1, generatorOptions);
                GeneralDataTransferUtil.Start(generatorOptions.TargetFramwork, generatorOptions.BaseNameSpace, generatorOptions.IgnorePrefix, result.Item1, generatorOptions);
            }
            //将解决方法包含到项目中
            InternalUtil.IncludeProject(generatorOptions.BaseNameSpace, generatorOptions.BaseProjectPath);
            Console.WriteLine("generator code completed.");
        }
    }
}