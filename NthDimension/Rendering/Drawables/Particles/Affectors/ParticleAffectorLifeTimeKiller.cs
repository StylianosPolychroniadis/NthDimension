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
    public class ParticleAffectorLifeTimeKiller : ParticleAffector
    {
        public ParticleAffectorLifeTimeKiller(ApplicationObject parent)
        {
            Parent = parent;
        }

        public override void affect(ref Particle[] particles)
        {
            int parts = particles.Length;
            float timestamp = ApplicationBase.Instance.VAR_FrameTime;
            for (int i = 0; i < parts; i++)
            {
                if (particles[i].spawnTime + particles[i].lifeTime < timestamp)
                    particles[i].alive = false;
            }
        }
    }
}
