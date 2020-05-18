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
using NthDimension.Rendering.Serialization;

namespace NthDimension.Rendering.Shaders
{
    using System;
    using System.Collections.Generic;

    [Serializable, ProtoContract]
    public struct ShaderCacheObject
    {
        [ProtoMember(10)]
        public ListShader shaders;
        [ProtoMember(20)]
        public ListSnippet snippets;
    }
}
