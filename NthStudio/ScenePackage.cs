using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public sealed class ScenePackage
    {
        public const string NodeScenePackage = "package";
        public const string NodeScenePackageIdentifier = "identifier";
        public const string NodeScenePackageSceneFile = "scenefile";
        public const string NodeScenePackageModelsFile = "modelsfile";
        public const string NodeScenePackageTexturesFile = "texturesfile";
        public const string NodeScenePackageMaterialsFile = "materialsfile";

        public readonly string SceneIdentifier;
        public readonly string SceneFile;
        public readonly string ModelsFile;
        public readonly string TexturesFile;
        public readonly string MaterialsFile;

        public long SceneFileSize = 0L;
        public long ModelsFileSize = 0L;
        public long TexturesFileSize = 0L;
        public long MaterialsFileSize = 0L;

        /// <summary>
        /// A cache file package pointing to the cache file
        /// </summary>
        /// <param name="identifier">This is the Scenes\[FolderName] used when building the cache</param>
        /// <param name="sceneXmlFile">The relative path to the .xsn scene definition file</param>
        /// <param name="modelsCacheFile">The relative path to the models cahche file</param>
        /// <param name="texturesCacheFile">The relative path to the textures cache file</param>
        /// <param name="materialsCacheFile">The relative path to the materials cache file</param>
        public ScenePackage(string identifier, string sceneXmlFile, string modelsCacheFile, string texturesCacheFile, string materialsCacheFile)
        {
            this.SceneIdentifier = identifier;
            this.SceneFile = sceneXmlFile;
            this.ModelsFile = modelsCacheFile;
            this.TexturesFile = texturesCacheFile;
            this.MaterialsFile = materialsCacheFile;
        }
    }
}
