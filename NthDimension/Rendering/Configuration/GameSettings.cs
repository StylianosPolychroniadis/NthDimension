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

namespace NthDimension.Rendering.Configuration
{
    public class GameSettings
    {
        public bool         dontShowSettings;
        public bool         debugMode;

        public bool         generateCache               = false;
        public bool         useCache                    = true;

        public bool         usePolygonReduction         = true;
        public bool         invertMouseY                = false;
        public bool         rememberMe                  = false;

        public bool         diagnostics                 = false;
        public bool         diagnosticsFrame            = false;

        public string       defaultUser                 = string.Empty;
        public string       defaultPassword             = string.Empty;



        //public bool         logConsole              = true;

        public string       lastCacheHash               = string.Empty;
        public string       modelCacheFile              = "cacheModel.ucf";
        public string       modelMaleCacheFile          = "cacheMale.ucf";
        public string       modelFemaleCacheFile        = "cacheFemale.ucf";
        public string       modelNpcCacheFile           = "cacheNpc.ucf";
        public string       modelAnimationCacheFile     = "cacheAnimation.ucf";
        public string       modelApparelCacheFile       = "cacheApparel.ucf";
        public string       modelVehiclesCacheFile      = "cacheVehicles.ucf";
        public string       materialCacheFile           = "cacheMaterial.ucf";
        public string       materialVehiclesCacheFile   = "cacheVehiclesMat.ucf";
        public string       shaderCacheFile             = "cacheShader.ucf";
        public string       templateCacheFile           = "cacheTemplate.ucf";
        public string       textureCacheFile            = "cacheTexture.ucf";
        public string       textureVehiclesCacheFile    = "cacheVehiclesTex.ucr";

        public static string ModelFolder                = "data\\models\\";
        public static string ModelBaseFolder            = "data\\models\\base\\";
        public static string ModelCharacterFolder       = "data\\models\\characters\\";
        public static string ModelSceneFolder           = "data\\models\\scenes\\";
        public static string ModelsSkyboxFolder         = "data\\models\\skybox\\";
        public static string ModelfirstpersonFolder         = "data\\models\\firstperson\\";

        public static string MaterialFolder             = "data\\materials\\";
        public static string MaterialDefShadingFolder   = "data\\materials\\defShading\\";
        public static string MaterialHudFolder          = "data\\materials\\hud\\";
        public static string MaterialParticlesFolder    = "data\\materials\\particles\\";
        public static string MaterialSceneFolder        = "data\\materials\\scenes\\";
        public static string MaterialTempFolder         = "data\\materials\\tmp\\";


        public static string ShaderFolder               = "data\\shaders\\";
        public static string ShaderSceneFilterFolder    = "data\\shaders\\SceneFilters\\";

        public static string TemplateFolder             = "data\\templates\\";

        public static string TextureFolder              = "data\\textures\\";
        public static string TextureBaseFolder          = "data\\textures\\base\\";
        public static string TextureCharacterFolder     = "data\\textures\\characters\\";
        public static string TextureHudFolder           = "data\\textures\\hud\\";
        public static string TextureParticleFolder      = "data\\textures\\particles\\";
        public static string TextureSceneFolder         = "data\\textures\\scenes\\";
        public static string TextureSkyboxFolder        = "data\\textures\\skybox\\";
        public static string TextureFirstPersonFolder        = "data\\textures\\firstperson\\";
        public static string TextureTempFolder          = "data\\textures\\tmp\\";

        public static string VideoFolder                = "data\\video\\";

        public static string AudioFolder                = "data\\audio\\";

        /// <summary>
        /// The default extension for model cache file. All Autodesk .obj files and Collada .dae files
        /// </summary>
        public static string EXTENSION_CacheFileModelAnimation  = ".nani";
        public static string EXTENSION_CacheFileModel           = ".ngeo";
        public static string EXTENSION_CacheFileTexture         = ".ntex";
        public static string EXTENSION_CacheFileMaterial        = ".nmat";
        public static string EXTENSION_CacheFileShader          = ".nucs";
        public static string EXTENSION_CacheFileTemplate        = ".ntpl";

        public static string EXTENSION_SCENE_File               = ".n";

    }
}
