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

using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Scenegraph;

namespace NthDimension.Rendering.Drawables.Framebuffers
{
    using System;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using Rendering.Scenegraph;


    public class CubemapBufferSets : ApplicationObject
    {
        private Framebuffer[]           outFrameBuffers = new Framebuffer[6];
        public FramebufferSet[]         FrameBufferSets = new FramebufferSet[6];

        public ViewInfo[]               cubeView = new ViewInfo[6];

        public Vector3[] viewDirections = new Vector3[] {
            new Vector3(-1,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0),
            new Vector3(0,0,-1),
            new Vector3(0,1,0),
            new Vector3(0,-1,0)
        };

        public Vector3[] upDirections = new Vector3[] {
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,0,-1),
            new Vector3(0,0,1)
        };

        public int[] outTextures = new int[6];

        public CubemapBufferSets(SceneGame parent, FramebufferCreator mFramebufferCreator, int size)
        {
            this.Scene = parent;
            Parent = parent;

            Vector2 vecSize = new Vector2(size, size);

            //float fovy = (float)Math.PI / 2;

            for (int i = 0; i < 6; i++)
            {
                if (outFrameBuffers[i] != null) outFrameBuffers[i].Delete();
                outFrameBuffers[i] = mFramebufferCreator.createFrameBuffer(size, size, PixelInternalFormat.Rgba8, true);
                outTextures[i] = outFrameBuffers[i].ColorTexture;
                FrameBufferSets[i] = new FramebufferSet(mFramebufferCreator, vecSize, outFrameBuffers[i]);

                cubeView[i] = new ViewInfo(this);
                cubeView[i].PointingDirection = viewDirections[i];
                cubeView[i].upVec = upDirections[i];

                cubeView[i].aspect = ApplicationBase.Instance.Width / ApplicationBase.Instance.Height;
                //cubeView[i].fovy = fovy;
                cubeView[i].UpdateProjectionMatrix();
            }
        }

        public override void Update()
        {
            
            Position = Scene.EyePos;

            updateChilds();
        }

        public void Delete()
        {
            for (int o = 0; o < outFrameBuffers.Length; o++)
                if (null != outFrameBuffers[o])
                    outFrameBuffers[o].Delete();

            for (int s = 0; s < FrameBufferSets.Length; s++)
                if (null != FrameBufferSets[s])
                    FrameBufferSets[s].Delete();
        }
    }
}
