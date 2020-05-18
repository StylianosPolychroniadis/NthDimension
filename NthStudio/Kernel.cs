using NthDimension.Compute;
using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public class Kernel
    {
        internal ComputePlatform                      clPlatform;
        internal ComputeContext                       clContext;
        internal ComputeContextPropertyList           clProperties;
        internal ComputeKernel                        clKernel;
        internal ComputeProgram                       clProgram;
        internal ComputeCommandQueue                  clCommands;
        internal uint                                 workers;

        public string                               KernelName          = "New Kernel";

        

        public Kernel(ComputeDevice cDevice, string kernelSource)
        {
            clPlatform      = cDevice.Platform;
            clProperties    = new ComputeContextPropertyList(clPlatform);
            clContext       = new ComputeContext(clPlatform.Devices, clProperties, null, IntPtr.Zero);
            clCommands      = new ComputeCommandQueue(clContext, cDevice, ComputeCommandQueueFlags.None);
            clProgram       = new ComputeProgram(clContext, new string[] { kernelSource });

            int i = kernelSource.IndexOf("__kernel");
            if (i > -1)
            {
                int j = kernelSource.IndexOf("(", i);
                if (j > -1)
                {
                    string raw = kernelSource.Substring(i + 8, j - i - 8);
                    string[] parts = raw.Trim().Split(' ');
                    for (int k = parts.Length - 1; k != 0; k--)
                    {
                        if (!string.IsNullOrEmpty(parts[k]))
                        {
                            KernelName = parts[k];
                            break;
                        } // if
                    } // for k
                } // if j
            } // if i
        }

        public void BuildKernels()
        {
            string msg = null;
            try
            {
                clProgram.Build(null, "", null, IntPtr.Zero);
                clKernel = clProgram.CreateKernel(KernelName);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            if (clKernel == null)
                throw new Exception(msg);
        }

        public virtual void AllocateBuffers() { }
        public virtual void ConfigureKernel() { }
        public virtual void ExecuteKernel()
        {
            clCommands.Execute(clKernel, null, new long[] { workers }, null, null);
        }
        public void FinishKernel()
        {
            clCommands.Finish();
        }
        public virtual void ReadResult()
        {
            
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                clCommands.Dispose();
                clKernel.Dispose();
                clProgram.Dispose();
                clContext.Dispose();
                
            }
        }
    }
}
