/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/


using System;
using System.Collections.Generic;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables;
using NthDimension.Physics.Collision;
using NthDimension.Physics.Dynamics;
using NthDimension.Physics.LinearMath;

namespace NthDimension.Rendering.GameViews
{
    // TODO:: Rename to Camera
    public class ViewInfo : ApplicationObject
    {
        public Matrix4          modelviewMatrix                     = Matrix4.Identity;
        public Matrix4          invModelviewMatrix;
        public Matrix4          projectionMatrix                    = Matrix4.Identity;
        public Matrix4          modelviewProjectionMatrix           = Matrix4.Identity;
        public Matrix4          invModelviewProjectionMatrix;

        public float            zNear                   = Settings.Instance.view.CullZNear; // { get { return Configuration.Settings.Instance.view.CullZNear; } }
        public float            zFar                    = Settings.Instance.view.CullZFar; // { get { return Configuration.Settings.Instance.view.CullZFar; } }

        public float            dofFocus                = 10;              // original 10

        public float            fovy                    { get { float ret = MathHelper.DegreesToRadians(45f);
                                                                return ret; } } //= (float)Math.PI / 2;
        public float            aspect                  = 1;

        public Vector3          upVec                   = new Vector3(0, 1, 0);
        public Vector3          pointingDirectionRight;
        public Vector3          pointingDirectionUp;

        new public Vector3      pointingDirection;
        new public Vector3      position;

        #region Ctor
        public ViewInfo(ApplicationObject parent)
        {
            Parent = parent;
            UpdateProjectionMatrix();
        }

        public ViewInfo()
        {
        }
        #endregion

        #region Update

        public override void Update()
        {
            if (Parent.PointingDirection != Vector3.Zero && Parent.PointingDirection != PointingDirection)
            {
                PointingDirection = Parent.PointingDirection;
                wasUpdated = true;
            }

            if (Parent.Position != Vector3.Zero && Parent.Position != position)
            {
                #region Original
                //Position = Parent.Position;     // Original
                #endregion

                #region New Implementation

                if (Parent is ApplicationUser && ((ApplicationUser)Parent).PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)
                    position = ApplicationBase.Instance.Scene.EyePos;
                else
                    position = Parent.Position;

                #endregion

                wasUpdated = true;
            }

            if (wasUpdated)
            {
                GenerateModelViewMatrix();
                GenerateViewProjectionMatrix();
                calculateVectors();

                updateChilds();

            }
        }

        #endregion


        #region Frustrum Check
        //public virtual bool FrustrumCheck(Drawable drawable)
        //{
        //    Vector4 vSpacePos = GenericMethods.Mult(new Vector4(drawable.Position, 1), modelviewProjectionMatrix);


                

        //    float range = drawable.BoundingSphere;

        //    float distToDrawAble = (position - drawable.Position).Length;

        //    if (distToDrawAble < range * 1.5f)
        //        return true;

        //    if (distToDrawAble - range > zFar)
        //        return false;

        //    if (vSpacePos.W <= 0)
        //        return false;

        //    range /= vSpacePos.W * 0.6f;

        //    if (float.IsNaN(range) || float.IsInfinity(range))
        //        return false;

        //    vSpacePos /= vSpacePos.W;

        //    return (
        //        vSpacePos.X < (1f + range) && vSpacePos.X > -(1f + range) &&
        //        vSpacePos.Y < (1f + range * aspect) && vSpacePos.Y > -(1f + range * aspect)
        //        );
        //}
        #endregion

        public void GenerateViewProjectionMatrix()
        {
            modelviewProjectionMatrix       = Matrix4.Mult(modelviewMatrix, projectionMatrix);
            invModelviewProjectionMatrix    = Matrix4.Invert(modelviewProjectionMatrix);
        }
        public Matrix4 GenerateModelViewMatrix()
        {
            modelviewMatrix = Matrix4.LookAt(Position, Position + PointingDirection, upVec);
            invModelviewMatrix = Matrix4.Invert(modelviewMatrix);
            return modelviewMatrix;
        }
        public float GetFocus()
        {
            RaycastCallback raycast = new RaycastCallback(raycastCallback); RigidBody body; JVector normal; float frac;

            bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position), 
                                                                     GenericMethods.FromOpenTKVector(PointingDirection),
                                                                     raycast, 
                                                                     out body, 
                                                                     out normal, 
                                                                     out frac);

            if (result)
                return frac;
            else
                return 10;// return zFar; // using old zFar = 100 instead of 4000
            
        }
        public float GetFocus(float smoothing)
        {
            return dofFocus = GetFocus() * (1 - smoothing) + dofFocus * smoothing;
        }
        public Matrix4 UpdateProjectionMatrix()
        {
            return projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);
        }
        
        internal bool checkForUpdates(List<Drawable> drawables)
        {
            //Parallel.ForEach(drawables, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
            //    (drawable, state) =>
                foreach (var drawable in drawables)
                {
                    if (drawable.wasUpdated && /*drawable.IsVisible*/  frustrumCheck(drawable))
                    {
                        wasUpdated = true;
                        //return;
                        break;
                    }
                }
                //);
            return wasUpdated = false;
        }
        internal void calculateVectors()
        {
            Vector4 bottomLeft = new Vector4(-1, -1, 1, 1);
            bottomLeft = GenericMethods.Mult(bottomLeft, invModelviewProjectionMatrix);
            bottomLeft /= bottomLeft.W;

            Vector4 bottomRight = new Vector4(1, -1, 1, 1);
            bottomRight = GenericMethods.Mult(bottomRight, invModelviewProjectionMatrix);
            bottomRight /= bottomRight.W;

            Vector4 topLeft = new Vector4(-1, 1, 1, 1);
            topLeft = GenericMethods.Mult(topLeft, invModelviewProjectionMatrix);
            topLeft /= topLeft.W;

            pointingDirectionUp = topLeft.Xyz - bottomLeft.Xyz;
            pointingDirectionRight = bottomRight.Xyz - bottomLeft.Xyz;
        }

        
        private bool raycastCallback(RigidBody body, JVector normal, float frac)
        {
            return (body != Parent.RigidBody);
        }

        public virtual bool frustrumCheck(Drawable drawable)
        {
            Vector4 vSpacePos = GenericMethods.Mult(new Vector4(drawable.Position, 1), modelviewProjectionMatrix);

            float range = drawable.BoundingSphere;

            float distToDrawAble = (position - drawable.Position).Length;

            if (distToDrawAble < range * 1.5f)
                return true;

            if (distToDrawAble - range > zFar)
                return false;

            if (vSpacePos.W <= 0)
                return false;

            range /= vSpacePos.W * 0.6f;

            if (float.IsNaN(range) || float.IsInfinity(range))
                return false;

            vSpacePos /= vSpacePos.W;

            return (
                vSpacePos.X < (1f + range) && vSpacePos.X > -(1f + range) &&
                vSpacePos.Y < (1f + range * aspect) && vSpacePos.Y > -(1f + range * aspect)
                );
        }


        #region Properties
        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public override Vector3 PointingDirection
        {
            get
            {
                return Vector3.Normalize(pointingDirection);
            }
            set
            {
                pointingDirection = value * zFar;
            }
        }
        #endregion
    }
}
