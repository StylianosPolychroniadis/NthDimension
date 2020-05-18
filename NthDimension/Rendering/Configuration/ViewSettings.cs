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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Configuration
{
    public class ViewSettings
    {
        public float CullZNear          = 0.1f;
        public float CullZFar           = 4000f; // 4000f
        //public float FovY               = (float)Math.PI / 4f;


        public float Visibility         = 800;
        public float DistanceStatic0    = 0.55f;
        public float DistanceStatic1    = 0.65f;
        public float DistanceStatic2    = 0.70f;
        public float DistanceStatic3    = 0.75f;
        public float DistanceAnimated0  = 0.55f;
        public float DistanceAnimated1  = 0.65f;
        public float DistanceAnimated2  = 0.70f;
        public float DistanceAnimated3  = 0.75f;


    }


}
