namespace NthDimension.Procedural.Traffic
{
    /// (C) Copyright 2019-2020 for Stylianos Polychroniadis, SYSCON Technologies, Hellas
    /// All Rights Reserved.
    /// Not to be reproduced, copied, or otherwise used outside NthDimension development
    /// Rev.: 1.0

    using System;
    using System.Collections.Generic;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Geometry;

    public class LaneMesh
    {
        #region Fields
        private List<Vector2>   roadLine;
        private List<Vector2>   roadCurve;
        private Vector2[]       pointsL;
        private Vector2[]       pointsR;
        private List<Vector2>   textureUVs;
        private List<int>       elements;
        private List<Vector3>   vertices;
        private List<Vector3>   normals;
        private List<Face>      faces;

        private int             segments;
        private float           offset;
        private float           width;
        private float           height;
        private float           worldHeight;
        #endregion Fields

        public Vector2[]    RoadCurve
        {
            get { return this.roadCurve.ToArray(); }
        }
        public Vector2[]    TextureCoordinates
        {
            get { return this.textureUVs.ToArray(); }
        }
        public int[]        ElementArray
        {
            get { return this.elements.ToArray(); }
        }
        public Vector3[]    Vertices
        {
            get { return this.vertices.ToArray(); }
        }
        public Vector3[]    Normals
        {
            get
            {
                return normals.ToArray(); }
        }
        public Face[]       Faces
        {
            get { return faces.ToArray(); }
        }

        public LaneMesh(Vector2[] roadLine, int segments, float offset, float width, float height, float worldHeight = 0f)
        {
            this.segments       = segments;
            this.offset         = offset;
            this.width          = width /= 2;
            this.height         = height;
            this.worldHeight    = worldHeight;

            this.roadLine       = new List<Vector2>();
            this.roadLine.AddRange(roadLine);
            int roadLength      = this.roadLine.Count;
            if (roadLength == 0) throw new Exception("Road line array is empty");
            this.roadCurve      = new List<Vector2>();
            this.roadCurve.AddRange(RoadMath.GetBezierApproximation(roadLine, segments));
            int curveLength     = this.roadCurve.Count;
            if (curveLength == 0) throw new Exception("Failed to approximate bezier curve");

            this.pointsL        = new Vector2[curveLength * 2];
            this.pointsR        = new Vector2[curveLength * 2];

            float angle = 0f;

            for (int i = 0; i < curveLength; i++)
            {
                if (i > 1 && i < roadLength - 2)
                {
                    float a0    = (float)Math.Atan((roadCurve[i - 1].Y - roadCurve[i].Y) / (roadCurve[i - 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                    float a1    = (float)Math.Atan((roadCurve[i + 1].Y - roadCurve[i].Y) / (roadCurve[i + 1].X - roadCurve[i].X)) - MathHelper.PiOver2;

                    angle       = (a0 + a1) / 2;
                }
                else if (i < 2)
                    angle       = (float)Math.Atan((roadCurve[i + 1].Y - roadCurve[i].Y) / (roadCurve[i + 1].X - roadCurve[i].X)) - MathHelper.PiOver2;
                else
                    angle       = (float)Math.Atan((roadCurve[i - 1].Y - roadCurve[i].Y) / (roadCurve[i - 1].X - roadCurve[i].X)) - MathHelper.PiOver2;

                float xl        = (offset + width) * (float)Math.Cos(angle);
                float yl        = (offset + width) * (float)Math.Sin(angle);
                float xr        = (offset - width) * (float)Math.Cos(angle);
                float yr        = (offset - width) * (float)Math.Sin(angle);

                pointsL[i]      = new Vector2(this.roadCurve[i].X + xl, this.roadCurve[i].Y + yl);
                pointsR[i]      = new Vector2(this.roadCurve[i].X + xr, this.roadCurve[i].Y + yr);
            }

            this.vertices       = new List<Vector3>();
            this.generateVertices(ref this.vertices, pointsL, pointsR, curveLength);

            this.textureUVs     = new List<Vector2>();
            this.generateTextureCoordinates(ref this.textureUVs, curveLength);

            this.elements       = new List<int>();
            this.generateElementArray(ref this.elements, curveLength);

            this.normals        = new List<Vector3>();
            this.generateNormals(ref normals, this.vertices.ToArray(), this.elements.ToArray());

            this.faces          = new List<Face>();
            for (int f = 0; f < elements.Count; f += 3)
            {
                try
                {
                    VertexIndices a = new VertexIndices((int)elements[f + 0], (int)elements[f + 0], (int)elements[f + 0]);
                    VertexIndices b = new VertexIndices((int)elements[f + 1], (int)elements[f + 1], (int)elements[f + 1]);
                    VertexIndices c = new VertexIndices((int)elements[f + 2], (int)elements[f + 2], (int)elements[f + 2]);
                    faces.Add(new Face(a, b, c));
                }
                catch { }
            }           
        }

        private void generateVertices(ref List<Vector3> vertices, Vector2[] pointsL, Vector2[] pointsR, int curveLength)
        {
            for (int i = 0; i < curveLength - 1; i++)
            {
                vertices.AddRange(
                    new Vector3[]{
                        new Vector3(pointsL[i].X,   height,     pointsL[i].Y),          //0 0
                        new Vector3(pointsR[i].X,   height,     pointsR[i].Y),          //1 0
                        new Vector3(pointsL[i+1].X, height,     pointsL[i+1].Y),        //0 1
                        new Vector3(pointsR[i+1].X, height,     pointsR[i+1].Y)});      //1 1
            }
            for (int i = curveLength - 1; i > 0; i--)
            {
                vertices.AddRange(
                    new Vector3[]{
                        new Vector3(pointsL[i].X,   height,     pointsL[i].Y),
                        new Vector3(pointsL[i].X,   worldHeight,          pointsL[i].Y),
                        new Vector3(pointsL[i-1].X, height,     pointsL[i-1].Y),
                        new Vector3(pointsL[i-1].X, worldHeight,          pointsL[i-1].Y)});
            }
            for (int i = 0; i < curveLength - 1; i++)
            {
                vertices.AddRange(
                    new Vector3[]{
                        new Vector3(pointsR[i].X,   height,     pointsR[i].Y),
                        new Vector3(pointsR[i].X,   worldHeight,          pointsR[i].Y),
                        new Vector3(pointsR[i+1].X, height,     pointsR[i+1].Y),
                        new Vector3(pointsR[i+1].X, worldHeight,          pointsR[i+1].Y)});
            }
        }
        private void generateVertices(ref Vector3[] vertices, Vector2[] pointsL, Vector2[] pointsR, int curveLength)
        {
            for (int i = 0; i < curveLength - 1; i++)
            {
                LaneMesh.Push<Vector3>(
                    new Vector3[]{
                        new Vector3(pointsL[i].X,   height,     pointsL[i].Y),          //0 0
                        new Vector3(pointsR[i].X,   height,     pointsR[i].Y),          //1 0
                        new Vector3(pointsL[i+1].X, height,     pointsL[i+1].Y),        //0 1
                        new Vector3(pointsR[i+1].X, height,     pointsR[i+1].Y)}, 
                    ref vertices);      //1 1
            }
            for (int i = curveLength - 1; i > 0; i--)
            {
                LaneMesh.Push<Vector3>(
                    new Vector3[]{
                        new Vector3(pointsL[i].X,   height,     pointsL[i].Y),
                        new Vector3(pointsL[i].X,   worldHeight,          pointsL[i].Y),
                        new Vector3(pointsL[i-1].X, height,     pointsL[i-1].Y),
                        new Vector3(pointsL[i-1].X, worldHeight,          pointsL[i-1].Y)},
                    ref vertices);
            }
            for (int i = 0; i < curveLength - 1; i++)
            {
                LaneMesh.Push<Vector3>(
                    new Vector3[]{
                        new Vector3(pointsR[i].X,   height,     pointsR[i].Y),
                        new Vector3(pointsR[i].X,   worldHeight,          pointsR[i].Y),
                        new Vector3(pointsR[i+1].X, height,     pointsR[i+1].Y),
                        new Vector3(pointsR[i+1].X, worldHeight,          pointsR[i+1].Y)},
                    ref vertices);
            }
        }

        private void generateTextureCoordinates(ref List<Vector2> textureCoordinates, int curveLength)
        {
            for (int i = 0; i < (curveLength - 1) * 3; i++)
            {
                textureCoordinates.AddRange(
                    new Vector2[]{
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(0, 1),
                        new Vector2(1, 1)});
            }
        }
        private void generateTextureCoordinates(ref Vector2[] textureCoordinates, int curveLength)
        {
            for (int i = 0; i < (curveLength - 1) * 3; i++)
            {
                LaneMesh.Push<Vector2>(
                    new Vector2[]{
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(0, 1),
                        new Vector2(1, 1)}, 
                    ref textureCoordinates);
            }
        }

        private void generateElementArray(ref List<int> elements, int curveLength)
        {
            for (int i = 0; i < (curveLength - 1) * 3; i++)
            {
                int off = i * 4;
                this.elements.AddRange(
                    new int[] {
                        off+0, off+1, off+2,
                        off+2, off+1, off+3});
            }
        }
        private void generateElementArray(ref int[] elements, int curveLength)
        {
            for (int i = 0; i < (curveLength - 1) * 3; i++)
            {
                int off = i * 4;
                LaneMesh.Push<int>(
                    new int[] {
                        off+0, off+1, off+2,
                        off+2, off+1, off+3},
                    ref elements);
            }
        }

        private void generateNormals(ref List<Vector3> normals, Vector3[] verts, int[] indices)
        {
            for(int i = 2; i < indices.Length; i++)
            {
                int index1 = indices[i - 2],
                    index2 = indices[i - 1],
                    index3 = indices[i];

                Vector3 normal = calcNormal(verts[index1], verts[index2], verts[index3]);

                if (normal.Y < 0)
                    normal *= -1.0f;

                normals.Add(normal); // NOTE:: Not the correnct function. Method should be 
                                     //         cumulative. See oriignal at NthDimension.Rendering.Drawables.Landscape.Terrain.generateNormals(...)
            }
        }
        private void generateNormals(ref Vector3[] normals, Vector3[] verts, int[] indices)
        {
            for (int i = 2; i < indices.Length; i++)
            {
                int index1 = indices[i - 2],
                    index2 = indices[i - 1],
                    index3 = indices[i];

                Vector3 normal = calcNormal(verts[index1], verts[index2], verts[index3]);

                if (normal.Y < 0)
                    normal *= -1.0f;

                //normals.Add(normal); // NOTE:: Not the correnct function. Method should be 
                //                     //         cumulative. See oriignal at NthDimension.Rendering.Drawables.Landscape.Terrain.generateNormals(...)
                LaneMesh.Push<Vector3>(
                    normal,
                    ref normals);
            }
        }

        private Vector3 calcNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 first = new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
            Vector3 second = new Vector3(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
            return Vector3.NormalizeFast(Vector3.Cross(first, second));
        }

        static int Push<T>(T value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + 1);
            values[values.Length - 1] = value;
            return values.Length - 1;
        }
        static void Push<T>(T[] value, ref T[] values)
        {
            Array.Resize<T>(ref values, values.Length + value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                values[values.Length - value.Length + i] = value[i];
            }
        }
    }
}
