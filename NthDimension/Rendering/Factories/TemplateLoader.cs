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
using System.IO;
using System.Xml;
using ProtoBuf;
using NthDimension.Algebra;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Serialization;

namespace NthDimension.Rendering.Loaders
{
    public class TemplateLoader : ApplicationObject
    {
        public List<Template> templates = new List<Template> { };
        private Hashtable templateNames = new Hashtable();

        public Hashtable Entries
        {
            get { return templateNames; }
        }

        public List<PhysModel> templatePhyModels = new List<PhysModel> { };

        public TemplateLoader()
            : base()
        {
        }

        public void readCacheFile(string filename = "", Action<int> callback = null)
        {
            if (filename == string.Empty)
                filename = Settings.Instance.game.templateCacheFile;

            if (!File.Exists(filename))
                Utilities.ConsoleUtil.errorlog("READ CACHE", String.Format(" WARNING: Cache file {0} is invalid", Path.GetFileName(filename)));
            else
            {
                FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                List<Template> tmpTemplates;

                using (fileStream)
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

                    tmpTemplates = (List<Template>)GenericMethods.ByteArrayToObject<List<Template>>(bytes);
                    fileStream.Close();
                }

                int templateCount = tmpTemplates.Count;
                for (int i = 0; i < templateCount; i++)
                {
                    Template curTmp = tmpTemplates[i];
                    string name = curTmp.name;

                    if (!templateNames.ContainsKey(name))
                    {
                        curTmp.type = Template.Type.fromCache;

                        int identifier = templates.Count;

                        curTmp.identifier = identifier;
                        curTmp.loaded = true;

                        templateNames.Add(name, identifier);
                        templates.Add(curTmp);
                    }
                }


                Utilities.ConsoleUtil.log(string.Format("\tAdded {0} templates from {1}", templateCount, Path.GetFileName(filename)));
            }
        }

        public void writeCacheFile(string directory = "", Action<string> callback = null)
        {
            if (directory == string.Empty)
                directory = Settings.Instance.game.templateCacheFile;
            else
                directory = Path.Combine(directory, Settings.Instance.game.templateCacheFile);

            List<Template> saveList = new List<Template> { };
            foreach (var template in templates)
            {
                if (null != callback)
                    callback(string.Format("caching template {0}", template.name));

                template.cache(ref saveList);
            }

            FileStream fileStream = new FileStream(directory, FileMode.Create, FileAccess.Write);

            using (fileStream)
            {
                byte[] saveAry = GenericMethods.ObjectToByteArray<List<Template>>(saveList);
                fileStream.Write(saveAry, 0, saveAry.Length);
                fileStream.Close();
            }
        }

        public void fromXmlFile(string pointer)
        {
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


            name = name.Replace(GameSettings.TemplateFolder, "");

            Debug.Print(pointer + " " + name);

            if (!templateNames.ContainsKey(name))
            {
                Template newTemp = new Template();

                newTemp.type = Template.Type.fromXml;
                newTemp.loaded = false;
                newTemp.name = name;
                newTemp.pointer = pointer;
                newTemp.filePosition = 0;

                register(newTemp);
            }
        }


        private void register(Template newTemp)
        {
            newTemp.identifier = templates.Count;
            templates.Add(newTemp);
            templateNames.Add(newTemp.name, newTemp.identifier);
        }

        public void loadTemplates(Action<float> callback = null)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                if (!templates[i].loaded)
                    loadTemplate(templates[i]);

                if (null != callback)
                    callback((float)i / (float)templates.Count);
            }
        }

        public Template getTemplate(string name)
        {
            int id = -1;

            try
            {
                id = (int)templateNames[name];
            }
            catch //(Exception te)
            {
                throw new Exception(string.Format("Template Loader: Template not found {0}", name));
            }
            return templates[id];
        }


        public Template getTemplate(int id)
        {
            return templates[id];
        }

        //public float loadSingleTemplates()
        //{
        //    for (int i = 0; i < templates.Count; i++)
        //    {
        //        if (!templates[i].loaded)
        //        {
        //            loadTemplate(templates[i]);
        //            return (float)i / (float)templates.Count;
        //        }
        //    }
        //    return 1;
        //}

        private void loadTemplate(Template target)
        {
            XmlTextReader reader = new XmlTextReader(target.pointer);

            target.meshes = new ListString() { };
            target.pmeshes = new ListString() { };
            target.materials = new ListString() { };

            target.isStatic = false;
            target.useType = Template.UseType.Model;

            while (reader.Read())
            {
                // parsing data in template tag
                if (reader.Name == "template")
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "name")
                            target.name = reader.Value;

                        if (reader.Name == "type")
                        {
                            switch (reader.Value)
                            {
                                case "animated":
                                    target.useType = Template.UseType.Animated;
                                    break;
                                case "meta":
                                    target.useType = Template.UseType.Meta;
                                    break;
                                default:
                                    target.useType = Template.UseType.Model;
                                    break;
                            }
                        }

                    }

                    Utilities.ConsoleUtil.log("parsing template: " + target.name);
                    reader.MoveToElement();

                    while (reader.Read())
                    {
                        if (reader.Name == "template")
                            break;

                        if (reader.Name == "static")
                            target.isStatic = true;

                        if (reader.Name == "material" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "source")
                                {
                                    target.materials.Add(reader.Value);
                                    Utilities.ConsoleUtil.log("material: " + reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "mesh" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "source")
                                {
                                    target.meshes.Add(reader.Value);
                                    Utilities.ConsoleUtil.log("mesh: " + reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "pmesh" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "source")
                                {
                                    target.pmeshes.Add(reader.Value);
                                    Utilities.ConsoleUtil.log("phys mesh: " + reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "position" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "offset")
                                {
                                    target.positionOffset = GenericMethods.FloatFromString(reader.Value);
                                    Utilities.ConsoleUtil.log("offset: " + reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "volume" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "radius")
                                {
                                    target.volumeRadius = GenericMethods.FloatFromString(reader.Value);
                                    Utilities.ConsoleUtil.log("radius: " + reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "light" && reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                target.hasLight = true;

                                if (reader.Name == "color")
                                {
                                    target.lightColor = GenericMethods.Vector3FromString(reader.Value);
                                }
                            }
                        }

                        if (reader.Name == "normal")
                        {
                            target.normal = true;
                        }

                        target.loaded = true;
                        templates[target.identifier] = target;
                    }
                }
            }
        }
    }
}
