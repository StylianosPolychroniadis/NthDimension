﻿/* LICENSE
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NthDimension.Algebra;
using NthDimension.Rendering.Animation;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Serialization;

namespace NthDimension.Rendering.Loaders
{
    public class ColladaScene
    {
        public ListVector3 positionVboDataList;
        public ListVector3 normalVboDataList;
        public ListVector2 textureVboDataList;
        public ListFace FaceList;
        public ListVertex FpIndiceList;

        public float[][] boneWeights;
        public int[][] boneIds;


        float lastFrame = 0;
        int frameCount = 0;

        NumberFormatInfo nfi = GenericMethods.getNfi();
        static char[] seperator = " ".ToCharArray();

        private List<ColladaObject> colladaObjects = new List<ColladaObject> { };
        private List<ColladaNode> nodes = new List<ColladaNode> { };
        private List<ColladaLibraryAnimation> animationLibs = new List<ColladaLibraryAnimation> { };

        private Collada collada;
        public string armatureName;
        public float stepSize;
        private AnimationDataGenerator colladaAnimationDataGenerator;

        public ListAnimationData animationData = new ListAnimationData { };

        class ColladaObject
        {
            public string                       id;
            public List<ColladaInput>           inputs          = new List<ColladaInput> { };
            public ColladaObject                source;
            protected ColladaScene              scene;

            protected List<ColladaObject>       childs          = new List<ColladaObject> { };

            protected string nodename;
            protected ColladaObject parent;
            private string name;
            public string sid;

            public virtual ListVector3 Vector3Data { get { return source.Vector3Data; } set { } }
            public virtual ListVector2 Vector2Data { get { return source.Vector2Data; } set { } }

            public virtual string[] NameAry { get { return source.NameAry; } set { } }
            public virtual float[] FloatAry
            {
                get
                {
                    if (source != null)
                    {
                        return source.FloatAry;
                    }
                    else
                    {
                        return null;
                    }

                }
                set { }
            }
            public virtual List<Matrix4> Matrices { get { return source.Matrices; } set { } }

            public ColladaObject(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene, string nodename)
            {
                this.parent = parent;
                if (parent != null)
                    parent.childs.Add(this);

                this.scene = scene;
                scene.colladaObjects.Add(this);

                this.nodename = nodename;

                while (reader.MoveToNextAttribute())
                {
                    specialHeaderAttributes(ref reader);
                    genericHeaderAttributes(ref reader);
                }
                while (reader.Read() && !(reader.Name == nodename && reader.NodeType == XmlNodeType.EndElement))
                {
                    specialAttributes(ref reader);
                    genericAttributes(ref reader);
                }
            }

            public override string ToString()
            {
                return id;
            }

            protected ColladaInput getInput(string p)
            {
                foreach (var input in inputs)
                {
                    if (input.semantic == p)
                        return input;
                }
                return null;
            }

            protected int CalculateAttributeCount()
            {
                int attributeCount = 0;
                foreach (var input in inputs)
                {
                    if (input.offset > attributeCount)
                        attributeCount = input.offset;
                }
                attributeCount++;
                return attributeCount;
            }

            public ColladaObject(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
            {
                this.parent = parent;
                parent.childs.Add(this);

                this.scene = scene;
                scene.colladaObjects.Add(this);

                while (reader.MoveToNextAttribute())
                {
                    specialHeaderAttributes(ref reader);
                    genericHeaderAttributes(ref reader);
                }
            }

            protected virtual void specialHeaderAttributes(ref XmlTextReader reader)
            {
            }

            private void genericHeaderAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "id")
                {
                    id = reader.Value;
                }
                if (reader.Name == "name")
                {
                    name = reader.Value;
                }
                if (reader.Name == "sid")
                    sid = reader.Value;
            }

            protected virtual void specialAttributes(ref XmlTextReader reader)
            {
            }

            private void genericAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "source" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaSource(ref reader, this, scene);
                }

                if (reader.Name == "vertices" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaVerts(ref reader, this, scene);
                }

                if (reader.Name == "library_animations" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaLibraryAnimation(ref reader, this, scene);
                }

                if (reader.Name == "animation" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaAnimation(ref reader, this, scene);
                }

                if (reader.Name == "controller" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaControler(ref reader, this, scene);
                }

                if (reader.Name == "polylist" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaPolys(ref reader, this, scene);
                }
                /*
                if (reader.Name == "visual_scene")
                {
                    ColladaObject newObj = new ColladaVScene(ref reader, this, scene);
                    scene.colladaObjects.Add(newObj);
                }
                */
                if (reader.Name == "vertex_weights" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaVertWeights(ref reader, this, scene);
                }

                if (reader.Name == "node" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaObject newObj = new ColladaNode(ref reader, this, scene);
                }

                if (reader.Name == "input")
                    inputs.Add(new ColladaInput(ref reader, this, scene));
            }

            public virtual void generate()
            {
            }
        }

        class ColladaLibraryAnimation : ColladaObject
        {
            public List<ColladaAnimation> animations = new List<ColladaAnimation> { };
            public ColladaLibraryAnimation(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "library_animations")
            {

            }
        }

        /*
        class ColladaVScene : ColladaObject
        {
            public ColladaVScene(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "visual_scene")
            {
            }
        }
        */

        class ColladaNode : ColladaObject
        {
            public TranslationData translationData = new TranslationData();
            public RotationData rotationData = new RotationData();
            public TransformationData transformationData = new TransformationData();
            public string referenceName;
            private string treeName;
            public bool rootNode;
            public List<Matrix4> boneAnimationMatrices;
            public Matrix4 boneBaseMatrix = Matrix4.Identity;
            public Matrix4 invBaseMatrix;

            List<ColladaNode> childnodes = new List<ColladaNode> { };

            public struct TransformationData
            {
                public Matrix4 baseMatrix;

                public List<Matrix4> transformationMatrices;
                public List<Matrix4> animationMatrices;

                public void flush()
                {
                    transformationMatrices = null;
                }

                public List<Matrix4> generateMatrices(int frameCount)
                {
                    animationMatrices = new List<Matrix4> { };

                    if (transformationMatrices == null)
                        for (int i = 0; i < frameCount; i++)
                        {
                            animationMatrices.Add(baseMatrix);
                        }
                    else
                    {
                        int dataFrameCount = transformationMatrices.Count;
                        for (int i = 0; i < frameCount; i++)
                        {
                            Matrix4 curMat;

                            if (dataFrameCount > i)
                                curMat = transformationMatrices[i];
                            else
                                curMat = transformationMatrices[dataFrameCount - 1];

                            animationMatrices.Add(curMat);
                        }
                    }

                    return animationMatrices;
                }
            }

            public struct TranslationData
            {
                private Vector3 baseTranslation;
                public Matrix4 baseMatrix;

                public List<Vector3> animationTranslation;

                public List<Matrix4> animationMatrices;

                public Vector3 BaseTranslation
                {
                    get { return baseTranslation; }
                    set
                    {
                        baseTranslation = value;
                        baseMatrix = Matrix4.CreateTranslation(baseTranslation);
                    }
                }

                public void flush()
                {
                    animationTranslation = null;
                }

                public List<Matrix4> generateMatrices(int frameCount)
                {
                    animationMatrices = new List<Matrix4> { };

                    if (animationTranslation == null)
                        for (int i = 0; i < frameCount; i++)
                        {
                            animationMatrices.Add(baseMatrix);
                        }
                    else
                    {
                        int dataFrameCount = animationTranslation.Count;
                        for (int i = 0; i < frameCount; i++)
                        {
                            Vector3 translation;

                            if (dataFrameCount > i)
                                translation = animationTranslation[i];
                            else
                                translation = animationTranslation[dataFrameCount - 1];

                            animationMatrices.Add(Matrix4.CreateTranslation(translation));
                        }
                    }

                    return animationMatrices;
                }
            }

            public struct RotationData
            {
                private Vector3 baseRotation;
                public List<float> animationRotationX;
                public List<float> animationRotationY;
                public List<float> animationRotationZ;

                public List<Matrix4> animationMatrices;

                public Matrix4 baseMatrix;

                public Vector3 BaseRotation
                {
                    get { return baseRotation; }
                    set
                    {
                        baseRotation = value;
                        buildBaseMat();
                    }
                }

                private void buildBaseMat()
                {
                    float rotX = MathHelper.DegreesToRadians(baseRotation.X);
                    float rotY = MathHelper.DegreesToRadians(baseRotation.Y);
                    float rotZ = MathHelper.DegreesToRadians(baseRotation.Z);

                    baseMatrix = Matrix4.Identity;

                    baseMatrix *= Matrix4.CreateRotationX(rotX);
                    baseMatrix *= Matrix4.CreateRotationY(rotY);
                    baseMatrix *= Matrix4.CreateRotationZ(rotZ);
                }

                public void flush()
                {
                    animationRotationX = null;
                    animationRotationY = null;
                    animationRotationZ = null;
                }

                public List<Matrix4> generateMatrices(int frameCount)
                {
                    float rotX = MathHelper.DegreesToRadians(baseRotation.X);
                    float rotY = MathHelper.DegreesToRadians(baseRotation.Y);
                    float rotZ = MathHelper.DegreesToRadians(baseRotation.Z);

                    animationMatrices = new List<Matrix4> { };

                    List<Matrix4> animationMatricesX = new List<Matrix4> { };
                    if (animationRotationX == null)
                        animationMatricesX.Add(Matrix4.CreateRotationX(rotX));
                    else
                    {
                        foreach (var frameRotation in animationRotationX)
                        {
                            float rot = MathHelper.DegreesToRadians(frameRotation);
                            animationMatricesX.Add(Matrix4.CreateRotationX(rot));
                        }
                    }

                    List<Matrix4> animationMatricesY = new List<Matrix4> { };
                    if (animationRotationY == null)
                        animationMatricesY.Add(Matrix4.CreateRotationY(rotY));
                    else
                    {
                        foreach (var frameRotation in animationRotationY)
                        {
                            float rot = MathHelper.DegreesToRadians(frameRotation);
                            animationMatricesY.Add(Matrix4.CreateRotationY(rot));
                        }
                    }

                    List<Matrix4> animationMatricesZ = new List<Matrix4> { };
                    if (animationRotationZ == null)
                        animationMatricesZ.Add(Matrix4.CreateRotationZ(rotZ));
                    else
                    {
                        foreach (var frameRotation in animationRotationZ)
                        {
                            float rot = MathHelper.DegreesToRadians(frameRotation);
                            animationMatricesZ.Add(Matrix4.CreateRotationZ(rot));
                        }
                    }

                    int frameCountX = animationMatricesX.Count;
                    int frameCountY = animationMatricesY.Count;
                    int frameCountZ = animationMatricesZ.Count;

                    for (int i = 0; i < frameCount; i++)
                    {
                        Matrix4 tmpMat = Matrix4.Identity;

                        Matrix4 rotationX;
                        if (frameCountX > i)
                            rotationX = animationMatricesX[i];
                        else
                            rotationX = animationMatricesX[frameCountX - 1];

                        Matrix4 rotationY;
                        if (frameCountY > i)
                            rotationY = animationMatricesY[i];
                        else
                            rotationY = animationMatricesY[frameCountY - 1];

                        Matrix4 rotationZ;
                        if (frameCountZ > i)
                            rotationZ = animationMatricesZ[i];
                        else
                            rotationZ = animationMatricesZ[frameCountZ - 1];

                        tmpMat *= rotationX;
                        tmpMat *= rotationY;
                        tmpMat *= rotationZ;

                        animationMatrices.Add(tmpMat);
                    }

                    return animationMatrices;
                }
            }

            public ColladaNode(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "node")
            {
                scene.nodes.Add(this);

                TreeName = treeName;
            }

            protected override void specialHeaderAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "id")
                {
                    treeName = reader.Value;
                    rootNode = true;
                }

                if (reader.Name == "sid")
                    sid = reader.Value;
            }

            protected override void specialAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "node" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ColladaNode newObj = new ColladaJoint(ref reader, this, scene);
                    childnodes.Add(newObj);
                    reader.Read();
                }

                if (reader.Name == "translate" && reader.NodeType != XmlNodeType.EndElement)
                    setTranslation(ref reader);

                if (reader.Name == "rotate" && reader.NodeType != XmlNodeType.EndElement)
                    setRotation(ref reader);

                if (reader.Name == "matrix" && reader.NodeType != XmlNodeType.EndElement)
                    setMatix(ref reader);
            }

            private void setMatix(ref XmlTextReader reader)
            {
                reader.Read();
                string[] tmpAry = reader.Value.Trim().Split(' ');
                float[] tmpFAry = GenericMethods.FloatAryFromStringAry(tmpAry);
                transformationData.baseMatrix = Matrix4.Transpose(GenericMethods.Matrix4FromArray(tmpFAry));
            }

            private void setRotation(ref XmlTextReader reader)
            {
                reader.Read();
                string[] tmpAry = reader.Value.Trim().Split(' ');
                Vector3 pointer = new Vector3();
                pointer.X = GenericMethods.FloatFromString(tmpAry[0]);
                pointer.Y = GenericMethods.FloatFromString(tmpAry[1]);
                pointer.Z = GenericMethods.FloatFromString(tmpAry[2]);

                Vector3 rotation = pointer * GenericMethods.FloatFromString(tmpAry[3]);
                rotationData.BaseRotation += rotation;
            }

            private void setTranslation(ref XmlTextReader reader)
            {
                reader.Read();
                string[] tmpAry = reader.Value.Trim().Split(' ');
                Vector3 newTranslation = new Vector3();
                newTranslation.X = GenericMethods.FloatFromString(tmpAry[0]);
                newTranslation.Y = GenericMethods.FloatFromString(tmpAry[1]);
                newTranslation.Z = GenericMethods.FloatFromString(tmpAry[2]);
                translationData.BaseTranslation = newTranslation;
            }

            public void generateBase()
            {
                boneBaseMatrix = Matrix4.Identity;

                if (transformationData.baseMatrix == GenericMethods.Matrix4Zero)
                    transformationData.baseMatrix = Matrix4.Identity;

                if (rotationData.baseMatrix == GenericMethods.Matrix4Zero)
                    rotationData.baseMatrix = Matrix4.Identity;

                if (translationData.baseMatrix == GenericMethods.Matrix4Zero)
                    translationData.baseMatrix = Matrix4.Identity;

                boneBaseMatrix *= transformationData.baseMatrix;
                boneBaseMatrix *= rotationData.baseMatrix;
                boneBaseMatrix *= translationData.baseMatrix;
                boneBaseMatrix *= ParentBaseMatix;

                invBaseMatrix = Matrix4.Invert(boneBaseMatrix);

                generateBaseChilds();
            }

            private void generateBaseChilds()
            {
                foreach (var node in childnodes)
                {
                    node.generateBase();
                }
            }

            public override void generate()
            {
                List<Matrix4> translationMatrices = translationData.generateMatrices(scene.frameCount);
                List<Matrix4> rotationMatrices = rotationData.generateMatrices(scene.frameCount);
                List<Matrix4> transformationMatrices = transformationData.generateMatrices(scene.frameCount);

                Matrix4 basePOffset = ParentBaseMatix;
                List<Matrix4> parentOffset = ParentAnimationMatrices;

                boneAnimationMatrices = new List<Matrix4> { };
                int frameCount = scene.frameCount;
                for (int i = 0; i < frameCount; i++)
                {
                    Matrix4 frameMatrix = transformationMatrices[i];
                    frameMatrix *= rotationMatrices[i];
                    frameMatrix *= translationMatrices[i];
                    frameMatrix *= parentOffset[i];

                    boneAnimationMatrices.Add(frameMatrix);
                }

                generateChilds();

                translationData.flush();
                rotationData.flush();
                transformationData.flush();
            }

            public void generateChilds()
            {
                foreach (var node in childnodes)
                {
                    node.generate();
                }
            }

            public virtual List<Matrix4> ParentAnimationMatrices
            {
                get
                {
                    List<Matrix4> tmpList = new List<Matrix4> { };

                    int framecount = scene.frameCount;
                    for (int i = 0; i < framecount; i++)
                    {
                        tmpList.Add(Matrix4.Identity);
                    }

                    return tmpList;
                }
                set {; }
            }
            public virtual Matrix4 ParentBaseMatix
            {
                get
                {
                    return Matrix4.Identity;
                }
                set
                {
                }
            }

            public string TreeName
            {
                get { return null; }
                set
                {
                    this.treeName = value;
                    referenceName = treeName + "_" + sid;

                    foreach (var child in childs)
                    {
                        ColladaJoint jointChild = (ColladaJoint)child;
                        jointChild.TreeName = value;
                    }
                }
            }
        }

        class ColladaJoint : ColladaNode
        {
            public ColladaJoint(ref XmlTextReader reader, ColladaNode parent, ColladaScene scene)
                : base(ref reader, parent, scene)
            {
                //scene.nodes.Add(this);
            }

            protected override void specialHeaderAttributes(ref XmlTextReader reader)
            {
            }

            public override List<Matrix4> ParentAnimationMatrices
            {
                get
                {
                    ColladaNode parNode = (ColladaNode)parent;
                    return parNode.boneAnimationMatrices;
                }
                set
                {
                    base.ParentAnimationMatrices = value;
                }
            }

            public override Matrix4 ParentBaseMatix
            {
                get
                {
                    ColladaNode nodeParent = (ColladaNode)parent;
                    return nodeParent.boneBaseMatrix;
                }
                set
                {
                }
            }

        }

        class Collada : ColladaObject
        {
            public Collada(ref XmlTextReader reader, ColladaScene scene)
                : base(ref reader, null, scene, "COLLADA")
            {
            }
        }

        class ColladaAnimation : ColladaObject
        {
            private ColladaChannel channel;

            class ColladaChannel : ColladaObject
            {
                public ColladaNode targetObject;
                public Target targetValue;
                private string targetString;

                public enum Target
                {
                    location,
                    rotationX,
                    rotationY,
                    rotationZ,
                    transform
                }

                public ColladaChannel(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                    : base(ref reader, parent, scene)
                {
                }
                /*
                private ColladaNode getNodeByRefName(string name)
                {
                    foreach (var node in scene.nodes)
                    {
                        if (node.referenceName == name)
                            return node;
                    }
                    return null;
                }
                */
                private ColladaNode getNodeById(string name)
                {
                    foreach (var node in scene.nodes)
                    {
                        if (node.sid == name)
                            return node;
                        if (node.referenceName == name)
                            return node;
                    }
                    return null;
                }

                protected override void specialHeaderAttributes(ref XmlTextReader reader)
                {
                    if (reader.Name == "target")
                        targetString = reader.Value;
                }

                public void resolveTarget()
                {
                    string[] tmpAry = targetString.Split('/');
                    targetObject = getNodeById(tmpAry[0]);


                    switch (tmpAry[1])
                    {
                        case "location":
                            targetValue = Target.location;
                            break;
                        case "rotationX.ANGLE":
                            targetValue = Target.rotationX;
                            break;
                        case "rotationY.ANGLE":
                            targetValue = Target.rotationY;
                            break;
                        case "rotationZ.ANGLE":
                            targetValue = Target.rotationZ;
                            break;
                        case "transform":
                            targetValue = Target.transform;
                            break;
                        default:
                            break;
                    }
                }
            }

            public ColladaAnimation(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "animation")
            {
                ColladaLibraryAnimation libPar = (ColladaLibraryAnimation)parent;
                libPar.animations.Add(this);
            }

            protected override void specialAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "channel")
                {
                    channel = new ColladaChannel(ref reader, this, scene);
                }
            }

            public override void generate()
            {
                //tell the channel to fill in objects
                channel.resolveTarget();

                //get inputs
                ColladaInput input              = getInput("INPUT");
                ColladaInput output             = getInput("OUTPUT");
                ColladaInput interpolation      = getInput("INTERPOLATION");

                //get basic animation info
                float resolution                = scene.stepSize;
                float[] timeSteps               = input.FloatAry;

                int frameCount;
                if (timeSteps != null)
                    frameCount                  = timeSteps.Length;
                else
                {
                    Utilities.ConsoleUtil.log("(!) COLLADA ERROR: reading animation data failed: " + this.id);
                    return;
                }


                float endframe = timeSteps[timeSteps.Length - 1];
                if (scene.lastFrame < endframe)
                    scene.lastFrame = endframe;

                //prepare variables
                int frame = 0;
                float frameTimeA;
                float frameTimeB;

                if (channel.targetValue == ColladaChannel.Target.location)
                {

                    //get raw data
                    List<Vector3> locationVecs = output.Vector3Data;
                    List<Vector3> locationOut = new List<Vector3> { };

                    Vector3 frameValueA;
                    Vector3 frameValueB;


                    for (float time = 0; time < endframe + resolution; time += resolution)
                    {
                        //find out what frame we are in
                        for (int i = frame; i < frameCount; i++)
                        {
                            frame = i;
                            if (timeSteps[i] >= time)
                                break;
                        }

                        if (time >= endframe)
                        {
                            locationOut.Add(locationVecs[frameCount - 1]);
                        }
                        else if (frame == 0)
                        {
                            locationOut.Add(locationVecs[0]);

                        }
                        else
                        {
                            frameTimeA = timeSteps[frame - 1];
                            frameTimeB = timeSteps[frame];

                            frameValueA = locationVecs[frame - 1];
                            frameValueB = locationVecs[frame];

                            float timeSpan = frameTimeB - frameTimeA;
                            float localTime = time - frameTimeA;

                            float weight = localTime / timeSpan;

                            locationOut.Add(frameValueA * (1 - weight) + frameValueB * weight);
                        }
                    }

                    channel.targetObject.translationData.animationTranslation = locationOut;

                    if (scene.frameCount < locationOut.Count)
                        scene.frameCount = locationOut.Count;
                }
                else if (channel.targetValue == ColladaChannel.Target.transform)
                {
                    List<Matrix4> transformMatrices = output.Matrices;
                    List<Matrix4> transformationOut = new List<Matrix4> { };

                    Matrix4 frameValueA;
                    Matrix4 frameValueB;

                    for (float time = 0; time < endframe + resolution; time += resolution)
                    {
                        //find out what frame we are in
                        for (int i = frame; i < frameCount; i++)
                        {
                            frame = i;
                            if (timeSteps[i] >= time)
                                break;
                        }

                        if (time >= endframe)
                        {
                            transformationOut.Add(transformMatrices[frameCount - 1]);
                        }
                        else if (frame == 0)
                        {
                            transformationOut.Add(transformMatrices[0]);
                        }
                        else
                        {
                            frameTimeA = timeSteps[frame - 1];
                            frameTimeB = timeSteps[frame];

                            frameValueA = transformMatrices[frame - 1];
                            frameValueB = transformMatrices[frame];

                            float timeSpan = frameTimeB - frameTimeA;
                            float localTime = time - frameTimeA;

                            float weight = localTime / timeSpan;

                            transformationOut.Add(GenericMethods.BlendMatrix(frameValueB, frameValueA, weight));
                        }
                    }

                    channel.targetObject.transformationData.transformationMatrices = transformationOut;

                    if (scene.frameCount < transformationOut.Count)
                        scene.frameCount = transformationOut.Count;
                }
                else
                {
                    //get raw data
                    float[] roatationFloats = output.FloatAry;
                    List<float> roatationOut = new List<float> { };

                    float frameValueA;
                    float frameValueB;

                    for (float time = 0; time < endframe + resolution; time += resolution)
                    {
                        //find out what frame we are in
                        for (int i = frame; i < frameCount; i++)
                        {
                            frame = i;
                            if (timeSteps[i] >= time)
                                break;
                        }

                        if (time >= endframe)
                        {
                            roatationOut.Add(roatationFloats[frameCount - 1]);
                        }
                        else if (frame == 0)
                        {
                            roatationOut.Add(roatationFloats[0]);
                        }
                        else
                        {
                            frameTimeA = timeSteps[frame - 1];
                            frameTimeB = timeSteps[frame];

                            frameValueA = roatationFloats[frame - 1];
                            frameValueB = roatationFloats[frame];

                            float timeSpan = frameTimeB - frameTimeA;
                            float localTime = time - frameTimeA;

                            float weight = localTime / timeSpan;

                            roatationOut.Add(frameValueA * (1 - weight) + frameValueB * weight);
                        }
                    }

                    switch (channel.targetValue)
                    {
                        case ColladaChannel.Target.rotationX:
                            channel.targetObject.rotationData.animationRotationX = roatationOut;
                            break;
                        case ColladaChannel.Target.rotationY:
                            channel.targetObject.rotationData.animationRotationY = roatationOut;
                            break;
                        case ColladaChannel.Target.rotationZ:
                            channel.targetObject.rotationData.animationRotationZ = roatationOut;
                            break;
                        default:
                            break;
                    }

                    if (scene.frameCount < roatationOut.Count)
                        scene.frameCount = roatationOut.Count;
                }
            }
        }

        class ColladaControler : ColladaObject
        {
            public ColladaControler(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "controller")
            {
            }

            protected override void specialHeaderAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "name")
                {
                    scene.armatureName = reader.Value;
                }
            }
        }

        class ColladaPolys : ColladaObject
        {
            public ListFace Polys = new ListFace { };

            int[] vCounts;
            int[] rawIndices;

            public ColladaPolys(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "polylist")
            {
                int position = 0;
                foreach (var vCount in vCounts)
                {
                    Polys.Add(new Face(vCount, position));
                    position += vCount;
                }

                int attributeCount = CalculateAttributeCount();

                ColladaInput vertIn = getInput("VERTEX");
                ColladaInput normalIn = getInput("NORMAL");
                ColladaInput texIn = getInput("TEXCOORD");

                scene.positionVboDataList = vertIn.Vector3Data;
                scene.normalVboDataList = normalIn.Vector3Data;
                scene.textureVboDataList = GenericMethods.FlipY(texIn.Vector2Data);

                int offset = vertIn.offset;
                int normaloffset = normalIn.offset;
                int texoffset = texIn.offset;

                foreach (var Poly in Polys)
                {
                    int basepos = Poly.position * attributeCount;
                    foreach (var vert in Poly.Vertex)
                    {
                        vert.Vi = rawIndices[basepos + offset];
                        vert.Ni = rawIndices[basepos + normaloffset];
                        vert.Ti = rawIndices[basepos + texoffset];
                        basepos += attributeCount;
                    }
                }

                scene.FaceList = Polys;
            }

            protected override void specialAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "vcount" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    string[] tmpAry = reader.Value.Trim().Split(seperator);

                    int tmpLenth = tmpAry.Length - 1;
                    vCounts = new int[tmpLenth];

                    for (int i = 0; i < tmpLenth; i++)
                    {
                        vCounts[i] = GenericMethods.IntFromString(tmpAry[i]);
                    }
                }

                if (reader.Name == "p" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    string[] tmpAry = reader.Value.Trim().Split(seperator);

                    int tmpLenth = tmpAry.Length;
                    rawIndices = new int[tmpLenth];

                    for (int i = 0; i < tmpLenth; i++)
                    {
                        rawIndices[i] = GenericMethods.IntFromString(tmpAry[i]);
                    }
                }
            }
        }

        class ColladaInput : ColladaObject
        {
            public string semantic = "";
            public int offset = 0;

            public ColladaInput(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene)
            {
            }

            public override string ToString()
            {
                return semantic;
            }

            protected override void specialHeaderAttributes(ref XmlTextReader reader)
            {

                if (reader.Name == "semantic")
                    semantic = reader.Value;

                if (reader.Name == "offset")
                    offset = GenericMethods.IntFromString(reader.Value);

                if (reader.Name == "source")
                {
                    string target = reader.Value.Remove(0, 1);
                    foreach (var colladaObject in scene.colladaObjects)
                    {
                        if (colladaObject.id == target)
                            source = colladaObject;
                    }
                }
            }
        }

        class ColladaVerts : ColladaObject
        {
            public ColladaVerts(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "vertices")
            {
            }

            public override ListVector3 Vector3Data
            {
                get
                {
                    return inputs[0].Vector3Data;
                }
                set { }
            }
        }

        class ColladaSource : ColladaObject
        {
            protected float[] floatAry;
            protected string[] nameAry;

            public ColladaSource(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "source")
            {
            }

            protected override void specialAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "float_array" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    string[] tmpAry = reader.Value.Trim().Split(seperator);

                    floatAry = GenericMethods.FloatAryFromStringAry(tmpAry);
                }

                if (reader.Name == "Name_array" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    nameAry = reader.Value.Trim().Split(seperator);
                }
            }

            public override string[] NameAry { get { return nameAry; } set { } }
            public override float[] FloatAry { get { return floatAry; } set { } }

            public override List<Matrix4> Matrices
            {
                get
                {
                    List<Matrix4> matrices = new List<Matrix4> { };

                    int matrixCount = floatAry.Length / 16;
                    for (int i = 0; i < matrixCount; i++)
                    {
                        int pos = i * 16;
                        Matrix4 tmpMat = new Matrix4();

                        tmpMat.M11 = floatAry[0 + pos];
                        tmpMat.M12 = floatAry[1 + pos];
                        tmpMat.M13 = floatAry[2 + pos];
                        tmpMat.M14 = floatAry[3 + pos];

                        tmpMat.M21 = floatAry[4 + pos];
                        tmpMat.M22 = floatAry[5 + pos];
                        tmpMat.M23 = floatAry[6 + pos];
                        tmpMat.M24 = floatAry[7 + pos];

                        tmpMat.M31 = floatAry[8 + pos];
                        tmpMat.M32 = floatAry[9 + pos];
                        tmpMat.M33 = floatAry[10 + pos];
                        tmpMat.M34 = floatAry[11 + pos];

                        tmpMat.M41 = floatAry[12 + pos];
                        tmpMat.M42 = floatAry[13 + pos];
                        tmpMat.M43 = floatAry[14 + pos];
                        tmpMat.M44 = floatAry[15 + pos];

                        matrices.Add(Matrix4.Transpose(tmpMat));
                    }


                    return matrices;
                }
                set
                {
                    base.Matrices = value;
                }
            }

            public override ListVector3 Vector3Data
            {
                get
                {
                    int vecCount = floatAry.Length / 3;
                    ListVector3 tmpVec = new ListVector3 { };
                    for (int i = 0; i < vecCount; i++)
                    {
                        int position = i * 3;
                        tmpVec.Add(
                            new Vector3(
                            floatAry[position],
                            floatAry[position + 1],
                            floatAry[position + 2]));
                    }
                    return tmpVec;
                }
                set { }
            }

            public override ListVector2 Vector2Data
            {
                get
                {
                    int vecCount = floatAry.Length / 2;
                    ListVector2 tmpVec = new ListVector2 { };
                    for (int i = 0; i < vecCount; i++)
                    {
                        int position = i * 2;
                        tmpVec.Add(
                            new Vector2(
                            floatAry[position],
                            floatAry[position + 1]));
                    }
                    return tmpVec;
                }
                set { }
            }
        }

        class ColladaVertWeights : ColladaObject
        {
            int[] vCounts;
            int[] weightIndices;

            const int maxAffBones = 3;

            private struct WeightJointPair
            {
                public int joint;
                public float weight;

                public WeightJointPair(int joint, float weight)
                {
                    this.joint = joint;
                    this.weight = weight;
                }
            }

            private static int CompareByWeight(WeightJointPair pairA, WeightJointPair pairB)
            {
                return pairB.weight.CompareTo(pairA.weight);
            }

            public ColladaVertWeights(ref XmlTextReader reader, ColladaObject parent, ColladaScene scene)
                : base(ref reader, parent, scene, "vertex_weights")
            {
                int groupCount = 0;
                ColladaInput jointIn = getInput("JOINT");
                ColladaInput weightIn = getInput("WEIGHT");

                float[] weights = weightIn.FloatAry;

                int groupOffset = jointIn.offset;
                int weightOffset = weightIn.offset;

                int vertexCount = vCounts.Length;
                int attributeCount = CalculateAttributeCount();
                int readerPos = 0;

                string[] boneNames = jointIn.NameAry;
                scene.colladaAnimationDataGenerator.boneNames = boneNames;

                groupCount = boneNames.Length;
                if (groupCount > maxAffBones)
                    groupCount = maxAffBones;

                float[][] tmpBoneWeights = new float[groupCount][];
                int[][] tmpBoneIds = new int[groupCount][];

                for (int i = 0; i < groupCount; i++)
                {
                    tmpBoneWeights[i] = new float[vertexCount];
                    tmpBoneIds[i] = new int[vertexCount];
                }

                for (int i = 0; i < vertexCount; i++)
                {
                    int curGroups = vCounts[i];
                    List<WeightJointPair> weightList = new List<WeightJointPair> { };

                    for (int j = 0; j < curGroups; j++)
                    {
                        int id = weightIndices[readerPos + groupOffset];

                        int weightIndex = weightIndices[readerPos + weightOffset];
                        float weight = weights[weightIndex];


                        weightList.Add(new WeightJointPair(id, weight));

                        readerPos += attributeCount;
                    }

                    weightList.Sort(CompareByWeight);

                    float totalWeight = 0;
                    for (int j = 0; j < groupCount && j < curGroups; j++)
                    {
                        totalWeight += weightList[j].weight;
                    }
                    for (int j = 0; j < groupCount && j < curGroups; j++)
                    {

                        tmpBoneWeights[j][i] = weightList[j].weight / totalWeight;
                        tmpBoneIds[j][i] = weightList[j].joint + 1;
                    }
                }

                scene.boneWeights = tmpBoneWeights;
                scene.boneIds = tmpBoneIds;
            }

            protected override void specialAttributes(ref XmlTextReader reader)
            {
                if (reader.Name == "vcount" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    string[] tmpAry = reader.Value.Trim().Split(seperator);

                    int tmpLenth = tmpAry.Length - 1;
                    vCounts = new int[tmpLenth];

                    for (int i = 0; i < tmpLenth; i++)
                    {
                        vCounts[i] = GenericMethods.IntFromString(tmpAry[i]);
                    }
                }

                if (reader.Name == "v" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    string[] tmpAry = reader.Value.Trim().Split(seperator);

                    int tmpLenth = tmpAry.Length;
                    weightIndices = new int[tmpLenth];

                    for (int i = 0; i < tmpLenth; i++)
                    {
                        weightIndices[i] = GenericMethods.IntFromString(tmpAry[i]);
                    }
                }
            }
        }

        public ColladaScene(string pointer)
        {
            colladaAnimationDataGenerator = new AnimationDataGenerator();

            prepareFile(pointer);

            XmlTextReader reader = new XmlTextReader(pointer);

            stepSize = 1.0f / 10;

            while (reader.Read())
            {
                if (reader.Name == "COLLADA")
                    collada = new Collada(ref reader, this);
            }

            reader.Close();
        }

        internal void saveToTempGeometry(ref MeshVbo target)
        {
            if (FaceList != null)
            {
                // //////
                target.MeshData.Faces           = FaceList;
                target.MeshData.PositionCache   = positionVboDataList;
                target.MeshData.NormalCache     = normalVboDataList;
                target.MeshData.TextureCache    = textureVboDataList;
                

                if (null == target.MeshData.MeshBones)
                    target.MeshData.MeshBones   = new MeshBoneVboData();

               
                target.MeshData.MeshBones.BoneWeightList = boneWeights;
                target.MeshData.MeshBones.BoneIdList = boneIds;

            }
            else
                target.Type = MeshVbo.MeshType.Empty;
        }

        internal void saveToTempAnimation(ref MeshVbo target)
        {
            // TODO:: Drop this function - not needed?
            if (animationData.Count > 0)
            {
                //target.AnimationData = animationData;
                //target.AnimationFrame = animationData[0];
              
                target.IsAnimated = true;
            }
        }

        class AnimationDataGenerator
        {
            public string[] boneNames;
            public ColladaNode[] referingNodes;

            internal void generate(List<ColladaNode> nodes, ref AnimationData animationData)
            {
                int boneCount = boneNames.Length;
                int frameCount = animationData.Matrices.Length;
               
                referingNodes = new ColladaNode[boneCount];
                for (int i = 0; i < boneCount; i++)
                {
                    string curName = boneNames[i];
                    foreach (var node in nodes)
                    {
                        if (node.sid == curName)
                        {
                            referingNodes[i] = node;
                            break;
                        }
                    }
                }

                Matrix4[][] allMatrices = new Matrix4[frameCount][];
               
                for (int curframe = 0; curframe < frameCount; curframe++)
                {
                    Matrix4[] curMatrices = new Matrix4[boneCount];
                    for (int j = 0; j < boneCount; j++)
                    {
                        curMatrices[j] = referingNodes[j].invBaseMatrix * referingNodes[j].boneAnimationMatrices[curframe];
                        //curMatrices[j] = referingNodes[j].invBaseMatrix;
                        //curMatrices[j] = referingNodes[j].boneAnimationMatrices[curframe];
                    }
                    allMatrices[curframe] = curMatrices;
                }

                animationData.Matrices = allMatrices;
            }
        }

        internal void appendAnimations(string daeFile)
        {
            foreach (var node in nodes)
            {
                if (node.rootNode)
                    node.generateBase();
            }

            //string pointer = list[0].pointer;
            string pointer = daeFile;

            prepareFile(pointer);

            XmlTextReader reader = new XmlTextReader(pointer);

            frameCount = 0;

            while (reader.Read())
            {
                if (reader.Name == "library_animations" && reader.NodeType == XmlNodeType.Element)
                {
                    ColladaLibraryAnimation aniLib = new ColladaLibraryAnimation(ref reader, collada, this);

                    foreach (var animation in aniLib.animations)
                    {
                        animation.generate();
                    }

                    if (nodes.Count > 0 && colladaAnimationDataGenerator.boneNames != null)
                    {
                        foreach (var node in nodes)
                        {
                            if (node.rootNode)
                                node.generate();
                        }
                        AnimationData curAnimationData = new AnimationData();

                        curAnimationData.Matrices = new Matrix4[frameCount][];
                      
                        curAnimationData.stepSize = stepSize;
                        curAnimationData.lastFrame = lastFrame;

                        colladaAnimationDataGenerator.generate(nodes, ref curAnimationData);

                        animationData.Add(curAnimationData);
                        
                    }
                }
            }

            reader.Close();
        }

        

        string[] search = new string[] {  "[\"", "\"]" /*, "\r\n"*/ };
        string[] replace = new string[] { "['",  "']" /*, " "*/ };
        private void prepareFile(string filename)
        {
            StringBuilder result = new StringBuilder();

            if (System.IO.File.Exists(filename))
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string newLine = line.Replace(search[0], replace[0]);
                        newLine = newLine.Replace(search[1], replace[1]);
                        //newLine = newLine.Replace(search[2], replace[2]);
                        result.AppendLine(newLine);
                        //result.Append(newLine);
                        //result.Append(" ");



                    }
                }
            }
            else
            {
                throw new FileNotFoundException(filename);
            }

            using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(result);
                streamWriter.Close();
                fileStream.Close();
            }
        }
    }
}
