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

// Added Extract functionality on ReadCache, ToDo seperate extract and improve logic (what happens to those already loaded?)
//  as it only when reading the cache file you can extract


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;
using NthDimension.FFMpeg;

namespace NthDimension.Rendering.Loaders
{
    public class MaterialLoader : ApplicationObject
    {
        private static bool         baseMaterialsLoaded;

        public List<Material>       materials               = new List<Material> { };
        private Hashtable           materialNames           = new Hashtable();

        protected List<string>      CustomCacheFiles        = new List<string>();

        public Hashtable            Entries
        {
            get { return materialNames; }
        }       // NOTE:: Never really used

        public MaterialLoader() { }

        public Material GetMaterialByName(string name)
        {
            int id = -1;

            try
            {
                id = (int)materialNames[name];
                if (!materials[id].loaded)
                    loadMaterial(materials[id]);
            }
            catch
            {
                
                ConsoleUtil.errorlog("Material Loader: ", string.Format("Material not found {0}", name));

               // return new Material(); // Uncommenting this freezes First Person View (when no materials for the buttons are found)
            }

            //if (id == -1)
            //    return new Material();

            if (id < 0)
                throw new Exception(string.Format("Material [{0}] has Id [{2}]", name, id));

            return materials[id];
        }

        #region Cache read/write

        public void AddCustomCacheFile(string cacheFile)
        {
            this.CustomCacheFiles.Add(cacheFile);
        }
        public void ReadCacheFile(string filename = "", Action<int> callback = null, bool extract = false)
        {
            List<Material> tmpMaterials = new List<Material>();
            // Load default
            if (!baseMaterialsLoaded)
            {
                if (filename == string.Empty)
                    filename = Settings.Instance.game.materialCacheFile;

                if (!File.Exists(filename))
                {
                    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(filename)));
                }
                else { 
                    FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    readStream(ref fileStream, ref tmpMaterials, callback);
                }
                baseMaterialsLoaded = true;
            }
            // Load custom
            if (CustomCacheFiles.Count > 0)
            {
                foreach (string path in CustomCacheFiles)
                {
                    if (!File.Exists(path))
                    {
                        //throw new FileNotFoundException(path);
                        ConsoleUtil.errorlog("Cache file not found ", path);
                        continue;
                    }

                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    readStream(ref fs, ref tmpMaterials, null);
                    fs.Close();
                }
            }

            int materialCount = tmpMaterials.Count;
            for (int i = 0; i < materialCount; i++)
            {
                Material curMat = tmpMaterials[i];
                string name = curMat.name;

                if (!materialNames.ContainsKey(name))
                {
                    curMat.type = Material.Type.fromCache;

                    int identifier = materials.Count;

                    curMat.identifier = identifier;

                    materialNames.Add(name, identifier);
                    materials.Add(curMat);
                }

                if (extract)
                {
                    try
                    {
                        string dirBase = Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents_Extracted, GameSettings.MaterialFolder);
                        string file = Path.GetFileName(curMat.name);
                        string dirLocal = name.ToString().Replace(file, string.Empty);

                        string dir = Path.Combine(dirBase, dirLocal);
                        string path = Path.Combine(dir, file);

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            ConsoleUtil.log(string.Format("<> CREATED Directory {0}", dir));
                        }

                        using (StreamWriter sr = new StreamWriter(path + "_meta"))
                        {
                            for (int t = 0; t < curMat.Textures.Length; t++)
                                sr.WriteLine(curMat.Textures[t].name);

                            if (null != curMat.shader.Name)
                                sr.WriteLine(string.Format("Base     :", curMat.shader.Name));

                            if (null != curMat.definfoshader.Name)
                                sr.WriteLine(string.Format("Deferred :", curMat.definfoshader.Name));

                            if (null != curMat.shadowshader.Name)
                                sr.WriteLine(string.Format("Shadow   :", curMat.shadowshader.Name));

                            if (null != curMat.ssnshader.Name)
                                sr.WriteLine(string.Format("SSNormal :", curMat.ssnshader.Name));

                            if (null != curMat.selectionshader.Name)
                                sr.WriteLine(string.Format("Select   :", curMat.selectionshader.Name));
                        }

                        ConsoleUtil.log(string.Format("<> Extracted Material {0}", name));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.log(string.Format("<! Failed Extract Material {0}", name));
                    }
                }
            }

            CustomCacheFiles.Clear(); // TODO:: Update log what custom files added
            Utilities.ConsoleUtil.log(string.Format("\tAdded {0} materials from {1}", materialCount, Path.GetFileName(filename)));
        }

        protected void readStream(ref FileStream fileStream, ref List<Material> tmpMaterials, Action<int> callback)
        {
            //using (fileStream)
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

                tmpMaterials.AddRange((List<Material>)GenericMethods.ByteArrayToObject<List<Material>>(bytes));
                //fileStream.Close();
            }
        }
        public void WriteCacheFile(/*string directory = "",*/ Action<string> callback = null)
        {
            // Commented Apr-18-18 Seperating materials for each scene cache file
            //if (directory == string.Empty)
            //    directory = Settings.Instance.game.materialCacheFile;
            //else
            //    directory = Path.Combine(directory, Settings.Instance.game.materialCacheFile);

            List<Material> SaveList = new List<Material> { };
            List<string> sceneNames = new List<string>();

            foreach (var material in materials)
            {
                if (null != callback)
                    callback(string.Format("caching material {0}", material.name));

                // Prepare Cache Data /////////////////////////////////////////////////////////
                if(!(material.type == Material.Type.dynamic))
                    material.cacheMaterial(ref SaveList);

                // Retrieve Scene Names /////////////////////////////////////////////////////// 
                if (material.name.Contains("scenes"))
                {
                    try
                    {
                        string[] nameParts = material.name.Split('\\');
                        string sceneName = nameParts[1];
                        if (!sceneNames.Contains(sceneName))
                            sceneNames.Add(sceneName);
                    }
                    catch (Exception eD)
                    {
                        ConsoleUtil.errorlog("MaterialLoader.WriteCacheFile() failed to retrieve Scene name ", eD.Message);
                    }
                }
            }

            // Collect Each Scene Models //////////////////////////////////////////////////////
            List<ListMaterial> groupScenes = new List<ListMaterial>();
            foreach (string scene in sceneNames)
            {
                string filterName = string.Format(@"scenes\{0}", scene);
                ListMaterial newScene = new ListMaterial(SaveList.Where(m => m.name.Contains(filterName)).ToList()) { ListName = scene };
                groupScenes.Add(newScene);
                // Remove Scene Models from Collection ////////////////////////////////////////////
                SaveList.RemoveAll(item => newScene.Contains(item));

                ConsoleUtil.log(string.Format("+++ Seperated materials for Scene {0}", scene));
            }

            // Collect Vehicles Materials
            ListMaterial vehicles = new ListMaterial(SaveList.Where(m => m.name.Contains(@"vehicles\")).ToList());
            SaveList.RemoveAll(item => vehicles.Contains(item));

            // Write Cache Files to Disk
            string materialsCache = Settings.Instance.game.materialCacheFile;
            try
            {
                using (FileStream fileStream = new FileStream(materialsCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<List<Material>>(SaveList);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write Materials ", "Failed to store default materials " + mE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (Exception mE)
            {
                ConsoleUtil.errorlog("Cache Write ", "Failed to store characters " + mE.Message);
            }

            string vehiclesCache = Settings.Instance.game.materialVehiclesCacheFile;
            try
            {
                using (FileStream fileStream = new FileStream(vehiclesCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<List<Material>>(vehicles);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write Materials ", "Failed to store vehicle materials " + mE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (Exception mE)
            {
                ConsoleUtil.errorlog("Cache Write ", "Failed to store vehicles " + mE.Message);
            }


            foreach (ListMaterial listScene in groupScenes)
            {
                string sceneCacheFile = string.Format("cacheMaterial{0}.ucf", listScene.ListName.ToLower());
                try
                {
                    using (FileStream fileStream = new FileStream(sceneCacheFile, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            byte[] saveAry = GenericMethods.ObjectToByteArray<ListMaterial>(listScene);
                            fileStream.Write(saveAry, 0, saveAry.Length);
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Cache Write Materials ", string.Format("Failed to store {0} materials ", listScene.ListName) + mE.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                }
                catch (Exception mE)
                {
                    ConsoleUtil.errorlog("Cache Write ", "Failed to store characters " + mE.Message);
                }
            }
        }
        #endregion

        #region Proprietary .Xml
        /// <summary>
        /// WARNING: Case sensitive all
        /// </summary>
        /// <param name="pointer"></param>
        public void FromXmlFile(string pointer)
        {
            string name = pointer;//string.Empty;
          
            if(name.Contains(GameSettings.MaterialFolder))
                name = name.Replace(GameSettings.MaterialFolder, "");

            if (!materialNames.Contains(name))
            {
                Material newMat = new Material();

                newMat.type = Material.Type.fromXml;
                newMat.name = name;
                newMat.pointer = pointer;       // *consumed by loadMaterialXml
                newMat.setArys();

                register(newMat);
            }
        }
        private void loadMaterialXml(Material target)
        {
            if (!File.Exists(target.pointer)) return;

            XmlTextReader reader = new XmlTextReader(target.pointer);

            //target.envMapAlphaBaseTexture = false;

            Utilities.ConsoleUtil.log(string.Format("<> Loading Material from xmf: {0} - {1} ", target.name, target.pointer));

            while (reader.Read())
            {
                // parsing data in material tag
                #region Shaders
                // SPol:  Design consideration: Xml files are loaded on thle fly. But all assets must be instantiated
                //          and available (eg shaders compiled, textures loaded). A later-load function would be nice but
                //          should be carefully crafted
                if (reader.Name == "material" && reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        #region Shaders
                        if (reader.Name == "shader")
                            target.shader = ApplicationBase.Instance.ShaderLoader.GetShaderByName(reader.Value);

                        else if (reader.Name == "ssnshader")
                            target.ssnshader = ApplicationBase.Instance.ShaderLoader.GetShaderByName(reader.Value);

                        else if (reader.Name == "selection")
                            target.selectionshader = ApplicationBase.Instance.ShaderLoader.GetShaderByName(reader.Value);

                        else if (reader.Name == "shadow")
                            target.shadowshader = ApplicationBase.Instance.ShaderLoader.GetShaderByName(reader.Value);

                        else if (reader.Name == "definfo")
                            target.definfoshader = ApplicationBase.Instance.ShaderLoader.GetShaderByName(reader.Value);
                        #endregion Shaders
                    }
                    Utilities.ConsoleUtil.log("    shader: " + target.shader.Name);
                    // TODO:: Log the rest shaders
                    reader.MoveToElement();
                }
                #endregion
                // parsing textures // TODO:: PBR complete
                #region Textures (2D)
                if (reader.Name == "textures" && reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        try
                        {
                            if (reader.Name         == "base")
                                target.setTexture(Material.TexType.baseTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "base2")
                                target.setTexture(Material.TexType.baseTextureTwo, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "base3")
                                target.setTexture(Material.TexType.baseTextureThree, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "base4")
                                target.setTexture(Material.TexType.baseTextureFour, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "normal")
                                target.setTexture(Material.TexType.normalTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "reflection")
                                target.setTexture(Material.TexType.reflectionTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "definfo")
                                target.setTexture(Material.TexType.definfoTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "emit")
                                target.setTexture(Material.TexType.emitTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));
                            else if (reader.Name    == "video" || reader.Name == "videoloop")
                            {
                                if (!ApplicationBase.Instance.IsIntegratedGpu)
                                {
                                    target.type = Material.Type.dynamic;

                                    string videoFolderFull = Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, GameSettings.VideoFolder);
                                    string videoPath = Path.Combine(videoFolderFull, reader.Value);

                                    //ConsoleUtil.log(string.Format("   ffmpeg video: {0}", videoPath));

                                    VideoSource videoSource = FFMpeg.VideoGL3x.Read(videoPath);

                                    videoSource.Loop = reader.Name == "videoloop";

                                    if (null == videoSource)
                                        throw new Exception(string.Format("Failed to create ffmpeg videosource for {0}", videoPath));

                                    #region old? delete?
                                    //string tname = videoSource.Path;
                                    //string strRemove = string.Format("{0}\\{1}", NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, GameSettings.VideoFolder);
                                    //tname = tname.Replace(strRemove, string.Empty);

                                    //Texture t = new Texture();
                                    //t.texture = videoSource.VideoPlayback.TextureHandle;
                                    //t.identifier = ApplicationBase.Instance.TextureLoader.textures.Count;
                                    //t.type = Texture.Type.fromVideo;
                                    //t.pointer = t.name = tname;
                                    //t.bitmap = new System.Drawing.Bitmap(videoSource.VideoSize.Width, videoSource.VideoSize.Height);
                                    //t.loaded = true;

                                    //ApplicationBase.Instance.TextureLoader.registerTexture(t);

                                    //target.setTexture(Material.TexType.baseTexture, t);
                                    //target.propertys.useAlpha = true;
                                    ////#endregion
                                    #endregion

                                    if (!ApplicationBase.Instance.VideoSources.ContainsKey(videoSource))
                                        ApplicationBase.Instance.VideoSources.Add(videoSource, target);

                                    ConsoleUtil.log(string.Format("+++ Added Video Source {0}", reader.Value));
                                }
                                else
                                {
                                    ConsoleUtil.log(string.Format("<!> No ffmpeg support on integrated graphics Gpu ", reader.Value));
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            if(e is NotImplementedException)
                                ConsoleUtil.errorlog("Error in Material: ", string.Format("Failed to load '{0}', NotImplemented {1}", target.name, reader.Value));
                            else
                                ConsoleUtil.errorlog("Error in Material: ", string.Format("Failed to load '{0}', texture '{1}' is missing", target.name, reader.Value));
                            continue;
                        }
                    }
                    // TODO:: Log the rest textures
                    reader.MoveToElement();
                }
                #endregion

                #region Textures (3D)
                #region Environment map (eg reflections)
                // parsing envmap data
                if (reader.Name == "envmap")
                {
                    target.propertys.useEnv = true;
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "source")
                            {
                                if (reader.Value == "normalalpha")
                                    target.propertys.envMapAlphaNormalTexture = true;

                                else if (reader.Value == "basealpha")
                                    target.propertys.envMapAlphaBaseTexture = true;

                                else
                                    target.setTexture(Material.TexType.envMapTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));

                            }
                            else if (reader.Name == "tint")
                            {
                                target.propertys.envMapTint = GenericMethods.Vector3FromString(reader.Value);
                            }
                        }
                        reader.MoveToElement();
                    }
                }
                #endregion Env map

                #region Specular map (Shininness TODO:: Metalness)
                ///* -- moved to textures
                // parsing specular data
                if (reader.Name == "specmap")
                {
                    target.propertys.useSpec = true;
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "source")
                            {
                                if (reader.Value == "normalalpha")
                                    target.propertys.specMapAlphaNormalTexture = true;

                                else if (reader.Value == "basealpha")
                                    target.propertys.specMapAlphaBaseTexture = true;

                                else
                                    target.setTexture(Material.TexType.specMapTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));

                            }
                            else if (reader.Name == "tint")
                            {
                                target.propertys.specMapTint = GenericMethods.Vector3FromString(reader.Value);
                            }
                            else if (reader.Name == "exp")
                            {
                                target.propertys.specExp = GenericMethods.FloatFromString(reader.Value);
                            }
                        }
                        reader.MoveToElement();
                    }
                }
                #endregion

                #region Emissive map (eg Statically lighted regions)
                // parsing emit data
                if (reader.Name == "emit")
                {
                    target.propertys.useEmit = true;
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "source")
                            {
                                if (reader.Value == "normalalpha")
                                    target.propertys.emitMapAlphaNormalTexture = true;

                                else if (reader.Value == "basealpha")
                                    target.propertys.emitMapAlphaBaseTexture = true;

                                else
                                    target.setTexture(Material.TexType.emitMapTexture, ApplicationBase.Instance.TextureLoader.getTexture(reader.Value, target.name));

                            }
                            else if (reader.Name == "tint")
                            {
                                target.propertys.emitMapTint = GenericMethods.Vector3FromString(reader.Value);
                            }
                        }
                        reader.MoveToElement();
                    }
                }
                #endregion

                #region HeightMap

                #endregion HeightMap

                #endregion

                #region Transparency
                // parsing transparency data
                if (reader.Name == "transparency")
                {
                    target.propertys.useAlpha = true;
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "refraction")
                                target.propertys.refStrength = GenericMethods.FloatFromString(reader.Value);

                            if (reader.Name == "blur")
                                target.propertys.blurStrength = GenericMethods.FloatFromString(reader.Value);

                            if (reader.Name == "fresnel")
                                target.propertys.fresnelStrength = GenericMethods.FloatFromString(reader.Value);
                        }
                        reader.MoveToElement();
                    }
                }
                #endregion

                #region Global Lighting (TODO:: Sunlight Color)
                // parsing lighting data
                if (reader.Name == "lighted")
                {
                    target.propertys.useLight = true;
                }
                #endregion

                #region Fresnel Lighting (Lighthouse lighting)

                // parsing fresnel data
                if (reader.Name == "fresnel" && reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "exp")
                            target.propertys.fresnelExp = GenericMethods.FloatFromString(reader.Value);

                        else if (reader.Name == "strength")
                            target.propertys.fresnelStr = GenericMethods.FloatFromString(reader.Value);
                    }
                    reader.MoveToElement();
                }
                #endregion

                if (reader.Name == "tiled")
                {
                    target.propertys.useTile = true;
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "tileu")
                                target.propertys.tileU = int.Parse(reader.Value);

                            if (reader.Name == "tilev")
                                target.propertys.tileV = int.Parse(reader.Value);
                        }
                    }



                    target.setTextureWrap(target.propertys.tileU, target.propertys.tileV);
                }

                // parsing nucull
                if (reader.Name == "nocull")
                {
                    target.propertys.noCull = true;
                }

                // parsing nucull
                if (reader.Name == "nodepthmask")
                {
                    target.propertys.noDepthMask = true;
                }

                // parsing additive
                if (reader.Name == "additive")
                {
                    target.propertys.additive = true;
                }

                target.loaded = true;
                materials[target.identifier] = target;
            }
        }
        #endregion

        private void register(Material newMat)
        {
            if (!materialNames.Contains(newMat.name))
            {
                newMat.identifier = materials.Count;
                materials.Add(newMat);
                materialNames.Add(newMat.name, newMat.identifier);
            }
        }

        public void LoadMaterials(Action<float> callback = null)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (!materials[i].loaded)
                    loadMaterial(materials[i]);

                if (null != callback)
                    callback((float)i / (float)materials.Count);
            }
        }

        private void loadMaterial(Material material)
        {
            try
            {
                switch (material.type)
                {
                    case Material.Type.fromXml:
                        loadMaterialXml(material);
                        break;
                    case Material.Type.fromCache:
                        loadMaterialCache(material);
                        break;
                    case Material.Type.dynamic:
                        throw new NotImplementedException();
#pragma warning disable CS0162
                        break;
                    default:
                        break;
                }
            }
            catch(Exception lE)
            {
                ConsoleUtil.errorlog(string.Format("(!)Error Loading Material: {0}", material.name), string.Format("{0}", lE.Message));
            }
        }

        private void loadMaterialCache(Material material)
        {

            Utilities.ConsoleUtil.log(string.Format("<> Loading Material from xmf: {0} - {1} ", material.name, material.pointer));
            //shaders
            material.resolveShaders(ApplicationBase.Instance.ShaderLoader);

            //textures
            material.resolveTextures(ApplicationBase.Instance.TextureLoader);

            material.loaded = true;

            materials[material.identifier] = material;
        }



    }
}
