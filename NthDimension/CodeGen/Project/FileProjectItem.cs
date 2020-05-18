namespace NthDimension.CodeGen.Project
{
    using System.ComponentModel;
    using System.IO;

    public enum CopyToOutputDirectory
    {
        Never,
        Always,
        PreserveNewest
    }

    public class FileProjectItem : ProjectItem
    {
        /// <summary>
        /// Creates a new FileProjectItem with the specified include.
        /// </summary>
        public FileProjectItem(IProject project, ItemType itemType, string include)
            : base(project, itemType, include)
        {
        }

        /// <summary>
        /// Creates a new FileProjectItem including a dummy file.
        /// </summary>
        public FileProjectItem(IProject project, ItemType itemType)
            : base(project, itemType)
        {
        }


        //[Editor(typeof(BuildActionEditor), typeof(UITypeEditor))]
        public string BuildAction
        {
            get
            {
                return this.ItemType.ItemName;
            }
            set
            {
                this.ItemType = new ItemType(value);
            }
        }

        public CopyToOutputDirectory CopyToOutputDirectory
        {
            get
            {
                return GetEvaluatedMetadata("CopyToOutputDirectory", CopyToOutputDirectory.Never);
            }
            set
            {
                SetEvaluatedMetadata("CopyToOutputDirectory", value);
            }
        }

        public string CustomTool
        {
            get
            {
                return GetEvaluatedMetadata("Generator");
            }
            set
            {
                SetEvaluatedMetadata("Generator", value);
            }
        }



        public string CustomToolNamespace
        {
            get
            {
                return GetEvaluatedMetadata("CustomToolNamespace");
            }
            set
            {
                SetEvaluatedMetadata("CustomToolNamespace", value);
            }
        }

        [Browsable(false)]
        public string DependentUpon
        {
            get
            {
                return GetEvaluatedMetadata("DependentUpon");
            }
            set
            {
                SetEvaluatedMetadata("DependentUpon", value);
            }
        }

        [Browsable(false)]
        public string SubType
        {
            get
            {
                return GetEvaluatedMetadata("SubType");
            }
            set
            {
                SetEvaluatedMetadata("SubType", value);
            }
        }

        [Browsable(false)]
        public bool IsLink
        {
            get
            {
                return HasMetadata("Link") || !Utilities.FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName);
            }
        }

        [Browsable(false)]
        /// <summary>
        /// Gets the name of the file in the virtual project file system.
        /// This is normally the same as Include, except for linked files, where it is
        /// the value of Properties["Link"].
        /// </summary>
        public string VirtualName
        {
            get
            {
                if (HasMetadata("Link"))
                    return GetEvaluatedMetadata("Link");
                else if (Utilities.FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName))
                    return this.Include;
                else
                    return Path.GetFileName(this.Include);
            }
        }
    }
}
