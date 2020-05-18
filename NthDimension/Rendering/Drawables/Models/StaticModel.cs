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
using System.Xml;

namespace NthDimension.Rendering.Drawables.Models
{
    public class StaticModel : Model
    {

        new public static string nodename = "smodel";

        public StaticModel(ApplicationObject parent):base(parent)
        {
            
        }

        public override void save(ref StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string position             = GenericMethods.StringFromVector3(this.Position);
            string rotation             = null != AvatarBody ? 
                                          GenericMethods.StringFromJMatrix(AvatarBody.Orientation) :
                                          "1|0|0|0|1|0|0|0|1";
            string size                 = GenericMethods.StringFromVector3(this.Size);
            string stringMaterial       = GenericMethods.StringFromStringList(Materials);
            string meshes               = GenericMethods.StringFromStringList(Meshes);
           


            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "'>");
            sb.AppendLine(tab2 + "<position>" + position + "</position>");
            sb.AppendLine(tab2 + "<rotation>" + rotation + "</rotation>");
            sb.AppendLine(tab2 + "<size>" + size + "</size>");
            sb.AppendLine(tab2 + "<materials>" + stringMaterial + "</materials>");
            sb.AppendLine(tab2 + "<meshes>" + meshes + "</meshes>");
            sb.AppendLine(tab2 + "<isstatic />");
            if (IgnoreCulling)
                sb.AppendLine(tab2 + "<cullignore />");
            if (IgnoreLod == true)
                sb.AppendLine("<lodignore />");
            if (Renderlayer == RenderLayer.Transparent)
                sb.AppendLine("<transparent />");
            if (CastShadows == false)
                sb.AppendLine("<noshadow />");
            
            //Utilities.ConsoleUtil.log(string.Format("@ Saving Static Model: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            if (reader.Name.ToLower() == "materials" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Materials = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "meshes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Meshes = GenericMethods.StringListFromString(reader.Value);
            }

            //if (reader.Name == "pboxes" && reader.NodeType != XmlNodeType.EndElement)
            //{
            //    reader.Read();
            //    PhysBoxes = GenericMethods.StringListFromString(reader.Value);
            //}

            if (reader.Name.ToLower() == "cullignore")
                IgnoreCulling = true;

            if (reader.Name.ToLower() == "lodignore")
                IgnoreLod = true;

            if (reader.Name.ToLower() == "transparent")
                Renderlayer = RenderLayer.Transparent;

            if (reader.Name.ToLower() == "noshadow")
                CastShadows = false;


        }
    }
}
