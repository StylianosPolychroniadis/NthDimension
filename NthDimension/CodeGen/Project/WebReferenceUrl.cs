namespace NthDimension.CodeGen.Project
{
    using System.ComponentModel;
    using System.IO;

    public sealed class WebReferenceUrl : ProjectItem
    {
        [ReadOnly(true)]
        public string UpdateFromURL
        {
            get
            {
                return GetEvaluatedMetadata("UpdateFromURL");
            }
            set
            {
                SetEvaluatedMetadata("UpdateFromURL", value);
            }
        }

        [Browsable(false)]
        public string ServiceLocationURL
        {
            get
            {
                return GetEvaluatedMetadata("ServiceLocationURL");
            }
            set
            {
                SetEvaluatedMetadata("ServiceLocationURL", value);
            }
        }

        [Browsable(false)]
        public string CachedDynamicPropName
        {
            get
            {
                return GetEvaluatedMetadata("CachedDynamicPropName");
            }
            set
            {
                SetEvaluatedMetadata("CachedDynamicPropName", value);
            }
        }

        [Browsable(false)]
        public string CachedAppSettingsObjectName
        {
            get
            {
                return GetEvaluatedMetadata("CachedAppSettingsObjectName");
            }
            set
            {
                SetEvaluatedMetadata("CachedAppSettingsObjectName", value);
            }
        }

        [Browsable(false)]
        public string CachedSettingsPropName
        {
            get
            {
                return GetEvaluatedMetadata("CachedSettingsPropName");
            }
            set
            {
                SetEvaluatedMetadata("CachedSettingsPropName", value);
            }
        }

        [Browsable(false)]
        public string Namespace
        {
            get
            {
                string ns = GetEvaluatedMetadata("Namespace");
                if (ns.Length > 0)
                {
                    return ns;
                }
                return Project.RootNamespace;
            }
            set
            {
                SetEvaluatedMetadata("Namespace", value);
            }
        }

        [Browsable(false)]
        public string RelPath
        {
            get
            {
                return GetEvaluatedMetadata("RelPath");
            }
            set
            {
                SetEvaluatedMetadata("RelPath", value);
            }
        }

        [ReadOnly(true)]
        public string UrlBehavior
        {
            get
            {
                return GetEvaluatedMetadata("UrlBehavior");
            }
            set
            {
                SetEvaluatedMetadata("UrlBehavior", value);
            }
        }

        public override string FileName
        {
            get
            {
                if (Project != null && RelPath != null)
                {
                    return Path.Combine(Project.Directory, RelPath.Trim('\\'));
                }
                return null;
            }
            set
            {
                if (Project != null)
                {
                    RelPath = Utilities.FileUtility.GetRelativePath(Project.Directory, value);
                }
            }
        }

        public WebReferenceUrl(IProject project)
            : base(project, ItemType.WebReferenceUrl)
        {
            UrlBehavior = "Static";
        }
    }
}
