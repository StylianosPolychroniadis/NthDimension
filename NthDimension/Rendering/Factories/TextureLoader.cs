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

//#define IMAGEFAST

// Added Extract functionality on ReadCache, ToDo seperate extract and improve logic (what happens to those already loaded?)
//  as it only when reading the cache file you can extract

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using NthDimension.Rasterizer;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;
using System.Collections.Concurrent;
using System.Linq;
using NthDimension.Collections;

namespace NthDimension.Rendering.Loaders
{
    public class TextureLoader : ApplicationObject
    {
        public ThreadSafeList<Texture> Pending = new ThreadSafeList<Texture>();
       

        public List<Texture> textures = new List<Texture> { };
        private Hashtable textureNames = new Hashtable();

        protected List<string> CustomCacheFiles = new List<string>();


        public bool GenerateMipmaps = false;

        private static bool baseTexturesLoaded = false;

        public Hashtable Entries
        {
            get { return textureNames; }
        }


        public TextureLoader()
        {
            //this.gameWindow = mGameWindow;
        }


        public void AddCustomCacheFile(string cacheFile)
        {
            this.CustomCacheFiles.Add(cacheFile);
        }
        public void ReadCacheFile(string filename = "", Action<int> callback = null, bool extract = false)
        {
            ListTexture tmpTextures = new ListTexture();

            if (!baseTexturesLoaded)
            {
                if (filename == string.Empty)
                    filename = Settings.Instance.game.textureCacheFile;

                if (!File.Exists(filename))
                    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(filename)));
                else
                {

                    FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    
                    ListTexture tbase = readStream(ref fileStream, callback);
                    tmpTextures.AddRange(tbase);
                    //readStream(ref fileStream, ref tmpTextures, callback);
                    fileStream.Close();
                    ConsoleUtil.log(string.Format("\tAdded {0} base textures from {1}", tbase.Count, Path.GetFileName(filename)));
                }

                baseTexturesLoaded = true;
            }

            #region New

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
                    ListTexture tcustom = readStream(ref fs, null);
                    tmpTextures.AddRange(tcustom);
                    //readStream(ref fs, ref tmpTextures, null);
                    fs.Close();
                    ConsoleUtil.log(string.Format("\tAdded {0} textures from {1}", tcustom.Count, Path.GetFileName(path)));
                }
            }
            #endregion New

            if (tmpTextures != null)
                foreach (var Texture in tmpTextures)
                {
                    Texture curTex = Texture;
                    string name = Texture.name;

                    if (!textureNames.ContainsKey(name))
                    {
                        int identifier = textures.Count;

                        curTex.type = Texture.Type.fromPng;
                        curTex.identifier = identifier;
                        curTex.bitmap = new Bitmap(GenericMethods.byteArrayToImage(curTex.cacheBitmap));
                        textures.Add(curTex);
                        textureNames.Add(name, identifier);
                    }

                    #region Extract
                    if (extract)
                    {
                        string dirBase = Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents_Extracted, GameSettings.TextureFolder);
                        string file = Path.GetFileName(Texture.pointer);
                        string dirLocal = name.ToString().Replace(file, string.Empty);

                        string dir = Path.Combine(dirBase, dirLocal);
                        string path = Path.Combine(dir, file);



                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            ConsoleUtil.log(string.Format("<> CREATED Directory {0}", dir));
                        }
                        curTex.bitmap.Save(path);
                        ConsoleUtil.log(string.Format("<> Extracted Texture {0}", name));
                    }
                    #endregion Extract
                }

            CustomCacheFiles.Clear();
        }

        protected ListTexture readStream(ref FileStream fileStream, /*ref ListTexture tmpTextures,*/ Action<int> callback = null)
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


                return  new ListTexture(GenericMethods.ByteArrayToObject<ListTexture>(bytes));
                //fileStream.Close();
            }
        }

        public void WriteCacheFile(/*string directory = "",*/ Action<string> callback = null)
        {
            // Commented Apr-18-18 Seperating Scene Textures to dedicated scene cache files
            //if (directory == string.Empty)
            //    directory = Settings.Instance.game.textureCacheFile;
            //else
            //    directory = Path.Combine(directory, Settings.Instance.game.textureCacheFile);

            ListTexture     SaveList = new ListTexture();
            List<string>    sceneNames = new List<string>();

            foreach (var texture in textures)
            {
                if (null != callback)
                    callback(string.Format("caching texture {0}", texture.name));
                
                // Prepare Cache Data /////////////////////////////////////////////////////////
                texture.cacheTexture(ref SaveList);

                // Retrieve Scene Names /////////////////////////////////////////////////////// 
                if (texture.name.Contains("scenes"))
                {
                    try
                    {
                        string[] nameParts = texture.name.Split('\\');
                        string sceneName = nameParts[1];
                        if (!sceneNames.Contains(sceneName))
                            sceneNames.Add(sceneName);
                    }
                    catch (Exception eD)
                    {
                        ConsoleUtil.errorlog("TextureLoader.WriteCacheFile() failed to retrieve Scene name ", eD.Message);
                    }
                }
            }

            // Collect Each Scene Textures ///////////////////////////////////////////////////
            List<ListTexture> groupScenes = new List<ListTexture>();
            foreach (string scene in sceneNames)
            {
                string filterName = string.Format(@"scenes\{0}", scene);
                ListTexture newScene = new ListTexture(SaveList.Where(m => m.name.Contains(filterName)).ToList()) { ListName = scene };
                groupScenes.Add(newScene);
                // Remove Scene Models from Collection ////////////////////////////////////////////
                SaveList.RemoveAll(item => newScene.Contains(item));

                ConsoleUtil.log(string.Format("+++ Seperated textures for Scene {0}", scene));
            }

            ListTexture vehicles = new ListTexture(SaveList.Where(m => m.name.Contains(@"vehicles\")).ToList());
            SaveList.RemoveAll(item => vehicles.Contains(item));


            // Write Cache Files to Disk
            string texturesCache = Settings.Instance.game.textureCacheFile;
            try
            {
                using (FileStream fileStream = new FileStream(texturesCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListTexture>(SaveList);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                    }
                    catch (Exception tE)
                    {
                        ConsoleUtil.errorlog("Cache Write Textures ", "Failed to store default textures " + tE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (Exception tcE)
            {
                ConsoleUtil.errorlog("Cache Write Textures ", "Failed to store default textures " + tcE.Message);
            }
            string vehiclesCache = Settings.Instance.game.textureVehiclesCacheFile;
            try
            {
                using (FileStream fileStream = new FileStream(vehiclesCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListTexture>(vehicles);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                    }
                    catch (Exception tE)
                    {
                        ConsoleUtil.errorlog("Cache Write Textures ", "Failed to store vehicle textures " + tE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (Exception tcE)
            {
                ConsoleUtil.errorlog("Cache Write Textures ", "Failed to store vehicle textures " + tcE.Message);
            }
            foreach (ListTexture listScene in groupScenes)
            {
                string sceneCacheFile = string.Format("cacheTexture{0}.ucf", listScene.ListName.ToLower());
                using (FileStream fileStream = new FileStream(sceneCacheFile, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListTexture>(listScene);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                    }
                    catch (Exception tE)
                    {
                        ConsoleUtil.errorlog("Cache Write Textures ", string.Format("Failed to store {0} textures ", listScene.ListName) + tE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
        }

        public Texture getTexture(string name, string fromMaterial = "")
        {
            if (name == "" || name == null)
                return new Texture();

            int identifier = -1;

            try
            {
                identifier = (int)textureNames[name];
            }
            catch
            {
                if (fromMaterial.Length > 0)
                    //throw new Exception(string.Format("Texture Loader: Fatal error in material {0} definition. Texture not found {1}", fromMaterial, name));
                    ConsoleUtil.errorlog("Texture Loader Error: ", string.Format("Material {0} definition. Texture not found {1}", fromMaterial, name));
                else
                    //throw new Exception(string.Format("Texture Loader: Texture not found {0}", name));
                    ConsoleUtil.errorlog("Texture Loader Error: ", string.Format("Texture not found {0}", name));

            }


            if (!textures[identifier].loaded)
                loadTexture(textures[identifier]);

            return textures[identifier];
        }

        public int getTextureId(string name)
        {
            return getTexture(name).texture;
        }

        public void fromPng(string file)
        {
            fromPng(file, true);
        }

        public void fromPng(string file, bool sampling)
        {
            string name = file;

            //try
            //{
            //    if (file.Contains(GameBase.Instance.path + "\\"))
            //    {
            //        name = name.Replace(GameBase.Instance.path + "\\", "");
                    
            //    }
            //    else if (file.Contains(DirectoryUtil.Documents_MySoci))
            //        name = Path.GetFileName(file);
            //    else
            //        name = file;
            //}
            //catch (NullReferenceException)
            //{

            //}

            if(name.Contains(GameSettings.TextureFolder))
                name = name.Replace(GameSettings.TextureFolder, "");

            if (!textureNames.ContainsKey(name))
            {
                Texture curTexture = new Texture();

                curTexture.identifier = textures.Count;
                curTexture.type = Texture.Type.fromPng;
                curTexture.loaded = false;
                curTexture.pointer = file;
                curTexture.name = name;
                curTexture.multisampling = sampling;

                registerTexture(curTexture);

            }
        }

        public void fromDds(string file)
        {
            string name = file;//string.Empty;




            //name = Path.GetFileName(name);

            if (name.Contains(GameSettings.TextureFolder))
                name = name.Replace(GameSettings.TextureFolder, "");


            if (!textureNames.ContainsKey(name))
            {
                Texture curTexture = new Texture();

                curTexture.identifier = textures.Count;
                curTexture.type = Texture.Type.fromDds;
                curTexture.loaded = false;
                curTexture.pointer = file;
                curTexture.name = name;

                registerTexture(curTexture);
            }
        }

        public void fromFramebuffer(string name, int texture)
        {
            Texture curTexture = new Texture();

            curTexture.identifier = textures.Count;
            curTexture.type = Texture.Type.fromFramebuffer;
            curTexture.loaded = true;
            curTexture.texture = texture;
            curTexture.name = name;

            registerTexture(curTexture);


        }

        public void fromBitmap(string name)
        {
            if (textureNames.ContainsKey(name))
                throw new Exception("Texture name already exists. Must be a unique name");

            Texture curTexture = new Texture();
            curTexture.identifier = textures.Count;
            curTexture.type = Texture.Type.fromVideo;
            curTexture.loaded = false;                   //????????
            curTexture.name = name;
            curTexture.texture = curTexture.identifier; // Not Sure, looks ok

            registerTexture(curTexture);
        }

        public void fromWebsite(string url)
        {
            
        }

        public void registerTexture(Texture curTex)
        {
            if (textureNames.ContainsKey(curTex.name))
                textureNames.Remove(curTex.name);

            textureNames.Add(curTex.name, curTex.identifier);
            textures.Add(curTex);
        }

        public void LoadTextures(Action<float> callback = null, bool force = false)
        {
            GenerateMipmaps = true; // switched to false for multithreaded loading of textures

            for (int i = 0; i < textures.Count; i++)
            {
                try
                {
                    if (!textures[i].loaded)
                        loadTexture(textures[i], force);

                    if (null != callback)
                        callback((float) i / (float) textures.Count);
                }
                catch
                {
                }
            }
        }

        //public float loadSingleTextures()
        //{
        //    /*
        //    for (int i = 0; i < textures.Count; i++)
        //    {
        //        if (!textures[i].loaded)
        //        {
        //            loadTexture(textures[i]);
        //            return (float)i / (float)textures.Count;
        //        }
        //    }
        //     * */
        //    return 1;
        //}

        public void loadTexture(Texture target, bool force = false)
        {
            switch (target.type)
            {
                case Texture.Type.fromPng:
                    try
                    {
                        
                        loadTextureFromPng(target, force);
                    }
                    catch (Exception e)
                    {
                        //throw e;
                        ConsoleUtil.errorlog(string.Format("(!)Error Loading Texture: {0} ", target), e.Message);
                    }
                    break;
                case Texture.Type.fromDds:
                    try
                    {
                        loadTextureFromDds(target);
                    }
                    catch (Exception e)
                    {
                        //throw e;
                        ConsoleUtil.errorlog(string.Format("(!)Error Loading Texture: {0} ", target), e.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        private void loadTextureFromDds(Texture target)
        {
            Utilities.ConsoleUtil.logFileSize(string.Format("<> Loading Texture from disk: {0} ", target.name), GenericMethods.FileSizeReadable(target.pointer));

            //GameBase.Instance.Renderer.Enable(EnableCap.Texture2D);
            ApplicationBase.Instance.Renderer.Texture2DEnabled = true;

            uint ImageTextureHandle;
            TextureTarget ImageTextureTarget;

            DDSLoader.LoadFromDisk(target.pointer, out ImageTextureHandle, out ImageTextureTarget);

            // load succeeded, Texture can be used.
            ApplicationBase.Instance.Renderer.BindTexture(ImageTextureTarget, ImageTextureHandle);
            ApplicationBase.Instance.Renderer.TexParameter(ImageTextureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            int[] MipMapCount = new int[1];
            ApplicationBase.Instance.Renderer.GetTexParameter(ImageTextureTarget, GetTextureParameter.TextureMaxLevel, out MipMapCount[0]);
            if (MipMapCount[0] == 0) // if no MipMaps are present, use linear Filter
                ApplicationBase.Instance.Renderer.TexParameter(ImageTextureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            else // MipMaps are present, use trilinear Filter
                ApplicationBase.Instance.Renderer.TexParameter(ImageTextureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            target.texture = (int)ImageTextureHandle;
            target.loaded = true;

            textures[target.identifier] = target;
        }

        private void loadTextureFromPng(Texture target, bool force = false)
        {
            if (String.IsNullOrEmpty(target.pointer))
                throw new ArgumentException(target.pointer);

            Bitmap bmp;

            #region Texture from Cache or from File
            if (target.cacheBitmap == null)
            {



                Utilities.ConsoleUtil.logFileSize(string.Format("<> Loading Texture from disk: {0} ", target.name),
                    GenericMethods.FileSizeReadable(target.pointer));

#if !IMAGEFAST
                FileStream imgPathfs = new FileStream(target.pointer, FileMode.Open, FileAccess.Read);
                bmp = new Bitmap(Image.FromStream(imgPathfs, true, false));
                imgPathfs.Dispose();
#else

                bmp = (Bitmap) Imaging.ImageFast.FromFile(target.pointer);
#endif
                //bmp = new Bitmap(target.pointer);
            }
            else
            {
                Utilities.ConsoleUtil.log(string.Format("<> Loading Texture from cache: {0} ", target.name));
                bmp = (Bitmap)GenericMethods.byteArrayToImage(target.cacheBitmap);
            }


#endregion


            target.CacheBitmap = bmp;
            target.bitmap = bmp;

            if (!force)
                Pending.Add(target);
            else
            {
                // Everything below should be executed in UI Thread

                target.texture = ApplicationBase.Instance.Renderer.GenTexture();
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, target.texture);

                target.bitmap = null;

                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, NthDimension.Rasterizer.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);

                int sampling = 0;
                if (target.multisampling)
                    sampling = (int)TextureMinFilter.Linear;
                else
                    sampling = (int)TextureMinFilter.Nearest;


                //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);    // Commented due to mipmap use above
                //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);

                #region Mipmap
                // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // On newer video cards, we can use Game.Instance.Renderer.GenerateMipmaps() or Game.Instance.Renderer.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.



                int[] MipMapCount = new int[1];
                //GameBase.Instance.Renderer.Enable(EnableCap.Texture2D);
                ApplicationBase.Instance.Renderer.Texture2DEnabled = true;




                if (GenerateMipmaps)
                {
                    ApplicationBase.Instance.Renderer.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                    ApplicationBase.Instance.Renderer.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureMaxLevel, out MipMapCount[0]);
                }


                if (!GenerateMipmaps || MipMapCount[0] == 0) // if no MipMaps are present, use linear Filter
                {
                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);
                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);
                }
                else // MipMaps are present
                {
                    target.Mipmaped = true;
                    target.Mipmaps = MipMapCount[0];

                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                    ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                    //Game.Instance.checkGlError("Generate Mipmap");
                }
                #endregion

                target.loaded = true;
                textures[target.identifier] = target;
            }
        }

        public void UploadPngToGpu(Texture target)
        {
            Bitmap bmp = target.bitmap;
            // Everything below should be executed in UI Thread

            target.texture = ApplicationBase.Instance.Renderer.GenTexture();
            ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, target.texture);

            target.bitmap = null;

            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, NthDimension.Rasterizer.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            int sampling = 0;
            if (target.multisampling)
                sampling = (int)TextureMinFilter.Linear;
            else
                sampling = (int)TextureMinFilter.Nearest;


            //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);    // Commented due to mipmap use above
            //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);

            #region Mipmap
            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use Game.Instance.Renderer.GenerateMipmaps() or Game.Instance.Renderer.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.



            int[] MipMapCount = new int[1];
            //GameBase.Instance.Renderer.Enable(EnableCap.Texture2D);
            ApplicationBase.Instance.Renderer.Texture2DEnabled = true;




            if (GenerateMipmaps)
            {
                ApplicationBase.Instance.Renderer.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                ApplicationBase.Instance.Renderer.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureMaxLevel, out MipMapCount[0]);
            }


            if (!GenerateMipmaps || MipMapCount[0] == 0) // if no MipMaps are present, use linear Filter
            {
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);
            }
            else // MipMaps are present
            {
                target.Mipmaped = true;
                target.Mipmaps = MipMapCount[0];

                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                //Game.Instance.checkGlError("Generate Mipmap");
            }
            #endregion

            target.loaded = true;
            textures[target.identifier] = target;
        }

        public void DeleteTexture(int texture)
        {
            uint textid = (uint) texture;
            ConsoleUtil.log(string.Format("Deleting Texture2D {0}", textid));
            try
            {
                ApplicationBase.Instance.Renderer.DeleteTextures(1, ref textid);
            }
            catch (Exception dT)
            {
                ConsoleUtil.errorlog(string.Format("DeleteTexture {0}", textid), dT.Message);
            }
            ApplicationBase.Instance.CheckGlError(string.Format("DeleteTexture {0}", textid));
        }

        public void DeleteTexture(string name)
        {
            int texid = getTextureId(name);
            this.DeleteTexture(texid);
        }


        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        public static Bitmap GrabScreenshot()
        {
            if (!ApplicationBase.Instance.Renderer.HasContext())
                throw new Exception("OpenGL Graphics Context Missing");

            Bitmap bmp = new Bitmap(ApplicationBase.Instance.ClientSize.Width, ApplicationBase.Instance.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(ApplicationBase.Instance.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ApplicationBase.Instance.Renderer.ReadPixels(0, 0, ApplicationBase.Instance.ClientSize.Width, ApplicationBase.Instance.ClientSize.Height, NthDimension.Rasterizer.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            int uid = 0;
            while (File.Exists("screenshot" + uid + ".png"))
                uid++;

            // Save the image as a Png.
            bmp.Save("screenshot" + uid + ".png", System.Drawing.Imaging.ImageFormat.Png);

            return bmp;
        }

        private void extractTexture()
        {

        }
    }

}
