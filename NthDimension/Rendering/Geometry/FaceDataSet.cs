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
using ProtoBuf;
using NthDimension.Algebra;

namespace NthDimension.Rendering.Geometry
{
    [ProtoContract]
    struct FaceDataSet
    {
        [ProtoMember(10)]
        public Vector3          position;
        [ProtoMember(20)]
        public Vector3          normal;
        [ProtoMember(30)]
        public Vector3          tangent;
        [ProtoMember(40)]
        public float[]          boneWeight;
        [ProtoMember(50)]
        public int[]            boneId;
        [ProtoMember(60)]
        public Vector2          texture;

        public bool Equals(FaceDataSet vert)
        {
            if (GenericMethods.estSize(this.position - vert.position) < 0.001f &&
                GenericMethods.estSize(this.texture - vert.texture) < 0.001f &&
                GenericMethods.estSize(this.normal - vert.normal) < 0.001f)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Position: " + position.ToString());
            sb.AppendLine("Normal: " + normal.ToString());
            sb.AppendLine("Texture: " + texture.ToString());
            return sb.ToString();
        }
    }
}
