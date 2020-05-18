using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Algebra;
using NthDimension.Procedural.Celestial;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Utilities;
using NthDimension.Rendering.Configuration;

namespace NthDimension.Procedural
{
    public class SolarGenerator
    {
        protected Planet earth;

        public SolarGenerator(Rendering.Scenegraph.SceneGame scene)
        {
            earth = new Planet("Earth (Gaia)",
                                //new Vector3d(0d, 0d, 20000d),
                                new Vector3d(0d, 0d, 30d),
                                //90000d,       // 6371 km = 6371000 m
                                100,
                                new PlanetModel.CubeTexturePaths()
                                {
                                    Left      = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0004.png"),
                                    Right     = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0005.png"),
                                    Top       = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0002.png"),
                                    Bottom    = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0003.png"),
                                    Front     = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0006.png"),
                                    Back      = Path.Combine(GameSettings.TextureFolder, @"planet\earth\base_0001.png")
                                },
                                new PlanetModel.CubeTexturePaths()
                                {
                                    Left      = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0004.png"),
                                    Right     = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0005.png"),
                                    Top       = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0002.png"),
                                    Bottom    = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0003.png"),
                                    Front     = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0006.png"),
                                    Back      = Path.Combine(GameSettings.TextureFolder, @"planet\earth\heightmap_0001.png"),
                                },
                                Path.Combine(GameSettings.TextureFolder, @"planet\earth\detail.png"));

            earth.DrawableModel.IgnoreCulling = true;
            earth.DrawableModel.IgnoreLod = true;

            scene.AddDrawable(earth.DrawableModel);
        }
    }
}
