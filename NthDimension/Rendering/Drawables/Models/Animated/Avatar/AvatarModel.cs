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
using NthDimension.Rendering.Animation;

namespace NthDimension.Rendering.Drawables.Models
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Geometry;



    public class AvatarModel : AnimatedModel
    {
        new public static string                    nodename                = "avatarmodel";

        private bool                                faceCreated             = false;
        protected Model                             avatarFaceModel;
        private Vector3                             m_avatarScaleOffsetVec  = new Vector3(1f, 1f, 1f);
        private Vector3                             highest                 = new Vector3();

        private readonly string                     avatarFaceMaterialFile  = "test_avatarFace.xmf";            // From Avatar Model Configuration TODO 
        private readonly string                     avatarFaceMeshFile      = "characters\\test_face.obj";    // From Avatar Model Configuration TODO 

        #region ctor
        public AvatarModel(ApplicationObject parent) : base(parent)
        {
            

        }
        #endregion

        private void createFaceMode()
        {
            avatarFaceModel                     = new Model(this);
            avatarFaceModel.setMaterial(avatarFaceMaterialFile);
            avatarFaceModel.setMesh(avatarFaceMeshFile);
            avatarFaceModel.Size                = m_avatarScaleOffsetVec;       // TODO:: Switch to Matrix
            avatarFaceModel.IsVisible           = true;
            avatarFaceModel.Scene               = Scene;
            faceCreated                         = true;
        }

        public override void Update()
        {
            base.Update();

            // TODO:: ForEach Attachment in Attachments

            if (!faceCreated)
            {
                this.createFaceMode();

                for (int m = 0; m < this.meshes[0].MeshData.Positions.Length; m++)
                    if (this.meshes[0].MeshData.Positions[m].Y > highest.Y)
                        highest = this.meshes[0].MeshData.Positions[m];

            }

            Vector3 restoreScale            = this.Size;
            Vector3 offset                  = new Vector3(0f, -.18f, .11f);//new Vector3(0f, 1.25f, .20f);
            Matrix4 rotation                = new Matrix4();
            Matrix4 scale                   = Matrix4.CreateScale(restoreScale);
            Quaternion qrot                 = new Quaternion();
            Vector3 headPos                 = new Vector3();

            this.Size                       = new Vector3(1f, 1f, 1f);

            Matrix4 boneMat                 = new Matrix4(this.AnimationFrame.ActiveMatrices[5].ToArray());

            qrot                            = boneMat.ExtractRotation();
            rotation                        = Matrix4.CreateFromQuaternion(qrot);

            Vector3 lhighest                = highest + offset;
            Vector3 v                       = lhighest;
            boneMat.TransformVector(ref v);
            scale.TransformVector(ref v);
            headPos                         = v;


            this.avatarFaceModel.Orientation    = rotation;//Matrix4.CreateFromQuaternion(fmat.ExtractRotation());
            this.avatarFaceModel.Position       = this.Position + headPos;


            this.avatarFaceModel.IgnoreCulling = true;
            if (this.avatarFaceModel.meshes[0].CurrentLod != MeshVbo.MeshLod.Level0)
                this.avatarFaceModel.meshes[0].CurrentLod = MeshVbo.MeshLod.Level0;
            this.avatarFaceModel.Size = new Vector3(1, 1, 1);
            this.Size = restoreScale;
        }

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            base.specialLoad(ref reader, type);


            //Vector3 head = new Vector3();

            //if (!this.KnownVertices.TryGetValue("head", out head))
            //    throw new Exception("Attachment point 'head' not configured. Cannot proceed creating avatar face and hair");

            //Attachments.AddRange(new Attachment[]
            //{
            //    new Attachment(this, "Face",    5, head, new Vector3(0f, -.18f, .11f),      Quaternion.Identity,        "characters\\test_face.obj",        "test_avatarFace.xmf"),
            //    new Attachment(this, "Hair",    5, head, new Vector3(0f, 0f, 0f),           Quaternion.Identity,        "characters\\test_hair.obj",        "test_avatarHair.xmf")
            //});


        }
    }
}
