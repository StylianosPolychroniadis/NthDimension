using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Algebra
{
    public class NormDist
    {
        private readonly float stdev;

        private readonly float mean;

        private readonly float max;

        private readonly float min;

        private readonly Random random;

        private double? nextNum;

        public NormDist(float stdev, float mean, float min, float max, int? seed = null)
        {
            this.stdev = stdev;
            this.mean = mean;
            this.min = min;
            this.max = max;
            if (seed.HasValue)
            {
                this.random = new Random(seed.Value);
                return;
            }
            this.random = new Random();
        }

        private double NextValueInternal()
        {
            if (this.nextNum.HasValue)
            {
                double value = this.nextNum.Value;
                this.nextNum = null;
                return value;
            }
            double num;
            double num2;
            double num3;
            do
            {
                num = this.random.NextDouble() * 2.0 - 1.0;
                num2 = this.random.NextDouble() * 2.0 - 1.0;
                num3 = num * num + num2 * num2;
            }
            while (num3 >= 1.0);
            this.nextNum = new double?(num * System.Math.Sqrt(-2.0 * System.Math.Log(num3) / num3));
            return num2 * System.Math.Sqrt(-2.0 * System.Math.Log(num3) / num3);
        }

        public double NextValue()
        {
            double num = this.NextValueInternal() * (double)this.stdev + (double)this.mean;
            if (num < (double)this.min)
            {
                num = (double)this.min;
            }
            if (num > (double)this.max)
            {
                num = (double)this.max;
            }
            return num;
        }
    }
}
