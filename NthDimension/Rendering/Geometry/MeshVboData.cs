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

// Mesh Specification Revision 2.0   - August 2017

using System;

namespace NthDimension.Rendering.Geometry
{
    using ProtoBuf;
    using Newtonsoft.Json;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Animation;
    using NthDimension.Rendering.Serialization;

    [ProtoContract, Serializable]
    public class MeshVboData        // Not a struct - we need the class itself never copies of it
    {
        public volatile int[]                           vaoHandle = new int[0];

        [ProtoMember(5)]  public volatile bool          LodEnabled          = false;
        [ProtoMember(10)] public volatile bool          ContainsVbo;
        // Vbo Cache
        /*[ProtoMember(20)]*/  public ListVector3       PositionCache       = new ListVector3();
        /*[ProtoMember(30)]*/  public ListVector3       NormalCache         = new ListVector3();
        /*[ProtoMember(40)]*/  public ListVector2       TextureCache        = new ListVector2();
        // Vbo Data
        [ProtoMember(50)]  public int[]                 Indices             { get; set; }
        [ProtoMember(60)]  public Vector3[]             Positions           { get; set; }
        [ProtoMember(70)]  public Vector3[]             Normals             { get; set; }
        [ProtoMember(80)]  public Vector3[]             Tangents            { get; set; }
        [ProtoMember(90)]  public Vector2[]             Textures            { get; set; }
        // Faces
        [ProtoMember(100)] public ListFace              Faces               = new ListFace();
        // Vbo Handles
        [ProtoMember(110)] public volatile int          EboHandle;          // { get; set; }
        [ProtoMember(120)] public volatile int          PositionHandle;     // { get; set; }
        [ProtoMember(130)] public volatile int          NormalHandle;       // { get; set; }
        [ProtoMember(140)] public volatile int          TangentHandle;      // { get; set; }
        [ProtoMember(150)] public volatile int          TextureHandle;      // { get; set; }
        [ProtoMember(160)] public MeshBoneVboData       MeshBones           { get; set; }

        public MeshVboData()
        {
            this.ContainsVbo    = false;

            this.Indices        = new int[]     {};
            this.Positions      = new Vector3[] {};
            this.Normals        = new Vector3[] {};
            this.Tangents       = new Vector3[] {};
            this.Textures       = new Vector2[] {};

            // TODO Put those two in a struct
            //this.MeshBones      = new MeshBoneVboData();
            //this.MeshAnimation  = new MeshAnimationData();
        }
    }


    [ProtoContract (SkipConstructor = true), System.Serializable]
    public class MeshBoneVboData
    {
        [ProtoMember(10)] public int[]                  BoneIdHandles       { get; set; }
        [ProtoMember(20)] public int[]                  BoneWeigthHandles   { get; set; }
        [ProtoMember(30)] public int                    AffectingBonesCount { get; set; }
        public int[][]                                  BoneIds         {
            get
            {
                if (null != mboneIdVboData)
                    return mboneIdVboData;

                if (null != boneIdVboDataString)
                    return mboneIdVboData = JsonConvert.DeserializeObject<int[][]>(boneIdVboDataString);



                return null;
            }
            set
            {
                if (null != value)
                {
                    mboneIdVboData = value;

                    boneIdVboDataString = JsonConvert.SerializeObject(mboneIdVboData, Formatting.Indented);
                }
            }
        }
        public float[][]                                BoneWeights     {
            get
            {
                if (null != mboneWeightVboData)
                {
                    AffectingBonesCount = mboneWeightVboData.Length;
                    return mboneWeightVboData;
                }

                if (null != boneWeightVboDataString)
                {

                    mboneWeightVboData = JsonConvert.DeserializeObject<float[][]>(boneWeightVboDataString);

                    return mboneWeightVboData;
                }



                return null;
            }
            set
            {
                if (null != value)
                {
                    mboneWeightVboData = value;

                    boneWeightVboDataString = JsonConvert.SerializeObject(mboneWeightVboData, Formatting.Indented);
                }
            }
        }
        public int[][]                                  BoneIdList      {
            get
            {
                if (null != mboneIdList)
                    return mboneIdList;

                if (null != boneIdListString)
                    return mboneIdList = JsonConvert.DeserializeObject<int[][]>(boneIdListString);



                return null;
            }
            set
            {
                if (null != value)
                {
                    mboneIdList = value;

                    boneIdListString = JsonConvert.SerializeObject(mboneIdList, Formatting.Indented);
                }
            }
        }
        public float[][]                                BoneWeightList  {
            get
            {
                if (null != mboneWeightList)
                    return mboneWeightList;

                if (null != boneWeightListString)
                    return mboneWeightList = JsonConvert.DeserializeObject<float[][]>(boneWeightListString);



                return null;
            }
            set
            {
                if (null != value)
                {
                    mboneWeightList = value;

                    boneWeightListString = JsonConvert.SerializeObject(mboneWeightList, Formatting.Indented);
                }
            }
        }

        #region Serialization to ProtoBuf through Json
        [ProtoMember(40)] private string                boneIdVboDataString;
        [ProtoMember(50)] private string                boneWeightVboDataString;
        [ProtoMember(60)] private string                boneIdListString;
        [ProtoMember(70)] private string                boneWeightListString;

        private int[][]                                 mboneIdVboData;
        private float[][]                               mboneWeightVboData;
        private int[][]                                 mboneIdList;
        private float[][]                               mboneWeightList;
        #endregion

        public MeshBoneVboData()
        {
            this.AffectingBonesCount    = 0;
            this.BoneIdHandles          = new int[] { };
            this.BoneWeigthHandles      = new int[] { };
            this.BoneIds                = new int[][] { };
            this.BoneWeights            = new float[][] { };
            this.BoneIdList             = new int[][] { };
            this.BoneWeightList         = new float[][] { };
        }
    }

    //[ProtoContract]
    //public struct MeshAnimationData
    //{
    //    [ProtoMember(10)] public ListAnimationData      AnimationData       { get; set; }
    //    [ProtoMember(20)] public AnimationData          AnimationFrame      { get; set; }
    //    [ProtoMember(30)] public float                  AnimationTotalFrames        { get; set; }
    //}
}
