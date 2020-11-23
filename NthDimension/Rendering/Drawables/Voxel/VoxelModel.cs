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

using NthDimension.Rendering.Drawables.Voxel;

namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.Text;
    using System.Xml;

    using NthDimension.Algebra;
    using Rendering.Drawables.Voxel;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Rendering.GameViews;
    using NthDimension.Rendering.Geometry;

#if VOXELS
    class VoxelModel : PhysModel
    {
        public VoxelVolume volume;

        new public static string nodename = "metamodel";

        public VoxelModel(ApplicationObject parent)
            : base(parent)
        {
            volume = new VoxelVolumeSphere(Scene.VoxelManager, 3f);

            IsStatic = true;

            grabable = false;
        }

        public override void save(ref StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string position = GenericMethods.StringFromVector3(this.Position);
            string rotation = GenericMethods.StringFromJMatrix(RigidBody.Orientation);
            string stringMaterial = GenericMethods.StringFromStringList(Materials);
            string meshes = GenericMethods.StringFromStringList(Meshes);
            string pboxes = GenericMethods.StringFromStringList(PhysBoxes);
            string radius = GenericMethods.StringFromFloat(volume.AffectionRadius);

            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "'>");
            sb.AppendLine(tab2 + "<position>" + position + "</position>");
            //sb.AppendLine(tab2 + "<rotation>" + rotation + "</rotation>");
            sb.AppendLine(tab2 + "<materials>" + stringMaterial + "</materials>");
            sb.AppendLine(tab2 + "<meshes>" + meshes + "</meshes>");
            sb.AppendLine(tab2 + "<pboxes>" + pboxes + "</pboxes>");
            sb.AppendLine(tab2 + "<vradius>" + radius + "</vradius>");


            Utilities.ConsoleUtil.log(string.Format("@ Saving Meta-Model: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            base.specialLoad(ref reader, type);

            if (reader.Name == "vradius" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                volume.AffectionRadius = GenericMethods.FloatFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "noshadow")
                CastShadows = false;
        }

        public override RigidBody RigidBody { get { return body; } set { body = value; forceUpdate(); body.IsStatic = true; } }

        public override void Update()
        {
            foreach (MeshVbo m in this.meshes)
                if (m.CurrentLod != MeshVbo.MeshLod.Level0)
                    m.CurrentLod = MeshVbo.MeshLod.Level0;
        }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                volume.Position = value;
            }
        }

        public override void kill()
        {
            volume.kill();
            base.kill();
        }

        //public override void draw(ViewInfo curView, bool targetLayer)
        //{
        //}

        //public override void drawNormal(ViewInfo curView)
        //{
        //}

        //public override void drawShadow(ViewInfo curView)
        //{
        //}

        //public override void drawDefInfo(ViewInfo curView)
        //{
        //}
    }
#endif
}
