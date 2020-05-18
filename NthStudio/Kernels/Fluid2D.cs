namespace NthStudio.Kernels
{
    using NthDimension.Algebra;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Fluid_Node
    {
        public Vector3 pos;
        public Vector3 normal;
        public int r, g, b;
        public Color color;

        public Fluid_Node()
        {
            pos = new Vector3();
            color = new Color();
            float Temperature = 0f;
            float Pressure = 0f;
        }
    }

    public class Fluid2D
    {
        public float timestep = 0.0166667f;
        public float diffuseRate = 0.0005f;
        public float viscosityRate = 0.00001f;
        private Vector3 gravity = new Vector3(0, 9.8f, 0);
        public Fluid_Node[,] smoke;
        public static int NumOfNodes;
        private int[] indices;

        public bool ShowVelocity = false;
        public bool IsBoundryD = true;
        public bool IsBoundryV = true;
        public bool IsBoundryU = false;
        public bool SpecialBoundryProject = false;
        public int BoundryD = 0;
        public int BoundryU = 1;
        public int BoundryV = 2;
        private float[,] density;
        private float[,] density_prev;
        private float[,] v;
        private float[,] v_prev;
        private float[,] u;
        private float[,] u_prev;
        public float VelocitySourceCenter = (85.0f);
        public float DensitySourceCenter = (90.0f);

        public Fluid2D(int Nodes)
        {
            NumOfNodes = Nodes;
            ClearData(NumOfNodes);
        }

        public void UpdateFrame()
        {
            for (int i = 0; i < NumOfNodes; i++)
                for (int j = 0; j < NumOfNodes; j++)
                {
                    v_prev[i, j] = (0.0f);
                    u_prev[i, j] = (0.0f);
                    density_prev[i, j] = 0.0f;
                }
            density_prev[2, NumOfNodes / 2 - 2] = DensitySourceCenter * 0.8f;
            density_prev[2, NumOfNodes / 2 - 1] = DensitySourceCenter * 0.9f;
            density_prev[2, NumOfNodes / 2] = DensitySourceCenter;
            density_prev[2, NumOfNodes / 2 + 1] = DensitySourceCenter * 0.9f;
            density_prev[2, NumOfNodes / 2 + 2] = DensitySourceCenter * 0.8f;
            u_prev[2, NumOfNodes / 2 - 2] = VelocitySourceCenter * 0.3f;
            u_prev[2, NumOfNodes / 2 - 1] = VelocitySourceCenter * 0.6f;
            u_prev[2, NumOfNodes / 2] = VelocitySourceCenter;
            u_prev[2, NumOfNodes / 2 + 1] = VelocitySourceCenter * 0.6f;
            u_prev[2, NumOfNodes / 2 + 2] = VelocitySourceCenter * 0.3f;
            VelocityStep();
            DensityStep();
            CalcColor();
        }
        internal void ResetToSize(int n)
        {
            ClearData(n);
            for (int i = NumOfNodes - 13; i < NumOfNodes - 11; i++)
                for (int j = NumOfNodes - 13; j < NumOfNodes - 11; j++)
                {
                    density[i, j] = 2000.0f;
                }

        }

        /// <summary>
        ///Override and implement graphics API (eg OpenGL, DirectX, GDI+, etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnVertexBufferCreate(object sender, EventArgs e)
        {
            int totalNodes = NumOfNodes * NumOfNodes;

#if DIRECTX
                VertexBuffer                            buffer = (VertexBuffer)sender;
                CustomVertex.PositionNormalColored[]    verts = new CustomVertex.PositionNormalColored[totalNodes];
           
                int counter = 0;
                for (int i = 0; i < NumOfNodes; i++)
                    for (int j = 0; j < NumOfNodes; j++)
                    {
                        verts[counter].Position = smoke[i, j].pos;
                        verts[counter].Normal = smoke[i, j].normal;
                        verts[counter].Color = smoke[i, j].color.ToArgb();
                  
                        counter++;
                    }
                buffer.SetData(verts, 0, LockFlags.None);
#endif
        }
        /// <summary>
        ///Override and implement graphics API (eg OpenGL, DirectX, GDI+, etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnIndexBufferCreate(object sender, EventArgs e)
        {
#if DIRECTX
                IndexBuffer buffer = (IndexBuffer)sender;
                int counter = 0;
                for (int j = 0; j < NumOfNodes - 1; j++)
                    for (int i = 0; i < NumOfNodes - 1; i++)
                    {
                        indices[counter] = i + j * (NumOfNodes); //v1
                        counter++;
                        indices[counter] = i + 1 + j * (NumOfNodes); //v2
                        counter++;
                        indices[counter] = i + 1 + (j + 1) * (NumOfNodes); //v4
                        counter++;

                        indices[counter] = i + j * (NumOfNodes); //v1
                        counter++;
                        indices[counter] = i + 1 + (j + 1) * (NumOfNodes); //v4
                        counter++;
                        indices[counter] = i + (j + 1) * (NumOfNodes); //v3
                        counter++;
                    }
                buffer.SetData(indices, 0, LockFlags.None);
#endif
        }

        private void ClearData(int Nodes)
        {
            NumOfNodes = Nodes;
            smoke = new Fluid_Node[Nodes, Nodes];
            indices = new int[(NumOfNodes - 1) * (NumOfNodes - 1) * 6];
            density = new float[Nodes, Nodes];
            density_prev = new float[Nodes, Nodes];
            v = new float[Nodes, Nodes];
            v_prev = new float[Nodes, Nodes];
            u = new float[Nodes, Nodes];
            u_prev = new float[Nodes, Nodes];
            for (int i = 0; i < NumOfNodes; i++)
                for (int j = 0; j < NumOfNodes; j++)
                {
                    smoke[i, j] = new Fluid_Node();
                }

            for (int i = 0; i < NumOfNodes; i++)
                for (int j = 0; j < NumOfNodes; j++)
                {
                    smoke[i, j].pos.X = (float)(j) / (float)(NumOfNodes) * 14.0f;
                    smoke[i, j].pos.Y = (float)(i) / (float)(NumOfNodes) * 14.0f;
                    smoke[i, j].pos.Z = (float)(-2.0f);
                    u[i, j] = (float)(0.0f);
                    v[i, j] = (float)(0.0f);
                    u_prev[i, j] = 0.0f;
                    v_prev[i, j] = 0.0f;
                    density[i, j] = 0.0f;
                    density_prev[i, j] = 0.0f;
                }

#if DIRECTX
                vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), NumOfNodes * NumOfNodes, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
                vb.Created += new EventHandler(this.OnVertexBufferCreate);
                OnVertexBufferCreate(vb, null);

                ib = new IndexBuffer(typeof(int), indices.Length, device, Usage.WriteOnly, Pool.Default);
                ib.Created += new EventHandler(this.OnIndexBufferCreate);
                OnIndexBufferCreate(ib, null);
#endif
        }
        void LinearSolver(int b, float a, float c, ref float[,] current, float[,] previous)
        {
            for (int k = 0; k < 20; k++)
            {
                for (int i = 1; i < NumOfNodes - 1; i++)
                    for (int j = 1; j < NumOfNodes - 1; j++)
                    {
                        current[i, j] =
                            (
                            previous[i, j] +
                            a * (current[i - 1, j] + current[i + 1, j] +
                            current[i, j - 1] + current[i, j + 1])
                            ) / c;
                    }
                set_bnd(b, ref current);
            }
        }
        void set_bnd(int b, ref float[,] x)
        {
            int i;
            if ((b == 0) && (!IsBoundryD)) return;
            if ((b == 1) && (!IsBoundryU)) return;
            if ((b == 2) && (!IsBoundryV)) return;
            for (i = 1; i <= NumOfNodes - 2; i++)
            {
                if (b == 1)
                {
                    x[0, i] = -x[1, i];
                    x[NumOfNodes - 1, i] = -x[NumOfNodes - 2, i];
                }
                else
                {
                    x[0, i] = x[1, i];
                    x[NumOfNodes - 1, i] = x[NumOfNodes - 2, i];
                }
                if (b == 2)
                {
                    x[i, 0] = -x[i, 1];
                    x[i, NumOfNodes - 1] = -x[i, NumOfNodes - 2];
                }
                else
                {
                    x[i, 0] = x[i, 1];
                    x[i, NumOfNodes - 1] = x[i, NumOfNodes - 2];
                }
            }

            x[0, 0] = 0.5f * (x[1, 0] + x[0, 1]);
            x[0, NumOfNodes - 1] = 0.5f * (x[1, NumOfNodes - 1] + x[0, NumOfNodes - 2]);
            x[NumOfNodes - 1, 0] = 0.5f * (x[NumOfNodes - 2, 0] + x[NumOfNodes - 1, 1]);
            x[NumOfNodes - 1, NumOfNodes - 1] = 0.5f * (x[NumOfNodes - 2, NumOfNodes - 1] + x[NumOfNodes - 1, NumOfNodes - 2]);

        }

        private void add_source(ref float[,] current, float[,] previous)
        {
            for (int i = 0; i < NumOfNodes; i++)
                for (int j = 0; j < NumOfNodes; j++)
                    current[i, j] += timestep * previous[i, j];
        }
        void project(ref float[,] u, ref float[,] v, ref float[,] u_prev, ref float[,] v_prev)
        {
            for (int i = 1; i < NumOfNodes - 1; i++)
                for (int j = 1; j < NumOfNodes - 1; j++)
                {
                    v_prev[i, j] = -0.5f * (u[i + 1, j] - u[i - 1, j] +
                        v[i, j + 1] - v[i, j - 1]) / (NumOfNodes - 2);
                    u_prev[i, j] = 0;
                }
            if (SpecialBoundryProject)
            {
                set_bnd(BoundryU, ref v_prev); set_bnd(BoundryV, ref u_prev);
            }
            else
            {
                set_bnd(BoundryD, ref v_prev); set_bnd(BoundryD, ref u_prev);
            }

            LinearSolver(0, 1, 4, ref u_prev, v_prev);

            for (int i = 1; i < NumOfNodes - 1; i++)
                for (int j = 1; j < NumOfNodes - 1; j++)
                {
                    u[i, j] -= 0.5f * (NumOfNodes - 2) * (u_prev[i + 1, j] - u_prev[i - 1, j]);
                    v[i, j] -= 0.5f * (NumOfNodes - 2) * (u_prev[i, j + 1] - u_prev[i, j - 1]);
                }
            set_bnd(BoundryU, ref u); set_bnd(BoundryV, ref v);
        }
        void Advection(int b, ref float[,] current, float[,] previous, float[,] u, float[,] v)
        {
            int i0, j0, i1, j1;
            float x, y, s0, t0, s1, t1, dt0;
            dt0 = timestep * (NumOfNodes - 2);
            for (int i = 1; i < NumOfNodes - 1; i++)
                for (int j = 1; j < NumOfNodes - 1; j++)
                {
                    x = i - dt0 * u[i, j];
                    y = j - dt0 * v[i, j];
                    if (x < 0.5f)
                    {
                        x = 0.5f;
                    }
                    if (x > NumOfNodes - 2 + 0.5f)
                    {
                        x = NumOfNodes - 2 + 0.5f;
                    }
                    i0 = (int)(x);

                    i1 = i0 + 1;
                    if (y < 0.5f)
                    {
                        y = 0.5f;
                    }
                    if (y > NumOfNodes - 2 + 0.5f)
                    {
                        y = NumOfNodes - 2 + 0.5f;
                    }
                    j0 = (int)(y);

                    j1 = j0 + 1;
                    s1 = x - i0;
                    s0 = 1 - s1;
                    t1 = y - j0;
                    t0 = 1 - t1;
                    current[i, j] = s0 * (t0 * previous[i0, j0] + t1 * previous[i0, j1]) +
                     s1 * (t0 * previous[i1, j0] + t1 * previous[i1, j1]);
                }
            set_bnd(b, ref current);
        }
        void SwapArrays(ref float[,] a, ref float[,] b)
        {
            float[,] temp = a;
            a = b;
            b = temp;
        }
        void Diffuse(int b, ref float[,] current, float[,] prev, float damping)
        {
            float a = damping * (NumOfNodes - 2) * (NumOfNodes - 2) * timestep;
            LinearSolver(b, a, 1 + 4 * a, ref current, prev);
        }
        void VelocityStep()
        {
            add_source(ref u, u_prev);
            add_source(ref v, v_prev);
            SwapArrays(ref u, ref u_prev);
            Diffuse(BoundryU, ref u, u_prev, viscosityRate);
            SwapArrays(ref v, ref v_prev);
            Diffuse(BoundryV, ref v, v_prev, viscosityRate);
            project(ref u, ref v, ref u_prev, ref v_prev);
            SwapArrays(ref u, ref u_prev);
            SwapArrays(ref v, ref v_prev);
            Advection(BoundryU, ref u, u_prev, u_prev, v_prev);
            Advection(BoundryV, ref v, v_prev, u_prev, v_prev);
            project(ref u, ref v, ref u_prev, ref v_prev);
        }
        void DensityStep()
        {
            add_source(ref density, density_prev);
            SwapArrays(ref density, ref density_prev);
            Diffuse(BoundryD, ref density, density_prev, diffuseRate);
            SwapArrays(ref density, ref density_prev);
            Advection(BoundryD, ref density, density_prev, u, v);
        }

        #region Draw
        public void DrawFluid(Graphics g)
        {
            this.UpdateFrame();
            this.DrawSmoke();
        }
        public void DrawSmoke()
        {
#if DIRECTX
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                //device.VertexFormat = VertexFormats.Position;
                device.SetStreamSource(0, vb, 0);
                device.Indices = ib;
                device.Transform.World *= Matrix.Translation(0, 0, 0);
                //device.RenderState.FillMode = FillMode.WireFrame;
                device.SetTexture(0, Tex);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (NumOfNodes) * (NumOfNodes), 0, indices.Length / 3);

                device.RenderState.SourceBlend = Blend.SourceAlpha; //set the source to be the source's alpha
                device.RenderState.DestinationBlend = Blend.InvSourceAlpha; //set the destination to be the inverse of the source's alpha
                device.RenderState.AlphaBlendEnable = true; //enable


                if (ShowVelocity)
                {
                    CustomVertex.PositionNormalColored[] verts = new CustomVertex.PositionNormalColored[2];
                    for (int i = 1; i < NumOfNodes - 1; i++)
                        for (int j = 1; j < NumOfNodes - 1; j++)
                        {
                            verts[0].Position = new Vector3(smoke[i, j].pos.X, smoke[i, j].pos.Y, smoke[i, j].pos.Z);
                            verts[0].Color = Spectrum(smoke[i, j].r).ToArgb();   //System.Drawing.Color.White.ToArgb();
                            verts[1].Position = new Vector3(smoke[i, j].pos.X + v[i, j], smoke[i, j].pos.Y + u[i, j], smoke[i, j].pos.Z);
                            verts[1].Color = Spectrum(smoke[i, j].r).ToArgb();   //System.Drawing.Color.Aqua.ToArgb();
                            device.DrawUserPrimitives(PrimitiveType.LineList, 1, verts);

                        }
                }
#endif


        }
        private void CalcColor()
        {
            for (int i = 1; i < NumOfNodes - 1; i++)
                for (int j = 1; j < NumOfNodes - 1; j++)
                {
                    smoke[i, j].r = ((int)((density[i, j]) * 10));

                    if (smoke[i, j].r > 255) { smoke[i, j].r = 255; }

                    if (smoke[i, j].r < 0) { smoke[i, j].r = 0; }

                    //if(smoke[i, j].r / 3 > )
                    //int r = 

                    //smoke[i, j].color = Spectrum(smoke[i, j].r);

                    smoke[i, j].color = System.Drawing.Color.FromArgb(smoke[i, j].r, 255, smoke[i, j].r, 0);
                    //(255 - smoke[i, j].r, 255 - smoke[i, j].r, 255 - smoke[i, j].r, 255 - smoke[i, j].r);


                }
            OnVertexBufferCreate(/*vb*/ this, null);
        }
        private Color Spectrum(int Percent)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            if (Percent <= 25)
            {
                r = 0;
                g = Percent / 25 * 255;
                b = 255;
                return Color.FromArgb(r, g, b);
            }
            if (Percent <= 50)
            {
                r = 0;
                g = 255;
                b = (50 - Percent) / 50 * 255;
                return Color.FromArgb(r, g, b);
            }
            if (Percent <= 75)
            {
                r = Percent / 75 * 255;
                g = 255;
                b = 0;
                return Color.FromArgb(r, g, b);
            }
            if (Percent <= 100)
            {
                r = 255;
                g = (100 - Percent) / 100 * 255;
                b = 0;
                return Color.FromArgb(r, g, b);
            }


            return Color.FromArgb(r, g, b);
        }
        #endregion Draw

        internal void AddRandomVelocity()
        {
            Random rnd = new Random();
            int x = rnd.Next(NumOfNodes - 2) + 1;
            int y = rnd.Next(NumOfNodes - 2) + 1;
            v[x, y] = (float)(x + y) * (rnd.Next(20) - 10);
            u[x, y] = (float)(x + y) * (rnd.Next(20) - 10);
        }
        internal void AddRandomDensity()
        {
            Random rnd = new Random();
            int x = rnd.Next(NumOfNodes - 2);
            int y = rnd.Next(NumOfNodes - 2);
            density[x, y] = 345.0f;
        }
        internal void RemoveRandomDensity()
        {
            Random rnd = new Random();
            int x = rnd.Next(NumOfNodes - 2);
            int y = rnd.Next(NumOfNodes - 2);
            density[x, y] = -345.0f;

        }
    }

}
