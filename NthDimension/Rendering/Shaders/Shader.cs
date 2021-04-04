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
using OpenTK.Graphics.OpenGL;
using ProtoBuf;
using NthDimension.Algebra;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Shaders
{ // Platform
    [ProtoContract]
    public enum enuShaderType
    {
        [ProtoMember(10)]
        fromFile,
        [ProtoMember(20)]
        fromXml,
        [ProtoMember(30)]
        fromCache
    }

    [Serializable, ProtoContract]
    public struct Shader
    {
        #region Generic Properties
        //generic
        [ProtoMember(40)]
        public volatile string[]             Pointer;
        [ProtoMember(50)]
        public volatile enuShaderType        type;
        [ProtoMember(60)]
        public volatile int                  Handle;            // Should be serialized?
        [ProtoMember(70)]
        public volatile int                  Identifier;
        [ProtoMember(80)]
        public volatile bool                 Loaded;            // Should be serialized?

        #endregion

        #region Storable Properties
        //stuff to be saved
        [ProtoMember(90)]
        public volatile string               Name;
        [ProtoMember(100)]
        public volatile string               VertexShader;
        [ProtoMember(110)]
        public volatile string               FragmentShader;
        #endregion

        #region Light Location Properties
        [ProtoMember(120)]
        public volatile int[]                LightLocationsLocation;
        [ProtoMember(130)]
        public volatile int[]                LightDirectionsLocation;
        [ProtoMember(140)]
        public volatile int[]                LightColorsLocation;
        [ProtoMember(150)]
        public volatile int[]                LightViewMatrixLocation;

        [ProtoMember(160)]
        public volatile int[]                LightActiveLocation;
        [ProtoMember(170)]
        public volatile int[]                LightTextureLocation;
        #endregion

        #region Sun Properties
        [ProtoMember(180)]
        public volatile int                  SunDirection;
        [ProtoMember(190)]
        public volatile int                  SunColor;

        // TODO --> REFACTOR to COUNT = 4
        [ProtoMember(200)]
        public volatile int                  SunMatrix;
        [ProtoMember(210)]
        public volatile int                  SunInnerMatrix;
        // TODO <-- End of Refactor
        #endregion

        #region Bone Properties
        [ProtoMember(220)]
        public volatile int[]                BoneMatrixLocations;
        #endregion

        private volatile int[]               locations;
        private string[]                    uniformNames;

        public string[] Uniforms
        {
            get { return uniformNames; }
        }

        public Shader NameOnly()
        {
            Shader tmpShader = new Shader();

            tmpShader.Name = Name;

            return tmpShader;
        }

        public void Cache(ref ShaderCacheObject cacheObject)
        {
            Shader tmpShader = new Shader();

            tmpShader.Name = Name;
            tmpShader.VertexShader = VertexShader;
            tmpShader.FragmentShader = FragmentShader;

            cacheObject.shaders.Add(tmpShader);
        }
        public void GenerateLocations()
        {
            uniformNames = Enum.GetNames(typeof(Uniform));

            int handlesCount = uniformNames.Length;
            locations = new int[handlesCount];

            for (int i = 0; i < handlesCount; i++)
            {
                locations[i] = ApplicationBase.Instance.Renderer.GetUniformLocation(Handle, uniformNames[i]);
                if (locations[i] != -1)
                {
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    ConsoleUtil.log(string.Format("\tUniform: {0} Location: {1}", uniformNames[i], locations[i]));
                    Console.ForegroundColor = c;
                }
                //else
                //{
                //    ConsoleUtil.errorlog(string.Format("\tUniform: {0} ", names[i]), "does not exist");
                //}
            }


        }
        public void InsertGenUniform(Uniform uniform, object uniObj)
        {
            if (uniObj is int)
            {
                int tmpValue = (int)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is float)
            {
                float tmpValue = (float)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is Vector2)
            {
                Vector2 tmpValue = (Vector2)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is Vector3)
            {
                Vector3 tmpValue = (Vector3)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is Matrix4)
            {
                Matrix4 tmpValue = (Matrix4)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is Vector2d)
            {
                Vector2d tmpValue = (Vector2d)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }
            if (uniObj is Vector3d)
            {
                Vector3d tmpValue = (Vector3d)uniObj;
                InsertUniform(uniform, ref tmpValue);
                return;
            }

            throw new Exception("unable to assigin Uniform");
        }
        public  void InsertUniform(Uniform uni, ref int value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.Uniform1(location, 1, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString()); 
        }
        public void InsertUniform(Uniform uni, ref float value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.Uniform1(location, 1, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString());
        }
        //public  void InsertUniform(Uniform uni, ref double value)
        //{
        //    int location = locations[(int)uni];
        //    if (location != -1)
        //        Game.Instance.Renderer.Uniform1(location, ref value);
                
        //}

        // TODO:: UnBind (All InsertUniform's)
        // Brake to x floats per signature and override later on Extension the Vectors
        public void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector2 value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.Uniform2(location, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString());


        }
        public void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector3 value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.Uniform3(location, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString());

        }
        public void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector4 value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.Uniform4(location, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString());
        }
        public  void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector2d value)
        {
            throw new NotImplementedException(string.Format("No renderer support for uniform type {0}", value.GetType().Name));
        }
        public  void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector3d value)
        {
            throw new NotImplementedException(string.Format("No renderer support for uniform type {0}", value.GetType().Name));
        }
        public  void InsertUniform(Uniform uni, ref NthDimension.Algebra.Vector4d value)
        {
            throw new NotImplementedException(string.Format("No renderer support for uniform type {0}", value.GetType().Name));
        }
        public  void InsertUniform(Uniform uni, ref Matrix3 value)
        {
            //throw new NotImplementedException(string.Format("No renderer support for uniform type {0}", value.GetType().Name));
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.UniformMatrix3(location, false, ref value);
        }
        public  void InsertUniform(Uniform uni, ref Matrix4 value)
        {
            int location = locations[(int)uni];
            if (location != -1)
                ApplicationBase.Instance.Renderer.UniformMatrix4(location, false, ref value);
            //else
            //    ConsoleUtil.errorlog(string.Format("Shader {0} InsertUniform() returns -1 ", this.Name), uni.ToString());

        }
        public  void InsertUniform(Uniform uni, ref Matrix3d value)
        {
            throw new NotImplementedException(string.Format("No renderer support for uniform type {0}", value.GetType().Name));
        }
        //public  void InsertUniform(Uniform uni, ref Matrix4d value)
        //{
        //    int location = locations[(int)uni];
        //    if (location != -1)
        //    {
        //        OpenTK.Matrix4d val = value.ToOpenTK();
        //        Game.Instance.Renderer.UniformMatrix4(location, false, ref val);
        //        value = val.ToRafa();
        //    }
        //}

        public Shader nameOnly()
        {
            Shader tmpShader = new Shader();

            tmpShader.Name = Name;

            return tmpShader;
        }

        public void setLoaded(bool val)
        {
            Loaded = val;
        }

        public int ActiveAttributes
        {
            get
            {
                int ret = 0;
                ApplicationBase.Instance.Renderer.GetProgram(this.Handle, Rasterizer.GetProgramParameterName.ActiveAttributes, out ret);
                return ret;
            }            
        }

        public int ActiveUniforms
        {
            get
            {
                int ret = 0;
                ApplicationBase.Instance.Renderer.GetProgram(this.Handle, Rasterizer.GetProgramParameterName.ActiveUniforms, out ret);
                return ret;
            }
        }

        public void Reset()
        {
            Loaded = false;
        }
    }
}
