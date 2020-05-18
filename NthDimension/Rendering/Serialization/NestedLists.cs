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

namespace NthDimension.Rendering.Serialization
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract(SkipConstructor = true)]
    public class ListFloat : List<float> { }

    [ProtoContract(SkipConstructor = true)]
    public class ListInt : List<int> { }

    [ProtoContract(SkipConstructor = true)]
    public class NestedFloat : List<ListFloat> { }

    [ProtoContract(SkipConstructor = true)]
    public class NestedInt : List<ListInt>
    {
     
    }

}
