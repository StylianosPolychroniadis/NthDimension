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


namespace NthDimension.Rendering
{
    using NthDimension.Rendering.Serialization;
    [Serializable, ProtoContract]
    public struct Template
    {
        [ProtoContract]
        public enum Type
        {
            [ProtoMember(1)]
            fromXml,
            [ProtoMember(2)]
            fromCache
        };

        [ProtoMember(10)]
        public int identifier;

        [ProtoMember(20)]
        public ListString meshes;
        [ProtoMember(30)]
        public ListString pmeshes;
        [ProtoMember(40)]
        public ListString materials;

        [ProtoMember(50)]
        public string pointer;
        [ProtoMember(60)]
        public string name;

        [ProtoMember(70)]
        public bool isStatic;
        [ProtoMember(80)]
        public bool loaded;


        [ProtoMember(90)]
        public float positionOffset;
        [ProtoMember(100)]
        public int filePosition;
        [ProtoMember(110)]
        public bool hasLight;
        [ProtoMember(120)]
        public Vector3 lightColor;
        [ProtoMember(130)]
        public bool normal;

        [ProtoContract]
        public enum UseType
        {
            [ProtoMember(131)]
            Model,
            [ProtoMember(132)]
            Animated,
            [ProtoMember(133)]
            Meta
        }

        [ProtoMember(140)]
        public float volumeRadius;
        [ProtoMember(150)]
        public UseType useType;

        [ProtoMember(160)]
        public Type type;

        internal void cache(ref List<Template> SaveList)
        {
            SaveList.Add(this);
        }
    }
}
