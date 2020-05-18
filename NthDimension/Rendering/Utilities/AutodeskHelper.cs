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
using System.Globalization;
using System.IO;
using NthDimension.Rendering.Geometry;

namespace NthDimension.Rendering.Utilities
{
    public static class AutodeskHelper
    {
        public static void SaveToObj(this MeshVboData MeshData, string path)
        {
            if (null == MeshData)
                return;


            using (var fs = File.Open(path, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine("# {0}", DateTime.Now);
                    sw.WriteLine("# {0} Vertices", MeshData.Positions.Length);
                    foreach (var v in MeshData.Positions)
                    {
                        sw.WriteLine("v {0} {1} {2}",
                            v.X.ToString(CultureInfo.InvariantCulture),
                            v.Y.ToString(CultureInfo.InvariantCulture),
                            v.Z.ToString(CultureInfo.InvariantCulture));
                    }
                    sw.WriteLine();
                    sw.WriteLine("# {0} Normals", MeshData.Faces.Count);
                    foreach (var n in MeshData.Normals)
                    {
                        sw.WriteLine("vn {0} {1} {2}",
                            n.X.ToString(CultureInfo.InvariantCulture),
                            n.Y.ToString(CultureInfo.InvariantCulture),
                            n.Z.ToString(CultureInfo.InvariantCulture));
                    }
                    sw.WriteLine();
                    sw.WriteLine("# {0} Faces", MeshData.Faces.Count);
                    foreach (var f in MeshData.Faces)
                    {
                        sw.WriteLine("f {0} {1} {2}", f.Vertex[0].Vi + 1, f.Vertex[1].Vi + 1, f.Vertex[2].Vi + 1);
                    }

                    // TODO: Add Vertex Texture Indices/Normal Indices

                    //if (mesh.Splits.Count > 0)
                    //{
                    //    sw.WriteLine();
                    //    sw.WriteLine("# {0} Split Records", mesh.Splits.Count);
                    //    foreach (var s in mesh.Splits)
                    //    {
                    //        // vsplit als Kommentar schreiben, so daß die Datei auch weiterhin
                    //        // eine gültige .obj Datei bleibt.
                    //        sw.Write("#vsplit {0} {{{1} {2} {3}}} {{{4} {5} {6}}} {{ ", s.S + 1,
                    //            s.SPosition.X.ToString(CultureInfo.InvariantCulture),
                    //            s.SPosition.Y.ToString(CultureInfo.InvariantCulture),
                    //            s.SPosition.Z.ToString(CultureInfo.InvariantCulture),
                    //            s.TPosition.X.ToString(CultureInfo.InvariantCulture),
                    //            s.TPosition.Y.ToString(CultureInfo.InvariantCulture),
                    //            s.TPosition.Z.ToString(CultureInfo.InvariantCulture));
                    //        foreach (var f in s.Faces)
                    //        {
                    //            sw.Write("({0} {1} {2}) ", f.Indices[0] + 1, f.Indices[1] + 1,
                    //                f.Indices[2] + 1);
                    //        }
                    //        sw.Write("}");
                    //        sw.WriteLine();
                    //    }
                    //}
                }
            }
        }
    }
}
