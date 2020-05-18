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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using NthDimension.Algebra;
using NthDimension.Rendering.Animation;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Geometry.Simplification;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;


namespace NthDimension.Rendering.Loaders
{
    public partial class MeshLoader
    {

        
        #region Autodesk .obj
        public MeshVbo FromObj(string pointer)
        {
            string name = pointer;

            //try
            //{
            //    if (pointer.Contains(GameBase.Instance.path + "\\"))
            //        name = pointer.Replace(GameBase.Instance.path + "\\", "");
            //    else
            //        name = pointer;
            //}
            //catch (NullReferenceException)
            //{


            //}

            name = name.Replace(GameSettings.ModelFolder, "");

            Debug.Print(pointer + " " + name);

            if (!meshesNames.ContainsKey(name))
            {

                MeshVbo curMesh = new MeshVbo();
                curMesh.Type = MeshVbo.MeshType.Obj;
                curMesh.Pointer = pointer;
                curMesh.Identifier = meshes.Count;
                curMesh.Name = name;

                meshesNames.Add(curMesh.Name, curMesh.Identifier);

                meshes.Add(curMesh);

                return curMesh;
            }
            else
            {
                return GetMeshByName(name);
            }

        }
        private void loadObj(MeshVbo target)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            swfullObj.Start();

            string pointer  = target.Pointer;
            int identifier  = target.Identifier;
            string name     = string.Empty;

            if (!String.IsNullOrEmpty(target.Name))
                name = target.Name;

            Utilities.ConsoleUtil.logFileSize(string.Format("<>  Loading Waveform mesh: {0} \t\t\t", target.Pointer), GenericMethods.FileSizeReadable(target.Pointer));
            ListVector3     positionData    = new ListVector3 { };
            ListVector3     normalData      = new ListVector3 { };
            ListVector2     textureData     = new ListVector2 { };
            ListFace        faceData        = new ListFace { };
            //ListVertex FpIndiceList = new ListVertex { };

            #region Read File

            // Read the file and display it line by line.
            string line;

            if (!Directory.Exists(Path.GetDirectoryName(target.Pointer)) || !File.Exists(target.Pointer))
                // ONLY FOR DEBUG
            {
                ConsoleUtil.errorlog("Skipping File: ", string.Format(" {0} - file is missing", target.Pointer));
                return;
            }

            System.IO.StreamReader file = new System.IO.StreamReader(target.Pointer);

            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace("  ", " ").TrimEnd();

                #region Texture Coordinate

                if (line.StartsWith("vt"))
                {
                    string[] sline = line.Split(new string[] { " " }, 10, StringSplitOptions.None);

                    if (sline[0] == "vt")
                    {
                        float X = float.Parse(sline[1], nfi);
                        float Y = 1 - float.Parse(sline[2], nfi);
                        textureData.Add(new Vector2(X, Y));

                    }
                }

                #endregion

                #region Vertex Normal

                if (line.StartsWith("vn"))
                {
                    string[] sline = line.Split(new string[] { " " }, 10, StringSplitOptions.None);
                    if (sline[0] == "vn")
                    {
                        float X = float.Parse(sline[1], nfi);
                        float Y = float.Parse(sline[2], nfi);
                        float Z = float.Parse(sline[3], nfi);
                        normalData.Add(new Vector3(X, Y, Z));

                    }
                }

                #endregion

                #region Vertex Position

                if (line.StartsWith("v"))
                {
                    string[] sline = line.Split(new string[] { " " }, 10, StringSplitOptions.None);
                    if (sline[0] == "v")
                    {
                        float X = float.Parse(sline[1], nfi);
                        float Y = float.Parse(sline[2], nfi);
                        float Z = float.Parse(sline[3], nfi);
                        positionData.Add(new Vector3(X, Y, Z));
                    }
                }

                #endregion

                #region Face

                /*
                Here is an example of vertex only declaration:
                f v1 v2 v3 ...

                Numbers following the 'f' letter can also be separate with the slash sign ('/'). There is only a maximum of two slashes. 
                If only one '/' is used the first number on the left of the sign, indicates a vertex index and the second number on the right, 
                indicates an index in the texture coordinates array. Here is an example of vertex/texture coordinate face declaration:
                f v1/vt1 v2/vt2 v3/vt3 ...

                If two such signs are used to separate three numbers, the first number indicates a vertex index, the second number an index in 
                the normal array and the third number an array in the texture coordinates array. Note that vertex normals and texture coordinates 
                are not mandatory. They are only there for convenience if your modeling tool for instance supports vertex normal editing and that 
                you wish to export this information with your model. Texture coordinates (which can also generally be edited in a 3D modeling 
                application) are used for texturing. Here is an example of vertex/texture coordinate/normal face declaration:
                f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 ...

                Finally, when two numbers are separated by two '/', the first number on the left represent a vertex index and the second number 
                on the right indicates a normal index (an index in the normals array). Here is an examples of vertex/normal face declaration:
                f v1//vn1 v2//vn2 v3//vn3 ... 
    
                // Note for f: 
                //          f v1 v2 v3 (v4)                     // f index1 index2 index3 (index4)
                //          f v1/vt1 v2/vt2 v3/vt3              // f index1/texture1 index2/texture2 index3/texture3 (index4/texture4)
                //          f v1//vn1 v2//vn2 v3//vn3           // f index1//normal1 index2//normal2 index3//normal3 (index4//normal4)
                //          TODO:: Change/fix the code below to match correctly the obj file
                // 1) split each index by space
                // 2) count slashes -> none simply vertex index
                // a)                  one vertex index/texture index
                // b)                  two vertex index/normal index/texture index 
                */

                if (line.StartsWith("f"))
                {
                    if (!line.Contains("/"))
                    {
                        string[] sline = line.Split(new string[] { " " }, 10, StringSplitOptions.None);

                        #region Triangle

                        if (sline.Length == 4)
                        {

                            int v1 = int.Parse(sline[1]) - 1;
                            int v2 = int.Parse(sline[2]) - 1;
                            int v3 = int.Parse(sline[3]) - 1;

                            VertexIndices va = new VertexIndices(v1);
                            VertexIndices vb = new VertexIndices(v2);
                            VertexIndices vc = new VertexIndices(v3);

                            faceData.Add(new Face(va, vb, vc));
                        }

                        #endregion

                        #region Quad

                        if (sline.Length == 5)
                        {
                            int v1 = int.Parse(sline[1]) - 1;
                            int v2 = int.Parse(sline[2]) - 1;
                            int v3 = int.Parse(sline[3]) - 1;
                            int v4 = int.Parse(sline[4]) - 1;

                            VertexIndices va = new VertexIndices(v1);
                            VertexIndices vb = new VertexIndices(v2);
                            VertexIndices vc = new VertexIndices(v3);
                            VertexIndices vd = new VertexIndices(v4);

                            faceData.Add(new Face(new VertexIndices(va), new VertexIndices(vb), new VertexIndices(vc), new VertexIndices(vd)));
                        }

                        #endregion
                    }

                    if (line.Contains("/"))
                    {
                        string[] sline = line.Split(new string[] { " " }, 10, StringSplitOptions.None);

                        if (sline[0] == "f")
                        {
                            #region Triangle

                            if (sline.Length == 4)
                            {
                                VertexIndices va, vb, vc;

                                string[] segmentVa = sline[1].Split(new string[] { "/" }, 10, StringSplitOptions.None);
                                string[] segmentVb = sline[2].Split(new string[] { "/" }, 10, StringSplitOptions.None);
                                string[] segmentVc = sline[3].Split(new string[] { "/" }, 10, StringSplitOptions.None);

                                #region Vertex Index / Texture Index

                                if (segmentVa.Length == 2 && segmentVb.Length == 2 && segmentVc.Length == 2)
                                //  f vi/ti vi/ti vi/ti
                                {
                                    int va_vi, va_ti = 0;
                                    int vb_vi, vb_ti = 0;
                                    int vc_vi, vc_ti = 0;

                                    int.TryParse(segmentVa[0], out va_vi);
                                    int.TryParse(segmentVa[1], out va_ti);

                                    int.TryParse(segmentVb[0], out vb_vi);
                                    int.TryParse(segmentVb[1], out vb_ti);

                                    int.TryParse(segmentVc[0], out vc_vi);
                                    int.TryParse(segmentVc[1], out vc_ti);


                                    va = new VertexIndices(va_vi - 1, va_ti - 1, 0);
                                    vb = new VertexIndices(vb_vi - 1, vb_ti - 1, 0);
                                    vc = new VertexIndices(vc_vi - 1, vc_ti - 1, 0);

                                    faceData.Add(new Face(va, vb, vc));
                                }

                                #endregion

                                #region Vertex Index / Texture Index / Normal Index

                                if (segmentVa.Length == 3 && segmentVb.Length == 3 && segmentVc.Length == 3)
                                // f vi/ti/ni vi/ti/ni vi/ti/ni -or- f vi//ni vi//ni vi//ni
                                {
                                    int va_vi, va_ti, va_ni = 0;
                                    int vb_vi, vb_ti, vb_ni = 0;
                                    int vc_vi, vc_ti, vc_ni = 0;

                                    int.TryParse(segmentVa[0], out va_vi);
                                    int.TryParse(segmentVa[1], out va_ti);
                                    int.TryParse(segmentVa[2], out va_ni);

                                    int.TryParse(segmentVb[0], out vb_vi);
                                    int.TryParse(segmentVb[1], out vb_ti);
                                    int.TryParse(segmentVb[2], out vb_ni);

                                    int.TryParse(segmentVc[0], out vc_vi);
                                    int.TryParse(segmentVc[1], out vc_ti);
                                    int.TryParse(segmentVc[2], out vc_ni);


                                    va = new VertexIndices(va_vi - 1, va_ti - 1, va_ni - 1);
                                    vb = new VertexIndices(vb_vi - 1, vb_ti - 1, vb_ni - 1);
                                    vc = new VertexIndices(vc_vi - 1, vc_ti - 1, vc_ni - 1);

                                    faceData.Add(new Face(va, vb, vc));
                                }

                                #endregion

                            }

                            #endregion

                            #region Quad

                            if (sline.Length == 5)
                            {
                                VertexIndices va, vb, vc, vd;

                                string[] segmentVa = sline[1].Split(new string[] { "/" }, 10, StringSplitOptions.None);
                                string[] segmentVb = sline[2].Split(new string[] { "/" }, 10, StringSplitOptions.None);
                                string[] segmentVc = sline[3].Split(new string[] { "/" }, 10, StringSplitOptions.None);
                                string[] segmentVd = sline[4].Split(new string[] { "/" }, 10, StringSplitOptions.None);

                                #region Vertex Index / Texture Index

                                if (segmentVa.Length == 2 && segmentVb.Length == 2 && segmentVc.Length == 2 &&
                                    segmentVd.Length == 2) // f vi/ti vi/ti vi/ti vi/ti
                                {
                                    int va_vi, va_ti = 0;
                                    int vb_vi, vb_ti = 0;
                                    int vc_vi, vc_ti = 0;
                                    int vd_vi, vd_ti = 0;

                                    int.TryParse(segmentVa[0], out va_vi);
                                    int.TryParse(segmentVa[1], out va_ti);

                                    int.TryParse(segmentVb[0], out vb_vi);
                                    int.TryParse(segmentVb[1], out vb_ti);

                                    int.TryParse(segmentVc[0], out vc_vi);
                                    int.TryParse(segmentVc[1], out vc_ti);

                                    int.TryParse(segmentVd[0], out vd_vi);
                                    int.TryParse(segmentVd[1], out vd_ti);

                                    va = new VertexIndices(va_vi - 1, va_ti - 1, 0);
                                    vb = new VertexIndices(vb_vi - 1, vb_ti - 1, 0);
                                    vc = new VertexIndices(vc_vi - 1, vc_ti - 1, 0);
                                    vd = new VertexIndices(vd_vi - 1, vd_ti - 1, 0);

                                    faceData.Add(new Face(va, vb, vc, vd));
                                }

                                #endregion

                                #region Vertex Index / Texture Index / Normal Index

                                if (segmentVa.Length == 3 && segmentVb.Length == 3 && segmentVc.Length == 3 &&
                                    segmentVd.Length == 3)
                                // f vi/ti/ni vi/ti/ni vi/ti/ni vi/ti/ni -or- f vi//ni vi//ni vi//ni vi//ni 
                                {
                                    int va_vi, va_ti, va_ni = 0;
                                    int vb_vi, vb_ti, vb_ni = 0;
                                    int vc_vi, vc_ti, vc_ni = 0;
                                    int vd_vi, vd_ti, vd_ni = 0;

                                    int.TryParse(segmentVa[0], out va_vi);
                                    int.TryParse(segmentVa[1], out va_ti);
                                    int.TryParse(segmentVa[2], out va_ni);

                                    int.TryParse(segmentVb[0], out vb_vi);
                                    int.TryParse(segmentVb[1], out vb_ti);
                                    int.TryParse(segmentVb[2], out vb_ni);

                                    int.TryParse(segmentVc[0], out vc_vi);
                                    int.TryParse(segmentVc[1], out vc_ti);
                                    int.TryParse(segmentVc[2], out vc_ni);

                                    int.TryParse(segmentVd[0], out vd_vi);
                                    int.TryParse(segmentVd[1], out vd_ti);
                                    int.TryParse(segmentVd[2], out vd_ni);

                                    va = new VertexIndices(va_vi - 1, va_ti - 1, va_ni - 1);
                                    vb = new VertexIndices(vb_vi - 1, vb_ti - 1, vb_ni - 1);
                                    vc = new VertexIndices(vc_vi - 1, vc_ti - 1, vc_ni - 1);
                                    vd = new VertexIndices(vd_vi - 1, vd_ti - 1, vd_ni - 1);

                                    faceData.Add(new Face(va, vb, vc, vd));

                                }

                                #endregion
                            }

                            #endregion
                        }
                    }
                }

                #endregion
            }

            file.Close();

            #endregion

            Utilities.ConsoleUtil.log(string.Format("   " + ConsoleUtil.bulletWhite + "Read mesh file {2}: Done " + ConsoleUtil.tick + "\t\t\t\t| {0}",
                sw.ToHumanReadable(), Environment.NewLine, target.Name));

            //target                  = this.createMesh(positionData, normalData, textureData, faceData);
            #region TODO:: Replace by above call (hint: need to maintain ref to target)
            target.Identifier = meshes.Count;
            target.IsAnimated = false;

            Utilities.ConsoleUtil.log(string.Format("<>  Loading solid mesh: {0} polygon(s), please wait...", faceData.Count));

            if (null == target.MeshData.PositionCache)
                target.MeshData.PositionCache = new ListVector3();
            if (null == target.MeshData.NormalCache)
                target.MeshData.NormalCache = new ListVector3();
            if (null == target.MeshData.TextureCache)
                target.MeshData.TextureCache = new ListVector2();
            if (null == target.MeshData.Faces)
                target.MeshData.Faces = new ListFace();

            #region saveToTempCache
            target.MeshData.PositionCache.AddRange(positionData);
            target.MeshData.NormalCache.AddRange(normalData);
            target.MeshData.TextureCache.AddRange(textureData);
            target.MeshData.Faces.AddRange(faceData);
            #endregion

            ParseFaceList(ref target, true, MeshVbo.MeshLod.Level0);
            GenerateVBO(ref target, MeshVbo.MeshLod.Level0);
            Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level0));
            //                                      @"   -Datasets         ({1})... {0}"

            #region Lod Levels 1-3
            int l0F = 0, l1F = 0, l2F = 0, l3F = 0, l0V = 0, l1V = 0, l2V = 0, l3V = 0;

            l0F = target.MeshData.Faces.Count;
            l0V = target.MeshData.Positions.Length;
            sw.Restart();

            int level1 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedOne)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticOne);

            int level2 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedTwo)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticTwo);

            int level3 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedThree)
                                           : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticThree);


            if (level1 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level1;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level1, level1);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level1);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level1);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level1));
                //                                           Polygon construction
                l1F = target.MeshData.Faces.Count;
                l1V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }

            if (level2 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level2;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level2, level2);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level2);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level2);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level2));
                l2F = target.MeshData.Faces.Count;
                l2V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }

            if (level3 > 4 && !target.Name.Contains("_pbox.obj"))
            {
                //target.CurrentLod = Mesh.MeshLod.Level3;
                simplifyMesh(ref target, MeshVbo.MeshLod.Level3, level3);
                ParseFaceList(ref target, true, MeshVbo.MeshLod.Level3);
                GenerateVBO(ref target, MeshVbo.MeshLod.Level3);
                Utilities.ConsoleUtil.log(string.Format(@"   " + ConsoleUtil.bulletWhite + "Build polygon    ({2}): Done " + ConsoleUtil.tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, MeshVbo.MeshLod.Level3));
                l3F = target.MeshData.Faces.Count;
                l3V = target.MeshData.Positions.Length;
                sw.Restart();
            }
            else
            {
                target.CurrentLod = MeshVbo.MeshLod.Level0;
                target.MeshData.LodEnabled = false;
            }
            #endregion

            target.CurrentLod = MeshVbo.MeshLod.Level0;
            target.IsLoaded = true;



            #region Display Diagnostics

            string ttime = swfullObj.ToHumanReadable();

            //string ttime =  string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); 
            ConsoleColor cc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level0, l0F, l0V));
            if (level1 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level1, l1F, l1V));
            if (level2 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level2, l2F, l2V));
            if (level3 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", MeshVbo.MeshLod.Level3, l3F, l3V));
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Utilities.ConsoleUtil.log(string.Format("{0}   Waveform complete! Total time\t\t\t\t\t[{1}] {2}",
                Environment.NewLine, ttime, Environment.NewLine));
            Console.ForegroundColor = cc;
            #endregion

            target.Name             = name;
            target.Identifier       = identifier;
            target.Pointer          = pointer;
            target.IsAnimated       = false;
            target.CurrentLod       = MeshVbo.MeshLod.Level0;
            target.IsLoaded         = true;

            if (target.Identifier != -1)
                meshes[target.Identifier] = target;


            #endregion

            sw.Stop();
            swfullObj.Stop();
        }

        #endregion

       

    }
}
