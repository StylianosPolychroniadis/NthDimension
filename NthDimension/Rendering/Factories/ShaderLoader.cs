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

#define _WINDOWS_

using NthDimension.Rendering.Serialization;
using System.ComponentModel;
using NthDimension.Rendering.Shaders;
using Shader = NthDimension.Rendering.Shaders.Shader;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;
using System.Xml;
using NthDimension.Rasterizer;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Utilities;

#if _WINDOWS_
#endif

namespace NthDimension.Rendering.Loaders
{
    public class ShaderLoader : ApplicationObject
    {
        public class MyTaskProgressChangedEventArgs : ProgressChangedEventArgs
        {
            private string _currentFile;

            public string CurrentFile
            {
                get { return _currentFile; }
            }

            public MyTaskProgressChangedEventArgs(int progressPercentage, string currentFile,
                object userState)
                : base(progressPercentage, userState)
            {
                _currentFile = currentFile;
            }
        }

#if _WINDOWS_ // TODO:: UnBind
        public List<Shaders.Shader> shaders = new List<Shaders.Shader> { };
#endif
        public List<Snippet> snippets = new List<Snippet> { };

        private int vertexShaderHandle;
        private int fragmentShaderHandle;
        public Hashtable shaderNames = new Hashtable();

        public Hashtable Entries
        {
            get { return shaderNames; }
        }

        public const int maxNoLights = 10;
        private int maxNoBones = 64;

        const string varMarker = "#variables";
        const string codeMarker = "#code";
        const string includeMarker = "#include";
        const string functionsMarker = "#functions";

        enum Target
        {
            code,
            variable,
            function
        };

        public ShaderLoader()
        {

        }

        public Shaders.Shader GetShaderByName(string name)
        {
            if (name == null)
                return new Shaders.Shader();

            int id = -1;

            try
            {
                id = (int) shaderNames[name];
            }
            catch //(Exception se)
            {
                throw new Exception(string.Format("Shader Loader: Shader not found {0}", name));
            }

            return shaders[id];
        }

        #region Cache read/write

        public void ReadCacheFile(string filename = "", Action<int> callback = null)
        {
            if (filename == string.Empty)
                filename = Settings.Instance.game.shaderCacheFile;

            if (!File.Exists(filename))
                ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(filename)));
            else
            {
                FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                ShaderCacheObject cacheObject;

                using (fileStream)
                {
                    // Read the source file into a byte array.
                    byte[] bytes = new byte[fileStream.Length];
                    int numBytesToRead = (int)fileStream.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;

                        if (null != callback)
                            callback((int)(100 * numBytesRead / fileStream.Length));
                    }



                    cacheObject = (ShaderCacheObject)GenericMethods.ByteArrayToObject<ShaderCacheObject>(bytes);
                    fileStream.Close();
                }

                if (null != cacheObject.shaders)
                {
                    foreach (var shader in cacheObject.shaders)
                    {
                        Shaders.Shader curShader = shader;

                        string name = shader.Name;

                        if (!shaderNames.ContainsKey(shader.Name))
                        {
                            int identifier = shaders.Count;

                            curShader.type = enuShaderType.fromCache;
                            curShader.Identifier = identifier;

                            shaders.Add(curShader);
                            shaderNames.Add(name, identifier);
                        }
                    }
                    foreach (var newSnippet in cacheObject.snippets)
                    {
                        loadSnippetFromCache(newSnippet);
                    }
                }
             
                Utilities.ConsoleUtil.log("\tAdded " + cacheObject.shaders.Count + " shaders from cache");
                Utilities.ConsoleUtil.log("\tAdded " + cacheObject.snippets.Count + " shader-snippets from cache");
            }

        }

        public void WriteCacheFile(string directory = "", Action<string> callback = null)
        {
            if (string.Empty == directory)
                directory = Settings.Instance.game.shaderCacheFile;
            else
                directory = Path.Combine(directory, Settings.Instance.game.shaderCacheFile);

            ShaderCacheObject cacheObject = new ShaderCacheObject();
            cacheObject.shaders = new ListShader { };
            cacheObject.snippets = new ListSnippet { };

            foreach (var shader in shaders)
            {
                if (null != callback)
                    callback(string.Format("caching shader {0}", shader.Name));

                shader.Cache(ref cacheObject);


            }
            foreach (var snippet in snippets)
            {
                if (null != callback)
                    callback(string.Format("caching snippet {0}", snippet.name));

                snippet.cache(ref cacheObject);
            }



            FileStream fileStream = new FileStream(directory, FileMode.Create, FileAccess.Write);

            using (fileStream)
            {
                byte[] saveAry = GenericMethods.ObjectToByteArray<ShaderCacheObject>(cacheObject);
                fileStream.Write(saveAry, 0, saveAry.Length);
                fileStream.Close();
            }
        }

        private void loadShaderFromCache(Shaders.Shader target)
        {
            Utilities.ConsoleUtil.log(string.Format("<> Loading shader {0}{1}", /*NthDimension.Utilities.ConsoleEffects.Foreground_Cyan +*/
                                                                                   target.Name //+
                                                                                /*NthDimension.Utilities.ConsoleEffects.Foreground_Default*/,
                                                                               Environment.NewLine));

            ConsoleUtil.log(string.Format("   Compiling...", target.Name, Environment.NewLine));

            int shaderProgramHandle;
            string log = string.Empty;

            #region Create Shader
            vertexShaderHandle = ApplicationBase.Instance.Renderer.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = ApplicationBase.Instance.Renderer.CreateShader(ShaderType.FragmentShader);
            #endregion Create Shader

            #region Shader Source
            ApplicationBase.Instance.Renderer.ShaderSource(vertexShaderHandle, target.VertexShader);
            ApplicationBase.Instance.Renderer.ShaderSource(fragmentShaderHandle, target.FragmentShader);
            #endregion Shader Source

            #region Compile Vertex Shader
            ApplicationBase.Instance.Renderer.CompileShader(vertexShaderHandle);
            int length = 0;
            ApplicationBase.Instance.Renderer.GetShader(vertexShaderHandle, ShaderParameter.InfoLogLength, out length);

            if (length > 0)
            {
                length = 0;
                log = string.Empty;
                log = ApplicationBase.Instance.Renderer.GetShaderInfoLog(vertexShaderHandle);
                parseLog(log, "Vertex: ", target.VertexShader);
            }
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Load Vertex Shader {0}:{1}", this.GetType(),
                Name));
            #endregion

            #region Compile Fragment Shader
            ApplicationBase.Instance.Renderer.CompileShader(fragmentShaderHandle);
            ApplicationBase.Instance.Renderer.GetShader(fragmentShaderHandle, ShaderParameter.InfoLogLength, out length);

            if (length > 0)
            {
                log = string.Empty;
                log = ApplicationBase.Instance.Renderer.GetShaderInfoLog(fragmentShaderHandle);
                parseLog(log, "Fragment: ", target.FragmentShader);
            }
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Load Fragment Shader {0}:{1}", this.GetType(),
                Name));
            #endregion

            #region Create Program
            shaderProgramHandle = ApplicationBase.Instance.Renderer.CreateProgram();
            #endregion Create Program

            #region Attach Shader
            ApplicationBase.Instance.Renderer.AttachShader(shaderProgramHandle, vertexShaderHandle);
            ApplicationBase.Instance.Renderer.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            #endregion Attach Shader

            #region Link Program
            ApplicationBase.Instance.Renderer.LinkProgram(shaderProgramHandle);
            #endregion Link Program

            #region Check Link Status
            int linkStatus = -1;
            ApplicationBase.Instance.Renderer.GetProgram(shaderProgramHandle, GetProgramParameterName.LinkStatus, out linkStatus);
            ConsoleUtil.log(string.Format("\tLink Status     : {0}", linkStatus));
            #endregion Check Link Status

            #region Validate Program
            ApplicationBase.Instance.Renderer.ValidateProgram(shaderProgramHandle);
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.GetProgram(shaderProgramHandle, GetProgramParameterName.ValidateStatus, out validateStatus);
            ConsoleUtil.log(string.Format("\tValidate Status : {0}", validateStatus));
            #endregion Validate Program

            #region Check Program InfoLog
            log = string.Empty;
            log = ApplicationBase.Instance.Renderer.GetProgramInfoLog(shaderProgramHandle);
            log = log.Replace("\n", " ");
            ConsoleUtil.log(string.Format("\tProgram: {0}", log));
            #endregion Check Program Info Log

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Load Shader by Type {0}:{1}", this.GetType(),
                Name));

            // Detach whether success or failure
            // https://www.khronos.org/opengl/wiki/Shader_Compilation
            ApplicationBase.Instance.Renderer.DetachShader(shaderProgramHandle, vertexShaderHandle);
            ApplicationBase.Instance.Renderer.DetachShader(shaderProgramHandle, fragmentShaderHandle);

            target.Handle = shaderProgramHandle;

            setHandles(ref target);

            target.Loaded = true;

            shaders[target.Identifier] = target;

            ConsoleUtil.log(string.Format("   Compilation Finished!{1}{1}", target.Name, Environment.NewLine));
        }

        private void loadSnippetFromCache(Snippet newSnippet)
        {
            foreach (var snippet in snippets)
            {
                if (snippet.name == newSnippet.name)
                    return;
            }

            snippets.Add(newSnippet);
        }

        private void setHandles(ref Shaders.Shader target)
        {
            int shaderProgramHandle = target.Handle;

            // Set uniforms
            target.GenerateLocations();

            target.SunDirection =
                ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "sunLightStruct.direction");
            target.SunColor = ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "sunLightStruct.color");
            target.SunMatrix =
                ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "sunLightStruct.view_matrix");
            target.SunInnerMatrix =
                ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "sunLightStruct.inner_view_matrix");

            target.LightLocationsLocation = new int[maxNoLights];
            target.LightDirectionsLocation = new int[maxNoLights];
            target.LightColorsLocation = new int[maxNoLights];
            target.LightViewMatrixLocation = new int[maxNoLights];
            target.LightActiveLocation = new int[maxNoLights];
            target.LightTextureLocation = new int[maxNoLights];

            for (int i = 0; i < maxNoLights; i++)
            {
                target.LightActiveLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].actve");

                target.LightLocationsLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].position");
                target.LightDirectionsLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].direction");
                target.LightColorsLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].color");

                target.LightTextureLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].textre");

                target.LightViewMatrixLocation[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "lightStructs[" + i + "].view_matrix");
            }

            target.BoneMatrixLocations = new int[maxNoBones];
            for (int i = 0; i < maxNoBones; i++)
            {
                target.BoneMatrixLocations[i] =
                    ApplicationBase.Instance.Renderer.GetUniformLocation(shaderProgramHandle, "bone_matrix[" + i + "]");
            }
        }

        private void parseLog(string log, string name, string fullShader)
        {
            Utilities.ConsoleUtil.log(string.Format("\t{0} : {1}", name, log));

            StringBuilder fullSb = new StringBuilder();
            string newline;

            string tmpShader = fullShader;

            int nextIndex;
            int curline = 1;

            while ((nextIndex = tmpShader.IndexOf("\r\n")) != -1 && tmpShader != "\r\n")
            {
                if (tmpShader.Length > nextIndex + 2)
                    newline = tmpShader.Remove(nextIndex + 2);
                else
                    newline = tmpShader;

                tmpShader = tmpShader.Remove(0, newline.Length);

                fullSb.Append("\t" + curline + "\t|" + newline);

                curline++;
            }

            if (log.Contains("ERROR") || log.Contains("error") || log.Contains("WARNING") || log.Contains("warning"))
                Utilities.ConsoleUtil.log(
                  "\t--------------------------------------------\n" +
                     fullSb.ToString() +
                  "\t--------------------------------------------\n"
                );
        }

        #endregion

        #region .Text file

        public Shaders.Shader FromTextFile(string vfile, string ffile)
        {
            string name = ffile; //string.Empty;

            //try
            //{
            //    if (ffile.Contains(GameBase.Instance.path + "\\"))
            //        name = ffile.Replace(GameBase.Instance.path + "\\", "");
            //    else
            //        name = ffile;
            //}
            //catch (NullReferenceException)
            //{

            //}

            name = name.Replace(GameSettings.ShaderFolder, "");

            Debug.Print(ffile + " " + name);

            if (!shaderNames.ContainsKey(ffile))
            {
                Shaders.Shader curShader = new Shaders.Shader();

                int identifier = shaders.Count;

                curShader.type = enuShaderType.fromFile;
                curShader.Pointer = new string[] {vfile, ffile};
                curShader.Identifier = identifier;
                curShader.Name = name;
                curShader.Loaded = false;

                registerShader(curShader);
                return curShader;
            }
            else
            {
                return GetShaderByName(ffile);
            }
        }

        private void loadShaderFromFile(Shaders.Shader target)
        {
            string vfile = target.Pointer[0];
            string ffile = target.Pointer[1];

            target.VertexShader = readFile(vfile);
            target.FragmentShader = readFile(ffile);

            loadShaderFromCache(target);
        }

        private string readFile(string filename)
        {
            string line;
            StringBuilder wholeFile = new StringBuilder();

            System.IO.StreamReader file =
                new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(includeMarker))
                {
                    string[] sline = line.Split(' ');
                    appendSnip(sline, ref wholeFile);
                }
                else
                {
                    wholeFile.AppendLine(line);
                }
            }

            wholeFile.Replace(varMarker, "");
            wholeFile.Replace(functionsMarker, "");

            //ConsoleUtil.log(wholeFile.ToString());
            return wholeFile.ToString();
        }

        private void appendSnip(string[] arguments, ref StringBuilder wholeFile)
        {
            int noArguments = arguments.Length;
            for (int i = 0; i < noArguments; i++)
            {
                if (arguments[i].Contains(includeMarker))
                {
                    string snipName = arguments[i + 1];

                    foreach (var snipet in snippets)
                    {
                        if (snipName == snipet.name)
                        {
                            string modText = snipet.text;

                            for (int j = 0; j < noArguments; j++)
                            {
                                string CurArgument = arguments[j];
                                if (CurArgument.Contains("replace:"))
                                {
                                    string[] subArguments = CurArgument.Split(':');
                                    modText = modText.Replace(subArguments[1], subArguments[2]);
                                }
                            }

                            wholeFile.Replace(varMarker, snipet.variables);
                            wholeFile.Replace(functionsMarker, snipet.functions);
                            wholeFile.AppendLine(modText);
                        }
                    }
                }
            }
        }

        #endregion

        #region .Xml file

        public Shaders.Shader FromXmlFile(string file)
        {
            string name = file; //string.Empty;

            //try
            //{
            //    if (file.Contains(GameBase.Instance.path + "\\"))
            //        name = file.Replace(GameBase.Instance.path + "\\", "");
            //    else
                    name = file;
            //}
            //catch (Exception ex)
            //{
            //    ConsoleUtil.errorlog("ShaderLoader::FromXmlFile() ", ex.Message);

            //}

            name = name.Replace(GameSettings.ShaderFolder, "");

            Debug.Print(file + " " + name);

            if (!shaderNames.ContainsKey(name))
            {
                Shaders.Shader curShader = new Shaders.Shader();

                int identifier = shaders.Count;

                curShader.type = enuShaderType.fromXml;
                curShader.Pointer = new string[] {file};
                curShader.Identifier = identifier;
                curShader.Name = name;
                curShader.Loaded = false;

                registerShader(curShader);
                return curShader;
            }
            else
            {
                return GetShaderByName(name);
            }
        }

        private void loadShaderXml(Shaders.Shader target)
        {
            XmlTextReader reader = new XmlTextReader(target.Pointer[0]);

            string path = Path.GetDirectoryName(target.Pointer[0]) + "\\";

            //target.envMapAlphaBaseTexture = false;
            string vertex = string.Empty;
            string fragment = string.Empty;
            

            target.Pointer = new string[2];

            while (reader.Read())
            {
                // parsing data in material tag
                if (reader.Name == "shaderpair" && reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "vertex")
                        {
                            target.Pointer[0] = path + reader.Value;
                            vertex = target.Pointer[0];
                        }

                        else if (reader.Name == "fragment")
                        {
                            target.Pointer[1] = path + reader.Value;
                            fragment =  target.Pointer[1];
                        }
                    }
                    reader.MoveToElement();
                }
            }

            string vertexlog    = string.IsNullOrEmpty(vertex) ? "Vertex   : Not Used"    : string.Format("Vertex   : {0}", vertex);
            string fragmentlog  = string.IsNullOrEmpty(vertex) ? "Fragment : Not Used"    : string.Format("Fragment : {0}", fragment);

            Utilities.ConsoleUtil.log(string.Format("<> Loading shader {0}{3}   {1}{3}   {2}", /*NthDimension.Utilities.ConsoleEffects.Foreground_Blue + */
                                                                                                    target.Name/* +*/
                                                                                               /*NthDimension.Utilities.ConsoleEffects.Default*/, 
                                                                                               vertexlog, 
                                                                                               fragmentlog, 
                                                                                               Environment.NewLine));

            loadShaderFromFile(target);
        }

        #endregion

        private void registerShader(Shaders.Shader newShader)
        {

            shaderNames.Add(newShader.Name, newShader.Identifier);
            shaders.Add(newShader);
        }

        private void loadShaderByType(Shaders.Shader shader)
        {
            switch (shader.type)
            {
                case enuShaderType.fromFile:
                    loadShaderFromFile(shader);
                    break;
                case enuShaderType.fromXml:
                    loadShaderXml(shader);
                    break;
                case enuShaderType.fromCache:
                    loadShaderFromCache(shader);
                    break;
                default:
                    break;
            }
        }

        public void LoadShaders(Action<float> callback = null)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                if (!shaders[i].Loaded)
                    loadShaderByType(shaders[i]);

                if (null != callback)
                    callback((float) i / (float) shaders.Count);
            }
        }

        public void ForceReload(Action<float> callback = null)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                
                loadShaderByType(shaders[i]);
            }
        }

        public void LoadSnippet(string file)
        {
            Snippet newSnippet = new Snippet();

            string name = file; //string.Empty;

            //try
            //{
            //    if (file.Contains(GameBase.Instance.path + "\\"))
            //        name = file.Replace(GameBase.Instance.path + "\\", "");
            //    else
            //        name = file;
            //}
            //catch (NullReferenceException)
            //{


            //}


            name = name.Replace(GameSettings.ShaderFolder, "");

            Debug.Print(file + " " + name);

            newSnippet.name = name;


            foreach (var snippet in snippets)
            {
                if (snippet.name == newSnippet.name)
                    return;
            }

            string line;
            StringBuilder codeSb = new StringBuilder();
            StringBuilder variableSb = new StringBuilder();
            StringBuilder functionSb = new StringBuilder();

            Target curtarget = Target.code;

            System.IO.StreamReader mFile =
                new System.IO.StreamReader(file);
            while ((line = mFile.ReadLine()) != null)
            {
                if (line.Contains(varMarker))
                {
                    curtarget = Target.variable;
                }
                else if (line.Contains(codeMarker))
                {
                    curtarget = Target.code;
                }
                else if (line.Contains(functionsMarker))
                {
                    curtarget = Target.function;
                }
                else
                {
                    if (curtarget == Target.code)
                        codeSb.AppendLine(line);
                    else if (curtarget == Target.variable)
                        variableSb.AppendLine(line);
                    else if (curtarget == Target.function)
                        functionSb.AppendLine(line);
                }
            }

            variableSb.AppendLine(varMarker);
            functionSb.AppendLine(functionsMarker);

            //ConsoleUtil.log(wholeFile.ToString());
            newSnippet.text = codeSb.ToString();
            newSnippet.variables = variableSb.ToString();
            newSnippet.functions = functionSb.ToString();

            snippets.Add(newSnippet);
        }
    }
}
