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

namespace NthDimension.Rendering.Shaders
{
    using System;

    [Serializable, ProtoContract]
    public struct Snippet
    {
        [ProtoMember(10)]
        public string name;
        [ProtoMember(20)]
        public string text;
        [ProtoMember(30)]
        public string variables;
        [ProtoMember(40)]
        public string functions;

        public void cache(ref ShaderCacheObject cacheObject)
        {
            cacheObject.snippets.Add(this);
        }
    }
}
