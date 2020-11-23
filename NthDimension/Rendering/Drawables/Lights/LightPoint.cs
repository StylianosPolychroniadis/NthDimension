using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Lights
{
    // TODO:: Build point light shader and add Activate function accordingly


    public class LightPoint : Light
    {
        new public static string nodename = "pointlamp";
        public int lightId;
        public int ProjectionTexture = 0;

        private float nextFarUpdate;
        private string texturename;
        private bool useProjectionTexture = false;  // was false
        
        public LightPoint(ApplicationObject parent)
        {
            Parent = (Drawable)parent;
            Scene.PointLights.Add(this);
            Name = Scene.GetUniqueName();
            IgnoreCulling = true;
            this.setupShadow();
            Parent.forceUpdate();
            createRenderObject();
            shadowQuality = Settings.Instance.video.shadowQuality;

        }
        private void createRenderObject()
        {
            //drawable = new LightVolume(this);

            //drawable.setMaterial("defShading\\lightSpot.xmf");
            ////drawable.setMaterial("defShading\\volSpot.xmf");

            //drawable.setMesh("base\\spotlight_cone.obj");
            //drawable.IgnoreCulling = true;
            //drawable.Color = new Vector4(0.6f, 0.7f, 1.0f, 1);
            //drawable.IsVisible = true;

            ////this.Texture = "base\\uvmap.png";     // MAR-19-18 DEBUG (tring to display Light Volume)
        }


        private void setupShadow()
        {
            viewInfo = new GameViews.ViewInfo(this);
            viewInfo.zNear = 0.7f;  // was 0.7
            viewInfo.zFar = 1500f;
            viewInfo.UpdateProjectionMatrix();

            // NOTE FOVY was set to PI/2 in Original
        }


    }
}
