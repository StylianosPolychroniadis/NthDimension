using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GLTerrain {

    public class SimWindow : GameWindow {
        private Camera mainCam = null;
        private Terrain terrain = null;
        private Skybox skybox = null;
        private GUILayer gui = null;
        private Timer guiTimer = null;
        private Light light = null;

        private RenderTarget reflectTarget = null;
        private RenderTarget refractTarget = null;
        private RenderTarget sceneTarget = null;
        private RenderTarget bloomTarget = null;
        private RenderTarget bloomTarget2 = null;
        private RenderTarget bloomTarget3 = null;
        private RenderTarget bloomTarget4 = null;

        private ScreenQuad screenQuad = null;

        private ShaderProgram terrainShader = null;
        private ShaderProgram waterShader = null;
        private ShaderProgram sceneShader = null;
        private ShaderProgram bloomShader = null;

        private Texture waterTex = null;
        private Geometry water = null;

        private Vector3 cameraVel = new Vector3();
        private Vector3 fogColor = new Vector3(0.847f, 0.788f, 0.671f);

        private float texOffset = 0.0f;
        private float waterHeight = 50;
        private bool leftButton = false;
        private bool wireframe = false;
        private bool bloom = true;

        public SimWindow(int width, int height, bool fullscreen) :
            base(width, height, GraphicsMode.Default, "Flight Sim",
            (fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.Default)) {
            this.VSync = VSyncMode.Off;

            Keyboard.KeyDown += OnKeyDown;
            Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(OnMouseButtonDown);
            Mouse.ButtonUp += new EventHandler<MouseButtonEventArgs>(OnMouseButtonUp);
            Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

            mainCam = new Camera(1.0f, 6000.0f, MathHelper.DegreesToRadians(45.0f), new Viewport(width, height));
            mainCam.Position = new Vector3(0, 350, 0);

            guiTimer = new Timer(0.2, true, UpdateGUI);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.Enable(EnableCap.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Enable(EnableCap.Blend);
            GL.CullFace(CullFaceMode.Back);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(2.0f);

            GL.ClearColor(new Color4(0.85f, 0.85f, 1.0f, 1.0f));

            light = new Light(LightName.Light0, LightType.Directional, new Vector3(0.9f, 0.6f, -1), Vector3.Zero);

            // set up objects
            terrain = new Terrain(
                2049,1025,//3048, 3048,
                512, 512,
                @"heightmap2.png", 350.0f,
                @"colormap.png", 1.0f);

            skybox = new Skybox(@"sky\day1.bmp", @"sky\day5.bmp", @"sky\day2.bmp", @"sky\day4.bmp", @"sky\day3.bmp", "");
            //skybox = new Skybox(@"sky\sunset1.bmp", @"sky\sunset5.bmp", @"sky\sunset2.bmp", @"sky\sunset4.bmp", @"sky\sunset3.bmp", "");

            water = new Geometry(BeginMode.Quads, new VertexBuffer(new Vertex[] {
                new Vertex() {X = -10000, Y = waterHeight, Z = 10000, S = 0, T = 0},
                new Vertex() {X = 10000, Y = waterHeight, Z = 10000, S = 60, T = 0},
                new Vertex() {X = 10000, Y = waterHeight, Z = -10000, S = 60, T = 60},
                new Vertex() {X = -10000, Y = waterHeight, Z = -10000, S = 0, T = 60},
            }, new ushort[] {
                0, 1, 2, 3
            }));

            waterTex = new Texture(@"water-normal.bmp", false, TextureUnit.Texture1);

            CreateShaders();

            reflectTarget = new RenderTarget(new Texture(new Bitmap(1024, 1024), true, false));
            reflectTarget.Target.Wrap = false;

            refractTarget = new RenderTarget(new Texture(new Bitmap(1024, 1024), true, false, TextureUnit.Texture2));
            refractTarget.Target.Wrap = false;

            sceneTarget = new RenderTarget(new Texture(new Bitmap(mainCam.View.Width, mainCam.View.Height), true, false));
            sceneTarget.Target.Wrap = false;

            bloomTarget = new RenderTarget(new Texture(new Bitmap(mainCam.View.Width / 2, mainCam.View.Height / 2), true, false, TextureUnit.Texture1));
            bloomTarget.Target.Wrap = false;

            bloomTarget2 = new RenderTarget(new Texture(new Bitmap(bloomTarget.Width / 2, bloomTarget.Height / 2), true, false, TextureUnit.Texture2));
            bloomTarget2.Target.Wrap = false;

            bloomTarget3 = new RenderTarget(new Texture(new Bitmap(bloomTarget2.Width / 2, bloomTarget2.Height / 2), true, false, TextureUnit.Texture3));
            bloomTarget3.Target.Wrap = false;

            bloomTarget4 = new RenderTarget(new Texture(new Bitmap(bloomTarget3.Width / 2, bloomTarget3.Height / 2), true, false, TextureUnit.Texture4));
            bloomTarget4.Target.Wrap = false;

            screenQuad = new ScreenQuad();

            gui = new GUILayer(mainCam.View);
            gui.AddLabel("fpsLabel", "FPS", new Font("Arial", 20.0f, FontStyle.Bold), 0, 10, Layer.Layer1, Color.White);
        }

        private void UpdateGUI(double actualElapsed) {
            string fpsText = "FPS: " + this.RenderFrequency.ToString("#");
            gui.UpdateObjectText("fpsLabel", fpsText);
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e) {
            if (leftButton) {
                mainCam.Yaw += e.XDelta / (float)mainCam.View.Width * MathHelper.TwoPi;
                mainCam.Pitch += e.YDelta / (float)mainCam.View.Height * MathHelper.TwoPi;
            }
        }

        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Left) {
                leftButton = false;
            }
        }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Left) {
                leftButton = true;
            }
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.F1:
                    wireframe = true;
                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                    break;
                case Key.F2:
                    wireframe = false;
                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                    break;
                case Key.N:
                    terrain.DrawNormals = !terrain.DrawNormals;
                    break;
                case Key.B:
                    bloom = !bloom;
                    break;
                case Key.Escape:
                    Exit();
                    break;
            }
        }

        protected override void OnUnload(EventArgs e) {
            base.OnUnload(e);

            waterShader.Dispose();
            waterTex.Dispose();
            water.Dispose();
            terrain.Dispose();
            skybox.Dispose();
            gui.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            bool zInput = false, xInput = false, rollInput = false;

            float accel = 5.0f;
            float maxVel = 30.0f;
            float decel = 100.0f;

            if (Keyboard[Key.RShift]) {
                accel = 100.0f;
                maxVel = 400.0f;
                decel = 2000.0f;
            }

            if (Keyboard[Key.Up]) {
                cameraVel.Z += accel;
                zInput = true;
            }
            if (Keyboard[Key.Down]) {
                cameraVel.Z -= accel;
                zInput = true;
            }
            if (Keyboard[Key.Left]) {
                cameraVel.X -= accel;
                xInput = true;
            }
            if (Keyboard[Key.Right]) {
                cameraVel.X += accel;
                xInput = true;
            }

            // roll
            if (Keyboard[Key.Q]) {
                mainCam.Roll += 0.06f;
                rollInput = true;
            }
            if (Keyboard[Key.E]) {
                mainCam.Roll -= 0.06f;
                rollInput = true;
            }
            if (Keyboard[Key.W]) {
                mainCam.Roll = 0.0f;
                rollInput = true;
            }

            if (Math.Abs(cameraVel.Z) > maxVel) {
                cameraVel.Z = Math.Sign(cameraVel.Z) * maxVel;
            }

            if (Math.Abs(cameraVel.X) > maxVel) {
                cameraVel.X = Math.Sign(cameraVel.X) * maxVel;
            }

            float decelMag = (float)(decel * e.Time);
            if (!zInput) {
                cameraVel.Z -= Math.Sign(cameraVel.Z) * decelMag;
                if (Math.Abs(cameraVel.Z) < decelMag) {
                    cameraVel.Z = 0.0f;
                }
            }

            if (!xInput) {
                cameraVel.X -= Math.Sign(cameraVel.X) * decelMag;
                if (Math.Abs(cameraVel.X) < decelMag) {
                    cameraVel.X = 0.0f;
                }
            }

            if (xInput || zInput || rollInput) {
                mainCam.MoveForward((float)(cameraVel.Z * e.Time));
                mainCam.Strafe((float)(cameraVel.X * e.Time));
            }

            waterShader.SetUniform("eyePos", mainCam.Position);
            waterShader.Detach();

            texOffset += (float)(0.04 * e.Time);
            while (texOffset > 1.0f) {
                texOffset -= 1.0f;
            }

            guiTimer.Update(e.Time);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            mainCam.View.Width = ClientSize.Width;
            mainCam.View.Height = ClientSize.Height;
            mainCam.View.MakeCurrent();

            Matrix4 proj = mainCam.GetProjection();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            gui.ResizeView(mainCam.View);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            Matrix4 camera = mainCam.GetModelView();
            GL.LoadMatrix(ref camera);

            RenderReflection();
            RenderRefraction();

            // render to scene texture
            sceneTarget.Bind();
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                skybox.Draw(mainCam, false);

                RenderObjects();

                RenderWater();
            }
            sceneTarget.Unbind(false);

            // render scene to bloom with blur
            RenderBloom(sceneTarget.Target, bloomTarget, true);

            // render bloom to bloom 2
            RenderBloom(bloomTarget.Target, bloomTarget2, false);

            // render bloom 2 to bloom 3
            RenderBloom(bloomTarget2.Target, bloomTarget3, false);

            // render bloom 3 to bloom 4
            RenderBloom(bloomTarget3.Target, bloomTarget4, false);

            // render the final screen texture
            sceneTarget.Target.Bind();
            if (bloom) {
                bloomTarget.Target.Bind();
                bloomTarget2.Target.Bind();
                bloomTarget3.Target.Bind();
                bloomTarget4.Target.Bind();
                sceneShader.Attach();
            }

            screenQuad.Draw();

            if (bloom) {
                sceneShader.Detach();
                bloomTarget.Target.Unbind();
                bloomTarget2.Target.Unbind();
                bloomTarget3.Target.Unbind();
                bloomTarget4.Target.Unbind();
            }
            sceneTarget.Target.Unbind();

            // draw overlay
            if (wireframe) {
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            }

            gui.Draw();

            if (wireframe) {
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            }

            SwapBuffers();
        }

        private void RenderBloom(Texture scene, RenderTarget target, bool brightPass) {
            scene.Bind();
            target.Bind();
            {
                bloomShader.SetUniform("scene", scene.UnitNumber);
                bloomShader.SetUniform("power", brightPass ? 64.0f : 1.0f);
                bloomShader.SetUniform("pixelX", 1.0f / target.Width);
                bloomShader.SetUniform("pixelY", 1.0f / target.Height);
                bloomShader.Attach();

                screenQuad.Draw();

                bloomShader.Detach();
            }
            target.Unbind(false);
            scene.Unbind();
        }

        private void RenderRefraction() {
            refractTarget.Bind();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrainShader.SetUniform("clip", 1);
            RenderObjects();
            terrainShader.SetUniform("clip", 0);

            refractTarget.Unbind(true);
        }

        private void RenderWater() {
            reflectTarget.Target.Bind();
            refractTarget.Target.Bind();
            waterTex.Bind();

            waterShader.SetUniform("offset", texOffset);
            waterShader.Attach();

            water.Draw();

            waterTex.Unbind();
            reflectTarget.Target.Unbind();
            refractTarget.Target.Unbind();
            waterShader.Detach();
        }

        private void RenderReflection() {
            terrainShader.SetUniform("clip", -1);

            GL.PushMatrix();
            reflectTarget.Bind();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 camera = mainCam.GetModelView();
            GL.LoadMatrix(ref camera);

            GL.Scale(1, -1, 1);
            terrain.CenterPoint = new Vector3(terrain.CenterPoint.X, terrain.CenterPoint.Y - waterHeight * 2, terrain.CenterPoint.Z);

            skybox.Draw(mainCam, true);
            RenderObjects();

            terrain.CenterPoint = new Vector3(terrain.CenterPoint.X, terrain.CenterPoint.Y + waterHeight * 2, terrain.CenterPoint.Z); ;

            reflectTarget.Unbind(true);
            GL.PopMatrix();

            terrainShader.SetUniform("clip", 0);
        }

        private void RenderObjects() {
            // draw objects
            terrain.Draw();
        }

        private void CreateShaders() {
            waterShader = new ShaderProgram(
@"
varying vec3 eyeVector;

uniform vec3 lightPos, eyePos;
uniform float offset;

void main () {
    eyeVector = eyePos - vec3(gl_Vertex);
    vec4 vertex = gl_ModelViewProjectionMatrix * gl_Vertex;

    gl_Position = vertex;
    gl_TexCoord[0] = vec4(gl_MultiTexCoord0.x + offset, gl_MultiTexCoord0.y, gl_MultiTexCoord0.zw);
    gl_TexCoord[1] = vec4(gl_MultiTexCoord0.x + offset, gl_MultiTexCoord0.y + offset, gl_MultiTexCoord0.zw);
    gl_TexCoord[2] = vertex;
}",
@"
uniform sampler2D diffuseMap, normalMap, refractionMap;
uniform vec3 lightPos, eyePos, fogColor;
uniform float offset;

varying vec3 eyeVector;

void main() {
    vec3 normal = texture2D(normalMap, -gl_TexCoord[1].st).rbg * 2.0 - 1.0;
    normal += texture2D(normalMap, vec2(gl_TexCoord[0].s + 0.5, gl_TexCoord[0].t)).rbg * 2.0 - 1.0;
    normal = normalize(normal);

    vec3 eyeVec = normalize(eyeVector);
    vec3 reflected = normalize(-reflect(normalize(lightPos), normal));
    float fresnel = 1.0 - dot(eyeVec, vec3(0.0, 1.0, 0.0));

    vec2 distortion = normal.xz * vec2(10.0);
    vec4 projVertex = vec4(gl_TexCoord[2].x + distortion.x, gl_TexCoord[2].y + distortion.y, gl_TexCoord[2].z, gl_TexCoord[2].w);
    vec2 projTexCoord = projVertex.xy / projVertex.z * 0.5 + 0.5;

    vec4 reflectColor = texture2D(diffuseMap, projTexCoord);
    vec4 refractColor = texture2D(refractionMap, projTexCoord);

    float spec = max(dot(eyeVec, reflected), 0.0);

    vec4 color = mix(refractColor, reflectColor, fresnel);
    color *= vec4(0.8, 0.8, 1.0, 1.0);
    color += pow(spec, 128.0);
    //color += pow(reflectColor, vec4(32.0));

    const float density = 1.0;
    float z = gl_FragCoord.z / gl_FragCoord.w / 5000.0;
    float fog = exp2(-density * density * z * z * 1.442695);

    gl_FragColor = vec4(mix(fogColor, color.rgb, clamp(fog, 0.0, 1.0)), 1.0);
}");
            waterShader.SetUniform("diffuseMap", 0);
            waterShader.SetUniform("normalMap", 1);
            waterShader.SetUniform("refractionMap", 2);
            waterShader.SetUniform("lightPos", light.Position);
            waterShader.SetUniform("fogColor", fogColor);

            terrainShader = new ShaderProgram(
@"
varying float clipY;
void main () {
    clipY = gl_Vertex.y;
    gl_Position = gl_ModelViewProjectionMatrix * vec4(gl_Vertex);
    gl_TexCoord[0] = gl_MultiTexCoord0;
}",
"const float waterHeight = " + waterHeight.ToString("#.0") + ";" +
@"
uniform vec3 fogColor;
uniform sampler2D texture;
uniform vec3 eyePos;
uniform int clip;
varying float clipY;

void main() {
    if ((clip < 0 && clipY < waterHeight) ||
        (clip > 0 && clipY > waterHeight + 5.0)) {
        discard;
    } else {
        float density = 1.0;
        float z = gl_FragCoord.z / gl_FragCoord.w / 5000.0;
        float fog = exp2(-density * density * z * z * 1.442695);

        vec4 tex = texture2D(texture, gl_TexCoord[0].st);
        gl_FragColor = vec4(mix(fogColor, tex.rgb, clamp(fog, 0.0, 1.0)), 1.0);
    }
}");
            terrainShader.SetUniform("fogColor", fogColor);
            terrain.Shader = terrainShader;

            sceneShader = new ShaderProgram("",
@"
uniform sampler2D scene, bloom, bloom2, bloom3, bloom4;
void main() {
    vec4 sceneColor = texture2D(scene, gl_TexCoord[0].st);
    vec4 bloomColor = texture2D(bloom, gl_TexCoord[0].st);
    vec4 bloom2Color = texture2D(bloom2, gl_TexCoord[0].st);
    vec4 bloom3Color = texture2D(bloom3, gl_TexCoord[0].st);
    vec4 bloom4Color = texture2D(bloom4, gl_TexCoord[0].st);
    vec4 sum = (bloomColor + bloom2Color + bloom3Color + bloom4Color);
    gl_FragColor = sum + sceneColor;
}");
            sceneShader.SetUniform("scene", 0);
            sceneShader.SetUniform("bloom", 1);
            sceneShader.SetUniform("bloom2", 2);
            sceneShader.SetUniform("bloom3", 3);
            sceneShader.SetUniform("bloom4", 4);

            bloomShader = new ShaderProgram("",
@"
uniform sampler2D scene;
uniform float pixelX, pixelY, power;
uniform float kernel[25];

void main() {
    vec4 sum = vec4(0.0);
    int num = 0;

    for (float y = -2.0; y < 3.0; y += 1.0) {
        for (float x = -2.0; x < 3.0; x += 1.0) {
            sum += texture2D(scene, gl_TexCoord[0].st + vec2(pixelX * x, pixelY * y)) * vec4(kernel[num]);
            num++;
        }
    }

    gl_FragColor = pow(sum, vec4(power));
}");
            bloomShader.SetUniform("kernel", new float[] {
                1.0f / 331.0f,  4.0f / 331.0f,  7.0f / 331.0f,  4.0f / 331.0f, 1.0f / 331.0f,
                4.0f / 331.0f, 20.0f / 331.0f, 33.0f / 331.0f, 20.0f / 331.0f, 4.0f / 331.0f,
                7.0f / 331.0f, 33.0f / 331.0f, 55.0f / 331.0f, 33.0f / 331.0f, 7.0f / 331.0f,
                4.0f / 331.0f, 20.0f / 331.0f, 33.0f / 331.0f, 20.0f / 331.0f, 4.0f / 331.0f,
                1.0f / 331.0f,  4.0f / 331.0f,  7.0f / 331.0f,  4.0f / 331.0f, 1.0f / 331.0f
            });
        }
    }
}