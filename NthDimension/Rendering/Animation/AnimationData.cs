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


//#define DEBUG_LOG_ANI_DESERIALIZE
//#define DEBUG_VERBOSE
#define DEBUG_TIME
#define SERIALIZE_FLOAT16
#define MTHREAD


#if DEBUG_LOG_ANI_DESERIALIZE
    using NthDimension.Graphics.Utilities;
#endif

namespace NthDimension.Rendering.Animation
{
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using ProtoBuf;
    using NthDimension.Algebra;



    /// <summary>
    /// Represents a distinct animation and holds the animation bone matrix data
    /// </summary>
    [Serializable, ProtoContract]
    public class AnimationData// : ICloneable   
    {
        /// <summary>
        /// Animation playback mode
        /// Once - runs the animation once then switches to the Transition one
        /// Loop - once the last frame of the animatiopn has been reached, return to the first frame
        /// </summary>
        [ProtoContract]
        public enum PlaybackMode        // NOTE:: Currently using string name for mode
        {
            [ProtoMember(10)]
            Once = 0,
            [ProtoMember(20)]
            Loop = 1
        }

        /// <summary>
        /// The step/frame period of the animation
        /// </summary>
        [ProtoMember(10)]
        public float                stepSize;
        /// <summary>
        /// The position of the last frame
        /// </summary>
        [ProtoMember(20)]
        public float                lastFrame;
        /// <summary>
        /// /// The current frame position of the animation 
        /// </summary>
        [ProtoMember(30)]
        public float                animationPos;
        /// <summary>
        /// The animation literal name
        /// </summary>
        [ProtoMember(40)]
        public string               name;
        /// <summary>
        /// The path to the animation file
        /// </summary>
        [ProtoMember(50)]
        public string               pointer;
        /// <summary>
        /// The handle for the animation
        /// </summary>
        [ProtoMember(55)]
        public int                  identifier;
        /// <summary>
        /// Store the matrices data into a string via Json. 
        /// This value is then serialized by Protobuf
        /// </summary>
        [ProtoMember(60)]
        private string              mmatricesString;
        /// <summary>
        /// The active dataset of the bone matrices
        /// </summary>
        [ProtoMember(70)]
        public Matrix4[]            activeMatrices;
        /// <summary>
        /// Animation playback speed can be overriden on runtime (see AnimationRuntime class)
        /// </summary>
        [ProtoMember(80)]
        public float                AnimationSpeed = 1f;
        /// <summary>
        /// Animation playback mode (once, loop, etc)
        /// Can be overriden on runtime (see AnimationRuntime class)
        /// </summary>
        [ProtoMember(90)]
        public string               Playback = string.Empty;            // TODO:: Replace by PlaybackMode
        /// <summary>
        /// The name of the next animation once the current has finished (valid onle for Playback == Once)
        /// Can be overriden on runtime (see AnimationRuntime class)
        /// </summary>
        [ProtoMember(100)]
        public string               Transition = string.Empty;          // TODO:: Either int index (add to xmd file attribute id) of create function GetAnimationByName(string name)
        /// <summary>
        /// The entire animation bone data
        /// </summary>
        public Matrix4[][]          Matrices
        {
            get
            {
                if (null != mmatrices)
                    return mmatrices;

                if (null != mmatricesString)
                {
#if DEBUG_LOG_ANI_DESERIALIZE
                        //Utilities.ConsoleUtil.log(string.Format("Deserializing animation data: {0}", mmatricesString));
                    Utilities.ConsoleUtil.log(string.Format("Deserializing animation data: {0}", this.Name));
#endif

#if DEBUG_TIME
                    Utilities.ConsoleUtil.log(string.Format("{0}Deserializing animation data: {1}", Environment.NewLine, this.name), false);
                    DateTime start = DateTime.Now;
#endif

#if SERIALIZE_FLOAT16
                    float16[][] fder = JsonConvert.DeserializeObject<float16[][]>(mmatricesString,
                            new JsonSerializerSettings()
                            {
                                FloatParseHandling = FloatParseHandling.Decimal,
                                Formatting = 0,
                                Culture = new CultureInfo("en-US")
                            });

                    mmatrices = new Matrix4[fder.Length][];

                    #region Single Thread
#if !MTHREAD
                    for (int r = 0; r < fder.Length; r++)
                        {
                            mmatrices[r] = new Matrix4[fder[r].Length];

#if DEBUG_VERBOSE
                            ConsoleUtil.log(string.Format("[ {0} - Frame {1} ]{2}", this.name, r, Environment.NewLine));
#endif

                            for (int m = 0; m < fder[r].Length; m++)
                            {
                                mmatrices[r][m] = float16.FromFloat16(fder[r][m]);
#if DEBUG_VERBOSE
                                ConsoleUtil.log(string.Format("     Joint {0}: {1} {2}", m, fder[r][m].ToString(), Environment.NewLine));
#endif
                            }
                        }
#endif
                    #endregion

                    #region MultiThread
#if MTHREAD
                    System.Collections.Concurrent.ConcurrentDictionary<int, Matrix4[]> mThreadList = new System.Collections.Concurrent.ConcurrentDictionary<int, Matrix4[]>();
                    System.Threading.Tasks.Parallel.For(0, fder.Length, r =>
                    {
                        Matrix4[] mat = new Matrix4[fder[r].Length];
                        for (int m = 0; m < fder[r].Length; m++)
                            mat[m] = float16.FromFloat16(fder[r][m]);
                        mThreadList.TryAdd(r, mat);

                    });
                    mThreadList = new System.Collections.Concurrent.ConcurrentDictionary<int, Matrix4[]>(mThreadList.OrderBy(p => p.Key));
                    mmatrices   = mThreadList.Values.ToArray();
#endif
                    #endregion

#if DEBUG_TIME
                    NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("Deserializing animation data: {0}\t\t\t\t{1}", this.name, (DateTime.Now - start).ToString("g")), false);
#endif

                    return mmatrices;
#else
                    return mmatrices = JsonConvert.DeserializeObject<Matrix4[][]>(mmatricesString, new JsonSerializerSettings()
                    {
                        FloatParseHandling = FloatParseHandling.Decimal,
                        Formatting = 0,
                        Culture = new CultureInfo("en-US")
                    });
#endif
                }

                return null;
            }
            set
            {
                if (null != value)
                {
                    mmatrices = value;

#if SERIALIZE_FLOAT16
                    List<float16[]> mList = new List<float16[]>();

                    for (int r = 0; r < mmatrices.Length; r++)
                    {
                        if (mmatrices[r] == null)
                            continue;

                        try
                        {
                            float16[] frame = new float16[mmatrices[r].Length];
                            for (int m = 0; m < mmatrices[r].Length; m++)
                                frame[m]            = float16.FromMatrix4(mmatrices[r][m]);
                            mList.Add(frame);
                        }
                        catch (Exception sE)
                        {
                            Utilities.ConsoleUtil.errorlog("AnimationData Frame Error: ", string.Format("Failed to parse frame {0} animation data. Reason: {1}", r, sE.Message));
                        }
                    }

                    float16[][] fser = mList.ToArray();

                    if(mList.Count > 0)
                    try
                    {
                            mmatricesString = string.Empty;
                            Utilities.ConsoleUtil.log(string.Format("Serializing animation data: {0}", fser.ToString()));
                            mmatricesString = JsonConvert.SerializeObject(fser, new JsonSerializerSettings()
                            {
                                FloatParseHandling = FloatParseHandling.Decimal,
                                Formatting = 0,
                                Culture = new CultureInfo("en-US")
                            });

                            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            //using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
                            //using (NullJsonWriter njw = new NullJsonWriter(sw))
                            //{
                            //    JsonSerializer ser = new JsonSerializer();
                            //    ser.Formatting = Formatting.None;
                            //    ser.Serialize(njw, fser);
                            //}
                            //mmatricesString = sb.ToString();
                        }
                    catch (Exception sE)
                        {
                            Utilities.ConsoleUtil.errorlog("AnimationData Error: ", string.Format("Failed to serialize animation data. Reason: {0}", sE.Message));
                        }
#else
                    Utilities.ConsoleUtil.log(string.Format("Serializing animation data: {0}", mmatrices));
                    // TODO:: Serialize fser instead of mmatrices
                    mmatricesString = JsonConvert.SerializeObject(mmatrices, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal,
                                                                                                            Formatting = 0,
                                                                                                            Culture = new CultureInfo("en-US")
                                                                                                          });
#endif
                }
            }
        }
        private Matrix4[][]         mmatrices;

        public override string      ToString()
        {
            return name;
        }

    
        // TODO:: Serialize to string JSON this instead of Matrix4
        class float16
        {
            public float M11;
            public float M12;
            public float M13;
            public float M14;

            public float M21;
            public float M22;
            public float M23;
            public float M24;

            public float M31;
            public float M32;
            public float M33;
            public float M34;

            public float M41;
            public float M42;
            public float M43;
            public float M44;

            public float16(float m11, float m12, float m13, float m14,
                float m21, float m22, float m23, float m24,
                float m31, float m32, float m33, float m34,
                float m41, float m42, float m43, float m44)
            {
                this.M11 = m11;
                this.M12 = m12;
                this.M13 = m13;
                this.M14 = m14;

                this.M21 = m21;
                this.M22 = m22;
                this.M23 = m23;
                this.M24 = m24;

                this.M31 = m31;
                this.M32 = m32;
                this.M33 = m33;
                this.M34 = m34;

                this.M41 = m41;
                this.M42 = m42;
                this.M43 = m43;
                this.M44 = m44;

            }

            public override string ToString()
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
                                      M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
            }

            public static float16 FromMatrix4(Matrix4 m)
            {
                return new float16(m.M11,
                    m.M12,
                    m.M13,
                    m.M14,

                    m.M21,
                    m.M22,
                    m.M23,
                    m.M24,

                    m.M31,
                    m.M32,
                    m.M33,
                    m.M34,

                    m.M41,
                    m.M42,
                    m.M43,
                    m.M44);

            }
            public static Matrix4 FromFloat16(float16 m)
            {
                return new Matrix4(m.M11,
                    m.M12,
                    m.M13,
                    m.M14,

                    m.M21,
                    m.M22,
                    m.M23,
                    m.M24,

                    m.M31,
                    m.M32,
                    m.M33,
                    m.M34,

                    m.M41,
                    m.M42,
                    m.M43,
                    m.M44);
            }
        }
    }

    /// <summary>
    /// Used as a bridge between the cached animation data and the runtime animation settings
    /// </summary>
    public class AnimationRuntime
    {
        /// <summary>
        /// The handle for the animation
        /// </summary>
        public int          Identifier              = -1;
        /// <summary>
        /// The animation literal name
        /// </summary>
        public string       Name                    = string.Empty;
        /// <summary>
        /// The path to the animation file
        /// </summary>
        public string       Pointer                 = string.Empty;
        /// <summary>
        /// The step/frame period of the animation
        /// </summary>
        public float        StepSize                = 0f;
        /// <summary>
        /// The position of the last frame
        /// </summary>
        public float        LastFrame               = 0f;
        /// <summary>
        /// The current frame position of the animation 
        /// </summary>
        public float        AnimationPos            = 0f;
        /// <summary>
        /// Animation playback speed
        /// </summary>
        public float        AnimationSpeed          = 1f;
        /// <summary>
        /// The name of the next animation once the current has finished (valid onle for Playback == Once)
        /// </summary>
        public string       Playback                = "once";
        /// <summary>
        /// The name of the next animation once the current has finished (valid onle for Playback == Once)
        /// </summary>
        public string       Transition              = string.Empty;
        /// <summary>
        /// The active dataset of the bone matrices
        /// </summary>
        public Matrix4[]    ActiveMatrices          = null;                  // Reference values from meshes cache
        /// <summary>
        /// The entire animation bone data
        /// </summary>
        public Matrix4[][]  Matrices                = null;                  // Reference values from meshes cahce


        public AnimationRuntime(AnimationData frame)
        {
            this.Identifier = frame.identifier;
            this.Name = frame.name;
            this.Pointer = frame.pointer;
            this.StepSize = frame.stepSize;
            this.LastFrame = frame.lastFrame;
            this.AnimationPos = frame.animationPos;
            this.AnimationSpeed = frame.AnimationSpeed;
            this.ActiveMatrices = frame.activeMatrices;
            this.Matrices = frame.Matrices;
            this.ActiveMatrices = Matrices[0];
        }

        public AnimationRuntime(int id, string name, string pointer, float stepSize, float lastFrame, float animationPos, float animationSpeed, Matrix4[] activeMatrices, Matrix4[][] matrices) 
        {
            this.Identifier             = id;
            this.Name                   = name;
            this.Pointer                = pointer;
            this.StepSize               = stepSize;
            this.LastFrame              = lastFrame;
            this.AnimationPos           = animationPos;
            this.AnimationSpeed         = animationSpeed;
            this.ActiveMatrices         = activeMatrices;
            this.Matrices               = matrices;
            this.ActiveMatrices         = matrices[0];
        }
    }


    // DEBUGING Json Serialization Exceptions
    public class NullJsonWriter : JsonTextWriter
    {
        public NullJsonWriter(System.IO.TextWriter writer) : base(writer)
        {
        }
        public override void WriteNull()
        {
            base.WriteValue(string.Empty);
        }
    }
}
