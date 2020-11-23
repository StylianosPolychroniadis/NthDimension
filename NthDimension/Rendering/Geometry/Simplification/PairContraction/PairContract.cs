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

using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Geometry.Simplification
{
    // Implements algorithm from
    //https://www.cs.cmu.edu/~./garland/Papers/quadrics.pdf

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NthDimension.Algebra;

    using NthDimension.Rendering.Configuration;

    public class PairContract // : ProgressiveMesh
    {
        //public const float Level0Factor = 1f;       // 100% Detail - No reduction
        //public const float Level1Factor = 0.75f;    // 75%  Detail
        //public const float Level2Factor = 0.5f;     // 50% Detail
        //public const float Level3Factor = 0.05f;    //  5% Detail

        #region Fields
        private IDictionary<int, Matrix4d>              Q;                                                                  // The Q matrices for the vertices
        private IDictionary<int, Vector3>               vertices            = new SortedDictionary<int, Vector3>();          // The mesh vertices
        private IList<Face>                             faces;                                                              // The faces of the mesh
        private ISet<Pair>                              pairs               = new SortedSet<Pair>();                        // The vertex pairs, which are gradually contracted           
        private readonly double                         distanceThreshold;                                                  // The distance-based threshold for determining valid vertex pairs
        private IDictionary<int, ISet<Face>>            incidentFaces       = new Dictionary<int, ISet<Face>>();        // Saves references to each of its vertex facets
        private IDictionary<int, ISet<Pair>>            pairsPerVertex      = new Dictionary<int, ISet<Pair>>();            // Stores vertex pairs for each vertex to which he belongs
        private Stack<int>                              contractionIndices  = new Stack<int>();                             // Stack the original indices of the vertices that were contracted
        private Stack<VertexSplit>                      splits              = new Stack<VertexSplit>();                     // Stores VertexSplit entries to create a Progressive Mesh
        private bool                                    createSplitRecords;                                                 // Specifies whether to create VertexSplit entries
        private bool                                    strict;
        protected MeshVbo InputMesh;
        protected MeshVbo.MeshLod TargetLod;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the PairContract class
        /// </summary>
        /// <param name="opts">
        /// The arguments provided to the algorithm
        /// </param>
        public PairContract(MeshSettings opts) //: base(opts)
        {
            distanceThreshold       = opts.DistanceThreshold;
            createSplitRecords      = opts.SplitRecords;
            strict                  = opts.StrictMode;
            //Print("PairContract initialized with distanceThreshold = {0}", distanceThreshold);
        }
        #endregion

        #region IDisposable
        //public override void Dispose()
        //{
            
        //}
        #endregion

        #region Simplify
        /// <summary>
        /// Simplifies the specified input mesh
        /// </summary>
        /// <param name="input">
        /// The mesh to be simplified
        /// </param>
        /// <param name="targetFaceCount">
        /// The number of faces to which the generated simplification is intended
        /// </param>
        /// <param name="createSplitRecords">
        /// True to create vertex split entries for the mesh; Otherwise false
        /// </param>
        /// <param name="verbose">
        /// True to generate diagnostic output during simplification
        /// </param>
        /// <returns>
        /// The generated simplified mesh
        /// </returns>
        public  /*Mesh*/  void Simplify(ref MeshVbo input, MeshVbo.MeshLod lod, int targetFaceCount)
        {
            if (lod == MeshVbo.MeshLod.Level0) return;

            this.InputMesh      = input;
            this.TargetLod      = lod;

            // We start with the vertices and faces of the output mesh.
            faces = new List<Face>(input.MeshData.Faces);

            for (int i = 0; i < input.MeshData.PositionCache.Count; i++)
                vertices[i] = input.MeshData.PositionCache[i];
            
            // 1. Calculate the initial Q matrices for each vertex v
            Q = computeInitialQForEachVertex(strict);
            
            // 2. Determine all valid vertex pairs
            pairs = computeValidPairs();
            
            // 3. Iteratively, the pair contract with the least cost and accordingly update the cost of all affected couples
            while (faces.Count > targetFaceCount)
            {
                var pair = pairs.First();                 //		var pair = FindLeastCostPair();
                    ConsoleUtil.log(string.Format("   - Contracting pair ({0}, {1}) with contraction cost = {2}", pair.Vertex1, pair.Vertex2, pair.Cost.ToString("e")), false);
                contractPair(pair);
                ConsoleUtil.log(string.Format("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t", faces.Count), false);
                ConsoleUtil.log(string.Format("   - New polygon count: {0}", faces.Count), false);
            }

            ConsoleUtil.log(string.Format("   - Building Mesh {0} LOD {1}", input.Name, lod));
            
            // 4. Create and return a new mesh instance
            /*return*/ this.buildMesh(ref input);
        }

        /// <summary>
        /// Calculates the initial Q matrices for all vertices
        /// </summary>
        /// <param name="strict">
        /// True to throw an InvalidOperationException if a degenerate face is discovered
        /// </param>
        /// <returns>
        /// A map of the initial Q matrices
        /// </returns>
        /// <exception cref="InvalidOperationException">
		/// A degenerate face was found whose vertices are collinear
		/// </exception>
        private IDictionary<int, Matrix4d> computeInitialQForEachVertex(bool strict)
        {
            var q = new Dictionary<int, Matrix4d>();
            // Kp matrix for each plane, i.e. Each face
            var kp = computeKpForEachPlane(strict);
            // Q Initialize matrix with 0 for each vertex.
            for (int i = 0; i < vertices.Count; i++)
                q[i] = new Matrix4d();
            // Q is the sum of all Kp matrices of the incident facets of each vertex
            for (int c = 0; c < faces.Count; c++)
            {
                var f = faces[c];
                for (int i = 0; i < f.Vertex.Length; i++)
                {
q[ f.Vertex[i].Vi ] = q[ f.Vertex[i].Vi ] + kp[c];
                }
            }
            return q;
        }

        /// <summary>
        /// Computes the Kp matrix for the planes of all faces
        /// </summary>
        /// <param name="strict">
        /// True to throw an InvalidOperationException if a degenerate face is discovered
        /// </param>
        /// <returns>
        /// A list of Kp matrices
        /// </returns>
        private IList<Matrix4d> computeKpForEachPlane(bool strict)
        {
            var kp = new List<Matrix4d>();
            var degenerate = new List<Face>();
            foreach (var f in faces)
            {
                int idx1 = f.Vertex[0].Vi;
                int idx2 = f.Vertex[1].Vi;
                int idx3 = f.Vertex[2].Vi;


                var points = new[]
                {
                    InputMesh.MeshData.PositionCache[idx1].ToVector3d(),
                    InputMesh.MeshData.PositionCache[idx2].ToVector3d(),
                    InputMesh.MeshData.PositionCache[idx3].ToVector3d()
                };

                // Plane from the 3 local vectors
                var dir1 = points[1] - points[0];
                var dir2 = points[2] - points[0];
                var n = Vector3d.Cross(dir1, dir2);
                // If the cross product is the zero vector, the directional vectors are collinear, 
                // i.e. The vertices lie on a straight line and the face is degenerate
                if (n == Vector3d.Zero)
                {
                    degenerate.Add(f);
                    if (strict)
                    {
                        var msg = new StringBuilder()
                            .AppendFormat("Encountered degenerate face ({0} {1} {2})",
                                f.Vertex[0], f.Vertex[1], f.Vertex[2])
                            .AppendLine()
                            .AppendFormat("Vertex 1: {0}\n", points[0])
                            .AppendFormat("Vertex 2: {0}\n", points[1])
                            .AppendFormat("Vertex 3: {0}\n", points[2])
                            .ToString();
                        throw new InvalidOperationException(msg);
                    }
                }
                else
                {
                    n.Normalize();
                    var a = n.X;
                    var b = n.Y;
                    var c = n.Z;
                    var d = -Vector3d.Dot(n, points[0]);
                    // Please refer to [Gar97], Section 5 ("Deriving Error Quadrics").
                    var m = new Matrix4d()
                    {
                        M11 = a*a,
                        M12 = a*b,
                        M13 = a*c,
                        M14 = a*d,
                        M21 = a*b,
                        M22 = b*b,
                        M23 = b*c,
                        M24 = b*d,
                        M31 = a*c,
                        M32 = b*c,
                        M33 = c*c,
                        M34 = c*d,
                        M41 = a*d,
                        M42 = b*d,
                        M43 = c*d,
                        M44 = d*d
                    };
                    kp.Add(m);
                }
            }
            if (degenerate.Count > 0)
                ConsoleUtil.log(string.Format("   -- Warning: {0} degenerate faces found.", degenerate.Count));
            foreach (var d in degenerate)
                faces.Remove(d);
            return kp;
        }

        /// <summary>
        /// Calculates all valid vertex pairs
        /// </summary>
        /// <returns>
        /// The set of all valid vertex pairs
        /// </returns>
        ISet<Pair> computeValidPairs()
        {
            // 1. Criterion: 2 vertices are a contraction pair when they are connected by an edge
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                // Vertices of a triangle are each connected by edges and therefore are valid pairs.



var s = f.Vertex.Select(v => v.Vi).OrderBy(val => val).ToArray();
                for (int c = 0; c < s.Length; c++)
                {
                    if (!pairsPerVertex.ContainsKey(s[c]))
                        pairsPerVertex[s[c]] = new HashSet<Pair>();
                    if (!incidentFaces.ContainsKey(s[c]))
                        incidentFaces[s[c]] = new HashSet<Face>();
                    incidentFaces[s[c]].Add(f);
                }

                var p = computeMinimumCostPair(s[0], s[1]);
                if (pairs.Add(p))
                    ConsoleUtil.log(string.Format("   -- Added Vertexpair ({0}, {1})", s[0], s[1]), false);
                pairsPerVertex[s[0]].Add(p);
                pairsPerVertex[s[1]].Add(p);

                p = computeMinimumCostPair(s[0], s[2]);
                if (pairs.Add(p))
                    ConsoleUtil.log(string.Format("   -- Added Vertexpair ({0}, {1})", s[0], s[2]), false);
                pairsPerVertex[s[0]].Add(p);
                pairsPerVertex[s[2]].Add(p);

                p = computeMinimumCostPair(s[1], s[2]);
                if (pairs.Add(p))
                    ConsoleUtil.log(string.Format("   -- Added Vertexpair ({0}, {1})", s[1], s[2]), false);
                pairsPerVertex[s[1]].Add(p);
                pairsPerVertex[s[2]].Add(p);

            }
            // 2. Criterion: 2 vertices are a contraction pair when the Euclidean distance < Threshold parameter t
            if (distanceThreshold > 0)
            {
                // Check only if a threshold has been specified
                for (int i = 0; i < vertices.Count; i++)
                    for (int c = i + 1; c < vertices.Count; c++)
if ((vertices[i] - vertices[c]).Length < distanceThreshold)
                            if (pairs.Add(computeMinimumCostPair(i, c)))
                                ConsoleUtil.log(string.Format("   -- Added Vertexpair ({0}, {1})", i, c), false);
            }
            return pairs;
        }

        /// <summary>
        /// Determines the cost of the contraction of the specified vertices
        /// </summary>
        /// <param name="s">
        /// The First vertex of the Pair
        /// </param>
        /// <param name="t">
        /// The Second vertex of the pair
        /// </param>
        /// <returns>
        /// An instance of the pair class
        /// </returns>
        private Pair computeMinimumCostPair(int s, int t)
        {
            Vector3d target;
            double cost;
            var q = Q[s] + Q[t];
            // Refer to [Gar97], Section 4 ("Approximating Error With Quadrics").
            var m = new Matrix4d()
            {
                M11 = q.M11,
                M12 = q.M12,
                M13 = q.M13,
                M14 = q.M14,
                M21 = q.M12,
                M22 = q.M22,
                M23 = q.M23,
                M24 = q.M24,
                M31 = q.M13,
                M32 = q.M23,
                M33 = q.M33,
                M34 = q.M34,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };
            // If m is invertible, the optimal position can be determined.      // if (m.Determinant != 0) {

            try
            {
                // Determinant is not equal to 0 for invertible matrices.
                var inv = Matrix4d.Invert(m);
                target = new Vector3d(inv.M14, inv.M24, inv.M34);
                cost = computeVertexError(target, q);
            }
            catch (InvalidOperationException)
            {
                //			} else {
                // Otherwise, select the best value from the position of Vertex 1, Vertex 2, and Center.
                var v1 = vertices[s].ToVector3d();
                var v2 = vertices[t].ToVector3d(); ;
                var mp = new Vector3d()
                {
                    X = (v1.X + v2.X) / 2,
                    Y = (v1.Y + v2.Y) / 2,
                    Z = (v1.Z + v2.Z) / 2
                };
                var candidates = new[] {
                    new { cost = computeVertexError(v1, q), target = v1 },
                    new { cost = computeVertexError(v2, q), target = v2 },
                    new { cost = computeVertexError(mp, q), target = mp }
                };
                var best = (from p in candidates
                            orderby p.cost
                            select p).First();
                target = best.target;
                cost = best.cost;
            }
            return new Pair(s, t, target, cost);
        }

        /// <summary>
        /// Bestimmt den geometrischen Fehler des angegebenen Vertex in Bezug auf die Fehlerquadrik Q
        /// </summary>
        /// <param name="v">
        /// The vertex whose geometric error is to be determined
        /// </param>
        /// <param name="q">
        /// The underlying error line
        /// </param>
        /// <returns>
        /// The geometric error at the location of the given vertex
        /// </returns>
        private double computeVertexError(Vector3d v, Matrix4d q)
        {
            var h = new Vector4d(v, 1);
            // Geometrical error Δ(v) = vᵀQv.
            return Vector4d.Dot(Vector4d.Transform(h, q), h);
        }

        /// <summary>
        /// Contracts the specified vertex pair
        /// </summary>
        /// <param name="p">
        /// The vertex pair to be contracted
        /// </param>
        private void contractPair(Pair p)
        {
            if (createSplitRecords)
                addSplitRecord(p);

            // 1. Koordinaten von Vertex 1 werden auf neue Koordinaten abgeändert.
            vertices[p.Vertex1] = p.Target.ToVector3f();
            // 2. Matrix Q von Vertex 1 anpassen.
            Q[p.Vertex1] = Q[p.Vertex1] + Q[p.Vertex2];
            // 3. Alle Referenzen von Facetten auf Vertex 2 auf Vertex 1 umbiegen.
            var facesOfVertex2 = incidentFaces[p.Vertex2];
            var facesOfVertex1 = incidentFaces[p.Vertex1];
            var degeneratedFaces = new HashSet<Face>();

            // Jede Facette von Vertex 2 wird entweder Vertex 1 hinzugefügt, oder
            // degeneriert.
            foreach (var f in facesOfVertex2)
            {
                if (facesOfVertex1.Contains(f))
                {
                    degeneratedFaces.Add(f);
                }
                else
                {
                    // Indices umbiegen und zu faces von Vertex 1 hinzufügen.
                    for (int i = 0; i < f.Vertex.Length; i++)
                    {
                        if (f.Vertex[i].Vi == p.Vertex2)
                            f.Vertex[i].Vi = p.Vertex1;
                    }
                    facesOfVertex1.Add(f);
                }
            }
            // Nun degenerierte Facetten entfernen.
            foreach (var f in degeneratedFaces)
            {
                for (int i = 0; i < f.Vertex.Length; i++)
                    incidentFaces[f.Vertex[i].Vi].Remove(f);
                faces.Remove(f);
            }

            // Vertex 2 aus Map löschen.
            vertices.Remove(p.Vertex2);

            // Alle Vertexpaare zu denen Vertex 2 gehört dem Set von Vertex 1 hinzufügen.
            pairsPerVertex[p.Vertex1].UnionWith(pairsPerVertex[p.Vertex2]);
            // Anschließend alle Paare von Vertex 1 ggf. umbiegen und aktualisieren.
            var remove = new List<Pair>();
            foreach (var pair in pairsPerVertex[p.Vertex1])
            {
                // Aus Collection temporär entfernen, da nach Neuberechnung der Kosten
                // neu einsortiert werden muss.
                pairs.Remove(pair);
                int s = pair.Vertex1, t = pair.Vertex2;
                if (s == p.Vertex2)
                    s = p.Vertex1;
                if (t == p.Vertex2)
                    t = p.Vertex1;
                if (s == t)
                {
                    remove.Add(pair);
                }
                else
                {
                    var np = computeMinimumCostPair(Math.Min(s, t), Math.Max(s, t));
                    pair.Vertex1 = np.Vertex1;
                    pair.Vertex2 = np.Vertex2;
                    pair.Target = np.Target;
                    pair.Cost = np.Cost;

                    pairs.Add(pair);
                }
            }
            // "Degenerierte" Paare entfernen.
            foreach (var r in remove)
            {
                pairsPerVertex[p.Vertex1].Remove(r);
            }
        }

        /// <summary>
        /// Creates and stores a VertexSplit entry for the specified pair
        /// </summary>
        /// <param name="p">
        /// The pair for which a VertexSplit entry is to be created
        /// </param>
        private void addSplitRecord(Pair p)
        {
            contractionIndices.Push(p.Vertex2);
            var split = new VertexSplit()
            {
                S = p.Vertex1,
                SPosition = vertices[p.Vertex1].ToVector3d(),
                TPosition = vertices[p.Vertex2].ToVector3d()
            };
            foreach (var f in incidentFaces[p.Vertex2])
            {
                VertexIndex nv0;
                VertexIndex nv1;
                VertexIndex nv2;

                if (f.Vertex[0].Vi == p.Vertex2)
                    nv0 = new VertexIndex(-1);
                else
                    nv0 = f.Vertex[0];

                if (f.Vertex[1].Vi == p.Vertex2)
                    nv1 = new VertexIndex(-1);
                else
                    nv1 = f.Vertex[1];

                if (f.Vertex[2].Vi == p.Vertex2)
                    nv2 = new VertexIndex(-1);
                else
                    nv2 = f.Vertex[2];

                // -1 is later replaced by actual index, if this is known.
                split.Faces.Add(new Face(nv0, nv1, nv2));
            }
            splits.Push(split);
        }

        /// <summary>
        /// Creates a new mesh instance with the current vertex and facet data
        /// </summary>
        /// <returns>
        /// The generated mesh instance
        /// </returns>
        private void buildMesh(ref MeshVbo mesh)
        {
            // Mapping from old to new vertexindices for facets.
            var mapping = new Dictionary<int, int>();       // TODO:: I need int Key, <int Vi, int Ti, int Ni>
            int index = 0;

            //var verts = new List<Vertex>();
            var verts = new List<Vector3>();
            var _faces = new List<Face>();

            foreach (var p in vertices)
            {
                mapping.Add(p.Key, index++);
                verts.Add(p.Value);
            }

            


            foreach (var f in faces)
            {
                // TODO:: Check for Tri vs Quad
                #region Triangle
                if (f.Vertex.Length == 3)
                {
                    VertexIndex Va = new VertexIndex(mapping[f.Vertex[0].Vi]);
                    VertexIndex Vb = new VertexIndex(mapping[f.Vertex[1].Vi]);
                    VertexIndex Vc = new VertexIndex(mapping[f.Vertex[2].Vi]);


                    _faces.Add(new Face(Va, Vb, Vc));
                }
                #endregion

                #region Quad

                if (f.Vertex.Length == 4)
                {
                    throw new NotImplementedException("Build mesh does not yet support quads");
                }
                #endregion
            }
            // Progressive Mesh: Indices in mapping for "future" vertices
            foreach (var c in contractionIndices)
                mapping.Add(c, index++);

            int n = verts.Count;
            foreach (var s in splits)
            {
                var t = n++;
                s.S = mapping[s.S];

                foreach (var f in s.Faces)
                {
                    for (int i = 0; i < f.Vertex.Length; i++)
                    {
                        f.Vertex[i].Vi = f.Vertex[i].Vi < 0 ? 
                                t 
                            : 
                                mapping[f.Vertex[i].Vi];
                    }
                }
            }

            mesh.CurrentLod = this.TargetLod;

            mesh.MeshData.PositionCache.Clear();
            mesh.MeshData.PositionCache.AddRange(verts);

            mesh.MeshData.NormalCache = new ListVector3();
            mesh.MeshData.TextureCache = new ListVector2();

            foreach (var face in _faces)
            {
                mesh.MeshData.Faces.Add(face);

                Vector3[] vposition = new Vector3[3]
                {
                    verts[face.Vertex[0].Vi],
                    verts[face.Vertex[1].Vi],
                    verts[face.Vertex[2].Vi]
                };

                Vector3 v1 = vposition[1] - vposition[0];
                Vector3 v2 = vposition[2] - vposition[0];
                Vector3 fnormal = Vector3.Cross(v1, v2);

                mesh.MeshData.NormalCache.Add(fnormal);
                mesh.MeshData.TextureCache.Add(new Vector2(0f, 0f));
            }
        }
        #endregion
    }
}
