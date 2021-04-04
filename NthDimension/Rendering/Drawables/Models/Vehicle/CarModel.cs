using Newtonsoft.Json;
using NthDimension.Algebra;
using NthDimension.Rendering.GameViews;
using System;
using System.IO;
using System.Linq;

namespace NthDimension.Rendering.Drawables.Models
{
    
    public class GroundVehicleModel : Model
    {
        public enum VehicleController                       { AI, Player, Network, Passive }
        public enum PlayerView                              { Exterior, Cabin, Camera, Debug }
        

        public sealed class GroundVehicleController
        {
            protected readonly ApplicationUserInput input;

            public bool Accelerate, Brake, Left, Right, CruiseControl, CControlInc, CControlDec;
            public bool EngineStart, Forward, Reverse;
            public bool ExteriorView, CabinView;

            public GroundVehicleController() { this.input = ApplicationBase.Instance.AppInput; }
            public GroundVehicleController(ApplicationUserInput input, int keyboardDevice ) //*KeyboardDevice device*/)
            {
                this.input          = input;

                Accelerate          = input.Up      || input.W; // device[Key.Up];
                Brake               = input.Down    || input.S; // device[Key.Down];
                Left                = input.Left    || input.A; // device[Key.Left];
                Right               = input.Right   || input.D; // device[Key.Right];

                EngineStart         = false;                    // device[Key.E];    //epsilon

                CruiseControl       = false;                    // device[Key.C];
                CControlInc         = false;                    // device[Key.KeypadPlus];
                CControlDec         = false;                    // device[Key.KeypadMinus];

                Forward             = input.Q;                  // false; // device[Key.F];    //digama
                Reverse             = input.Z;                  // false; // device[Key.RShift];

                CabinView           = input.F1;                 // false; // device[Key.Number1];
                ExteriorView        = input.F2;                 // false; // device[Key.Number2];

                //if (CabinView || ExteriorView)
                //    GroundVehicleCamera.ResetView();  // Hold mouse at middle of screen
            }
        }

        

        public const int            rightIndex              = 0;
        public const int            upIndex                 = 1;
        public const int            forwardIndex            = 2;

        public ViewInfo             ViewInfo;

        public GroundVehicleController 
                                    prevState               = new GroundVehicleController();

        public Vector3              Dimensions;
        public Vector3              DriverEyeLocation;
        public Vector2              DriverViewAngle;
        public Vector3              DebugLocation;

        public float                Mass;                       // Kg????
        public float                MaxEngineForce;             // Nt/cm^2????
        public float                MaxBrakeForce;              // Nt/cm^2????
        public float                MaximumSpeed;               // Km/h?????
        public float                engineForce;                // Nt????
        public float                brakeForce;                 // Nt????
        public float                steeringValue;              // angle [ ... ]
        public float                SteeringIncrement;          // angle, arc?
        public float                SteeringClamp;              // angle, arc?
        public float                ExteriorEyeDistance;        // 
        public float                FrameDelta = 0;
        public float                CurrentSpeedKmHour;
        public float                CruiseControl = 0;
        public float                gear = 1;
        public bool                 EngineRunning = true;

        public PlayerView           ViewMode                    { get; private set; }

        private float x, y;
        



        public GroundVehicleModel(ApplicationObject parent)
            :base(parent)
        {
            ViewMode = PlayerView.Exterior;
        }

        internal virtual void updateExterior() { }
        internal virtual void updateInterior() { }
        public override void Update()
        {
            base.Update();

            this.applyForces(ApplicationBase.Instance.VAR_FrameTime, new GroundVehicleController());

            x = (float) ApplicationBase.Instance.AppInput.MouseX / ApplicationBase.Instance.Width;
            y = (float) ApplicationBase.Instance.AppInput.MouseY / ApplicationBase.Instance.Height;

            switch (ViewMode)
            {
                case PlayerView.Camera:
                case PlayerView.Exterior:
                    {
                        this.updateExterior();
                    }
                    break;
                case PlayerView.Debug:
                case PlayerView.Cabin:
                    {
                        this.updateInterior();
                    }
                    break;
            }

           
        }


        internal virtual GroundVehicleController handleAi(GroundVehicleModel target)
        {
            /*Map CurrentMap,*/
            return new GroundVehicleController();
        }
        internal virtual void handleInput(GroundVehicleController k) { }

        internal virtual void applyForces(float elapsedTime, GroundVehicleController controller)
        { }
        public void          SwitchView(PlayerView viewMode)
        {
            ViewMode = viewMode;
        }

        public Matrix4 GenerateLookAt()
        {
            Matrix4         e                       = new Matrix4();
            Matrix4         t                       = new Matrix4();
            Quaternion      orientation             = Quaternion.Identity;
            Vector3         centerOfMassPosition    = Vector3.Zero;

            
            orientation             = Quaternion.FromMatrix(new Matrix3(ViewInfo.Orientation)); //JiterPhys;
            centerOfMassPosition    = ViewInfo.Position;

#if BULLETPHYS
            DriverViewAngle                         = vehicle.DriverViewAngle;
            ExteriorEyeDistance                     = vehicle.ExteriorEyeDistance;
            DebugLocation                           = vehicle.DebugLocation;

            Quaternion  orientation                 = vehicle.raycastVehicle.RigidBody.Orientation;
            Vector3     centerOfMassPosition        = vehicle.body.CenterOfMassPosition;
#else
            throw new NotImplementedException("Get the orientation quaternion and position vector of the physics body");
#endif

            switch (ViewMode)
            {
                case PlayerView.Cabin:
                    e = Matrix4.CreateTranslation(DriverEyeLocation);
                    t = Matrix4.CreateTranslation(DriverEyeLocation + new Vector3(0, 0, 2)) * 
                                                        Matrix4.CreateRotationX((y * DriverViewAngle.X) - (DriverViewAngle.X / 2)) * 
                                                        Matrix4.CreateRotationY((-x * DriverViewAngle.Y) + (DriverViewAngle.Y / 2));
                    break;
                case PlayerView.Exterior:
                    t = Matrix4.Identity;
                    e = Matrix4.CreateTranslation(0, ExteriorEyeDistance, 0) * 
                                                    Matrix4.CreateRotationX((y * MathFunc.PiOver4) + MathFunc.PiOver4) * 
                                                    Matrix4.CreateRotationY((-x * MathFunc.TwoPi));
                    break;
                case PlayerView.Camera:
                    return Matrix4.LookAt(new Vector3(4, 0.1f, 2), centerOfMassPosition, Vector3.UnitY);
                case PlayerView.Debug:
                    e = Matrix4.CreateTranslation(DebugLocation);
                    t = Matrix4.CreateTranslation(DebugLocation + new Vector3(0, 0, 2)) * 
                                                    Matrix4.CreateRotationX((y * MathFunc.Pi) - (MathFunc.Pi / 2)) * 
                                                    Matrix4.CreateRotationY((-x * MathFunc.Pi) + MathFunc.PiOver2);
                    break;
            }

            e *= Matrix4.CreateFromQuaternion(orientation);
            t *= Matrix4.CreateFromQuaternion(orientation);
            e *= Matrix4.CreateTranslation(centerOfMassPosition);
            t *= Matrix4.CreateTranslation(centerOfMassPosition);

            return Matrix4.LookAt(e.ExtractTranslation(), t.ExtractTranslation(), Vector3.UnitY);
        }

        

    }

    public class CarModel : GroundVehicleModel
    {
        new public static string        nodename            = "carmodel";
        const string                    assetsPath          = "models\\vehicles\\";
        private string                  carIdentifier;

#if BULLETPHYS
        public CollisionShape           collisionShape;
        public RigidBody                body;
        public RaycastVehicle           raycastVehicle;
#endif

        internal CarWheelModel          m_wheelFR;
        internal CarWheelModel          m_wheelFL;
        internal CarWheelModel          m_wheelBR;
        internal CarWheelModel          m_wheelBL;
        internal CarCabinModel          m_cabin;            // TODO:: (Idea) Switch all interrior models to CarCockpitModel class (contains cabin, gauges, etc)
        internal CarGaugeModel          m_speedGauge;
        internal CarSteeringWheelModel  m_steering;

        #region possible exxageration (remove if not necessary)
        internal Vector3                m_wheelFRPosition;
        internal Vector3                m_wheelFLPosition;
        internal Vector3                m_wheelBRPosition;
        internal Vector3                m_wheelBLPosition;
        internal Vector3                m_cabinPosition;
        internal Vector3                m_needlePosition;

        internal Matrix4                m_wheelFROrientation;
        internal Matrix4                m_wheelFLOrientation;
        internal Matrix4                m_wheelBROrientation;
        internal Matrix4                m_wheelBLOrientation;
        internal Matrix4                m_cabinOrientation;
        internal Matrix4                m_needleOrientation;
#endregion

        public PlayerView               ViewMode            { get; set; }
        public VehicleController        ControllerMode      { get; set; }

       


        public CarModel(ApplicationObject parent, VehicleController controller, string path, int color, ref Rendering.Loaders.MeshLoader meshLoader)
            :base(parent)
        {
            ViewMode            = PlayerView.Exterior;
            ControllerMode      = controller;

            carIdentifier       = assetsPath + path;
            DriverEyeLocation   = new Vector3();

            Load(carIdentifier + "\\config.json", this);

#if BULLETPHYS
            CollisionShape chassisShape     = new BoxShape(Dimensions.Y / 2, Dimensions.Z / 2, Dimensions.X / 2);
            collisionShape                  = new CompoundShape(); 

            OpenTK.Matrix4 localTrans       = OpenTK.Matrix4.CreateTranslation(0 * OpenTK.Vector3.UnitY);               //localTrans effectively shifts the center of mass with respect to the chassis

            ((CompoundShape)collisionShape).AddChildShape(localTrans, chassisShape);

            RaycastVehicle.VehicleTuning    tuning  = new RaycastVehicle.VehicleTuning();
            raycastVehicle                          = new RaycastVehicle(tuning, body, vehicleRayCaster);

            body.ActivationState                    = ActivationState.DisableDeactivation;
            
            raycastVehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);

            raycastVehicle.AddWheel(new Vector3(FrontWheelLocation.Y, 
                                                SuspensionHeight, 
                                                FrontWheelLocation.X), 
                                    wheelDirectionConstrolShape, 
                                    wheelAxleControlShape, 
                                    SuspensionRestLength, 
                                    WheelRadius, 
                                    tuning, 
                                    true);

            raycastVehicle.AddWheel(new Vector3(-FrontWheelLocation.Y, 
                                                SuspensionHeight, 
                                                FrontWheelLocation.X), 
                                    wheelDirectionConstrolShape, 
                                    wheelAxleControlShape, 
                                    SuspensionRestLength, 
                                    WheelRadius, 
                                    tuning, 
                                    true);

            raycastVehicle.AddWheel(new Vector3(-RearWheelLocation.Y, 
                                                SuspensionHeight, 
                                                -RearWheelLocation.X), 
                                    wheelDirectionConstrolShape, 
                                    wheelAxleControlShape, 
                                    SuspensionRestLength, 
                                    WheelRadius, 
                                    tuning, 
                                    false);

            raycastVehicle.AddWheel(new Vector3(RearWheelLocation.Y, 
                                                SuspensionHeight, 
                                                -RearWheelLocation.X), 
                                    wheelDirectionConstrolShape, 
                                    wheelAxleControlShape, 
                                    SuspensionRestLength, 
                                    WheelRadius, 
                                    tuning, 
                                    false);

            for (int i = 0; i < raycastVehicle.NumWheels; i++)
            {
                WheelInfo wheel                 = raycastVehicle.GetWheelInfo(i);
                wheel.SuspensionStiffness       = SuspensionStiffness;
                wheel.WheelsDampingRelaxation   = SuspensionDamping;
                wheel.WheelsDampingCompression  = SuspensionCompression;
                wheel.FrictionSlip              = WheelFriction;
                wheel.RollInfluence             = RollInfluence;
            }
#endif

            SetBodyMesh(string.Format("{0}\\body.obj", path), string.Empty, Vector3.Zero.ToString(), Matrix4.Identity.ToString(), Vector3.One.ToString() );
            SetWheelMesh(string.Format("{0}\\wheel.obj", path), string.Empty);
            SetCabinMesh(string.Format("{0}\\cabin.obj", path), string.Empty);
        }


        internal override void applyForces(float elapsedTime, GroundVehicleController controller)
        {
#if BULLETPHYS
            raycastVehicle.ApplyEngineForce(engineForce*gear, 2);
            raycastVehicle.SetBrake(brakeForce, 2);
            raycastVehicle.ApplyEngineForce(engineForce*gear, 3);
            raycastVehicle.SetBrake(brakeForce, 3);
            raycastVehicle.SetBrake(brakeForce, 0);
            raycastVehicle.SetBrake(brakeForce, 1);

            raycastVehicle.SetSteeringValue(steeringValue, 0);
            raycastVehicle.SetSteeringValue(steeringValue, 1);
#endif
        }
        internal override void updateExterior()
        {
            /*
            OpenTK.Matrix4 rotation         = OpenTK.Matrix4.CreateRotationX(raycastVehicle.GetWheelInfo(1).Rotation);

            OpenTK.Matrix4 wheelFrontLeft   = OpenTK.Matrix4.CreateRotationY(-(float)MathHelper.PiOver2) * rotation;
            wheelFrontLeft                  = raycastVehicle.GetWheelTransformWS(0) * LookAt;

            OpenTK.Matrix4 wheelFrontRight  = OpenTK.Matrix4.CreateRotationY((float)MathHelper.Pi);
            wheelFrontRight                *= raycastVehicle.GetWheelTransformWS(1) * LookAt;

            OpenTK.Matrix4 wheelBackLeft    = OpenTK.Matrix4.CreateRotationY((float)MathHelper.Pi);
            wheelBackLeft                  *= raycastVehicle.GetWheelTransformWS(2) * LookAt;

            OpenTK.Matrix4 wheelBackRight   = raycastVehicle.GetWheelTransformWS(3) * LookAt;
            */

        }
        internal override void updateInterior() { }

        internal override void handleInput(GroundVehicleController k)
        {
            base.handleInput(k);

#if BULLETPHYS
            CurrentSpeedKmHour = raycastVehicle.CurrentSpeedKmHour;
#endif
            float maxSteering = SteeringClamp * (1 - (CurrentSpeedKmHour / MaximumSpeed));
            float incSteering = SteeringIncrement * (1 - (CurrentSpeedKmHour / MaximumSpeed));
            if (k.Brake || k.Accelerate) CruiseControl = 0;
            else if (k.CruiseControl && !prevState.CruiseControl)
            {
                if (CruiseControl == 0) CruiseControl = 10;
                else CruiseControl = 0;
            }
            if (CruiseControl > 0 && k.CControlInc && !prevState.CControlInc) CruiseControl += 10;
            if (CruiseControl > 0 && k.CControlDec && !prevState.CControlDec) CruiseControl -= 10;
            if (!k.Left && !k.Right && steeringValue > -SteeringIncrement * 3 && steeringValue < SteeringIncrement * 3) steeringValue = 0;
            if (k.Left)
            {
                steeringValue += incSteering;
                if (steeringValue > maxSteering) steeringValue = maxSteering;
            }
            else if (steeringValue > 3 * SteeringIncrement) steeringValue -= 3 * SteeringIncrement;

            if (k.Right)
            {
                steeringValue -= incSteering;
                if (steeringValue < -maxSteering)
                    steeringValue = -maxSteering;
            }
            else if (steeringValue < 3 * -SteeringIncrement) steeringValue += 3 * SteeringIncrement;

            if (CruiseControl > 0 && CurrentSpeedKmHour < CruiseControl) k.Accelerate = true;

            if (k.Accelerate && engineForce < MaxEngineForce) engineForce += 50f;
            else if (engineForce > 0) engineForce *= 0.5f;

            if (k.Brake && brakeForce < MaxBrakeForce)
            {
                engineForce *= 0.5f;
                brakeForce = MaxBrakeForce;
            }
            else brakeForce = 0;
            if (k.EngineStart && !prevState.EngineStart) EngineRunning = !EngineRunning;
            if (!EngineRunning) gear = 0;
            if (k.Forward && System.Math.Abs(CurrentSpeedKmHour) < 3f) gear = 1;
            if (k.Reverse && System.Math.Abs(CurrentSpeedKmHour) < 3f) gear = -0.5f;

            if (k.CabinView) ViewMode = PlayerView.Cabin;
            if (k.ExteriorView) ViewMode = PlayerView.Exterior;

            prevState = k;
        }
        internal override GroundVehicleController handleAi(GroundVehicleModel target)
        {
            GroundVehicleController result = new GroundVehicleController();
            ////result.CruiseControl = true;
            //CruiseControl = 20;
            ////result.Brake = true;
            //Vector2 pos = body.CenterOfMassPosition.Xz;

            //Vector2 target = CurrentMap.Roads[road].RPaths[0].Points[point].Xz;
            ////Vector2 target = tar.body.CenterOfMassPosition.Xz;

            //float angle = Misc.getVectorAngle(pos - target) + MathHelper.PiOver2;
            //angle = Misc.normalizeAngle(angle);
            //float curAngle = ((float)System.Math.Asin(raycastVehicle.ChassisWorldTransform.ExtractRotation().Y) * -2);
            //curAngle = Misc.normalizeAngle(curAngle);
            //curAngle = Misc.normalizeAngle(angle - curAngle);
            //if (curAngle > System.Math.PI && curAngle < MathHelper.TwoPi - 0.01f) result.Left = true;
            //else if (curAngle > 0.01f) result.Right = true;
            //float dis = (pos - target).Length;
            //if (dis < Dimensions.X / 2 && CurrentMap.Roads[road].RPaths[0].Points.Length - 1 > point) point++;

            return result;
        }

        public void SetBodyMesh(string mesh, string material, string position, string orientation, string size)
        {
            if(!String.IsNullOrEmpty(material))
                this.setMaterial(material);

            this.setMesh(mesh);
            this.Position           = GenericMethods.Vector3FromString(position);
            this.Orientation        = Matrix4.Identity;
            this.Size               = GenericMethods.Vector3FromString(size);
            this.Scene              = Scene;
        }
        public void SetWheelMesh(string mesh, string material)
        {
            m_wheelFR               = new CarWheelModel(this);
            m_wheelFL               = new CarWheelModel(this);
            m_wheelBR               = new CarWheelModel(this);
            m_wheelBL               = new CarWheelModel(this);

            m_wheelFR.setMesh(mesh);

            m_wheelFRPosition           = m_wheelFRPosition         = Vector3.Zero;       // TODO:: Calc Pos and Orientation Here
            m_wheelFROrientation        = m_wheelFROrientation      = Matrix4.Identity;
            m_wheelFR.Size                                          = Vector3.One;
            m_wheelFR.Scene                                         = Scene;

            m_wheelFL.setMesh(mesh);

            m_wheelFLPosition           = m_wheelFLPosition         = Vector3.Zero;       // TODO:: Calc Pos and Orientation Here
            m_wheelFLOrientation        = m_wheelFLOrientation      = Matrix4.Identity;
            m_wheelFL.Size              = Vector3.One;
            m_wheelFL.Scene             = Scene;

            m_wheelBR.setMesh(mesh);

            m_wheelBRPosition           = m_wheelBRPosition         = Vector3.Zero;       // TODO:: Calc Pos and Orientation Here
            m_wheelBROrientation        = m_wheelBROrientation      = Matrix4.Identity;
            m_wheelBR.Size              = Vector3.One;
            m_wheelBR.Scene             = Scene;

            m_wheelBL.setMesh(mesh);

            m_wheelBLPosition           = m_wheelBLPosition         = Vector3.Zero;       // TODO:: Calc Pos and Orientation Here
            m_wheelBLOrientation        = m_wheelBLOrientation      = Matrix4.Identity;
            m_wheelBL.Size              = Vector3.One;
            m_wheelBL.Scene             = Scene;

            if (String.IsNullOrEmpty(material))
            {
                m_wheelFR.setMaterial(material);
                m_wheelFL.setMaterial(material);
                m_wheelBR.setMaterial(material);
                m_wheelBL.setMaterial(material);
            }
        }
        public void SetCabinMesh(string mesh, string material)
        {
            m_cabin                                             = new CarCabinModel(this);
            m_cabin.setMesh(mesh);
            m_cabin.Position        = m_cabinPosition           = Vector3.Zero;
            m_cabin.Orientation     = m_cabinOrientation        = Matrix4.Identity;
            m_cabin.Size                                        = Vector3.One;
            m_cabin.Scene                                       = Scene;

            if (!String.IsNullOrEmpty(material))
                m_cabin.setMaterial(material);
        }
        public void SetGaugeMesh(string mesh, string material)
        {
            m_speedGauge                = new CarGaugeModel(this);
            m_speedGauge.setMesh(mesh);
            m_speedGauge.Position       = m_cabinPosition           = Vector3.Zero;
            m_speedGauge.Orientation    = m_cabinOrientation        = Matrix4.Identity;
            m_speedGauge.Size                                       = Vector3.One;
            m_speedGauge.Scene                                      = Scene;

            if (!String.IsNullOrEmpty(material))
                m_speedGauge.setMaterial(material);
        }
        public void SetSteeringWheelMesh(string mesh, string material)
        {
            m_steering                                              = new CarSteeringWheelModel(this);
            m_steering.setMesh(mesh);
            m_steering.Position                                     = Vector3.Zero;
            m_steering.Orientation                                  = Matrix4.Identity;
            m_steering.Size                                         = Vector3.One;
            m_steering.Scene                                        = Scene;

            if (!string.IsNullOrEmpty(material))
                m_steering.setMaterial(material);
        }

        public static void Load(string path, object obj)
        {

            switch (path.Split('.')[1])
            {
                case "dat":
                    string[] file = File.ReadAllLines(path);
                    for (int i = 0; i < file.Length; i++)
                    {
                        if (file[i] != "" && file[i][0] != '#' && file[i].Contains('='))
                        {
                            string[] line = file[i].Replace(" ", "").Split('=');
                            if (obj.GetType().GetField(line[0]) != null)
                            {

                                Type t = obj.GetType().GetField(line[0]).FieldType;
                                obj.GetType().GetField(line[0]).SetValue(obj, parseValue(line[1], t));
                            }
                        }
                    }
                    break;
                case "json":
                    string json = File.ReadAllText(path);
                    JsonConvert.PopulateObject(json, obj);
                    break;
            }

        }

        private static object parseValue(string v, Type t)
        {
            object value = null;
            if (t == typeof(string)) value = v;
            else if (t == typeof(int)) value = Misc.toInt(v);
            else if (t == typeof(float)) value = Misc.toFloat(v);
            else if (t == typeof(bool)) value = Convert.ToBoolean(v);
            else if (t == typeof(Vector2))
            {
                string[] p = v.Split(',');
                value = new Vector2(Misc.toFloat(p[0]), Misc.toFloat(p[1]));
            }
            else if (t == typeof(Vector3))
            {
                string[] p = v.Split(',');
                value = new Vector3(Misc.toFloat(p[0]), Misc.toFloat(p[1]), Misc.toFloat(p[2]));
            }
            else if (t == typeof(Vector2[]))
            {
                string[] points = v.Split(';');
                Vector2[] r = new Vector2[0];
                for (int j = 0; j < points.Length; j++)
                {
                    if (points[j].Contains(':'))
                    {
                        string[] tocka = points[j].Split(':');
                        Misc.Push<Vector2>(new Vector2(Misc.toFloat(tocka[0]), Misc.toFloat(tocka[1])), ref r);
                    }
                }
                value = r;
            }
            return value;
        }

    }

    public class CarCabinModel : Model
    {
        new public static string nodename = "carcabinmodel";
        public CarCabinModel(CarModel parent)
            :base(parent)
        {

        }
    }

    public class CarWheelModel : Model
    {
        new public static string nodename = "carwheelmodel";
        public CarWheelModel(CarModel parent)
            : base(parent)
        {

        }
    }

    public class CarGaugeModel : Model
    {
        new public static string nodename = "cargaugemodel";
        public CarGaugeModel(CarModel parent)
            :base(parent)
        {

        }
    }

    public class CarSteeringWheelModel : Model
    {
        public CarSteeringWheelModel(CarModel parent)
            :base(parent)
        {

        }
    }
    static class Misc
    {
        public static float toFloat(string data)
        {
            float result = 0;
            float.TryParse(data, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }
        public static void parseFloat(string data, out float result)
        {
            float.TryParse(data, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }

        public static int toInt(string data)
        {
            int result = 0;
            int.TryParse(data, out result);
            return result;
        }

        public static int Push<T>(T value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + 1);
            values[values.Length - 1] = value;
            return values.Length - 1;
        }

        public static void Push<T>(T[] value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                values[values.Length - value.Length + i] = value[i];
            }
        }

        //public static int LoadTexture(string filename, float scale)
        //{
        //    /*if (String.IsNullOrEmpty(filename))
        //        throw new ArgumentException(filename);*/

        //    int id = GL.GenTexture();
        //    GL.BindTexture(TextureTarget.Texture2D, id);

        //    Bitmap bmp = new Bitmap(filename);
        //    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
        //    BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)(bmp_data.Width / scale), (int)(bmp_data.Height / scale), 0,
        //        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        //    bmp.UnlockBits(bmp_data);

        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        //    return id;
        //}

        public static Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            Vector2[] points = new Vector2[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        public static Vector2 GetBezierPoint(double t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((float)((1 - t) * P0.X + t * P1.X), (float)((1 - t) * P0.Y + t * P1.Y));
        }
        public static float normalizeAngle(float angle)
        {
            angle %= MathFunc.Pi * 2;
            if (angle < 0) angle += MathFunc.Pi * 2;
            if (angle > (3.145f * 2)) angle = 0;
            return angle;
        }

        public static float getVectorAngle(Vector2 vector)
        {
            if (vector.Y >= 0 && vector.X >= 0) return (float)System.Math.Asin(vector.Y / vector.Length);
            if (vector.Y >= 0 && vector.X <= 0) return MathFunc.Pi - (float)System.Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X >= 0) return (MathFunc.Pi * 2) + (float)System.Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X <= 0) return MathFunc.Pi - (float)System.Math.Asin(vector.Y / vector.Length);
            return 0;
        }
    }
}
