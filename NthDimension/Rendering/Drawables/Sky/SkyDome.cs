
//http://blog.simonrodriguez.fr/articles/28-06-2015_un_ciel_dans_le_shader.html
// also http://csc.lsu.edu/~kooima/misc/cs594/final/part2.html

namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.Xml;
    using NthDimension.Algebra;
    using NthDimension.Rendering.GameViews;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Scenegraph;
    using NthDimension.Rendering.Shaders;

    public class SkyDome : Model
    {
        new public static string nodename = "skydome";
#pragma warning disable CS0414
        float                               weather                 = 1; // [0.5...1.0] Cloudy ... Clear
        float                               time                    = 0; // Time in seconds
        bool                                m_assetsInitialized     = false;
        private const string                defaultMaterial         = "skydome\\skydome.xmf";
        private const string                defaultMesh             = "skydome\\skydome.obj";
        private const int                   secondsInDay            = 86400;
        private const float                 radsPerSecond           = MathFunc.TwoPi / secondsInDay;
        //private const float                 secondsPerColor         = secondsInDay / 13; // 13 is the length of sunLightColors array
        private float                       nextRandomWeather       = 0f;
        private Random                      random                  = new Random(-123729);
        private Vector3                     sunRotationVector       = new Vector3(0f, 1f, 1f);
        private Vector3                     sunPositionOriginal;

        private Lights.LightDirectional     sunLight;

        public Vector3                      SunPosition             = new Vector3(0, 5, 1);
        public Vector4                      SunColor                = Vector4.One;
        public Vector3                      SunColorAmbient         = new Vector3(.5f, .5f, .5f);
        public Matrix3                      StarRotation            = Matrix3.Identity;
        public float                        TimeInterval            = 0.1f;

        class LightColor
        {
            public string Text { get; }
            public Vector4 Color { get; }

            public LightColor(string text, Vector4 color)
            {
                this.Text = text;
                this.Color = color; 
            }
        }

        System.Collections.Generic.List<LightColor> sunLightColors = new System.Collections.Generic.List<LightColor>() {
            new LightColor("midnight",       System.Drawing.Color.FromArgb(255, 20, 20, 20).ToVector4()),
            new LightColor("breaking dawn",  System.Drawing.Color.FromArgb(255, 50, 50, 50).ToVector4()),
            new LightColor("dawn",           System.Drawing.Color.FromArgb(255, 80, 80, 80).ToVector4()),
            new LightColor("post dawn",      System.Drawing.Color.FromArgb(255, 110, 110, 110).ToVector4()),
            new LightColor("sunrise",        System.Drawing.Color.FromArgb(255, 140, 140, 140).ToVector4()),
            new LightColor("morning",        System.Drawing.Color.FromArgb(255, 172, 172, 172).ToVector4()),
            new LightColor("noon",           System.Drawing.Color.FromArgb(255, 200, 200, 200).ToVector4()),
            new LightColor("afternoon",      System.Drawing.Color.FromArgb(255, 180, 180, 180).ToVector4()),
            new LightColor("evening",        System.Drawing.Color.FromArgb(255, 150, 150, 150).ToVector4()),
            new LightColor("sunset",         System.Drawing.Color.FromArgb(255, 120, 120, 120).ToVector4()),
            new LightColor("dusk",           System.Drawing.Color.FromArgb(255, 90, 90, 90).ToVector4()),
            new LightColor("post dusk",      System.Drawing.Color.FromArgb(255, 60, 60, 60).ToVector4()),
            new LightColor("night",          System.Drawing.Color.FromArgb(255, 30, 30, 30).ToVector4())              
        };

        System.Collections.Generic.List<LightColor> sunLightAmbientColors = new System.Collections.Generic.List<LightColor>() {
            new LightColor("midnight",       new Vector4(0.01f, 0.01f, 0.01f, 1f)),                  
            new LightColor("breaking dawn",  new Vector4(0.08f, 0.08f, 0.08f, 1f)),
            new LightColor("dawn",           new Vector4(0.1f,  0.1f,  0.1f,  1f)),                   
            new LightColor("post dawn",      new Vector4(0.15f, 0.15f, 0.15f, 1f)),
            new LightColor("sunrise",        new Vector4(0.25f, 0.25f, 0.25f, 1f)),
            new LightColor("morning",        new Vector4(0.35f, 0.35f, 0.35f, 1f)),
            new LightColor("noon",           new Vector4(0.55f, 0.55f, 0.55f, 1f)),
            new LightColor("afternoon",      new Vector4(0.45f, 0.45f, 0.45f, 1f)),
            new LightColor("evening",        new Vector4(0.35f, 0.35f, 0.35f, 1f)),
            new LightColor("sunset",         new Vector4(0.25f, 0.25f, 0.25f, 1f)),
            new LightColor("dusk",           new Vector4(0.15f, 0.15f, 0.15f, 1f)),
            new LightColor("post dusk",      new Vector4(0.10f, 0.10f, 0.10f, 1f)),
            new LightColor("night",          new Vector4(0.04f, 0.04f, 0.04f, 1f))
        };

        public SkyDome(ApplicationObject obj)
            :this(obj as SceneGame)// Ugly hack
        { }
        public SkyDome(SceneGame mScene):base(mScene)
        {
            PrimitiveType = Rasterizer.PrimitiveType.Triangles;
            Parent                      = mScene;
            this.Scene                  = mScene;
            mScene.AddDrawable(this);
        }


        public override void Update()
        {
#if _DEVUI_
            return;
#endif
            if (!m_assetsInitialized)
                this.createRenderObject();

            for (int m = 0; m < this.meshes.Length; m++)
                this.meshes[m].CurrentLod = MeshVbo.MeshLod.Level0;

            if (null == sunPositionOriginal)
                sunPositionOriginal = SunPosition;

            Position = ApplicationBase.Instance.Player.Position;                // FIX IT. DOES NOT WORK/NO EFFECT AT ALL

            this.modelMatrix = Matrix4.CreateTranslation(Position) *
                              Orientation *
                              Matrix4.CreateScale(Size);

            time += TimeInterval;
            if (time > 86400f)
                time = 0f;
            
            TimeSpan tsp = TimeSpan.FromSeconds(time);
            string hourOfDay = tsp.ToString(@"hh\:mm\:ss\:fff");

            Matrix4 rot = Matrix4.CreateRotationX(sunRotationVector.X * (time * radsPerSecond)) *
                          Matrix4.CreateRotationY(sunRotationVector.Y * (time * radsPerSecond)) *
                          Matrix4.CreateRotationZ(sunRotationVector.Z * (time * radsPerSecond));

            Vector3 srot = new Vector3(sunPositionOriginal);

            rot.TransformVector(ref srot);
                        
            SunPosition = srot;
            sunLight.PointingDirection = srot.Y >= 0 ? -srot.Normalized() : Vector3.Zero;   // Do not cast shadows when sun Y is below the horizon

            StarRotation = new Matrix3(rot.Row0.Xyz, rot.Row1.Xyz, rot.Row2.Xyz);



            float secondsPerColor = secondsInDay / sunLightColors.Count;

            int colorIndex = (int)(time / secondsPerColor);

            float lerpFactor = 0f;

            if (time > secondsPerColor)
                lerpFactor = (time - (secondsPerColor * colorIndex)) / secondsPerColor;//
            else
                lerpFactor = time / secondsPerColor;

            if (colorIndex + 1 < sunLightColors.Count)
            {                
                SunColor        = Vector4.Lerp(sunLightColors[colorIndex].Color,                        sunLightColors[colorIndex + 1].Color,           lerpFactor);
                SunColorAmbient = Vector4.Lerp(sunLightAmbientColors[colorIndex].Color,                 sunLightAmbientColors[colorIndex + 1].Color,    lerpFactor).Xyz;
            }
            else
            {
                SunColor        = Vector4.Lerp(sunLightColors[sunLightColors.Count - 1].Color,          sunLightColors[0].Color,                        lerpFactor);
                SunColorAmbient = Vector4.Lerp(sunLightColors[sunLightAmbientColors.Count - 1].Color,   sunLightAmbientColors[0].Color,                 lerpFactor).Xyz;
            }

            sunLight.Color = SunColor;
            sunLight.lightAmbient = SunColorAmbient;

#if DEBUG && !OPTIMIZED
            Utilities.ConsoleUtil.log(string.Format("ColorIndex: {0} {1} Time: {2} {3}",  colorIndex, sunLightColors[colorIndex].Text, time, hourOfDay));
#endif

            updateChilds();
        }

        public override void draw(ViewInfo curView, bool renderlayer)
        {
            if (vaoHandle != null && IsVisible && m_assetsInitialized)
            {
                Shader shader = new Shader();
                MeshVbo curMesh = meshes[0];
                Material curMaterial = materials[0];

                System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                drawTime.Start();
                //#if DEBUG
                if (!ApplicationBase.Instance.DrawnMeshes.Contains(curMesh))       // USED FOR DEBUG ONLY
                    ApplicationBase.Instance.DrawnMeshes.Add(curMesh);

                shader = activateMaterial(ref curMaterial);
                ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1}:{2} Shader: {3}", this.GetType(), Name, curMesh.Name, shader.Name));

                // curView projection matrix should be infinite

              

                SetupMatrices(ref curView, ref shader, ref curMesh);                

                setSpecialUniforms(ref shader, ref curMesh);
                
                ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1}:{2} Shader: {3}", this.GetType(), Name, curMesh.Name, shader.Name));



                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[0]);
                ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType,
                                                               curMesh.MeshData.Indices.Length,
                                                               Rasterizer.DrawElementsType.UnsignedInt,
                                                               IntPtr.Zero);
                ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1}:{2} Shader: {3}", this.GetType(), Name, curMesh.Name, shader.Name));
                curMesh.DrawTimeAllPasses += drawTime.ElapsedTicks;
                curMesh.DrawCalls++;
            }
        }
        public override Shader activateMaterial(ref Material curMat)
        {
            int texunit = 0;
            Shaders.Shader shader = curMat.shader;
            int handle = shader.Handle;
            Material.Propertys propertys = curMat.propertys;


            if (!shader.Loaded)
                return shader;

#if _SHADERDEBUG_
                    Factories.ShaderLoader.UsingShader(shader);
#endif

            ApplicationBase.Instance.Renderer.UseProgram(handle);

#region Log Validate Status
#if !OPTIMIZED
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(handle);
            ApplicationBase.Instance.Renderer.GetProgram(handle, Rasterizer.GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                Utilities.ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", shader.Name, validateStatus));
#endif
#endregion Check Validate Status

            // TODO:: Maintain the uniform inside the shader program
            // https://stackoverflow.com/questions/39786138/opengl-pass-texture-to-program-once-or-at-every-rendering

            /*
             You should bind the texture before every draw. You only need to set the location once. 
                You can also do layout(binding = 1) in your shader code for that. The location uniform 
                stays with the program. The texture binding is a global GL state. Also be careful about 
                ActiveTexture: it is a global GL state.

                Good practice would be:

                On program creation, once, set texture location (uniform)
                On draw: SetActive(i), Bind(i), Draw, SetActive(i) Bind(0), SetActive(0)
             */

#region not working
            //int tint0 = materials[0].getTextureId(Material.TexType.baseTexture);
            //int tint1 = materials[0].getTextureId(Material.TexType.baseTextureTwo);
            //int sun = materials[0].getTextureId(Material.TexType.baseTextureThree);
            //int moon = materials[0].getTextureId(Material.TexType.baseTextureFour);
            //int cloud0 = materials[0].getTextureId(Material.TexType.auxTexture);
            //int cloud1 = materials[0].getTextureId(Material.TexType.auxTextureTwo);
            //int shader_tint0 = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_tinta.ToString());
            //int shader_tint1 = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_tintb.ToString());
            //int shader_sun = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_sun.ToString());
            //int shader_moon = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_moon.ToString());
            //int shader_cloud0 = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_cloudsa.ToString());
            //int shader_cloud1 = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, Uniform.sky_cloudsb.ToString());
            //// Tint 0
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 0);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_tint0, tint0);
            //// Tint 1            
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 1);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_tint1, tint1);
            //// Sun            
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 2);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_sun, sun);
            //// Moon            
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 3);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_moon, moon);
            //// Clouds 0            
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 4);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_cloud0, cloud0);
            //// Clouds 1            
            //ApplicationBase.Instance.Renderer.ActiveTexture(Rasterizer.TextureUnit.Texture0 + 5);            
            //ApplicationBase.Instance.Renderer.Uniform1(shader_cloud1, cloud1);

            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, tint0);
            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, tint1);
            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, sun);
            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, moon);
            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, cloud0);
            //ApplicationBase.Instance.Renderer.BindTexture(Rasterizer.TextureTarget.Texture2D, cloud1);
#endregion not working

            curMat.activateTexture(Material.TexType.baseTexture, ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.baseTextureTwo, ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.baseTextureThree, ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.baseTextureFour, ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.auxTexture, ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.auxTextureTwo, ref texunit, ref handle);

            ApplicationBase.Instance.Renderer.CullFaceEnabled = true;
            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            ApplicationBase.Instance.Renderer.DepthMask(false);



#if _SHADERDEBUG_
                    Factories.ShaderLoader.UsingShader(shader);
#endif


            return shader;
        }
        protected override void setSpecialUniforms(ref Shaders.Shader shader, ref MeshVbo curMesh)
        {
#if _DEVUI_
            return;
#endif
            if (!shader.Loaded) return;

            if (null != Scene.DirectionalLights &&
                Scene.DirectionalLights.Count == 0) return;

            if (null == ApplicationBase.Instance.Player) return;
      
            shader.InsertUniform(Uniform.sky_time, ref time);
            shader.InsertUniform(Uniform.sky_clouds, ref weather);

            shader.InsertUniform(Uniform.sky_sunp, ref SunPosition);
            shader.InsertUniform(Uniform.sky_stars, ref StarRotation);
        }
        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            if (reader.Name.ToLower() == "materials" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Materials = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "meshes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Meshes = GenericMethods.StringListFromString(reader.Value);
            }
            if (reader.Name.ToLower() == "weather" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                weather = GenericMethods.FloatFromString(reader.Value);
            }
            if (reader.Name.ToLower() == "time" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                time = GenericMethods.FloatFromString(reader.Value);
            }
            if (reader.Name.ToLower() == "interval" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                TimeInterval = GenericMethods.FloatFromString(reader.Value);
            }
            if (reader.Name.ToLower() == "sun" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                SunPosition = GenericMethods.Vector3FromString(reader.Value);
                sunPositionOriginal = SunPosition;
            }
            if (reader.Name.ToLower() == "star" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                StarRotation = GenericMethods.Matrix3FromString(reader.Value);
            }
        }
        private void createRenderObject()
        {
            m_assetsInitialized = true;

            //float skyScale = 6f;// ApplicationBase.Instance.Player.ViewInfo.zFar / (float)Math.Sqrt(3);

            //Size = new Vector3(skyScale, skyScale, skyScale);

            try
            {
                //addMaterial("skydome\\skydome.xmf");                 // TODO:: Implement
                //addMesh("skydome\\skydome_ok.obj");          // TODO:: Implement
                //materials[0].propertys.useAlpha = true;
                //materials[0].propertys.noDepthMask = false;
                //materials[0].propertys.noCull = true;
                
                IsVisible = true;
                IgnoreCulling = true;
                IgnoreLod = true;
                Renderlayer = RenderLayer.Both;
                
                //this.setMaterial(materials[0].name);
                //this.setMesh(meshes[0].Name);
            }
            catch
            {

            }

#region Directional Light
            /*
             <sunlight name='key' enabled='true' >
		        <min>30</min> <!-- 30 -->
		        <max>200</max> <!-- 640 or 980 -->
		        <color>0.75|0.75|0.75|0.75</color>
		        <ambient>0.38|0.332|0.333</ambient>
		        <direction>-0.01017817|-0.4817817|0.015</direction>
	            </sunlight>
            */
            sunLight = new Lights.LightDirectional(this.Scene)
            {
                Name = "SkyDome_Sun",
                ShortRange = 30,
                LongRange = 200,
                Color = SunColor,
                lightAmbient = SunColorAmbient
            };

            

#endregion Directional Light
        }
    }
}


#region Vertex Shader
/* 
# version 330 core
//---------IN------------
in vec3 vpoint;
in vec3 vnormal;
//---------UNIFORM------------
uniform vec3 sun_pos;//sun position in world space
uniform mat4 mvp;
uniform mat3 rot_stars;//rotation matrix for the stars
//---------OUT------------
out vec3 pos;
out vec3 sun_norm;
out vec3 star_pos;

//---------MAIN------------
void main(){
    gl_Position = mvp * vec4(vpoint, 1.0);
    pos = vpoint;

    //Sun pos being a constant vector, we can normalize it in the vshader
    //and pass it to the fshader without having to re-normalize it
    sun_norm = normalize(sun_pos);

    //And we compute an approximate star position using the special rotation matrix
    star_pos = rot_stars * normalize(pos);
}

     */
#endregion

#region Fragment Shader
/* 
# version 330 core
//---------IN------------
in vec3 pos;
in vec3 sun_norm;
in vec3 star_pos;
//---------UNIFORM------------
uniform sampler2D tint;			//the color of the sky on the half-sphere where the sun is. (time x height)
uniform sampler2D tint2;		//the color of the sky on the opposite half-sphere. (time x height)
uniform sampler2D sun;			//sun texture (radius x time)
uniform sampler2D moon;			//moon texture (circular)
uniform sampler2D clouds1;		//light clouds texture (spherical UV projection)
uniform sampler2D clouds2;		//heavy clouds texture (spherical UV projection)
uniform float weather;			//mixing factor (0.5 to 1.0)
uniform float time;
//---------OUT------------
out vec3 color;

//---------NOISE GENERATION------------
//Noise generation based on a simple hash, to ensure that if a given point on the dome
//(after taking into account the rotation of the sky) is a star, it remains a star all night long
float Hash( float n ){
        return fract( (1.0 + sin(n)) * 415.92653);
}
float Noise3d( vec3 x ){
    float xhash = Hash(round(400*x.x) * 37.0);
    float yhash = Hash(round(400*x.y) * 57.0);
    float zhash = Hash(round(400*x.z) * 67.0);
    return fract(xhash + yhash + zhash);
}

//---------MAIN------------
void main(){
    vec3 pos_norm = normalize(pos);
    float dist = dot(sun_norm,pos_norm);

    //We read the tint texture according to the position of the sun and the weather factor
    vec3 color_wo_sun = texture(tint2, vec2((sun_norm.y + 1.0) / 2.0,max(0.01,pos_norm.y))).rgb;
    vec3 color_w_sun = texture(tint, vec2((sun_norm.y + 1.0) / 2.0,max(0.01,pos_norm.y))).rgb;
    color = weather*mix(color_wo_sun,color_w_sun,dist*0.5+0.5);

    //Computing u and v for the clouds textures (spherical projection)
    float u = 0.5 + atan(pos_norm.z,pos_norm.x)/(2*3.14159265);
    float v = - 0.5 + asin(pos_norm.y)/3.14159265;

    //Cloud color
    //color depending on the weather (shade of grey) *  (day or night) ?
    vec3 cloud_color = vec3(min(weather*3.0/2.0,1.0))*(sun_norm.y > 0 ? 0.95 : 0.95+sun_norm.y*1.8);

    //Reading from the clouds maps
    //mixing according to the weather (1.0 -> clouds1 (sunny), 0.5 -> clouds2 (rainy))
    //+ time translation along the u-axis (horizontal) for the clouds movement
    float transparency = mix(texture(clouds2,vec2(u+time,v)).r,texture(clouds1,vec2(u+time,v)).r,(weather-0.5)*2.0);

    // Stars
    if(sun_norm.y<0.1){//Night or dawn
        float threshold = 0.99;
        //We generate a random value between 0 and 1
        float star_intensity = Noise3d(normalize(star_pos));
        //And we apply a threshold to keep only the brightest areas
        if (star_intensity >= threshold){
            //We compute the star intensity
            star_intensity = pow((star_intensity - threshold)/(1.0 - threshold), 6.0)*(-sun_norm.y+0.1);
            color += vec3(star_intensity);
        }
    }

    //Sun
    float radius = length(pos_norm-sun_norm);
    if(radius < 0.05){//We are in the area of the sky which is covered by the sun
        float time = clamp(sun_norm.y,0.01,1);
        radius = radius/0.05;
        if(radius < 1.0-0.001){//< we need a small bias to avoid flickering on the border of the texture
            //We read the alpha value from a texture where x = radius and y=height in the sky (~time)
            vec4 sun_color = texture(sun,vec2(radius,time));
            color = mix(color,sun_color.rgb,sun_color.a);
        }
    }

    //Moon
    float radius_moon = length(pos_norm+sun_norm);//the moon is at position -sun_pos
    if(radius_moon < 0.03){//We are in the area of the sky which is covered by the moon
        //We define a local plane tangent to the skydome at -sun_norm
        //We work in model space (everything normalized)
        vec3 n1 = normalize(cross(-sun_norm,vec3(0,1,0)));
        vec3 n2 = normalize(cross(-sun_norm,n1));
        //We project pos_norm on this plane
        float x = dot(pos_norm,n1);
        float y = dot(pos_norm,n2);
        //x,y are two sine, ranging approx from 0 to sqrt(2)*0.03. We scale them to [-1,1], then we will translate to [0,1]
        float scale = 23.57*0.5;
        //we need a compensation term because we made projection on the plane and not on the real sphere + other approximations.
        float compensation = 1.4;
        //And we read in the texture of the moon. The projection we did previously allows us to have an undeformed moon
        //(for the sun we didn't care as there are no details on it)
        color = mix(color,texture(moon,vec2(x,y)*scale*compensation+vec2(0.5)).rgb,clamp(-sun_norm.y*3,0,1));
    }

    //Final mix
    //mixing with the cloud color allows us to hide things behind clouds (sun, stars, moon)
    color = mix(color,cloud_color,clamp((2-weather)*transparency,0,1));

}

     */
#endregion