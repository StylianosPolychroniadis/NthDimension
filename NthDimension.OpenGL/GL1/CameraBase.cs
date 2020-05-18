using System;
using System.Linq;

using NthDimension.Algebra;

namespace NthDimension.Rasterizer.GL1
{
    public delegate void OnMouseDownHandler(MouseEvent e);
    public delegate void OnMouseMoveHandler(MouseEvent e);
    public delegate void OnMouseUpHandler(MouseEvent e);
    public delegate void OnMouseWheel(MouseEvent e);
    public delegate void ProjectionModeChanged(enuProjectionMode projMode);

    #region Enumerators
    //////[DataContract(Namespace = "SYSCON.Graphics.Cameras")]
    public enum enuProjectionMode
    {
        //////[EnumMember(Value = "Orthgraphic3D")]
        Orthographic3D,
        //////[EnumMember(Value = "Persective3D")]
        Perspective3D
    }

    //////[DataContract(Name = "enuViewOrthographic", Namespace = "SYSCON.Graphics.Cameras"), Serializable]
    public enum enuViewOrthographic
    {
        //////[EnumMember(Value = "Front")]
        Front,
        //////[EnumMember(Value = "Back")]
        Back,
        // ////[EnumMember(Value = "Left")]
        Left,
        //////[EnumMember(Value = "Right")]
        Right,
        //////[EnumMember(Value = "Top")]
        Top,
        //////[EnumMember(Value = "Bottom")]
        Bottom
    };

    ////[DataContract(Name = "enuViewPerspective", Namespace = "SYSCON.Graphics.Cameras")]
    public enum enuViewPerspective
    {
        ////[EnumMember(Value = "View3D")]
        View3D,
        ////[EnumMember(Value = "Flight3D")]
        Flight3D,
        ////[EnumMember(Value = "ResetView3D")]
        ResetView3D
    };
    #endregion
    [Serializable] // Required by CodeDOM
    ////[DataContract(Name = "CameraBase", IsReference = true, Namespace = "SYSCON.Graphics.Cameras")]
   //[KnownType(typeof(IInteractiveCamera))]
    public abstract class CameraBase //: // SceneElement        // Part of the engine scenegraph, now obsolete 2020, 
                                         // IDrawable           // Used for two reasons 1) An interface to change/instanciate the Renderer 2) Should draw the Frustum later on
    {
        protected RendererBase _renderer;

        internal Matrix4 m_projectionMatrix = Matrix4.Identity;
        internal Matrix4 m_viewMatrix = Matrix4.Identity;

        //internal Rectangle                      m_clientRectangle;
        internal int m_effectiveWidth = 1;
        internal int m_effectiveHeight = 1;
        internal int m_aspectWidth;
        internal int m_aspectHeight;
        internal int[] m_viewport = new int[4];
        internal double m_aspectRatio = 1.0d;
        internal double m_clipNear = 0.1d;
        internal double m_clipFar = 3000d;
        internal double m_fovX = 65d;              // horizontal field of view in degrees
        internal double m_fovY = 65d;              // vertical field of view in degrees
        internal double m_fieldOfView = 1f;
        internal double m_orthoZoom = 10;
        internal double m_orthoWidth;
        internal double m_orthoHeight;


        internal Vector3 m_upVector = new Vector3();    // Vector
        internal Vector3 m_rightVector = new Vector3();    // Vector
        internal Vector3 m_forwardVector = new Vector3();    // Vector

        internal Vector3 m_position = new Vector3();    // Point
        internal Vector3 m_targetPosition = new Vector3();    // Point

        internal Quaternion m_orientation = new Quaternion();    // Point
        internal Quaternion m_targetOrientation = new Quaternion();    // Point
        //internal ViewPoint                      m_viewPoint;

        internal enuProjectionMode m_projectionMode = enuProjectionMode.Orthographic3D;
        internal enuViewOrthographic m_viewTypeOrtho3d = enuViewOrthographic.Top;

        internal int m_mousePosX = 0;
        internal int m_mousePosY = 0;
        internal int m_oldMousePosX = 0;
        internal int m_oldMousePosY = 0;


        #region Events - Bindable
        public event OnMouseDownHandler OnMouseDown;
        public event OnMouseMoveHandler OnMouseMove;
        public event OnMouseUpHandler OnMouseUp;
        public event OnMouseWheel OnMouseWheel;
        public event ProjectionModeChanged OnProjectionModeChanged;
        #endregion

        public CameraBase(RendererBase renderer)
        {
            //Name = "Camera";
            _renderer = renderer;
        }

        public virtual void AdjustBestView(Vector3 boundTL, Vector3 boundBR)
        {
            float spanX = boundBR.X - boundTL.X;
            float spanY = boundBR.Y - boundTL.Y;
            float spanZ = boundBR.Z - boundTL.Z;

            float distX = 0.5f * spanX / (float)Math.Tan(Math.PI / 180.0 * m_fovX / 2);
            float distY = 0.5f * spanY / (float)Math.Tan(Math.PI / 180.0 * m_fovY / 2);

            float dist = Math.Max(distX, distY);

            m_position += new Vector3((boundTL.X + boundBR.X) * 0.5f,         // Shift of the camera
                                (boundTL.Y + boundBR.Y) * 0.5f,
                                boundBR.Z + dist);


            Apply();
        }
        public virtual void Apply()
        {
            ApplyProjectionMatrix();
            ApplyViewMatrix();
        }
        public virtual void Apply(ref Matrix4 matrix)
        {
            ////////////////// TODO:: Copy in Apply() with the correct matrix /////////////////////
            m_forwardVector.X = matrix.Row2.X;
            m_forwardVector.Y = matrix.Row2.Y;
            m_forwardVector.Z = matrix.Row2.Z;

            m_position = new Vector3(matrix.M41, matrix.M42, matrix.M43);

            m_forwardVector = m_position + m_forwardVector;
            m_rightVector = m_position - m_forwardVector;
            m_upVector = new Vector3(matrix.Row1.X, matrix.Row1.Y, matrix.Row1.Z);
            ///////////////////////////////////////////////////////////////////////////////////////

            ApplyProjectionMatrix(ref matrix);
            ApplyViewMatrix();
        }
        public virtual void ApplyProjectionMatrix()
        {
            CalculateProjectionMatrix();
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadMatrix(ref m_projectionMatrix);
        }
        public virtual void ApplyProjectionMatrix(ref Matrix4 projection)
        {
            CalculateProjectionMatrix();
            Matrix4 result = m_projectionMatrix * projection;
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadMatrix(ref result);
        }
        public virtual void ApplyViewMatrix()
        {
            CalculateViewMatrix();
            _renderer.MatrixMode(MatrixMode.Modelview);
            _renderer.LoadMatrix(ref m_viewMatrix);
        }
        public virtual void CalculateProjectionMatrix()
        {
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadIdentity();

            TransformProjectionMatrix();

            float[] matrix = new float[16];
            float[] pmatrix = m_projectionMatrix.ToArray();

            _renderer.GetFloat(GetPName.ProjectionMatrix, matrix);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    pmatrix[(i * 4) + j] = matrix[(i * 4) + j];

            Matrix4_Extensions.FromArray(m_projectionMatrix, pmatrix);

            _renderer.MatrixMode(MatrixMode.Modelview);

        }
        public virtual void CalculateViewMatrix()
        {
            m_viewMatrix = Matrix4.LookAt(m_position, m_targetPosition, m_upVector);
        }
        public virtual void Resize(int width, int height)
        {
            if (null == _renderer)
                return;

            m_aspectWidth = width;
            m_aspectHeight = height;

            if (height == 0)
                m_effectiveHeight = 1;
            else
                m_effectiveHeight = height;

            m_effectiveWidth = width;

            _renderer.Viewport(0, 0, m_effectiveWidth, m_effectiveHeight);

            m_aspectRatio = (double)((double)m_effectiveWidth / m_effectiveHeight);

            Apply();
        }
        public virtual void SetOrthographic()
        {
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadIdentity();
            _renderer.Ortho(0, m_aspectWidth, 0, m_aspectHeight, m_clipNear, m_clipFar / 10);
            _renderer.MatrixMode(MatrixMode.Modelview);

            m_projectionMode = enuProjectionMode.Orthographic3D;
            ProjectionModeChanged();
        }
        public virtual void SetOrthographic(double clipNear, double clipFar)        // Created for compass axis. Failed so you may want to delete
        {
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadIdentity();
            _renderer.Ortho(0, m_aspectWidth, 0, m_aspectHeight, clipNear, clipFar);
            _renderer.MatrixMode(MatrixMode.Modelview);

            m_projectionMode = enuProjectionMode.Orthographic3D;
            ProjectionModeChanged();
        }
        public virtual void SetPerspective()
        {
            _renderer.MatrixMode(MatrixMode.Projection);
            _renderer.LoadIdentity();

            m_aspectRatio = m_aspectWidth / m_aspectHeight;

            gluPerspective(m_fovY, m_aspectRatio, m_clipNear, m_clipFar);

            _renderer.MatrixMode(MatrixMode.Modelview);

            m_projectionMode = enuProjectionMode.Perspective3D;
            ProjectionModeChanged();

        }



        public virtual void SetLookAtPosition(Vector3 target)
        {
            this.m_targetPosition = target;
        }

        public abstract void TransformProjectionMatrix();

        internal void gluPerspective(double fovy, double aspect, double zNear, double zFar)
        {
            double __glPi = 3.14159265358979323846;
            Matrix4d m = Matrix4d.Identity;
            double sine, cotangent, deltaZ;
            double radians = fovy / 2 * __glPi / 180;

            deltaZ = zFar - zNear;
            sine = Math.Sin(radians);
            if ((deltaZ == 0) || (sine == 0) || (aspect == 0))
            {
                return;
            }

            cotangent = Math.Cos(radians) / sine;

            m.M11 = cotangent / aspect;
            m.M22 = cotangent;
            m.M33 = -(zFar + zNear) / deltaZ;
            m.M34 = -1;
            m.M43 = -2 * zNear * zFar / deltaZ;
            m.M44 = 0;

            _renderer.MultMatrix(ref m);
        }
        internal void gluLookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            gluLookAt(eye.X, eye.Y, eye.Z, target.X, target.Y, target.Z, up.X, up.Y, up.Z);
        }
        internal void gluLookAt(double eyex, double eyey, double eyez, double targetx, double targety, double targetz, double upx, double upy, double upz)
        {
            Vector3 forward, side, up;
            Matrix4 m;

            forward.X = (float)(targetx - eyex);
            forward.Y = (float)(targety - eyey);
            forward.Z = (float)(targetz - eyez);

            up.X = (float)upx;
            up.Y = (float)upy;
            up.Z = (float)upz;

            forward.Normalize();

            /* Side = target x up */
            side = Vector3.Cross(forward, up);
            side.Normalize();

            /* Recompute up as: up = side x target */
            up = Vector3.Cross(side, forward);

            m = Matrix4.Identity;

            m.M11 = side.X;
            m.M21 = side.Y;
            m.M31 = side.Z;

            m.M12 = up.X;
            m.M22 = up.Y;
            m.M32 = up.Z;

            m.M13 = -forward.X;
            m.M23 = -forward.Y;
            m.M33 = -forward.Z;

            _renderer.MultMatrix(ref m);
            _renderer.Translate(-eyex, -eyey, -eyez);

            //_renderer.MultMatrix(ref m);
            //_renderer.Translate(-eyex, -eyey, -eyez);
        }

        public virtual void MouseDown(int X, int Y, enuMouseButton Button)
        {
            #region Raise Events
            if (null != this.OnMouseDown)
            {
                MouseEvent md = new MouseEvent(X, Y, 0, Button);
                this.OnMouseDown(md);
            }
            #endregion
        }
        public virtual void MouseMove(int X, int Y, enuMouseButton Button)
        {
            #region Raise Events
            if (null != this.OnMouseMove)
            {
                MouseEvent md = new MouseEvent(X, Y, 0, Button);
                this.OnMouseMove(md);
            }
            #endregion
        }
        public virtual void MouseUp(int X, int Y, enuMouseButton Button)
        {
            #region Raise Events
            if (null != this.OnMouseUp)
            {
                MouseEvent md = new MouseEvent(X, Y, 0, Button);
                this.OnMouseUp(md);
            }
            #endregion
        }
        public virtual void MouseWheel(int X, int Y, int Wheel, enuMouseButton Button)
        {
            #region Raise Events
            if (null != this.OnMouseUp)
            {
                MouseEvent md = new MouseEvent(X, Y, Wheel, Button);
                this.OnMouseWheel(md);
            }
            #endregion
        }
        public virtual void ProjectionModeChanged()
        {
            if (null != this.OnProjectionModeChanged)
                OnProjectionModeChanged(m_projectionMode);
        }

        #region Properties
        //[DataMember(Order = 1, Name = "EffectiveWidth", IsRequired = false, EmitDefaultValue = true)]
        public int EffectiveWidth
        {
            get { return m_effectiveWidth; }
        }
        //[DataMember(Order = 2, Name = "EffectiveHeight", IsRequired = false, EmitDefaultValue = true)]
        public int EffectiveHeight
        {
            get { return m_effectiveHeight; }
        }
        //[DataMember(Order = 3, Name = "Position", IsRequired = false, EmitDefaultValue = true)]
        public Vector3 Position
        {
            get { return m_position; }
        }
        //[DataMember(Order = 4, Name = "AspectWidth", IsRequired = false, EmitDefaultValue = true)]
        public int AspectWidth
        {
            get { return m_aspectWidth; }
            set { m_aspectWidth = value; }
        }
        //[DataMember(Order = 5, Name = "AspectHeight", IsRequired = false, EmitDefaultValue = true)]
        public int AspectHeight
        {
            get { return m_aspectHeight; }
            set { m_aspectHeight = value; }
        }
        //[DataMember(Order = 6, Name = "AspectRatio", IsRequired = false, EmitDefaultValue = true)]
        public double AspectRatio
        {
            get { return m_aspectRatio; }
            set { m_aspectRatio = value; }
        }
        //[DataMember(Order = 7, Name = "Viewport", IsRequired = false, EmitDefaultValue = true)]
        public int[] Viewport
        {
            get { return m_viewport; }
            set { m_viewport = value; }
        }
        //[DataMember(Order = 8, Name = "ProjectionMatrix", IsRequired = false, EmitDefaultValue = true)]
        public Matrix4 ProjectionMatrix
        {
            get { return m_projectionMatrix; }
            set { m_projectionMatrix = value; }
        }
        //[DataMember(Order = 9, Name = "ProjectionMatrix", IsRequired = false, EmitDefaultValue = true)]
        public Matrix4 ViewMatrix
        {
            get { return m_viewMatrix; }
            set { m_viewMatrix = value; }
        }
        //[DataMember(Order = 10, Name = "ProjectionMode", IsRequired = false, EmitDefaultValue = true)]
        public enuProjectionMode ProjectionMode
        {
            get { return m_projectionMode; }
            set { m_projectionMode = value; /*Apply();*/}
        }

        //[IgnoreDataMember]
        public Vector3 UpVector
        {
            get { return m_upVector; }
        }
        //[IgnoreDataMember]
        public Vector3 RightVector
        {
            get { return m_rightVector; }
        }

        //[IgnoreDataMember]
        public bool IsCurrent { get; set; }
        #endregion

        #region IDrawable Implementation
        public void UpdateFrame(RendererBase renderer)
        {
            throw new NotImplementedException();
        }
        public void RenderFrame(RendererBase renderer)
        {
            //TODO:: Enabled the Viewing Frustum Cone and the Camera shape in the scene
            if (IsCurrent) return; // Do not draw ourself
            //TODO:: Need a way to set IsCurrent to false when another camera is used to view the scene
        }
        public void ResetRenderer(RendererBase renderer)
        {
            _renderer = renderer;
        }

        public enuCullFaceMode CullingMode
        {
            get { return enuCullFaceMode.Front; }
            set { }
        }
        #endregion
    }
}
