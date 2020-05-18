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

namespace NthDimension
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using NthDimension.Rendering.Configuration;
    

    public class Settings
    {
        public static Settings Instance = new Settings();

        public VideoSettings video = new VideoSettings();
        public SoundSettings sound = new SoundSettings();
        public GameSettings game = new GameSettings();
        public MeshSettings mesh = new MeshSettings();
        public ViewSettings view = new ViewSettings();

        public void SaveSettings(string path)
        {
            try
            {
                FileStream fs = File.Create(path);
                SaveSettings(fs);
                fs.Close();
            }
            catch (System.Exception ex)
            {
                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("(!) Error occured while saving settings: {0}", ex.Message));
            }
        }

        public void SaveSettings(Stream output)
        {
            XmlWriter xw = NthDimension.Rendering.GenericMethods.CoolXMLWriter(output);
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            ser.Serialize(xw, this);
            xw.Close();
        }

        public void LoadSettings(string path)
        {
            try
            {
                FileStream fs = File.OpenRead(path);
                LoadSettings(fs);
                fs.Close();
            }
            catch (System.Exception ex)
            {
                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("Error occured while loading settings: {0}", ex.Message));
            }

        }

        public void LoadSettings(Stream input)
        {
            XmlReader xr = XmlReader.Create(input);
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            Settings s = (Settings)ser.Deserialize(xr);
            video = s.video;
            sound = s.sound;
            game = s.game;
            mesh = s.mesh;
            view = s.view;
            xr.Close();
        }
    }
}
