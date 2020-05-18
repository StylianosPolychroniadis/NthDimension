using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Algebra
{
    /// <summary>
    /// Class for representing a Bezier path, and methods for getting suitable points to 
    /// draw the curve with line segments.
    /// </summary>

    public class BezierPath
    {
        private const int SEGMENTS_PER_CURVE = 120;
        private const float MINIMUM_SQR_DISTANCE = 0.01f;

        /// <summary>
        /// This corresponds to about 172 degrees, 8 degrees from a traight line
        /// </summary>
        private const float DIVISION_THRESHOLD = -0.99f;

        private List<Vector3> controlPoints;

        /// <summary>
        /// how many bezier curves in this path?
        /// </summary>
        private int curveCount;

        /// <summary>
        /// Constructs a new empty Bezier curve. Use one of these methods
        /// to add points: SetControlPoints, Interpolate, SamplePoints.
        /// </summary>
        public BezierPath()
        {
            controlPoints = new List<Vector3>();
        }

        /// <summary>
        /// Sets the control points of this Bezier path.
        /// Points 0-3 forms the first Bezier curve, points 
        /// 3-6 forms the second curve, etc.
        /// </summary>            
        public void SetControlPoints(List<Vector3> newControlPoints)
        {
            controlPoints.Clear();
            controlPoints.AddRange(newControlPoints);
            curveCount = (controlPoints.Count - 1) / 3;
        }

        /// <summary>
        /// Returns the control points for this Bezier curve.
        /// </summary>
        public List<Vector3> GetControlPoints()
        {
            return controlPoints;
        }

        /// <summary>
        /// Calculates a Bezier interpolated path for the given points.
        /// </summary>    

        public void Interpolate(List<Vector3> segmentPoints, float scale)
        {
            controlPoints.Clear();

            if (segmentPoints.Count < 2)
            {
                return;
            }

            for (int i = 0; i < segmentPoints.Count; i++)
            {
                if (i == 0) // is first
                {
                    Vector3 p1 = segmentPoints[i];
                    Vector3 p2 = segmentPoints[i + 1];

                    Vector3 tangent = (p2 - p1);
                    Vector3 q1 = p1 + scale * tangent;

                    controlPoints.Add(p1);
                    controlPoints.Add(q1);
                }
                else if (i == segmentPoints.Count - 1) //last
                {
                    Vector3 p0 = segmentPoints[i - 1];
                    Vector3 p1 = segmentPoints[i];
                    Vector3 tangent = (p1 - p0);
                    Vector3 q0 = p1 - scale * tangent;

                    controlPoints.Add(q0);
                    controlPoints.Add(p1);
                }
                else
                {
                    Vector3 p0 = segmentPoints[i - 1];
                    Vector3 p1 = segmentPoints[i];
                    Vector3 p2 = segmentPoints[i + 1];
                    Vector3 tangent = (p2 - p0).Normalized();
                    Vector3 q0 = p1 - scale * tangent * (p1 - p0).Length;
                    Vector3 q1 = p1 + scale * tangent * (p2 - p1).Length;

                    controlPoints.Add(q0);
                    controlPoints.Add(p1);
                    controlPoints.Add(q1);
                }
            }

            curveCount = (controlPoints.Count - 1) / 3;
        }

        /// <summary>
        /// Sample the given points as a Bezier path.
        ///</summary>
        public void SamplePoints(List<Vector3> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
        {
            if (sourcePoints.Count < 2)
            {
                return;
            }

            Stack<Vector3> samplePoints = new Stack<Vector3>();

            samplePoints.Push(sourcePoints[0]);

            Vector3 potentialSamplePoint = sourcePoints[1];

            int i = 2;

            for (i = 2; i < sourcePoints.Count; i++)
            {
                if (
                    ((potentialSamplePoint - sourcePoints[i]).LengthSquared > minSqrDistance) &&
                    ((samplePoints.Peek() - sourcePoints[i]).LengthSquared > maxSqrDistance))
                {
                    samplePoints.Push(potentialSamplePoint);
                }

                potentialSamplePoint = sourcePoints[i];
            }

            //now handle last bit of curve
            Vector3 p1 = samplePoints.Pop(); //last sample point
            Vector3 p0 = samplePoints.Peek(); //second last sample point
            Vector3 tangent = (p0 - potentialSamplePoint).Normalized();
            float d2 = (potentialSamplePoint - p1).Length;
            float d1 = (p1 - p0).Length;
            p1 = p1 + tangent * ((d1 - d2) / 2);

            samplePoints.Push(p1);
            samplePoints.Push(potentialSamplePoint);


            Interpolate(new List<Vector3>(samplePoints), scale);
        }

        /// <summary>
        /// Caluclates a point on the path.            
        /// @param curveIndex The index of the curve that the point is on. For example, 
        /// the second curve (index 1) is the curve with controlpoints 3, 4, 5, and 6.            
        /// @param t The paramater indicating where on the curve the point is. 0 corresponds 
        /// to the "left" point, 1 corresponds to the "right" end point.
        ///</summary>
        public Vector3 CalculateBezierPoint(int curveIndex, float t)
        {
            int nodeIndex = curveIndex * 3;

            Vector3 p0 = controlPoints[nodeIndex];
            Vector3 p1 = controlPoints[nodeIndex + 1];
            Vector3 p2 = controlPoints[nodeIndex + 2];
            Vector3 p3 = controlPoints[nodeIndex + 3];

            return CalculateBezierPoint(t, p0, p1, p2, p3);
        }

        /// <summary>
        /// Gets the drawing points. This implementation simply calculates a certain number
        /// of points per curve.
        /// </summary>
        public List<Vector3> GetDrawingPoints0()
        {
            List<Vector3> drawingPoints = new List<Vector3>();

            for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                if (curveIndex == 0) //Only do this for the first end point. 
                                     //When i != 0, this coincides with the 
                                     //end point of the previous segment,
                {
                    drawingPoints.Add(CalculateBezierPoint(curveIndex, 0));
                }

                for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
                {
                    float t = j / (float)SEGMENTS_PER_CURVE;
                    drawingPoints.Add(CalculateBezierPoint(curveIndex, t));
                }
            }

            return drawingPoints;
        }

        /// <summary>
        /// Gets the drawing points. This implementation simply calculates a certain number
        /// of points per curve.
        /// This is a lsightly different inplementation from the one above.
        /// </summary>
        public List<Vector3> GetDrawingPoints1()
        {
            List<Vector3> drawingPoints = new List<Vector3>();

            for (int i = 0; i < controlPoints.Count - 3; i += 3)
            {
                Vector3 p0 = controlPoints[i];
                Vector3 p1 = controlPoints[i + 1];
                Vector3 p2 = controlPoints[i + 2];
                Vector3 p3 = controlPoints[i + 3];

                if (i == 0) //only do this for the first end point. When i != 0, this coincides with the end point of the previous segment,
                {
                    drawingPoints.Add(CalculateBezierPoint(0, p0, p1, p2, p3));
                }

                for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
                {
                    float t = j / (float)SEGMENTS_PER_CURVE;
                    drawingPoints.Add(CalculateBezierPoint(t, p0, p1, p2, p3));
                }
            }

            return drawingPoints;
        }

        /// <summary>
        /// This gets the drawing points of a bezier curve, using recursive division,
        /// which results in less points for the same accuracy as the above implementation.
        /// </summary>
        public List<Vector3> GetDrawingPoints2()
        {
            List<Vector3> drawingPoints = new List<Vector3>();

            for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
            {
                List<Vector3> bezierCurveDrawingPoints = FindDrawingPoints(curveIndex);

                if (curveIndex != 0)
                {
                    //remove the fist point, as it coincides with the last point of the previous Bezier curve.
                    bezierCurveDrawingPoints.RemoveAt(0);
                }

                drawingPoints.AddRange(bezierCurveDrawingPoints);
            }

            return drawingPoints;
        }

        List<Vector3> FindDrawingPoints(int curveIndex)
        {
            List<Vector3> pointList = new List<Vector3>();

            Vector3 left = CalculateBezierPoint(curveIndex, 0);
            Vector3 right = CalculateBezierPoint(curveIndex, 1);

            pointList.Add(left);
            pointList.Add(right);

            FindDrawingPoints(curveIndex, 0, 1, pointList, 1);

            return pointList;
        }

        /// <summary>
        /// @returns the number of points added.
        /// </summary>
        int FindDrawingPoints(int curveIndex, float t0, float t1,
            List<Vector3> pointList, int insertionIndex)
        {
            Vector3 left = CalculateBezierPoint(curveIndex, t0);
            Vector3 right = CalculateBezierPoint(curveIndex, t1);

            if ((left - right).LengthSquared < MINIMUM_SQR_DISTANCE)
            {
                return 0;
            }

            float tMid = (t0 + t1) / 2;
            Vector3 mid = CalculateBezierPoint(curveIndex, tMid);

            Vector3 leftDirection = (left - mid).Normalized();
            Vector3 rightDirection = (right - mid).Normalized();

            if (Vector3.Dot(leftDirection, rightDirection) > DIVISION_THRESHOLD || System.Math.Abs(tMid - 0.5f) < 0.0001f)
            {
                int pointsAddedCount = 0;

                pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, insertionIndex);
                pointList.Insert(insertionIndex + pointsAddedCount, mid);
                pointsAddedCount++;
                pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, insertionIndex + pointsAddedCount);

                return pointsAddedCount;
            }

            return 0;
        }



        /// <summary>
        /// Caluclates a point on the Bezier curve represented with the four controlpoints given.
        /// </summary>
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term

            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;

        }
    }
}
