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

namespace NthDimension.Rendering.Loaders
{
    using System.Diagnostics;
    using System.Xml;

    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Utilities;

    public partial class MeshLoader
    {
        #region Proprietary .xmd 
        public MeshVbo FromXmd(string pointer)
        {
            #region Fix paths
            string name = pointer;//string.Empty;

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
            #endregion

            if (!meshesNames.ContainsKey(name))
            {
                MeshVbo curMesh = new MeshVbo();
               
                XmlTextReader reader = new XmlTextReader(pointer);
                while (reader.Read())
                {
                    if (reader.Name             == "model")
                    {
                        while (reader.Read())
                        {
                            #region Geometry

                            if (reader.Name         == "mesh")
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name     == "source")
                                    {
                                        curMesh.Pointer = GameSettings.ModelCharacterFolder + reader.Value;

                                        
                                        loadColladaGeometry(ref curMesh);
                                        ParseFaceList(ref curMesh, false, MeshVbo.MeshLod.Level0, false);

                                    }
                                }
                            #endregion

                            // TODO:: Should be read from seperate .animation file

                            //#region Animation
                            //if (reader.Name == "animation")
                            //{
                            //    string anisource = string.Empty;
                            //    float frames = 0f;

                            //    string aniname = string.Empty;
                            //    string anitransition = string.Empty;
                            //    string aniplayback = string.Empty;
                            //    float anispeed = 0f;

                            //    curMesh.IsAnimated = true;
                            //        while (reader.MoveToNextAttribute())
                            //        {
                            //            if (reader.Name == "source")
                            //                anisource = GameSettings.ModelCharacterFolder + reader.Value;
                            //            else if (reader.Name == "frames")
                            //                frames = GenericMethods.FloatFromString(reader.Value);
                            //            else if (reader.Name == "name")
                            //                aniname = reader.Value;
                            //            else if (reader.Name == "playback")
                            //                aniplayback = reader.Value;
                            //            else if (reader.Name == "speed")
                            //                anispeed = GenericMethods.FloatFromString(reader.Value);
                            //            else if (reader.Name == "transition")
                            //                anitransition = reader.Value;
                            //        }
                                
                            //    loadManagedCollada(ref curMesh, anisource, aniname, frames, anispeed, aniplayback, anitransition);
                            //}
                            //#endregion
                        }
                    }


                }
                reader.Close();

//// Disabled Nov-25-2019 loading dae from xsn efforts
GenerateVBO(ref curMesh, MeshVbo.MeshLod.Level0);

//if (curMesh.IsAnimated)
//{
//    //curMesh.SetAnimation(curMesh.AnimationData[0].identifier);
//    //// Switch to the first animation
//    //curMesh.AnimationTotalFrames = curMesh.AnimationData[0].lastFrame;
//}

                

// curMesh.IsLoaded = true;
                curMesh.Type = MeshVbo.MeshType.ColladaGeometry;
                curMesh.Pointer = pointer;// pointer.Replace(GameSettings.ModelFolder, "");
                curMesh.Identifier = meshes.Count;
                curMesh.Name = name;

                meshesNames.Add(curMesh.Name, curMesh.Identifier);
                meshes.Add(curMesh);

curMesh.CurrentLod = MeshVbo.MeshLod.Level0;
curMesh.IsLoaded = true;

if (curMesh.Identifier != -1)
    meshes[curMesh.Identifier] = curMesh;                

                return curMesh;
            }
            else
            {
                return GetMeshByName(name);
            }
        }

        #endregion

        #region Proprietary .animation

        public void FromAnimation(string pointer)
        {
            #region Fix paths
            string name = pointer;//string.Empty;

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
            #endregion

            XmlTextReader reader = new XmlTextReader(pointer);
            while (reader.Read())
            {
                if (reader.Name == "sequences")
                {
                    while (reader.Read())
                    {
                        #region Animation
                        if (reader.Name == "animation")
                        {
                            string anisource = string.Empty;
                            float frames = 0f;

                            string aniname = string.Empty;
                            string anitransition = string.Empty;
                            string aniplayback = string.Empty;
                            float anispeed = 0f;

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "source")
                                    anisource = GameSettings.ModelCharacterFolder + reader.Value;
                                else if (reader.Name == "frames")
                                    frames = GenericMethods.FloatFromString(reader.Value);
                                else if (reader.Name == "name")
                                    aniname = reader.Value;
                                else if (reader.Name == "playback")
                                    aniplayback = reader.Value;
                                else if (reader.Name == "speed")
                                    anispeed = GenericMethods.FloatFromString(reader.Value);
                                else if (reader.Name == "transition")
                                    anitransition = reader.Value;
                            }

                            if (!AnimationLoader.animationNames.ContainsKey(anisource))
                                loadColladaAnimation(anisource, aniname, frames, anispeed, aniplayback, anitransition);
                        }
                        #endregion
                    }
                }
            }

        }
        #endregion

        #region Collada .dae [Collada .dae files are only considered to be Characters]
        public MeshVbo FromCollada(string pointer)
        {
            string name = pointer; //string.Empty;

            //try
            //{
            //    if (pointer.Contains(GameBase.Instance.path + "\\"))
            //        name = pointer.Replace(GameBase.Instance.path + "\\", "");
            //    else
            //        name = pointer;
            //}
            //catch (Exception e)
            //{
            //    ConsoleUtil.errorlog("MeshLoader.FromCollada() ", e.Message);
            //}

            name = name.Replace(GameSettings.ModelFolder, "");

            Debug.Print(pointer + " " + name);

            if (!meshesNames.ContainsKey(name))
            {
                MeshVbo curMesh = new MeshVbo();

                curMesh.Type = MeshVbo.MeshType.ColladaManaged;
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
        private void loadColladaGeometry(ref MeshVbo target)
        {
           
            Utilities.ConsoleUtil.logFileSize(string.Format("<> Loading Collada Geometry: {0} ", target.Pointer), GenericMethods.FileSizeReadable(target.Pointer));
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (target.CurrentLod != MeshVbo.MeshLod.Level0)
                target.CurrentLod = MeshVbo.MeshLod.Level0;

            ColladaScene colladaScene = new ColladaScene(target.Pointer);
            colladaScene.saveToTempGeometry(ref target);

            target.MeshData.LodEnabled = true;
            target.IsLoaded = true;

            target.CurrentLod = MeshVbo.MeshLod.Level1;
            target.MeshData.LodEnabled = false;
            target.CurrentLod = MeshVbo.MeshLod.Level2;
            target.MeshData.LodEnabled = false;
            target.CurrentLod = MeshVbo.MeshLod.Level3;
            target.MeshData.LodEnabled = false;

            //if (target.Type != Mesh.MeshType.Empty)
            //{
            //    #region Build Lod Level 0
            //    ParseFaceList(ref target, false, Mesh.MeshLod.Level0);
            //    GenerateVBO(ref target, Mesh.MeshLod.Level0);
            //    #endregion

            //    //target.MeshData.SaveToObj(string.Format(@"D:\{0}-lod0.obj", target.Name));


            //    #region Calculate Lod Factors
            //    int l0F = 0, l1F = 0, l2F = 0, l3F = 0, l0V = 0, l1V = 0, l2V = 0, l3V = 0;

            //    l0F = target.MeshData.Faces.Count;
            //    l0V = target.MeshData.Positions.Length;
            //    sw.Restart();

            //    int level1 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedOne)
            //                                   : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticOne);

            //    int level2 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedTwo)
            //                                   : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticTwo);

            //    int level3 = target.IsAnimated ? (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodAnimatedThree)
            //                                   : (int)(target.MeshData.Faces.Count * Settings.Instance.mesh.LodStaticThree);
            //    #endregion


            //    #region Build Lod Level 1
            //    if (level1 > 4 && !target.Name.Contains("_pbox.obj"))
            //    {
            //        try
            //        {
            //            //target.CurrentLod = Mesh.MeshLod.Level1;
            //            simplifyMesh(ref target, Mesh.MeshLod.Level1, level1);
            //            ParseFaceList(ref target, true, Mesh.MeshLod.Level1);
            //            GenerateVBO(ref target, Mesh.MeshLod.Level1);
            //            Utilities.ConsoleUtil.log(string.Format(@"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level1));

            //            l1F = target.MeshData.Faces.Count;
            //            l1V = target.MeshData.Positions.Length;
            //            sw.Restart();
            //        }
            //        catch (Exception pE)
            //        {
            //            ConsoleUtil.errorlog(string.Format("ERROR ParseGeometry {0}{1}", target.Name, Environment.NewLine), string.Format("{0}{1}{2}", pE.Message, Environment.NewLine, pE.StackTrace));
            //        }
            //        finally
            //        {
            //            target.CurrentLod = Mesh.MeshLod.Level0;
            //            target.MeshData.LodEnabled = false;
            //        }
            //    }
            //    else
            //    {
            //        sw.Restart();
            //        target.CurrentLod = Mesh.MeshLod.Level0;
            //        target.MeshData.LodEnabled = false;
            //    }
            //    #endregion

            //    //target.MeshData.SaveToObj(string.Format(@"D:\{0}-lod1.obj", target.Name));

            //    #region Build Lod Level 2
            //    if (level2 > 4 && !target.Name.Contains("_pbox.obj"))
            //    {
            //        try
            //        {
            //            //target.CurrentLod = Mesh.MeshLod.Level2;
            //            simplifyMesh(ref target, Mesh.MeshLod.Level2, level2);
            //            ParseFaceList(ref target, true, Mesh.MeshLod.Level2);
            //            GenerateVBO(ref target, Mesh.MeshLod.Level2);
            //            Utilities.ConsoleUtil.log(string.Format(@"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level2));
            //            l2F = target.MeshData.Faces.Count;
            //            l2V = target.MeshData.Positions.Length;
            //            sw.Restart();
            //        }
            //        catch (Exception pE)
            //        {
            //            ConsoleUtil.errorlog(string.Format("ERROR ParseGeometry {0}{1}", target.Name, Environment.NewLine), string.Format("{0}{1}{2}", pE.Message, Environment.NewLine, pE.StackTrace));
            //        }
            //        finally
            //        {
            //            sw.Restart();
            //            target.CurrentLod = Mesh.MeshLod.Level0;
            //            target.MeshData.LodEnabled = false;
            //        }
            //    }
            //    else
            //    {
            //        target.CurrentLod = Mesh.MeshLod.Level0;
            //        target.MeshData.LodEnabled = false;
            //    }
            //    #endregion

            //    //target.MeshData.SaveToObj(string.Format(@"D:\{0}-lod2.obj", target.Name));

            //    #region Build Lod Level 3
            //    if (level3 > 4 && !target.Name.Contains("_pbox.obj"))
            //    {
            //        try
            //        {
            //            //target.CurrentLod = Mesh.MeshLod.Level3;
            //            simplifyMesh(ref target, Mesh.MeshLod.Level3, level3);
            //            ParseFaceList(ref target, true, Mesh.MeshLod.Level3);
            //            GenerateVBO(ref target, Mesh.MeshLod.Level3);
            //            Utilities.ConsoleUtil.log(string.Format(@"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}", sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level3));
            //            l3F = target.MeshData.Faces.Count;
            //            l3V = target.MeshData.Positions.Length;
            //            sw.Restart();
            //        }
            //        catch (Exception pE)
            //        {
            //            ConsoleUtil.errorlog(string.Format("ERROR ParseGeometry {0}{1}", target.Name, Environment.NewLine), string.Format("{0}{1}{2}", pE.Message, Environment.NewLine, pE.StackTrace));
            //        }
            //        finally
            //        {
            //            sw.Restart();
            //            target.CurrentLod = Mesh.MeshLod.Level0;
            //            target.MeshData.LodEnabled = false;
            //        }
            //    }
            //    else
            //    {
            //        target.CurrentLod = Mesh.MeshLod.Level0;
            //        target.MeshData.LodEnabled = false;
            //    }
            //    #endregion

            //    //target.MeshData.SaveToObj(string.Format(@"D:\{0}-lod3.obj", target.Name));

            //    #region Display Diagnostics

            //    string ttime = swfullObj.ToHumanReadable();

            //    //string ttime =  string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); 
            //    ConsoleColor cc = Console.ForegroundColor;
            //    Console.ForegroundColor = ConsoleColor.DarkBlue;
            //    Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", Mesh.MeshLod.Level0, l0F, l0V));
            //    if (level1 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", Mesh.MeshLod.Level1, l1F, l1V));
            //    if (level2 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", Mesh.MeshLod.Level2, l2F, l2V));
            //    if (level3 > 4) Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", Mesh.MeshLod.Level3, l3F, l3V));
            //    Console.ForegroundColor = ConsoleColor.DarkGreen;
            //    Utilities.ConsoleUtil.log(string.Format("{0}   Collada complete! Total time\t\t\t\t\t[{1}] {2}",
            //        Environment.NewLine, ttime, Environment.NewLine));
            //    Console.ForegroundColor = cc;
            //    #endregion
            //}



            target.CurrentLod = MeshVbo.MeshLod.Level0;
            target.IsLoaded = true;

//// Disabled Nov-25-19 -> YES -> target.Identifier was 0 so it was replacing the Quad2D object
//if (target.Identifier != -1)
//    meshes[target.Identifier] = target;



        }
        private void loadColladaAnimation(/*ref Mesh target,*/ string daeFile, string animationName, float frames, float speed = 1.0f, string playback = "loop", string transition = "")
        {
            Utilities.ConsoleUtil.logFileSize(string.Format("<> Loading Collada Animation: {0} ", daeFile), GenericMethods.FileSizeReadable(daeFile));

            Stopwatch sw = new Stopwatch();
            sw.Start();


            ColladaScene colladaScene = new ColladaScene(daeFile);  // NOTE -WARNING- DAE is ONLY considered to be Characters and resides in models\characters\ folder
            colladaScene.stepSize = 1.0f / frames;

            colladaScene.appendAnimations(daeFile);


            foreach (var animation in colladaScene.animationData)    // Should ALWAYS be only one
            {
                animation.pointer = daeFile;
                animation.name = animationName;
                animation.AnimationSpeed = speed;
                animation.Playback = playback;
                animation.Transition = transition;

                animation.identifier = AnimationLoader.Animations.Count;

                AnimationLoader.Animations.Add(animation);
                AnimationLoader.animationNames.Add(animation.pointer, animation.identifier);

                ConsoleUtil.log(string.Format("   Added animation Id {0} file {1} with: Name {2}, Speed {3}, Playback {4}, {5} frames {6}",
                                animation.identifier,
                                animation.pointer,
                                animation.name,
                                animation.AnimationSpeed,
                                animation.Playback,
                                animation.Transition == string.Empty ? string.Empty : string.Concat("Transition ", animation.Transition),
                                animation.Matrices.Length));
            }
          
        }

        //private void loadManagedCollada_old(/*ref Mesh target,*/ string daeFile, string animationName, float frames, float speed = 1.0f, string playback = "loop", string transition = "")
        //{
        //    Utilities.ConsoleUtil.logFileSize(string.Format("<> Loading Managed Collada: {0} ", daeFile), GenericMethods.FileSizeReadable(daeFile));

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    target.CurrentLod = Mesh.MeshLod.Level0;

        //    ColladaScene colladaScene   = new ColladaScene(daeFile);  // NOTE -WARNING- DAE is ONLY considered to be Characters and resides in models\characters\ folder
        //    colladaScene.stepSize       = 1.0f / frames;

        //    colladaScene.appendAnimations(daeFile);

        //    if(null == target.AnimationData)
        //        target.AnimationData = new ListAnimationData();

        //    foreach (var animation in colladaScene.animationData)    // Should ALWAYS be only one
        //    {
        //        animation.pointer = daeFile;
        //        animation.name = animationName;
        //        animation.AnimationSpeed = speed;
        //        animation.Playback = playback;
        //        animation.Transition = transition;
        //        animation.identifier = target.AnimationData.Count;
        //        target.AnimationData.Add(animation);
        //        ConsoleUtil.log(string.Format("   Added animation Id {0} file {1} with: Name {2}, Speed {3}, Playback {4}, Transition {5}", 
        //                        animation.identifier,
        //                        animation.pointer,
        //                        animation.name,
        //                        animation.AnimationSpeed,
        //                        animation.Playback,
        //                        animation.Transition));
        //    }

        //    colladaScene.saveToTempAnimation(ref target);
        //    //colladaScene.saveToTempGeometry(ref target);

        //    //target.IsLoaded = true;
        //    //target.IsAnimated = true;
        //    ////if (target.Type != Mesh.MeshType.Empty)
        //    ////{
        //    ////    #region Build Lod Level 0

        //    ////    if (!meshes.Contains(target))
        //    ////    {
        //    ////        ParseFaceList(ref target, false, Mesh.MeshLod.Level0, false);


        //    ////        GenerateVBO(ref target, Mesh.MeshLod.Level0);

        //    ////        #endregion

        //    ////        #region Calculate Lod Factors

        //    ////        int l0F = 0, l1F = 0, l2F = 0, l3F = 0, l0V = 0, l1V = 0, l2V = 0, l3V = 0;

        //    ////        l0F = target.MeshData.Faces.Count;
        //    ////        l0V = target.MeshData.Positions.Length;
        //    ////        sw.Restart();

        //    ////        int level1 = target.IsAnimated
        //    ////            ? (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodAnimatedOne)
        //    ////            : (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodStaticOne);

        //    ////        int level2 = target.IsAnimated
        //    ////            ? (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodAnimatedTwo)
        //    ////            : (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodStaticTwo);

        //    ////        int level3 = target.IsAnimated
        //    ////            ? (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodAnimatedThree)
        //    ////            : (int) (target.MeshData.Faces.Count*Settings.Instance.mesh.LodStaticThree);

        //    ////        #endregion

        //    ////        #region Build Lod Level 1

        //    ////        if (level1 > 4 && !target.Name.Contains("_pbox.obj"))
        //    ////        {
        //    ////            //target.CurrentLod = Mesh.MeshLod.Level1;
        //    ////            simplifyMesh(ref target, Mesh.MeshLod.Level1, level1);
        //    ////            ParseFaceList(ref target, true, Mesh.MeshLod.Level1);
        //    ////            GenerateVBO(ref target, Mesh.MeshLod.Level1);
        //    ////            Utilities.ConsoleUtil.log(
        //    ////                string.Format(
        //    ////                    @"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}",
        //    ////                    sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level1));

        //    ////            l1F = target.MeshData.Faces.Count;
        //    ////            l1V = target.MeshData.Positions.Length;
        //    ////            sw.Restart();
        //    ////        }
        //    ////        else
        //    ////        {
        //    ////            target.CurrentLod = Mesh.MeshLod.Level0;
        //    ////            target.MeshData.LodEnabled = false;
        //    ////        }

        //    ////        #endregion

        //    ////        #region Build Lod Level 2

        //    ////        if (level2 > 4 && !target.Name.Contains("_pbox.obj"))
        //    ////        {
        //    ////            //target.CurrentLod = Mesh.MeshLod.Level2;
        //    ////            simplifyMesh(ref target, Mesh.MeshLod.Level2, level2);
        //    ////            ParseFaceList(ref target, true, Mesh.MeshLod.Level2);
        //    ////            GenerateVBO(ref target, Mesh.MeshLod.Level2);
        //    ////            Utilities.ConsoleUtil.log(
        //    ////                string.Format(
        //    ////                    @"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}",
        //    ////                    sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level2));
        //    ////            l2F = target.MeshData.Faces.Count;
        //    ////            l2V = target.MeshData.Positions.Length;
        //    ////            sw.Restart();
        //    ////        }
        //    ////        else
        //    ////        {
        //    ////            target.CurrentLod = Mesh.MeshLod.Level0;
        //    ////            target.MeshData.LodEnabled = false;
        //    ////        }

        //    ////        #endregion

        //    ////        #region Build Lod Level 3

        //    ////        if (level3 > 4 && !target.Name.Contains("_pbox.obj"))
        //    ////        {
        //    ////            //target.CurrentLod = Mesh.MeshLod.Level3;
        //    ////            simplifyMesh(ref target, Mesh.MeshLod.Level3, level3);
        //    ////            ParseFaceList(ref target, true, Mesh.MeshLod.Level3);
        //    ////            GenerateVBO(ref target, Mesh.MeshLod.Level3);
        //    ////            Utilities.ConsoleUtil.log(
        //    ////                string.Format(
        //    ////                    @"   " + bullet + "Build polygon    ({2}): Done " + tick + "\t\t\t\t\t| {0} {1}",
        //    ////                    sw.ToHumanReadable(), Environment.NewLine, Mesh.MeshLod.Level3));
        //    ////            l3F = target.MeshData.Faces.Count;
        //    ////            l3V = target.MeshData.Positions.Length;
        //    ////            sw.Restart();
        //    ////        }
        //    ////        else
        //    ////        {
        //    ////            target.CurrentLod = Mesh.MeshLod.Level0;
        //    ////            target.MeshData.LodEnabled = false;
        //    ////        }

        //    ////        #endregion

        //    ////        #region Display Diagnostics

        //    ////        string ttime = swfullObj.ToHumanReadable();

        //    ////        //string ttime =  string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); 
        //    ////        ConsoleColor cc = Console.ForegroundColor;
        //    ////        Console.ForegroundColor = ConsoleColor.DarkBlue;
        //    ////        Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices", Mesh.MeshLod.Level0,
        //    ////            l0F, l0V));
        //    ////        if (level1 > 4)
        //    ////            Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices",
        //    ////                Mesh.MeshLod.Level1, l1F, l1V));
        //    ////        if (level2 > 4)
        //    ////            Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices",
        //    ////                Mesh.MeshLod.Level2, l2F, l2V));
        //    ////        if (level3 > 4)
        //    ////            Utilities.ConsoleUtil.log(string.Format("   {0} * {1} Polygons, {2} Vertices",
        //    ////                Mesh.MeshLod.Level3, l3F, l3V));
        //    ////        Console.ForegroundColor = ConsoleColor.DarkGreen;
        //    ////        Utilities.ConsoleUtil.log(string.Format("{0}   Collada complete! Total time\t\t\t\t\t[{1}] {2}",
        //    ////            Environment.NewLine, ttime, Environment.NewLine));
        //    ////        Console.ForegroundColor = cc;

        //    ////        #endregion

        //    ////        target.CurrentLod = Mesh.MeshLod.Level0;
        //    ////        target.IsLoaded = true;

        //    ////        if (target.Identifier != -1)
        //    ////            meshes[target.Identifier] = target;
        //    ////    }
        //    ////    else
        //    ////    {
        //    ////        ConsoleUtil.log(string.Format("   Using mesh {0} for managed collada {1}", target.Name, target.Pointer));
        //    ////        target = GetMeshByName(target.Name);
        //    ////    }


        //    ////}

        //    //////target.CurrentLod = Mesh.MeshLod.Level0;
        //    //////target.IsLoaded = true;

        //    //////if (target.Identifier != -1)
        //    //////    meshes[target.Identifier] = target;
        //}
        #endregion
    }
}
