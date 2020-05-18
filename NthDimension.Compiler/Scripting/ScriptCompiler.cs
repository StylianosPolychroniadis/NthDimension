namespace NthStudio.Compiler.Scripting
{
    using Microsoft.CSharp;
    using System.CodeDom.Compiler;

    internal class ScriptCompiler
    {
        private static ScriptCompiler           defaultCompiler;
        public static ScriptCompiler            DefaultCompiler
        {
            get
            {
                if (defaultCompiler == null)
                {
                    defaultCompiler = new ScriptCompiler();
                }

                return defaultCompiler;
            }
        }

        private CSharpCodeProvider              codeProvider;
        private ICodeCompiler                   codeCompiler;

        public ScriptCompiler()
        {
            codeProvider = new CSharpCodeProvider();
            codeCompiler = codeProvider.CreateCompiler();
        }

        public CompilerResults Compile(Script script, CompilerOutputDelegate cod)
        {
            CompilerParameters compileParams = new CompilerParameters();
            compileParams.GenerateExecutable = false;
            compileParams.GenerateInMemory = true;
            compileParams.IncludeDebugInformation = false;
            compileParams.TreatWarningsAsErrors = false;
            compileParams.ReferencedAssemblies.AddRange((string[])script.ReferencedAssemblies.ToStringArray());

            CompilerResults results = codeCompiler.CompileAssemblyFromSource(compileParams, script.TemplatedSource);

            if (results.Errors.HasErrors)
            {
                cod("-- Compilation of script failed");

                foreach (CompilerError err in results.Errors)
                {
                    cod(err.ToString());
                }
            }
            else
            {
                cod("-- Compilation of script succesfull");
            }


            return results;
        }
    }
}
