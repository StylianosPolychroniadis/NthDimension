using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;

using NthDimension.Algebra;
using NthDimension.Rasterizer;
//using NthDimension.Algebra.Geometry;
//using NthDimension.Common;
//using NthDimension.Common.Utilities;
//using NthDimension.Graphics.Buffers;
//using NthDimension.Graphics.Drawables;
//using NthDimension.Graphics.Forms;
//using NthDimension.Graphics.Geometry;
//using NthDimension.Graphics.Modelling;
using NthDimension.Rasterizer.GL1.Factories;
//using NthDimension.Graphics.Renderer;
////using SYSCON.Graphics.Shaders;
using enuPolygonMode = NthDimension.Graphics.Geometry.enuPolygonMode;
using BlendingFactorDest = OpenTK.Graphics.OpenGL.BlendingFactorDest;
using BlendingFactorSrc = OpenTK.Graphics.OpenGL.BlendingFactorSrc;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using HintMode = OpenTK.Graphics.OpenGL.HintMode;
using HintTarget = OpenTK.Graphics.OpenGL.HintTarget;
using MaterialFace = NthDimension.Graphics.Renderer.MaterialFace;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

//using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
//using BlendingFactorDest = OpenTK.Graphics.OpenGL.BlendingFactorDest;
//using BlendingFactorSrc = SYSCON.Graphics.Renderer.BlendingFactorSrc;
//using GetPName = OpenTK.Graphics.OpenGL.GetPName;
//using MathHelper = OpenTK.MathHelper;
//using ShadingModel = OpenTK.Graphics.OpenGL.ShadingModel;
//using Vector3 = OpenTK.Vector3;

namespace NthDimension.Rasterizer.GL1
{
    // Master TODO:: 1) Implement VAOs
    //        TODO:: 2) Implement VBOs
    //        TODO:: 3) Implement Shaders
    

    [Serializable] // Required by CodeDOM
    public partial class RendererGL1x : RendererBase //, 
                              //IControlRenderDevice
                              ////, ITextureRenderDevice
                              //// TODO:: IInputDevice
    {
        public event LogApplication             SendLogToApplication;
        public event LogErrorApplication        SendErrorLogToApplication;



        #region Glu API
        //internal const string LIBRARY_GLU = "Glu32.dll";
        //[DllImport(LIBRARY_GLU, SetLastError = true)]
        //private static extern IntPtr gluNewQuadric();

        //public override IntPtr CreateQuadric()
        //{
        //    MakeCurrent();
        //    IntPtr quad = gluNewQuadric();
        //    return quad;

        //}
        #endregion

        #region 3DAPI
        // From 3DAPI

        internal int Width;
        internal int Height;
        internal int programID;
        
        internal bool mt = false;

        internal GLTexture2D rendertexture;
        internal Vector3     rendertexturePos;
        internal Vector3     rendertextureRot;

        internal ManualResetEvent eventcontrol = new ManualResetEvent(false);

        internal void ntfyReady()
        {
            DrawRenderCommands();
        }
        #endregion

        /// <summary>
        /// Used to specify explictly a version of OpenGL.
        /// </summary>
        public enum OpenGLVersion
        {
            /// <summary>
            /// Version 1.1
            /// </summary>
            [Version(1, 1)]
            OpenGL1_1,

            /// <summary>
            /// Version 1.2
            /// </summary>
            [Version(1, 2)]
            OpenGL1_2,

            /// <summary>
            /// Version 1.3
            /// </summary>
            [Version(1, 3)]
            OpenGL1_3,

            /// <summary>
            /// Version 1.4
            /// </summary>
            [Version(1, 4)]
            OpenGL1_4,

            /// <summary>
            /// Version 1.5
            /// </summary>
            [Version(1, 5)]
            OpenGL1_5,

            /// <summary>
            /// OpenGL 2.0
            /// </summary>
            [Version(2, 0)]
            OpenGL2_0,

            /// <summary>
            /// OpenGL 2.1
            /// </summary>
            [Version(2, 1)]
            OpenGL2_1,

            /// <summary>
            /// OpenGL 3.0. This is the first version of OpenGL that requires a specially constructed render context.
            /// </summary>
            [Version(3, 0)]
            OpenGL3_0,

            /// <summary>
            /// OpenGL 3.1
            /// </summary>
            [Version(3, 1)]
            OpenGL3_1,

            /// <summary>
            /// OpenGL 3.2
            /// </summary>
            [Version(3, 2)]
            OpenGL3_2,

            /// <summary>
            /// OpenGL 3.3
            /// </summary>
            [Version(3, 3)]
            OpenGL3_3,

            /// <summary>
            /// OpenGL 4.0
            /// </summary>
            [Version(4, 0)]
            OpenGL4_0,

            /// <summary>
            /// OpenGL 4.1
            /// </summary>
            [Version(4, 1)]
            OpenGL4_1,

            /// <summary>
            /// OpenGL 4.2
            /// </summary>
            [Version(4, 2)]
            OpenGL4_2,

            /// <summary>
            /// OpenGL 4.3
            /// </summary>
            [Version(4, 3)]
            OpenGL4_3,

            /// <summary>
            /// OpenGL 4.4
            /// </summary>
            [Version(4, 4)]
            OpenGL4_4
        }

        // EXPERIMENTAL / Working
        // ultraSandbox & System.Rendering
        #region -Experimental- @VAO
        // --------------------------------------------------------------
        // Vars used in functions BeginFixedPipeline3D/EndFixedPipeline3D to attempt to construct a 
        // gpu readable Array<struct> objects for gpu memory alocation
        // Eventualy a VAO object will be bound to the gpu using this 
        // information to describe the geometric, etc properties 
        protected List<Vertex2>                   m_vertexBuffer;
        protected Stack<List<Vertex2>>            m_VbStack;
        // --------------------------------------------------------------

        protected TextureFactoryBase              m_textureFactory;
        #endregion

        public RendererGL1x(/*enuGraphicsMode mode*/) 
            : base() //(mode)
        {
            //GraphicsMode = mode;

            m_textureFactory = new TextureFactory();

            //3DAPI
            //System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            //mthread.Start();
            //eventcontrol.WaitOne();
            //internwaterboard = new GLKeyboard(mwind.Keyboard);
            //internmouse = new GLMouse(mwind.Mouse);

            #region RenderContextProvider


            #endregion

        }


        #region ErrorLevel
        public override bool ErrorLevel(string contains)
        {
            //if (!GL.GetString(StringName.Extensions).Contains("GL_EXT_framebuffer_object"))
            //{
            //    throw new NotSupportedException(
            //         "GL_EXT_framebuffer_object extension is required. Please update your drivers.");
            //    Exit();
            //}

            throw new NotImplementedException();
        }
        #endregion

        #region Device
        //public override Drawables.BasicShader CreateBasicShader()
        //{
        //    throw new NotImplementedException();
        //}

        #region Texture


        #region TexUtil
        /// <summary>
        /// The TexUtil class is released under the MIT-license.
        /// /Olof Bjarnason
        /// </summary>
        public static class TexUtil
        {
            #region Public

            /// <summary>
            /// Initialize OpenGL state to enable alpha-blended texturing.
            /// Disable again with GL.Disable(EnableCap.Texture2D).
            /// Call this before drawing any texture, when you boot your
            /// application, eg. in OnLoad() of GameWindow or Form_Load()
            /// if you're building a WinForm app.
            /// </summary>
            public static void InitTexturing()
            {
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);
                //GL.DepthMask(true);
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
                //GL.Enable(EnableCap.CullFace);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.ShadeModel(OpenTK.Graphics.OpenGL.ShadingModel.Flat);
                //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
                
            }

            public static void EndTexturing()
            {
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            }

            /// <summary>
            /// Create an opaque OpenGL texture object from a given byte-array of r,g,b-triplets.
            /// Make sure width and height is 1, 2, .., 32, 64, 128, 256 and so on in size since all
            /// 3d graphics cards support those dimensions. Not necessarily square. Don't forget
            /// to call GL.DeleteTexture(int) when you don't need the texture anymore (eg. when switching
            /// levels in your game).
            /// </summary>
            public static int CreateRGBTexture(int width, int height, byte[] rgb)
            {
                return CreateTexture(width, height, false, rgb);
            }

            /// <summary>
            /// Create a translucent OpenGL texture object from given byte-array of r,g,b,a-triplets.
            /// See CreateRGBTexture for more info.
            /// </summary>
            public static int CreateRGBATexture(int width, int height, byte[] rgba)
            {
                return CreateTexture(width, height, true, rgba);
            }

            /// <summary>
            /// Create an OpenGL texture (translucent or opaque) from a given Bitmap.
            /// 24- and 32-bit bitmaps supported.
            /// </summary>
            public static int CreateTextureFromBitmap(Bitmap bitmap)
            {
               
                //if(TexLib.TextureManager.TextureList)
                int tex = GiveMeATexture(TextureUnit.Texture0);
                
                //GL.BindTexture(TextureTarget.Texture2D, tex);

                System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                          new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                          System.Drawing.Imaging.ImageLockMode.ReadOnly,
                          System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D.ToOpenGL(),
                  0,
                  OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba,
                  data.Width, data.Height,
                  0,
                  OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                  OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                  data.Scan0);
                
                bitmap.UnlockBits(data);
                
                SetParameters();
                
                return tex;
            }

            /// <summary>
            /// Create an OpenGL texture (translucent or opaque) by loading a bitmap
            /// from file. 24- and 32-bit bitmaps supported.
            /// </summary>
            public static int CreateTextureFromFile(string path)
            {
                return CreateTextureFromBitmap(new Bitmap(Bitmap.FromFile(path)));
            }

            #endregion

            private static int CreateTexture(int width, int height, bool alpha, byte[] bytes, TextureUnit textureUnit = TextureUnit.Texture0)
            {
                int expectedBytes = width * height * (alpha ? 4 : 3);
                Debug.Assert(expectedBytes == bytes.Length);
                int tex = GiveMeATexture(textureUnit);

                Upload(width, height, alpha, bytes);
                SetParameters();
                return tex;
            }

            private static int GiveMeATexture(TextureUnit textureUnit)
            {
                GL.ActiveTexture(textureUnit.ToOpenGL());

                int tex = 0;
                GL.GenTextures(1, out tex);
                GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), tex);
                return tex;
            }

            private static void SetParameters()
            {
                GL.TexParameter(
                  TextureTarget.Texture2D.ToOpenGL(),
                  TextureParameterName.TextureMinFilter.ToOpenGL(),
                  (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(),
                  TextureParameterName.TextureMagFilter.ToOpenGL(),
                  (int)TextureMagFilter.Linear);
            }

            private static void Upload(int width, int height, bool alpha, byte[] bytes)
            {
                OpenTK.Graphics.OpenGL.PixelInternalFormat internalFormat = alpha ? OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba : OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb;
                OpenTK.Graphics.OpenGL.PixelFormat format = alpha ? OpenTK.Graphics.OpenGL.PixelFormat.Rgba : OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
                GL.TexImage2D<byte>(
                  TextureTarget.Texture2D.ToOpenGL(),
                  0,
                  internalFormat,
                  width, height,
                  0,
                  format,
                  OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                  bytes);
            }
        }
        #endregion

        //public override Drawables.Texture2D CreateTexture(int Width, int Height)
        //{
        //    throw new NotImplementedException();
        //}

        //public override Drawables.Texture2D CreateTextureFromBitmap(System.Drawing.Bitmap bitmap)
        //{
        //    //BitmapData data = bitmap.LockBits(
        //    //  new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //    //  ImageLockMode.ReadOnly,
        //    //  Img.PixelFormat.Format32bppArgb);
        //    //var tex = GiveMeATexture();
        //    //GL.BindTexture(TextureTarget.Texture2D, tex);
        //    //GL.TexImage2D(
        //    //  TextureTarget.Texture2D,
        //    //  0,
        //    //  PixelInternalFormat.Rgba,
        //    //  data.Width, data.Height,
        //    //  0,
        //    //  PixelFormat.Bgra,
        //    //  PixelType.UnsignedByte,
        //    //  data.Scan0);
        //    //bitmap.UnlockBits(data);
        //    //SetParameters();
        //    //return tex;
        //}


        #endregion

        public override NthDimension.Graphics.Input.Keyboard DefaultKeyboard
        {
            get { throw new NotImplementedException(); }
        }


        //public override Mouse DefaultMouse
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public override NthDimension.Graphics.Input.Touchpad DefaultTouchpad
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region Rendering.Net

        public event EventHandler Created;

        public event EventHandler Disposed;

        private OpenTK.Graphics.GraphicsContext renderingContext = null;
        private IntPtr controlHandle;
        private Control __control;

        //private bool fullScreen = false;
        OpenTK.Platform.IWindowInfo Info;
        static object SYNC_OBJECT = new object();
        static OpenTK.Graphics.GraphicsContext currentRenderingContext = null;

        //public IRenderContextProvider RenderContextProvider
        //{
        //    get {  return }
        //}


        private IntPtr _renderContextHandle;
        public override IntPtr RenderContextHandle
        {
            get { return _renderContextHandle; }//RendererGL.currentRenderingContext.GraphicsMode.Index.Value; }
        }

        private IntPtr _deviceContextHandle;
        public override IntPtr DeviceContextHandle
        { 
            //get {  return Win32.GetDC(this.controlHandle); }
            get { return _deviceContextHandle; }
        }

        // For FrameBuffer?
        public int ImageWidth { get; protected set; }
        public int ImageHeight { get; protected set; }

        public void SyncExecute(Action a)
        {
            lock (SYNC_OBJECT)
            {
                if (this.renderingContext != currentRenderingContext)
                    MakeCurrent();
                a();
            }
        }

        void __control_Resize(object sender, EventArgs e)
        {
            if (__control.ClientSize.Width > 0 && __control.ClientSize.Height > 0)
            {
                ImageWidth = __control.ClientSize.Width;
                ImageHeight = __control.ClientSize.Height;
                GL.Viewport(0, 0, ImageWidth, ImageHeight);

                //foreach (var rt in renderTargets) rt.Dispose();

                //renderTargets.Clear();

                //backBufferRenderTarget = new RenderTargetsInfo { Width = ImageWidth, Height = ImageHeight, FB = 0, DepthSurface = 0, DestinationTextures = new TextureBuffer[] { null } };
                //renderTargets.Push(backBufferRenderTarget);
            }
        }

        #region MakeCurrent()
        public void MakeCurrent()
        {
            try
            {
                this.renderingContext.MakeCurrent(Info);
                GetError(true);
                currentRenderingContext = this.renderingContext;
            }
            catch (Exception)
            {
                // Re-try to build the device, usefull after hibernation, etc
               CreateDevice(this.__control);
                GetError(true);
            }
            
        }
        #endregion MakeCurrent()

        #region SwapBuffers()
        public void SwapBuffers()
        {
            renderingContext.SwapBuffers();
            GetError(true);
        }
        #endregion SwapBuffers()

        #region DestroyContexts()
        public void DestroyContexts()
        {
            if (renderingContext != null)
            {
                renderingContext.Dispose();
                renderingContext = null;
            }
        }
        #endregion DestroyContexts()

        #region IControlRenderDevice [Rendering.NET ]
        public bool IsCreated
        {
            get { return renderingContext != null; }
        }
        public void CreateDevice(System.Windows.Forms.Control __control)
        {
            if (this.__control == __control)
                return;
            this.controlHandle = __control.Handle;
            this.__control = __control;

            this.Info = OpenTK.Platform.Utilities.CreateWindowsWindowInfo(__control.Handle);

            renderingContext = new OpenTK.Graphics.GraphicsContext(OpenTK.Graphics.GraphicsMode.Default, Info);
            GetError(true);
            renderingContext.VSync = false;
            GetError(true);
            MakeCurrent();
            GetError(true);
            ((OpenTK.Graphics.IGraphicsContextInternal)renderingContext).LoadAll();

            __control.Resize += new EventHandler(__control_Resize);

            __control_Resize(null, null);

            if (renderingContext != null)
            {
                if (Created != null)
                    Created(this, EventArgs.Empty);
            }

            _renderContextHandle = ((OpenTK.Graphics.IGraphicsContextInternal)renderingContext).Context.Handle;
            _deviceContextHandle = Win32.GetDC(__control.Handle);
        }

        ISite site;
        public System.ComponentModel.ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }
        #endregion

        #endregion

        #region Graphics API (strings)
        public override string GraphicsApi
        {
            get
            {
                return GL.GetString(StringName.Renderer.ToOpenGL());
            }
        }
        public override string GraphicsApiVersion
        {
            get { return GL.GetString(StringName.Version.ToOpenGL()); }
        }
        #endregion

        #region 2D - TrueTypeFonts


       

        

        public override void DrawTrueTypeFont2D(int fontIndex, Vector3 color, string text, float top, float left, float scaleWidth, float scaleHeight, int format )
        {
            if(m_trueTypeFonts.Count <= 0)
                return;
            
            m_trueTypeFonts[fontIndex].color = color;
            m_trueTypeFonts[fontIndex].drawString(left, top, text, scaleWidth, scaleHeight);
        }
        #endregion

        #region 2D - User Interface
        public override void BeginFixedPipeline2D()
        {
            if (m_RestoreRenderState)
            {
                //// Get previous parameter values before changing them.
                GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.BlendSrc, out m_PrevBlendSrc);
                GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.BlendDst, out m_PrevBlendDst);
                GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.AlphaTestFunc, out m_PrevAlphaFunc);
                GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.AlphaTestRef, out m_PrevAlphaRef);

                m_WasBlendEnabled = GL.IsEnabled(OpenTK.Graphics.OpenGL.EnableCap.Blend);
                m_WasTexture2DEnabled = GL.IsEnabled(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                m_WasDepthTestEnabled = GL.IsEnabled(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            }

            //// Set default values and enable/disable caps.
            GL.BlendFunc(OpenTK.Graphics.OpenGL.BlendingFactorSrc.SrcAlpha, OpenTK.Graphics.OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
            GL.AlphaFunc(AlphaFunction.Greater, 1.0f);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);

            //m_VertNum = 0;
            m_DrawCallCount = 0;
            m_ClipEnabled = false;
            m_TextureEnabled = false;
            m_LastTextureID = -1;

            GL.EnableClientState(ArrayCap.VertexArray.ToOpenGL());
            GL.EnableClientState(ArrayCap.ColorArray.ToOpenGL());
            GL.EnableClientState(ArrayCap.TextureCoordArray.ToOpenGL());
        }
        public override void EndFixedPipeline2D()
        {
            Flush();

            if (m_RestoreRenderState)
            {
                GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), 0);

                // Restore the previous parameter values.
                GL.BlendFunc((OpenTK.Graphics.OpenGL.BlendingFactorSrc)m_PrevBlendSrc, (OpenTK.Graphics.OpenGL.BlendingFactorDest)m_PrevBlendDst);
                GL.AlphaFunc((AlphaFunction)m_PrevAlphaFunc, m_PrevAlphaRef);

                if (!m_WasBlendEnabled)
                    GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend);

                if (m_WasTexture2DEnabled && !m_TextureEnabled)
                    GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);

                if (m_WasDepthTestEnabled)
                    GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            }

            GL.DisableClientState(ArrayCap.VertexArray.ToOpenGL());
            GL.DisableClientState(ArrayCap.ColorArray.ToOpenGL());
            GL.DisableClientState(ArrayCap.TextureCoordArray.ToOpenGL());
        }
        #endregion

        #region [ FrameBuffer ]
        [Obsolete]
        public override void StartFrameBuffer(uint fboId, float width, float height)
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer.ToOpenGL(), fboId);
            
            GL.ClearColor(0f,0f,0f,1f);

            this.Clear(Renderer.ClearBufferMask.ColorBufferBit);

            Matrix4 orthographicProjection = Matrix4.CreateOrthographicOffCenter(0f, 
                                                                                 width, 
                                                                                 0f, 
                                                                                 height, 
                                                                                 -1f, 
                                                                                 1f);

            this.Viewport(0,0, (int) width, (int) height);
            this.MatrixMode(Renderer.MatrixMode.Projection);
            this.LoadMatrix(ref orthographicProjection);
        }
        [Obsolete]
        public override void EndFrameBuffer(uint fboId, float screenWidth, float screenHeight)
        {
            Matrix4 orthographicProjection = Matrix4.CreateOrthographicOffCenter(0f,
                                                                                 screenWidth,
                                                                                 0f,
                                                                                 screenHeight,
                                                                                 -1f,
                                                                                 1f);

            this.MatrixMode(Renderer.MatrixMode.Projection);
            this.LoadMatrix(ref orthographicProjection);
            this.Viewport(0, 0, (int)screenWidth, (int)screenHeight);

            //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);        // NOTE:: Shouldn't 0 be fboId>>>
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt.ToOpenGL(), fboId);       
        }
        #endregion

        public override string GetString(StringName sName)
        {
            //string ret = GL.GetString(sName.ToOpenGL());
            //GetError(true);
            //return ret;

            throw new NotImplementedException();
        }
        public override ErrorCode GetError(bool raiseUIevent = false)
        {
            //if (_NO_CHECK_ERR_INSIDE_BEGIN_END_BLOCK)   // MUST NOT CALL GL.GetError inside glBegin()-glEnd() Block
            //    return ErrorCode.NoError;

            //ErrorCode ret =  ErrorCode.NoError;

            //try
            //{
            //    ret = GL.GetError().ToSyscon();

            //    if (raiseUIevent && ret != ErrorCode.NoError)
            //        RaiseApplicationError(string.Format("{0} from {1} at {2}", ret, Bend.WhoCalls.WhoCalledMe().ToString(), Bend.WhoCalls.StackTrace().ToString()), ErrorSeverity.ERROR);
            //}
            //catch { }

            //return ret;

            throw new NotImplementedException();
        }

        #region [ OpenGL 1.x ]
        private bool _NO_CHECK_ERR_INSIDE_BEGIN_END_BLOCK = false;
        public override void BeginFixedPipeline3D(enuPolygonMode beginMode)
        {
            _NO_CHECK_ERR_INSIDE_BEGIN_END_BLOCK = true;
            GL.Begin(beginMode.ToOpenGL());
            GetError(true);
            //m_vertexBuffer.Clear();             // Start a new object-entity by clearing all its vertex information


            //((ITextureRenderDevice)this).BeginScene(new TextureBuffer[0]);

        }       
        public override void EndFixedPipeline3D()
        {
            //m_VbStack.Push(m_vertexBuffer);     // The new object-entity has finished, store it   // TODO:: I need to call             _buffers.Pop(); //somewhere
            GL.End();
            _NO_CHECK_ERR_INSIDE_BEGIN_END_BLOCK = false;
            GetError(true);
        }

        public override void BeginFixedPipelineOrtho(System.Drawing.Rectangle clientRectangle)
        {
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection); GetError(true);
            GL.PushMatrix(); GetError(true);
            GL.LoadIdentity(); GetError(true);

            GL.Ortho(0, clientRectangle.Width, clientRectangle.Height, 0, -1.0, 100.0); GetError(true);

            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview); GetError(true);
            GL.PushMatrix(); GetError(true);
            GL.LoadIdentity(); GetError(true);
        }
        public override void EndFixedPipelineOrtho()
        {
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview); GetError(true);
            GL.PopMatrix(); GetError(true);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection); GetError(true);
            GL.PopMatrix(); GetError(true);
        }

        public override bool Blend
        {
            get
            {
                bool dt;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.Blend, out dt);
                return dt;
            }
            set
            {
                if (value)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend); GetError(true); }
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend); GetError(true); }
            }
        }
        public override void BlendEquation(BlendEquationMode mode)
        {
            OpenTK.Graphics.OpenGL.GL.BlendEquation(mode.ToOpenGL()); GetError(true);
        }
        public override void BlendFunc(Renderer.BlendingFactorSrc src, Renderer.BlendingFactorDest dest)
        {
            GL.BlendFunc(src.ToOpenGL(), dest.ToOpenGL()); GetError(true);
        }

        public override void BeginAttributes()
        {
            GL.PushAttrib(AttribMask.AllAttribBits.ToOpenGL()); GetError(true);
        }
        public override void EndAttributes()
        {
            GL.PopAttrib(); GetError(true);
        }

        public override void Clear(Renderer.ClearBufferMask mask)
        {
            GL.Clear(mask.ToOpenGL()); GetError(true);
        }
        public override void ClearDepth(double depth)
        {
            GL.ClearDepth(depth); GetError(true);
        }
        public override Color ClearColor
        {
            get { return m_clearColor; }
            set
            {
                m_clearColor = value;
                GL.ClearColor(value); GetError(true);
            }
        }

        public override bool CullFace
        {
            get
            {
                bool cf;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.CullFace, out cf);
                return cf;
            }
            set
            {
                if (value)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.CullFace); GetError(true); }
                //GL.CullFace(CullFaceMode
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.CullFace); GetError(true); }
            }
        }
        public override void CullMode(Renderer.enuCullFaceMode mode)
        {
            if (mode == enuCullFaceMode.Front)
            { GL.CullFace(OpenTK.Graphics.OpenGL.CullFaceMode.Front); GetError(true); }

            if (mode == enuCullFaceMode.Back)
            { GL.CullFace(OpenTK.Graphics.OpenGL.CullFaceMode.Back); GetError(true); }

            if (mode == enuCullFaceMode.FrontAndBack)
            { GL.CullFace(OpenTK.Graphics.OpenGL.CullFaceMode.FrontAndBack); GetError(true); }
        }

        public override bool DepthTest
        {
            get
            {
                bool dt;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.DepthTest, out dt);
                return dt;
            }
            set
            {
                if (value)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest); GetError(true); }
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest); GetError(true); }
            }
        }
        public override void DepthFunc(Renderer.DepthFunction depth)
        {
            GL.DepthFunc(depth.ToOpenGL()); GetError(true);
        }
        public override void DepthMask(bool mask)
        {
            GL.DepthMask(mask); GetError(true);
        }

        public override bool Lighting
        {
            // default should be true
            set
            {
                if (value == true)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true); }
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true); }
                }
            get
            {
                bool l = false;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.Lighting, out l);
                return l;
            }
        }
        public override float LineWidth
        {
            set
            {
                GL.LineWidth(value); GetError(true);
            }
            get
            {
                float lw = 0;
                GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.LineWidth, out lw);
                return lw;
            }
        }
        public override void Color3(float r, float g, float b)
        {
            GL.Color3(r, g, b); GetError(true);
        }
        public override void Color4(System.Drawing.Color color)
        {
            GL.Color4(color); GetError(true);
        }
        public override void Color4(Color4 color)
        {
            GL.Color4(color.ToOpenGL()); GetError(true);
        }

        public override void Color4(float r, float g, float b, float a)
        {
            GL.Color4(r,g,b,a); GetError(true);
        }
        public override void TexCoord2(float s, float t)
        {
            GL.TexCoord2(s,t); GetError(true);
        }

        public override void Vertex3(float x, float y, float z)
        {
            GL.Vertex3(x, y, z); GetError(true);
        }

        public override void Dispose()
        {
            DestroyContexts();
            
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        public override unsafe void Flush()
        {
            GL.Flush(); GetError(true);
        }

        public override void Enable(EnableCap cap)
        {
            GL.Enable(cap.ToOpenGL()); GetError(true);
        }
        public override void Disable(EnableCap cap)
        {
            GL.Disable(cap.ToOpenGL()); GetError(true);
        }

        public override void HighQualityLines(bool blending = false, bool depthMask = false)
        {
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.LineSmooth); GetError(true);

            if (blending)
            {
                Blend = true;
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend); GetError(true);
                this.BlendFunc(NthDimension.Graphics.Renderer.BlendingFactorSrc.SrcAlpha, NthDimension.Graphics.Renderer.BlendingFactorDest.OneMinusSrcAlpha); GetError(true);
            }

            GL.DepthMask(depthMask); GetError(true);// false          
            GL.Hint(OpenTK.Graphics.OpenGL.HintTarget.LineSmoothHint, OpenTK.Graphics.OpenGL.HintMode.Nicest); GetError(true);
        }
        public override void HighQualityModels(bool blending = true)
        {
            ////	Set parameters that give us some high quality settings.
            //GL.DepthMask(true);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.ColorMaterial);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Normalize);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Light0);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            ////GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Normalize);
            ////GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            ////GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            //GL.ShadeModel(OpenTK.Graphics.OpenGL.ShadingModel.Smooth);
            ////GL.LightModel(OpenTK.Graphics.OpenGL.LightModelParameter.LightModelTwoSide, 1);
            //GL.LightModel(LightModelParameter.LightModelAmbient, 1);

            //if (blending)
            //{
            //    GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            //    this.BlendFunc(SYSCON.Graphics.Renderer.BlendingFactorSrc.SrcAlpha, SYSCON.Graphics.Renderer.BlendingFactorDest.OneMinusSrcAlpha);
            //}

            //GL.Hint(OpenTK.Graphics.OpenGL.HintTarget.PerspectiveCorrectionHint, OpenTK.Graphics.OpenGL.HintMode.Nicest);
            ////GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0, 0, 0, 1 });
            //// Set up some lighting state that never changes
            //GL.ShadeModel(OpenTK.Graphics.OpenGL.ShadingModel.Smooth);

            //	Set parameters that give us some high quality settings.
            GL.DepthMask(true); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.ColorMaterial); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Normalize); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Light0); GetError(true);
            GL.LightModel(OpenTK.Graphics.OpenGL.LightModelParameter.LightModelTwoSide, 1); GetError(true);
            //GL.LightModel(LightModelParameter.LightModelAmbient, 1);
            GL.ShadeModel(OpenTK.Graphics.OpenGL.ShadingModel.Smooth); GetError(true);
            GL.Hint(OpenTK.Graphics.OpenGL.HintTarget.PerspectiveCorrectionHint, OpenTK.Graphics.OpenGL.HintMode.Nicest); GetError(true);

            if (blending)
            {
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend); GetError(true);
                this.BlendFunc(NthDimension.Graphics.Renderer.BlendingFactorSrc.SrcAlpha, NthDimension.Graphics.Renderer.BlendingFactorDest.OneMinusSrcAlpha); GetError(true);
            }
        }

        public override void Hint(Renderer.HintTarget target, Renderer.HintMode mode)
        {
            //Hint(HintTarget target, HintMode mode)
            GL.Hint(target.ToOpenGL(), mode.ToOpenGL()); GetError(true);
        }

        public override void LightModel(LightModelParameter pname, int param)
        {
            GL.LightModel(pname.ToOpenGL(), param); GetError(true);
        }
        public override void ShadeModel(Renderer.ShadingModel model)
        {
            GL.ShadeModel(model.ToOpenGL()); GetError(true);
        }

        public override void SetRenderTarget(Drawables.Texture2D texture, Algebra.Vector3 campos, Algebra.Vector3 camrot)
        {
            rendertexturePos = campos;
            rendertextureRot = camrot;
            rendertexture = texture as GLTexture2D;
        }

        
        

        //public override void Enabled(Model model)
        //{
        //    //Mesh model.Mesh = model.Mesh;
        //    //_renderer.PushMatrix();                                              // Moved to Render()
        //    //_renderer.MultMatrix(this.Transformation.WorldMatrix.ToArray());     // Moved to Render()


        //    Color auxColor = Color.Black;
        //    Algebra.Vector3 tNormal = new Algebra.Vector3();
        //    int auxContPoli = 0;                // Todo:: Investigate whether this affects rendering

        //    bool auxPickingFaces = ((model.SelectionMode == enuSelectionType.QuadFaces) && model.IsFocused && model.IsPicking) || model.FindUnfocused;
        //    bool auxPickingEdges = (model.SelectionMode == enuSelectionType.Edges) && model.IsFocused;

        //    #region Enabled QuadFaces              [ drawFace ] glVertex3

        //    Lighting = true;
        //    #region for each Triangle
        //    // TODO:: Requires m_selectionMode check? See Vertices...


        //    for (int i = 0; i < model.Mesh.QuadFaces.Count; i++)
        //    {
        //        Face face = model.Mesh.QuadFaces[i];

        //        //if (modelMesh.Normals.Count > 0)
        //        //    tNormal = modelMesh.Normals[i].n1; // TODO:: Drop Normal from Face Class

        //        if (model.Mesh.QuadFaces[i].VertexIndices.Count == 3)
        //            tNormal = model.Mesh.QuadFaces[i].GetTriangleNormal(
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.Z),
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.Z),
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.Z));

        //        if (model.Mesh.QuadFaces[i].VertexIndices.Count == 4)
        //            tNormal = model.Mesh.QuadFaces[i].GetQuadNormal(new Algebra.Vector3(
        //                model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[0].PositionIndex].Position.Z),
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[1].PositionIndex].Position.Z),
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[2].PositionIndex].Position.Z),
        //                new Algebra.Vector3(model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[3].PositionIndex].Position.X,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[3].PositionIndex].Position.Y,
        //                            model.Mesh.Positions[(int)model.Mesh.QuadFaces[i].VertexIndices[3].PositionIndex].Position.Z));

        //        if (face.PreSelected)
        //        {


        //            if (face.Selected)
        //                auxColor = model.ColorPreAndSelected;
        //            else
        //                auxColor = model.ColorPreSelected;

        //            DrawFace(face.VertexIndices,
        //                                model.Mesh.Positions,
        //                                tNormal,
        //                                auxColor,
        //                                auxContPoli,
        //                                model.Mesh.Id,
        //                                auxPickingFaces);

        //        }
        //        else if (face.Selected)
        //        {
        //            if (model.ExtrudeAction)
        //                auxColor = model.ColorExtruded;
        //            else
        //                auxColor = model.ColorSelected;

        //            DrawFace(face.VertexIndices,
        //                                        model.Mesh.Positions,
        //                                        tNormal,
        //                                        auxColor,
        //                                        auxContPoli,
        //                                        model.Mesh.Id,
        //                                        auxPickingFaces);
        //        }
        //        else
        //            DrawFace(face.VertexIndices,
        //                                model.Mesh.Positions,
        //                                tNormal,
        //                                model.MeshColor,
        //                                auxContPoli,
        //                                model.Mesh.Id,
        //                                auxPickingFaces);
        //        auxContPoli++;
        //    }
        //    #endregion
        //    Lighting = false;

        //    #endregion  Enabled QuadFaces

        //    #region Enabled Normals    [          ] glVertex3

        //    //if (ShowNormals)
        //    //{
        //    //    //	Set the colour to red.
        //    //    _renderer.PushAttrib(AttribMask.AllAttribBits);
        //    //    _renderer.Disable(EnableCap.Lighting);

        //    //    GraphicsHelper.BeginDraw(BeginMode.Lines);
        //    //    _renderer.Color4(1, 0, 0, 1);
        //    //    #region foreach Triangle

        //    //    foreach (Face tri in modelMesh.QuadFaces)
        //    //    {
        //    //        #region foreach VertexIndex

        //    //        foreach (VertexIndex vxi in tri.VertexIndices)
        //    //        {
        //    //            if (vxi.NormalIndex != -1 &&
        //    //                vxi.PositionIndex != -1)
        //    //            {
        //    //                Algebra.Vector3 enuDock = modelMesh.Positions[(int)vxi.PositionIndex].Position;
        //    //                Algebra.Vector3 nor = modelMesh.Positions[(int)vxi.PositionIndex].Normal;
        //    //                Algebra.Vector3 v = enuDock + nor;

        //    //                _renderer.Vertex3(enuDock);
        //    //                _renderer.Vertex3(v);
        //    //            }
        //    //        }

        //    //        #endregion
        //    //    }

        //    //    #endregion
        //    //    GraphicsHelper.EndDraw();

        //    //    _renderer.PopAttrib(); //	Restore the attributes.

        //    //}

        //    #endregion

        //    #region Enabled Edges                      [ drawLine ]  glVertex3
        //    if (model.SelectionMode == enuSelectionType.Edges)
        //        LineWidth = 2f;
        //    else
        //        LineWidth = 1f;

        //    #region for each Edge
        //    for (int i = 0; i < model.Mesh.Edges.Count; i++)
        //    {
        //        Edge edge = model.Mesh.Edges[i];
        //        bool colorValid = true;

        //        if (edge.PreSelected)
        //            if (edge.Selected)
        //                auxColor = model.ColorPreAndSelected;
        //            else
        //                auxColor = model.ColorPreSelected;
        //        else if (edge.Selected && model.SelectionMode == enuSelectionType.Edges)
        //            auxColor = model.ColorSelected;
        //        else if (model.IsFocused)
        //            auxColor = model.ColorNoSelected;
        //        else
        //            colorValid = false;

        //        if (colorValid)
        //        {
        //            DrawLine(edge.VertexA, edge.VertexB, auxColor,
        //                                (int)edge.Id, model.Mesh.Id, auxPickingEdges);
        //        }
        //    }
        //    #endregion
        //    #endregion Enabled Edges

        //    //GlHelper_shaderProgram.LineWidth = 1.0f;       

        //    #region Enabled Vertices                   [ drawVertex ]  glVertex3
        //    if (model.SelectionMode == enuSelectionType.Vertices)      // TODO:: Why this call is not included in Triangle and Edge calls as well? - Because it draws the Vertices small boxes
        //    {
        //        int cp = 0;

        //        for (int j = 0; j < model.Mesh.Positions.Count; j++)
        //        #region for each Vertex
        //        {
        //            Algebra.Vertex v = model.Mesh.Positions[j];
        //            bool colorValid = true;

        //            v.Index = (uint)cp;
        //            if (v.IsPreselected)
        //                if (v.IsSelected)
        //                    auxColor = model.ColorPreAndSelected;
        //                else
        //                    auxColor = model.ColorPreSelected;
        //            else if (v.IsSelected)
        //                auxColor = model.ColorSelected;
        //            else
        //                if (model.IsFocused)
        //                    auxColor = model.ColorNoSelected;
        //                else
        //                    colorValid = false;

        //            if (colorValid)
        //            {
        //                if (model.Mesh.Id < 0 || v.Index < 0)
        //                {
        //                }

        //                DrawVertex(v, auxColor, model.Mesh.Id, model.IsFocused);
        //            }
        //            cp++;
        //        }
        //        #endregion
        //    }
        //    #endregion Enabled Vertices

        //    //_renderer.PopMatrix();                                               // Moved to Render()
        //    //_renderer.PopAttrib();
        //}
        public override void Draw(Scenegraph.SceneRoot sceneRoot)
        {
            throw new NotImplementedException();
        }
        public override void DrawAABB(BoundingVolume volume)
        {
            {
                this.PushAttrib(AttribMask.CurrentBit |
                                AttribMask.DepthBufferBit |
                                AttribMask.EnableBit |
                                AttribMask.HintBit |
                                AttribMask.LightingBit |
                                AttribMask.LineBit |
                                AttribMask.PolygonBit |
                                AttribMask.TransformBit);

                float oldLineWidth = this.LineWidth;

                GL.Color4(1f, 0.2f, 0.2f, 1f); GetError(true);
                
                this.LineWidth = 1f;
                this.PolygonMode(MaterialFace.Front, Renderer.PolygonMode.Line); //renderMode == enuRenderMode.Wire ? PolygonMode.Fill : PolygonMode.Line);
                this.HighQualityLines();

                this.DepthTest = true;

                this.BeginFixedPipeline3D(enuPolygonMode.Quads); // Draw The Cube Using quads

                InsertVertex(volume.Hhl); // Top Right Of The Quad (Top)
                InsertVertex(volume.Lhl); // Top Left Of The Quad (Top)
                InsertVertex(volume.Lhh); // Bottom Left Of The Quad (Top)
                InsertVertex(volume.Hhh); // Bottom Right Of The Quad (Top)
                InsertVertex(volume.Hlh); // Top Right Of The Quad (Bottom)
                InsertVertex(volume.Llh); // Top Left Of The Quad (Bottom)
                InsertVertex(volume.Lll); // Bottom Left Of The Quad (Bottom)
                InsertVertex(volume.Hll); // Bottom Right Of The Quad (Bottom)
                InsertVertex(volume.Hhh); // Top Right Of The Quad (Front)
                InsertVertex(volume.Lhh); // Top Left Of The Quad (Front)
                InsertVertex(volume.Llh); // Bottom Left Of The Quad (Front)
                InsertVertex(volume.Hlh); // Bottom Right Of The Quad (Front)
                InsertVertex(volume.Hll); // Top Right Of The Quad (Back)
                InsertVertex(volume.Lll); // Top Left Of The Quad (Back)
                InsertVertex(volume.Lhl); // Bottom Left Of The Quad (Back)
                InsertVertex(volume.Hhl); // Bottom Right Of The Quad (Back)
                InsertVertex(volume.Lhh); // Top Right Of The Quad (Left)
                InsertVertex(volume.Lhl); // Top Left Of The Quad (Left)
                InsertVertex(volume.Lll); // Bottom Left Of The Quad (Left)
                InsertVertex(volume.Llh); // Bottom Right Of The Quad (Left)
                InsertVertex(volume.Hhl); // Top Right Of The Quad (Right)
                InsertVertex(volume.Hhh); // Top Left Of The Quad (Right)
                InsertVertex(volume.Hlh); // Bottom Left Of The Quad (Right)
                InsertVertex(volume.Hll); // Bottom Right Of The Quad (Right)

                this.EndFixedPipeline3D(); // End Drawing The Cube

                this.LineWidth = oldLineWidth;

                this.PopAttrib();
                
            }
        }
        public override void DrawAngle(Algebra.Vector3 orig, Algebra.Vector3 vtx, Algebra.Vector3 vty, float angle)
        {
            // Convert to Mesh

            int i = 0;

            DepthTest = false;     //GL.Disable(EnableCap.DepthTest);
            CullFace = false;      // GL.Disable(EnableCap.CullFace);
            Lighting = false;      //GL.Disable(EnableCap.Lighting);

            Blend = true;          //GL.Enable(EnableCap.Blend);

            BlendFunc(Renderer.BlendingFactorSrc.SrcAlpha, Renderer.BlendingFactorDest.OneMinusSrcAlpha);

            GL.Color4(1, 1, 0, 0.5f); GetError(true);

            BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            InsertVertex(orig);

            for (i = 0; i <= 50; i++)
            {
                Algebra.Vector3 vt;
                vt = vtx * (float)Math.Cos(((angle) / 50) * i);
                vt += vty * (float)Math.Sin(((angle) / 50) * i);
                vt += orig;
                InsertVertex(vt.X, vt.Y, vt.Z);
            }
            EndFixedPipeline3D();

            Blend = false;

            GL.Color4(1, 1, 0.2f, 1); GetError(true);

            BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            InsertVertex(orig);
            for (i = 0; i <= 50; i++)
            {
                Algebra.Vector3 vt;
                vt = vtx * (float)Math.Cos(((angle) / 50) * i);
                vt += vty * (float)Math.Sin(((angle) / 50) * i);
                vt += orig;
                InsertVertex(vt.X, vt.Y, vt.Z);
            }
            EndFixedPipeline3D();
        }
        public override void DrawAxis(Algebra.Vector3 orig, Algebra.Vector3 axis, Algebra.Vector3 vtx, Algebra.Vector3 vty, float fct, float fct2, Algebra.Vector4 color)
        {
            DepthTest = false;   // GL.Disable(EnableCap.DepthTest);
            Lighting = false;    // GL.Disable(EnableCap.Lighting);

            GL.Color4(ref color.X); GetError(true);

            BeginFixedPipeline3D(enuPolygonMode.Lines);
            InsertVertex(orig);
            InsertVertex(orig.X + axis.X, orig.Y + axis.Y, orig.Z + axis.Z);
            EndFixedPipeline3D();

            BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            for (int i = 0; i <= 30; i++)
            {
                Algebra.Vector3 pt;
                pt = vtx * (float)Math.Cos(((2 * MathHelper.Pi) / 30.0f) * i) * fct;
                pt += vty * (float)Math.Sin(((2 * MathHelper.Pi) / 30.0f) * i) * fct;
                pt += axis * fct2;
                pt += orig;
                InsertVertex(pt);

                pt = vtx * (float)Math.Cos(((2 * MathHelper.Pi) / 30.0f) * (i + 1)) * fct;
                pt += vty * (float)Math.Sin(((2 * MathHelper.Pi) / 30.0f) * (i + 1)) * fct;
                pt += axis * fct2;
                pt += orig;
                InsertVertex(pt);

                InsertVertex(orig.X + axis.X, orig.Y + axis.Y, orig.Z + axis.Z);
            }
            EndFixedPipeline3D();

            DepthTest = true;
            Lighting = true;
        }
        public override void DrawCircle(Algebra.Vector3 orig, float r, float g, float b, Algebra.Vector3 vtx, Algebra.Vector3 vty)
        {
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest); GetError(true);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true);
            GL.Color4(r, g, b, 1f); GetError(true);

            BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            for (int i = 0; i < 50; i++)
            {
                Algebra.Vector3 vt;
                vt = vtx * (float)Math.Cos((2 * MathHelper.Pi / 50) * i);
                vt += vty * (float)Math.Sin((2 * MathHelper.Pi / 50) * i);
                vt += orig;
                InsertVertex(vt.X, vt.Y, vt.Z);
            }
            EndFixedPipeline3D();
        }
        public override void DrawCircleX(float size, float lineWidth, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex(0.0f, (float)(Math.Sin((double)rads) * (double)size), (float)(Math.Cos((double)rads) * (double)size));
            EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleX(float size, float lineWidth, System.Drawing.Color unColor, float posx = 0f, float posy = 0f, float posz = 0f)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex(posx, (float)(posy + Math.Sin((double)rads) * (double)size), (float)(posz + Math.Cos((double)rads) * (double)size));
            this.EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleY(float size, float lineWidth, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex((float)(Math.Sin((double)rads) * (double)size), 0.0f, (float)(Math.Cos((double)rads) * (double)size));
            this.EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleY(float size, float lineWidth, System.Drawing.Color unColor, float posx = 0f, float posy = 0f, float posz = 0f)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex((float)(posx + Math.Sin((double)rads) * (double)size), posy, (float)(posz + Math.Cos((double)rads) * (double)size));
            this.EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleZ(float size, float lineWidth, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex((float)(Math.Sin((double)rads) * (double)size), (float)(Math.Cos((double)rads) * (double)size), 0.0f);
            this.EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleZ(float size, float lineWidth, System.Drawing.Color unColor, float posx = 0f, float posy = 0f, float posz = 0f)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.LineWidth(lineWidth); GetError(true);
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.LineLoop);
            float rads_step = MathHelper.Pi * 0.01f;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += rads_step)
                InsertVertex((float)(posx + Math.Sin((double)rads) * (double)size), (float)(posy + Math.Cos((double)rads) * (double)size), posz);
            this.EndFixedPipeline3D();
            GL.LineWidth(1.0f); GetError(true);
        }
        public override void DrawCircleHalf(Algebra.Vector3 orig, float r, float g, float b, Algebra.Vector3 vtx, Algebra.Vector3 vty, ref Algebra.Vector4 camPlan)
        {
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest); GetError(true);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting); GetError(true);
            GL.Color4(r, g, b, 1f); GetError(true);

            BeginFixedPipeline3D(enuPolygonMode.LineStrip);
            for (int i = 0; i < 30; i++)
            {
                Algebra.Vector3 vt;
                vt = vtx * (float)Math.Cos((MathHelper.Pi / 30) * i);
                vt += vty * (float)Math.Sin((MathHelper.Pi / 30) * i);
                vt += orig;
                if (!float.IsNaN(camPlan.DotNormal(vt))) // float.NaN
                    InsertVertex(vt.X, vt.Y, vt.Z);
            }
            EndFixedPipeline3D();
        }
        public override void DrawConeX(float width, float height, float offset)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);

            InsertVertex(offset + height, 0f, 0f);

            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(offset, x, z);
            }
            x = width;
            z = 0;

            InsertVertex(offset, x, z);

            this.EndFixedPipeline3D();
        }
        public override void DrawConeX(float width, float height, float offset, ref Algebra.BoundingAABB aabb)
        {
            Algebra.Vector3 minVector;
            Algebra.Vector3 maxVector;

            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);

            InsertVertex(offset + height, 0f, 0f);

            minVector = new Algebra.Vector3(offset + height, 0f, 0f);
            maxVector = new Algebra.Vector3(offset + height, 0f, 0f);

            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(offset, x, z);

                minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(offset, x, z));
                maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(offset, x, z));
            }
            x = width;
            z = 0;

            InsertVertex(offset, x, z);

            minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(offset, x, z));
            maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(offset, x, z));

            aabb = new BoundingAABB(minVector, maxVector);

            this.EndFixedPipeline3D();
        }
        public override void DrawConeY(float width, float height, float offset)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            InsertVertex(0f, offset + height, 0f);
            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(x, offset, z);
            }
            x = width;
            z = 0;
            InsertVertex(x, offset, z);
            this.EndFixedPipeline3D();
        }
        public override void DrawConeY(float width, float height, float offset, ref Algebra.BoundingAABB aabb)
        {
            Algebra.Vector3 minVector;
            Algebra.Vector3 maxVector;

            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            InsertVertex(0f, offset + height, 0f);

            minVector = new Algebra.Vector3(0f, offset + height, 0f);
            maxVector = new Algebra.Vector3(0f, offset + height, 0f);
            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(x, offset, z);

                minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(x, offset, z));
                maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(x, offset, z));
            }
            x = width;
            z = 0;
            InsertVertex(x, offset, z);

            minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(x, offset, z));
            maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(x, offset, z));

            aabb = new BoundingAABB(minVector, maxVector);

            this.EndFixedPipeline3D();
        }
        public override void DrawConeZ(float width, float height, float offset)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            InsertVertex(0f, 0f, offset + height);
            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(x, z, offset);
            }
            x = width;
            z = 0;
            InsertVertex(x, z, offset);
            this.EndFixedPipeline3D();
        }
        public override void DrawConeZ(float width, float height, float offset, ref Algebra.BoundingAABB aabb)
        {
            Algebra.Vector3 minVector;
            Algebra.Vector3 maxVector;
            // From http://code.google.com/p/3d-editor-toolkit/
            this.BeginFixedPipeline3D(enuPolygonMode.TriangleFan);
            InsertVertex(0f, 0f, offset + height);

            minVector = new Algebra.Vector3(0f, 0f, offset + height);
            maxVector = new Algebra.Vector3(0f, 0f, offset + height);

            float x, z;
            for (float rads = 0.0f; rads < MathHelper.Pi * 2.0f; rads += 0.1f)
            {
                x = ((float)Math.Cos((double)rads) * width);
                z = ((float)Math.Sin((double)rads) * width);
                InsertVertex(x, z, offset);

                minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(x, z, offset));
                maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(x, z, offset));
            }
            x = width;
            z = 0;
            InsertVertex(x, z, offset);

            minVector = Algebra.Vector3.Min(minVector, new Algebra.Vector3(x, z, offset));
            maxVector = Algebra.Vector3.Max(maxVector, new Algebra.Vector3(x, z, offset));

            aabb = new BoundingAABB(minVector, maxVector);

            this.EndFixedPipeline3D();
        }
        public override void DrawCube(float size, float posX, float posY, float posZ, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            uint[] indices =
                {
                    0, 1, 2,
                    3, 2, 1,
                    4, 0, 6,
                    6, 0, 2,
                    5, 1, 4,
                    4, 1, 0,
                    7, 3, 1,
                    7, 1, 5,
                    5, 4, 7,
                    7, 4, 6,
                    7, 2, 3,
                    7, 6, 2
                };

            float[] vertices =
                {	
                    1,  1,  1,
                    -1,  1,  1,
                    1, -1,  1,
                    -1, -1,  1,
                    1,  1, -1,
                    -1,  1, -1,
                    1, -1, -1,
                    -1, -1, -1
                };

            size *= 0.5f;

            GL.PushMatrix();        // Νέο local spaceGetError(true);

            // MultMatrix εδώ αν ήθελα να το μετακινήσω και πάνω στο κόσμο...

            // Εδώ είμαι local-space
            GL.Translate(posX, posY, posZ); GetError(true);
            GL.Scale(size, size, size); GetError(true);
            GL.Color4(unColor); GetError(true);
            GL.EnableClientState(OpenTK.Graphics.OpenGL.EnableCap.VertexArray); GetError(true);
            GL.VertexPointer(3, VertexPointerType.Float.ToOpenGL(), 0, vertices); GetError(true);
            GL.DrawElements(OpenTK.Graphics.OpenGL.BeginMode.Triangles, 36, DrawElementsType.UnsignedInt.ToOpenGL(), indices); GetError(true);
            GL.DisableClientState(OpenTK.Graphics.OpenGL.EnableCap.VertexArray); GetError(true);

            GL.PopMatrix(); GetError(true);    // Έξοδος local space
        }
        public override void DrawFace(uint[] indices, List<Algebra.Vertex> vertices, Algebra.Vector3 normal, System.Drawing.Color unColor, int poliId, uint obj3dId, bool picking)
        {
            // TODO:: No
            if (poliId >= 0 && obj3dId >= 0)
            {
                GL.Color4(unColor); GetError(true);

                //if (picking) PickingHelper.StartPickingNames(obj3dId, (uint)poliId);
                this.BeginFixedPipeline3D(enuPolygonMode.Polygon);
                foreach (uint indice in indices)
                {
                    GL.Normal3(normal.X, normal.Y, normal.Z); GetError(true);
                    InsertVertex(vertices[(int)indice].Position.X, vertices[(int)indice].Position.Y, vertices[(int)indice].Position.Z);
                }
                this.EndFixedPipeline3D();

                //if (picking) PickingHelper.EndElement();
            }
        }
        public override void DrawFace(Dictionary<uint, Algebra.VertexIndex> indices, List<Algebra.Vertex> vertices, Algebra.Vector3 normal, System.Drawing.Color unColor, int poliId, uint obj3dId, bool picking)
        {
            // PropertiesMatrix implementation
            if (poliId >= 0 && obj3dId >= 0)
            {
                GL.Color4(unColor); GetError(true);

                //if (picking) PickingHelper.StartPickingNames(obj3dId, (uint)poliId);
                this.BeginFixedPipeline3D(enuPolygonMode.Polygon);
                foreach (KeyValuePair<uint, VertexIndex> indice in indices)
                {
                    GL.Normal3(normal.X, normal.Y, normal.Z); GetError(true);
                    InsertVertex(vertices[(int)indice.Key].Position.X, vertices[(int)indice.Key].Position.Y, vertices[(int)indice.Key].Position.Z);
                }
                this.EndFixedPipeline3D();

                //if (picking) PickingHelper.EndElement();
            }
        }
        public override void DrawFace(List<uint> indices, List<Algebra.Vertex> vertices, Algebra.Vector3 normal, System.Drawing.Color unColor, int poliId, uint obj3dId, bool picking)
        {
            try
            {
                if (poliId >= 0 && obj3dId >= 0)
                {
                    GL.Color4(unColor); GetError(true);

                    //if (picking) PickingHelper.StartPickingNames(obj3dId, (uint)poliId);
                    this.BeginFixedPipeline3D(enuPolygonMode.Polygon);
                    foreach (uint indice in indices)
                    {
                        GL.Normal3(normal.X, normal.Y, normal.Z); GetError(true);
                        InsertVertex(vertices[(int)indice].Position.X, vertices[(int)indice].Position.Y, vertices[(int)indice].Position.Z);
                    }
                    this.EndFixedPipeline3D();

                    //if (picking) PickingHelper.EndElement();
                }
            }
            catch { }
        }
        public override void DrawFace(List<Algebra.VertexIndex> indices, List<Algebra.Vertex> vertices, Algebra.Vector3 normal, System.Drawing.Color unColor, int poliId, uint obj3dId, bool picking, enuPolygonMode beginMode = enuPolygonMode.Polygon)
        {
            try
            {
                if (poliId >= 0 && obj3dId >= 0)
                {
                    GL.Color4(unColor); GetError(true);

                    //if (picking) PickingHelper.StartPickingNames(obj3dId, (uint)poliId);
                    this.BeginFixedPipeline3D(beginMode);
                    foreach (VertexIndex indice in indices)
                    {
                        GL.Normal3(normal.X, normal.Y, normal.Z); GetError(true);

                        InsertVertex(vertices[(int)indice.PositionIndex].Position.X,
                                   vertices[(int)indice.PositionIndex].Position.Y,
                                   vertices[(int)indice.PositionIndex].Position.Z);
                    }
                    this.EndFixedPipeline3D();

                    //if (picking) PickingHelper.EndElement();
                }
            }
            catch { }
        }
        public override void DrawLine(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, System.Drawing.Color unColor, bool blend = true, float lineSize = 1.0f)
        {
            GL.Color4(unColor); GetError(true);
            if (blend)
            {
                this.Blend = blend;
                this.BlendFunc(Renderer.BlendingFactorSrc.SrcAlpha, Renderer.BlendingFactorDest.OneMinusSrcAlpha);

            }

            LineWidth = lineSize;

            BeginFixedPipeline3D(enuPolygonMode.Lines);
            InsertVertex(v1x, v1y, v1z);
            InsertVertex(v2x, v2y, v2z);
            if (blend) Blend = false;
            EndFixedPipeline3D();

            //LineWidth = 1.0f;
        }
        public override void DrawLine(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, System.Drawing.Color unColor, int lineId, uint obj3dId, float lineSize = 1.0f)
        {
            if (lineId >= 0 && obj3dId >= 0)
            {
                //if (picking)
                //{
                //    GL.LoadName(obj3dId);
                //    GL.PushName(lineId);
                //}
                LineWidth = lineSize;
                

                GL.Color4(unColor); GetError(true);

                this.BeginFixedPipeline3D(enuPolygonMode.Lines);  // Original
                
                InsertVertex(v1x, v1y, v1z);
                InsertVertex(v2x, v2y, v2z);

                this.EndFixedPipeline3D();

                //LineWidth = 1.0f;

                //if (picking)
                //{
                //    GL.PopName();
                //}
            }
        }
        public override void DrawLine(Algebra.Vertex v1, Algebra.Vertex v2, System.Drawing.Color unColor, int lineId, uint obj3dId, float lineSize = 1.0f)
        {
            DrawLine(v1.Position.X, v1.Position.Y, v1.Position.Z, v2.Position.X, v2.Position.Y, v2.Position.Z, unColor, lineId, obj3dId, lineSize);
        }
        public override void DrawQuad(Algebra.Vector3 orig, float size, bool bSelected, Algebra.Vector3 axisU, Algebra.Vector3 axisV)
        {
            this.DepthTest = false;         //GL.Disable(EnableCap.DepthTest);
            this.CullFace = false;          //GL.Disable(EnableCap.CullFace);
            this.Lighting = false;          //GL.Disable(EnableCap.Lighting);

            this.Blend = true;              //GL.Enable(EnableCap.Blend);
            this.BlendFunc(Renderer.BlendingFactorSrc.SrcAlpha, Renderer.BlendingFactorDest.OneMinusSrcAlpha);


            Algebra.Vector3[] pts = new Algebra.Vector3[4];
            pts[0] = orig;
            pts[1] = orig + (axisU * size);
            pts[2] = orig + (axisU + axisV) * size;
            pts[3] = orig + (axisV * size);

            if (!bSelected)
            { GL.Color4(1f, 1f, 0f, 0.5f); GetError(true); }
            else
            { GL.Color4(1, 1, 1, 0.6f); GetError(true); }

            BeginFixedPipeline3D(enuPolygonMode.Quads);
            //this.InsertVertex(ref pts[0].X);
            //this.InsertVertex(ref pts[1].X);
            //this.InsertVertex(ref pts[2].X);
            //this.InsertVertex(ref pts[3].X);

            this.InsertVertex(pts[0]);
            this.InsertVertex(pts[1]);
            this.InsertVertex(pts[2]);
            this.InsertVertex(pts[3]);
            EndFixedPipeline3D();

            if (!bSelected)
            { GL.Color4(1, 1, 0.2f, 1); GetError(true); }
            else
            { GL.Color4(1, 1, 1, 0.6f); GetError(true); }

            BeginFixedPipeline3D(enuPolygonMode.LineStrip);
            //this.InsertVertex(ref pts[0].X);
            //this.InsertVertex(ref pts[1].X);
            //this.InsertVertex(ref pts[2].X);
            //this.InsertVertex(ref pts[3].X);

            this.InsertVertex(pts[0]);
            this.InsertVertex(pts[1]);
            this.InsertVertex(pts[2]);
            this.InsertVertex(pts[3]);
            EndFixedPipeline3D();

            this.Blend = false;
            this.Lighting = true;
            this.CullFace = true;
            this.DepthTest = true;
        }
        public override void DrawQuadXZ(float size, float sizeOffset, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            size += sizeOffset;
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.Quads);
            InsertVertex(sizeOffset, 0, sizeOffset);
            InsertVertex(size, 0, sizeOffset);
            InsertVertex(size, 0, size);
            InsertVertex(sizeOffset, 0, size);
            this.EndFixedPipeline3D();
        }
        public override void DrawQuadZY(float size, float sizeOffset, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            size += sizeOffset;
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.Quads);
            InsertVertex(0, sizeOffset, sizeOffset);
            InsertVertex(0, size, sizeOffset);
            InsertVertex(0, size, size);
            InsertVertex(0, sizeOffset, size);
            this.EndFixedPipeline3D();
        }
        public override void DrawQuadYX(float size, float sizeOffset, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            size += sizeOffset;
            GL.Color4(unColor); GetError(true);
            this.BeginFixedPipeline3D(enuPolygonMode.Quads);
            InsertVertex(sizeOffset, sizeOffset, 0);
            InsertVertex(sizeOffset, size, 0);
            InsertVertex(size, size, 0);
            InsertVertex(size, sizeOffset, 0);
            this.EndFixedPipeline3D();
        }
        public override void DrawRect(System.Drawing.Rectangle rect, Color drawColor, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
        {
            if (m_VertNum + 4 >= MaxVerts)
            {
                Flush();
            }

            if (m_ClipEnabled)
            {
                // cpu scissors test

                if (rect.Y < ClipRegion.Y)
                {
                    int oldHeight = rect.Height;
                    int delta = ClipRegion.Y - rect.Y;
                    rect.Y = ClipRegion.Y;
                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v1 += dv * (v2 - v1);
                }

                if ((rect.Y + rect.Height) > (ClipRegion.Y + ClipRegion.Height))
                {
                    int oldHeight = rect.Height;
                    int delta = (rect.Y + rect.Height) - (ClipRegion.Y + ClipRegion.Height);

                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v2 -= dv * (v2 - v1);
                }

                if (rect.X < ClipRegion.X)
                {
                    int oldWidth = rect.Width;
                    int delta = ClipRegion.X - rect.X;
                    rect.X = ClipRegion.X;
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u1 += du * (u2 - u1);
                }

                if ((rect.X + rect.Width) > (ClipRegion.X + ClipRegion.Width))
                {
                    int oldWidth = rect.Width;
                    int delta = (rect.X + rect.Width) - (ClipRegion.X + ClipRegion.Width);

                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u2 -= du * (u2 - u1);
                }
            }

            //int vertexIndex = m_VertNum;   // Changed Sep-20-15 when implementing 2d
            int vertexIndex = m_VertNum;

            if (vertexIndex >= MaxVerts)
                return;
            m_Vertices[vertexIndex].x = (short)rect.X;
            m_Vertices[vertexIndex].y = (short)rect.Y;
            m_Vertices[vertexIndex].u = u1;
            m_Vertices[vertexIndex].v = v1;
            m_Vertices[vertexIndex].r = drawColor.R;
            m_Vertices[vertexIndex].g = drawColor.G;
            m_Vertices[vertexIndex].b = drawColor.B;
            m_Vertices[vertexIndex].a = drawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
            m_Vertices[vertexIndex].y = (short)rect.Y;
            m_Vertices[vertexIndex].u = u2;
            m_Vertices[vertexIndex].v = v1;
            m_Vertices[vertexIndex].r = drawColor.R;
            m_Vertices[vertexIndex].g = drawColor.G;
            m_Vertices[vertexIndex].b = drawColor.B;
            m_Vertices[vertexIndex].a = drawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
            m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
            m_Vertices[vertexIndex].u = u2;
            m_Vertices[vertexIndex].v = v2;
            m_Vertices[vertexIndex].r = drawColor.R;
            m_Vertices[vertexIndex].g = drawColor.G;
            m_Vertices[vertexIndex].b = drawColor.B;
            m_Vertices[vertexIndex].a = drawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)rect.X;
            m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
            m_Vertices[vertexIndex].u = u1;
            m_Vertices[vertexIndex].v = v2;
            m_Vertices[vertexIndex].r = drawColor.R;
            m_Vertices[vertexIndex].g = drawColor.G;
            m_Vertices[vertexIndex].b = drawColor.B;
            m_Vertices[vertexIndex].a = drawColor.A;

            m_VertNum += 4;
        }
        public override void DrawFilledRect(System.Drawing.Rectangle rect, Color rectColor)
        {
            // CAUTION:: This Function belongs to 2d UI (gwen) ONLY!!!
            if (m_TextureEnabled)
            {
                Flush();
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true);
                m_TextureEnabled = false;
            }

            rect = Translate(rect);

            DrawRect(rect, rectColor);
        }
        public override void DrawSelectionSquare(float x1, float y1, float x2, float y2, System.Drawing.Color unColor, Rectangle clientRect)
        {
            string tout = "X1: " + x1.ToString() + " Y1: " + y1.ToString() + "\n" +
                          "X2: " + x2.ToString() + " Y2: " + y2.ToString();

            float curLineWidth      = this.LineWidth;
            this.Lighting           = false;
            this.LineWidth          = 1f;

            this.PushAttrib(AttribMask.CurrentBit | AttribMask.ColorBufferBit |
                                AttribMask.DepthBufferBit |
                                AttribMask.EnableBit |
                                AttribMask.HintBit |
                                AttribMask.LightingBit |
                                AttribMask.LineBit |
                                AttribMask.PolygonBit |
                                AttribMask.TransformBit
                                );

            this.DepthTest = false; GetError(true);
            this.Enable(EnableCap.LineSmooth); GetError(true);
            GL.Color4(unColor); GetError(true);

            this.BeginFixedPipelineOrtho(new Rectangle(clientRect.X,clientRect.Y,clientRect.Width, clientRect.Height)); 
            this.BeginFixedPipeline3D(enuPolygonMode.LineStrip);
            GL.Vertex2(x1, y1);  // TODO:: Implement in RendererBase
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x1, y1);
            this.EndFixedPipeline3D();
            this.EndFixedPipelineOrtho();

            this.PopAttrib();
        }
        public override void DrawSphere(float radius, int lats, int longs, System.Drawing.Color unColor)
        {
            // From http://code.google.com/p/3d-editor-toolkit/
            GL.Color4(unColor); GetError(true);

            for (int i = 1; i <= lats; i++)
            {
                float lat0 = MathHelper.Pi * (-0.5f + (float)(i - 1) / (float)lats);
                float z0 = radius * (float)Math.Sin((float)lat0);
                float zr0 = radius * (float)Math.Cos((float)lat0);

                float lat1 = MathHelper.Pi * (-0.5f + (float)i / (float)lats);
                float z1 = radius * (float)Math.Sin((float)lat1);
                float zr1 = radius * (float)Math.Cos((float)lat1);

                this.BeginFixedPipeline3D(enuPolygonMode.QuadStrip);
                for (int j = 0; j <= longs; j++)
                {
                    float lng = 2 * MathHelper.Pi * (float)(j - 1) / (float)longs;
                    float x = (float)Math.Cos((float)lng);
                    float y = (float)Math.Sin((float)lng);
                    GL.Normal3(x * zr1, y * zr1, z1); GetError(true);
                    InsertVertex(x * zr1, y * zr1, z1);
                    GL.Normal3(x * zr0, y * zr0, z0); GetError(true);
                    InsertVertex(x * zr0, y * zr0, z0);
                }
                this.EndFixedPipeline3D();
            }
        }
        public override void DrawTriangle(Algebra.Vector3 orig, float size, bool bSelected, Algebra.Vector3 axisU, Algebra.Vector3 axisV)
        {
            DepthTest = false;         //GL.Disable(EnableCap.DepthTest);
            CullFace = false;          //GL.Disable(EnableCap.CullFace);
            Lighting = false;          //GL.Disable(EnableCap.Lighting);
            Blend = true;              //GL.Enable(EnableCap.Blend);
            BlendFunc(Renderer.BlendingFactorSrc.SrcAlpha, Renderer.BlendingFactorDest.OneMinusSrcAlpha);

            Algebra.Vector3[] pts = new Algebra.Vector3[3];
            pts[0] = orig;

            pts[1] = (axisU);
            pts[2] = (axisV);

            pts[1] *= size;
            pts[2] *= size;
            pts[1] += orig;
            pts[2] += orig;

            if (!bSelected)
            { GL.Color4(1, 1, 0, 0.5f); GetError(true); }
            else
            { GL.Color4(1, 1, 1, 0.6f); GetError(true); }

            BeginFixedPipeline3D(enuPolygonMode.Triangles);
            InsertVertex(pts[0]);
            InsertVertex(pts[1]);
            InsertVertex(pts[2]);
            //InsertVertex(ref pts[3].X); // ERROR
            EndFixedPipeline3D();

            if (!bSelected)
            { GL.Color4(1, 1, 0.2f, 1); GetError(true); }
            else
            { GL.Color4(1, 1, 1, 0.6f); GetError(true); }

                BeginFixedPipeline3D(enuPolygonMode.LineStrip);
            InsertVertex(pts[0]);
            InsertVertex(pts[1]);
            InsertVertex(pts[2]);
            EndFixedPipeline3D();

            Blend = false;          //GL.Disable(EnableCap.Blend);
            DepthTest = true;         //GL.Disable(EnableCap.DepthTest);
            CullFace = true;          //GL.Disable(EnableCap.CullFace);
            Lighting = true;
        }
        public override void DrawVertex(Algebra.Vertex v, System.Drawing.Color color, uint objId, float pointSize = 1f)
        {
            if (v.Index >= 0 && objId >= 0)
            {
                //if (picking) PickingHelper.StartElement(objId, v.Index);

                //GL.PushAttrib(AttribMask.PointBit);
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth); GetError(true);
                GL.PointSize(pointSize); GetError(true); // was 6.0f on July-22-2016
                GL.Color4(color); GetError(true);

                //if (picking) PickingHelper.StartPickingNames(objId, v.Index);
                this.BeginFixedPipeline3D(enuPolygonMode.Points);
                InsertVertex(v.Position.X, v.Position.Y, v.Position.Z);
                this.EndFixedPipeline3D();

                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth); GetError(true);
                GL.PointSize(1.0f); GetError(true);
                //GL.PopAttrib();
                //if (picking) PickingHelper.EndElement();                            //GL.PopName();
            }
        }
        public override void DrawVertex(Algebra.Vertex[] v, System.Drawing.Color color, enuPolygonMode mode = enuPolygonMode.Points, bool smooth = true, float pointSize = 1f)
        {
            GL.Color4(color); GetError(true);
            GL.PointSize(pointSize); GetError(true);

            if (smooth)
            { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth); GetError(true); }

            this.BeginFixedPipeline3D(mode);

            for (int i = 0; i < v.Length; i++)
                InsertVertex(v[i].Position.X,
                           v[i].Position.Y,
                           v[i].Position.Z);
            
            this.EndFixedPipeline3D();

            GL.PointSize(1.0f); GetError(true);

            if (smooth)
            { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth); GetError(true); }


            }
        /// <summary>
        /// Function overrides the Renderer.BeginFixedPipeline3D function. User must call-it
        /// </summary>
        /// <param name="v">Vertices</param>
        /// <param name="colors">Color per Vertex</param>
        /// <param name="mode">LineLoop</param>
        /// <param name="smooth">Smooth enabled</param>
        /// <param name="pointSize"></param>
        public override void DrawVertices(Algebra.Vector3[] v, Color4[] colors, enuPolygonMode mode = enuPolygonMode.Points)
        {
            //GL.Color4(color);
            if(v.Length != colors.Length)
                throw new Exception("Vertex Colors must match Vertex Count", new Exception(string.Format("DrawVertex(Vertex[{0}], Colors[{1}])", v.Length.ToString(), colors.Length.ToString())));


            //if (smooth)
            //{
            //    GL.Enable(EnableCap.PointSmooth);
            //    GL.PointSize(1.0f);
            //}

            //this.BeginFixedPipeline3D(mode);  // Note: Without?

            for (int i = 0; i < v.Length; i++)
            {
                GL.Color4(new OpenTK.Graphics.Color4(colors[i].R, colors[i].G, colors[i].B, colors[i].A)); GetError(true);

                InsertVertex(v[i].X,
                           v[i].Y,
                           v[i].Z);
            }

            //this.EndFixedPipeline3D();

            //if (smooth)
            //{
            //    GL.Disable(EnableCap.PointSmooth);
            //    GL.PointSize(1.0f);
            //}
        }

        public override void PointSize(float pointSize = 1.0f)
        {
            GL.PointSize(pointSize); GetError(true);
        }
        //public override void LineSize(float lineSize = .0f)
        //{
        //    GL.LineWidth(lineSize);
        //}

        public override void GetFloat(Renderer.GetPName pname, float[] data)
        {
            GL.GetFloat(pname.ToOpenGL(), data); GetError(true);
        }
        public override void GetFloat(Renderer.GetPName pname, out Matrix4 m)
        {
            OpenTK.Matrix4 o = new OpenTK.Matrix4();
            GL.GetFloat(pname.ToOpenGL(), out o); GetError(true);

            m = o.ToSyscon();
        }
        public override int[] GetInteger(Renderer.GetPName pname, int[] data)
        {
            // warning, supposedly data is returned to the calling object

            GL.GetInteger(pname.ToOpenGL(), data); GetError(true);

            return data;
        }

        //public override void InsertVertex(ref float f)
        //{
        //    //if (!_isRendering)
        //    //    throw new Exception("OpenGL renderer vertex buffer locked");

        //    InsertVertex(ref f);
        //}
        public override void InsertVertex(Algebra.Vector3 vertex)
        {
            InsertVertex(vertex.X, vertex.Y, vertex.Z);
        }
        public override void InsertVertex(float x, float y, float z)
        {

            GL.Vertex3(x, y, z); GetError(true);  // TODO:: This should write to a buffer instead if I want to use VBOs




            return;

            //switch (GraphicsMode)
            //{
            //        case enuGraphicsMode.VAO:
            //        //m_vertexBuffer.Add();
            //        break;
            //}

            // TODO:: Add VAO Support
            /*
             switch(enuOpenGLVersion)
             {
                 case(enuOpenGLVersion.GL40)
                    bufferVertex3(x,y,z);
                    break;
                 case(enuOpenGLVersion.GL10)
                    GL.Vertex3(x,y,z);
                    break;
             }
             
             */
            

        }
        public override void InsertNormal(float x, float y, float z)
        {
            GL.Normal3(x,y,z); GetError(true);
        }

        public override Algebra.Vector4 Project(Algebra.Vector4 objPos, Algebra.Matrix4 projection, Algebra.Matrix4 view, System.Drawing.Size viewport)
        {
            Algebra.Vector4 vec = objPos;
            vec = Algebra.Vector4.Transform(vec, Algebra.Matrix4.Mult(projection, view));
            vec.X = (vec.X + 1) * ((float)viewport.Width / 2);
            vec.Y = (vec.Y + 1) * ((float)viewport.Height / 2);
            return vec;
        }
        public override Algebra.Vector4 UnProject(Algebra.Matrix4 projection, Algebra.Matrix4 view, System.Drawing.Size viewport, Algebra.Vector2 mouse)
        {
            Algebra.Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = 0;
            vec.W = 1.0f;

            Algebra.Matrix4 viewInv = Algebra.Matrix4.Invert(view);
            Algebra.Matrix4 projInv = Algebra.Matrix4.Invert(projection);

            Algebra.Vector4.Transform(ref vec, ref projInv, out vec);
            Algebra.Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec;
        }
        public override Algebra.Vector4 UnProject(Algebra.Matrix4d projection, Algebra.Matrix4d view, System.Drawing.Size viewport, Algebra.Vector2 mouse)
        {
            Algebra.Vector4d vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = 0;
            vec.W = 1.0f;

            Algebra.Matrix4d viewInv = Algebra.Matrix4d.Invert(view);
            Algebra.Matrix4d projInv = Algebra.Matrix4d.Invert(projection);

            Algebra.Vector4.Transform(ref vec, ref projInv, out vec);
            Algebra.Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return new Vector4((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);
            
        }
        //public override int             UnProject(Vector3d win, ref Vector3d obj)
        //{
            


        //    //return UnProject(new Vector3d(win.X, win.Y, win.Z), 
        //    //                    modelMatrix, 
        //    //                    projMatrix, 
        //    //                    viewport, 
        //    //                    ref obj);

        //    Matrix4d finalMatrix;
        //    Vector4d _in;
        //    Vector4d _out;

        //    finalMatrix = Matrix4d.Mult(modelMatrix.ToSyscon(), projMatrix.ToSyscon());

        //    //if (!__gluInvertMatrixd(finalMatrix, finalMatrix)) return(GL_FALSE);
        //    try
        //    {
        //        finalMatrix.Invert();
        //    }
        //    catch (Exception fmat)
        //    {
        //        // Matrices empty when lost focus // Matrix is singular and cannot be inverted
        //    }


        //    _in.X = win.X;
        //    _in.Y = win.Y;
        //    _in.Z = win.Z;
        //    _in.W = 1.0;

        //    /* Map x and y from window coordinates */
        //    _in.X = (_in.X - viewport[0]) / viewport[2];
        //    _in.Y = (_in.Y - viewport[1]) / viewport[3];

        //    /* Map to range -1 to 1 */
        //    _in.X = _in.X * 2 - 1;
        //    _in.Y = _in.Y * 2 - 1;
        //    _in.Z = _in.Z * 2 - 1;

        //    //__gluMultMatrixVecd(finalMatrix, _in, _out);
        //    // check if this works:
        //    _out = Vector4d.Transform(_in, finalMatrix);

        //    if (_out.W == 0.0)
        //        return (0);

        //    _out.X /= _out.W;
        //    _out.Y /= _out.W;
        //    _out.Z /= _out.W;
            

        //    obj = new Vector3d(_out.X, _out.Y, _out.Z);

        //    return (1);
        //}

        public override void LoadIdentity()
        {
            GL.LoadIdentity(); GetError(true);
        }
        public override void LoadMatrix(ref Algebra.Matrix4 matrix)
        {
            OpenTK.Matrix4 m = matrix.ToOpenGL();
            GL.LoadMatrix(ref m); GetError(true);
            matrix = m.ToSyscon();
        }
        public override void MatrixMode(Renderer.MatrixMode mode)
        {
            OpenTK.Graphics.OpenGL.MatrixMode glMode = mode.ToOpenGL();
            GL.MatrixMode(glMode); GetError(true);
        }

        public override void MultMatrix(float[] matrix)
        {
            GL.MultMatrix(matrix); GetError(true);
        }
        public override void MultMatrix(ref Algebra.Matrix4 matrix)
        {
            OpenTK.Matrix4 m = matrix.ToOpenGL();
            GL.MultMatrix(ref m); GetError(true);
            matrix = m.ToSyscon();
        }
        public override void MultMatrix(ref Algebra.Matrix4d matrix)
        {
            OpenTK.Matrix4d m = matrix.ToOpenGL();
            GL.MultMatrix(ref m); GetError(true);
            matrix = m.ToSyscon();
        }

        public override void PushAttrib(AttribMask mask)
        {
            GL.PushAttrib(mask.ToOpenGL()); GetError(true); // TODO:: Note CHECK for Attrib loss due to cast here
        }
        public override void PopAttrib()
        {
            GL.PopAttrib(); GetError(true);
        }
        public override void PushMatrix()
        {
            GL.PushMatrix(); GetError(true);
        }
        public override void PopMatrix()
        {
            GL.PopMatrix(); GetError(true);
        }

        public override void PushFaceColor(Algebra.Geometry.Face face)
        {
            //if (face.ColorMaterial != null)
            //    face.ColorMaterial.Push();
        }
        public override void PopFaceColor(Algebra.Geometry.Face face)
        {
            //face.ColorMaterial.Pop();
        }
        public override void PolygonOffset(float factor, float units)
        {
            GL.PolygonOffset(factor, units); GetError(true);
        }
        public override bool PolygonOffsetFill
        {
            get
            {
                bool poliOffset;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.PolygonOffsetFill, out poliOffset);
                return poliOffset;
            }
            set
            {
                if (value)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.PolygonOffsetFill); GetError(true); }
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PolygonOffsetFill); GetError(true); }
                }
        }
        public override void PolygonMode(Renderer.MaterialFace face, Renderer.PolygonMode mode)
        {
            GL.PolygonMode(face.ToOpenGL(), mode.ToOpenGL()); GetError(true);
        }
        public override void SetShadingMode(Renderer.ShadingModel shading)
        {
            GL.ShadeModel(shading.ToOpenGL()); GetError(true);
        }

        public override void GluOrtho2D(double left, double right, double bottom, double top)
        {
            Ortho(left, right, bottom, top, -1.0, 1);
        }
        public override void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            GL.Ortho(left, right, bottom, top, zNear, zFar); GetError(true);
        }

        public override bool Texture2D
        {
            get
            {
                bool dt;
                GL.GetBoolean(OpenTK.Graphics.OpenGL.GetPName.Texture2D, out dt);
                return dt;
            }
            set
            {
                if (value)
                { GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true); }
                else
                { GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true); }
            }
        }

        public override void Translate(double x, double y, double z)
        {
            GL.Translate(x,y,z); GetError(true);
        }
        public override void Rotate(float degrees, Algebra.Vector3 axis)
        {
            GL.Rotate(degrees, axis.ToOpenGL()); GetError(true);
        }
        //public override void Scale(float x, float y, float z)
        //{
            
        //}

        public override void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x,y,width,height); GetError(true);
        }

        public override Vector2 WorldToScreen0(Vector3 world)
        {
            // Added 02-July-2015

            // ref: http://www.songho.ca/opengl/gl_transform.html
            OpenTK.Vector4 obj = new OpenTK.Vector4(world.X, world.Y, world.Z, 1.0f);

            OpenTK.Matrix4 projection = new OpenTK.Matrix4();
            OpenTK.Matrix4 modelView = new OpenTK.Matrix4();
            OpenTK.Vector4 viewPort = new OpenTK.Vector4();

            GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.ModelviewMatrix, out modelView); GetError(true);
            GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.ProjectionMatrix, out projection); GetError(true);
            GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.Viewport, out viewPort); GetError(true);

            //modelView.Invert();     // Added during debug
            //projection.Invert();    // Added during debug

            modelView.Transpose();
            projection.Transpose();

            OpenTK.Vector4
                eye = OpenTK.Vector4.Transform(obj, modelView),
                clip = OpenTK.Vector4.Transform(eye, projection);

            OpenTK.Vector3
                ndc = new OpenTK.Vector3(clip.X / clip.W, 
                                         clip.Y / clip.W, 
                                         clip.Z / clip.W);

            OpenTK.Vector2
                w = new OpenTK.Vector2( viewPort.Z / 2 * ndc.X + viewPort.X + viewPort.Z / 2,
                                        viewPort.W / 2 * ndc.Y + viewPort.Y + viewPort.W / 2);

            return w.ToSyscon();
        }
        public override Point WorldToScreen(Vector3 p)
        {
            Matrix4 model, proj;
            int[] view = new int[4];

            this.GetFloat(NthDimension.Graphics.Renderer.GetPName.ModelviewMatrix, out model);
            this.GetFloat(NthDimension.Graphics.Renderer.GetPName.ProjectionMatrix, out proj);
            this.GetInteger(NthDimension.Graphics.Renderer.GetPName.Viewport, view);

            //model.Transpose();
            //proj.Transpose();

            double wx = 0, wy = 0, wz = 0;

            int d = gluProject
                            (
                              p.X,
                              p.Y,
                              p.Z,
                              model,
                              proj,
                              view,
                              ref wx,
                              ref wy,
                              ref wz
                            );

            return new Point((int)wx, (int)wy);
        }
        public override Point WorldToScreen(Vector3 p, Matrix4 modelview)
        {
            Matrix4 model, proj;
            int[] view = new int[4];

            //this.GetFloat(SYSCON.Graphics.Renderer.GetPName.ModelviewMatrix, out model);
            this.GetFloat(NthDimension.Graphics.Renderer.GetPName.ProjectionMatrix, out proj);
            this.GetInteger(NthDimension.Graphics.Renderer.GetPName.Viewport, view);

            //model.Transpose();
            //proj.Transpose();

            double wx = 0, wy = 0, wz = 0;

            int d = gluProject
                            (
                              p.X,
                              p.Y,
                              p.Z,
                              modelview,
                              proj,
                              view,
                              ref wx,
                              ref wy,
                              ref wz
                            );

            return new Point((int)wx, (int)wy);
        }
        int gluProject
                      (
                       float objx,
                       float objy,
                       float objz,
                       Matrix4 modelMatrix,
                       Matrix4 projMatrix,
                       int[] viewport,
                       ref double winx,
                       ref double winy,
                       ref double winz
                      )
        {
            Vector4 _in;
            Vector4 _out;

            _in.X = objx;
            _in.Y = objy;
            _in.Z = objz;
            _in.W = 1.0f;
            //__gluMultMatrixVecd(modelMatrix, in, out); // Commented out by code author
            //__gluMultMatrixVecd(projMatrix, out, in);  // Commented out by code author
            //TODO: check if multiplication is in right order

            _out = Vector4.Transform(_in, modelMatrix);
            _in = Vector4.Transform(_out, projMatrix);

            if (_in.W == 0.0)
                return (0);

            _in.X /= _in.W;
            _in.Y /= _in.W;
            _in.Z /= _in.W;

            /* Map x, y and z to range 0-1 */
            _in.X = _in.X * 0.5f + 0.5f;
            _in.Y = _in.Y * 0.5f + 0.5f;
            _in.Z = _in.Z * 0.5f + 0.5f;

            /* Map x,y to viewport */
            _in.X = _in.X * viewport[2] + viewport[0];
            _in.Y = _in.Y * viewport[3] + viewport[1];

            winx = _in.X;
            winy = _in.Y;
            winz = _in.Z;
            return (1);
        }
        #endregion

        #region [ OpenGL 3.x ] IVbo Members
        // RendererGL.Buffers - TBRemoved
        // Multiple layers of Vertex Buffer Objects

        // WARNING:: Do NOT MODIFY! functions are OK

        //[Obsolete("Use RendererGL.Buffers instead")]
        //public Vbo LoadVBO<TVertex>(TVertex[] vertices, uint[] elements) where TVertex : struct
        //{

        //    return new Vbo();
        //}

        #endregion

        public override void ActiveTexture(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit.ToOpenGL()); GetError(true);
        }
        public override void ClientActiveTexture(TextureUnit textureUnit)
        {
            GL.ClientActiveTexture(textureUnit.ToOpenGL()); GetError(true);
        }
        public override void BindTexture(TextureTarget textureTarget, int textureId)
        {
            GL.BindTexture(textureTarget.ToOpenGL(), textureId); GetError(true);
        }
        public override void GenTextures(int n, out int textures)
        {
            GL.GenTextures(n, out textures); GetError(true);
        }
        public override void DeleteTexture(int textureId)
        {
            GL.DeleteTexture(textureId); GetError(true);
        }
        public override void TexEnv(TextureEnvTarget target, TextureEnvParameter pname, int param)
        {
            GL.TexEnv(target.ToOpenGL(), pname.ToOpenGL(), param); GetError(true);
        }

        public override void EnableClientState(ArrayCap cap)
        {
            GL.EnableClientState(cap.ToOpenGL()); GetError(true);
        }
        public override void DisableClientState(ArrayCap cap)
        {
            GL.DisableClientState(cap.ToOpenGL()); GetError(true);
        }

        public override void ColorPointer(int size, ColorPointerType type, int stride, int offset)
        {
            GL.ColorPointer(size, type.ToOpenGL(), stride, offset); GetError(true);
        }
        public override void VertexPointer(int size, VertexPointerType type, int stride, int offset)
        {
            GL.VertexPointer(size, type.ToOpenGL(), stride, offset); GetError(true);
        }
        public override void NormalPointer(NormalPointerType type, int stride, int offset)
        {
            GL.NormalPointer(type.ToOpenGL(), stride, offset); GetError(true);
        }
        public override void TexCoordPointer(int size, TexCoordPointerType type, int stride, int offset)
        {
            GL.TexCoordPointer(size, type.ToOpenGL(), stride, offset); GetError(true);
        }

        public override int loadTextureFromFile(string filename)
        {
            try
            {
                GLTexture2D tex = new GLTexture2D(loadBMP(filename)); GetError(true);

                return tex.AssetId;
            }
            catch (ArgumentException)
            {
                //Debug.Trace("Class Texture: Error creating Bitmap: " + url);
                return -1;
            }
        }

        public override void loadTextureFromBitmapData(int glTextureId, Bitmap TextureBitmap, string name = null, bool hasAlpha = false, bool mipmap = true)
        {
            //get the data out of the bitmap
            System.Drawing.Imaging.BitmapData TextureData;

            if (hasAlpha)
            {
                TextureData = TextureBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

            }
            else
            {
                TextureData = TextureBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                    );
            }
            
            //Code to get the data to the OpenGL Driver
            
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true);
            GL.ActiveTexture(TextureUnit.Texture0.ToOpenGL()); GetError(true);

            //tell OpenGL that this is a 2D texture
            GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), 
                           glTextureId); GetError(true);

            //the following code sets certian parameters for the texture
            // GL.TexEnv (TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Combine);
            // GL.TexEnv (TextureEnvTarget.TextureEnv, TextureEnvParameter.CombineRgb, (float)TextureEnvMode.Modulate);

            GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(), 
                            TextureParameterName.TextureMagFilter.ToOpenGL(),
                            (float)TextureMagFilter.Linear); GetError(true);

            // this assumes mipmaps are present...
            GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(), 
                            TextureParameterName.TextureMinFilter.ToOpenGL(),
                            (float)TextureMinFilter.LinearMipmapLinear); GetError(true);


            // tell OpenGL to build mipmaps out of the bitmap data
            // .. what a mess ... http://www.g-truc.net/post-0256.html
            // this is the old way, must be called before texture is loaded, see below for new way...
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (float)1.0f);

            // tell openGL the next line begins on a word boundary...
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4); GetError(true);

            // load the texture
            if (hasAlpha)
            {
                GL.TexImage2D(
                    TextureTarget.Texture2D.ToOpenGL(),
                    0, // level
                    OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba,
                    TextureBitmap.Width, TextureBitmap.Height,
                    0, // border
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, // why is this Bgr when the lockbits is rgb!?
                    OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                    TextureData.Scan0
                    ); GetError(true);
                //Console.WriteLine("SSTexture: loaded alpha ({0},{1}) from: {2}", TextureBitmap.Width, TextureBitmap.Height, name);
            }
            else
            {
                GL.TexImage2D(
                    TextureTarget.Texture2D.ToOpenGL(),
                    0, // level
                    OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb,
                    TextureBitmap.Width, TextureBitmap.Height,
                    0, // border
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgr, // why is this Bgr when the lockbits is rgb!?
                    OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                    TextureData.Scan0
                    ); GetError(true);
                //Console.WriteLine("SSTexture: loaded ({0},{1}) from: {2}", TextureBitmap.Width, TextureBitmap.Height, name);
            }

            // this is the new way to generate mipmaps
            if (mipmap)
            {
                GL.GenerateMipmap(OpenTK.Graphics.OpenGL.GenerateMipmapTarget.Texture2D); GetError(true);
            }

            //free the bitmap data (we dont need it anymore because it has been passed to the OpenGL driver
            TextureBitmap.UnlockBits(TextureData);
        }

        private static Bitmap loadBMP(string fileName)
        {
            if (fileName == null || fileName == string.Empty)
            {                  // Make Sure A Filename Was Given
                Console.WriteLine("Class Texture: No filename was provided");
                return null;                                                    // If Not Return Null
            }

            string fileName1 = string.Format("Data{0}{1}",                      // Look For Data\Filename
                System.IO.Path.DirectorySeparatorChar, fileName);
            string fileName2 = string.Format("{0}{1}{0}{1}Data{1}{2}",          // Look For ..\..\Data\Filename
                "..", System.IO.Path.DirectorySeparatorChar, fileName);

            // Make Sure The File Exists In One Of The Usual Directories
            if (!System.IO.File.Exists(fileName) && !System.IO.File.Exists(fileName1) && !System.IO.File.Exists(fileName2))
            {
                Console.WriteLine("Class Texture: Bitmap: " + fileName + " does not exist");
                return null;                                                    // If Not Return Null
            }

            if (System.IO.File.Exists(fileName))
            {                                         // Does The File Exist Here?
                return new Bitmap(fileName);                                    // Load The Bitmap
            }
            else if (System.IO.File.Exists(fileName1))
            {                                   // Does The File Exist Here?
                return new Bitmap(fileName1);                                   // Load The Bitmap
            }
            else if (System.IO.File.Exists(fileName2))
            {                                   // Does The File Exist Here?
                return new Bitmap(fileName2);                                   // Load The Bitmap
            }
            Console.WriteLine("Error loading Texture Bitmap: " + fileName);
            return null;                                                        // If Load Failed Return Null
        }

        private static void BindTexture(int index)
        {
            if (index == -1)
                return;

            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Texture); 
            GL.LoadIdentity(); 

            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, index); 
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview); 
        }

        #region -- Frame Buffers
        public override bool FBO_Supported()
        {
            string version_string = GL.GetString(StringName.Version.ToOpenGL()); GetError(true);
            // TODO improve in time for OpenGL 10.0 backends
            int major = version_string[0] - '0';
            int minor = version_string[2] - '0';
            Version version = new Version(major, minor); // todo: improve
            if (version < new Version(3, 0))
            {
                var str = GL.GetString(StringName.Extensions.ToOpenGL()).ToLower(); GetError(true);
                if (!str.Contains("framebuffer_object"))
                {
                    Console.WriteLine("framebuffers not supported by the GL version ");
                    return false;
                }
            }
            return true;
        }

        public override void FBO_Create(TextureUnit texUnit, int textureWidth, int textureHeight, out int textureId, out int frameBufferId, out TextureUnit textureUnit)
        {
            textureId = -1;
            frameBufferId = -1;
            textureUnit = TextureUnit.Texture0;

            RaiseApplicationError("FBO_Create is Not Implemented", ErrorSeverity.WARNING);

            //GL.Ext.GenFramebuffers(1, out frameBufferId);
            //if (frameBufferId < 0)
            //{
            //    throw new Exception("gen fb failed");
            //}
            //textureId = GL.GenTexture();
            //textureWidth = textureWidth;
            //textureHeight = textureHeight;

            //// bind the texture and set it up...
            //textureUnit = texUnit;

            ////BindShadowMapToTexture();
            //GL.ActiveTexture(textureUnit.ToOpenGL());
            //GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), textureId);

            //GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(),
            //    TextureParameterName.TextureMagFilter.ToOpenGL(),
            //    (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(),
            //    TextureParameterName.TextureMinFilter.ToOpenGL(),
            //    (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(),
            //    TextureParameterName.TextureWrapS.ToOpenGL(),
            //    (int)TextureWrapMode.ClampToEdge);
            //GL.TexParameter(TextureTarget.Texture2D.ToOpenGL(),
            //    TextureParameterName.TextureWrapT.ToOpenGL(),
            //    (int)TextureWrapMode.ClampToEdge);

            //GL.TexImage2D(TextureTarget.Texture2D.ToOpenGL(), 0,
            //    OpenTK.Graphics.OpenGL.PixelInternalFormat.DepthComponent32f,
            //    textureWidth, textureHeight, 0,
            //    OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, OpenTK.Graphics.OpenGL.PixelType.Float, IntPtr.Zero);
            //// done creating texture, unbind
            //GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), 0);


            //// ----------------------------
            //// now bind the texture to framebuffer..
            //GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer.ToOpenGL(), frameBufferId);
            //GL.Ext.BindFramebuffer(FramebufferTarget.ReadFramebuffer.ToOpenGL(), frameBufferId);

            //GL.Ext.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer.ToOpenGL(), 
            //                            FramebufferAttachment.DepthAttachment,
            //                            TextureTarget.Texture2D.ToOpenGL(), 
            //                            textureId, 
            //                            0);
            ////GL.Ext.FramebufferTexture (FramebufferTarget.FramebufferExt, FramebufferAttachment.Color,
            ////(int)All.None, 0);

            //GL.Viewport(0, 0, textureWidth, textureHeight);
            //GL.DrawBuffer(DrawBufferMode.None);
            //GL.ReadBuffer(ReadBufferMode.None);

            //if (!FBO_AssertOK(FramebufferTarget.DrawFramebuffer, textureId, frameBufferId))
            //{
            //    throw new Exception("failed to create-and-bind shadowmap FBO");
            //}

            //// leave in a sane state...
            //FBO_Unbind();
            //GL.ActiveTexture(TextureUnit.Texture0.ToOpenGL());
            //if (!FBO_AssertOK(FramebufferTarget.DrawFramebuffer, textureId, frameBufferId))
            //{
            //    throw new Exception("failed to ubind shadowmap FBO");
            //}
        }

        public override void FBO_PrepareForRead(TextureUnit textureUnit, int textureId)
        {
            GL.ActiveTexture(textureUnit.ToOpenGL()); GetError(true);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true);
            GL.BindTexture(TextureTarget.Texture2D.ToOpenGL(), textureId); GetError(true);
        }

        public override void FBO_FinishRead(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit.ToOpenGL()); GetError(true);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D); GetError(true);
        }

        public override void FBO_PrepareForRender(int framebufferId, int textureId, int textureWidth, int textureHeight)
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer.ToOpenGL(), framebufferId); GetError(true);
            // GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,FramebufferAttachment.DepthAttachment,
            //	TextureTarget.Texture2D, m_textureID, 0);

            GL.Viewport(0, 0, textureWidth, textureHeight); GetError(true);

            // turn off reading and writing to color data
            GL.DrawBuffer(DrawBufferMode.None); GetError(true);

            if (!FBO_AssertOK(FramebufferTarget.DrawFramebuffer, textureId, framebufferId))
                throw new Exception("failed to bind framebuffer for drawing");

            GL.Clear(ClearBufferMask.DepthBufferBit); GetError(true);
        }

        public override bool FBO_AssertOK(FramebufferTarget target, int textureId, int framebufferId)
        {
            bool m_isValid = false;
            var currCode = GL.Ext.CheckFramebufferStatus(target.ToOpenGL()); GetError(true);
            if (currCode != OpenTK.Graphics.OpenGL.FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Frame buffer operation failed: " + currCode.ToString());
                m_isValid = false;
                FBO_DeleteData(textureId, framebufferId);
            }
            else
            {
                m_isValid = true;
            }
            return m_isValid;
        }

        public override void FBO_DeleteData(int textureId, int framebufferId)
        {
            // TODO: who/when calling this?
            GL.DeleteTexture(textureId); GetError(true);
            GL.Ext.DeleteFramebuffers(1, new int[] { framebufferId } ); GetError(true);
        }

        public override void FBO_Unbind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer.ToOpenGL(), 0); GetError(true);
            GL.Ext.BindFramebuffer(FramebufferTarget.ReadFramebuffer.ToOpenGL(), 0); GetError(true);

        }
        #endregion

        #region SSM - Lighting
        // TODO:: Move to Partial Class File RendererGL.Lighting.cs

        public override void Light(LightName name, LightParameter pname, Vector4 lparam)
        {
            GL.Light(name.ToOpenGL(), pname.ToOpenGL(), lparam.ToOpenGL()); GetError(true);
        }
        public override void Light_Disable(int idx)
        {
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Light0 + idx); GetError(true);
        }
        public override void Light_Setup(ref Matrix4 modelView, LightName lightName, LightName firstName, Vector4 Ambient, Vector4 Diffuse, Vector4 Specular)
        {
            OpenTK.Matrix4 mv = modelView.ToOpenGL();
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview); GetError(true);
            GL.LoadMatrix(ref mv); GetError(true);
            modelView = mv.ToSyscon();

            GL.Light(lightName.ToOpenGL(), OpenTK.Graphics.OpenGL.LightParameter.Ambient, Ambient.ToOpenGL()); GetError(true);// ambient light color (R,G,B,A)

            GL.Light(lightName.ToOpenGL(), OpenTK.Graphics.OpenGL.LightParameter.Diffuse, Diffuse.ToOpenGL()); GetError(true);// diffuse color (R,G,B,A)

            GL.Light(lightName.ToOpenGL(), OpenTK.Graphics.OpenGL.LightParameter.Specular, Specular.ToOpenGL()); GetError(true);// specular light color (R,G,B,A)

            int idx = lightName - firstName;
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Light0 + idx); GetError(true);
        }

        #endregion

        #region SSM - Shadow Maps
        
        #endregion

        #region -- gwen 2d ui

        // Moved to RendererGL.GwenNet.cs File (partial class)

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct Vertex2
        //{
        //    public short x, y;
        //    public float u, v;
        //    public byte r, g, b, a;
        //}

        //private Color m_clearColor = Color.Black;

        //private const int MaxVerts = 1024;  // TODO:: Increase
        //private int m_VertNum;
        ////private Color this.DrawColor;
        //private readonly Vertex2[] m_Vertices;
        //private readonly int m_VertexSize;

        ////private readonly Dictionary<Tuple<String, UserInterface.Font>, TextRenderer> m_StringCache;
        //private readonly System.Drawing.Graphics m_gdi;

        //private bool m_RestoreRenderState;
        //private bool m_ClipEnabled;
        //private bool m_TextureEnabled;
        //private StringFormat m_StringFormat;


        //private static int m_LastTextureID;
        //private bool m_WasBlendEnabled, m_WasTexture2DEnabled, m_WasDepthTestEnabled;
        //private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
        //private float m_PrevAlphaRef;
        //private int m_DrawCallCount;

        //private Point m_RenderOffset;
        //private Rectangle m_ClipRegion;
        ////private static IBuffer<Vector3> m_pointsBuffer; 

        //private int TranslateX(int x)
        //{
        //    int x1 = x + m_RenderOffset.X;
        //    return Util.Ceil(x1 * Scale);
        //}
        //private int TranslateY(int y)
        //{
        //    int y1 = y + m_RenderOffset.Y;
        //    return Util.Ceil(y1 * Scale);
        //}
        ///// <summary>
        ///// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        //public void Translate(ref int x, ref int y)
        //{
        //    x += m_RenderOffset.X;
        //    y += m_RenderOffset.Y;

        //    x = Util.Ceil(x * Scale);
        //    y = Util.Ceil(y * Scale);
        //}
        ///// <summary>
        ///// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        ///// </summary>
        //public Point Translate(Point p)
        //{
        //    int x = p.X;
        //    int y = p.Y;
        //    Translate(ref x, ref y);
        //    return new Point(x, y);
        //}
        ///// <summary>
        ///// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        ///// </summary>
        //private Rectangle Translate(Rectangle rect)
        //{
        //    return new Rectangle(TranslateX(rect.X), TranslateY(rect.Y), Util.Ceil(rect.Width * Scale), Util.Ceil(rect.Height * Scale));
        //}

        //// used for 2D Graphics
        //public virtual float Scale { get; set; }
        ///// <summary>
        ///// Cache to texture provider.
        ///// </summary>
        ////public virtual ICacheToTexture CacheToTexture { get { return null; } }
        /////// <summary>
        /////// Gets or sets the current drawing color.
        /////// </summary>
        ////public virtual Color DrawColor { get; set; }
        ///// <summary>
        ///// Rendering offset. No need to touch it usually.
        ///// </summary>
        //public Point RenderOffset { get { return m_RenderOffset; } set { m_RenderOffset = value; } }
        ///// <summary>
        ///// Clipping rectangle.
        ///// </summary>
        //public Rectangle ClipRegion { get { return m_ClipRegion; } set { m_ClipRegion = value; } }
        ///// <summary>
        ///// Indicates whether the clip region is visible.
        ///// </summary>
        //public bool ClipRegionVisible
        //{
        //    get
        //    {
        //        if (m_ClipRegion.Width <= 0 || m_ClipRegion.Height <= 0)
        //            return false;

        //        return true;
        //    }
        //}
        #endregion

        // ---------------------- EXPERIMENTAL / WORKING --------------------------------------
        // ------------------------------------------------------------------------------------



        // ------------------------------------------------------------------------------------

            
    }

    
}
