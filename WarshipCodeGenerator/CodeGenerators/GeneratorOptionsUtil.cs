using EnvDTE;

namespace WarshipCodeGenerator.CodeGenerators
{
    /// <summary>
    /// desc：
    /// author：yjq 2019/10/12 10:41:52
    /// </summary>
    public class GeneratorOptionsUtil
    {
        public static GeneratorOptions Create(CodeClass codeClass, string baseDirPath)
        {
            var generatorOptions = new GeneratorOptions();
            generatorOptions.BaseProjectPath = baseDirPath;
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            foreach (CodeAttribute classAttribute in codeClass.Attributes)
            {
                switch (classAttribute.Name.ToLower())
                {
                    //[CodeGeneratorConnection("localhost")]
                    case "codegeneratorconnection":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.Connection = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorSchema("public")]
                    case "codegeneratorschema":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.Schema = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorDatabaseType("public")]
                    case "codegeneratordatabasetype":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.DatabaseType = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorTablePrefix("public")]
                    case "codegeneratortableprefix":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.TablePrefix = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorIgnorePrefix("public")]
                    case "codegeneratorignoreprefix":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.IgnorePrefix = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorTargetFramwork("public")]
                    case "codegeneratortargetframwork":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.TargetFramwork = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorBaseNameSpace("public")]
                    case "codegeneratorbasenamespace":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.BaseNameSpace = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                    //[CodeGeneratorIsOverride(true)]
                    case "codegeneratorisoverride":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.IsOverride = bool.Parse(classAttribute.Value.Trim().TrimStart('"').TrimEnd('"'));
                        }
                        break;
                    //[CodeGeneratorBaseProjectPath("public")]
                    case "codegeneratorbaseprojectpath":
                        if (!string.IsNullOrEmpty(classAttribute.Value?.TrimStart('"').TrimEnd('"')))
                        {
                            generatorOptions.BaseProjectPath = classAttribute.Value.Trim().TrimStart('"').TrimEnd('"');
                        }
                        break;
                }
            }
            generatorOptions.Check();
            return generatorOptions;
        }
    }
}