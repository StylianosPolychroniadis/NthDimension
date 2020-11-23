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

namespace NthDimension.Rendering.Geometry
{

    using ProtoBuf;

    [ProtoBuf.ProtoContract(SkipConstructor = true), System.Serializable]
    public class Face
    {
        [ProtoMember(10)]
        public VertexIndex[] Vertex;
        [ProtoMember(20)]
        public bool isTemp;
        [ProtoMember(30)]
        public int position;

        public Face(VertexIndex ind1, VertexIndex ind2)
        {
            Vertex = new VertexIndex[2];
            Vertex[0] = ind1;
            Vertex[1] = ind2;
        }
        public Face(VertexIndex ind1, VertexIndex ind2, VertexIndex ind3)
        {
            Vertex = new VertexIndex[3];
            Vertex[0] = ind1;
            Vertex[1] = ind2;
            Vertex[2] = ind3;
        }

        public Face(VertexIndex ind1, VertexIndex ind2, VertexIndex ind3, VertexIndex ind4)
        {
            Vertex = new VertexIndex[4];
            Vertex[0] = ind1;
            Vertex[1] = ind2;
            Vertex[2] = ind3;
            Vertex[3] = ind4;
        }

        public Face(int vCount, int position)
        {
            Vertex = new VertexIndex[vCount];
            this.position = position;

            for (int i = 0; i < vCount; i++)
                Vertex[i] = new VertexIndex();

        }
    }
}
