using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTV3D65;

namespace nDimension.nGraphics
{
    public class DirectionalLight
    {
        TVLightEngine _lights;
        TVMathLibrary _maths;

        private TV_LIGHT _light;
        private int _number;
        private string _name;
        public TV_3DVECTOR direction;
        private TV_COLOR _color;

        public DirectionalLight(string name)
        {
            _lights = Classes.CoreDX.Light;
            _maths = Classes.CoreDX.MathLib;
            _name = name;
        }

        /// <summary>
        /// Use this method only once, and after having set all parameters.
        /// </summary>
        public void Create()
        {
            _number = _lights.CreateDirectionalLight(direction, _color.r, _color.g, _color.b, _name, 1.0f);
            _lights.GetLight(_number, ref _light);
            _lights.EnableLight(_number, true);
        }

        /// <summary>
        /// Use this method whenever the light parameters are changed.
        /// </summary>
        public void Update()
        {
            Sync();
            _lights.SetLight(_number, ref _light);
            _lights.EnableLight(_number, _color.a != 0f);
        }

        private void Sync()
        {
            _light.direction = direction;
            _maths.TVColorScale(ref _light.diffuse, new TV_COLOR(_color.r, _color.g, _color.b, 1.0f), _color.a);
            _light.ambient = _light.specular = _light.diffuse;
        }

        public void SetBaseColor(float r, float g, float b)
        {
            _color.r = r;
            _color.g = g;
            _color.b = b;
        }

        /*public void SetAmbientColor(float r, float g, float b) {
            _light.ambient = new TV_COLOR(r, g, b, 1.0f);
        }*/

        public void SetIntensity(float i)
        {
            _color.a = i;
        }
    }
}
