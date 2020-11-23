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

//#define _NO_VERTEX_REDUCTION_

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using NthDimension.Algebra;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Geometry.Simplification;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;
using NthDimension.Collections;


namespace NthDimension.Rendering.Loaders
{
    public partial class MeshLoader : ApplicationObject, IDisposable
    {
        public ThreadSafeList<MeshVbo>     Pending = new ThreadSafeList<MeshVbo>();
        
        public ListMesh                 meshes              = new ListMesh { };
        public static Hashtable         meshesNames         = new Hashtable();

        private NumberFormatInfo        nfi             = GenericMethods.getNfi();
        Stopwatch                       swfullObj       = new Stopwatch();


        protected List<string>          CustomCacheFiles = new List<string>();

        private static bool baseModelsLoaded = false;

        #region Properties
        public Hashtable Entries
        {
            get { return meshesNames; }
        }
        #endregion

        public MeshLoader()
        {
        }

        public  void Dispose()
        {
        }

        #region Cache read/write
        public void AddCustomCacheFile(string cacheFile)
        {
            this.CustomCacheFiles.Add(cacheFile);
        }

        public void ReadCacheFile(Action<int> callback = null)
        {
            ListMesh tmpMeshes = new ListMesh();

            if (!baseModelsLoaded)
            {
                string maleCharacters   = Settings.Instance.game.modelMaleCacheFile;
                string femaleCharacters = Settings.Instance.game.modelFemaleCacheFile;
                string apparel          = Settings.Instance.game.modelApparelCacheFile;
                //string npcCharacters    = Settings.Instance.game.modelNpcCacheFile;
                string scene            = Settings.Instance.game.modelCacheFile;
                //string vehicles = Settings.Instance.game.modelVehiclesCacheFile;

                #region cacheModel.ucf
                if (File.Exists(scene))
                    using (FileStream fileStream = new FileStream(scene, FileMode.Open, FileAccess.Read))
                    {
                        try
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

                            ListMesh sceneModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                            tmpMeshes.AddRange(sceneModels);
                            ConsoleUtil.log(string.Format("\tAdded {0} scene models from {1}", sceneModels.Count, scene));
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Error reading scene cache ", mE.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                else
                    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelCacheFile)));
                #endregion

                #region cacheMale.ucf
                if (File.Exists(maleCharacters))
                    using (FileStream fileStream = new FileStream(maleCharacters, FileMode.Open, FileAccess.Read))
                    {
                        try
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

                            ListMesh characterModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                            tmpMeshes.AddRange(characterModels);
                            ConsoleUtil.log(string.Format("\tAdded {0} male character models from {1}", characterModels.Count, maleCharacters));
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Error Reading characters cache ", mE.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                else
                    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelMaleCacheFile)));
                #endregion

                #region cacheFemale.ucf
                if (File.Exists(femaleCharacters))
                    using (FileStream fileStream = new FileStream(femaleCharacters, FileMode.Open, FileAccess.Read))
                    {
                        try
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

                            ListMesh characterModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                            tmpMeshes.AddRange(characterModels);
                            ConsoleUtil.log(string.Format("\tAdded {0} female character models from {1}", characterModels.Count, femaleCharacters));
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Error Reading characters cache ", mE.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                else
                    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelFemaleCacheFile)));
                #endregion

                #region cacheApparelModel.ucf
                if (File.Exists(apparel))
                    using (FileStream fileStream = new FileStream(apparel, FileMode.Open, FileAccess.Read))
                    {
                        try
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

                            ListMesh apparelModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                            tmpMeshes.AddRange(apparelModels);
                            ConsoleUtil.log(string.Format("\tAdded {0} character apparel models from {1}", apparelModels.Count, apparel));
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Error reading apparel cache ", mE.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                else
                    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelApparelCacheFile)));
                #endregion

                #region cacheNpc.ucf
                //if (File.Exists(npcCharacters))
                //    using (FileStream fileStream = new FileStream(npcCharacters, FileMode.Open, FileAccess.Read))
                //    {
                //        try
                //        {
                //            // Read the source file into a byte array.
                //            byte[] bytes = new byte[fileStream.Length];
                //            int numBytesToRead = (int)fileStream.Length;
                //            int numBytesRead = 0;
                //            while (numBytesToRead > 0)
                //            {
                //                // Read may return anything from 0 to numBytesToRead.
                //                int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);

                //                // Break when the end of the file is reached.
                //                if (n == 0)
                //                    break;

                //                numBytesRead += n;
                //                numBytesToRead -= n;

                //                if (null != callback)
                //                    callback((int)(100 * numBytesRead / fileStream.Length));
                //            }

                //            ListMesh characterNpcModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                //            tmpMeshes.AddRange(characterNpcModels);
                //            ConsoleUtil.log(string.Format("Added {0} npc character models from {1}", characterNpcModels.Count, npcCharacters));
                //        }
                //        catch (Exception mE)
                //        {
                //            ConsoleUtil.errorlog("Error Reading npc characters cache ", mE.Message);
                //        }
                //        finally
                //        {
                //            fileStream.Close();
                //        }
                //    }
                //else
                //    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelNpcCacheFile)));
                #endregion cacheNpc.ucf


                #region cacheVehicles.ucf
                //if (File.Exists(vehicles))
                //    using (FileStream fileStream = new FileStream(vehicles, FileMode.Open, FileAccess.Read))
                //    {
                //        try
                //        {
                //            // Read the source file into a byte array.
                //            byte[] bytes = new byte[fileStream.Length];
                //            int numBytesToRead = (int)fileStream.Length;
                //            int numBytesRead = 0;
                //            while (numBytesToRead > 0)
                //            {
                //                // Read may return anything from 0 to numBytesToRead.
                //                int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);

                //                // Break when the end of the file is reached.
                //                if (n == 0)
                //                    break;

                //                numBytesRead += n;
                //                numBytesToRead -= n;

                //                if (null != callback)
                //                    callback((int)(100 * numBytesRead / fileStream.Length));
                //            }

                //            ListMesh vehicleModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                //            tmpMeshes.AddRange(vehicleModels);
                //            ConsoleUtil.log(string.Format("Added {0} vehicle models from {1}", vehicleModels.Count, apparel));
                //        }
                //        catch (Exception mE)
                //        {
                //            ConsoleUtil.errorlog("Error reading vehicles cache ", mE.Message);
                //        }
                //        finally
                //        {
                //            fileStream.Close();
                //        }
                //    }
                //else
                //    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelVehiclesCacheFile)));
                #endregion

                baseModelsLoaded = true;
            }

            #region Custom Cache Files

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

                    if (File.Exists(path))
                        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            try
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

                                ListMesh customModels = (ListMesh)GenericMethods.ByteArrayToObject<ListMesh>(bytes);
                                tmpMeshes.AddRange(customModels);
                                ConsoleUtil.log(string.Format("\tAdded {0} custom models from {1}", customModels.Count, Path.GetFileName(path)));
                            }
                            catch (Exception mE)
                            {
                                ConsoleUtil.errorlog(string.Format("\tError Reading custom cache {0}", Path.GetFileName(path)), mE.Message);
                            }
                            finally
                            {
                                fileStream.Close();
                            }
                        }
                    else
                        ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", path));
                }
            }
            #endregion Custom Cache Files

            int meshCount = tmpMeshes.Count;
            for (int i = 0; i < meshCount; i++)
            {
                MeshVbo curMesh = new MeshVbo();
                curMesh = tmpMeshes[i];

                string name = curMesh.Name;

                if (!meshesNames.ContainsKey(name))
                {
                    if (curMesh.MeshData.Indices != null)
                    {
                        curMesh.Type = MeshVbo.MeshType.FromCache;
                    }
                    else
                    {
                        curMesh.Type = MeshVbo.MeshType.Empty;
                        curMesh.IsLoaded = true;
                    }

                    int identifier = meshes.Count;
                    curMesh.Identifier = identifier;

                    meshesNames.Add(name, identifier);
                    meshes.Add(curMesh);
                    curMesh.CurrentLod = MeshVbo.MeshLod.Level0;
                }
            }

            CustomCacheFiles.Clear();
            Utilities.ConsoleUtil.log("    Loaded a Total of " + meshCount + " meshes from cache");
        }
        public void WriteCacheFile(/*string directory = "",*/ Action<string> callback = null)
        {
            string staticModelsCache    = Settings.Instance.game.modelCacheFile;
            string maleModelsChache     = Settings.Instance.game.modelMaleCacheFile;
            string femaleModelsCache    = Settings.Instance.game.modelFemaleCacheFile;
            string apparelModelCache    = Settings.Instance.game.modelApparelCacheFile;
            //string npcModelCache        = Settings.Instance.game.modelNpcCacheFile;
            string vehicleModelCache    = Settings.Instance.game.modelVehiclesCacheFile;

            ListMesh SaveList = new ListMesh { };
            List<string> sceneNames = new List<string>();

            foreach (var mesh in meshes)
            {
                if (null != callback)
                    callback(string.Format("caching mesh {0}", mesh.Name));
                
                // Prepare Cache Data /////////////////////////////////////////////////////////
                mesh.CacheMesh(ref SaveList);

                // Retrieve Scene Names /////////////////////////////////////////////////////// 
                if (mesh.Name.Contains("scenes"))
                {
                    try
                    {
                        string[] nameParts = mesh.Name.Split('\\');
                        string sceneName = nameParts[1];
                        if (!sceneNames.Contains(sceneName))
                            sceneNames.Add(sceneName);
                    }
                    catch (Exception eD)
                    {
                        ConsoleUtil.errorlog("MeshLoader.WriteCacheFile() failed to retrieve Scene name ", eD.Message);
                    }
                }
            }

            // Collect Male Character Models //////////////////////////////////////////////////
            ListMesh maleCharacters = new ListMesh(SaveList.Where(m => m.Name.Contains(@"characters\male") &&
                                                                          !m.Name.Contains(@"characters\male\apparel")).ToList());

            // Collect Female Character Models/////////////////////////////////////////////////
            ListMesh femaleCharacters = new ListMesh(SaveList.Where(m => m.Name.Contains(@"characters\female") &&
                                                                          !m.Name.Contains(@"characters\male\apparel")).ToList());

            // Collect Character Apparel Models ///////////////////////////////////////////////
            ListMesh apparel = new ListMesh(SaveList.Where(m => m.Name.Contains(@"characters\male\apparel") ||
                                                                m.Name.Contains(@"characters\female\apparel")).ToList());
            //// Collect npc Characters
            //ListMesh npcCharacters  = new ListMesh(SaveList.Where(m => m.Name.Contains(@"characters\npc")).ToList());


            // Collect Each Scene Models //////////////////////////////////////////////////////
            List<ListMesh> groupScenes = new List<ListMesh>();
            foreach (string scene in sceneNames)
            {
                string filterName = string.Format(@"scenes\{0}", scene);
                ListMesh newScene = new ListMesh(SaveList.Where(m => m.Name.Contains(filterName)).ToList()) {ListName = scene};
                groupScenes.Add(newScene);
            // Remove Scene Models from Collection ////////////////////////////////////////////
                SaveList.RemoveAll(item => newScene.Contains(item));

                ConsoleUtil.log(string.Format("+++ Seperated models for Scene {0}", scene));
            }
            // Collect Vehicle Models
            ListMesh vehicles = new ListMesh(SaveList.Where(m => m.Name.Contains(@"vehicles\")).ToList());

            // Remove Character Related Models (Bodies & Clothes) from Collection /////////////
            SaveList.RemoveAll(item => maleCharacters.Contains(item));
            SaveList.RemoveAll(item => femaleCharacters.Contains(item));
            SaveList.RemoveAll(item => apparel.Contains(item));
            //SaveList.RemoveAll(item => npcCharacters.Contains(item));
            SaveList.RemoveAll(item => vehicles.Contains(item));

            // Write Cache Files to Disk
            try
            {
                using (FileStream fileStream = new FileStream(maleModelsChache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(maleCharacters);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                        ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", maleModelsChache, maleCharacters.Count, GenericMethods.FileSizeReadable(maleModelsChache)));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write ", "Failed to store characters " + mE.Message);
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
            try
            {
                using (FileStream fileStream = new FileStream(femaleModelsCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(femaleCharacters);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                        ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", femaleModelsCache, femaleCharacters.Count, GenericMethods.FileSizeReadable(femaleModelsCache)));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write ", "Failed to store characters " + mE.Message);
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
            try
            {
                using (FileStream fileStream = new FileStream(apparelModelCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(apparel);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                        ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", apparelModelCache, apparel.Count, GenericMethods.FileSizeReadable(apparelModelCache)));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write ", "Failed to store apparel " + mE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (Exception mE)
            {
                ConsoleUtil.errorlog("Cache Write ", "Failed to store apparel " + mE.Message);
            }
            //try
            //{
            //    using (FileStream fileStream = new FileStream(npcModelCache, FileMode.Create, FileAccess.Write))
            //    {
            //        try
            //        {
            //            byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(npcCharacters);
            //            fileStream.Write(saveAry, 0, saveAry.Length);
            //            ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", maleModelsChache, maleCharacters.Count, GenericMethods.FileSizeReadable(maleModelsChache)));
            //        }
            //        catch (Exception mE)
            //        {
            //            ConsoleUtil.errorlog("Cache Write ", "Failed to store NPCs " + mE.Message);
            //        }
            //        finally
            //        {
            //            fileStream.Close();
            //        }
            //    }
            //}
            //catch (Exception mE)
            //{
            //    ConsoleUtil.errorlog("Cache Write ", "Failed to store NPCs " + mE.Message);
            //}
            try
            {
                using (FileStream fileStream = new FileStream(vehicleModelCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(vehicles);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                        ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", vehicleModelCache, vehicles.Count, GenericMethods.FileSizeReadable(vehicleModelCache)));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write ", "Failed to store vehicles " + mE.Message);
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
            try
            {
                using (FileStream fileStream = new FileStream(staticModelsCache, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(SaveList);
                        fileStream.Write(saveAry, 0, saveAry.Length);
                        ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", staticModelsCache, SaveList.Count, GenericMethods.FileSizeReadable(staticModelsCache)));
                    }
                    catch (Exception mE)
                    {
                        ConsoleUtil.errorlog("Cache Write ", "Failed to store scene " + mE.Message);
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

            foreach (ListMesh listScene in groupScenes)
            {
                string sceneCacheFile = string.Format("cacheModel{0}.ucf", listScene.ListName.ToLower());
                try
                {
                    using (FileStream fileStream = new FileStream(sceneCacheFile, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            byte[] saveAry = GenericMethods.ObjectToByteArray<ListMesh>(listScene);
                            fileStream.Write(saveAry, 0, saveAry.Length);
                            ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} models, filesize: {2}", sceneCacheFile, listScene.Count, GenericMethods.FileSizeReadable(sceneCacheFile)));
                        }
                        catch (Exception mE)
                        {
                            ConsoleUtil.errorlog("Cache Write ", "Failed to store scene " + mE.Message);
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

        public MeshVbo GetMeshByName(string name)
        {
            int id = -1;

            name = name.Trim();

            try
            {
                id = (int)meshesNames[name];
            }
            catch //(Exception me)
            {
                //throw new Exception(string.Format("Mesh Loader: Mesh not found '{0}'", name));
                ConsoleUtil.errorlog("Mesh Loader: ", string.Format("mesh '{0}' not found", name));
            }
            return meshes[id];
        }
        public void ParseFaceList(ref MeshVbo target, bool genNormal, MeshVbo.MeshLod lod, bool triangulate = true, bool reduce = true, bool smoothing = true)
        {

            ConsoleUtil.log(string.Format("    ", lod));
            ConsoleUtil.log(string.Format("                            --- {0} ---                            ", lod));
            ConsoleUtil.log(string.Format(" {0}", target.IsAnimated ? "Animated" : "Static"));
            Stopwatch sw = new Stopwatch();                                                                      // Function individual procedure stopwatch

            target.CurrentLod = lod;
            target.MeshData.LodEnabled = true;

            #region ForEach Lod

            // TODO:: Re-Validate Voxel-only usability for removeTempFaces
            #region Remove Temporary faces (voxel) and triangulate faces
            List<Vector3> positionVboDataList       = target.MeshData.PositionCache;                                // Transfer Vertex Position data from File to positionVboDataList
            List<Vector3> normalVboDataList         = target.MeshData.NormalCache;                                  // Transfer Vertex Normal data from File to normalVboDataList
            List<Vector2> textureVboDataList        = target.MeshData.TextureCache;                                 // Transfer Vertex Texture UV data from File to textureVboDataList


            float[][] boneWeightList;
            int[][] boneIdList;
            int affBones = 0;

            if (null == target.MeshData.MeshBones)
                target.MeshData.MeshBones           = new MeshBoneVboData();

            if (null != target.MeshData.MeshBones.BoneWeightList)                                                                // If Mesh contains BoneWeights transfer them to boneWeightList
                boneWeightList                      = target.MeshData.MeshBones.BoneWeightList.OfType<float[]>().ToArray();
            else
                boneWeightList = null;

            if (null != target.MeshData.MeshBones.BoneIdList)                                                                    // If Mesh Contains BoneIds transfer them to boneIdList
                boneIdList                          = target.MeshData.MeshBones.BoneIdList.OfType<int[]>().ToArray();
            else
                boneIdList = null;

            if (boneWeightList != null)                                                                                         // Count the affecting Bones
                affBones                            = boneWeightList.Length;

            List<Face> faceDataList                 = target.MeshData.Faces;                                                    // Transfer Face data from File to faceDataList

            int processedFaces = 0;


            //sw_total.Start();
            sw.Start();                                                                                                         // Start the Stopwatch

            removeTempFaces(ref faceDataList);                                                                              // Remove Temporary Faces (valid for voxel mesh) 

            if (triangulate)
            {
                //removeTempFaces(ref faceDataList);                                                                              // Remove Temporary Faces (valid for voxel mesh) 
                                                                                                                                //int facesBefore = target.MeshData.Faces.Count;
                convertToTriangularFace(ref faceDataList);                                                                      // Convert Quad Faces to Triangle Faces               
            }

            Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Triangulation    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, lod));
            //                                      @"   -Datasets         ({1})... {0}"
            #endregion

            sw.Restart();

            // TODO:: Test what happens if no normals / textures are provided from file
            #region Generate Normals/Tangents
            Vector3[] tmpnormalVboData              = new Vector3[normalVboDataList.Count];
            Vector3[] tmptangentVboData             = new Vector3[normalVboDataList.Count];
            Vector3[] normalPositionData            = new Vector3[normalVboDataList.Count];
            Vector2[] normalUvData                    = new Vector2[normalVboDataList.Count];

            List<Vector3> normalHelperList          = new List<Vector3> { };
            List<Vector3> tangentHelperList         = new List<Vector3> { };
            List<Vector2> normalUvHelperList        = new List<Vector2> { };
            List<Vector3> positionHelperlist        = new List<Vector3> { };


            int faceCount                           = faceDataList.Count;
            object locker                           = new object();

            //Parallel.For(0, faceCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            for (int i = 0; i < faceCount; i++)                                                                         // For All Faces
            {

                #region For each triangle get its three Positions / Normals /Texture UVs
                // get all the information from Lists into Facelist
                Vector3[] vposition = new Vector3[3];                                                   // Create three Vector3 array -> vposition
                Vector3[] vnormal = new Vector3[3];                                                   // Create three Vector3 array -> vnormal
                Vector2[] vtexture = new Vector2[3];                                                   // Create three Vector3 array -> vtexture

                for (int j = 0; j < 3; j++)                                                                                 // Iterrate three times -> j
                {
                    lock (locker)
                    {
                        vposition[j] = positionVboDataList[faceDataList[i].Vertex[j].Vi];                    // Get the Vertex Position Index of the j/3 vertex of Face[i] and use as index for positionVboData to put position in vposition[j]
                        if (normalVboDataList.Count > 0)
                            vnormal[j] = normalVboDataList[faceDataList[i].Vertex[j].Ni];                      // Get the Vertex Normal Index of the j/3 vertex of Face[i] and use as index for normalVboData to put normal in vnormal[j]  NOTE:: normalVboData may be null
                        if (textureVboDataList.Count > 0)
                            vtexture[j] = textureVboDataList[faceDataList[i].Vertex[j].Ti];                     // Get the Vertex Texture Index of the j/3 vertex of Face[i] and use as index for textureVboData to put texture uv in vtexture[j]  NOTE:: textureVboData may be null
                    }
                }
                #endregion

                #region Calculate Normal for triangle
                // Dir  = (B - A) x (C - A)
                // Norm = Dir / len(Dir)

                // calculating face normal and tangent
                Vector3 edge1                      = vposition[1] - vposition[0];
                Vector3 edge2                      = vposition[2] - vposition[0];

                Vector3 fnormal                    = Vector3.Cross(edge1, edge2);                                //.Normalized();  // ALL Normals MUST be Normalized [0-1] prior to importing to the engine 
                #endregion

                #region Calculate Tangent for triangle
                Vector2 vtexture1 = vtexture[1] - vtexture[0];
                Vector2 vtexture2 = vtexture[2] - vtexture[0];

                float s = 1f / (vtexture2.X - vtexture1.X * vtexture2.Y / vtexture1.Y);
                float r = 1f / (vtexture1.X - vtexture2.X * vtexture1.Y / vtexture2.Y);

                Vector3 tangent = Vector3.Normalize(r * edge1 + s * edge2);

                if (tangent == Vector3.Zero)
                    Utilities.ConsoleUtil.errorlog("   ERROR: ", "Tangent Vector is Zero");
                #endregion

                #region Normal / Tangent Smoothing

                if (smoothing)
                {
                    Face curFace = faceDataList[i];                                                  // Put current face to curFace
                    for (int j = 0; j < 3; j++)                                                                                 // Iterate three times -> j (for each Vertex in triangle)
                    {
                        lock (locker)
                        {
                            #region For each Vertex in Triangle

                            VertexIndex curVert = curFace.Vertex[j];                                                // Put j/3 triangle vertex into curVert

                            // if Normal[Normalindice] has not been assigned a uv coordinate do so and set normal
                            if (/*normalUvData.Length > 0 &&*/ normalUvData[curVert.Ni] == Vector2.Zero)
                            {
                                normalUvData[curVert.Ni] = vtexture[j];
                                normalPositionData[curVert.Ni] = vposition[j];

                                tmpnormalVboData[curVert.Ni] = fnormal;
                                tmptangentVboData[curVert.Ni] = tangent;
                            }
                            else
                            {
                                // if Normal[Normalindice] is of the same Uv and place simply add
                                if (/*normalUvData.Length > 0 &&*/ normalUvData[curVert.Ni] == vtexture[j] &&
                                    normalPositionData[curVert.Ni] == vposition[j])
                                {
                                    tmpnormalVboData[curVert.Ni] += fnormal;
                                    tmptangentVboData[curVert.Ni] += tangent;
                                }
                                else
                                {
                                    for (int k = 0; k < normalUvHelperList.Count; k++)
                                    {
                                        // if Normalhelper[Normalindice] is of the same Uv and position simply add
                                        if (normalUvHelperList[k] == vtexture[j] &&
                                            positionHelperlist[k] == vposition[j])
                                        {
                                            tangentHelperList[k] += tangent;
                                            normalHelperList[k] += fnormal;

                                            curVert.Normalihelper = k;
                                        }
                                    }
                                    // if matching Normalhelper has not been found create new one
                                    if (faceDataList[i].Vertex[j].Normalihelper == -1)
                                    {
                                        normalUvHelperList.Add(vtexture[j]);
                                        tangentHelperList.Add(tangent);
                                        normalHelperList.Add(fnormal);
                                        positionHelperlist.Add(vposition[j]);
                                        curVert.Normalihelper = normalUvHelperList.Count - 1;
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
                #endregion

                lock (locker)
                {
                    processedFaces++;
                    string remainingTan = (faceCount - processedFaces) > 0
                        ? string.Format(" Remaining: {0}/{1} ", faceCount - processedFaces, faceCount)
                        : string.Format(" Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine);
                    Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Normals/Tangents ({1}):{0}", remainingTan, lod), false);
                    //                                      @"   -Datasets         ({1})... {0}"
                }
            }
            //);
            #endregion

            sw.Restart();

            #region Prepare Datasets
            // put Faces into DataSets (so we can easyly compare them)
            List<FaceDataSet> faceDataSet = new List<FaceDataSet> { };

            for (int i = 0; i < faceCount; i++)
            //Parallel.For(0, faceCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                //lock (locker)
                //{
                Face curFace = faceDataList[i];

                for (int j = 0; j < 3; j++)
                {

                    try
                    {
                        #region try-block

                        VertexIndex oldVert = curFace.Vertex[j];
                        FaceDataSet curVert = new FaceDataSet();
                        curVert.position = positionVboDataList[oldVert.Vi];

                        if (normalVboDataList.Count > 0)
                            curVert.normal = normalVboDataList[oldVert.Ni];

                        if (oldVert.Normalihelper != -1)
                        {
                            if (genNormal)
                                curVert.normal = Vector3.Normalize(normalHelperList[oldVert.Normalihelper]); //-dont use calculated normal
                            curVert.tangent = Vector3.Normalize(tangentHelperList[oldVert.Normalihelper]);
                        }
                        else
                        {
                            if (genNormal)
                                curVert.normal = Vector3.Normalize(tmpnormalVboData[oldVert.Ni]);               //-use calculated normal
                            curVert.tangent = Vector3.Normalize(tmptangentVboData[oldVert.Ni]);
                        }
                        if (affBones > 0)
                        {
                            curVert.boneWeight = new float[affBones];
                            curVert.boneId = new int[affBones];
                        }

                        for (int k = 0; k < affBones; k++)
                        {
                            try
                            {
                                if (oldVert.Vi == boneWeightList[k].Length)
                                    oldVert.Vi--;

                                curVert.boneWeight[k] = boneWeightList[k][oldVert.Vi];
                                curVert.boneId[k] = boneIdList[k][oldVert.Vi];
                            }
                            catch //(IndexOutOfRangeException ie)
                            {
                                // Some models throw IndexOutOfRange Exceptions, investigate Collada format
                            }
                        }

                        if (textureVboDataList.Count > 0)
                            curVert.texture = textureVboDataList[oldVert.Ti];

                        #endregion
                        faceDataSet.Add(curVert);
                    }
                    catch //(IndexOutOfRangeException e)
                    {

                        // Some oldVert.Vi indices are pointing to the last boneWeightList[k] index ie oldVert.vi = boneWeightList[k].Count + 1!!!! this throws
                        // and index out of range exception

                        //for (int k = 0; k < affBones; k++)
                        //{
                        //    curVert.boneWeight[k] = boneWeightList[k][oldVert.Vi];
                        //    curVert.boneId[k] = boneIdList[k][oldVert.Vi];
                        //}
                    }



                }

                string remainingFaces = (faceCount - i) > 1
                        ? string.Format(" Remaining: {0}/{1} ", (faceCount - i).ToString(), faceCount.ToString())
                        : string.Format(" Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Datasets         ({1}):{0}", remainingFaces, lod), false);

                //}

            }
            #endregion

            sw.Restart();

            #region Reduce/Remove vertices
            ////Remove unneded verts
            int noVerts = faceDataSet.Count;
            List<FaceDataSet> newVertList = new List<FaceDataSet> { };
            List<int> newIndiceList = new List<int> { };

          

                for (int i = 0; i < noVerts; i++)
                ////Parallel.For(0, noVerts, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
                {
                    ////lock (locker)

                    FaceDataSet curVert = faceDataSet[i];

                    if (reduce && Settings.Instance.game.usePolygonReduction)
                    {
                        int curNewVertCount = newVertList.Count;
                        int index = -1;

                        for (int j = curNewVertCount - 1; j >= 0; j--)
                            if (newVertList[j].Equals(curVert))
                                index = j;

                        if (index < 0)
                        {
                            index = curNewVertCount;
                            newVertList.Add(curVert);
                        }

                        newIndiceList.Add(index);

                        string remainingVerts = (noVerts - i) > 1
                            ? string.Format(" Remaining: {0}/{1} ", (noVerts - i), noVerts)
                            : string.Format(" Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine);
                        Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Vertex reduction ({1}):{0}", remainingVerts, lod), false);
                    }
                    else
                    {
                        string skippingVerts = (noVerts - i) > 1
                            ? string.Format(" Remaining: {0}/{1} ", (noVerts - i), noVerts)
                            : string.Format(" Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine);
                        Utilities.ConsoleUtil.log(string.Format("   " + ConsoleUtil.bulletWhite + "Skipping vertices ({1}):{0}", skippingVerts, lod), false);
                        newVertList.Add(curVert);
                        newIndiceList.Add(i);
                    }

                }
                //Utilities.ConsoleUtil.log(string.Format("   -New vertex and index lists created\t\t\t\t\t[{0} ms]", sw.ElapsedMilliseconds));



                sw.Restart();
            
            

            #endregion

            #region Put Face data into Arrays
            int newIndiceCount = newIndiceList.Count;
            int[] indicesVboData = new int[newIndiceCount];

            int newVertCount = newVertList.Count;
            Vector3[] positionVboData = new Vector3[newVertCount];
            Vector3[] normalVboData = new Vector3[newVertCount];
            Vector3[] tangentVboData = new Vector3[newVertCount];
            Vector2[] textureVboData = new Vector2[newVertCount];

            int[][] boneIdVboData = new int[affBones][];
            float[][] boneWeightVboData = new float[affBones][];

            for (int i = 0; i < affBones; i++)
            {
                boneIdVboData[i] = new int[newVertCount];
                boneWeightVboData[i] = new float[newVertCount];
            }
            #endregion

            #region Re-Create Vbo data



            //Parallel.For(0, newVertCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            for (int i = 0; i < newVertCount; i++)
            {
                lock (locker)
                {
                    FaceDataSet curVert = newVertList[i];

                    positionVboData[i] = curVert.position;
                    normalVboData[i] = curVert.normal;
                    tangentVboData[i] = curVert.tangent;
                    textureVboData[i] = curVert.texture;

                    for (int k = 0; k < affBones; k++)
                    {
                        boneWeightVboData[k][i] = curVert.boneWeight[k];
                        boneIdVboData[k][i] = curVert.boneId[k];
                    }
                    string bVbo = (newVertCount - i) > 1
                        ? string.Format(" Remaining: {0}/{1} ", (newVertCount - i).ToString(), newVertCount)
                        : string.Format(" Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} ", sw.ToHumanReadable());
                    Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Vbo construction ({1}):{0}", bVbo, lod), false);
                    //                                          @"   -Datasets         ({1})... {0}"
                }

            }
            //);
            #endregion

            sw.Stop();

            #region Re-Establish Indices
            for (int i = 0; i < newIndiceCount; i++)
                indicesVboData[i] = newIndiceList[i];
            #endregion

            #region Calculate Bounding Sphere
            //calculate a bounding Sphere
            if (lod == MeshVbo.MeshLod.Level0)
            {
                float sphere = 0;

                foreach (var vec in positionVboData)
                {
                    float length = vec.LengthFast;
                    if (length > sphere)
                        sphere = length;
                }

                target.BoundingSphere = sphere;
            }
            #endregion

            #region Pass Vbo Indices
            //deleting unneded
            //target.MeshData.PositionCache = null;     // Disabled -> To be used by Simplify
            //target.MeshData.NormalCache = null;
            target.MeshData.Tangents = null;
            target.MeshData.Indices = null;

            target.MeshData.MeshBones.BoneWeightList = null;
            target.MeshData.MeshBones.BoneIdList = null;

            //returning mesh info ... DONE :D
            target.MeshData.Positions = positionVboData;
            target.MeshData.Normals = normalVboData;
            target.MeshData.Tangents = tangentVboData;
            target.MeshData.Textures = textureVboData;
            target.MeshData.Indices = indicesVboData;

            target.MeshData.MeshBones.BoneIds = new int[affBones][];
            target.MeshData.MeshBones.BoneWeights = new float[affBones][];
            for (int i = 0; i < affBones; i++)
            {
                target.MeshData.MeshBones.BoneIds[i] = boneIdVboData[i];
                target.MeshData.MeshBones.BoneWeights[i] = boneWeightVboData[i];
            }



            #endregion

            #endregion


        }
        public void GenerateVBO(ref MeshVbo target, MeshVbo.MeshLod lodLevel)
        {
            int affectingBonesCount = 0;
            target.CurrentLod = lodLevel;

            if(null != target.MeshData.MeshBones)
            if (/*target.CurrentLod == Mesh.MeshLod.Level0 &&*/  
                target.MeshData.MeshBones.BoneWeights != null)
            {
                affectingBonesCount = target.MeshData.MeshBones.BoneWeights.Length;

                target.MeshData.MeshBones.AffectingBonesCount   = affectingBonesCount;
                target.MeshData.MeshBones.BoneWeigthHandles     = new int[affectingBonesCount];
                target.MeshData.MeshBones.BoneIdHandles         = new int[affectingBonesCount];
            }

#if _WINDOWS_
            if (!target.MeshData.ContainsVbo)
            {
                
                #region 1. Generate Buffers and Acquire their Indices

                int normal, position, texture, tangent, ebo = -1;

                ApplicationBase.Instance.Renderer.GenBuffers(1, out normal);
                ApplicationBase.Instance.Renderer.GenBuffers(1, out position);
                ApplicationBase.Instance.Renderer.GenBuffers(1, out texture);
                ApplicationBase.Instance.Renderer.GenBuffers(1, out tangent);
                ApplicationBase.Instance.Renderer.GenBuffers(1, out ebo);

                target.MeshData.NormalHandle = normal;
                target.MeshData.PositionHandle = position;
                target.MeshData.TextureHandle = texture;
                target.MeshData.TangentHandle = tangent;
                target.MeshData.EboHandle = ebo;

                if (affectingBonesCount > 0)
                {
                    ApplicationBase.Instance.Renderer.GenBuffers(affectingBonesCount, target.MeshData.MeshBones.BoneWeigthHandles); // !out
                    ApplicationBase.Instance.Renderer.GenBuffers(affectingBonesCount, target.MeshData.MeshBones.BoneIdHandles); // !out
                }
                #endregion
            }

            #region 2. Bind Arrays and Buffer Data
            // NORMALS
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.NormalHandle);
            ApplicationBase.Instance.Renderer.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(target.MeshData.Normals.Length*Vector3.SizeInBytes), target.MeshData.Normals, BufferUsageHint.StaticDraw);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create NormalBuffer {0}:{1}", this.GetType(), Name));
            // POSITIONS
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.PositionHandle);
            ApplicationBase.Instance.Renderer.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(target.MeshData.Positions.Length*Vector3.SizeInBytes), target.MeshData.Positions, BufferUsageHint.StaticDraw);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create PositionBuffer {0}:{1}", this.GetType(), Name));
            // TEXTURES
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.TextureHandle);
            ApplicationBase.Instance.Renderer.BufferData<Vector2>(BufferTarget.ArrayBuffer, new IntPtr(target.MeshData.Textures.Length*Vector2.SizeInBytes), target.MeshData.Textures, BufferUsageHint.StaticDraw);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create UVBuffer {0}:{1}", this.GetType(), Name));
            // TANGENTS
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.TangentHandle);
            ApplicationBase.Instance.Renderer.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(target.MeshData.Tangents.Length*Vector3.SizeInBytes), target.MeshData.Tangents, BufferUsageHint.StaticDraw);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create TangentBuffer {0}:{1}", this.GetType(), Name));
            // BONES
            //if (target.CurrentLod == Mesh.MeshLod.Level0)
            //{
            for (int bone = 0; bone < affectingBonesCount; bone++)
            {
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.MeshBones.BoneWeigthHandles[bone]);
                ApplicationBase.Instance.Renderer.BufferData<float>(BufferTarget.ArrayBuffer,
                    new IntPtr(target.MeshData.MeshBones.BoneWeights[bone].Length*sizeof (float)),
                    //target.MeshData.MeshBones.BoneWeights[bone], BufferUsageHint.StaticDraw);
                    target.MeshData.MeshBones.BoneWeights[bone], BufferUsageHint.DynamicDraw);          // Modofied Apr-24-18 (Optimizing Draw Calls attempt)

                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, target.MeshData.MeshBones.BoneIdHandles[bone]);
                ApplicationBase.Instance.Renderer.BufferData<int>(BufferTarget.ArrayBuffer,
                    new IntPtr(target.MeshData.MeshBones.BoneIds[bone].Length*sizeof (int)), target.MeshData.MeshBones.BoneIds[bone],
                    //BufferUsageHint.StaticDraw);
                    BufferUsageHint.DynamicDraw);                                                       // Modofied Apr-24-18 (Optimizing Draw Calls attempt)
                ApplicationBase.Instance.CheckGlError(
                    string.Format("(!) OpenGL Error: Create VertexGroupBuffer {0}:{1} Group Id: {2}", this.GetType(),
                        Name, bone));
            }
            //}
            // ELEMENTS
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, target.MeshData.EboHandle);
            ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof (uint)*target.MeshData.Indices.Length), target.MeshData.Indices, BufferUsageHint.StaticDraw);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create IndexBuffer {0}:{1}", this.GetType(), Name));
            #endregion

            #region 3. Reset (Unbind Buffers)
            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, 0);
            #endregion
#endif
            // --- causes crash ---
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            target.MeshData.ContainsVbo = true;
        }
        public void LoadMeshes(Action<float> callback = null)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                if (!meshes[i].IsLoaded)
                    loadMeshByType(meshes[i]);

                if (null != callback)
                    callback((float)i / (float)meshes.Count);
            }
        }

        //public void RebuildVboAll()
        //{
        //    for (int i = 0; i < meshes.Count; i++)
        //    {
        //        meshes[i].IsLoaded = false;
        //        loadMeshByType(meshes[i]);
        //    }
        //}

        private void loadCached(MeshVbo target)
        {
            ConsoleUtil.log(string.Format("<> Loading mesh from cache: {0}", String.IsNullOrEmpty(target.Name) ? target.Pointer : target.Name));
            target.IsLoaded = true;

            GenerateVBO(ref target, MeshVbo.MeshLod.Level0);

            try { GenerateVBO(ref target, MeshVbo.MeshLod.Level1); } catch { }
            try { GenerateVBO(ref target, MeshVbo.MeshLod.Level2); } catch { }
            try { GenerateVBO(ref target, MeshVbo.MeshLod.Level3); } catch { }

            meshes[target.Identifier] = target;

            //if (target.Name.Contains(".xmd"))
            //{
            //    string output = string.Format("D:\\{0}.obj", target.Name.Replace("\\", "-"));
            //    AutodeskHelper.SaveToObj(target.GetMeshData(Mesh.MeshLod.Level0), output);
            //    ConsoleUtil.log(string.Format("Saved {0} at {1}", target.Name, output));
            //}

        }
        private void loadMeshByType(MeshVbo curMesh)
        {
            switch (curMesh.Type)
            {
                case MeshVbo.MeshType.Generated:
                    curMesh.CurrentLod = MeshVbo.MeshLod.Level0;
                    break;
                case MeshVbo.MeshType.Obj:
                    curMesh.CurrentLod = MeshVbo.MeshLod.Level0;
                    try { loadObj(curMesh); } catch (Exception lE) { ConsoleUtil.errorlog("OBJ Loader ", string.Format("Failed to read {0}. Reason {1}", curMesh.Name, lE.Message)); }
                    break;
               case MeshVbo.MeshType.ColladaGeometry:
                    try { loadColladaGeometry(ref curMesh); } catch (Exception lE) { ConsoleUtil.errorlog("OBJ Loader ", string.Format("Failed to read {0}. Reason {1}", curMesh.Name, lE.Message)); }
                    break;
                case MeshVbo.MeshType.FromCache:
                    loadCached(curMesh);
                    break;
                //case MeshVbo.MeshType.ColladaManaged:
                //    loadColladaAnimation(curMesh);
                    break;
                default:
                    break;
            }

            Pending.Add(curMesh);
        }      
        private void removeTempFaces(ref List<Face> FaceList)
        {
            int i = 0;
            while (i < FaceList.Count)
            {
                Face curFace = FaceList[i];
                if (curFace.isTemp)
                    FaceList.Remove(curFace);
                else
                    i++;
            }
        }
        private void convertToTriangularFace(ref List<Face> FaceList)
        {
            int faces = FaceList.Count;
            for (int i = 0; i < faces; i++)
            {
                Face curFace = FaceList[i];

                if (curFace.Vertex.Length > 3)
                {
                    FaceList[i] = new Face(curFace.Vertex[0], curFace.Vertex[1], curFace.Vertex[2]);
                    FaceList.Add(new Face(curFace.Vertex[2], curFace.Vertex[3], curFace.Vertex[0]));
                }
            }
        }
        private void simplifyMesh(ref MeshVbo target, MeshVbo.MeshLod lodLevel, int targetFaceCount)
        {
            PairContract pc = new PairContract(new MeshSettings());

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ConsoleUtil.log(string.Format("   -- Simplifying mesh {0}", target.Name));

            try
            {
                pc.Simplify(ref target, lodLevel, targetFaceCount);
            }
            catch //(Exception se)
            {
                ConsoleUtil.errorlog("Mesh Simplification ", string.Format("Failed {0} {1}", target.Name, lodLevel));
            }
            sw.Stop();
            //ConsoleColor cc = Console.ForegroundColor;
            //Console.ForegroundColor = ConsoleColor.Blue;
            ConsoleUtil.log(string.Format(@"   -- Mesh reduction @ {1}: Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0}", sw.ToHumanReadable(), lodLevel));
            //Console.ForegroundColor = cc;
        }


        public MeshVbo FromMesh(Vector3[] vertex, Vector3[] normal, Vector2[] textureUv, Face[] polygon)
        {
            ListVector3 pos = new ListVector3(); pos.AddRange(vertex);
            ListVector3 nor = new ListVector3(); nor.AddRange(normal);
            ListVector2 tex = new ListVector2(); tex.AddRange(textureUv);
            ListFace    tri = new ListFace();    tri.AddRange(polygon);

            return FromMesh(pos, nor, tex, tri);
        }

        public MeshVbo FromMesh(ListVector3 vertex, ListVector3 normal, ListVector2 textureUv, ListFace polygon, 
                                string name = "//Generated//", 
                                bool genNormal = true, 
                                bool triangulate = false, 
                                bool reduce = false, 
                                bool smoothing = false)
        {
            swfullObj.Start();

            MeshVbo ret = this.createMesh(vertex, normal, textureUv, polygon, genNormal, triangulate, reduce, smoothing);
            ret.Name = name;
            ret.Type = MeshVbo.MeshType.Generated;
            ret.Pointer = string.Empty;
            ret.CurrentLod = MeshVbo.MeshLod.Level0;
            ret.IsLoaded = true;

            if (ret.Identifier != -1)
            {
                if(meshes.Count == ret.Identifier + 1)
                    meshes[ret.Identifier] = ret;
                else
                    meshes.Add(ret);
            }

            swfullObj.Stop();

            return ret;
        }
        private MeshVbo createMeshLods(ListVector3 positionData, ListVector3 normalData, ListVector2 textureData, ListFace faceData)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            swfullObj.Start();

            MeshVbo target = new MeshVbo();
            target.Identifier = meshes.Count;
            target.IsAnimated = false;

            Utilities.ConsoleUtil.log(string.Format("<>  Loading solid mesh: {0} polygon(s), please wait...", faceData.Count));

            if (null == target.MeshData.PositionCache)
                target.MeshData.PositionCache = new ListVector3();
            if (null == target.MeshData.NormalCache)
                target.MeshData.NormalCache = new ListVector3();
            if (null == target.MeshData.TextureCache)
                target.MeshData.TextureCache = new ListVector2();
            if (null == target.MeshData.Faces)
                target.MeshData.Faces = new ListFace();

            #region saveToTempCache
            target.MeshData.PositionCache.AddRange(positionData);
            target.MeshData.NormalCache.AddRange(normalData);
            target.MeshData.TextureCache.AddRange(textureData);
            target.MeshData.Faces.AddRange(faceData);
            #endregion

            ParseFaceList(ref target, true, MeshVbo.MeshLod.Level0);
            GenerateVBO(ref target, MeshVbo.MeshLod.Level0);
            Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level0));
            //                                      @"   -Datasets         ({1})... {0}"

            #region Lod Levels 1-3
            int l0F = 0, l1F = 0, l2F = 0, l3F = 0, l0V = 0, l1V = 0, l2V = 0, l3V = 0;

            l0F = target.MeshData.Faces.Count;
            l0V = target.MeshData.Positions.Length;
            sw.Restart();

            int level1 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedOne)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticOne);

            int level2 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedTwo)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticTwo);

            int level3 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedThree)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticThree);


            if (level1 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level1;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level1, level1);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level1);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level1);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level1));
                //                                           Polygon construction
                l1F = target.MeshData.Faces.Count;
                l1V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }

            if (level2 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level2;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level2, level2);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level2);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level2);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level2));
                l2F = target.MeshData.Faces.Count;
                l2V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }

            if (level3 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level3;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level3, level3);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level3);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level3);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level3));
                l3F = target.MeshData.Faces.Count;
                l3V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }
            #endregion

            target.CurrentLod = MeshVbo.MeshLod.Level0;
            target.IsLoaded = true;



            #region Display Diagnostics

            string ttime = swfullObj.ToHumanReadable();

            //string ttime =  string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); 
            ConsoleColor cc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level0, l0F, l0V));
            if (level1 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level1, l1F, l1V));
            if (level2 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level2, l2F, l2V));
            if (level3 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level3, l3F, l3V));
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Utilities.ConsoleUtil.log(string.Format("{0}   Waveform complete! Total time\t\t\t\t\t[{1}] {2}",
                Environment.NewLine, ttime, Environment.NewLine));
            Console.ForegroundColor = cc;
            #endregion

            return target;
        }
        private MeshVbo createMesh(ListVector3 positionData, ListVector3 normalData, ListVector2 textureData, ListFace faceData,
                                    bool genNormal = true, bool triangulate = false, bool reduce = false, bool smoothing = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            swfullObj.Start();

            MeshVbo target = new MeshVbo();
            target.Identifier = meshes.Count;
            target.IsAnimated = false;

            Utilities.ConsoleUtil.log(string.Format("<>  Loading procedural mesh: {0} polygon(s), please wait...", faceData.Count));

            if (null == target.MeshData.PositionCache)
                target.MeshData.PositionCache = new ListVector3();
            if (null == target.MeshData.NormalCache)
                target.MeshData.NormalCache = new ListVector3();
            if (null == target.MeshData.TextureCache)
                target.MeshData.TextureCache = new ListVector2();
            if (null == target.MeshData.Faces)
                target.MeshData.Faces = new ListFace();

            #region saveToTempCache
            target.MeshData.PositionCache.AddRange(positionData);
            target.MeshData.NormalCache.AddRange(normalData);
            target.MeshData.TextureCache.AddRange(textureData);
            target.MeshData.Faces.AddRange(faceData);
            #endregion

            ParseFaceList(ref target, genNormal, MeshVbo.MeshLod.Level0, triangulate, reduce, smoothing);
            GenerateVBO(ref target, MeshVbo.MeshLod.Level0);
            
            Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level0));
            //                                      @"   -Datasets         ({1})... {0}"

            

            target.CurrentLod = MeshVbo.MeshLod.Level0;
            target.IsLoaded = true;



            #region Display Diagnostics

            string ttime = swfullObj.ToHumanReadable();

            //string ttime =  string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); 
            ConsoleColor cc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Utilities.ConsoleUtil.log(string.Format("{0}   Procedural mesh complete! Total time\t\t\t\t\t[{1}] {2}",
                Environment.NewLine, ttime, Environment.NewLine));
            Console.ForegroundColor = cc;
            #endregion

            return target;
        }

    }
}
