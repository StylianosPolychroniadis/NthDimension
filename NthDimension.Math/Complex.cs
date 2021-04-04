using System;

namespace NthDimension.Math
{
	public struct Complex
	{
		public float Real;

		public float Imag;

		public Complex(float real, float imaginary)
		{
			Real = real;
			Imag = imaginary;
		}

		public Complex(Complex c)
		{
			Real = c.Real;
			Imag = c.Imag;
		}

		public float Modulus()
		{
			return (float)System.Math.Sqrt(ModulusSquared());
		}

		public float ModulusSquared()
		{
			return Real * Real + Imag * Imag;
		}

		public float Argument()
		{
			return (float)System.Math.Atan2(Imag, Real);
		}

		public float ArgumentFast()
		{
			return Trigonometry.Atan2(Imag, Real);
		}

		public Complex Conjugate()
		{
			Complex result = default(Complex);
			result.Real = Real;
			result.Imag = 0f - Imag;
			return result;
		}

		public Complex Normalize()
		{
			float b = 1f / Modulus();
			return this * b;
		}

		public Complex NormalizeFast()
		{
			float b = 1.95f - ModulusSquared();
			return this * b;
		}

		public override string ToString()
		{
			return string.Format("real {0}, imag {1}", Real, Imag);
		}

		public static Complex FromAngle(double angle)
		{
			Complex result = default(Complex);
			result.Real = (float)System.Math.Cos(angle);
			result.Imag = (float)System.Math.Sin(angle);
			return result;
		}

		public static Complex FromAngleFast(float angle)
		{
			return Trigonometry.SinCos(angle);
		}

		public static bool operator ==(Complex leftHandSide, Complex rightHandSide)
		{
			if (leftHandSide.Real != rightHandSide.Real)
			{
				return false;
			}
			return leftHandSide.Imag == rightHandSide.Imag;
		}

		public static bool operator !=(Complex leftHandSide, Complex rightHandSide)
		{
			if (leftHandSide.Real != rightHandSide.Real)
			{
				return true;
			}
			return leftHandSide.Imag != rightHandSide.Imag;
		}

		public static Complex operator +(Complex a, Complex b)
		{
			Complex result = default(Complex);
			result.Real = a.Real + b.Real;
			result.Imag = a.Imag + b.Imag;
			return result;
		}

		public static Complex operator -(Complex a, Complex b)
		{
			Complex result = default(Complex);
			result.Real = a.Real - b.Real;
			result.Imag = a.Imag - b.Imag;
			return result;
		}

		public static Complex operator *(Complex a, Complex b)
		{
			Complex result = default(Complex);
			result.Real = a.Real * b.Real - a.Imag * b.Imag;
			result.Imag = a.Imag * b.Real + a.Real * b.Imag;
			return result;
		}

		public static Complex operator *(Complex a, float b)
		{
			Complex result = default(Complex);
			result.Real = a.Real * b;
			result.Imag = a.Imag * b;
			return result;
		}

		public static Complex operator *(float a, Complex b)
		{
			Complex result = default(Complex);
			result.Real = b.Real * a;
			result.Imag = b.Imag * a;
			return result;
		}

		public static Complex operator /(Complex a, Complex b)
		{
			float num = b.Real * b.Real + b.Imag * b.Imag;
			num = 1f / num;
			Complex result = default(Complex);
			result.Real = (a.Real * b.Real + a.Imag * b.Imag) * num;
			result.Imag = (a.Imag * b.Real - a.Real * b.Imag) * num;
			return result;
		}

		public static Complex operator /(Complex a, float b)
		{
			b = 1f / b;
			Complex result = default(Complex);
			result.Real = a.Real * b;
			result.Imag = a.Imag * b;
			return result;
		}

		public static Complex operator ~(Complex a)
		{
			return a.Conjugate();
		}

		public static implicit operator Complex(float d)
		{
			return new Complex(d, 0f);
		}

		public override int GetHashCode()
		{
			return (Real.GetHashCode() * 397) ^ Imag.GetHashCode();
		}

		public bool Equals(Complex obj)
		{
			if (obj.Real == Real)
			{
				return obj.Imag == Imag;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Complex))
			{
				return false;
			}
			return Equals((Complex)obj);
		}
	}
}
