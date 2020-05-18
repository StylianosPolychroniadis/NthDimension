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

using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Models;

namespace NthDimension.Rendering.Drawables.Voxel
{
    public class VoxelChunk : VoxelManager
    {
        VoxelDataGenerator voxelData;
        Model surface;
        VoxelMeshGenerator meshHelper;

        Vector3 start, end;

        public VoxelChunk(VoxelManager parent, Vector3 start, Vector3 end)
            : base(parent)
        {
            this.start = start;
            this.end = end;

            voxelData = new VoxelDataGenerator(this, start, end);
            meshHelper = new VoxelMeshGenerator(this, voxelData);

            Position = (end + start) / 2f;

            generateSurface();

            wasUpdated = false;
            //generateDebugSurface();
            //generateSurfaceMesh();
            //scene.generateParticleSys();

            surface.IsVisible = false;
        }

        private void generateSurface()
        {
            surface = new Model(this);
            surface.PrimitiveType = Rasterizer.PrimitiveType.Triangles;
            surface.addMaterial("voxel\\rock_face.xmf");

            surface.Color = new Vector4(0.8f, 0.3f, 0.8f, 1.0f) * 1f;
            surface.Size = (end - start) * 0.5f;

            surface.Position = Position;
        }

        public override void Update()
        {
            if (wasUpdated)
            {
                surface.setMesh(meshHelper.generateMesh(voxelData));
                if (voulumetrics.Count > 0)
                    surface.IsVisible = true;
            }
        }

        internal bool isAffected(VoxelVolume voxelVolume)
        {
            Vector3 position = voxelVolume.Position;
            float range = voxelVolume.AffectionRadius;

            return (position.X > start.X - range && position.X < end.X + range &&
                position.Y > start.Y - range && position.Y < end.Y + range &&
                position.Z > start.Z - range && position.Z < end.Z + range);
        }
    }
}
