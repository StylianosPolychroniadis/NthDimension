namespace NthDimension.CodeGen.Project
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;

    public abstract class ProjectItem : IDisposable
    {
        IProject project;
        // or: (virtual mode)
        volatile string fileNameCache;
        string virtualInclude;
        ItemType virtualItemType;
        bool treatIncludeAsLiteral;
        Dictionary<string, string> virtualMetadata = new Dictionary<string, string>();

        protected ProjectItem(IProject project, ItemType itemType)
            : this(project, itemType, null)
        {
        }

        protected ProjectItem(IProject project, ItemType itemType, string include)
            : this(project, itemType, include, true)
        {
        }

        protected ProjectItem(IProject project, ItemType itemType, string include, bool treatIncludeAsLiteral)
        {
            this.project = project;
            this.virtualItemType = itemType;
            this.virtualInclude = include ?? "";
            this.virtualMetadata = new Dictionary<string, string>();
            this.treatIncludeAsLiteral = treatIncludeAsLiteral;
        }
        [Browsable(false)]
        public IProject Project
        {
            get
            {
                return project;
            }
        }

        [Browsable(false)]
        public bool TreatIncludeAsLiteral
        {
            get { return treatIncludeAsLiteral; }
            set { treatIncludeAsLiteral = value; }
        }

        /// <summary>
        /// Gets the object used for synchronization. This is project.SyncRoot for items inside a project; or
        /// virtualMetadata for items without project.
        /// </summary>
        object SyncRoot
        {
            get
            {
                return virtualMetadata;
            }
        }

        [Browsable(false)]
        public ItemType ItemType
        {
            get
            {
                return virtualItemType;
            }
            set
            {
                virtualItemType = value;
            }
        }

        [Browsable(false)]
        public string Include
        {
            get
            {
                return virtualInclude;
            }
            set
            {
                virtualInclude = value ?? "";
            }
        }

        #region Metadata access
        public bool HasMetadata(string metadataName)
        {
            lock (SyncRoot)
            {
                return virtualMetadata.ContainsKey(metadataName);
            }
        }

        /// <summary>
        /// Gets the evaluated value of the metadata item with the specified name.
        /// Returns an empty string for non-existing meta data items.
        /// </summary>
        public string GetEvaluatedMetadata(string metadataName)
        {
            lock (SyncRoot)
            {
                string val;
                virtualMetadata.TryGetValue(metadataName, out val);
                if (val == null)
                    return "";
                else
                    return Unescape(val);

            }
        }

        /// <summary>
        /// Gets the value of the metadata item with the specified name.
        /// Returns defaultValue for non-existing meta data items.
        /// </summary>
        public T GetEvaluatedMetadata<T>(string metadataName, T defaultValue)
        {
            return Utilities.GenericConverter.FromString(GetEvaluatedMetadata(metadataName), defaultValue);
        }

        /// <summary>
        /// Gets the escaped/unevaluated value of the metadata item with the specified name.
        /// Returns an empty string for non-existing meta data items.
        /// </summary>
        public string GetMetadata(string metadataName)
        {
            lock (SyncRoot)
            {
                string val;
                virtualMetadata.TryGetValue(metadataName, out val);
                return val ?? "";
            }
        }

        /// <summary>
        /// Sets the value of the specified meta data item. The value is escaped before
        /// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
        /// </summary>
        public void SetEvaluatedMetadata<T>(string metadataName, T value)
        {
            SetEvaluatedMetadata(metadataName, Utilities.GenericConverter.ToString(value));
        }

        /// <summary>
        /// Sets the value of the specified meta data item. The value is escaped before
        /// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
        /// Setting value to null or an empty string results in removing the metadata item.
        /// </summary>
        public void SetEvaluatedMetadata(string metadataName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                RemoveMetadata(metadataName);
            }
            else
            {
                lock (SyncRoot)
                {
                    virtualMetadata[metadataName] = Escape(value);
                }
            }
        }

        /// <summary>
        /// Sets the value of the specified meta data item.
        /// Setting value to null or an empty string results in removing the metadata item.
        /// </summary>
        public void SetMetadata(string metadataName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                RemoveMetadata(metadataName);
            }
            else
            {
                lock (SyncRoot)
                {
                    virtualMetadata[metadataName] = value;
                }
            }
        }

        /// <summary>
        /// Removes the specified meta data item.
        /// </summary>
        public void RemoveMetadata(string metadataName)
        {
            lock (SyncRoot)
            {
                virtualMetadata.Remove(metadataName);
            }
        }

        /// <summary>
        /// Gets the names of all existing meta data items on this project item. The resulting collection
        /// is a copy that will not be affected by future changes to the project item.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<string> MetadataNames
        {
            get
            {
                lock (SyncRoot)
                {
                    return ToArray(virtualMetadata.Keys);
                }
            }
        }

        private T[] ToArray<T>(IEnumerable<T> input)
        {
            if (input is ICollection<T>)
            {
                ICollection<T> c = (ICollection<T>)input;
                T[] arr = new T[c.Count];
                c.CopyTo(arr, 0);
                return arr;
            }
            else
            {
                return new List<T>(input).ToArray();
            }
        }
        // <summary>
        /// Escapes special MSBuild characters ( '%', '*', '?', '@', '$', '(', ')', ';', "'" ).
        /// </summary>
        private string Escape(string text)
        {
            return text; //Microsoft. MSBuild.Utilities.Escape(text);
        }
        private string Unescape(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            StringBuilder b = null;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '%' && i + 2 < text.Length)
                {
                    if (b == null) b = new StringBuilder(text, 0, i, text.Length);
                    string a = text[i + 1].ToString() + text[i + 2].ToString();
                    int num;
                    if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    {
                        b.Append((char)num);
                        i += 2;
                    }
                    else
                    {
                        b.Append('%');
                    }
                }
                else
                {
                    if (b != null)
                    {
                        b.Append(c);
                    }
                }
            }
            if (b != null)
                return b.ToString();
            else
                return text;
        }
        #endregion
        bool disposed;

        /// <summary>
        /// Copies all meta data from this item to the target item.
        /// </summary>
        public virtual void CopyMetadataTo(ProjectItem targetItem)
        {
            lock (SyncRoot)
            {
                lock (targetItem.SyncRoot)
                {
                    foreach (string name in this.MetadataNames)
                    {
                        targetItem.SetMetadata(name, this.GetMetadata(name));
                    }
                }
            }
        }

        /// <summary>
        /// Gets/Sets the full path of the file represented by "Include".
        /// For ProjectItems that are not assigned to any project, the getter returns the value of Include
        /// and the setter throws a NotSupportedException.
        /// </summary>
        [Browsable(false)]
        public virtual string FileName
        {
            get
            {
                if (project == null)
                {
                    return this.Include;
                }
                string fileName = this.fileNameCache;
                if (fileName == null)
                {
                    lock (SyncRoot)
                    {
                        fileName = Path.Combine(project.Directory, this.Include);
                        try
                        {
                            if (Path.IsPathRooted(fileName))
                            {
                                fileName = Path.GetFullPath(fileName);
                            }
                        }
                        catch { }
                        fileNameCache = fileName;
                    }
                }
                return fileName;
            }
            set
            {
                if (project == null)
                {
                    throw new NotSupportedException("Not supported for items without project.");
                }
                this.Include = Utilities.FileUtility.GetRelativePath(project.Directory, value);
            }
        }

        public virtual void Dispose()
        {
            disposed = true;
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return disposed; }
        }

        public override string ToString()
        {
            return String.Format("[{0}: <{1} Include='{2}'>]",
                                 GetType().Name, this.ItemType.ItemName, this.Include);
        }
    }
}
