using NthDimension.Algebra;
using NthDimension.Rendering;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Models;


namespace NthDimension.Procedural.Celestial
{
    public class Planet : ApplicationObject
    {
        protected PlanetModel       m_planetModel;
        protected double            m_planetRadius          = 1f;

        public PlanetModel DrawableModel
        {
            get { return m_planetModel; }
        }

        public Planet(string name, Vector3d position, double radius, PlanetModel.CubeTexturePaths textures, PlanetModel.CubeTexturePaths heightmaps, string detailTexture)
        {
            this.Name           = name;
            this.Position       = new Vector3();// position;
                                                //this.ScenicRadius   = radius;

            #region Textures Loading
            if (System.IO.Path.GetExtension(detailTexture).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(detailTexture);
            if (System.IO.Path.GetExtension(detailTexture).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(detailTexture);
            if (System.IO.Path.GetExtension(detailTexture).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(detailTexture);

            // Color Maps

            if (System.IO.Path.GetExtension(textures.Left).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Left);
            if(System.IO.Path.GetExtension(textures.Left).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Left);
            if (System.IO.Path.GetExtension(textures.Left).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Left);

            if (System.IO.Path.GetExtension(textures.Right).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Right);
            if (System.IO.Path.GetExtension(textures.Right).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Right);
            if (System.IO.Path.GetExtension(textures.Right).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Right);

            if (System.IO.Path.GetExtension(textures.Top).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Top);
            if (System.IO.Path.GetExtension(textures.Top).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Top);
            if (System.IO.Path.GetExtension(textures.Top).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Top);

            if (System.IO.Path.GetExtension(textures.Bottom).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Bottom);
            if (System.IO.Path.GetExtension(textures.Bottom).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Bottom);
            if (System.IO.Path.GetExtension(textures.Bottom).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Bottom);

            if (System.IO.Path.GetExtension(textures.Front).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Front);
            if (System.IO.Path.GetExtension(textures.Front).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Front);
            if (System.IO.Path.GetExtension(textures.Front).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Front);

            if (System.IO.Path.GetExtension(textures.Back).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(textures.Back);
            if (System.IO.Path.GetExtension(textures.Back).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(textures.Back);
            if (System.IO.Path.GetExtension(textures.Back).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(textures.Back);

            // Heightmaps

            if (System.IO.Path.GetExtension(heightmaps.Left).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Left);
            if(System.IO.Path.GetExtension(heightmaps.Left).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Left);
            if (System.IO.Path.GetExtension(heightmaps.Left).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Left);

            if (System.IO.Path.GetExtension(heightmaps.Right).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Right);
            if (System.IO.Path.GetExtension(heightmaps.Right).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Right);
            if (System.IO.Path.GetExtension(heightmaps.Right).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Right);

            if (System.IO.Path.GetExtension(heightmaps.Top).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Top);
            if (System.IO.Path.GetExtension(heightmaps.Top).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Top);
            if (System.IO.Path.GetExtension(heightmaps.Top).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Top);

            if (System.IO.Path.GetExtension(heightmaps.Bottom).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Bottom);
            if (System.IO.Path.GetExtension(heightmaps.Bottom).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Bottom);
            if (System.IO.Path.GetExtension(heightmaps.Bottom).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Bottom);

            if (System.IO.Path.GetExtension(heightmaps.Front).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Front);
            if (System.IO.Path.GetExtension(heightmaps.Front).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Front);
            if (System.IO.Path.GetExtension(heightmaps.Front).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Front);

            if (System.IO.Path.GetExtension(heightmaps.Back).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(heightmaps.Back);
            if (System.IO.Path.GetExtension(heightmaps.Back).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(heightmaps.Back);
            if (System.IO.Path.GetExtension(heightmaps.Back).ToLower().Contains(".bmp"))
                ApplicationBase.Instance.TextureLoader.fromBitmap(heightmaps.Back);

            ApplicationBase.Instance.TextureLoader.LoadTextures(null, true);

            #endregion

            int left = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Left.Replace(GameSettings.TextureFolder, string.Empty));
            int right = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Right.Replace(GameSettings.TextureFolder, string.Empty));
            int top = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Top.Replace(GameSettings.TextureFolder, string.Empty));
            int bottom = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Bottom.Replace(GameSettings.TextureFolder, string.Empty));
            int front = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Front.Replace(GameSettings.TextureFolder, string.Empty));
            int back = ApplicationBase.Instance.TextureLoader.getTextureId(textures.Back.Replace(GameSettings.TextureFolder, string.Empty));


            PlanetModel.CubeTextureIndices mTextures = new PlanetModel.CubeTextureIndices()
            {
                Left        = left,
                Right       = right,
                Top         = top,
                Bottom      = bottom,
                Front       = front,
                Back        = back
            };

            //m_planetModel = new PlanetModel((float)radius, mTextures, heightmaps, detailTexture.Replace(GameSettings.TextureFolder, string.Empty));
            m_planetModel = new PlanetModel((float)radius, mTextures, heightmaps, detailTexture);
        }
    }
}
