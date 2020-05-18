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

#define DEBUG_VERBOSE

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Rendering.Animation;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Loaders
{
    public class AnimationLoader
    {
        public static ListAnimationData Animations      = new ListAnimationData();
        public static Hashtable         animationNames  = new Hashtable();

        private static bool baseAnimationsLoaded = false;

        public AnimationLoader()
        {
            
        }

        public static void ReadCacheFile(Action<int> callback = null)
        {
            if (baseAnimationsLoaded) return;

            string animationCache = Settings.Instance.game.modelAnimationCacheFile;

            if (!File.Exists(animationCache))
                ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(animationCache)));
            else
            {
                using (FileStream fileStream = new FileStream(animationCache, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        // Read the source file into a byte array.
                        byte[] bytes = new byte[fileStream.Length];
                        int numBytesToRead = (int)fileStream.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            // Read may return anything from 0 to numBytesToRead.
                            int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);

                            // Break when the end of the file is reached.
                            if (n == 0)
                                break;

                            numBytesRead += n;
                            numBytesToRead -= n;

                            if (null != callback)
                                callback((int)(100 * numBytesRead / fileStream.Length));
                        }

                        ListAnimationData animationData = (ListAnimationData)GenericMethods.ByteArrayToObject<ListAnimationData>(bytes);
                        AnimationLoader.Animations.AddRange(animationData);

                        foreach (var ani in AnimationLoader.Animations)
                            AnimationLoader.animationNames.Add(ani.pointer, ani.identifier);

                        ConsoleUtil.log(string.Format("\tAdded {0} animations from {1}", animationData.Count, animationCache));
                    }
                    catch (Exception aE)
                    {
                        ConsoleUtil.errorlog("Animation Loader Error: ", aE.Message);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
        }

        public static void WriteCacheFile()
        {
            string animationCache = Settings.Instance.game.modelAnimationCacheFile;

            // TODO Split cacheAnimations.ucf to cacheMaleAni.ucf and cacheFemaleAni.ucf

            using (FileStream fileStream = new FileStream(animationCache, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    byte[] saveAry = GenericMethods.ObjectToByteArray<ListAnimationData>(Animations);
                    fileStream.Write(saveAry, 0, saveAry.Length);
                    ConsoleUtil.log(string.Format("   Cache file {0} generated with {1} animations, filesize: {2}",
                        animationCache, Animations.Count, GenericMethods.FileSizeReadable(animationCache)));
                }
                catch (Exception aE)
                {
                    ConsoleUtil.errorlog("Cache Write ", "Failed to store animations " + aE.Message);
                }
                finally
                {
                    fileStream.Close();
                }
            }
        }

        public void FromAni(string pointer)
        {

            if (!File.Exists(pointer)) return; // Added Jul-06-18 Double Asset Files Locations

            string name = pointer;

            //name = name.Replace(GameSettings.ModelFolder, "");
            



            try
            {
                if (pointer.Contains(ApplicationBase.Instance.VAR_AppPath + "\\"))
                    name = pointer.Replace(ApplicationBase.Instance.VAR_AppPath + "\\", "");
                else
                    name = pointer;
            }
            catch (Exception aE)
            {
                ConsoleUtil.errorlog(string.Format("Error parsing {0}", pointer), aE.Message);
            }

            name = name.Replace(GameSettings.ModelFolder, "");

            ApplicationBase.Instance.MeshLoader.FromAnimation(pointer);
        }

        public AnimationData FromName(string name)
        {
            AnimationData ret = new AnimationData();
            try
            {
                ret = Animations.First(a => a.name == name);
            }
            catch (Exception aE)
            {
                ConsoleUtil.errorlog("Animation Loader.FromName: ", string.Format("{0} {1}", name, aE.Message));
            }

            return ret;
        }
    }

}
