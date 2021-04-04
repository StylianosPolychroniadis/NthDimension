using System;

namespace NthDimension.Math
{
	public static class Trigonometry
	{
		private const int ResolutionInBits = 16;

		private static readonly int _mask;

		private static readonly float _indexScale;

		private static readonly UnsafeBuffer _sinBuffer;

		private static readonly UnsafeBuffer _cosBuffer;

		private unsafe static readonly float* _sinPtr;

		private unsafe static readonly float* _cosPtr;

		unsafe static Trigonometry()
		{
			_mask = 65535;
			int num = _mask + 1;
			_sinBuffer = UnsafeBuffer.Create(num, 4);
			_cosBuffer = UnsafeBuffer.Create(num, 4);
			_sinPtr = (float*)(void*)_sinBuffer;
			_cosPtr = (float*)(void*)_cosBuffer;
			_indexScale = (float)num / ((float)System.Math.PI * 2f);
			for (int i = 0; i < num; i++)
			{
				_sinPtr[i] = (float)System.Math.Sin(((float)i + 0.5f) / (float)num * ((float)System.Math.PI * 2f));
				_cosPtr[i] = (float)System.Math.Cos(((float)i + 0.5f) / (float)num * ((float)System.Math.PI * 2f));
			}
			for (float num2 = 0f; num2 < (float)System.Math.PI * 2f; num2 += (float)System.Math.PI / 2f)
			{
				_sinPtr[(int)(num2 * _indexScale) & _mask] = (float)System.Math.Sin(num2);
				_cosPtr[(int)(num2 * _indexScale) & _mask] = (float)System.Math.Cos(num2);
			}
		}

		public unsafe static float Sin(float angle)
		{
			return _sinPtr[(int)(angle * _indexScale) & _mask];
		}

		public unsafe static float Cos(float angle)
		{
			return _cosPtr[(int)(angle * _indexScale) & _mask];
		}

		public unsafe static Complex SinCos(float rad)
		{
			int num = (int)(rad * _indexScale) & _mask;
			Complex result = default(Complex);
			result.Real = _cosPtr[num];
			result.Imag = _sinPtr[num];
			return result;
		}

		public static float Atan2(float y, float x)
		{
			if ((double)x == 0.0)
			{
				if ((double)y > 0.0)
				{
					return (float)System.Math.PI / 2f;
				}
				if ((double)y == 0.0)
				{
					return 0f;
				}
				return -(float)System.Math.PI / 2f;
			}
			float num = y / x;
			float num2;
			if ((double)System.Math.Abs(num) < 1.0)
			{
				num2 = num / (1f + 0.2854f * num * num);
				if ((double)x < 0.0)
				{
					if ((double)y < 0.0)
					{
						return num2 - (float)System.Math.PI;
					}
					return num2 + (float)System.Math.PI;
				}
			}
			else
			{
				num2 = (float)System.Math.PI / 2f - num / (num * num + 0.2854f);
				if ((double)y < 0.0)
				{
					return num2 - (float)System.Math.PI;
				}
			}
			return num2;
		}
	}
}
