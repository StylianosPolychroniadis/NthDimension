using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NthDimension.CalcEngine.Expressions;

namespace NthDimension.CalcEngine.Functions
{
    public class EsiFlowFunctions
    {
        #region Dll P/Invoke
        [DllImport("C:\\ESI\\LIB\\ESI_FLOW.DLL", EntryPoint = "ESISVER")]
        public static extern void ESIDLL_LFactor_SVER(ref double Rx);

        [DllImport("C:\\ESI\\LIB\\ESI_FLOW.DLL",
        BestFitMapping = false,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Auto,
        EntryPoint = "EXFLOWDL",
        ExactSpelling = true,
        PreserveSig = true,
        SetLastError = false,
        ThrowOnUnmappableChar = true)]
        public static extern void ExFlowDL(int[] IFINPx, 
                                            double[] FFINPx,    //change to int
                                            ref double TZEROx, 
                                            double[] CCOMPx, 
                                            double[] FFOUTx, 
                                            ref int IEROUTx);
        
        #endregion

        public static void Register(CalculationEngine ce)
        {
            // TODO:: Decide what to do with the array function calls
            //          Perhaps a Cell Range?
            //ce.RegisterFunction("ExFlowWW", 4, 49, ExFlowWW);
            ce.RegisterFunction("ExFlowWW", 1, 49, ExFlowWW);
            ce.RegisterFunction("ExFlowQB", 1, 49, ExFlowQB);
            ce.RegisterFunction("ExFlowQG", 1, 49, ExFlowQG);
            ce.RegisterFunction("ExFlowMA", 1, 49, ExFlowMA);
            ce.RegisterFunction("ExFlowCD", 1, 49, ExFlowCD);
            ce.RegisterFunction("ExFlowSG", 1, 49, ExFlowSG);
            ce.RegisterFunction("ExFlowGrsM", 1, 49, ExFlowGrsM);
            ce.RegisterFunction("ExFlowNetM", 1, 49, ExFlowNetM);
            ce.RegisterFunction("ExFlowGrsQ", 1, 49, ExFlowGrsQ);
            ce.RegisterFunction("ExFlowNetQ", 1, 49, ExFlowNetQ);
        }


        static object ExFlowWW(List<Expression> p)
        {
            // ====================================
            // ExFlowWW() = FFOUT(01) = Mass Flow            (lbm/hr  or kg/sec)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];


            try
            {
                ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                
            }
            //for (int i = 0; i < rFFOUT.Length; i++ )
            //    yield return rFFOUT[i];
            //return tally;
            return rFFOUT[0];
        }
        static object ExFlowQB(List<Expression> p)
        {
            // ====================================
            // ExFlowQB() = FFOUT(02) = Liquid Vol. Flow     (gal/min or m3/min)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 2);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[1];
        }
        static object ExFlowQG(List<Expression> p)
        {
            // ====================================
            // ExFlowQG() = FFOUT(03) = Nat. Gas Vol. Flow   (ft3/hr  or m3/sec)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 3);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[2];
        }
        static object ExFlowMA(List<Expression> p)
        {
            // ====================================
            // ExFlowMA() = FFOUT(04) = Throat Mach Number
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 4);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[3];
        }
        static object ExFlowCD(List<Expression> p)
        {
            // ====================================
            // ExFlowCD() = FFOUT(05) = Discharge Coeficient
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 5);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[4];
        }
        static object ExFlowSG(List<Expression> p)
        {
            // ====================================
            // ExFlowSG() = FFOUT(06) = Natural Gas Specific Gravity
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 6);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[5];
        }
        static object ExFlowGrsM(List<Expression> p)
        {
            // ====================================
            // ExFlowGrsM() = FFOUT(07) = Discharge Coeficient
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 7);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[6];
        }
        static object ExFlowNetM(List<Expression> p)
        {
            // ====================================
            // ExFlowNetM() = FFOUT(08) = Natural Gas Net  CV(P) (Btu/lbm or kJ/kg)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 8);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[7];
        }
        static object ExFlowGrsQ(List<Expression> p)
        {
            // ====================================
            // ExFlowGrsQ() = FFOUT(09) = Natural Gas Gross CV(P) (Btu/ft3 or kJ/m3)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 9);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[8];
        }
        static object ExFlowNetQ(List<Expression> p)
        {
            // ====================================
            // ExFlowNetQ() = FFOUT(10) = Natural Gas Net  CV(P) (Btu/ft3 or kJ/m3)
            // ====================================

            var tally = new Tally();
            foreach (Expression e in p)
            {
                tally.Add(e);
            }

            double TZERO = 32.174d;//60.0d;

            int IERRNUM = 0;

            int[] rIFINP = new int[6];
            double[] rFFINP = new double[10];
            double[] rCCOMP = new double[21];
            double[] rFFOUT = new double[12];


            rIFINP[0] = (int)tally.Values[0];
            rIFINP[1] = (int)tally.Values[1];
            rIFINP[2] = (int)tally.Values[2];
            rIFINP[3] = (int)tally.Values[3];
            rIFINP[4] = (int)tally.Values[4];
            rIFINP[5] = (int)tally.Values[5];

            rFFINP[0] = tally.Values[6];
            rFFINP[1] = tally.Values[7];
            rFFINP[2] = tally.Values[8];
            rFFINP[3] = tally.Values[9];
            rFFINP[4] = tally.Values[10];
            rFFINP[5] = tally.Values[11];
            rFFINP[6] = tally.Values[12];
            rFFINP[7] = tally.Values[13];
            rFFINP[8] = tally.Values[14];
            rFFINP[9] = tally.Values[15];

            rCCOMP[0] = tally.Values[16];
            rCCOMP[1] = tally.Values[17];
            rCCOMP[2] = tally.Values[18];
            rCCOMP[3] = tally.Values[19];
            rCCOMP[4] = tally.Values[20];
            rCCOMP[5] = tally.Values[21];
            rCCOMP[6] = tally.Values[22];
            rCCOMP[7] = tally.Values[23];
            rCCOMP[8] = tally.Values[24];
            rCCOMP[9] = tally.Values[25];
            rCCOMP[10] = tally.Values[26];
            rCCOMP[11] = tally.Values[27];
            rCCOMP[12] = tally.Values[28];
            rCCOMP[13] = tally.Values[29];
            rCCOMP[14] = tally.Values[30];
            rCCOMP[15] = tally.Values[31];
            rCCOMP[16] = tally.Values[32];
            rCCOMP[17] = tally.Values[33];
            rCCOMP[18] = tally.Values[34];
            rCCOMP[19] = tally.Values[35];
            rCCOMP[20] = tally.Values[36];

            rFFOUT[0] = tally.Values[37];
            rFFOUT[1] = tally.Values[38];
            rFFOUT[2] = tally.Values[39];
            rFFOUT[3] = tally.Values[40];
            rFFOUT[4] = tally.Values[41];
            rFFOUT[5] = tally.Values[42];
            rFFOUT[6] = tally.Values[43];
            rFFOUT[7] = tally.Values[44];
            rFFOUT[8] = tally.Values[45];
            rFFOUT[9] = tally.Values[46];
            rFFOUT[10] = tally.Values[47];
            rFFOUT[11] = tally.Values[48];

            //ExFlow_Values(rIFINP, rFFINP, rCCOMP, rFFOUT, 10);
            ExFlowDL(rIFINP, rFFINP, ref TZERO, rCCOMP, rFFOUT, ref IERRNUM);

            return rFFOUT[9];
        }
        
    }
}
