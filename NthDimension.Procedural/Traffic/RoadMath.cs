namespace NthDimension.Procedural.Traffic
{
    using System;
    using NthDimension.Algebra;

    public static class RoadMath
    {
        // TODO:: Switch to NthDimension.Algebra.BezierCurve & NthDimension.Algebra.BezierPath
        public static Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            Vector2[] points = new Vector2[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        public static Vector2 GetBezierPoint(double t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((float)((1 - t) * P0.X + t * P1.X), (float)((1 - t) * P0.Y + t * P1.Y));
        }
        public static float normalizeAngle(float angle)
        {
            angle %= MathHelper.Pi * 2;
            if (angle < 0) angle += MathHelper.Pi * 2;
            if (angle > (3.145f * 2)) angle = 0;
            return angle;
        }

        public static float getVectorAngle(Vector2 vector)
        {
            if (vector.Y >= 0 && vector.X >= 0) return (float)Math.Asin(vector.Y / vector.Length);
            if (vector.Y >= 0 && vector.X <= 0) return MathHelper.Pi - (float)Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X >= 0) return (MathHelper.Pi * 2) + (float)Math.Asin(vector.Y / vector.Length);
            if (vector.Y <= 0 && vector.X <= 0) return MathHelper.Pi - (float)Math.Asin(vector.Y / vector.Length);
            return 0;
        }
    }
}
