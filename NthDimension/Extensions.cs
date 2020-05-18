using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this Random random, IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int index = random.Next(i + 1);
                T value = items[i];
                items[i] = items[index];
                items[index] = value;
            }
        }
    }

    public static class Matrix4Extensions
    {
        public static Matrix4 Matrix4(this Matrix4 mat)
        {
            return new Matrix4(mat.Row0, mat.Row1, mat.Row2, mat.Row3);
        }
    }
}
