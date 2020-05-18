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
using System.Text;

namespace NthDimension.Physics.Dynamics
{

    // TODO: Check values, Documenation
    // Maybe some default materials, aka Material.Soft?
    public class Material
    {

        internal float kineticFriction = 0.3f;
        internal float staticFriction = 0.6f;
        internal float restitution = 0.0f;

        public Material() { }

        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        public float KineticFriction
        {
            get { return kineticFriction; }
            set { kineticFriction = value; }
        }

    }
}
