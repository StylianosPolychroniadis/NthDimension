namespace NthStudio.Compiler.VisualStudio.Project
{
    public enum Verbosity
    {
        Low, Medium, High
    }

    //[CliParse.ParsableClass("AnalyserParams")]
    public struct AnalyserParams //: CliParse.Parsable
    {
        //[CliParse.ParsableArgument("Path", ShortName = 'p', DefaultValue = "", Description = "The path to scan", ImpliedPosition = 1, Required = false)]
        public string Path { get; set; }
        //[CliParse.ParsableArgument("Assembly", ShortName = 'a', DefaultValue = "", Description = "The assembly to Analyse", ImpliedPosition = 2, Required = false)]
        public string AssemblyToAnalyse { get; set; }

        //[CliParse.ParsableArgument("Extensions", ShortName = 'e', DefaultValue = "*.csproj", Description = "A comma seperated list of project types to analyse; csproj support only now.")]
        public string Extensions { get; set; }
        //[CliParse.ParsableArgument("Verbosity", ShortName = 'v', Description = "How much information you want")]
        public Verbosity Verbosity { get; set; }
        //[CliParse.ParsableArgument("Summary", ShortName = 's', Description = "Include summary")]
        public bool Summary { get; set; }
        //[CliParse.ParsableArgument("Recursive", ShortName = 'r', Description = "Recursively scan sub directories", DefaultValue = false)]
        public bool Recursive { get; set; }
        //[CliParse.ParsableArgument("RecurseDependencies", ShortName = 'd', Description = "Recursively list a projects dependencies.", DefaultValue = true)]
        public bool RecurseDependencies { get; set; }
        //[CliParse.ParsableArgument("MaxDepth", ShortName = 'm', Description = "Maximum depth to recursively list a projects dependencies.", DefaultValue = 8)]
        public int RecurseDependenciesMaxDepth { get; set; }
        //[CliParse.ParsableArgument("IncludeSystemDependencies", ShortName = 's', Description = "Include System.* dependencies", DefaultValue = false)]
        public bool IncludeSystemDependencies { get; set; }


        public bool ShouldIncludeReference(string referenceName)
        {
            if ((referenceName.Equals("System") || referenceName.StartsWith("System.")) && IncludeSystemDependencies == false) return false;
            if ((referenceName.Equals("Microsoft") || referenceName.StartsWith("Microsoft.")) && IncludeSystemDependencies == false) return false;

            return true;
        }
    }
}
