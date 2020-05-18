using System;
using System.Runtime.InteropServices;

using NthDimension.Compute;
using NthDimension.Algebra;

namespace NthStudio.Kernels
{
    public class Mandelbrot : Kernel
    {
        const string                    KERNEL_FILENAME         = "data/kernels/Mandelbrot.c";

        public ComputeBuffer<Vector4i>  cbuf_Rng;
        public ComputeBuffer<byte>      cbuf_Result;
        public byte[]                   h_resultBuf;        // Texture2D pixels
        private GCHandle                gc_resultBuffer;
        
        private uint                    width;
        private uint                    height;
        public float                    reMin      = -2.0f;
        public float                    reMax      = 1.0f;
        public float                    imMin      = -1.0f;
        public float                    imMax      = 1.0f;
        public uint                     maxIter    = 200;

        public Mandelbrot(ComputeDevice cDevice, string kernelSource,
                            uint width, uint height,
                            uint workers,
                            float reMin = -2.0f,
                            float reMax = 1.0f,
                            float imMin = -1.0f,
                            float imMax = 1.0f,
                            uint maxIter = 200)
            : base(cDevice, kernelSource)
        {
            KernelName      = "Mandelbrot";
            this.width      = width;
            this.height     = height;
            this.workers    = workers;
            this.reMin      = reMin;
            this.reMax      = reMax;
            this.imMin      = imMin;
            this.imMax      = imMax;
            this.maxIter    = maxIter; 

            h_resultBuf     = new byte[width * height * 4];
            gc_resultBuffer = GCHandle.Alloc(h_resultBuf, GCHandleType.Pinned);
        }

        public override void AllocateBuffers()
        {
            Random rnd = new Random((int)DateTime.UtcNow.Ticks);

            Vector4i[] seeds = new Vector4i[workers];
            for (int i = 0; i < workers; i++)
                seeds[i] =
                    new Vector4i
                    {
                        X = (ushort)rnd.Next(),
                        Y = (ushort)rnd.Next(),
                        Z = (ushort)rnd.Next(),
                        W = (ushort)rnd.Next()
                    };

            cbuf_Rng =
                new ComputeBuffer<Vector4i>(
                    clContext,
                    ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                    seeds);

            cbuf_Result =
                new ComputeBuffer<byte>(
                    clContext,
                    ComputeMemoryFlags.ReadOnly,
                    width * height * 4);
        }
        public override void ConfigureKernel()
        {
            clKernel.SetValueArgument(0, width);
            clKernel.SetValueArgument(1, height);
            clKernel.SetValueArgument(2, reMin);
            clKernel.SetValueArgument(3, reMax);
            clKernel.SetValueArgument(4, imMin);
            clKernel.SetValueArgument(5, imMax);
            clKernel.SetValueArgument(6, maxIter);
            clKernel.SetMemoryArgument(7, cbuf_Rng);
            clKernel.SetMemoryArgument(8, cbuf_Result);
        }
        public override void ReadResult()
        {
            clCommands.Read(cbuf_Result, true, 0, width * height * 4, gc_resultBuffer.AddrOfPinnedObject(), null);
            clCommands.Finish();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing)
            {
                cbuf_Result.Dispose();
                cbuf_Rng.Dispose();
            }
        }

        public void Initialize(int width, int height)
        {
          

        }
    }
}
