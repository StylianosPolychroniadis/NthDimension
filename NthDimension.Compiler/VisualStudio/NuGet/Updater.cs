using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NthStudio.Compiler.VisualStudio.NuGet
{
    public static class Updater
    {
        public static string NormalizePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
        }

        public static void FindReferences(string searchPath, string searchPattern, IEnumerable<string> ignoredPaths, IList<PackageReference> references, IList<XDocument> documents)
        {
            Directory.EnumerateFiles(searchPath, searchPattern, SearchOption.AllDirectories).ToList().ForEach(
                fileName =>
                {
                    if (!ignoredPaths.Any(i => NormalizePath(fileName).Contains(NormalizePath(i))))
                    {
                        FindReferences(fileName, references, documents);
                    }
                });
        }

        public static void FindReferences(string fileName, IList<PackageReference> references, IList<XDocument> documents)
        {
            var document = XDocument.Load(fileName);
            documents.Add(document);
            foreach (var reference in document.Descendants().Where(x => x.Name.LocalName == "PackageReference"))
            {
                var name = reference.Attribute("Include").Value;
                var versionAttribute = reference.Attribute("Version");
                var version = versionAttribute != null ? versionAttribute.Value : reference.Elements().First(x => x.Name.LocalName == "Version").Value;
                var pr = new PackageReference()
                {
                    Name = name,
                    Version = version,
                    FileName = fileName,
                    Document = document,
                    Reference = reference,
                    VersionAttribute = versionAttribute
                };
                references.Add(pr);
            }
        }

        public static UpdaterResult FindReferences(this UpdaterResult updater, string searchPath, string searchPattern, IEnumerable<string> ignoredPaths)
        {
            FindReferences(searchPath, searchPattern, ignoredPaths, updater.References, updater.Documents);

            updater.GroupedReferences = updater.References
                .GroupBy(x => x.Name)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => (IList<PackageReference>)new ObservableCollection<PackageReference>(x));

            return updater;
        }

        public static UpdaterResult FindReferences(string searchPath, string searchPattern, IEnumerable<string> ignoredPaths)
        {
            return new UpdaterResult()
            {
                Documents = new ObservableCollection<XDocument>(),
                References = new ObservableCollection<PackageReference>()
            }
            .FindReferences(searchPath, searchPattern, ignoredPaths);
        }

        public static void PrintVersions(this UpdaterResult result)
        {
            Console.WriteLine("NuGet package dependencies versions:");
            foreach (var package in result.GroupedReferences)
            {
                Console.WriteLine(string.Format("Package {0} is installed:", package.Key));
                foreach (var v in package.Value)
                {
                    Console.WriteLine(string.Format("{0}, {1}", v.Version, v.FileName));
                }
            }
        }

        public static void ValidateVersions(this UpdaterResult result)
        {
            Console.WriteLine("Checking installed NuGet package dependencies versions:");
            foreach (var package in result.GroupedReferences)
            {
                var packageVersion = package.Value.First().Version;
                bool isValidVersion = package.Value.All(x => x.Version == packageVersion);
                if (!isValidVersion)
                {
                    Console.WriteLine(string.Format("Error: package {0} has multiple versions installed:", package.Key));
                    foreach (var v in package.Value)
                    {
                        Console.WriteLine(string.Format("{0}, {1}", v.Version, v.FileName));
                    }
                    throw new Exception("Detected multiple NuGet package version installed for different projects.");
                }
            };
            Console.WriteLine("All NuGet package dependencies versions are valid.");
        }

        public static void UpdateVersions(this UpdaterResult result, string name, string version)
        {
            Console.WriteLine("Updating NuGet package dependencies versions:");
            foreach (var v in result.GroupedReferences[name])
            {
                if (v.VersionAttribute != null)
                {
                    if (version != v.VersionAttribute.Value)
                    {
                        Console.WriteLine(string.Format("Name: {0}, old: {1}, new: {2}, file: {3}",name , v.VersionAttribute.Value, version, v.FileName));
                        v.VersionAttribute.Value = version;
                        v.Document.Save(v.FileName);
                    }
                }
                else
                {
                    var versionElement = v.Reference.Elements().First(x => x.Name.LocalName == "Version");
                    if (versionElement != null)
                    {
                        if (version != v.VersionAttribute.Value)
                        {
                            Console.WriteLine(string.Format("Name: {0}, old: {1}, new: {2}, file: {3}", name, versionElement.Value, version, v.FileName));
                            versionElement.Value = version;
                            v.Document.Save(v.FileName);
                        }
                    }
                }
            }
        }

        public static void UpdateVersions(KeyValuePair<string, IList<PackageReference>> package)
        {
            Console.WriteLine("Updating NuGet package dependencies versions:");
            foreach (var v in package.Value)
            {
                if (v.VersionAttribute != null)
                {
                    if (v.Version != v.VersionAttribute.Value)
                    {
                        Console.WriteLine(string.Format("Name: {0}, old: {1}, new: {2}, file: {3}", package.Key, v.VersionAttribute.Value, v.Version, v.FileName));
                        v.VersionAttribute.Value = v.Version;
                        v.Document.Save(v.FileName);
                    }
                }
                else
                {
                    var versionElement = v.Reference.Elements().First(x => x.Name.LocalName == "Version");
                    if (versionElement != null)
                    {
                        if (v.Version != v.VersionAttribute.Value)
                        {
                            Console.WriteLine(string.Format("Name: {0}, old: {1}, new: {2}, file: {3}", package.Key, versionElement.Value, v.Version, v.FileName));
                            versionElement.Value = v.Version;
                            v.Document.Save(v.FileName);
                        }
                    }
                }
            }
        }
    }
}
