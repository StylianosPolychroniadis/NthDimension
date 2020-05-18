using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NthDimension.CalcEngine.Expressions;

namespace NthDimension.CalcEngine.Functions
{
    static class EsiLFactorFunctions
    {
        #region Dll P/Invoke
        [DllImport("C:\\ESI\\LIB\\ESI_LFAT.DLL", EntryPoint = "ESISVER")]
        public static extern void ESIDLL_LFactor_SVER(ref double Rx);

        [DllImport("C:\\ESI\\LIB\\ESI_LFAT.DLL",
        BestFitMapping = false,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Auto,
        EntryPoint = "EXLFACT",
        ExactSpelling = true,
        PreserveSig = true,
        SetLastError = false,
        ThrowOnUnmappableChar = true)]
        public static extern void EXLFACT(ref int NFLAGx, ref double XHHVx, double[] WFx, double[] DFx, ref double XLFACTx, ref int IEROUTx);
        #endregion

        public static void Register(CalculationEngine ce)
        {
            ce.RegisterFunction("LFACTOR", 14, LFACTOR);            
        }

        static object LFACTOR(List<Expression> p)
        {
            if(p.Count != 14)
                throw new Exception("Wrong number of arguments");

            double      lfactor         = 0d;
            int         NFLAG           = 1;
            int         IERNUM          = 0;
            double      XHHV            = 0d;
            double[]    WF              = new double[8];
            double[]    DF              = new double[4];
            double      XLFACT          = 0d;

            // Calculate the L Factor

            for (int wfi = 0; wfi < WF.Length; wfi++)
                WF[wfi] = 0d;

            for (int dfi = 0; dfi < DF.Length; dfi++)
                DF[dfi] = 0d;

            NFLAG = (int)p[0];

            XHHV = p[1];

            WF[0] = p[2];
            WF[1] = p[3];
            WF[2] = p[4];
            WF[3] = p[5];
            WF[4] = p[6];
            WF[5] = p[7];
            WF[6] = p[8];
            WF[7] = p[9];

            DF[0] = p[10];
            DF[1] = p[11];
            DF[2] = p[12];
            DF[3] = p[13];

            try
            {
                EXLFACT(ref NFLAG, ref XHHV, WF, DF, ref XLFACT, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                {
                    //LFACTOR = "#N/A " + IEROUT;
                    throw new Exception("LFactor Exception # " + IERNUM.ToString());
                }
                else
                {
                    lfactor = XLFACT;
                }
            }

            return lfactor;
        }
    }
}
