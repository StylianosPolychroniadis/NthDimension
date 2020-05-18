using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace GLTerrain {
    public interface IMeshLoader {
        void LoadMesh(string filename);
        Geometry GetGeometry();
        IEnumerable<Shader> GetEffects();
        IEnumerable<Texture> GetTextures();
        bool IsLoaded { get; }
    }

    public class Mesh : IDisposable {
        public Geometry MeshBuffer { get; private set; }
        public Shader ShaderEffect { get; private set; }

        private List<Texture> textures = new List<Texture>();
        public IEnumerable<Texture> Textures {
            get {
                foreach (Texture tex in textures) {
                    yield return tex;
                }
            }
        }

        public Vector3 Position {
            get {
                return MeshBuffer.Position;
            }
            set {
                MeshBuffer.Position = value;
            }
        }

        public Quaternion Orientation {
            get {
                return MeshBuffer.Orientation;
            }
            set {
                MeshBuffer.Orientation = value;
            }
        }

        public Mesh(IMeshLoader loader, string filename) {
            if (!loader.IsLoaded) {
                loader.LoadMesh(filename);
            }

            MeshBuffer = loader.GetGeometry();
            textures.AddRange(loader.GetTextures());

            if (loader.GetEffects().Count() > 0) {
                ShaderEffect = loader.GetEffects().First();
            }

        }

        public void Draw() {
            MeshBuffer.Draw();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
