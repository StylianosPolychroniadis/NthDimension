namespace NthStudio.Gui.Widgets.Source.Tree
{
    using System.IO;
    using NthDimension.Forms.Widgets;

    public abstract class FileBasedNode : TreeNode
    {
        protected FileBasedNode(string fileName)
        {
            FileName = Utilities.PathUtil.AdaptPathSeparator(fileName);
            PathFile = Path.GetDirectoryName(FileName) + Path.DirectorySeparatorChar;
            //ToolTipText = FileName;
        }

        public string FileName
        {
            get;
            private set;
        }
        public string PathFile
        {
            get;
            private set;
        }
    }
}
