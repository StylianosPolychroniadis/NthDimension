using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GLTerrain {
    public class ColladaLoader : IMeshLoader {
        public ColladaLoader() {
        }

        public void LoadMesh(string filename) {
            throw new NotImplementedException();
        }

        public Geometry GetGeometry() {
            throw new NotImplementedException();
        }

        public IEnumerable<Shader> GetEffects() {
            throw new NotImplementedException();
        }

        public IEnumerable<Texture> GetTextures() {
            throw new NotImplementedException();
        }

        public bool IsLoaded {
            get { throw new NotImplementedException(); }
        }
    }
}
