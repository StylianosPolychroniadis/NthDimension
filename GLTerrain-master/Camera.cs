using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace GLTerrain {

    public static class MatrixHelper {
        public static Quaternion QuatFromMatrix(Matrix4 matrix) {
            float trace = matrix.M11 + matrix.M22 + matrix.M33;
            if (trace > 0.000001) {
                float s = (float) Math.Sqrt(trace + 1.0f);
                return new Quaternion(
                    (matrix.M31 - matrix.M23) / s,
                    (matrix.M13 - matrix.M31) / s,
                    (matrix.M21 - matrix.M12) / s,
                    s * 0.25f);
            } else if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33) {
                float s = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33) * 2;
                return new Quaternion(
                    s * 0.25f,
                    (matrix.M12 + matrix.M21) / s,
                    (matrix.M13 + matrix.M31) / s,
                    (matrix.M32 - matrix.M23) / s);
            } else if (matrix.M22 > matrix.M33) {
                float s = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33) * 2;
                return new Quaternion(
                    (matrix.M12 + matrix.M21) / s,
                    s * 0.25f,
                    (matrix.M23 + matrix.M32) / s,
                    (matrix.M13 - matrix.M31) / s);
            } else {
                float s = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22) * 2;
                return new Quaternion(
                    (matrix.M13 + matrix.M31) / s,
                    (matrix.M23 + matrix.M32) / s,
                    s * 0.25f,
                    (matrix.M21 - matrix.M12) / s);
            }
        }

        public static Matrix4 MatrixFromQuat(Quaternion quat) {
            Matrix4 mat = new Matrix4();
            float xx = quat.X * quat.X, xy = quat.X * quat.Y;
            float xz = quat.X * quat.Z, xw = quat.X * quat.W;
            float yy = quat.Y * quat.Y, yz = quat.Y * quat.Z;
            float yw = quat.Y * quat.W;
            float zz = quat.Z * quat.Z, zw = quat.Z * quat.W;

            mat.M11 = 1 - 2 * (yy + zz);
            mat.M12 = 2 * (xy - zw);
            mat.M13 = 2 * (xz + yw);

            mat.M21 = 2 * (xy + zw);
            mat.M22 = 1 - 2 * (xx + zz);
            mat.M23 = 2 * (yz - xw);

            mat.M31 = 2 * (xz - yw);
            mat.M32 = 2 * (yz + xw);
            mat.M33 = 1 - 2 * (xx + yy);

            mat.M44 = 1;

            return mat;
        }
    }

    public class Camera {
        public float FieldOfView { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        private float pitch = 0.0f;
        public float Pitch { get { return pitch; } set { pitch = value; UpdateOrientation(); } }

        private float yaw = 0.0f;
        public float Yaw { get { return yaw; } set { yaw = value; UpdateOrientation(); } }

        private float roll = 0.0f;
        public float Roll { get { return roll; } set { roll = value; UpdateOrientation(); } }

        public Viewport View { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; private set; }

        public Vector3 Forward {
            get {
                Matrix4 mat = MatrixHelper.MatrixFromQuat(Orientation);
                Vector3 forward = new Vector3(mat.M31, mat.M32, mat.M33);
                forward.NormalizeFast();
                return -forward;
            }
        }

        public Vector3 Right {
            get {
                Matrix4 mat = MatrixHelper.MatrixFromQuat(Orientation);
                Vector3 right = new Vector3(mat.M11, mat.M12, mat.M13);
                right.NormalizeFast();
                return right;
            }
        }

        public Vector3 Up {
            get {
                Matrix4 mat = MatrixHelper.MatrixFromQuat(Orientation);
                Vector3 up = new Vector3(mat.M21, mat.M22, mat.M23);
                up.NormalizeFast();
                return up;
            }
        }

        public Camera(float near, float far, float fov, Viewport view) {
            FieldOfView = fov;
            Near = near;
            Far = far;
            Orientation = Quaternion.Identity;
            View = view;

            Pitch = 0;
            Yaw = 0;
        }

        public void LookAt(Vector3 point, Vector3 up) {
            up.NormalizeFast();
            Matrix4 lookMat = Matrix4.LookAt(new Vector3(), point, up);
            Orientation = MatrixHelper.QuatFromMatrix(lookMat);
        }

        public void MoveForward(float units) {
            Position = Position + (Forward * units);
        }

        /// <summary>
        /// Positive is right, negative is left
        /// </summary>
        /// <param name="units"></param>
        public void Strafe(float units) {
            Position = Position + (Right * units);
        }

        public Matrix4 GetProjection() {
            return Matrix4.CreatePerspectiveFieldOfView(FieldOfView, View.AspectRatio, Near, Far);
        }

        public Matrix4 GetModelView() {
            UpdateOrientation();

            return Matrix4.CreateTranslation(-Position) * Matrix4.Rotate(Orientation);
        }

        private void UpdateOrientation() {
            if (Roll == 0.0f) {
                Orientation = Quaternion.FromAxisAngle(Vector3.UnitX, Pitch) *
                              Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
            } else {
                Orientation = Quaternion.FromAxisAngle(Right, Pitch) *
                              Quaternion.FromAxisAngle(Up, Yaw);
                Orientation *= Quaternion.FromAxisAngle(Forward, Roll);
            }
        }

    }
}
