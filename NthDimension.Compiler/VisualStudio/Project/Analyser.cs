

namespace NthStudio.Compiler.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class Analyser
    {
        public Dictionary<string, ProjectReferenceMap>      Projects
        {
            get { return csharpProjects;  }
        }

        private Dictionary<string, ProjectReferenceMap>     csharpProjects;              
        private string[]                                    refenceTypes                = new string[2]
                                                                                        {
                                                                                                "Reference",
                                                                                                "ProjectReference"
                                                                                        };

        #region Process
        public void Process(AnalyserParams analyserParams, StreamWriter streamWriter)
        {
            _sw = streamWriter;

            var filePaths = Enumerable.ToList<string>(FindFilesByExtension(analyserParams.Path)); // grab all csproj files


            csharpProjects = CSharpProjectsFromFiles(filePaths);


            // Gather all referenced items from all cs project files
            foreach (var project in csharpProjects.Values)
            {
                foreach (var referenceEntry in GetReferencesEntriesFromCSProj(project.File))
                {
                    if (!analyserParams.ShouldIncludeReference(referenceEntry.Name)) continue;  // skip items we want to ignore.

                    project.Uses.Add(referenceEntry);
                }
            }

            // update usedby
            foreach (var project in csharpProjects.Values)
            {
                foreach (var usedReferences in project.Uses)
                {
                    if (csharpProjects.ContainsKey(usedReferences.Name))
                    {
                        csharpProjects[usedReferences.Name].UsedBy.Add(project);
                    }
                }
            }


            Dictionary<string, ProjectReferenceMap> items = csharpProjects;
            if (analyserParams.AssemblyToAnalyse.Length > 1)
            {
                if (csharpProjects.ContainsKey(analyserParams.AssemblyToAnalyse) == false)
                {
                    _sw.WriteLine("The provided assembly value {0} was not found during scan, please check the assembly name is correct.", analyserParams.AssemblyToAnalyse);
                    return;
                }

                items = new Dictionary<string, ProjectReferenceMap>();
                items.Add(analyserParams.AssemblyToAnalyse, csharpProjects[analyserParams.AssemblyToAnalyse]);
            }

            foreach (var projectReferenceMap in items.Values)
            {
                PrintReferenceMap(projectReferenceMap, analyserParams);
            }

            if (analyserParams.Summary)
            {
                PrintSummary(analyserParams);
            }
        }

        private Dictionary<string, ProjectReferenceMap> CSharpProjectsFromFiles(IEnumerable<string> filePaths)
        {
            Dictionary<string, ProjectReferenceMap> csProjDictionary = new Dictionary<string, ProjectReferenceMap>();
            foreach (var filePath in filePaths)
            {
                var fi = new FileInfo(filePath);
                var name = fi.Name;
                var assembly = GetAssemblyNameFromCSProj(filePath);

                if (string.IsNullOrEmpty(assembly)) continue;

                var project = new ProjectReferenceMap
                {
                    Assembly = assembly,
                    File = filePath,
                    Name = name,
                    UsedBy = new List<ProjectReferenceMap>(),
                    Uses = new List<ReferenceEntry>()
                };

                if (!csProjDictionary.ContainsKey(assembly))
                {
                    csProjDictionary.Add(assembly, project);
                }
            }



            return csProjDictionary;
        }

        private IEnumerable<string> FindFilesByExtension(string path, string extension = "*.csproj", bool recursive = true)
        {
            //Collect ProjectReferenceMap Files and build list
            foreach (string file in Directory.EnumerateFiles(path, extension, SearchOption.AllDirectories))
            {
                if (file.Contains("_old")) continue;

                yield return file;
            }
        }

        private string GetAssemblyNameFromCSProj(string file)
        {
            try
            {
                XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
                XDocument projDefinition = XDocument.Load(file);
                //            string[] references = projDefinition
                //                .Element(msbuild + "ProjectReferenceMap")
                //                .Elements(msbuild + "ItemGroup")
                //                .Elements(msbuild + "Reference")
                //                .Elements(msbuild + "HintPath")
                //                .Select(refElem => refElem.Value).ToArray();
                //
                //            Array.Sort(references);
                //            foreach (string reference in references)
                //            {
                //                Console.WriteLine(reference);
                //            }


                return (
                    projDefinition.Element(msbuild + "Project")
                        .Element(msbuild + "PropertyGroup")
                        .Element(msbuild + "AssemblyName")
                        .Value);
            }
            catch (Exception e)
            {
                _sw.WriteLine(file + " caused error " + e.ToString());
                return string.Empty;
            }
        }

        private IEnumerable<ReferenceEntry> GetReferencesEntriesFromCSProj(string file)
        {
            //            XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
            //            XDocument projDefinition = XDocument.Load(file);
            //            List<ReferenceEntry> references = projDefinition
            //                .Element(msbuild + "Project")
            //                .Elements(msbuild + "ItemGroup")
            //                .Elements(msbuild + "Reference")
            //                //.Elements(msbuild + "HintPath")
            //                .Select<XElement, ReferenceEntry>(ParseReferenceEntry).ToList();

            string schema = "http://schemas.microsoft.com/developer/msbuild/2003";
            XNamespace msbuild = schema;
            XDocument projDefinition = XDocument.Load(file);
            List<ReferenceEntry> references = new List<ReferenceEntry>();

            for (int s = 0; s < refenceTypes.Length; s++)
                references.AddRange(projDefinition.Descendants(XName.Get(refenceTypes[s], schema))
                .Select<XElement, ReferenceEntry>(ParseReferenceEntry).ToList());

            return references;
        }

        private ReferenceEntry ParseReferenceEntry(XElement refElem)
        {
            XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
            /*
             <Reference Include="LTCExistingCoverageImport, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL">
              <VersionSpecified>False</VersionSpecified>
              <HintPath>..\..\Assemblies\BuildOutput\LTCExistingCoverageImport.dll</HintPath>
            </Reference>
             */
            var r = new ReferenceEntry();
            r.HintPath = refElem.Element(msbuild + "HintPath") == null ? "" : refElem.Element(msbuild + "HintPath").Value;

            var specVal = refElem.Element(msbuild + "VersionSpecified") == null ? "False" : refElem.Element(msbuild + "VersionSpecified").Value;
            r.VersionSpecified = specVal.ToLower().Equals("true");
            r.Include = refElem.Attribute("Include") == null ? "" : refElem.Attribute("Include").Value;

            r.Name = r.Include;
            //Include path includes some additional junk - strip 
            if (r.Include.IndexOf(',') > 0)
            {
                r.Name = r.Include.Split(',')[0];
                r.Version = r.Include.Split(',')[1].Split('=')[1];   // McHack happy meal
            }

            return r;
        }
        #endregion

        #region Print
        private StreamWriter                                _sw;
        private void PrintSummary(AnalyserParams analyserParams)
        {
            _sw.WriteLine("\n\n--------------------SUMMARY----------------------");

            _sw.WriteLine("Distinct assemblies found:" + csharpProjects.Keys.Count);

            _sw.WriteLine("Not referenced (Top level?)\n");

            var topLevel = from project in csharpProjects.Values
                           where project.UsedBy.Count == 0
                           orderby project.Name
                           select project;

            foreach (var project in topLevel)
            {
                _sw.WriteLine("    {0} - {1}", project.Name, project.File.Replace(analyserParams.Path, ""));
            }


            _sw.WriteLine("---------------------------\r\n\r\nTop 20 most referenced ()");
            var most = from project in csharpProjects.Values
                       where project.UsedBy.Count > 0
                       orderby project.UsedBy.Count descending
                       select project;

            foreach (var project in most.Take(20))
            {
                _sw.WriteLine("    {0} - {1}", project.Name, project.UsedBy.Count);
            }
        }

        private void PrintReferenceMap(ProjectReferenceMap projectReferenceMap, AnalyserParams analyserParams)
        {
            _sw.WriteLine("-------------------{0}--------------", projectReferenceMap.Name);
            _sw.WriteLine("References (" + projectReferenceMap.Uses.Count + "):");
            foreach (var usage in projectReferenceMap.Uses.OrderBy(t => t.Name))
            {
                PrintReferenceEntry(usage, analyserParams, "-->", 0);
            }

            _sw.WriteLine("Referenced by (" + projectReferenceMap.UsedBy.Count + "):");
            foreach (var usage in projectReferenceMap.UsedBy.OrderBy(t => t.Name))
            {
                _sw.WriteLine("  " + usage.Name);
            }
            _sw.WriteLine();
        }

        private void PrintReferenceEntry(ReferenceEntry usage, AnalyserParams analyserParams, string indent, int currentDepth)
        {
            var s = usage.Name;
            if (analyserParams.Verbosity >= Verbosity.Medium)
            {
                if (usage.VersionSpecified) s += "\n    " + "    VersionSpecific";
                if (!string.IsNullOrEmpty(usage.Version)) s += "\n    " + "    Version:" + usage.Version;

                if (analyserParams.Verbosity == Verbosity.High)
                    if (usage.HintPath.Length > 0) s += "\n    " + "    HintPath:" + usage.HintPath;
            }
            _sw.WriteLine("{0} {1}", indent, s);

            //recurse through entries
            if (analyserParams.RecurseDependencies && analyserParams.AssemblyToAnalyse.Length > 0 && csharpProjects.ContainsKey(usage.Name))
            {
                if (currentDepth > analyserParams.RecurseDependenciesMaxDepth)
                {
                    _sw.WriteLine(indent + "RECURSTION DEPTH EXCEEDED - potential circular references");
                    return;
                }

                //_sw.AppendLine(indent + "which references:");
                var project = csharpProjects[usage.Name];
                foreach (var dependency in project.Uses.OrderBy(t => t.Name))
                {
                    PrintReferenceEntry(dependency, analyserParams, indent + "-->", currentDepth + 1);
                }
            }
        }
        #endregion
    }
}
