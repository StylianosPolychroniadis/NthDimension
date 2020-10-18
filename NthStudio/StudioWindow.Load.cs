using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public partial class StudioWindow
    {
        public void LoadScene(ScenePackage e, string name, bool scanFileSystem = false, Action callback = null)
        {
            if (_engineStarted) return;
            _engineStarted = true;
_readyToPlay = false;
_assetsLoaded = false;

            this.Scene = new NthDimension.Rendering.Scenegraph.SceneGame();

            tSceneLoad = DateTime.Now;

            ScanNewFiles        = scanFileSystem;

            ActivateScene(new SceneItem(e, name));

            OnAssetsLoaded += delegate
            {
                _engineStarted  = false;
                _readyToPlay    = true;
                _assetsLoaded   = true;

                if (null != callback)
                    //callback(null);
                    callback();
            };
        }

        public void NewScene()
        {
           
            //this.Scene.CreatePhysics();
            //this.Scene.CreateOctree();

            var scnfile = System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.AppData_Temporary,
                                                               System.IO.Path.GetRandomFileName());

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(scnfile))
            {
                wr.WriteLine("<scene>");
                
                wr.WriteLine("<sunlight name='key' enabled='true'>");
		            wr.WriteLine("<min>30</min>");
		            wr.WriteLine("<max>160</max>");
		            wr.WriteLine("<color>1|1|1|1</color>");
                wr.WriteLine("<ambient>0.88|0.82|0.83</ambient>");      // Light
                //wr.WriteLine("<ambient>0.38|0.32|0.33</ambient>");    // Dark
                //wr.WriteLine("<position>0|60|0</position>");
		            wr.WriteLine("<direction>-0.04817817|-0.4817817|0.085</direction>");
	            wr.WriteLine("</sunlight>");
                  
                
                wr.WriteLine("</scene>");
            }

            var modelCache = string.Empty; // System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory,
                                           //                         "cacheModel.ucf");

            var textureCache = string.Empty; // System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory,
                                             //                  "cacheTexture.ucf");

            var materialCache = string.Empty;// System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory,
                                             //                   "cacheTexture.ucf");

            LoadScene(new ScenePackage("<EMPTY>",
                                        scnfile,
                                        modelCache,
                                        textureCache,
                                        materialCache
                                        ),
                                        "<EMPTY>", 
                                        false,
                                        _callback_NewSceneLoad);


            


        }


        private void _callback_NewSceneLoad()
        {
            #region Create a UV textured reference unit cube
            NthDimension.Rendering.Drawables.Models.Cube c = new NthDimension.Rendering.Drawables.Models.Cube(this.Scene);

            c.setMaterial("uvbox.xmf");

            this.Scene.Models.Add(c);

            NthDimension.Rendering.Utilities.ConsoleUtil.log("Cube Created");
            #endregion Create a UV textured reference unit cube
        }
    }
}
