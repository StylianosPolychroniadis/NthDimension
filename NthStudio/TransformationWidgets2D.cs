using NthDimension.Algebra;
using NthDimension.Rasterizer.NanoVG;
using NthDimension.Rendering.GameViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public class TransformationWidgets2D
    {
        protected enum ROTATETYPE : int
        {
            ROTATE_NONE,
            ROTATE_X,           // X-Axis
            ROTATE_Y,           // Y-Axis
            ROTATE_Z,           // Z-Axis
            ROTATE_SCREEN,      // Outer ring
            ROTATE_TWIN         // Inner ring
        }

        private static ROTATETYPE predictRotation   = ROTATETYPE.ROTATE_NONE;
        private static ROTATETYPE rotation          = ROTATETYPE.ROTATE_NONE;

        public static void DrawRotation(MATRIXMODE mode, Vector3 center, ViewInfo info)
        {
            if (null == info ||
                null == info.modelviewMatrix) return;

            //center = info.Position;


            Vector3 origin      = new Vector3(info.modelviewMatrix.ExtractTranslation());
            Vector3 eye         = StudioWindow.Instance.Scene.EyePos;
            Vector3 plnorm      = new Vector3(eye - origin); plnorm.Normalize();

            origin += center;

            //origin              += Matrix4.CreateTranslation(center).ExtractTranslation();

            Vector3 direction   = origin - eye;                                             direction.Normalize();
            Vector3 right       = Vector3.Cross(direction, new Vector3(0f, 1f, 0f));        right.Normalize();
            Vector3 up          = Vector3.Cross(direction, right);                          up.Normalize();

            //Vector3 axeX        = mode == MATRIXMODE.World ? new Vector3(1f, 0f, 0f) : new Vector3(1f, 0f, 0f).TransformVector(info.modelviewMatrix);
            //Vector3 axeY        = mode == MATRIXMODE.World ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 1f, 0f).TransformVector(info.modelviewMatrix);
            //Vector3 axeZ        = mode == MATRIXMODE.World ? new Vector3(0f, 0f, 1f) : new Vector3(0f, 0f, 1f).TransformVector(info.modelviewMatrix);

            Vector3 axeX = new Vector3(1f, 0f, 0f);
            Vector3 axeY = new Vector3(0f, 1f, 0f);
            Vector3 axeZ = new Vector3(0f, 0f, 1f);

            float screenFactor = computeScreenFactor(info);

            if (mode == MATRIXMODE.Local)
            {
                axeX = axeX.TransformVector(info.invModelviewMatrix);
                axeY = axeY.TransformVector(info.invModelviewMatrix);
                axeZ = axeZ.TransformVector(info.invModelviewMatrix);
                axeX.Normalize();
                axeY.Normalize();
                axeZ.Normalize();
            }

            if(predictRotation == ROTATETYPE.ROTATE_TWIN) {
                drawCircle(origin, 1f, 1f, 1f, right * screenFactor, up * screenFactor);
            } else {
                drawCircle(origin, .2f, .2f, .2f, right * screenFactor, up * screenFactor);
            }

            if(predictRotation == ROTATETYPE.ROTATE_SCREEN)
            {
                drawCircle(origin, 1.0f, 1.0f, 1.0f, up * 1.2f * screenFactor, right * 1.2f * screenFactor);
            } else {
                drawCircle(origin, 1.0f, 0.3f, 1.0f, up * 1.2f * screenFactor, right * 1.2f * screenFactor);
            }

            Vector4 viewPlane = new Vector4(plnorm, 0);

            Vector3 xright = Vector3.Cross(direction, axeX);    xright.Normalize();
            Vector3 xfront = Vector3.Cross(right, axeX);        xfront.Normalize();

            if (predictRotation == ROTATETYPE.ROTATE_X) {
                drawCircleHalf(origin, 1f, 1f, 1f, right * screenFactor, xfront * screenFactor, ref viewPlane);
            } else {
                drawCircleHalf(origin, 1f, 0f, 0f, xright * screenFactor, xfront * screenFactor, ref viewPlane);
            }

            Vector3 yright = Vector3.Cross(direction, axeY);    yright.Normalize();
            Vector3 yfront = Vector3.Cross(right, axeY);        yfront.Normalize();

            if (predictRotation == ROTATETYPE.ROTATE_Y) {
                drawCircleHalf(origin, 0f, 1f, 0f, yright * screenFactor, yfront * screenFactor, ref viewPlane);
            } else {
                drawCircleHalf(origin, 1f, 1f, 1f, yright * screenFactor, yfront * screenFactor, ref viewPlane);
            }

            Vector3 zright = Vector3.Cross(direction, axeZ);    zright.Normalize();
            Vector3 zfront = Vector3.Cross(right, axeZ);        zfront.Normalize();

            if(predictRotation == ROTATETYPE.ROTATE_Z) {
                drawCircleHalf(origin, 1f, 1f, 1f, zright * screenFactor, zfront * screenFactor, ref viewPlane);
            } else {
                drawCircleHalf(origin, 0f, 0f, 1f, zright * screenFactor, zfront * screenFactor, ref viewPlane);
            }

            if (rotation != ROTATETYPE.ROTATE_NONE &&
               rotation != ROTATETYPE.ROTATE_TWIN)
                drawAngle(origin, new Vector3(), new Vector3(), 0f);    // TODO:: NotImplemented
        }

        private static float computeScreenFactor(ViewInfo info)
        {

            //Matrix4 viewproj = info.modelviewMatrix * info.projectionMatrix; // use info modelviewproj direcctly

            float[] fmat = new float[16];
            StudioWindow.Instance.Renderer.GetFloat(NthDimension.Rasterizer.GetPName.ProjectionMatrix, fmat);

            Matrix4 proj = new Matrix4(fmat);

            Matrix4 viewproj = info.modelviewMatrix * proj;

            Vector3 translation = info.modelviewMatrix.ExtractTranslation();
            

            Vector4 trf = new Vector4(translation.X, translation.Y, translation.Z, 1.0f);
            //trf.Transform(viewproj);
            trf = Vector4.Transform(trf, viewproj);
            float mDisplayScale = 1.0f;
            return mDisplayScale * 0.15f * trf.W;
            //m_ScreenFactor = mDisplayScale * 2f * trf.W;
        }


        private static void drawAngle(Vector3 orig, Vector3 vtx, Vector3 vty, float angle)
        {
            //// Convert to Mesh

            //int i = 0;

            //DepthTest = false;     //GL.Disable(EnableCap.DepthTest);
            //CullFace = false;      // GL.Disable(EnableCap.CullFace);
            //Lighting = false;      //GL.Disable(EnableCap.Lighting);

            //Blend = true;          //GL.Enable(EnableCap.Blend);

            //BlendFunc(Renderer.BlendingFactorSrc.SrcAlpha, Renderer.BlendingFactorDest.OneMinusSrcAlpha);

            //GL.Color4(1, 1, 0, 0.5f);

            //BeginFixedPipeline3D(Geometry.BeginMode.TriangleFan);
            //InsertVertex(orig);

            //for (i = 0; i <= 50; i++)
            //{
            //    Algebra.Vector3 vt;
            //    vt = vtx * (float)Math.Cos(((angle) / 50) * i);
            //    vt += vty * (float)Math.Sin(((angle) / 50) * i);
            //    vt += orig;
            //    InsertVertex(vt.X, vt.Y, vt.Z);
            //}
            //EndFixedPipeline3D();

            //Blend = false;

            //GL.Color4(1, 1, 0.2f, 1);

            //BeginFixedPipeline3D(Geometry.BeginMode.LineLoop);
            //InsertVertex(orig);
            //for (i = 0; i <= 50; i++)
            //{
            //    Algebra.Vector3 vt;
            //    vt = vtx * (float)Math.Cos(((angle) / 50) * i);
            //    vt += vty * (float)Math.Sin(((angle) / 50) * i);
            //    vt += orig;
            //    InsertVertex(vt.X, vt.Y, vt.Z);
            //}
            //EndFixedPipeline3D();
        }
        private static void drawCircle(Vector3 orig, float r, float g, float b, Vector3 vtx, Vector3 vty)
        {
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            //GL.Color4(r, g, b, 1f);

            //BeginFixedPipeline3D(Geometry.BeginMode.LineLoop);
            //for (int i = 0; i < 50; i++)
            //{
            //    Algebra.Vector3 vt;
            //    vt = vtx * (float)Math.Cos((2 * MathHelper.Pi / 50) * i);
            //    vt += vty * (float)Math.Sin((2 * MathHelper.Pi / 50) * i);
            //    vt += orig;
            //    InsertVertex(vt.X, vt.Y, vt.Z);
            //}
            //EndFixedPipeline3D();

            if (null == StudioWindow.Instance) return;

            NVGcontext vg = StudioWindow.vg;
            StudioWindow.Instance.Renderer.DepthTestEnabled = false;

            NanoVG.nvgStrokeColor(vg,
                                System.Drawing.Color.FromArgb(255, (int) (r * 255), (int)(g * 255), (int)(b * 255)).ToNVGColor());

            NanoVG.nvgBeginPath(vg);
            
            for(int i = 0; i < 50; i++)
            {
                Vector3 vt  =  vtx * (float)Math.Cos((2 * MathHelper.Pi / 50) * i);
                vt          += vty * (float)Math.Sin((2 * MathHelper.Pi / 50) * i);
                vt          += orig;

                Vector2 v = ((StudioWindow)StudioWindow.Instance).UnProject(vt,
                    StudioWindow.Instance.Player.ViewInfo.modelviewMatrix,
                    StudioWindow.Instance.Player.ViewInfo.projectionMatrix,
                    StudioWindow.Instance.Width,
                    StudioWindow.Instance.Height);

                if (i == 0)
                    NanoVG.nvgMoveTo(vg, v.X, v.Y);

                NanoVG.nvgLineTo(vg, v.X, v.Y);


            }

            NanoVG.nvgClosePath(vg);
            NanoVG.nvgStroke(vg);

        }
        private static void drawCircleHalf(Vector3 orig, float r, float g, float b, Vector3 vtx, Vector3 vty, ref Vector4 camPlan)
        {
            //GL.Disable(EnableCap.DepthTest);
            //GL.Disable(EnableCap.Lighting);
            //GL.Color4(r, g, b, 1f);

            //BeginFixedPipeline3D(Geometry.BeginMode.LineStrip);
            //for (int i = 0; i < 30; i++)
            //{
            //    Algebra.Vector3 vt;
            //    vt = vtx * (float)Math.Cos((MathHelper.Pi / 30) * i);
            //    vt += vty * (float)Math.Sin((MathHelper.Pi / 30) * i);
            //    vt += orig;
            //    if (!float.IsNaN(camPlan.DotNormal(vt))) // float.NaN
            //        InsertVertex(vt.X, vt.Y, vt.Z);
            //}
            //EndFixedPipeline3D();

            if (null == StudioWindow.Instance) return;

            NVGcontext vg = StudioWindow.vg;
            StudioWindow.Instance.Renderer.DepthTestEnabled = false;

            NanoVG.nvgStrokeColor(vg,
                                System.Drawing.Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255)).ToNVGColor());

            NanoVG.nvgBeginPath(vg);

            for (int i = 0; i < 29; i++)
            {
                Vector3 vt = vtx * (float)Math.Cos((MathHelper.Pi / 30) * i);
                vt += vty * (float)Math.Sin((MathHelper.Pi / 30) * i);
                vt += orig;

                Vector2 v = ((StudioWindow)StudioWindow.Instance).UnProject(vt,
                    StudioWindow.Instance.Player.ViewInfo.modelviewMatrix,
                    StudioWindow.Instance.Player.ViewInfo.projectionMatrix,
                    StudioWindow.Instance.Width,
                    StudioWindow.Instance.Height);

                if (i == 0)
                    NanoVG.nvgMoveTo(vg, v.X, v.Y);
                else
                    if (!float.IsNaN(camPlan.DotNormal(vt)))
                        NanoVG.nvgLineTo(vg, v.X, v.Y);


            }

            //NanoVG.nvgClosePath(vg);
            NanoVG.nvgStroke(vg);
        }
    }
}
