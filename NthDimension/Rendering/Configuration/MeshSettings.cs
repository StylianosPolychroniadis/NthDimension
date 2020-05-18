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
    public class MeshSettings
    {

        /// <summary>
        /// Factor used to calculate the Target Face Count for static meshes Level 0. 
        /// Default is 1.0 (100%) and should not be changed
        /// </summary>
        //public float Detail0Static = 1.00f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for static meshes Level 1
        /// </summary>
        public float LodStaticOne = 0.75f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for static meshes Level 2
        /// </summary>
        public float LodStaticTwo = 0.50f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for static meshes Level 3
        /// </summary>
        public float LodStaticThree = 0.05f;

        /// <summary>
        /// Factor used to calculate the Target Face Count for animated meshes Level 0. 
        /// Default is 1.0 (100%) and should not be changed
        /// </summary>
        //public float Detail0Animated = 1.00f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for animated meshes Level 1.
        /// </summary>
        public float LodAnimatedOne = 0.75f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for animated meshes Level 2.
        /// </summary>
        public float LodAnimatedTwo = 0.50f;
        /// <summary>
        /// Factor used to calculate the Target Face Count for animated meshes Level 3.
        /// </summary>
        public float LodAnimatedThree = 0.05f;
        /// <summary>
        /// Specifies the distance-threshold value for pair-contraction. This
		///	option is only applicable for the `PairContract' algorithm and defaults to 0.
        /// </summary>
        public float DistanceThreshold = 0f;
        /// <summary>
        /// Specifies strict mode. This will cause simplification to fail
		///	if the input mesh is malformed or any other anomaly occurs
        /// </summary>
        public bool StrictMode = false;
        /// <summary>
        /// Expands the input progressive-mesh to the desired face-count. 
		///	If this option is specified, num-faces refers to the number of faces to 
		///	restore the mesh to.
        /// </summary>
        //public bool Restore = false;

        public bool SplitRecords = false;

        /// <summary>
        /// Adds vertex-split records to the resulting .obj file, effectively
        ///	creating a progressive-mesh representation of the input mesh
        /// </summary>
        //public bool ProgressiveMesh = true;

    }
}
