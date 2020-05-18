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
    // Note: each terrain quad should have it's own ParticleFloorKiller, or, make that dynamic bounding sphere terrain->height killer
    public class ParticleAffectorFloorKiller : ParticleAffector
    {
        float level;

        public ParticleAffectorFloorKiller(float level)
        {
            this.level = level;
        }

        public override void affect(ref Particle[] particles)
        {
            int parts = particles.Length;
            for (int i = 0; i < parts; i++)
            {
                if (particles[i].position.Y < level)
                    particles[i].alive = false;
            }

        }
    }
}
