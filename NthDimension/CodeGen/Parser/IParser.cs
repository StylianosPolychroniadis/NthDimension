﻿namespace NthDimension.CodeGen.Parser
{
    using Dom;
    using Project;

    public interface IParser
    {
        string[] LexerTags
        {
            get;
            set;
        }

        LanguageProperties Language
        {
            get;
        }

        IExpressionFinder CreateExpressionFinder(string fileName);

        /// <summary>
        /// Gets if the parser can parse the specified file.
        /// This method is used to get the correct parser for a specific file and normally decides based on the file
        /// extension.
        /// </summary>
        bool CanParse(string fileName);

        /// <summary>
        /// Gets if the parser can parse the specified project.
        /// Only when no parser for a project is found, the assembly is loaded.
        /// </summary>
        bool CanParse(IProject project);

        ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent);

        IResolver CreateResolver();

        NRefactory.Parser.Errors LastErrors
        {
            get;
        }
    }
}
