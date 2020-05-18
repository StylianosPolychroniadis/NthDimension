namespace NthDimension.CodeGen.Project
{
    using System.ComponentModel;
    using System.IO;

    public sealed class WebReferencesProjectItem : FileProjectItem
    {
        public WebReferencesProjectItem(IProject project)
            : base(project, ItemType.WebReferences)
        {
        }

        [Browsable(false)]
        public string Directory
        {
            get
            {
                return Path.Combine(Project.Directory, Include).Trim('\\', '/');
            }
        }
    }
}
