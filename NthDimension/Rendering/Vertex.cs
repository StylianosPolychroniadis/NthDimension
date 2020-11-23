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

using ProtoBuf;

namespace NthDimension
{
    [ProtoContract, System.Serializable]
    /// <summary>
    /// This is the Fast, GPU memory Vertex. 
    /// All Vertex classes should inherit this and 
    /// output on the current Vertex properties (indexing integers)
    /// </summary>
    public class VertexIndex
    {
        [ProtoMember(10)]
        public int Vi = 0;
        [ProtoMember(20)]
        public int Ti = 0;
        [ProtoMember(30)]
        public int Ni = 0;
        [ProtoMember(40)]
        public int Normalihelper = -1;

        public VertexIndex(int vi)
        {
            Vi = vi;
        }

        public VertexIndex(int vi, int ti, int ni)
        {
            Vi = vi;
            Ti = ti;
            Ni = ni;
        }

        public VertexIndex(VertexIndex v)
        {
            Vi = v.Vi;
            Ti = v.Ti;
            Ni = v.Ni;
        }

        public VertexIndex()
        {
        }
    }
}
