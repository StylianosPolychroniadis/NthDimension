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

namespace NthDimension.Rendering
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Drawables;
    using NthDimension.Physics.Collision;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;
    using NthDimension.Utilities;

    public abstract class ApplicationUserView : ApplicationObject
    {
        protected RaycastCallback           raycastCallback;
        protected Matrix4                   spawnOffset                 = Matrix4.CreateTranslation(new Vector3(0, -0.5f, 0));

        protected ApplicationUserInput      GameInput;

        protected bool                      wasActive                   = false;

        public Vector3                      MousePickRayStart;
        public Vector3                      MousePickRayEnd;

        public Vector3                      PositionOffset = Vector3.Zero;

        protected ApplicationUserView(ApplicationUser player, ApplicationUserInput gameInput)
        {
            this.Parent             = player;
            this.GameInput          = gameInput;
            this.raycastCallback    = new RaycastCallback(mRaycastCallback);
        }

        protected bool mRaycastCallback(RigidBody hitbody, JVector normal, float frac)
        {
            return (hitbody != Parent.AvatarBody);
        }

        public virtual void EnterView(Vector3 position) { }
        public virtual void LeaveView(ref Vector3 position) { }

        /// <summary>
        /// Returns a list of all models that consist the player avatar
        /// </summary>
        /// <returns></returns>
        public virtual Drawable[] GetAvatarModels()
        {
            return null;
        }
        /// <summary>
        /// Returns the height (Y-axis) of the ground mesh 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public virtual float GetNavigationMeshHeightAt(float X, float Z)
        {
            // NOTE NEVER USED - DELETE?
            return 0f;
        }
    }
}
