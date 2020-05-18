using MTV3D65;

namespace nDimension.nGraphics
{
    public static class Atmosphere
    {
        public static float _elapsedTime;
        public static float _accumulatedTime;
        public static long _startTime;
        public static long _endTime;
        public static float _frequency = 0f;
        public static float timeFactor = 750f;

        // Todo Add a Clock Datetime Value here and seperate the scene timing

        public static void StaticSKY_Init(string pTexture)
        {
            Classes.CoreDX.Atmosphere.SkySphere_Enable(false);
            //Classes.CoreDX.Texture.DeleteTexture(Classes.CoreDX.Globals.GetTex("ATMOSPHERE.SKYSPHERE"));
            //Classes.CoreDX.Atmosphere.SkySphere_SetTexture(Classes.CoreDX.Texture.LoadTexture(pTexture, "ATMOSPHERE.SKYSPHERE"));
            Classes.CoreDX.Atmosphere.SkySphere_SetParameters(false);
            Classes.CoreDX.Atmosphere.SkySphere_SetTexture(Classes.CoreDX.Globals.GetTex("ATMOSPHERE.SKYSPHERE"));
            Classes.CoreDX.Atmosphere.SkySphere_SetRadius(50000f);
            //Classes.CoreDX.Atmosphere.SkySphere_SetParameters(false);
            
        }

        public static void StaticSKY_Render()
        {
            Classes.CoreDX.Atmosphere.SkySphere_Render();
        }

        public static void DynamicSKY_Init()
        {
            Classes.CoreDX.cSKY = new nGraphics.Sky(45f);
        }
        
        public static void DynamicSKY_Update()
        {
            Classes.CoreDX.Engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_RADIAN);

            nGraphics.Atmosphere.UpdateTime();
            Classes.CoreDX.cSKY.ForcedUpdate();
            Classes.CoreDX.cSKY.Update();

            Classes.CoreDX.Engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
        }

        public static void DynamicSKY_Render()
        {
            Classes.CoreDX.ScreenI.Settings_SetTextureFilter(CONST_TV_TEXTUREFILTER.TV_FILTER_BILINEAR);
            Classes.CoreDX.ScreenI.Settings_SetAlphaTest(false, 0);
            Classes.CoreDX.ScreenI.Settings_SetAlphaBlending(true, CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

            Classes.CoreDX.cSKY.Render();

            Classes.CoreDX.ScreenI.Settings_SetTextureFilter(CONST_TV_TEXTUREFILTER.TV_FILTER_ANISOTROPIC);
            Classes.CoreDX.ScreenI.Settings_SetAlphaTest(true, 1);
            Classes.CoreDX.ScreenI.Settings_SetAlphaBlending(false, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA);
        }

        public static void UpdateTime()
        {
            _startTime = _endTime;
            Classes.CoreDX.QueryPerformanceCounter(ref _endTime);
            _elapsedTime = ((float)(_endTime - _startTime) / Frequency * 1000.0f);

            Classes.CoreDX.Clock = Classes.CoreDX.Clock.AddMilliseconds(_elapsedTime * timeFactor);
        }



        /// <summary>
        /// Gets the performance frequency returned by QueryPerformanceFrequency
        /// </summary>
        static float Frequency
        {
            get
            {
                if (_frequency == 0f)
                {
                    long frequency = 0L;
                    Classes.CoreDX.QueryPerformanceFrequency(ref frequency);
                    _frequency = (float)frequency;
                }

                return _frequency;
            }
        }
    }
}
