namespace NthDimension.CodeGen.Project
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;


    public interface IProject
    {
        /// <summary>
        /// Gets the list of items in the project. This member is thread-safe.
        /// The returned collection is guaranteed not to change - adding new items or removing existing items
        /// will create a new collection.
        /// </summary>
        ReadOnlyCollection<ProjectItem> Items
        {
            get;
        }

        /// <summary>
        /// Adds a new entry to the Items-collection
        /// </summary>
        void AddProjectItem(ProjectItem item);

        /// <summary>
        /// Removes an entry from the Items-collection
        /// </summary>
        bool RemoveProjectItem(ProjectItem item);

        /// <summary>
        /// Gets all items in the project that have the specified item type.
        /// This member is thread-safe.
        /// </summary>
        IEnumerable<ProjectItem> GetItemsOfType(ItemType type);

        /// <summary>
        /// Gets the language properties used for this project.
        /// </summary>
        Dom.LanguageProperties LanguageProperties
        {
            get;
        }

        /// <summary>
        /// Gets the ambience used for the project. This member is thread-safe.
        /// </summary>
        Dom.IAmbience Ambience
        {
            get;
        }

        /// <summary>
        /// Gets the directory of the project file.
        /// 
        /// This member is thread-safe.
        /// </summary>
        string Directory
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the assembly name of the assembly created when building this project.
        /// Equivalent to MSBuild property "AssemblyName".
        /// </summary>
        string AssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the root namespace of the project.
        /// </summary>
        string RootNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the full path of the output assembly.
        /// Returns null when the project does not output any assembly.
        /// </summary>
        string OutputAssemblyFullPath
        {
            get;
        }

        /// <summary>
        /// Gets the name of the project file.
        /// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
        /// 
        /// Only the getter is thread-safe.
        /// </summary>
        string FileName
        {
            get;
            set;
        }

    }
}
