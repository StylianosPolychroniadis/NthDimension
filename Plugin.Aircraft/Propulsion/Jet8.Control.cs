using NthDimension.Algebra;

namespace Plugin.Aircraft
{

    #region Jet8 Control
    public partial class Jet8
    {
        //public class FlapsEventArgs : System.EventArgs
        //{
        //    public float Value;

        //    public FlapsEventArgs(float value)
        //    {
        //        this.Value = value;
        //    }
        //}

        //public delegate void ChangeFlaps(FlapsEventArgs e);
        //public event ChangeFlaps OnChangeFlaps;

        private float AngleZ            = 0f;
        private float AngleZMin         = -0.66f;
        private float AngleZMax         = 0f;
        private float AngleE            = 0f;
        private float AngleEMin         = -0.5f;
        private float AngleEMax         = 0.5f;
        private float AngleR            = 0f;
        private float AngleRMin         = -0.5f;
        private float AngleRMax         = 0.5f;

        private Matrix4 Flap_ZL_Matrix      = Matrix4.Identity;
        private Matrix4 Flap_ZR_Matrix      = Matrix4.Identity;
        private Matrix4 Aileron_EL_Matrix   = Matrix4.Identity;
        private Matrix4 Aileron_ER_Matrix   = Matrix4.Identity;
        private Matrix4 LevelWing_RL_Matrix = Matrix4.Identity;
        private Matrix4 LevelWing_RR_Matrix = Matrix4.Identity;


        internal void Control_Flaps(float v)
        {
            AngleZ = (AngleZ + v * 0.005f);
            if (AngleZ > AngleZMax)
                AngleZ = AngleZMax;
            else if (AngleZMin > AngleZ)
                AngleZ = AngleZMin;

            // Affine Transform occurs here
            // Matrix m = scale * (vector3 rotCenter, quaternion rotation) rotation * translation
            Quaternion qzl = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0.3f), AngleZ);
            Flap_ZL_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.057f)) * Matrix4.CreateFromQuaternion(qzl) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation

            Quaternion qzr = Quaternion.FromAxisAngle(new Vector3(1f, 0f, -0.3f), AngleZ);
            Flap_ZR_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.057f)) * Matrix4.CreateFromQuaternion(qzr) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation

        }

        internal void Control_Aileron(float v)
        {
            AngleE = (AngleE + v * 0.005f);
            if (AngleE > AngleEMax)
                AngleE = AngleEMax;
            else if (AngleE < AngleEMin)
                AngleE = AngleEMin;

            // Affine Transform occurs here
            // Matrix m = scale * (vector3 rotCenter, quaternion rotation) rotation * translation
            Quaternion qel = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0.3f), AngleE);
            Aileron_EL_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.057f)) * Matrix4.CreateFromQuaternion(qel) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation

            Quaternion qer = Quaternion.FromAxisAngle(new Vector3(1f, 0f, -0.3f), AngleE);
            Aileron_ER_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.057f)) * Matrix4.CreateFromQuaternion(qer) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation
        }

        internal void Control_Level(float v)
        {
            AngleR = (AngleR + v * 0.005f);
            if (AngleR > AngleRMax)
                AngleR = AngleRMax;
            else if (AngleR < AngleRMin)
                AngleR = AngleRMin;

            // Affine Transform occurs here
            // Matrix m = scale * (vector3 rotCenter, quaternion rotation) rotation * translation
            Quaternion qrl = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0.3f), AngleE);
            LevelWing_RL_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.064f)) * Matrix4.CreateFromQuaternion(qrl) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation

            Quaternion qrr = Quaternion.FromAxisAngle(new Vector3(1f, 0f, -0.3f), AngleE);
            LevelWing_RR_Matrix = Matrix4.CreateScale(new Vector3(1f)) *                                                                 // Scale
                                Matrix4.CreateTranslation(new Vector3(0, 0.03f, -0.064f)) * Matrix4.CreateFromQuaternion(qrr) *       // Rotation arround point (affine transform)
                                Matrix4.CreateTranslation(new Vector3(0f, 0f, 0f));                                                 // Translation

        }
    }
#endregion Jet8 Control
    
}
