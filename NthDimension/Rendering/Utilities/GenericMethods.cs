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

namespace NthDimension.Rendering
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using ProtoBuf;

    using NthDimension.Algebra;
    using Physics.LinearMath;
    using NthDimension.Rendering.Serialization;

    // An enumerated type for the control messages
    // sent to the handler routine.
    public enum CtrlTypes
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    public static class GenericMethods
    {
        private static string       splitChar   = "|";

        #region Byte Array Utilities

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray<T>(this T obj)
        {
            if (obj == null)
                return null;

            //MemoryBlockStream ms = new MemoryBlockStream(32768);
            MemoryStream ms = new MemoryStream();

            #region .Net Serializer

#if NET_SERIALIZER
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
#endif

            #endregion .Net Serializer

            #region Protobuf-net

            Serializer.Serialize<T>(ms, obj);

            #endregion

            #region Aqla Serializer

            //AqlaSerializer.Serializer.Serialize<T>(ms, obj);

            #endregion

            #region ZeroFormatter

            //ZeroFormatterSerializer.Serialize<T>(ms, obj);

            #endregion

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            if (memStream.Length == 0)
                throw new SerializationException("Stream size cannot be 0");

            #region .Net Serializer

#if NET_SERIALIZER
            BinaryFormatter binForm = new BinaryFormatter();
            return (Object)binForm.Deserialize(memStream);
#endif

            #endregion

            #region prtobuf-net

            return Serializer.Deserialize<T>(memStream);

            #endregion

            #region Aqla Serializer

            //AqlaSerializer.Meta.RuntimeTypeModel model = AqlaSerializer.Meta.TypeModel.Create();
            //model.Add(typeof(ListVector2),  true).ArrayLengthReadLimit = 100000000;
            //model.Add(typeof(ListVector3),  true).ArrayLengthReadLimit = 100000000;
            //model.Add(typeof(ListFace),     true).ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(20, "normalVboDataList").ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(30, "textureVboDataList").ArrayLengthReadLimit = 100000000;

            ////model.Add(typeof(Geometry.Mesh), true).Add(230, "positionVboData").ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(240, "normalVboData").ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(250, "tangentVboData").ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(260, "textureVboData").ArrayLengthReadLimit = 100000000;
            ////model.Add(typeof(Geometry.Mesh), true).Add(270, "indicesVboData").ArrayLengthReadLimit = 100000000;

            //return model.Deserialize<T>(memStream);
            ////return AqlaSerializer.Serializer.Deserialize<T>(memStream);

            #endregion

            #region ZeroFormatter

            //return ZeroFormatterSerializer.Deserialize<T>(memStream);

            #endregion

        }

        #endregion

        #region Image Utilities
        public static byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        #endregion

        #region File Utilities
        static string[] sizeAbbrv = { "B", "KB", "MB", "GB", "TB" };
        public static string FileSizeReadable(string filename)
        {
            if (!File.Exists(filename))
                return String.Format("File {0} not found", filename);

            double len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizeAbbrv.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizeAbbrv[order]);
        }
        public static string FileSizeReadable(ulong size)
        {

            double len = (double) size;
            int order = 0;
            while (len >= 1024 && order < sizeAbbrv.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizeAbbrv[order]);
        }
        #endregion

        #region XML Utilites

        public static XmlWriter CoolXMLWriter(Stream output)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";

            return XmlWriter.Create(output, xws);
        }

        #endregion

        #region NumberFormat Utilities

        public static NumberFormatInfo getNfi()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalSeparator = ".";
            return nfi;
        }

        #endregion

        #region Timespan Utilities

        #region Pretifier (ie Just Now, 5 Min ago, etc)
        private static string GetSecondsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "second";
            return string.Format("{0} seconds", v);
        }
        private static string GetMinutesText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "minute";
            return string.Format("{0} minutes", v);
        }
        private static string GetHoursText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "hour";
            return string.Format("{0} hours", v);
        }
        private static string GetDaysText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "day";
            return string.Format("{0} days", v);
        }
        private static string GetWeeksText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "week";
            return string.Format("{0} weeks", v);
        }
        private static string GetMonthsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "month";
            return string.Format("{0} months", v);
        }
        private static string GetYearsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "year";
            return string.Format("{0} years", v);
        }

        private static readonly Func<int, string>[] TextFuncArray = new Func<int, string>[] { GetYearsText, GetMonthsText, GetWeeksText, GetDaysText, GetHoursText, GetMinutesText, GetSecondsText };
        private static readonly long[] SignificanceArray = new long[] {
                3600 * 24 * 365, // year
                3600 * 24 * 30, //month
                3600 * 24 * 7, // week
                3600 * 24, // day
                3600, //hour
                60, //min
                1 // sec
            };

        /// <summary>
        /// Transform a TimeSpan into user readable Text
        /// </summary>
        /// <param name="ts">the TimeSpan object</param>
        /// <param name="partCount">how many interval parts you want? (e.g.  2years and 5 days  is 2 parts)</param>
        /// <param name="neglectionFactor">determine the significance level of a part.</param>
        /// <returns>beautiful string</returns>
        public static string Prettify(this TimeSpan ts, int partCount = 2, double neglectionFactor = 0.2)
        {
            int[] tsPartsArray = new int[7];

            int d = (int)ts.TotalDays;
            tsPartsArray[0] = d / 365;
            d %= 365;
            tsPartsArray[1] = (int)(d / 30.41);
            d %= 30;
            tsPartsArray[2] = (int)(d / 7.5);
            d %= 7;
            tsPartsArray[3] = d;

            tsPartsArray[4] = ts.Hours;
            tsPartsArray[5] = ts.Minutes;
            tsPartsArray[6] = ts.Seconds;

            long totalSignificance = 1;
            int partIdx = 0;
            string[] parts = new string[partCount];
            for (int i = 0; i < tsPartsArray.Length; i++)
            {
                if (tsPartsArray[i] > 0)
                {
                    long significance = SignificanceArray[i] * tsPartsArray[i];

                    if ((significance / (double)totalSignificance) < neglectionFactor)
                    {
                        break;
                    }

                    totalSignificance += significance;

                    parts[partIdx] = TextFuncArray[i](tsPartsArray[i]);
                    if (++partIdx >= parts.Length) // no new parts are needed
                        break;

                }
            }

            if (partIdx == 0)
                return "Just Now";
            if (partIdx == 1)
                return parts[0];

            return string.Join(" ", parts, 0, partIdx - 1) + " ו" + parts[partIdx - 1];
        }

        #endregion


        #region To HumanReadable (for 

        //public static string ToHumanReadable(this TimeSpan ts)
        //{
        //    var parts = string
        //        .Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s:{4:D3}ms",
        //            ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)
        //        .Split(':')
        //        .SkipWhile(s => Regex.Match(s, @"00\w").Success) // skip zero-valued components
        //        .ToArray();
        //    return string.Join(" ", parts); // combine the result
        //}

        public static string ToHumanReadable(this TimeSpan t)
        {
            string answer;

            if (t.TotalSeconds < 1.0)
            {
                answer = string.Format("{0:D3} ms", t.Milliseconds);
            }
            else if (t.TotalMinutes < 1.0)
            {
                answer = String.Format("{0} sec {1:D3} msec", t.Seconds, t.Milliseconds);
            }
            else if (t.TotalHours < 1.0)
            {
                answer = String.Format("{0} min {1:D2} sec {2:D3} ms", t.Minutes, t.Seconds, t.Milliseconds);
            }
            else // more than 1 hour
            {
                answer = String.Format("{0} hrs {1:D2} min {2:D2} sec {3:D3} ms", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds);
            }

            return answer;
        }

        public static string ToHumanReadable(this Stopwatch s)
        {
            string answer;

            TimeSpan t = s.Elapsed;

            if (t.TotalSeconds < 1.0 )
            {
                answer = string.Format("{0} ms", t.Milliseconds);
            }
            else if (t.TotalMinutes < 1.0)
            {
                answer = String.Format("{0} sec", t.Seconds, t.Milliseconds);
            }
            else if (t.TotalHours < 1.0)
            {
                answer = String.Format("{0} min {1:D2} sec", t.Minutes, t.Seconds, t.Milliseconds);
            }
            else // more than 1 hour
            {
                answer = String.Format("{0} hrs {1:D2} min {2:D2} sec", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds);
            }

            return answer;
        }
        #endregion

        #endregion

        #region String Utilities

        public static string tabify(int amnt)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < amnt; i++)
            {
                sb.Append("\t");
            }
            return sb.ToString();
        }

        public static string StringFromFloat(float mFloat)
        {
            NumberFormatInfo nfi = getNfi();
            return mFloat.ToString(nfi);
        }

        public static string StringFromVector3(Vector3 mVec)
        {
            NumberFormatInfo nfi = getNfi();
            string mString =
                mVec.X.ToString(nfi) + splitChar +
                mVec.Y.ToString(nfi) + splitChar +
                mVec.Z.ToString(nfi);

            return mString;

        }

        public static string StringFromVector4(Vector4 mVec)
        {
            NumberFormatInfo nfi = getNfi();
            string mString =
                mVec.X.ToString(nfi) + splitChar +
                mVec.Y.ToString(nfi) + splitChar +
                mVec.Z.ToString(nfi) + splitChar +
                mVec.W.ToString(nfi);

            return mString;

        }

        public static string StringFromJMatrix(JMatrix mMat)
        {
            NumberFormatInfo nfi = getNfi();
            string mString =
                mMat.M11.ToString(nfi) + splitChar +
                mMat.M12.ToString(nfi) + splitChar +
                mMat.M13.ToString(nfi) + splitChar +
                mMat.M21.ToString(nfi) + splitChar +
                mMat.M22.ToString(nfi) + splitChar +
                mMat.M23.ToString(nfi) + splitChar +
                mMat.M31.ToString(nfi) + splitChar +
                mMat.M32.ToString(nfi) + splitChar +
                mMat.M33.ToString(nfi);

            return mString;

        }

        public static string StringFromStringList(List<string> stringAryList)
        {
            if (stringAryList.Count > 0)
            {
                StringWriter sw = new StringWriter();
                for (int i = 0; i < stringAryList.Count - 1; i++)
                {
                    //sw.Write(stringAryList[i]);
                    sw.Write(stringAryList[i].Trim());
                    sw.Write(splitChar);
                    sw.Write(Environment.NewLine);
                }
                sw.Write(stringAryList[stringAryList.Count - 1]);

                return sw.ToString();
            }
            else
            {
                return "";
            }
        }

        public static List<string> StringListFromString(string mString)
        {
            string[] splitString = mString.Split(splitChar.ToCharArray());
            List<string> stringList = new List<string> {};
            foreach (var curString in splitString)
            {
                stringList.Add(curString.Replace("\n", "").Replace("\r", "").Replace("\t",""));
            }
            return stringList;
            {

            }
        }

        public static JMatrix JMatrixFromString(string mString)
        {
            NumberFormatInfo nfi = GenericMethods.getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            JMatrix mMat = new JMatrix(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi),
                Single.Parse(fields[3], nfi),
                Single.Parse(fields[4], nfi),
                Single.Parse(fields[5], nfi),
                Single.Parse(fields[6], nfi),
                Single.Parse(fields[7], nfi),
                Single.Parse(fields[8], nfi));

            return mMat;
        }

        #endregion

        #region Int Utilities

        internal static int IntFromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            return Int32.Parse(mString, nfi);
        }

        #endregion

        #region Float Utilities
        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;

            if (value >= max)
                return max;

            return value;
        }

        public static float FloatFromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            return Single.Parse(mString, nfi);
        }

        internal static float[] FloatAryFromStringAry(string[] tmpAry)
        {
            int tmpLenth = tmpAry.Length;
            //float[] floatAry = new float[tmpLenth];
            List<float> ret = new List<float>();


            for (int i = 0; i < tmpLenth; i++)
            {
                if (tmpAry[i] == String.Empty)
                    continue;

                //floatAry[i] = GenericMethods.FloatFromString(tmpAry[i]);
                ret.Add(GenericMethods.FloatFromString(tmpAry[i]));
            }
            //return floatAry;
            return ret.ToArray();
        }

        public static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= Single.Epsilon));
        }

        public static bool nearlyEqual(float a, float b)
        {
            //float epsilon = float.Epsilon;
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < float.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < float.Epsilon;// (epsilon);
            }
            else
            { // use relative error
                return diff/(absA + absB) < float.Epsilon; //epsilon;
            }
        }
        #endregion

        #region Vector2 Utilities
        public static float estSize(Vector2 vec)
        {
            vec.X *= Math.Sign(vec.X);
            vec.Y *= Math.Sign(vec.Y);

            return vec.X + vec.Y;
        }
        internal static ListVector2 FlipY(ListVector2 list)
        {
            ListVector2 outVec = new ListVector2 {};
            foreach (var vec in list)
            {
                outVec.Add(new Vector2(vec.X, 1.0f - vec.Y));
            }
            return outVec;
        }

        public static Vector2 Vector2FromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            return new Vector2(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi));
        }
        #endregion

        #region Vector3 Utilities
        public static float estSize(Vector3 vec)
        {
            vec.X *= Math.Sign(vec.X);
            vec.Y *= Math.Sign(vec.Y);
            vec.Z *= Math.Sign(vec.Z);

            return vec.X + vec.Y + vec.Z;
        }
        public static Vector3 Vector3FromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            return new Vector3(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi));
        }
      

        public static JVector FromOpenTKVector(Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        public static Vector3 ToOpenTKVector(JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y) + (v2.Z - v1.Z) * (v2.Z - v1.Z));
        }

        public static Vector3 Subtract(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            Vector3 ret = new Vector3(v1);
            if (v2.X < ret.X)
                ret.X = v2.X;

            if (v2.Y < ret.Y)
                ret.Y = v2.Y;

            if (v2.Z < ret.Z)
                ret.Z = v2.Z;

            return ret;
        }
        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            Vector3 ret = new Vector3(v1);
            if (v2.X > ret.X)
                ret.X = v2.X;

            if (v2.Y > ret.Y)
                ret.Y = v2.Y;

            if (v2.Z > ret.Z)
                ret.Z = v2.Z;

            return ret;
        }

        public static float AngleXZ(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Atan2(v2.Z - v1.Z, v2.X - v1.X);
        }

        public static Vector3 BaryCenter(Vector3[] points)
        {
            Vector3 centerOfMass = new Vector3(0, 0, 0);

            foreach (Vector3 v in points)
                centerOfMass += v;

            centerOfMass.X /= points.Length;
            centerOfMass.Y /= points.Length;
            centerOfMass.Z /= points.Length;

            return centerOfMass;
        }

        public static Vector3d ToVector3d(this Vector3 vec)
        {
            return new Vector3d(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToVector3f(this Vector3d vec)
        {
            return new Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        }

        public static Vector3 ToLeftHanded(this Vector3 rightHandedVector)
        {
            return new Vector3(rightHandedVector.X, rightHandedVector.Z, rightHandedVector.Y);
        }
        #endregion

        #region Vector4 Utilities

        public static Vector4 Mult(Matrix4 m, Vector4 v)
        {
            Vector4 result = new Vector4();

            result.X = m.M11*v.X + m.M12*v.Y + m.M13*v.Z + m.M14*v.W;
            result.Y = m.M21*v.X + m.M22*v.Y + m.M23*v.Z + m.M24*v.W;
            result.Z = m.M31*v.X + m.M32*v.Y + m.M33*v.Z + m.M34*v.W;
            result.W = m.M41*v.X + m.M42*v.Y + m.M43*v.Z + m.M44*v.W;

            return result;
        }


        public static Vector4 Mult(Vector4 v, Matrix4 m)
        {
            Vector4 result = new Vector4();

            result.X = m.M11*v.X + m.M21*v.Y + m.M31*v.Z + m.M41*v.W;
            result.Y = m.M12*v.X + m.M22*v.Y + m.M32*v.Z + m.M42*v.W;
            result.Z = m.M13*v.X + m.M23*v.Y + m.M33*v.Z + m.M43*v.W;
            result.W = m.M14*v.X + m.M24*v.Y + m.M34*v.Z + m.M44*v.W;

            return result;
        }

        public static Color4 ToColor4(this Vector4 color)
        {
            return new Color4(color.X, color.Y, color.Z, color.W);
        }

        public static Vector4 Vector4FromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            return new Vector4(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi),
                Single.Parse(fields[3], nfi));
        }

        #endregion

        #region Matrix4 Utilities

        public static Matrix4 Matrix4Zero = new Matrix4(
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0);

        public static float[] ToArray(this Matrix4 m)
        {
            float[] arr = new float[16];
            arr[0] = m.M11;
            arr[1] = m.M12;
            arr[2] = m.M13;
            arr[3] = m.M14;
            arr[4] = m.M21;
            arr[5] = m.M22;
            arr[6] = m.M23;
            arr[7] = m.M24;
            arr[8] = m.M31;
            arr[9] = m.M32;
            arr[10] = m.M33;
            arr[11] = m.M34;
            arr[12] = m.M41;
            arr[13] = m.M42;
            arr[14] = m.M43;
            arr[15] = m.M44;

            return arr;
        }

        public static Matrix4 BlendMatrix(Matrix4 matA, Matrix4 matB, float weight)
        {
            Matrix4 matR = new Matrix4();

            matR.M11 = matA.M11*weight + matB.M11*(1 - weight);
            matR.M12 = matA.M12*weight + matB.M12*(1 - weight);
            matR.M13 = matA.M13*weight + matB.M13*(1 - weight);
            matR.M14 = matA.M14*weight + matB.M14*(1 - weight);

            matR.M21 = matA.M21*weight + matB.M21*(1 - weight);
            matR.M22 = matA.M22*weight + matB.M22*(1 - weight);
            matR.M23 = matA.M23*weight + matB.M23*(1 - weight);
            matR.M24 = matA.M24*weight + matB.M24*(1 - weight);

            matR.M31 = matA.M31*weight + matB.M31*(1 - weight);
            matR.M32 = matA.M32*weight + matB.M32*(1 - weight);
            matR.M33 = matA.M33*weight + matB.M33*(1 - weight);
            matR.M34 = matA.M34*weight + matB.M34*(1 - weight);

            matR.M41 = matA.M41*weight + matB.M41*(1 - weight);
            matR.M42 = matA.M42*weight + matB.M42*(1 - weight);
            matR.M43 = matA.M43*weight + matB.M43*(1 - weight);
            matR.M44 = matA.M44*weight + matB.M44*(1 - weight);

            return matR;
        }

        internal static Matrix4 MatrixFromVector(JVector normal)
        {
            float rotationY = (float) Math.Atan2(normal.X, normal.Z);

            Matrix4 tmpMatA = Matrix4.Identity;
            //ConsoleUtil.log(rotationZ);
            if (normal.Y < 0.99 && normal.Y > -0.99)
                tmpMatA = Matrix4.CreateRotationY(rotationY);

            Vector4 secondaryVec = MathHelper.Mult(tmpMatA, new Vector4(ToOpenTKVector(normal), 1));
            float rotationX = (float) Math.Atan2(secondaryVec.Z, secondaryVec.Y);
            Matrix4 tmpMatB = Matrix4.CreateRotationX(rotationX);

            return Matrix4.Mult(tmpMatB, tmpMatA);
        }

        internal static Matrix4 MatrixFromVector(Vector3 normal)
        {
            float rotationY = (float) Math.Atan2(normal.X, normal.Z);

            Matrix4 tmpMatA = Matrix4.Identity;
            //ConsoleUtil.log(rotationZ);
            if (normal.Y < 0.99 && normal.Y > -0.99)
                tmpMatA = Matrix4.CreateRotationY(rotationY);

            Vector4 secondaryVec = MathHelper.Mult(tmpMatA, new Vector4(normal, 1));
            float rotationX = (float) Math.Atan2(secondaryVec.Z, secondaryVec.Y);
            Matrix4 tmpMatB = Matrix4.CreateRotationX(rotationX);

            return Matrix4.Mult(tmpMatB, tmpMatA);
        }

        public static Matrix4 Matrix4FromArray(float[] ary)
        {
            Matrix4 tmpMat = new Matrix4();
            tmpMat.M11 = ary[0];
            tmpMat.M12 = ary[1];
            tmpMat.M13 = ary[2];
            tmpMat.M14 = ary[3];

            tmpMat.M21 = ary[4];
            tmpMat.M22 = ary[5];
            tmpMat.M23 = ary[6];
            tmpMat.M24 = ary[7];

            tmpMat.M31 = ary[8];
            tmpMat.M32 = ary[9];
            tmpMat.M33 = ary[10];
            tmpMat.M34 = ary[11];

            tmpMat.M41 = ary[12];
            tmpMat.M42 = ary[13];
            tmpMat.M43 = ary[14];
            tmpMat.M44 = ary[15];

            return tmpMat;
        }

        public static Matrix4 Matrix4FromString(string mString)
        {
            NumberFormatInfo nfi = GenericMethods.getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            Matrix4 mMat = new Matrix4(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi),
                Single.Parse(fields[3], nfi),
                Single.Parse(fields[4], nfi),
                Single.Parse(fields[5], nfi),
                Single.Parse(fields[6], nfi),
                Single.Parse(fields[7], nfi),
                Single.Parse(fields[8], nfi),
                Single.Parse(fields[9], nfi),
                Single.Parse(fields[10], nfi),
                Single.Parse(fields[11], nfi),
                Single.Parse(fields[12], nfi),
                Single.Parse(fields[13], nfi),
                Single.Parse(fields[14], nfi),
                Single.Parse(fields[15], nfi)
                );

            return mMat;
        }

        public static Matrix4 ToOpenTKMatrix(JMatrix matrix)
        {
            return new Matrix4(matrix.M11,
                matrix.M12,
                matrix.M13,
                0.0f,
                matrix.M21,
                matrix.M22,
                matrix.M23,
                0.0f,
                matrix.M31,
                matrix.M32,
                matrix.M33,
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static JMatrix FromOpenTKMatrix(Matrix4 matrix)
        {
            return new JMatrix(matrix.M11,
                matrix.M12,
                matrix.M13,
                matrix.M21,
                matrix.M22,
                matrix.M23,
                matrix.M31,
                matrix.M32,
                matrix.M33);
        }

        public static void TransformVector(this Matrix4 mat, ref Vector3 pV)
        {
            /* See this implementation also
             * 
             * func (m *Matrix3) TransformVector3(v *Vector3) *Vector3 {
	            newVec := &Vector3{}
	            newVec[0] = v[0]*m[0] + v[1]*m[1] + v[2] + m[2]
	            newVec[1] = v[0]*m[3] + v[1]*m[4] + v[2] + m[5]
	            newVec[2] = v[0]*m[6] + v[1]*m[7] + v[2] + m[8]
	            return newVec 
             * }
             */


            float auxX, auxY, auxZ;
            float[] arrayMat = mat.ToArray();

            float inverseW = 1.0f / (arrayMat[3] + arrayMat[7] + arrayMat[11] + arrayMat[15]);
            auxX = ((arrayMat[0] * pV.X) + (arrayMat[4] * pV.Y) + (arrayMat[8] * pV.Z) + arrayMat[12]) * inverseW;
            auxY = ((arrayMat[1] * pV.X) + (arrayMat[5] * pV.Y) + (arrayMat[9] * pV.Z) + arrayMat[13]) * inverseW;
            auxZ = ((arrayMat[2] * pV.X) + (arrayMat[6] * pV.Y) + (arrayMat[10] * pV.Z) + arrayMat[14]) * inverseW;

            pV.X = auxX;
            pV.Y = auxY;
            pV.Z = auxZ;
        }

        //Column Major
        public static float m0(Matrix4 m) { return m.Row0.X; }
        public static void m0(ref Matrix4 m, float value) { m.Row0.X = value; }

        public static float m1(Matrix4 m) { return m.Row1.X; }
        public static void m1(ref Matrix4 m, float value) { m.Row1.X = value; }

        public static float m2(Matrix4 m) { return m.Row2.X; }
        public static void m2(ref Matrix4 m, float value) { m.Row2.X = value; }

        public static float m3(Matrix4 m) { return m.Row3.X; }
        public static void m3(ref Matrix4 m, float value) { m.Row3.X = value; }

        public static float m4(Matrix4 m) { return m.Row0.Y; }
        public static void m4(ref Matrix4 m, float value) { m.Row0.Y = value; }

        public static float m5(Matrix4 m) { return m.Row1.Y; }
        public static void m5(ref Matrix4 m, float value) { m.Row1.Y = value; }

        public static float m6(Matrix4 m) { return m.Row2.Y; }
        public static void m6(ref Matrix4 m, float value) { m.Row2.Y = value; }

        public static float m7(Matrix4 m) { return m.Row3.Y; }
        public static void m7(ref Matrix4 m, float value) { m.Row2.Y = value; }

        public static float m8(Matrix4 m) { return m.Row0.Z; }
        public static void m8(ref Matrix4 m, float value) { m.Row0.Z = value; }

        public static float m9(Matrix4 m) { return m.Row1.Z; }
        public static void m9(ref Matrix4 m, float value) { m.Row1.Z = value; }

        public static float m10(Matrix4 m) { return m.Row2.Z; }
        public static void m10(ref Matrix4 m, float value) { m.Row2.Z = value; }

        public static float m11(Matrix4 m) { return m.Row3.Z; }
        public static void m11(ref Matrix4 m, float value) { m.Row3.Z = value; }

        public static float m12(Matrix4 m) { return m.Row0.W; }
        public static void m12(ref Matrix4 m, float value) { m.Row0.W = value; }

        public static float m13(Matrix4 m) { return m.Row1.W; }
        public static void m13(ref Matrix4 m, float value) { m.Row1.W = value; }

        public static float m14(Matrix4 m) { return m.Row2.W; }
        public static void m14(ref Matrix4 m, float value) { m.Row2.W = value; }

        public static float m15(Matrix4 m) { return m.Row3.W; }
        public static void m15(ref Matrix4 m, float value) { m.Row3.W = value; }

        public static float M11(Matrix4 m) { return m.Row0.X; }
        public static void M11(ref Matrix4 m, float value) { m.Row0.X = value; }

        public static float M12(Matrix4 m) { return m.Row0.Y; }
        public static void M12(ref Matrix4 m, float value) { m.Row0.Y = value; }

        public static float M13(Matrix4 m) { return m.Row0.Z; }
        public static void M13(ref Matrix4 m, float value) { m.Row0.Z = value; }

        public static float M14(Matrix4 m) { return m.Row0.W; }
        public static void M14(ref Matrix4 m, float value) { m.Row0.W = value; }

        public static float M21(Matrix4 m) { return m.Row1.X; }
        public static void M21(ref Matrix4 m, float value) { m.Row1.X = value; }

        public static float M22(Matrix4 m) { return m.Row1.Y; }
        public static void M22(ref Matrix4 m, float value) { m.Row1.Y = value; }

        public static float M23(Matrix4 m) { return m.Row1.Z; }
        public static void M23(ref Matrix4 m, float value) { m.Row1.Z = value; }

        public static float M24(Matrix4 m) { return m.Row1.W; }
        public static void M24(ref Matrix4 m, float value) { m.Row1.W = value; }

        public static float M31(Matrix4 m) { return m.Row2.X; }
        public static void M31(ref Matrix4 m, float value) { m.Row2.X = value; }

        public static float M32(Matrix4 m) { return m.Row2.Y; }
        public static void M32(ref Matrix4 m, float value) { m.Row2.Y = value; }

        public static float M33(Matrix4 m) { return m.Row2.Z; }
        public static void M33(ref Matrix4 m, float value) { m.Row2.Z = value; }

        public static float M34(Matrix4 m) { return m.Row2.W; }
        public static void M34(ref Matrix4 m, float value) { m.Row2.W = value; }

        public static float M41(Matrix4 m) { return m.Row3.X; }
        public static void M41(ref Matrix4 m, float value) { m.Row3.X = value; }

        public static float M42(Matrix4 m) { return m.Row3.Y; }
        public static void M42(ref Matrix4 m, float value) { m.Row3.Y = value; }

        public static float M43(Matrix4 m) { return m.Row3.Z; }
        public static void M43(ref Matrix4 m, float value) { m.Row3.Z = value; }

        public static float M44(Matrix4 m) { return m.Row3.W; }
        public static void M44(ref Matrix4 m, float value) { m.Row3.W = value; }

        public static Matrix4 FromQuaternion(Quaternion quat)
        {
            quat.Normalize();
            Matrix4 mat = new Matrix4();
            mat.M11 = 1 - 2 * (quat.Y * quat.Y) - 2 * (quat.Z * quat.Z);
            mat.M12 = 2 * (quat.X * quat.Y) - 2 * (quat.Z * quat.W);
            mat.M13 = 2 * (quat.X * quat.Z) + 2 * (quat.Y * quat.W);

            mat.M21 = 2 * (quat.X * quat.Y) + 2 * (quat.Z * quat.W);
            mat.M22 = 1 - 2 * (quat.X * quat.X) - 2 * (quat.Z * quat.Z);
            mat.M23 = 2 * (quat.Y * quat.Z) + 2 * (quat.X * quat.W);

            mat.M31 = 2 * (quat.X * quat.Z) + 2 * (quat.Y * quat.W);
            mat.M32 = 2 * (quat.Y * quat.Z) + 2 * (quat.X * quat.W);
            mat.M32 = 1 - 2 * (quat.X * quat.X) - 2 * (quat.Y * quat.Y);

            mat.Transpose();
            return mat;
        }

        public static void SetRotationRadians(ref Matrix4 mat, Vector3 angles)
        {
            double cr = System.Math.Cos(angles.X);
            double sr = System.Math.Sin(angles.X);
            double cp = System.Math.Cos(angles.Y);
            double sp = System.Math.Sin(angles.Y);
            double cy = System.Math.Cos(angles.Z);
            double sy = System.Math.Sin(angles.Z);

            mat.M11 = (float)(cp * cy);
            mat.M12 = (float)(cp * sy);
            mat.M13 = (float)(-sp);
            mat.M14 = (float)(0.0f);

            double srsp = sr * sp;
            double crsp = cr * sp;

            mat.M21 = (float)(srsp * cy - cr * sy);
            mat.M22 = (float)(srsp * sy + cr * cy);
            mat.M23 = (float)(sr * cp);

            mat.M31 = (float)(crsp * cy + sr * sy);
            mat.M32 = (float)(crsp * sy - sr * cy);
            mat.M34 = (float)(cr * cp);
        }

        public static void SetTranslation(ref Matrix4 mat, Vector3 translation)
        {
            mat.M41 = translation.X;
            mat.M42 = translation.Y;
            mat.M43 = translation.Z;
        }

        #endregion

        #region Quaternion Utilities

        public static Vector3 GetPitchYawRollRad(Quaternion q)
        {
            float roll  = (float) Math.Atan2(2*q.Y*q.W - 2*q.X*q.Z, 1 - 2*q.Y*q.Y - 2*q.Z*q.Z);
            float pitch = (float) Math.Atan2(2*q.X*q.W - 2*q.Y*q.Z, 1 - 2*q.X*q.X - 2*q.Z*q.Z);
            float yaw   = (float) Math.Asin(2*q.X*q.Y + 2*q.Z*q.W);

            return new Vector3(pitch, yaw, roll);
        }

        public static Vector3 GetPitchYawRollDeg(Quaternion q)
        {
            Vector3 ret = GetPitchYawRollRad(q);
            return new Vector3(MathHelper.RadiansToDegrees(ret.X),
                               MathHelper.RadiansToDegrees(ret.Y),
                               MathHelper.RadiansToDegrees(ret.Z));
        }

        public static Quaternion ToLeftHanded(this Quaternion rightHandedQuaternion)
        {
            return new Quaternion(-rightHandedQuaternion.X,
                                  -rightHandedQuaternion.Z,
                                  -rightHandedQuaternion.Y,
                                  rightHandedQuaternion.W);
        }
        #endregion

        #region JVector Utilities

        public static List<JVector> FromOpenTKVecArToJVecList(Vector3[] vectors)
        {
            List<JVector> mList = new List<JVector> {};
            foreach (Vector3 vector in vectors)
            {
                mList.Add(FromOpenTKVector(vector));
            }
            return mList;
        }

        #endregion

        #region MathHelper
        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static float Hypot(float x, float y)
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        #endregion
    }

}
