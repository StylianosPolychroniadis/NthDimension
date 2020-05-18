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

namespace NthDimension.Rendering.Drawables.Particles
{
    using NthDimension.Algebra;

    public struct Particle
    {
        public Vector3          position;
        public Vector3          vector;
        public int              rendertype;
        public float            size;
        public float            spawnTime;
        public float            lifeTime;
        public bool             alive;

        public Particle(Vector3 position)
        {
            this.position   = position;
            rendertype      = 0;
            vector          = Vector3.Zero;
            size            = 1f;
            spawnTime       = 0f;
            lifeTime        = 10f;
            alive           = false;
        }
    }
}
