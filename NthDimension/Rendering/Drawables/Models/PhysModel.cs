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

using NthDimension.Rendering.Culling;
using NthDimension.Rendering.Geometry;

namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    using NthDimension.Algebra;
    using Rendering.Geometry;
    using NthDimension.Physics.Collision.Shapes;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;


    public class PhysModel : Model
    {
        protected RigidBody                 body;

        private List<string>                pBoxList = new List<string> { };

        new public static string            nodename = "pmodel";

        public bool                         grabable = true;

        

        public PhysModel(ApplicationObject parent)
            : base(parent)
        {
           
        }

        public override RigidBody RigidBody { get { return body; } set { body = value; forceUpdate(); } }

        // saving body to database
        public override void save(ref StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string position = GenericMethods.StringFromVector3(this.Position);
            string rotation = GenericMethods.StringFromJMatrix(RigidBody.Orientation);
            string size = GenericMethods.StringFromVector3(this.Size);
            string stringMaterial = GenericMethods.StringFromStringList(Materials);
            string meshes = GenericMethods.StringFromStringList(Meshes);
            string pboxes = GenericMethods.StringFromStringList(PhysBoxes);

            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "'>");
            sb.AppendLine(tab2 + "<mass>" + RigidBody.Mass + "</mass>" );
            sb.AppendLine(tab2 + "<position>" + position + "</position>");
            sb.AppendLine(tab2 + "<rotation>" + rotation + "</rotation>");
            sb.AppendLine(tab2 + "<size>" + size + "</size>");
            sb.AppendLine(tab2 + "<materials>" + stringMaterial + "</materials>");
            sb.AppendLine(tab2 + "<meshes>" + meshes + "</meshes>");
            sb.AppendLine(tab2 + "<pboxes>" + pboxes + "</pboxes>");

            if (IsStatic)
                sb.AppendLine(tab2 + "<isstatic/>");

            if(IgnoreCulling)
                sb.AppendLine(tab2 + "<cullignore/>");

            if(NthDimension.Settings.Instance.game.diagnostics)
                Utilities.ConsoleUtil.log(string.Format("@ Saving Physics Model: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }

        public override Matrix4 Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                base.Orientation = value;
                if (body != null)
                {
                    body.Orientation = GenericMethods.FromOpenTKMatrix(value);
                }
            }
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

                if (body != null)
                {
                    body.Position = GenericMethods.FromOpenTKVector(value);
                }
            }
        }

        #region physicob management

        public void setPhysMesh(string name)
        {
            if (name == string.Empty)
                return;

            pBoxList.Add(name);

            MeshVbo pboxMesh = ApplicationBase.Instance.MeshLoader.GetMeshByName(name);            
            pboxMesh.CurrentLod = MeshVbo.MeshLod.Level0;
            Shape objShape = new ConvexHullShape(GenericMethods.FromOpenTKVecArToJVecList(pboxMesh.GetPositions(MeshVbo.MeshLod.Level0)));
            

            setPhysMesh(objShape);
        }

        public void setPhysMesh(Shape newShape, float mass = 20f)   // ISSUE here: do not pass the hardcoded value. Instead oblige every PhysModel to specify its own mass
        {
            RigidBody newBody = new RigidBody(newShape);

            newBody.Position = GenericMethods.FromOpenTKVector(Position);
            newBody.Mass = mass;

            newBody.Tag = this;
            newBody.Orientation = GenericMethods.FromOpenTKMatrix(Orientation);

            RigidBody = newBody;
            Scene.AddRigidBody(newBody);
            newBody.Tag = this;
        }

        public void setPhysMesh(string pMeshName, JMatrix mOrientation)
        {
            pBoxList.Add(pMeshName);

            MeshVbo pboxMesh = ApplicationBase.Instance.MeshLoader.GetMeshByName(pMeshName);
            ConvexHullShape objShape = new ConvexHullShape(GenericMethods.FromOpenTKVecArToJVecList(pboxMesh.MeshData.Positions));

            setPhysMesh(objShape);
        }



        // needs to be fixed (Compound Shapes)

        public List<string> PhysBoxes
        {
            get { return pBoxList; }
            set
            {

                if (value.Count == 1)
                {
                    setPhysMesh(value[0]);
                    return;
                }

                CompoundShape.TransformedShape[] transformedShapes = new CompoundShape.TransformedShape[value.Count];
                //BoxShape[] transformedShapes = new BoxShape[value.Count];

                for (int i = 0; i < value.Count; i++)
                {
                    pBoxList.Add(value[i]);

                    MeshVbo pboxMesh = ApplicationBase.Instance.MeshLoader.GetMeshByName(value[i]);
                    //ConvexHullShape curShape = new ConvexHullShape(GenericMethods.FromOpenTKVecArToJVecList(pboxMesh.positionVboData));

                    BoundingAABB testBox = BoundingAABB.CreateFromPoints(pboxMesh.MeshData.Positions);
                    BoxShape curShape = new BoxShape(testBox.Max.X - testBox.Min.X,
                                                    testBox.Max.Y - testBox.Min.Y,
                                                    testBox.Max.Z - testBox.Min.Z);

                   
                    transformedShapes[i] = new CompoundShape.TransformedShape();
                    transformedShapes[i].Shape = curShape;
                    transformedShapes[i].Orientation = JMatrix.Identity;
                    transformedShapes[i].Position =  GenericMethods.FromOpenTKVector(testBox.Center); // -1.0f *  curShape.Shift;
                }


                // Create one compound shape
                CompoundShape cs = new CompoundShape(transformedShapes);


                setPhysMesh(cs);
                //setPhysMesh();
            }
        }

        public void updateMatrix()
        {
            Position = GenericMethods.ToOpenTKVector(RigidBody.Position);
            Orientation = GenericMethods.ToOpenTKMatrix(RigidBody.Orientation);
        }

        #endregion physicob management

        #region update

        public override void Update()
        {
            updateSelection();

            if (RigidBody != null)
            {
                if (!RigidBody.IsStaticOrInactive || Forceupdate)
                {
                    wasUpdated = true;
                    updateMatrix();
                    updateChilds();
                }
            }
        }

        public override void forceUpdate()
        {
            updateSelection();

            if (RigidBody != null)
            {
                wasUpdated = true;
                updateMatrix();
                updateChilds();
            }
        }

        #endregion upadate

        public void dissolve()
        {
            createDisModel();
            kill();
        }

        public override void kill()
        {
            Scene.RemoveRigidBody(RigidBody);
           
            base.kill();
        }

        private void createDisModel()
        {
            DissolvingModel disModel = new DissolvingModel(Parent, this.Size);
                      
            foreach (Rendering.Material material in materials)
                disModel.addMaterial("effects\\dissolve.xmf");

            foreach (MeshVbo mesh in meshes)
                disModel.addMesh(mesh);

            disModel.Position = Position;
            disModel.Orientation = Orientation;
            disModel.Scene = Scene;            
        }

        public bool IsStatic
        {
            get { return body.IsStatic; }
            set
            {
                if (body != null)
                {
                    Scene.RemoveRigidBody(body);
                    body.IsStatic = value;
                    Scene.AddRigidBody(body);
                    body.IsActive = true;
                    body.Tag = this;
                }
            }
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

            if (reader.Name.ToLower() == "pboxes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                PhysBoxes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "isstatic")
                IsStatic = true;

            if (reader.Name.ToLower() == "noshadow")
                CastShadows = false;

            if (reader.Name.ToLower() == "mass")
                RigidBody.Mass = GenericMethods.FloatFromString(reader.Value);
        }
    }
}
