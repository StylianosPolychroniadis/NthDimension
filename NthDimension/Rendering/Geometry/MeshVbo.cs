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

using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NthDimension.Rendering.Loaders;

namespace NthDimension.Rendering.Geometry
{
    using System;
    using ProtoBuf;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Culling;
    using NthDimension.Rendering.Partition;
    using NthDimension.Rendering.Serialization;
    using Animation;

    [ProtoContract (SkipConstructor = true), Serializable]
    public partial class MeshVbo : OctreeObject //, ICloneable // OctreeObject Not Used
    {
        [ProtoContract] public enum MeshType
        {
            [ProtoMember(10)] Empty,
            [ProtoMember(20)] Generated,
            [ProtoMember(30)] Obj,
            [ProtoMember(40)] ColladaGeometry,
            [ProtoMember(50)] ColladaManaged,
            [ProtoMember(60)] FromCache
        }
        [ProtoContract] public enum MeshLod
        {
            [ProtoMember(10)] Level0 = 0,       // Highest Detail
            [ProtoMember(20)] Level1 = 1,
            [ProtoMember(30)] Level2 = 2,
            [ProtoMember(40)] Level3 = 3,       // Lowest Detail
        }

        [ProtoMember(10)]  public int                   Identifier;
        [ProtoMember(20)]  public string                Pointer;
        [ProtoMember(30)]  public string                Name;
        [ProtoMember(40)]  public MeshType              Type;
                           public MeshLod               CurrentLod = MeshLod.Level0;

        
        [ProtoMember(60)]  public bool                  HasCentroid;
        [ProtoMember(70)]  public bool                  IsAnimated;
        [ProtoMember(80)]  public bool                  IsLoaded;


        [ProtoMember(90)]  public float                 BoundingSphere;
                           public BoundingAABB          BoundingBoxLocal;
        public BoundingAABB                             BoundingBoxWorld;
        [ProtoMember(100)] public Vector3               CentroidLocal;
        public Vector3                                  CentroidWorld;
        public float                                    DistanceToCamera;

        [ProtoMember(110)] private  MeshVboData[]       meshDetailLevels;
        public  MeshVboData                             MeshData
                                                        {
                                                           get
                                                           {
                                                               return meshDetailLevels[(int)this.CurrentLod];
                                                           } }

        //[ProtoMember(120)]
        // TODO:: This routine is irrelevant, replace all 53 references with AnimationLoader.Animations
        public ListAnimationData                        AnimationData
        {
            get { return AnimationLoader.Animations;  }
            //set;
        }

        public int AnimationCount
        {
            get { return AnimationData.Count; }
        }

        public MeshVbo()
        {
            meshDetailLevels = new MeshVboData[Enum.GetNames(typeof(MeshLod)).Length];

            for (int i =0; i < Enum.GetNames(typeof(MeshLod)).Length; i++)
                    meshDetailLevels[i] = new MeshVboData();

            //AnimationData           = new ListAnimationData();
        }

        #region ToString

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Clone
        //public object Clone()
        //{
        //    Mesh m = (Mesh)this.MemberwiseClone();

        //    //if (null != this.AnimationData)
        //    //{
        //    //    m.AnimationData = (ListAnimationData) this.AnimationData.Clone();
        //    //    m.AnimationFrame = (AnimationData) this.AnimationFrame.Clone();
        //    //}

        //    //return m;
        //}
        #endregion

        #region CacheMesh
        internal void CacheMesh(ref ListMesh list)
        {
            MeshVbo tmp = new MeshVbo();

            if (this.Type == MeshType.Generated || !this.IsLoaded)
                return;

            tmp.Name = this.Name;

            if(this.Type == MeshType.Empty)
                list.Add(tmp);

            
            tmp.meshDetailLevels = new MeshVboData[this.meshDetailLevels.Length];

            for (int l = 0; l < this.meshDetailLevels.Length; l++)
            {
                    tmp.meshDetailLevels[l] = new MeshVboData();

                    // No need to store Face data    

                    tmp.meshDetailLevels[l].Indices = new int[this.meshDetailLevels[l].Indices.Length];
                    for (int i = 0; i < this.meshDetailLevels[l].Indices.Length; i++)
                        tmp.meshDetailLevels[l].Indices[i] = this.meshDetailLevels[l].Indices[i];
                
                    tmp.meshDetailLevels[l].Positions = new Vector3[this.meshDetailLevels[l].Positions.Length];
                    for (int i = 0; i < this.meshDetailLevels[l].Positions.Length; i++)
                        tmp.meshDetailLevels[l].Positions[i] = this.meshDetailLevels[l].Positions[i];

                    tmp.meshDetailLevels[l].Normals = new Vector3[this.meshDetailLevels[l].Normals.Length];
                    for (int i = 0; i < this.meshDetailLevels[l].Normals.Length; i++)
                        tmp.meshDetailLevels[l].Normals[i] = this.meshDetailLevels[l].Normals[i];

                    tmp.meshDetailLevels[l].Tangents = new Vector3[this.meshDetailLevels[l].Tangents.Length];
                    for(int i = 0; i < this.meshDetailLevels[l].Tangents.Length; i++)
                        tmp.meshDetailLevels[l].Tangents[i] = this.meshDetailLevels[l].Tangents[i];
                
                    tmp.meshDetailLevels[l].Textures    = new Vector2[this.meshDetailLevels[l].Textures.Length];
                    for(int i = 0; i < this.meshDetailLevels[l].Textures.Length; i++)
                        tmp.meshDetailLevels[l].Textures[i] = this.meshDetailLevels[l].Textures[i];

                if (null != this.meshDetailLevels[l].MeshBones)
                {
                    tmp.meshDetailLevels[l].MeshBones                         = new MeshBoneVboData();
                    tmp.meshDetailLevels[l].MeshBones.AffectingBonesCount     = this.meshDetailLevels[l].MeshBones.AffectingBonesCount;
                    tmp.meshDetailLevels[l].MeshBones.BoneIds                 = this.meshDetailLevels[l].MeshBones.BoneIds;
                    tmp.meshDetailLevels[l].MeshBones.BoneWeights             = this.meshDetailLevels[l].MeshBones.BoneWeights;
                    tmp.meshDetailLevels[l].MeshBones.BoneIdList              = this.meshDetailLevels[l].MeshBones.BoneIdList;
                    tmp.meshDetailLevels[l].MeshBones.BoneWeightList          = this.meshDetailLevels[l].MeshBones.BoneWeightList;

                }
            }

            // DO NOT SAVE TO MODEL CACHE - USE SEPERATE FILE INSTEAD
            //if (null != this.AnimationData && this.AnimationData.Count > 0)
            //    tmp.AnimationData = this.AnimationData;

            list.Add(tmp);
        }
        #endregion

        #region CalculateCentroid

        public void CalculateCentroid()
        {
#pragma warning disable CS0168
            try { this.CentroidLocal = this.CentroidWorld = GenericMethods.BaryCenter(this.GetPositions(MeshLod.Level0)); this.HasCentroid = true; } catch (Exception cE) { Utilities.ConsoleUtil.errorlog("Calculate Centroid Failed ", string.Format("{0}", this.Name)); this.HasCentroid = false; }
            try { this.BoundingBoxLocal = this.BoundingBoxWorld = BoundingAABB.CreateFromPoints(this.GetPositions(MeshLod.Level0)); } catch (Exception bE) { Utilities.ConsoleUtil.errorlog("Calculate AABB from Points Failed ", string.Format("{0}", this.Name)); this.HasCentroid = false; }
        }
        public void CalculateCentroid(Vector3 drawableWorldPosition)
        {
            this.CalculateCentroid();
            
            try { this.CentroidWorld = this.CentroidLocal + drawableWorldPosition; } catch { }

            Vector3[] vertices = this.GetPositions(CurrentLod);

            if (null != drawableWorldPosition)
                for (int v = 0; v < vertices.Length; v++)
                    vertices[v] += drawableWorldPosition;
            
            try { this.BoundingBoxWorld = BoundingAABB.CreateFromPoints(vertices); }
#pragma warning disable CS0168
            catch (Exception bE) { Utilities.ConsoleUtil.errorlog("Calculate AABB from Points Failed ", string.Format("{0}", this.Name)); this.HasCentroid = false; }
            this.HasCentroid = true;

            float sphere = 0;

            foreach (var vec in this.GetPositions(MeshLod.Level0))
            {
                float length = vec.LengthFast;
                if (length > sphere)
                    sphere = length;
            }

            this.BoundingSphere = sphere;
        }
        #endregion

        public Vector3[] GetPositions(MeshLod lod)
        {
            return this.meshDetailLevels[(int)lod].Positions; 
        }

        public void SetMeshData(MeshLod lod, MeshVboData data)
        {
            this.meshDetailLevels[(int)lod] = data;
        }
        public MeshVboData GetMeshData(MeshLod lod)
        {
            if(null != meshDetailLevels[(int)lod])
                return meshDetailLevels[(int)lod];

            return null;
        }

        #region Statistics

        public long DrawTimeAllPasses { get; set; }
        public long DrawTimeAllPassesPrevious { get; set; }
        public long DrawTimeAllPassesAccumulator { get; set; }
        public long AverageDrawTime { get; set; }

        public long DrawCalls { get; set; }

        #endregion


    }

    //[ProtoContract(SkipConstructor = true), Serializable]
    //public partial class CopyOfMesh : OctreeObject //, ICloneable // OctreeObject Not Used
    //{
    //    [ProtoContract]
    //    public enum MeshType
    //    {
    //        [ProtoMember(10)]
    //        Empty,
    //        [ProtoMember(20)]
    //        Generated,
    //        [ProtoMember(30)]
    //        Obj,
    //        [ProtoMember(40)]
    //        Collada,
    //        [ProtoMember(50)]
    //        ColladaManaged,
    //        [ProtoMember(60)]
    //        FromCache
    //    }
    //    [ProtoContract]
    //    public enum MeshLod
    //    {
    //        [ProtoMember(10)]
    //        Level0 = 0,       // Highest Detail
    //        [ProtoMember(20)]
    //        Level1 = 1,
    //        [ProtoMember(30)]
    //        Level2 = 2,
    //        [ProtoMember(40)]
    //        Level3 = 3,       // Lowest Detail
    //    }

    //    [ProtoMember(10)]
    //    public int Identifier;
    //    [ProtoMember(20)]
    //    public string Pointer;
    //    [ProtoMember(30)]
    //    public string Name;
    //    [ProtoMember(40)]
    //    public MeshType Type;
    //    public MeshLod CurrentLod = MeshLod.Level0;


    //    [ProtoMember(60)]
    //    public bool HasCentroid;
    //    [ProtoMember(70)]
    //    public bool IsAnimated;
    //    [ProtoMember(80)]
    //    public bool IsLoaded;


    //    [ProtoMember(90)]
    //    public float BoundingSphere;
    //    public BoundingAABB BoundingBox;
    //    [ProtoMember(100)]
    //    public Vector3 Centroid;

    //    [ProtoMember(110)]
    //    private MeshVboData[] meshLevels;
    //    public MeshVboData MeshData
    //    {
    //        get
    //        {
    //            return meshLevels[(int)this.CurrentLod];
    //        }
    //    }

    //    [ProtoMember(120)]
    //    public ListAnimationData AnimationData
    //    {
    //        get;
    //        set;
    //    }

    //    public int AnimationCount
    //    {
    //        get { return AnimationData.Count; }
    //    }



    //    public CopyOfMesh()
    //    {
    //        meshLevels = new MeshVboData[Enum.GetNames(typeof(MeshLod)).Length];

    //        for (int i = 0; i < Enum.GetNames(typeof(MeshLod)).Length; i++)
    //            meshLevels[i] = new MeshVboData();

    //        AnimationData = new ListAnimationData();
    //    }

    //    #region ToString

    //    public override string ToString()
    //    {
    //        return this.Name;
    //    }

    //    #endregion

    //    #region Clone
    //    //public object Clone()
    //    //{
    //    //    CopyOfMesh m = (CopyOfMesh)this.MemberwiseClone();

    //    //    //if (null != this.AnimationData)
    //    //    //{
    //    //    //    m.AnimationData = (ListAnimationData) this.AnimationData.Clone();
    //    //    //    m.AnimationFrame = (AnimationData) this.AnimationFrame.Clone();
    //    //    //}

    //    //    //return m;
    //    //}
    //    #endregion

    //    #region CacheMesh
    //    internal void CacheMesh(ref ListMesh list)
    //    {
    //        CopyOfMesh tmp = new CopyOfMesh();

    //        if (this.Type == MeshType.Generated || !this.IsLoaded)
    //            return;

    //        tmp.Name = this.Name;

    //        if (this.Type == MeshType.Empty)
    //            list.Add(tmp);


    //        tmp.meshLevels = new MeshVboData[this.meshLevels.Length];

    //        for (int l = 0; l < this.meshLevels.Length; l++)
    //        {
    //            tmp.meshLevels[l] = new MeshVboData();

    //            // No need to store Face data    

    //            tmp.meshLevels[l].Indices = new int[this.meshLevels[l].Indices.Length];
    //            for (int i = 0; i < this.meshLevels[l].Indices.Length; i++)
    //                tmp.meshLevels[l].Indices[i] = this.meshLevels[l].Indices[i];

    //            tmp.meshLevels[l].Positions = new Vector3[this.meshLevels[l].Positions.Length];
    //            for (int i = 0; i < this.meshLevels[l].Positions.Length; i++)
    //                tmp.meshLevels[l].Positions[i] = this.meshLevels[l].Positions[i];

    //            tmp.meshLevels[l].Normals = new Vector3[this.meshLevels[l].Normals.Length];
    //            for (int i = 0; i < this.meshLevels[l].Normals.Length; i++)
    //                tmp.meshLevels[l].Normals[i] = this.meshLevels[l].Normals[i];

    //            tmp.meshLevels[l].Tangents = new Vector3[this.meshLevels[l].Tangents.Length];
    //            for (int i = 0; i < this.meshLevels[l].Tangents.Length; i++)
    //                tmp.meshLevels[l].Tangents[i] = this.meshLevels[l].Tangents[i];

    //            tmp.meshLevels[l].Textures = new Vector2[this.meshLevels[l].Textures.Length];
    //            for (int i = 0; i < this.meshLevels[l].Textures.Length; i++)
    //                tmp.meshLevels[l].Textures[i] = this.meshLevels[l].Textures[i];

    //            if (null != this.meshLevels[l].MeshBones)
    //            {
    //                tmp.meshLevels[l].MeshBones = new MeshBoneVboData();
    //                tmp.meshLevels[l].MeshBones.AffectingBonesCount = this.meshLevels[l].MeshBones.AffectingBonesCount;
    //                tmp.meshLevels[l].MeshBones.BoneIds = this.meshLevels[l].MeshBones.BoneIds;
    //                tmp.meshLevels[l].MeshBones.BoneWeights = this.meshLevels[l].MeshBones.BoneWeights;
    //                tmp.meshLevels[l].MeshBones.BoneIdList = this.meshLevels[l].MeshBones.BoneIdList;
    //                tmp.meshLevels[l].MeshBones.BoneWeightList = this.meshLevels[l].MeshBones.BoneWeightList;

    //            }
    //        }

    //        if (null != this.AnimationData && this.AnimationData.Count > 0)
    //            tmp.AnimationData = this.AnimationData;

    //        list.Add(tmp);
    //    }
    //    #endregion

    //    #region CalculateCentroid

    //    public void CalculateCentroid()
    //    {


    //        this.Centroid = GenericMethods.BaryCenter(this.GetPositions(MeshLod.Level0));
    //        this.BoundingBox = BoundingAABB.CreateFromPoints(this.GetPositions(MeshLod.Level0));
    //        this.HasCentroid = true;
    //    }
    //    #endregion

    //    public Vector3[] GetPositions(MeshLod lod)
    //    {
    //        return this.meshLevels[(int)lod].Positions;
    //    }

    //    public void SetMeshData(MeshLod lod, MeshVboData data)
    //    {
    //        this.meshLevels[(int)lod] = data;
    //    }




    //}
}
