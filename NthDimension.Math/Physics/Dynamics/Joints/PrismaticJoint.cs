/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

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



#region Using Statements
using System;
using System.Collections.Generic;

using NthDimension.Physics.Dynamics;
using NthDimension.Physics.Collision.Shapes;
using NthDimension.Physics.Dynamics.Constraints;
using NthDimension.Physics.LinearMath;

#endregion

namespace NthDimension.Physics.Dynamics.Joints
{
    public class PrismaticJoint : Joint
    {
        // form prismatic joint
        FixedAngle fixedAngle;
        PointOnLine pointOnLine;

        PointPointDistance minDistance = null;
        PointPointDistance maxDistance = null;

        public PointPointDistance MaximumDistanceConstraint { get { return maxDistance; } }
        public PointPointDistance MinimumDistanceConstraint { get { return minDistance; } }

        public FixedAngle FixedAngleConstraint { get { return fixedAngle; } }
        public PointOnLine PointOnLineConstraint { get { return pointOnLine; } }

        public PrismaticJoint(PhysicsWorld physicsWorld, RigidBody body1, RigidBody body2)
            : base(physicsWorld)
        {
            fixedAngle = new FixedAngle(body1, body2);
            pointOnLine = new PointOnLine(body1, body2, body1.position, body2.position);
        }

        public PrismaticJoint(PhysicsWorld physicsWorld, RigidBody body1, RigidBody body2,float minimumDistance, float maximumDistance)
            : base(physicsWorld)
        {
            fixedAngle = new FixedAngle(body1, body2);
            pointOnLine = new PointOnLine(body1, body2, body1.position, body2.position);

            minDistance = new PointPointDistance(body1, body2, body1.position, body2.position);
            minDistance.Behavior = PointPointDistance.DistanceBehavior.LimitMinimumDistance;
            minDistance.Distance = minimumDistance;

            maxDistance = new PointPointDistance(body1, body2, body1.position, body2.position);
            maxDistance.Behavior = PointPointDistance.DistanceBehavior.LimitMaximumDistance;
            maxDistance.Distance = maximumDistance;
        }


        public PrismaticJoint(PhysicsWorld physicsWorld, RigidBody body1, RigidBody body2, JVector pointOnBody1,JVector pointOnBody2)
            : base(physicsWorld)
        {
            fixedAngle = new FixedAngle(body1, body2);
            pointOnLine = new PointOnLine(body1, body2, pointOnBody1, pointOnBody2);
        }


        public PrismaticJoint(PhysicsWorld physicsWorld, RigidBody body1, RigidBody body2, JVector pointOnBody1, JVector pointOnBody2, float maximumDistance, float minimumDistance)
            : base(physicsWorld)
        {
            fixedAngle = new FixedAngle(body1, body2);
            pointOnLine = new PointOnLine(body1, body2, pointOnBody1, pointOnBody2);
        }

        public override void Activate()
        {
            if (maxDistance != null) PhysicsWorld.AddConstraint(maxDistance);
            if (minDistance != null) PhysicsWorld.AddConstraint(minDistance);

            PhysicsWorld.AddConstraint(fixedAngle);
            PhysicsWorld.AddConstraint(pointOnLine);
        }

        public override void Deactivate()
        {
            if (maxDistance != null) PhysicsWorld.RemoveConstraint(maxDistance);
            if (minDistance != null) PhysicsWorld.RemoveConstraint(minDistance);

            PhysicsWorld.RemoveConstraint(fixedAngle);
            PhysicsWorld.RemoveConstraint(pointOnLine);
        }
    }
}
