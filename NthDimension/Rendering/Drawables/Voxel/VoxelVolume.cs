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

namespace NthDimension.Rendering.Drawables.Voxel
{
    using NthDimension.Algebra;
    public abstract class VoxelVolume : VoxelManager
    {
        private float affectionRadius;

        public VoxelVolume(VoxelManager parent)
        {
            Parent = parent;
        }

        public virtual int check(Vector3 pos)
        {
            return 1;
        }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                Parent.removeVolume(this);
                base.Position = value;
                Parent.addVolume(this);
            }
        }

        public override void kill()
        {
            Parent.removeVolume(this);
            killChilds();
        }

        public float AffectionRadius
        {
            get { return affectionRadius; }
            set
            {
                Parent.removeVolume(this);
                affectionRadius = value;
                Parent.addVolume(this);
            }
        }
    }
}
