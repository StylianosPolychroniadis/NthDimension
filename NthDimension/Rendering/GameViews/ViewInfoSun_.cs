//#define DEBUGVIEW

using NthDimension.Rendering.Drawables;

namespace NthDimension.Rendering.GameView
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.GameViews;
    using System;

    public class SunViewInfo : ViewInfo
    {
        float widith;

        public SunViewInfo(ApplicationObject parent, float width, float height)
            : base(parent)
        {
            this.widith = width;
            projectionMatrix = Matrix4.CreateOrthographic(widith, widith, -height * 0.5f, height * 0.5f);
            //projectionMatrix = Matrix4.CreateOrthographic(width, height, 0.1f, 4000);
        }

        public override void Update()
        {
#if DEBUGVIEW
            if(ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson ||
                ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPersonVR)
#endif
            {
                position            = Parent.Position;
                PointingDirection   = Parent.PointingDirection;
                modelviewMatrix     = Matrix4.LookAt(position, position + PointingDirection, new Vector3(0, 1, 0));

wasUpdated = true;
            }

#if DEBUGVIEW
            if (ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)
            {
                Vector3 pos = Scene.EyePos;// ApplicationBase.Instance.Player.Position; // (was Scene.EyePos but was completelly wrong. no shadows at all)

                float texelSize = 1f / Settings.Instance.video.shadowResolution;

                position = new Vector3((float)Math.Floor(pos.X / texelSize) * texelSize,
                    (float)Math.Floor(pos.Y / texelSize) * texelSize,
                    (float)Math.Floor(pos.Z / texelSize) * texelSize);

                Vector3 m_pointingDirection = new Vector3(0.4817817f, -0.4817817f, 0.731965f);

                PointingDirection = m_pointingDirection;// new Vector3(0.4817817f, -0.4817817f, 0.731965f); //pointingDirection = new Vector3(0.4817817f, -0.4817817f, 0.731965f);


                Vector3 camDir = Vector3.Zero;

                //modelviewMatrix = Matrix4.LookAt(position, position + (-camDir + PointingDirection).Normalized(), upVec);
                //modelviewMatrix *= ApplicationBase.Instance.Player.ViewInfo.invModelviewMatrix;

                modelviewMatrix = Matrix4.LookAt(pos, (pos + PointingDirection), upVec);
            }
#endif

            //GenerateViewProjectionMatrix();

            if (wasUpdated)
            {
                GenerateModelViewMatrix();
                GenerateViewProjectionMatrix();
                calculateVectors();

                updateChilds();

            }
        }

        //public override bool FrustrumCheck(Drawable drawable)
        //{
        //    Vector4 vSpacePos = GenericMethods.Mult(new Vector4(drawable.Position, 1), modelviewProjectionMatrix);

        //    float range = drawable.BoundingSphere * 2f / widith;

        //    if (float.IsNaN(range) || float.IsInfinity(range))
        //        return false;

        //    vSpacePos /= vSpacePos.W;

        //    return (
        //        vSpacePos.X < (1f + range) && vSpacePos.X > -(1f + range) &&
        //        vSpacePos.Y < (1f + range) && vSpacePos.Y > -(1f + range) &&
        //        vSpacePos.Z < (1f) && vSpacePos.Z > -(1f)
        //        );
        //}

        public void SetPointingDirection(Vector3 dir)
        {
            Parent.PointingDirection = dir;
        }

    }
}
