namespace NthDimension.CodeGen.Project
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;
    public class ReferenceAssembly : ProjectItem
    {
        protected ReferenceAssembly(IProject project, ItemType itemType)
            : base(project, itemType)
        {
        }

        public ReferenceAssembly(IProject project)
            : base(project, ItemType.Reference)
        {
        }

        public ReferenceAssembly(IProject project, string include)
            : base(project, ItemType.Reference, include)
        {
        }


        [Browsable(false)]
        public string HintPath
        {
            get
            {
                return GetEvaluatedMetadata("HintPath");
            }
            set
            {
                SetEvaluatedMetadata("HintPath", value);
            }
        }

        public string Aliases
        {
            get
            {
                return GetEvaluatedMetadata("Aliases", "global");
            }
            set
            {
                SetEvaluatedMetadata("Aliases", value);
            }
        }

        public bool SpecificVersion
        {
            get
            {
                return GetEvaluatedMetadata("SpecificVersion", false);
            }
            set
            {
                SetEvaluatedMetadata("SpecificVersion", value);
            }
        }

        public bool Private
        {
            get
            {
                return GetEvaluatedMetadata("Private", !IsGacReference);
            }
            set
            {
                SetEvaluatedMetadata("Private", value);
            }
        }

        [ReadOnly(true)]
        public string Name
        {
            get
            {
                AssemblyName assemblyName = GetAssemblyName(Include);
                if (assemblyName != null)
                {
                    return assemblyName.Name;
                }
                return Include;
            }
        }

        [ReadOnly(true)]
        public Version Version
        {
            get
            {
                AssemblyName assemblyName = GetAssemblyName(Include);
                if (assemblyName != null)
                {
                    return assemblyName.Version;
                }
                return null;
            }
        }

        [ReadOnly(true)]
        public string Culture
        {
            get
            {
                AssemblyName assemblyName = GetAssemblyName(Include);
                if (assemblyName != null && assemblyName.CultureInfo != null)
                {
                    return assemblyName.CultureInfo.Name;
                }
                return null;
            }
        }

        [ReadOnly(true)]
        public string PublicKeyToken
        {
            get
            {
                AssemblyName assemblyName = GetAssemblyName(Include);
                if (assemblyName != null)
                {
                    byte[] bytes = assemblyName.GetPublicKeyToken();
                    if (bytes != null)
                    {
                        StringBuilder token = new StringBuilder();
                        foreach (byte b in bytes)
                        {
                            token.Append(b.ToString("x2"));
                        }
                        return token.ToString();
                    }
                }
                return null;
            }
        }

        [ReadOnly(true)]
        public override string FileName
        {
            get
            {
                if (Project != null)
                {
                    string projectDir = Project.Directory;
                    string hintPath = HintPath;
                    try
                    {
                        if (hintPath != null && hintPath.Length > 0)
                        {
                            return Utilities.FileUtility.GetAbsolutePath(projectDir, hintPath);
                        }
                        string name = Utilities.FileUtility.GetAbsolutePath(projectDir, Include);
                        if (File.Exists(name))
                        {
                            return name;
                        }
                        if (File.Exists(name + ".dll"))
                        {
                            return name + ".dll";
                        }
                        if (File.Exists(name + ".exe"))
                        {
                            return name + ".exe";
                        }
                    }
                    catch { } // ignore errors when path is invalid
                }
                return Include;
            }
            set
            {
                // Set by file name is unsupported by references. (otherwise GAC references might have strange renaming effects ...)
            }
        }

        [Browsable(false)]
        public bool IsGacReference
        {
            get
            {
                return !Path.IsPathRooted(this.FileName);
            }
        }

        AssemblyName GetAssemblyName(string include)
        {
            try
            {
                if (this.ItemType == ItemType.Reference)
                {
                    return new AssemblyName(include);
                }
            }
            catch (ArgumentException) { }

            return null;
        }

    }
}
