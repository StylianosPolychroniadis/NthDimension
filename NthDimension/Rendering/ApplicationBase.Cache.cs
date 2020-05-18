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

namespace NthDimension.Rendering
{
    using System;
    using System.IO;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Loaders;
    using NthDimension.Rendering.Utilities;

    public partial class ApplicationBase
    {
        #region READ
        private void readMeshesCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.modelCacheFile;

            #region Models Cache
            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(Settings.Instance.game.modelCacheFile)) && fi.Length > 0)
            {
                MeshLoader.ReadCacheFile();
                ConsoleUtil.log(String.Format("(*) Mesh cache file {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            }
            //else
            //    ConsoleUtil.errorlog("READ CACHE", string.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(Settings.Instance.game.modelCacheFile)));


            #endregion
        }
        private void readAnimationsCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.modelAnimationCacheFile;
            
            #region Animations Cache

            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(Settings.Instance.game.modelAnimationCacheFile)) && fi.Length > 0)
            //{
                AnimationLoader.ReadCacheFile();
                ConsoleUtil.log(String.Format("(*) Animation cache file {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            //}
            //else
            //    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(cacheFile)));
            #endregion
        }
        private void readShadersCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.shaderCacheFile;

            #region Shaders Cache
            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(cacheFile)) && fi.Length > 0)
            {
                ShaderLoader.ReadCacheFile();
                ConsoleUtil.log(String.Format("(*) Shader cache file {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            }
            //else
            //    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(cacheFile)));
            #endregion
        }
        private void readTexturesCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.textureCacheFile;

            #region Textures Cache
            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(cacheFile)) && fi.Length > 0)
            //{
                TextureLoader.ReadCacheFile();
                ConsoleUtil.log(String.Format("(*) Texture cache file {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            //}
            //else
            //    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(cacheFile)));

            #endregion
        }
        private void readMaterialsCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.materialCacheFile;

                #region Materials Cache
            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(cacheFile)) && fi.Length > 0)
            //{
                MaterialLoader.ReadCacheFile();
                ConsoleUtil.log(String.Format("(*) Material cache file {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            //}
            //else
            //    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(cacheFile)));

            #endregion
        }
        private void readTemplatesCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.templateCacheFile;

            #region Templates Cache
            //FileInfo fi = new FileInfo(Path.GetFileName(cacheFile));
            //if (File.Exists(Path.GetFileName(cacheFile)) && fi.Length > 0)
            //{
                TemplateLoader.readCacheFile();
                ConsoleUtil.log(String.Format("(*) Template cache {0} read in {1} ms", Path.GetFileName(cacheFile), VAR_StopWatch.ElapsedMilliseconds));
            //}
            //else
            //    ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(cacheFile)));
            #endregion
        }
        #endregion READ

        #region WRITE
        private void writeMeshesCache(string cacheFile = "")
        {
            if (String.IsNullOrEmpty(cacheFile))
                cacheFile = Settings.Instance.game.modelCacheFile;

            #region WRITE CACHE FILES - Models

            if (Settings.Instance.game.generateCache)
            {
                VAR_StopWatch.Start();
                ConsoleUtil.log("============ Writing Cache File ===========");


                MeshLoader.WriteCacheFile();                                                                    // TODO:: SPECIFY FILE
                ConsoleUtil.log(String.Format(" Mesh cache file {0} written in {1} ms",
                    Path.GetFileName(cacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));

                VAR_StopWatch.Restart();
            }

            #endregion
        }
        private void writeAnimationsCache()
        {

            #region WRITE CACHE FILES - Animations
            if (Settings.Instance.game.generateCache)
            {
                VAR_StopWatch.Start();
                AnimationLoader.WriteCacheFile();
                ConsoleUtil.log(String.Format(" Animation cache file {0} written in {1} ms",
                    Path.GetFileName(Settings.Instance.game.modelAnimationCacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));
                VAR_StopWatch.Restart();

            }
            #endregion 
        }
        private void writeTexturesCache()
        {
            #region WRITE CACHE FILES - Textures

            if (Settings.Instance.game.generateCache)
            {
                TextureLoader.WriteCacheFile();
                ConsoleUtil.log(String.Format(" Texture cache file {0} written in {1} ms",
                    Path.GetFileName(Settings.Instance.game.textureCacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));
                VAR_StopWatch.Restart();
            }

            #endregion
        }
        public void writeShadersCache()
        {
            #region WRITE CACHE FILES - Shaders

            if (Settings.Instance.game.generateCache)
            {
                ShaderLoader.WriteCacheFile();
                ConsoleUtil.log(String.Format(" Shader cache file {0} written in {1} ms",
                    Path.GetFileName(Settings.Instance.game.shaderCacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));
                VAR_StopWatch.Restart();
            }

            #endregion 
        }
        public void writeMaterialsCache()
        {
            #region WRITE CACHE FILES - Materials

            if (Settings.Instance.game.generateCache)
            {
                MaterialLoader.WriteCacheFile();
                ConsoleUtil.log(String.Format(" Material cache file {0} written in {1} ms",
                    Path.GetFileName(Settings.Instance.game.materialCacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));
                VAR_StopWatch.Restart();
            }

            #endregion
        }
        private void writeTemplatesCache()
        {
            #region WRITE CACHE FILES -Templates

            if (Settings.Instance.game.generateCache)
            {
                TemplateLoader.writeCacheFile();
                ConsoleUtil.log(String.Format(" Template cache {0} written in {1} ms",
                    Path.GetFileName(Settings.Instance.game.templateCacheFile),
                    VAR_StopWatch.ElapsedMilliseconds));
                VAR_StopWatch.Stop();
                ConsoleUtil.log("===========================================");
            }

            #endregion
        }
        #endregion WRITE
    }
}
