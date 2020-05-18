/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

namespace NthDimension.Rendering.Loaders
{
    using System;
    using System.Collections.Generic;

    using NthDimension.Rendering.Utilities;
    using NthDimension.Utilities;

    public class FileSeeker : ApplicationObject
    {
        private string root = string.Empty;
        List<string> files = new List<string>();
        //public FileSeeker()
        //{
            
        //}
        public FileSeeker(string rootPath = "")//:this()
        {
            if (string.IsNullOrEmpty(rootPath))
                root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                root = rootPath;

            files = getFileList(root, true);
        }

        public void LoadAllFiles()
        {
            ConsoleUtil.log(string.Format(">> Seeking files in path {0}", root));
            foreach (var file in files)
            {
                string test = string.Empty;

                if (file.Contains(DirectoryUtil.Documents))
                    test = file;

                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    case ".ani":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found animation sequences file: " + relativePath);
                        ApplicationBase.Instance.AnimationLoader.FromAni(relativePath);
#endif
                        break;
                    case ".obj":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found object file: " + relativePath);
                        ApplicationBase.Instance.MeshLoader.FromObj(relativePath);
#endif
                        break;
                    case ".dae":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found object file: " + relativePath);
                        ApplicationBase.Instance.MeshLoader.FromCollada(relativePath);
#endif
                        break;
                    case ".xmd":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found object definition file: " + relativePath);
                        ApplicationBase.Instance.MeshLoader.FromXmd(relativePath);
#endif
                        break;
                    case ".png":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found image file: " + relativePath);
                        ApplicationBase.Instance.TextureLoader.fromPng(relativePath);
#endif
                        break;
                    case ".dds":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found image file: " + relativePath);
                        ApplicationBase.Instance.TextureLoader.fromDds(relativePath);
#endif
                        break;
                    case ".xmf":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found material file: " + relativePath);
                        ApplicationBase.Instance.MaterialLoader.FromXmlFile(relativePath);
#endif
                        break;
                    case ".xsp":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found shaderpair file: " + relativePath);
                        ApplicationBase.Instance.ShaderLoader.FromXmlFile(relativePath);
#endif
                        break;
                    case ".snip":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found shader snipet: " + relativePath);
                        ApplicationBase.Instance.ShaderLoader.LoadSnippet(relativePath);
#endif
                        break;
                    case ".xtmp":
#if !_DEVUI_
                        Utilities.ConsoleUtil.log("found template file: " + relativePath);
                        ApplicationBase.Instance.TemplateLoader.fromXmlFile(relativePath);
#endif
                        break;
                    default:
                        break;
                }
            }
            ConsoleUtil.log(string.Format("Done! Found {0} files total", files.Count));
        }

        public void LoadAllTextures()
        {
            ConsoleUtil.log(string.Format(">> Seeking files in path {0}", root));
            foreach (var file in files)
            {
                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    
                    case ".png":

                        Utilities.ConsoleUtil.log("found image file: " + relativePath);
                        ApplicationBase.Instance.TextureLoader.fromPng(relativePath);
                        break;
                    case ".dds":

                        Utilities.ConsoleUtil.log("found image file: " + relativePath);
                        ApplicationBase.Instance.TextureLoader.fromDds(relativePath);
                        break;
                    case ".xmf":

                        Utilities.ConsoleUtil.log("found material file: " + relativePath);
                        ApplicationBase.Instance.MaterialLoader.FromXmlFile(relativePath);
                        break;
                    default:
                        break;
                }
            }
            ConsoleUtil.log(string.Format("Done! Found {0} files total", files.Count));
        }

        public void LoadAllMaterials()
        {
            foreach (var file in files)
            {
                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    case ".xmf":

                        Utilities.ConsoleUtil.log("found material file: " + relativePath);
                        ApplicationBase.Instance.MaterialLoader.FromXmlFile(relativePath);
                        break;

                    default:
                        break;
                }
            }
        }

        public void LoadAllMeshes()
        {
            foreach (var file in files)
            {
                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    case ".obj":

                        Utilities.ConsoleUtil.log("found object file: " + relativePath);
                        ApplicationBase.Instance.MeshLoader.FromObj(relativePath);
                        break;
                    //case ".dae":
                    //    Utilities.ConsoleUtil.log("found object file: " + relativePath);
                    //    Game.Instance.MeshLoader.FromCollada(relativePath);
                    //    break;
                    case ".xmd":

                        Utilities.ConsoleUtil.log("found object definition file: " + relativePath);
                        ApplicationBase.Instance.MeshLoader.FromXmd(relativePath);
                        break;
                    default:
                        break;
                }
            }
        }
        public void LoadAllShaders()
        {
            foreach (var file in files)
            {
                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    
                    case ".xsp":

                        Utilities.ConsoleUtil.log("found shaderpair file: " + relativePath);
                        ApplicationBase.Instance.ShaderLoader.FromXmlFile(relativePath);
                        break;
                    case ".snip":
                        Utilities.ConsoleUtil.log("found shader snipet: " + relativePath);
                        ApplicationBase.Instance.ShaderLoader.LoadSnippet(relativePath);
                        break;
                    default:
                        break;
                }
            }
        }

        public void LoadAllTemplates()
        {
            foreach (var file in files)
            {
                string relativePath = file.Substring(root.Length + 1);
                string extention = System.IO.Path.GetExtension(file);

                switch (extention)
                {
                    case ".xtmp":
                        Utilities.ConsoleUtil.log("found template file: " + relativePath);
                        ApplicationBase.Instance.TemplateLoader.fromXmlFile(relativePath);
                        break;
                    default:
                        break;
                }
            }
        }

        private List<string> getFileList(string root, bool subFolders)
        {
            List<string> fileArray = new List<string>();

            try
            {
                string[] files = System.IO.Directory.GetFiles(root);
                string[] folders = System.IO.Directory.GetDirectories(root);

                for (int i = 0; i < files.Length; i++)
                    fileArray.Add(files[i].ToString());


                if (subFolders == true)
                    for (int i = 0; i < folders.Length; i++)
                        fileArray.AddRange(getFileList(folders[i], subFolders));
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
            return fileArray;
        }

        
       
    }
}
