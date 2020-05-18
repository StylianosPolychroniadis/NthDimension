﻿/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
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
using NthDimension.Physics.LinearMath;
using NthDimension.Physics.Collision.Shapes;
#endregion

namespace NthDimension.Physics.Dynamics.Joints
{

    /// <summary>
    /// A joint is a collection of internally handled constraints.
    /// </summary>
    public abstract class Joint
    {
        /// <summary>
        /// The world class to which the internal constraints
        /// should be added.
        /// </summary>
        public PhysicsWorld PhysicsWorld { get; private set; }

        /// <summary>
        /// Creates a new instance of the Joint class.
        /// </summary>
        /// <param name="physicsWorld">The world class to which the internal constraints
        /// should be added.</param>
        public Joint(PhysicsWorld physicsWorld) {this.PhysicsWorld = physicsWorld;}

        /// <summary>
        /// Adds the internal constraints of this joint to the world class.
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Removes the internal constraints of this joint from the world class.
        /// </summary>
        public abstract void Deactivate();
    }
}
