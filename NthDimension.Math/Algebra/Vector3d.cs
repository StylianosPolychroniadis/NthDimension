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
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace NthDimension.Algebra
{
    /// <summary>
    /// Represents a 3D vector using three double-precision floating-point numbers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3d : IEquatable<Vector3d>
    {
        #region Fields

        /// <summary>
        /// The X component of the Vector3.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the Vector3.
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z component of the Vector3.
        /// </summary>
        public double Z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="value">The value that will initialize this instance.</param>
        public Vector3d(double value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Constructs a new Vector3.
        /// </summary>
        /// <param name="x">The x component of the Vector3.</param>
        /// <param name="y">The y component of the Vector3.</param>
        /// <param name="z">The z component of the Vector3.</param>
        public Vector3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a new instance from the given Vector2d.
        /// </summary>
        /// <param name="v">The Vector2d to copy components from.</param>
        public Vector3d(Vector2d v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0.0f;
        }

        /// <summary>
        /// Constructs a new instance from the given Vector3d.
        /// </summary>
        /// <param name="v">The Vector3d to copy components from.</param>
        public Vector3d(Vector3d v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Constructs a new instance from the given Vector4d.
        /// </summary>
        /// <param name="v">The Vector4d to copy components from.</param>
        public Vector3d(Vector4d v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }


        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets the value at the index of the Vector.
        /// </summary>
        public double this[int index] {
            get{
                if(index == 0) return X;
                else if(index == 1) return Y;
                else if(index == 2) return Z;
                throw new IndexOutOfRangeException("You tried to access this vector at index: " + index);
            } set{
                if(index == 0) X = value;
                else if(index == 1) Y = value;
                else if(index == 2) Z = value;
                else throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
            }
        }

        #region Instance

        #region public void Add()

        /// <summary>Add the Vector passed as parameter to this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        //[CLSCompliant(false)]
        [Obsolete("Use static Add() method instead.")]
        public void Add(Vector3d right)
        {
            this.X += right.X;
            this.Y += right.Y;
            this.Z += right.Z;
        }

        /// <summary>Add the Vector passed as parameter to this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        //[CLSCompliant(false)]
        [Obsolete("Use static Add() method instead.")]
        public void Add(ref Vector3d right)
        {
            this.X += right.X;
            this.Y += right.Y;
            this.Z += right.Z;
        }

        #endregion public void Add()

        #region public void Sub()

        /// <summary>Subtract the Vector passed as parameter from this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        //[CLSCompliant(false)]
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(Vector3d right)
        {
            this.X -= right.X;
            this.Y -= right.Y;
            this.Z -= right.Z;
        }

        /// <summary>Subtract the Vector passed as parameter from this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        //[CLSCompliant(false)]
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(ref Vector3d right)
        {
            this.X -= right.X;
            this.Y -= right.Y;
            this.Z -= right.Z;
        }

        #endregion public void Sub()

        #region public void Mult()

        /// <summary>Multiply this instance by a scalar.</summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Mult(double f)
        {
            this.X *= f;
            this.Y *= f;
            this.Z *= f;
        }

        #endregion public void Mult()

        #region public void Div()

        /// <summary>Divide this instance by a scalar.</summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Divide() method instead.")]
        public void Div(double f)
        {
            double mult = 1.0 / f;
            this.X *= mult;
            this.Y *= mult;
            this.Z *= mult;
        }

        #endregion public void Div()

        #region public double Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthFast"/>
        /// <seealso cref="LengthSquared"/>
        public double Length
        {
            get
            {
                return System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        #endregion

        #region public double LengthFast

        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthSquared"/>
        public double LengthFast
        {
            get
            {
                return 1.0 / MathFunc.InverseSqrtFast(X * X + Y * Y + Z * Z);
            }
        }

        #endregion

        #region public double LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthFast"/>
        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        #endregion

        /// <summary>
        /// Returns a copy of the Vector3d scaled to unit length.
        /// </summary>
        /// <returns></returns>
        public Vector3d Normalized()
        {
            Vector3d v = this;
            v.Normalize();
            return v;
        }

        #region public void Normalize()

        /// <summary>
        /// Scales the Vector3d to unit length.
        /// </summary>
        public void Normalize()
        {
            double scale = 1.0 / this.Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the Vector3d to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            double scale = MathFunc.InverseSqrtFast(X * X + Y * Y + Z * Z);
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        #endregion

        #region public void Scale()

        /// <summary>
        /// Scales the current Vector3d by the given amounts.
        /// </summary>
        /// <param name="sx">The scale of the X component.</param>
        /// <param name="sy">The scale of the Y component.</param>
        /// <param name="sz">The scale of the Z component.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(double sx, double sy, double sz)
        {
            this.X = X * sx;
            this.Y = Y * sy;
            this.Z = Z * sz;
        }

        /// <summary>Scales this instance by the given parameter.</summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        //[CLSCompliant(false)]
        public void Scale(Vector3d scale)
        {
            this.X *= scale.X;
            this.Y *= scale.Y;
            this.Z *= scale.Z;
        }

        /// <summary>Scales this instance by the given parameter.</summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        //[CLSCompliant(false)]
        public void Scale(ref Vector3d scale)
        {
            this.X *= scale.X;
            this.Y *= scale.Y;
            this.Z *= scale.Z;
        }

        #endregion public void Scale()

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length Vector3d that points towards the X-axis.
        /// </summary>
        public static readonly Vector3d UnitX = new Vector3d(1, 0, 0);

        /// <summary>
        /// Defines a unit-length Vector3d that points towards the Y-axis.
        /// </summary>
        public static readonly Vector3d UnitY = new Vector3d(0, 1, 0);

        /// <summary>
        /// /// Defines a unit-length Vector3d that points towards the Z-axis.
        /// </summary>
        public static readonly Vector3d UnitZ = new Vector3d(0, 0, 1);

        /// <summary>
        /// Defines a zero-length Vector3.
        /// </summary>
        public static readonly Vector3d Zero = new Vector3d(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly Vector3d One = new Vector3d(1, 1, 1);

        /// <summary>
        /// Defines the size of the Vector3d struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector3d());

        #endregion

        #region Obsolete

        #region Sub

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        [Obsolete("Use static Subtract() method instead.")]
        public static Vector3d Sub(Vector3d a, Vector3d b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            a.Z -= b.Z;
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        [Obsolete("Use static Subtract() method instead.")]
        public static void Sub(ref Vector3d a, ref Vector3d b, out Vector3d result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the multiplication</returns>
        [Obsolete("Use static Multiply() method instead.")]
        public static Vector3d Mult(Vector3d a, double f)
        {
            a.X *= f;
            a.Y *= f;
            a.Z *= f;
            return a;
        }

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the multiplication</param>
        [Obsolete("Use static Multiply() method instead.")]
        public static void Mult(ref Vector3d a, double f, out Vector3d result)
        {
            result.X = a.X * f;
            result.Y = a.Y * f;
            result.Z = a.Z * f;
        }

        #endregion

        #region Div

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the division</returns>
        [Obsolete("Use static Divide() method instead.")]
        public static Vector3d Div(Vector3d a, double f)
        {
            double mult = 1.0 / f;
            a.X *= mult;
            a.Y *= mult;
            a.Z *= mult;
            return a;
        }

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the division</param>
        [Obsolete("Use static Divide() method instead.")]
        public static void Div(ref Vector3d a, double f, out Vector3d result)
        {
            double mult = 1.0 / f;
            result.X = a.X * mult;
            result.Y = a.Y * mult;
            result.Z = a.Z * mult;
        }

        #endregion

        #endregion

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        public static Vector3d Add(Vector3d a, Vector3d b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <param name="result">Result of operation.</param>
        public static void Add(ref Vector3d a, ref Vector3d b, out Vector3d result)
        {
            result = new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        public static Vector3d Subtract(Vector3d a, Vector3d b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        public static void Subtract(ref Vector3d a, ref Vector3d b, out Vector3d result)
        {
            result = new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        #endregion

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static Vector3d Multiply(Vector3d vector, double scale)
        {
            Multiply(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref Vector3d vector, double scale, out Vector3d result)
        {
            result = new Vector3d(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static Vector3d Multiply(Vector3d vector, Vector3d scale)
        {
            Multiply(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref Vector3d vector, ref Vector3d scale, out Vector3d result)
        {
            result = new Vector3d(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
        }

        #endregion

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static Vector3d Divide(Vector3d vector, double scale)
        {
            Divide(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref Vector3d vector, double scale, out Vector3d result)
        {
            Multiply(ref vector, 1 / scale, out result);
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static Vector3d Divide(Vector3d vector, Vector3d scale)
        {
            Divide(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divide a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref Vector3d vector, ref Vector3d scale, out Vector3d result)
        {
            result = new Vector3d(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z);
        }

        #endregion

        #region ComponentMin

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The component-wise minimum</returns>
        public static Vector3d ComponentMin(Vector3d a, Vector3d b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise minimum</param>
        public static void ComponentMin(ref Vector3d a, ref Vector3d b, out Vector3d result)
        {
            result.X = a.X < b.X ? a.X : b.X;
            result.Y = a.Y < b.Y ? a.Y : b.Y;
            result.Z = a.Z < b.Z ? a.Z : b.Z;
        }

        #endregion

        #region ComponentMax

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The component-wise maximum</returns>
        public static Vector3d ComponentMax(Vector3d a, Vector3d b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise maximum</param>
        public static void ComponentMax(ref Vector3d a, ref Vector3d b, out Vector3d result)
        {
            result.X = a.X > b.X ? a.X : b.X;
            result.Y = a.Y > b.Y ? a.Y : b.Y;
            result.Z = a.Z > b.Z ? a.Z : b.Z;
        }

        #endregion

        #region Min

        /// <summary>
        /// Returns the Vector3d with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>The minimum Vector3</returns>
        public static Vector3d Min(Vector3d left, Vector3d right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion

        #region Max

        /// <summary>
        /// Returns the Vector3d with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>The minimum Vector3</returns>
        public static Vector3d Max(Vector3d left, Vector3d right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>The clamped vector</returns>
        public static Vector3d Clamp(Vector3d vec, Vector3d min, Vector3d max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            vec.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(ref Vector3d vec, ref Vector3d min, ref Vector3d max, out Vector3d result)
        {
            result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            result.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static Vector3d Normalize(Vector3d vec)
        {
            double scale = 1.0 / vec.Length;
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref Vector3d vec, out Vector3d result)
        {
            double scale = 1.0 / vec.Length;
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
            result.Z = vec.Z * scale;
        }

        #endregion

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static Vector3d NormalizeFast(Vector3d vec)
        {
            double scale = MathFunc.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void NormalizeFast(ref Vector3d vec, out Vector3d result)
        {
            double scale = MathFunc.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
            result.Z = vec.Z * scale;
        }

        #endregion

        #region Dot

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The dot product of the two inputs</returns>
        public static double Dot(Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The dot product of the two inputs</param>
        public static void Dot(ref Vector3d left, ref Vector3d right, out double result)
        {
            result = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        #endregion

        #region Cross

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The cross product of the two inputs</returns>
        public static Vector3d Cross(Vector3d left, Vector3d right)
        {
            Vector3d result;
            Cross(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The cross product of the two inputs</returns>
        /// <param name="result">The cross product of the two inputs</param>
        public static void Cross(ref Vector3d left, ref Vector3d right, out Vector3d result)
        {
            result = new Vector3d(left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vector3d Lerp(Vector3d a, Vector3d b, double blend)
        {
            a.X = blend * (b.X - a.X) + a.X;
            a.Y = blend * (b.Y - a.Y) + a.Y;
            a.Z = blend * (b.Z - a.Z) + a.Z;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref Vector3d a, ref Vector3d b, double blend, out Vector3d result)
        {
            result.X = blend * (b.X - a.X) + a.X;
            result.Y = blend * (b.Y - a.Y) + a.Y;
            result.Z = blend * (b.Z - a.Z) + a.Z;
        }

        #endregion

        #region Barycentric

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        public static Vector3d BaryCentric(Vector3d a, Vector3d b, Vector3d c, double u, double v)
        {
            return a + u * (b - a) + v * (c - a);
        }

        /// <summary>Interpolate 3 Vectors using Barycentric coordinates</summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <param name="result">Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</param>
        public static void BaryCentric(ref Vector3d a, ref Vector3d b, ref Vector3d c, double u, double v, out Vector3d result)
        {
            result = a; // copy

            Vector3d temp = b; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, u, out temp);
            Add(ref result, ref temp, out result);

            temp = c; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, v, out temp);
            Add(ref result, ref temp, out result);
        }

        #endregion

        #region Transform

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vector3d TransformVector(Vector3d vec, Matrix4d mat)
        {
            return new Vector3d(
                Vector3d.Dot(vec, new Vector3d(mat.Column0)),
                Vector3d.Dot(vec, new Vector3d(mat.Column1)),
                Vector3d.Dot(vec, new Vector3d(mat.Column2)));
        }

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformVector(ref Vector3d vec, ref Matrix4d mat, out Vector3d result)
        {
            result.X = vec.X * mat.Row0.X +
                       vec.Y * mat.Row1.X +
                       vec.Z * mat.Row2.X;

            result.Y = vec.X * mat.Row0.Y +
                       vec.Y * mat.Row1.Y +
                       vec.Z * mat.Row2.Y;

            result.Z = vec.X * mat.Row0.Z +
                       vec.Y * mat.Row1.Z +
                       vec.Z * mat.Row2.Z;
        }

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static Vector3d TransformNormal(Vector3d norm, Matrix4d mat)
        {
            mat.Invert();
            return TransformNormalInverse(norm, mat);
        }

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed normal</param>
        public static void TransformNormal(ref Vector3d norm, ref Matrix4d mat, out Vector3d result)
        {
            Matrix4d Inverse = Matrix4d.Invert(mat);
            Vector3d.TransformNormalInverse(ref norm, ref Inverse, out result);
        }

        /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static Vector3d TransformNormalInverse(Vector3d norm, Matrix4d invMat)
        {
            return new Vector3d(
                Vector3d.Dot(norm, new Vector3d(invMat.Row0)),
                Vector3d.Dot(norm, new Vector3d(invMat.Row1)),
                Vector3d.Dot(norm, new Vector3d(invMat.Row2)));
        }

        /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <param name="result">The transformed normal</param>
        public static void TransformNormalInverse(ref Vector3d norm, ref Matrix4d invMat, out Vector3d result)
        {
            result.X = norm.X * invMat.Row0.X +
                       norm.Y * invMat.Row0.Y +
                       norm.Z * invMat.Row0.Z;

            result.Y = norm.X * invMat.Row1.X +
                       norm.Y * invMat.Row1.Y +
                       norm.Z * invMat.Row1.Z;

            result.Z = norm.X * invMat.Row2.X +
                       norm.Y * invMat.Row2.Y +
                       norm.Z * invMat.Row2.Z;
        }

        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed position</returns>
        public static Vector3d TransformPosition(Vector3d pos, Matrix4d mat)
        {
            return new Vector3d(
                Vector3d.Dot(pos, new Vector3d(mat.Column0)) + mat.Row3.X,
                Vector3d.Dot(pos, new Vector3d(mat.Column1)) + mat.Row3.Y,
                Vector3d.Dot(pos, new Vector3d(mat.Column2)) + mat.Row3.Z);
        }

        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed position</param>
        public static void TransformPosition(ref Vector3d pos, ref Matrix4d mat, out Vector3d result)
        {
            result.X = pos.X * mat.Row0.X +
                       pos.Y * mat.Row1.X +
                       pos.Z * mat.Row2.X +
                       mat.Row3.X;

            result.Y = pos.X * mat.Row0.Y +
                       pos.Y * mat.Row1.Y +
                       pos.Z * mat.Row2.Y +
                       mat.Row3.Y;

            result.Z = pos.X * mat.Row0.Z +
                       pos.Y * mat.Row1.Z +
                       pos.Z * mat.Row2.Z +
                       mat.Row3.Z;
        }

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vector3d Transform(Vector3d vec, Matrix4d mat)
        {
            Vector3d result;
            Transform(ref vec, ref mat, out result);
            return result;
        }

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void Transform(ref Vector3d vec, ref Matrix4d mat, out Vector3d result)
        {
            Vector4d v4 = new Vector4d(vec.X, vec.Y, vec.Z, 1.0);
            Vector4d.Transform(ref v4, ref mat, out v4);
            result = v4.Xyz;
        }

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector3d Transform(Vector3d vec, Quaterniond quat)
        {
            Vector3d result;
            Transform(ref vec, ref quat, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Transform(ref Vector3d vec, ref Quaterniond quat, out Vector3d result)
        {
            // Since vec.W == 0, we can optimize quat * vec * quat^-1 as follows:
            // vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.w * vec)
            Vector3d xyz = quat.Xyz, temp, temp2;
            Vector3d.Cross(ref xyz, ref vec, out temp);
            Vector3d.Multiply(ref vec, quat.W, out temp2);
            Vector3d.Add(ref temp, ref temp2, out temp);
            Vector3d.Cross(ref xyz, ref temp, out temp);
            Vector3d.Multiply(ref temp, 2, out temp);
            Vector3d.Add(ref vec, ref temp, out result);
        }

        /// <summary>
        /// Transform a Vector3d by the given Matrix, and project the resulting Vector4 back to a Vector3
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vector3d TransformPerspective(Vector3d vec, Matrix4d mat)
        {
            Vector3d result;
            TransformPerspective(ref vec, ref mat, out result);
            return result;
        }

        /// <summary>Transform a Vector3d by the given Matrix, and project the resulting Vector4d back to a Vector3d</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformPerspective(ref Vector3d vec, ref Matrix4d mat, out Vector3d result)
        {
            Vector4d v = new Vector4d(vec, 1);
            Vector4d.Transform(ref v, ref mat, out v);
            result.X = v.X / v.W;
            result.Y = v.Y / v.W;
            result.Z = v.Z / v.W;
        }

        #endregion

        #region CalculateAngle

        /// <summary>
        /// Calculates the angle (in radians) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        /// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
        public static double CalculateAngle(Vector3d first, Vector3d second)
        {
            double result;
            CalculateAngle(ref first, ref second, out result);
            return result;
        }

        /// <summary>Calculates the angle (in radians) between two vectors.</summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <param name="result">Angle (in radians) between the vectors.</param>
        /// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
        public static void CalculateAngle(ref Vector3d first, ref Vector3d second, out double result)
        {
            double temp;
            Vector3d.Dot(ref first, ref second, out temp);
            result = System.Math.Acos(MathFunc.Clamp(temp / (first.Length * second.Length), -1.0, 1.0));
        }

        #endregion

        #endregion

        #region Swizzle

        #region 2-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the X and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Xy { get { return new Vector2d(X, Y); } set { X = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the X and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Xz { get { return new Vector2d(X, Z); } set { X = value.X; Z = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the Y and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Yx { get { return new Vector2d(Y, X); } set { Y = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the Y and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Yz { get { return new Vector2d(Y, Z); } set { Y = value.X; Z = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the Z and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Zx { get { return new Vector2d(Z, X); } set { Z = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2d with the Z and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector2d Zy { get { return new Vector2d(Z, Y); } set { Z = value.X; Y = value.Y; } }

        #endregion

        #region 3-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector3d with the X, Z, and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3d Xzy { get { return new Vector3d(X, Z, Y); } set { X = value.X; Z = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3d with the Y, X, and Z components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3d Yxz { get { return new Vector3d(Y, X, Z); } set { Y = value.X; X = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3d with the Y, Z, and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3d Yzx { get { return new Vector3d(Y, Z, X); } set { Y = value.X; Z = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3d with the Z, X, and Y components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3d Zxy { get { return new Vector3d(Z, X, Y); } set { Z = value.X; X = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3d with the Z, Y, and X components of this instance.
        /// </summary>
        [XmlIgnore]
        public Vector3d Zyx { get { return new Vector3d(Z, Y, X); } set { Z = value.X; Y = value.Y; X = value.Z; } }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator +(Vector3d left, Vector3d right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator -(Vector3d vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator *(Vector3d vec, double scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator *(double scale, Vector3d vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }
		
        /// <summary>
        /// Component-wise multiplication between the specified instance by a scale vector.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3d operator *(Vector3d vec, Vector3d scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            vec.Z *= scale.Z;
            return vec;
        }
		
        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3d operator /(Vector3d vec, double scale)
        {
            double mult = 1 / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            return vec;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return !left.Equals(right);
        }

        /// <summary>Converts OpenTK.Vector3 to OpenTK.Vector3d.</summary>
        /// <param name="v3">The Vector3 to convert.</param>
        /// <returns>The resulting Vector3d.</returns>
        public static explicit operator Vector3d(Vector3 v3)
        {
            return new Vector3d(v3.X, v3.Y, v3.Z);
        }

        /// <summary>Converts OpenTK.Vector3d to OpenTK.Vector3.</summary>
        /// <param name="v3d">The Vector3d to convert.</param>
        /// <returns>The resulting Vector3.</returns>
        public static explicit operator Vector3(Vector3d v3d)
        {
            return new Vector3((float)v3d.X, (float)v3d.Y, (float)v3d.Z);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        private static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        /// <summary>
        /// Returns a System.String that represents the current Vector3.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2})", X, Y, Z, listSeparator);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3d))
                return false;

            return this.Equals((Vector3d)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Vector3> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(Vector3d other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        #endregion
    }

    /* GradientSpace Vector3d
    
    #region g3.Vector3d

    public struct Vector3d : IComparable<Vector3d>, IEquatable<Vector3d>
    {
        public double x;
        public double y;
        public double z; 

        public Vector3d(double f) { x = y = z = f; }
        public Vector3d(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }
        public Vector3d(double[] v2) { x = v2[0]; y = v2[1]; z = v2[2]; }
        public Vector3d(Vector3d copy) { x = copy.x; y = copy.y; z = copy.z; }
        public Vector3d(Vector3f copy) { x = copy.x; y = copy.y; z = copy.z; }

        static public readonly Vector3d Zero = new Vector3d(0.0f, 0.0f, 0.0f);
        static public readonly Vector3d One = new Vector3d(1.0f, 1.0f, 1.0f);
        static public readonly Vector3d AxisX = new Vector3d(1.0f, 0.0f, 0.0f);
        static public readonly Vector3d AxisY = new Vector3d(0.0f, 1.0f, 0.0f);
        static public readonly Vector3d AxisZ = new Vector3d(0.0f, 0.0f, 1.0f);
		static public readonly Vector3d MaxValue = new Vector3d(double.MaxValue,double.MaxValue,double.MaxValue);
		static public readonly Vector3d MinValue = new Vector3d(double.MinValue,double.MinValue,double.MinValue);

        public double this[int key]
        {
            get { return (key == 0) ? x : (key == 1) ? y : z; }
            set { if (key == 0) x = value; else if (key == 1) y = value; else z = value; }
        }

        public Vector2d xy {
            get { return new Vector2d(x, y); }
            set { x = value.x; y = value.y; }
        }
        public Vector2d xz {
            get { return new Vector2d(x, z); }
            set { x = value.x; z = value.y; }
        }
        public Vector2d yz {
            get { return new Vector2d(y, z); }
            set { y = value.x; z = value.y; }
        }

        public double LengthSquared
        {
            get { return x * x + y * y + z * z; }
        }
        public double Length
        {
            get { return System.Math.Sqrt(LengthSquared); }
        }

        public double LengthL1
        {
            get { return Math.Abs(x) + Math.Abs(y) + Math.Abs(z); }
        }

		public double Max {
			get { return Math.Max(x, Math.Max(y, z)); }
		}
		public double Min {
			get { return Math.Min(x, Math.Min(y, z)); }
		}
		public double MaxAbs {
			get { return Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z))); }
		}
		public double MinAbs {
			get { return Math.Min(Math.Abs(x), Math.Min(Math.Abs(y), Math.Abs(z))); }
		}

        public Vector3d Abs {
            get { return new Vector3d(Math.Abs(x), Math.Abs(y), Math.Abs(z)); }
        }

        public double Normalize(double epsilon = MathUtil.Epsilon)
        {
            double length = Length;
            if (length > epsilon) {
                double invLength = 1.0 / length;
                x *= invLength;
                y *= invLength;
                z *= invLength;
            } else {
                length = 0;
                x = y = z = 0;
            }
            return length;
        }
        public Vector3d Normalized
        {
            get {
                double length = Length;
                if (length > MathUtil.Epsilon) {
                    double invLength = 1.0 / length;
                    return new Vector3d(x * invLength, y * invLength, z * invLength);
                } else
                    return Vector3d.Zero;
            }
        }

		public bool IsNormalized {
			get { return Math.Abs( (x * x + y * y + z * z) - 1) < MathUtil.ZeroTolerance; }
		}

        public bool IsFinite
        {
            get { double f = x + y + z; return double.IsNaN(f) == false && double.IsInfinity(f) == false; }
        }

        public void Round(int nDecimals) {
            x = Math.Round(x, nDecimals);
            y = Math.Round(y, nDecimals);
            z = Math.Round(z, nDecimals);
        }


        public double Dot(Vector3d v2) {
            return x * v2.x + y * v2.y + z * v2.z;
        }
        public double Dot(ref Vector3d v2) {
            return x * v2.x + y * v2.y + z * v2.z;
        }

        public static double Dot(Vector3d v1, Vector3d v2) {
            return v1.Dot(ref v2);
        }

        public Vector3d Cross(Vector3d v2) {
            return new Vector3d(
                y * v2.z - z * v2.y,
                z * v2.x - x * v2.z,
                x * v2.y - y * v2.x);
        }
        public Vector3d Cross(ref Vector3d v2) {
            return new Vector3d(
                y * v2.z - z * v2.y,
                z * v2.x - x * v2.z,
                x * v2.y - y * v2.x);
        }
        public static Vector3d Cross(Vector3d v1, Vector3d v2) {
            return v1.Cross(ref v2);
        }

        public Vector3d UnitCross(ref Vector3d v2) {
            Vector3d n = new Vector3d(
                y * v2.z - z * v2.y,
                z * v2.x - x * v2.z,
                x * v2.y - y * v2.x);
            n.Normalize();
            return n;
        }
        public Vector3d UnitCross(Vector3d v2) {
            return UnitCross(ref v2);
        }


        public double AngleD(Vector3d v2)
        {
            double fDot = MathUtil.Clamp(Dot(v2), -1, 1);
            return Math.Acos(fDot) * MathUtil.Rad2Deg;
        }
        public static double AngleD(Vector3d v1, Vector3d v2)
        {
            return v1.AngleD(v2);
        }
        public double AngleR(Vector3d v2)
        {
            double fDot = MathUtil.Clamp(Dot(v2), -1, 1);
            return Math.Acos(fDot);
        }
        public static double AngleR(Vector3d v1, Vector3d v2)
        {
            return v1.AngleR(v2);
        }

		public double DistanceSquared(Vector3d v2) {
			double dx = v2.x-x, dy = v2.y-y, dz = v2.z-z;
			return dx*dx + dy*dy + dz*dz;
		}
		public double DistanceSquared(ref Vector3d v2) {
			double dx = v2.x-x, dy = v2.y-y, dz = v2.z-z;
			return dx*dx + dy*dy + dz*dz;
		}

        public double Distance(Vector3d v2) {
            double dx = v2.x-x, dy = v2.y-y, dz = v2.z-z;
			return System.Math.Sqrt(dx*dx + dy*dy + dz*dz);
		}
        public double Distance(ref Vector3d v2) {
            double dx = v2.x-x, dy = v2.y-y, dz = v2.z-z;
			return System.Math.Sqrt(dx*dx + dy*dy + dz*dz);
		}

        public void Set(Vector3d o)
        {
            x = o.x; y = o.y; z = o.z;
        }
        public void Set(double fX, double fY, double fZ)
        {
            x = fX; y = fY; z = fZ;
        }
        public void Add(Vector3d o)
        {
            x += o.x; y += o.y; z += o.z;
        }
        public void Subtract(Vector3d o)
        {
            x -= o.x; y -= o.y; z -= o.z;
        }



        public static Vector3d operator -(Vector3d v)
        {
            return new Vector3d(-v.x, -v.y, -v.z);
        }

        public static Vector3d operator *(double f, Vector3d v)
        {
            return new Vector3d(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3d operator *(Vector3d v, double f)
        {
            return new Vector3d(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3d operator /(Vector3d v, double f)
        {
            return new Vector3d(v.x / f, v.y / f, v.z / f);
        }
        public static Vector3d operator /(double f, Vector3d v)
        {
            return new Vector3d(f / v.x, f / v.y, f / v.z);
        }

        public static Vector3d operator *(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3d operator /(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x / b.x, a.y / b.y, a.z / b.z);
        }


        public static Vector3d operator +(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
        }
        public static Vector3d operator +(Vector3d v0, double f)
        {
            return new Vector3d(v0.x + f, v0.y + f, v0.z + f);
        }

        public static Vector3d operator -(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
        }
        public static Vector3d operator -(Vector3d v0, double f)
        {
            return new Vector3d(v0.x - f, v0.y - f, v0.z - f);
        }



        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }
        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return (a.x != b.x || a.y != b.y || a.z != b.z);
        }
        public override bool Equals(object obj)
        {
            return this == (Vector3d)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ x.GetHashCode();
                hash = (hash * 16777619) ^ y.GetHashCode();
                hash = (hash * 16777619) ^ z.GetHashCode();
                return hash;
            }
        }
        public int CompareTo(Vector3d other)
        {
            if (x != other.x)
                return x < other.x ? -1 : 1;
            else if (y != other.y)
                return y < other.y ? -1 : 1;
            else if (z != other.z)
                return z < other.z ? -1 : 1;
            return 0;
        }
        public bool Equals(Vector3d other)
        {
            return (x == other.x && y == other.y && z == other.z);
        }


        public bool EpsilonEqual(Vector3d v2, double epsilon) {
            return Math.Abs(x - v2.x) <= epsilon && 
                   Math.Abs(y - v2.y) <= epsilon &&
                   Math.Abs(z - v2.z) <= epsilon;
        }


        public static Vector3d Lerp(Vector3d a, Vector3d b, double t) {
            double s = 1 - t;
            return new Vector3d(s * a.x + t * b.x, s * a.y + t * b.y, s * a.z + t * b.z);
        }
        public static Vector3d Lerp(ref Vector3d a, ref Vector3d b, double t) {
            double s = 1 - t;
            return new Vector3d(s * a.x + t * b.x, s * a.y + t * b.y, s * a.z + t * b.z);
        }



        public override string ToString() {
            return string.Format("{0:F8} {1:F8} {2:F8}", x, y, z);
        }
        public string ToString(string fmt) {
            return string.Format("{0} {1} {2}", x.ToString(fmt), y.ToString(fmt), z.ToString(fmt));
        }



        public static implicit operator Vector3d(Vector3f v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }
        public static explicit operator Vector3f(Vector3d v)
        {
            return new Vector3f((float)v.x, (float)v.y, (float)v.z);
        }


#if G3_USING_UNITY
        public static implicit operator Vector3d(UnityEngine.Vector3 v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }
        public static explicit operator Vector3(Vector3d v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }
#endif




        // complicated functions go down here...


        /// <summary>
        /// Gram-Schmidt orthonormalization of the input vectors.
        /// [RMS] this is from WildMagic5, but I added returning the minLength value
        /// from GTEngine, because I use this in place of GTEngine's Orthonormalize in
        /// ComputeOrthogonalComplement below
        /// </summary>
        public static double Orthonormalize(ref Vector3d u, ref Vector3d v, ref Vector3d w)
        {
            // If the input vectors are v0, v1, and v2, then the Gram-Schmidt
            // orthonormalization produces vectors u0, u1, and u2 as follows,
            //
            //   u0 = v0/|v0|
            //   u1 = (v1-(u0*v1)u0)/|v1-(u0*v1)u0|
            //   u2 = (v2-(u0*v2)u0-(u1*v2)u1)/|v2-(u0*v2)u0-(u1*v2)u1|
            //
            // where |A| indicates length of vector A and A*B indicates dot
            // product of vectors A and B.

            // compute u0
            double minLength = u.Normalize();

            // compute u1
            double dot0 = u.Dot(v);
            v -= dot0 * u;
            double l = v.Normalize();
            if (l < minLength)
                minLength = l;

            // compute u2
            double dot1 = v.Dot(w);
            dot0 = u.Dot(w);
            w -= dot0 * u + dot1 * v;
            l = w.Normalize();
            if (l < minLength)
                minLength = l;

            return minLength;
        }


        /// <summary>
        /// Input W must be a unit-length vector.  The output vectors {U,V} are
        /// unit length and mutually perpendicular, and {U,V,W} is an orthonormal basis.
        /// ported from WildMagic5
        /// </summary>
        public static void GenerateComplementBasis(ref Vector3d u, ref Vector3d v, Vector3d w)
        {
            double invLength;

            if (Math.Abs(w.x) >= Math.Abs(w.y)) {
                // W.x or W.z is the largest magnitude component, swap them
                invLength = MathUtil.InvSqrt(w.x * w.x + w.z * w.z);
                u.x = -w.z * invLength;
                u.y = 0;
                u.z = +w.x * invLength;
                v.x = w.y * u.z;
                v.y = w.z * u.x - w.x * u.z;
                v.z = -w.y * u.x;
            } else {
                // W.y or W.z is the largest magnitude component, swap them
                invLength = MathUtil.InvSqrt(w.y * w.y + w.z * w.z);
                u.x = 0;
                u.y = +w.z * invLength;
                u.z = -w.y * invLength;
                v.x = w.y * u.z - w.z * u.y;
                v.y = -w.x * u.z;
                v.z = w.x * u.y;
            }
        }

        /// <summary>
        /// this function is ported from GTEngine.
        /// Compute a right-handed orthonormal basis for the orthogonal complement
        /// of the input vectors.  The function returns the smallest length of the
        /// unnormalized vectors computed during the process.  If this value is nearly
        /// zero, it is possible that the inputs are linearly dependent (within
        /// numerical round-off errors).  On input, numInputs must be 1 or 2 and
        /// v0 through v(numInputs-1) must be initialized.  On output, the
        /// vectors v0 through v2 form an orthonormal set.
        /// </summary>
        public static double ComputeOrthogonalComplement(int numInputs, Vector3d v0, ref Vector3d v1, ref Vector3d v2)
        {
            if (numInputs == 1) {
                if (Math.Abs(v0[0]) > Math.Abs(v0[1])) {
                    v1 = new Vector3d( -v0[2], 0.0, +v0[0] );
}
                else
                {
                    v1 = new Vector3d(0.0, +v0[2], -v0[1]);
                }
                numInputs = 2;
            }

            if (numInputs == 2) {
                v2 = Vector3d.Cross(v0, v1);
                return Vector3d.Orthonormalize(ref v0, ref v1, ref v2);
                //return Orthonormalize<3, Real>(3, v, robust);
            }

            return 0;
        }



        /// <summary>
        /// Returns two vectors perpendicular to n, as efficiently as possible.
        /// Duff et all method, from https://graphics.pixar.com/library/OrthonormalB/paper.pdf
        /// </summary>
        public static void MakePerpVectors(ref Vector3d n, out Vector3d b1, out Vector3d b2)
{
    if (n.z < 0.0)
    {
        double a = 1.0 / (1.0 - n.z);
        double b = n.x * n.y * a;
        //b1 = Vec3f(1.0f - n.x * n.x * a, -b, n.x);
        //b2 = Vec3f(b, n.y * n.y * a - 1.0f, -n.y);
        b1.x = 1.0f - n.x * n.x * a;
        b1.y = -b;
        b1.z = n.x;
        b2.x = b;
        b2.y = n.y * n.y * a - 1.0f;
        b2.z = -n.y;
    }
    else
    {
        double a = 1.0 / (1.0 + n.z);
        double b = -n.x * n.y * a;
        //b1 = Vec3f(1.0 - n.x * n.x * a, b, -n.x);
        //b2 = Vec3f(b, 1.0 - n.y * n.y * a, -n.y);
        b1.x = 1.0 - n.x * n.x * a;
        b1.y = b;
        b1.z = -n.x;
        b2.x = b;
        b2.y = 1.0 - n.y * n.y * a;
        b2.z = -n.y;
    }
}

    }

    #endregion


    */
}