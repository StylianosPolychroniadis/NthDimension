using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTV3D65;

namespace nDimension.nGraphics
{
    public class Sky
    {

        public Sky(float updateDelay) { Initialize(); }

        #region Additional TV3D Objects
        private static TVShader _skyShader, _cloudsShader;     // Shaders for the sky and clouds
        private static TVMesh _skyHemisphere, _cloudsDome;     // Hemisphere and dome meshes
        private static TVMesh _sun, _moon;                     // Celestial bodies
        #endregion

        #region Other Fields
        private static DirectionalLight _sunLight;             // Directional lights wrapper for the sun
        private static float _julianDay;                       // Needed for the sun position calculation
        private static TV_3DVECTOR _sunPosition, _moonPosition;// Updated in Update, rendered in Render...
        private static TV_2DVECTOR _sun2DPos, _moon2DPos;      // Updated in Update, rendered in Render...
        private static bool _sunFlareColl, _moonFlareColl;     // Was there a collision?
        private static bool _sunFlareFrustrum, _moonFlareFrustrum; // Were they in the frustrum?
        private static float _sunHalfFlareSize;                // And which size (for the sun)?
        private static float _moonIntensity;                   // And what intensity (for the moon)?

        // Sky shader value-holders
        private static float _turbidity = 2.25f;        // Between 2 and 6, how dense the atmosphere is        

        // Cloud shader value-holders
        private static TV_2DVECTOR _windPower = new TV_2DVECTOR(0.00001f, 0.00001f);
        private static TV_2DVECTOR _cloudsSize = new TV_2DVECTOR(0.6f, 1.25f);
        private static TV_2DVECTOR _layersOpacity = new TV_2DVECTOR(1f, 0.85f);

        // Cloud movement accumulators
        private static TV_2DVECTOR _cloudsTranslationOuter = new TV_2DVECTOR(0f, 0f);
        private static TV_2DVECTOR _cloudsTranslationInner = new TV_2DVECTOR(0f, 0f);

        // Text lines

        #endregion

        #region Constants
        const float LATITUDE = 0.6981317f;      // New York's latitude (40°N), in radians
        // const float LATITUDE = 0.22f;        // Default latitude (20°N), in radians

        const float SKY_RADIUS = 3000f;          // Fit it to your world's needs //1000f

        // Those should not be changed, but could if you want to tweak the colors
        private static TV_3DVECTOR FOG_NIGHT_COLOR = new TV_3DVECTOR(0.08f, 0.08f, 0.12f);
        private static TV_3DVECTOR CLOUDS_NIGHT_COLOR = new TV_3DVECTOR(0.175f, 0.175f, 0.2f);
        private static TV_3DVECTOR CLOUDS_DAY_COLOR = new TV_3DVECTOR(0.9f, 0.9f, 0.9f);

        const float SUN_RISE = (float)(Math.PI * 1d / 12d);
        const float SUN_SET = (float)(Math.PI * 1d / -10d);
        #endregion

        #region Overridden methods
        protected void Initialize()
        {
            Classes.CoreDX.Engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_RADIAN);
            // Instanciation
            _skyShader = Classes.CoreDX.Scene.CreateShader("SkyShader");
            _cloudsShader = Classes.CoreDX.Scene.CreateShader("CloudsShader");
            _skyHemisphere = Classes.CoreDX.Scene.CreateMeshBuilder("SkyHemisphere");
            _cloudsDome = Classes.CoreDX.Scene.CreateMeshBuilder("CloudsDome");

            // Moon Material
            Classes.CoreDX.Material.CreateMaterialQuick(0f, 0f, 0f, 0f, "Moon");

            // Textures
            LoadTextures();

            // Shaders
            _skyShader.CreateFromEffectFile(System.Windows.Forms.Application.StartupPath + @"\Shaders\Sky\SkyShader.fx");

            _cloudsShader.CreateFromEffectFile(System.Windows.Forms.Application.StartupPath + @"\Shaders\Sky\CloudsShader.fx");
            _cloudsShader.SetEffectParamTexture("texCubeNormalizer", Classes.CoreDX.Globals.GetTex("CubeNormalizationMap"));

            // Domes and hemispheres
            InitDomes();

            // Sun and Moon billboards initialization
            InitStars();

            // Fog
            Classes.CoreDX.Atmosphere.Fog_Enable(true);
            Classes.CoreDX.Atmosphere.Fog_SetType(CONST_TV_FOG.TV_FOG_EXP2, CONST_TV_FOGTYPE.TV_FOGTYPE_PIXEL);

            // Init the sun light
            _sunLight = new DirectionalLight("SunLight");
            _sunLight.SetBaseColor(1f, 1f, 1f);
            _sunLight.SetIntensity(1f);
            _sunLight.Create();

            // Flares need additive blending
            Classes.CoreDX.ScreenI.Settings_SetTextureFilter(CONST_TV_TEXTUREFILTER.TV_FILTER_BILINEAR);
            Classes.CoreDX.ScreenI.Settings_SetAlphaTest(false, 0);
            Classes.CoreDX.ScreenI.Settings_SetAlphaBlending(true, CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

            // -- Julian date calculation
            // I discarded the year in the equation because a number so big made the double's run out
            // of decimals and caused precision errors. I don't think we care if we're in 1975.
            int a = (14 - Classes.CoreDX.Clock.Month) / 12, y = 1975 + 4800 - a, m = Classes.CoreDX.Clock.Month + 12 * a - 3;
            _julianDay = Classes.CoreDX.Clock.DayOfYear + (153 * m + 2) / 5 + y * 365 + y / 4 - y / 100 + y / 400 - 32045;
            _julianDay -= 2442414;
            _julianDay -= 1f / 24f;

            Classes.CoreDX.Engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);

        }

        public void Update()
        {
            // -- This method could probably be split down, but I don't see the advantage, really
            // Get the camera position at Y = 0
            TV_3DVECTOR camPos, camPosXZ;
            camPosXZ = camPos = Classes.CoreDX.Scene.GetCamera().GetPosition();
            camPosXZ.y = 0f;

            // Sun position calculation
            SkyMaths.AltAzAngles sunAngles = SkyMaths.CalculateSunPosition(_julianDay + (float)Classes.CoreDX.Clock.TimeOfDay.TotalDays, LATITUDE);
            _sunPosition = Classes.CoreDX.MathLib.MoveAroundPoint(camPosXZ, 12500f, sunAngles.azimuth, -sunAngles.altitude);
            _sun.SetPosition(_sunPosition.x, _sunPosition.y, _sunPosition.z);

            // Moon position (approximate inverse of the sun, l0lz.)
            _moonPosition = _sunPosition;
            _moonPosition.y -= 9000f;
            Classes.CoreDX.MathLib.TVVec3Normalize(ref _moonPosition, _moonPosition);
            Classes.CoreDX.MathLib.TVVec3Scale(ref _moonPosition, _moonPosition, -19000.0f);
            _moon.SetPosition(_moonPosition.x, _moonPosition.y, _moonPosition.z);

            // Simple collision test not to render them over the landscape
            _sunFlareColl = _sunFlareFrustrum && !Classes.CoreDX.Scene.Collision(camPos, _sunPosition, (int)CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE);
            _moonFlareColl = _moonFlareFrustrum && !Classes.CoreDX.Scene.Collision(camPos, _moonPosition, (int)CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE);

            // Set the sun normalized vector and the sunTheta to the shader
            TV_3DVECTOR sunNormedPos = new TV_3DVECTOR();
            Classes.CoreDX.MathLib.TVVec3Subtract(ref sunNormedPos, _sunPosition, camPosXZ);
            Classes.CoreDX.MathLib.TVVec3Normalize(ref sunNormedPos, sunNormedPos);
            float sunTheta = SkyMaths.VectorToTheta(sunNormedPos);
            _skyShader.SetEffectParamFloat("_sunTheta", sunTheta);
            _skyShader.SetEffectParamVector3("_sunVector", sunNormedPos);

            // Set the size of the flares depending on viewport size and a dot product
            TV_3DMATRIX tempMatrix = Classes.CoreDX.Scene.GetCamera().GetRotationMatrix();
            TV_3DVECTOR normedLookAt = new TV_3DVECTOR(tempMatrix.m31, tempMatrix.m32, tempMatrix.m33);
            float dotProduct = Classes.CoreDX.MathLib.TVVec3Dot(normedLookAt, sunNormedPos);
            _sunHalfFlareSize = Classes.CoreDX.clientSizeX / 5f * (dotProduct * dotProduct * dotProduct);

            // Sun lightning direction
            TV_3DVECTOR sunDirection = new TV_3DVECTOR();
            Classes.CoreDX.MathLib.TVVec3Subtract(ref sunDirection, _sunPosition, camPosXZ);
            Classes.CoreDX.MathLib.TVVec3Normalize(ref _sunLight.direction, sunDirection);
            // Clamp the direction... could probably use better interpolation
            if (_sunLight.direction.y < 1f / 9f) _sunLight.direction.y = 1f / 9f;
            Classes.CoreDX.MathLib.TVVec3Scale(ref _sunLight.direction, _sunLight.direction, -1f);

            // Sun lightning intensity
            float sunIntensity = SkyMaths.Saturate(SkyMaths.Lerp(sunAngles.altitude, SUN_SET, SUN_RISE));

            // Stars and moon opacity
            _moonIntensity = SkyMaths.Saturate(SkyMaths.Lerp(sunAngles.altitude, 0f, SUN_SET)) * 0.95f;
            _skyShader.SetEffectParamFloat("_starsIntensity", _moonIntensity);
            _moonIntensity += 0.05f;
            Classes.CoreDX.Material.SetEmissive(Classes.CoreDX.Globals.GetMat("Moon"), _moonIntensity, _moonIntensity, _moonIntensity, 0f);

            // Moon (ambient) lightning intensity
            // TODO : A bug with ambient lightning forces me to use the material instead...
            float ambientColorRG = -0.05f * (1f - sunIntensity) + 0.21f;
            // _lights.SetGlobalAmbient(ambientColor, ambientColor, 0.2f);
            // _sunLight.SetAmbientColor(ambientColor,ambientColor, 0.2f);
            Classes.CoreDX.Material.SetAmbient(Classes.CoreDX.Globals.GetMat("Matte"), ambientColorRG, ambientColorRG, 0.2f, 1.0f);

            // A hack to kill the orange tones in the nightsky
            _skyShader.SetEffectParamFloat("_nightDarkness", 1f - (_moonIntensity - 0.05f));

            // Calculate the constant matrices
            SkyMaths.xyYColor _zenithColors = SkyMaths.SkyZenithColor(_turbidity, sunTheta);
            SkyMaths.xyYCoeffs _distribCoeffs = SkyMaths.DistributionCoefficients(_turbidity);
            _skyShader.SetEffectParamVector3("_zenithColor", _zenithColors.AsVector3);
            for (int i = 0; i < 5; i++)
            {
                _skyShader.SetEffectParamFloat("_xDistribCoeffs[" + i + "]", _distribCoeffs.x[i]);
                _skyShader.SetEffectParamFloat("_yDistribCoeffs[" + i + "]", _distribCoeffs.y[i]);
                _skyShader.SetEffectParamFloat("_YDistribCoeffs[" + i + "]", _distribCoeffs.Y[i]);
            }

            // Set the adaptative luminance and gamma corrections
            float gamma = 1f / (1.6f + (_turbidity - 2f) * 0.1f);
            _skyShader.SetEffectParamFloat("_invGammaCorrection", 1.5f * gamma);
            _skyShader.SetEffectParamFloat("_invPowLumFactor", gamma);
            _skyShader.SetEffectParamFloat("_invNegMaxLum", -1.25f / SkyMaths.MaximumLuminance(
                _turbidity, sunTheta, _zenithColors, _distribCoeffs));

            // Clouds movement
            TV_2DVECTOR _windEffectInner = new TV_2DVECTOR(_windPower.x * Atmosphere._accumulatedTime, _windPower.y * Atmosphere._accumulatedTime);
            Classes.CoreDX.MathLib.TVVec2Add(ref _cloudsTranslationInner, _cloudsTranslationInner, _windEffectInner);
            TV_2DVECTOR _windEffectOuter = new TV_2DVECTOR(_windPower.x / 2f * Atmosphere._accumulatedTime, _windPower.y / 2f * Atmosphere._accumulatedTime);
            Classes.CoreDX.MathLib.TVVec2Add(ref _cloudsTranslationOuter, _cloudsTranslationOuter, _windEffectOuter);
            _cloudsShader.SetEffectParamVector2("_cloudsTranslation[0]", _cloudsTranslationOuter);
            _cloudsShader.SetEffectParamVector2("_cloudsTranslation[1]", _cloudsTranslationInner);

            // Clouds coloring
            TV_COLOR atmoCol = SkyMaths.AtmosphereColor(_turbidity, sunTheta, _zenithColors, _distribCoeffs);
            TV_3DVECTOR atmoColVec = new TV_3DVECTOR(atmoCol.r, atmoCol.g, atmoCol.b);
            float dayState = SkyMaths.Saturate(SkyMaths.Lerp(sunAngles.altitude, (float)(Math.PI * 1f / (6f - _turbidity / 2f)), SUN_RISE));
            Classes.CoreDX.MathLib.TVVec3Lerp(ref atmoColVec, CLOUDS_NIGHT_COLOR, atmoColVec, sunIntensity);
            Classes.CoreDX.MathLib.TVVec3Lerp(ref atmoColVec, CLOUDS_DAY_COLOR, atmoColVec, dayState);
            _cloudsShader.SetEffectParamVector3("_cloudsColor", atmoColVec);

            // Sun light color and intensity settings
            _sunLight.SetIntensity(sunIntensity);
            _sunLight.SetBaseColor(atmoColVec.x, atmoColVec.y, atmoColVec.z);
            _sunLight.Update();

            Classes.CoreDX.Scene.SetBackgroundColor(atmoCol.r, atmoCol.g, atmoCol.b, atmoCol.a);
                                    
            Classes.CoreDX.Light.SetLightPosition(Lighting.LightsManager.SunId, _sunPosition.x, _sunPosition.y, _sunPosition.z);
            // Fog coloring
            Classes.CoreDX.Atmosphere.Fog_SetColor(atmoColVec.x / 2f, atmoColVec.y / 2f, atmoColVec.z / 2f);

            // Fog distance
            Classes.CoreDX.Atmosphere.Fog_SetParameters(0f, 0f, (float)(_turbidity / 8500d));
            //CoreDX.Atmosphere.Fog_SetParameters(0f, 0f, 0.001f);

            //// Cloud shader parameters
            _cloudsShader.SetEffectParamVector2("_cloudsSize", _cloudsSize);
            _cloudsShader.SetEffectParamVector2("_layersOpacity", _layersOpacity);

            //// Update the OSD

            Atmosphere._accumulatedTime = 0f;
            ////base.Update();
        }

        public void ForcedUpdate()
        {
            //base.ForcedUpdate();

            TV_3DVECTOR camPos;
            // Reposition the sky according to camera movement
            camPos = Classes.CoreDX.Scene.GetCamera().GetPosition();
            _skyHemisphere.SetPosition(camPos.x, camPos.y - 20000f, camPos.z);     // Error HERE!!! (Stelios)
            
            if(camPos.y < 0) { camPos.y = 0; }
            
            _cloudsDome.SetPosition(camPos.x, camPos.y - 20000f, camPos.z);        // TODO : AttachTo should work soon

            // Get the 2D position of sun and moon, and check if we should render them
            _sunFlareFrustrum = Classes.CoreDX.MathLib.Project3DPointTo2D(_sunPosition, ref _sun2DPos.x, ref _sun2DPos.y, true);
            _moonFlareFrustrum = Classes.CoreDX.MathLib.Project3DPointTo2D(_moonPosition, ref _moon2DPos.x, ref _moon2DPos.y, true);

            // Manage the input
            ManageInput();
        }

        public void Render()
        {
            Classes.CoreDX.Atmosphere.Fog_Enable(false);
            _skyHemisphere.Render();
            _sun.Render();
            _moon.Render();
            _cloudsDome.Render();
            Classes.CoreDX.Atmosphere.Fog_Enable(false); // true

            // Render the flares
            Classes.CoreDX.ScreenI.Action_Begin2D();
            if (_sunFlareFrustrum && _sunFlareColl)
            {
                int color = Classes.CoreDX.Globals.RGBA(0.325f, 0.31f, 0.28f, 1f);
                Classes.CoreDX.ScreenI.Draw_Texture(Classes.CoreDX.Globals.GetTex("Glow"),
                    _sun2DPos.x - _sunHalfFlareSize, _sun2DPos.y - _sunHalfFlareSize,
                    _sun2DPos.x + _sunHalfFlareSize, _sun2DPos.y + _sunHalfFlareSize,
                    color, color, color, color);
                color = Classes.CoreDX.Globals.RGBA(0.05f, 0.05f, 0.05f, 1f);
                Classes.CoreDX.ScreenI.Draw_Texture(Classes.CoreDX.Globals.GetTex("Rays"),
                    _sun2DPos.x - _sunHalfFlareSize * 2f, _sun2DPos.y - _sunHalfFlareSize * 2f,
                    _sun2DPos.x + _sunHalfFlareSize * 2f, _sun2DPos.y + _sunHalfFlareSize * 2f,
                    color, color, color, color);
            }
            if (_moonFlareFrustrum && _moonFlareColl)
            {
                int color = Classes.CoreDX.Globals.RGBA(0.15f * _moonIntensity, 0.15f * _moonIntensity, 0.225f * _moonIntensity, 1f);
                float halfSize = Classes.CoreDX.clientSizeX / 9f;
                Classes.CoreDX.ScreenI.Draw_Texture(Classes.CoreDX.Globals.GetTex("Glow"),
                    _moon2DPos.x - halfSize, _moon2DPos.y - halfSize,
                    _moon2DPos.x + halfSize, _moon2DPos.y + halfSize,
                    color, color, color, color);
            }
            Classes.CoreDX.ScreenI.Action_End2D();
        }
        #endregion

        #region Other Methods
        void LoadTextures()
        {
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\Starmap.dds", "StarMap");
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\CloudsLots.dds", "CloudLots");
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\CloudsLess.dds", "CloudLess");
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\Sun.dds", "Sun");
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\Moon.dds", "Moon");

            Classes.CoreDX.Texture.LoadCubeTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\CubeNormalizer_ULQ.dds", "CubeNormalizationMap", 256, CONST_TV_COLORKEY.TV_COLORKEY_NO, false);

            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\Glow.dds", "Glow");
            Classes.CoreDX.Texture.LoadTexture(System.Windows.Forms.Application.StartupPath + @"\Textures\Atmosphere\Rays.dds", "Rays");
        }

        void InitDomes()
        {
            // Sky hemisphere
            _skyHemisphere.LoadTVM(System.Windows.Forms.Application.StartupPath + @"\Resources\Hemisphere.tvm", false);
            _skyHemisphere.SetMeshFormat((int)(CONST_TV_MESHFORMAT.TV_MESHFORMAT_TEX1 |
                CONST_TV_MESHFORMAT.TV_MESHFORMAT_NOLIGHTING));
            _skyHemisphere.SetCollisionEnable(false);
            _skyHemisphere.SetScale(SKY_RADIUS, SKY_RADIUS, SKY_RADIUS);
            _skyHemisphere.SetTextureEx(0, Classes.CoreDX.Globals.GetTex("StarMap"));
            //_skyHemisphere.SetTextureEx(0, Classes.CoreDX.Globals.GetTex("Skysphere"));
            _skyHemisphere.SetShader(_skyShader);

            // Clouds dome
            _cloudsDome.LoadTVM(System.Windows.Forms.Application.StartupPath + @"\Resources\Dome.tvm", false);
            _cloudsDome.SetTextureEx(0, Classes.CoreDX.Globals.GetTex("CloudLots"));
            _cloudsDome.SetTextureEx(1, Classes.CoreDX.Globals.GetTex("CloudLess"));
            _cloudsDome.SetMeshFormat((int)(CONST_TV_MESHFORMAT.TV_MESHFORMAT_TEX1 |
                CONST_TV_MESHFORMAT.TV_MESHFORMAT_TEX2));
            _cloudsDome.SetCollisionEnable(false);
            _cloudsDome.SetScale(SKY_RADIUS * 0.9f, SKY_RADIUS * 0.9f, SKY_RADIUS  * 0.9f);
            _cloudsDome.SetShader(_cloudsShader);
        }

        void InitStars()
        {
            _sun = Classes.CoreDX.Scene.CreateBillboard(Classes.CoreDX.Globals.GetTex("Sun"), 0f, 0f, 0f, 1200f, 1200f, "Sun", true);
            _sun.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);
            _sun.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
            _sun.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_NONE);
            _sun.SetAlphaTest(false, 0, false);
            _sun.SetCollisionEnable(false);

            _moon = _sun.Duplicate("Moon", true);
            _moon.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED);
            _moon.SetScale(0.5f, 0.5f, 0.5f);
            _moon.SetTexture(Classes.CoreDX.Globals.GetTex("Moon"));
            _moon.SetMaterial(Classes.CoreDX.Globals.GetMat("Moon"));
        }

        void ManageInput() {
            int shiftPressed = 
                (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_LEFTSHIFT) || 
                Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_RIGHTSHIFT))
                ? -1 : 1;

            float speed = shiftPressed * nGraphics.Atmosphere._elapsedTime * 0.001f;

            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_T)) {
                _turbidity += speed;
                if (_turbidity < 2f)  _turbidity = 2f;
                if (_turbidity > 6f)  _turbidity = 6f;
            }
            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_P)) {
                _windPower.x = _windPower.y += speed * 0.00005f;
            }
            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_C)) {
                _cloudsSize.x = _cloudsSize.y += speed * 0.00005f;
                if (_cloudsSize.x < 0f)  _cloudsSize.x = _cloudsSize.y = 0f;
            }
            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_I)) {
                _layersOpacity.x += speed;
                _layersOpacity.x = SkyMaths.Saturate(_layersOpacity.x);
            }
            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_O)) {
                _layersOpacity.y += speed;
                _layersOpacity.y = SkyMaths.Saturate(_layersOpacity.y);
            }
            if (Classes.CoreDX.Input.IsKeyPressed(CONST_TV_KEY.TV_KEY_F)) {
                nGraphics.Atmosphere.timeFactor += speed * 100f;
                if (nGraphics.Atmosphere.timeFactor < 0f) nGraphics.Atmosphere.timeFactor = 0f;
            }
        }

        #endregion
    }
}
