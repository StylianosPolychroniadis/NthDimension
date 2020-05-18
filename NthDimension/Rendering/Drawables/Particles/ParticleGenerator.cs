using System.Collections.Generic;
using System.Drawing;

using NthDimension.Algebra;

namespace NthDimension.Rendering.Drawables.Particles
{

    public sealed class ParticleGenerator  : ApplicationObject
    {
        public int                      ParticleCount   = 0;
        public ParticleSystem           System;
        public List<ParticleAffector>   Affectors
        {
            get { return System.ParticleControllers; }
        }
        //public Color                    Color           = Color.FromArgb(255, 200, 64, 200);
        //public Vector3                  Position        = Vector3.Zero;
        //public Vector3                  Size            = Vector3.One;
        public Matrix4                  Rotation        = Matrix4.Identity;
        public Drawable.RenderLayer     RenderLayer     = Drawable.RenderLayer.Transparent;

        public string[,]                AssetFiles;
        public ParticleGenerator(ApplicationObject scene, Drawable.RenderLayer renderLayer, int particleCount, Vector4 color, string[,] materialsMeshes, Vector3 position, Vector3 size, Matrix4 orientation)
        {
            this.RenderLayer        = renderLayer;
            this.ParticleCount      = particleCount;
            this.Color              = color;
            this.AssetFiles         = materialsMeshes;
            this.Position           = position;
            this.Size               = size;
            this.Rotation           = orientation;
            

            this.System             = new ParticleSystem(scene);
            this.System.Parent      = scene;
            this.System.Color       = Color;
            this.System.Position    = Position;
            this.System.Orientation = Rotation;
            this.System.Renderlayer = RenderLayer;

            for (int i = 0; i < AssetFiles.Rank - 1; i++)  // ss.: wtf?
            {
                this.System.addMaterial(    AssetFiles[i, 0]);
                this.System.addMesh(        AssetFiles[i, 1]);
            }

            this.System.generateParticles(ParticleCount);
        }

        public ParticleGenerator(ApplicationObject scene, Drawable.RenderLayer renderLayer, int particleCount, Color color, string[,] materialsMeshes, Vector3 position, Vector3 size, Matrix4 orientation)
            : this(scene, renderLayer, particleCount, new Vector4(color.R / 255, color.G / 255, color.B / 255, color.A / 255), materialsMeshes, position, size, orientation)
        { }
    }
}
