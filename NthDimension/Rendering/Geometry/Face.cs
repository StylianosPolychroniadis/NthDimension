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
        public VertexIndices[] Vertex;
        [ProtoMember(20)]
        public bool isTemp;
        [ProtoMember(30)]
        public int position;


        public Face(VertexIndices ind1, VertexIndices ind2, VertexIndices ind3)
        {
            Vertex = new VertexIndices[3];
            Vertex[0] = ind1;
            Vertex[1] = ind2;
            Vertex[2] = ind3;
            // Log.e("VboCube",Vi+"/"+Ti+"/"+Ni);
        }

        public Face(VertexIndices ind1, VertexIndices ind2, VertexIndices ind3, VertexIndices ind4)
        {
            Vertex = new VertexIndices[4];
            Vertex[0] = ind1;
            Vertex[1] = ind2;
            Vertex[2] = ind3;
            Vertex[3] = ind4;
            // Log.e("VboCube",Vi+"/"+Ti+"/"+Ni);
        }

        public Face(int vCount, int position)
        {
            Vertex = new VertexIndices[vCount];
            this.position = position;

            for (int i = 0; i < vCount; i++)
                Vertex[i] = new VertexIndices();

        }
    }
}
