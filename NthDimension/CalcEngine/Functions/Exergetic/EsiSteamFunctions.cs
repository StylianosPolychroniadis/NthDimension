// Version 1    Oct-26-2015     Stylianos Polychroniadis

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NthDimension.CalcEngine.Expressions;

// Switching to .EXE call
using NthDimension.Net.UserInterface.Controls;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using FortranIO.EsiMaps;
using System.Diagnostics;

namespace NthDimension.CalcEngine.Functions
{
    #region General Development Notes
    // P is Pressure
    // T is Temperature
    // V is Specific Volume
    // C is Nozzle Constant
    // X is Quality
    // G is Exergy
    // S is Entropy
    // H is Enthapy
    // U is Internal Energy

    //ISFLAG = 1; // For StmPTz    F(1) = Pressure, F(2) = Temperature
    //ISFLAG = 2; // For StmPHz    F(1) = Pressure, F(3) = Enthalpy
    //ISFLAG = 3; // For StmPXz    F(1) = Pressure, F(4) = Quality
    //ISFLAG = 4; // For StmPVz    F(1) = Pressure, F(5) = Specific Volume
    //ISFLAG = 5; // For StmPSz    F(1) = Pressure, F(7) = Entropy
    //ISFLAG = 6; // For StmTXz    F(2) = Temperature, F(4) = XQuality
    //ISFLAG = 7; // For StmTVz    F(2) = Temperature, F(5) = Specific Volume
    //ISFLAG = 8; // for StmPCz    F(1) = Pressure, F(9) = Nozzle Constant, F(3) = Enthalpy Estiment

    // F's  1-Press, 2-Temp 3-Enthalpy, 4-Quality, 5-Specific Volume, 6-Exergy, 7-Entropy, 8-Internal energy, 9-nozzle constant
    #endregion

    static class EsiSteamFunctions
    {
        #region Dll P/Invoke
        [DllImport("C:\\ESI\\LIB\\ESI_STM.DLL", EntryPoint = "ESISVER", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern void ESIDLL_StmProp_SVER(ref double Rx);

        [DllImport("C:\\ESI\\LIB\\ESI_STM.DLL",
        //BestFitMapping = false,
        CallingConvention = CallingConvention.Cdecl,
        //CharSet = CharSet.Auto,
        //EntryPoint = "EXPROP",
        EntryPoint = "EXPROP")
        //ExactSpelling = true,
        //PreserveSig = true,
        //SetLastError = false,
        //ThrowOnUnmappableChar = true)
        ]
        public static extern void EXPROP(ref int KUSAVx, ref int ISFLAGx, double[] Fx, ref double TZEROx, ref int IERNUMx);
        #endregion

        private static string pathToExe = Path.Combine(Path.Combine(Properties.Settings.Default.rootDisk, @"MAPS\EX-FOSS\HEATRATE\" + Properties.Settings.Default.plantName), "ESIprop.exe");
        private static string pathToFile = Path.Combine(Path.Combine(Properties.Settings.Default.rootDisk, @"MAPS\EX-FOSS\HEATRATE\" + Properties.Settings.Default.plantName), "ESIprop.DAT");

        private static string p0    = "KU:";
        private static string p1    = "ISFLAG:";
        private static string p2    = "IERNUM:";
        private static string p3    = "Pressure:";
        private static string p4    = "FluidTemp:";
        private static string p5    = "Wet Bulb:";
        private static string p6    = "Dew Point:";
        private static string p7    = "QualityRH:";
        private static string p8    = "Enthalpy:";
        private static string p9    = "Entropy:";
        private static string p10   = "Sp Humidity:";

        public static void Register(CalculationEngine ce)
        {

            //ce.RegisterFunction("StmPTH", 3, StmPTH);
            //ce.RegisterFunction("StmPTV", 3, StmPTV);
            //ce.RegisterFunction("StmPSH", 3, StmPSH);
            //ce.RegisterFunction("StmPHS", 3, StmPHS);
            //ce.RegisterFunction("StmPHT", 3, StmPHT);
            //ce.RegisterFunction("StmPHX", 3, StmPHX);
            //ce.RegisterFunction("StmPTS", 3, StmPTS);
            //ce.RegisterFunction("StmPXH", 3, StmPXH);
            //ce.RegisterFunction("StmPXT", 3, StmPXT);
            //ce.RegisterFunction("StmTXP", 3, StmTXP);

            ce.RegisterFunction("StmPTH", 3, fStmPTH);
            //ce.RegisterFunction("StmPTV", 3, fStmPTV);
            ce.RegisterFunction("StmPSH", 3, fStmPSH);
            ce.RegisterFunction("StmPHS", 3, fStmPHS);
            ce.RegisterFunction("StmPHT", 3, fStmPHT);
            ce.RegisterFunction("StmPHX", 3, fStmPHX);
            ce.RegisterFunction("StmPTS", 3, fStmPTS);
            ce.RegisterFunction("StmPXH", 3, fStmPXH);
            ce.RegisterFunction("StmPXT", 3, fStmPXT);
            ce.RegisterFunction("StmTXP", 3, fStmTXP);
        }

        static object StmPTH(List<Expression> p)
        {
            // Should never be hit
            if(p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPTH      = 0d;
            short       I           = 0;
            int KU = 3;
            int ISFLAG = 1;
            int IERNUM = 0;

            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double      Pressure    = (double)  p[0];
            double      Temperature = (double)  p[1];
            double      EngUnits    = (double)  p[2];
            

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[1] = Temperature;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("StmPTH Exception # " + IERNUM.ToString());
                else
                    stmPTH = F[2];
            }
            

            // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
            

            return stmPTH;
        }
        static object StmPTV(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPTV = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 1;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Temperature = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[1] = Temperature;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("StmPTV Exception # " + IERNUM.ToString());
                else
                    stmPTV = F[4];
            }

            // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.


            return stmPTV;
        }
        static object StmPSH(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPSH      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 5;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero = 32.018;

            double      Pressure = (double)p[0];
            double      Entropy = (double)p[1];
            double      EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[6] = Entropy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }


            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPSH Exception # " + IERNUM.ToString());
                else
                    stmPSH = F[2];
            }
            

            

            return stmPSH;
        }
        static object StmPHS(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPHS      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 2;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double      Pressure = (double)p[0];
            double      Enthalpy = (double)p[1];
            double      EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPHS Exception # " + IERNUM.ToString());
                else
                    stmPHS = F[6];
            }

            return stmPHS;
        }
        static object StmPHT(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPHT      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 2;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Pressure = (double)p[0];
            double Enthalpy = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPHT Exception # " + IERNUM.ToString());
                else
                    stmPHT = F[1];
            }
            
            return stmPHT;
        }
        static object StmPHX(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPHX      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 2;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Pressure = (double)p[0];
            double Enthalpy = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPHX Exception # " + IERNUM.ToString());
                else
                    stmPHX = F[3];
            }
            

            

            return stmPHX;
        }
        static object StmPTS(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPTS      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 1;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Pressure = (double)p[0];
            double Temperature = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[1] = Temperature;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPTS Exception # " + IERNUM.ToString());
                else
                    stmPTS = F[6];
            }

            return stmPTS;
        }
        static object StmPXH(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPXH      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 3;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Pressure = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPXH Exception # " + IERNUM.ToString());
                else
                    stmPXH = F[2];
            }

            return stmPXH;
        }
        static object StmPXT(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmPXT      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 3;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Pressure = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmPXT Exception # " + IERNUM.ToString());
                else
                    stmPXT = F[1];
            }


            return stmPXT;
        }
        static object StmTXP(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double      stmTXP      = 0d;
            short       I           = 0;
            int         KU          = 3;
            int         ISFLAG      = 6;
            int         IERNUM      = 0;
            double[]    F           = new double[9];
            double      TZero       = 32.018;

            double Temperature = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[1] = Temperature;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                EXPROP(ref KU, ref ISFLAG, F, ref TZero, ref IERNUM);
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.
                if (IERNUM != 0)
                    throw new Exception("StmTXP Exception # " + IERNUM.ToString());
                else
                    stmTXP = F[0];
            }

            return stmTXP;
        }

        static object fStmPTH(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPTH = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 1;
            int IERNUM = 0;

            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Temperature = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[1] = Temperature;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1         = KU.ToString().PadLeft(5);
                prop.L2         = 1.ToString().PadLeft(5); //ISFLAG.ToString();
                prop.L3         = IERNUM.ToString().PadLeft(5);
                prop.L4         = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5         = Temperature.ToString("0.0000000000E+00").PadLeft(17);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(50);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(50);


                bool pExit = false;                
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                
                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPTH Exception # " + prop.L3.ToString());
                    else
                        stmPTH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }              
            }


            // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.


            return stmPTH;
        }
//        static object fStmPTV(List<Expression> p)
//        {
//            // Should never be hit
//            if (p.Count != 3)
//                throw new Exception("Wrong number of arguments");

//            double stmPTV = 0d;
//            short I = 0;
//            int KU = 3;
//            int ISFLAG = 1;
//            int IERNUM = 0;
//            double[] F = new double[9];
//            double TZero = 32.018;

//            double Pressure = (double)p[0];
//            double Temperature = (double)p[1];
//            double EngUnits = (double)p[2];


//            for (int fi = 0; fi < F.Length; fi++)
//                F[fi] = 0d;

//            F[0] = Pressure;
//            F[1] = Temperature;

//            if (EngUnits == 2d ||
//                EngUnits == 3d)
//                KU = 3;

//            if (EngUnits == 4d ||
//                EngUnits == 5d)
//            {
//                KU = 5;
//                TZero = 0.01;
//            }

//            try
//            {
//#if _DEBUG_
//                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
//#endif
//                #region Call EXPROP.EXE

//                if (!File.Exists(pathToExe))
//                    throw new FileNotFoundException("File not found", pathToExe);

//                if (!File.Exists(pathToFile))
//                    throw new FileNotFoundException("File not found", pathToFile);

//                ESIpropFile prop = new ESIpropFile();

//                prop.KU = KU.ToString();
//                prop.ISFLAG = ISFLAG.ToString();
//                prop.IERNUM = IERNUM.ToString();
//                prop.PRESSURE = Pressure.ToString();
//                prop.FLUIDTEMP = 0.ToString();

//                prop.WriteFile(prop.Filename);

//                while (isFileLocked(pathToFile))
//                    System.Threading.Thread.Sleep(10);

//                CommandPrompt cmd = new CommandPrompt(pathToFile, true,
//                                                      string.Empty,
//                                                      Application.StartupPath);
//                cmd.Run();

//                while (cmd.IsRunning)
//                    System.Threading.Thread.Sleep(50);

//                while (isFileLocked(pathToFile))
//                    System.Threading.Thread.Sleep(10);


//                #endregion

//#if _DEBUG_
//                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
//#endif
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//            finally
//            {

//                try
//                {
//                    ESIpropFile prop = new ESIpropFile();

//                    if (int.Parse(prop.IERNUM) != 0)
//                        throw new Exception("StmPTH Exception # " + IERNUM.ToString());
//                    else
//                        stmPTV = double.Parse(prop.SPHUMIDITY); //F[2];
//                }
//                catch (Exception rd)
//                {
//                    throw rd;
//                }
//            }

//            // Error Number is always a minimum of 3 digits with the first digits as the routine number, the last two digits are the error number.


//            return stmPTV;
//        }
        static object fStmPSH(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPSH = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 5;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Entropy = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[6] = Entropy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }


            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5); 
                prop.L2 = 5.ToString().PadLeft(5); 
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L10 = Entropy.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPSH Exception # " + prop.L3.ToString());
                    else
                        stmPSH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }




            return stmPSH;
        }
        
        static object fStmPHS(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPHS = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 2;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Enthalpy = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = 2.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L9 = Enthalpy.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPHS Exception # " + prop.L3.ToString());
                    else
                        stmPHS = double.Parse(prop.L10); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return stmPHS;
        }        
        static object fStmPHT(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPHT = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 2;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Enthalpy = (double)p[1];
            double EngUnits = (double)p[2];


            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = 2.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L9 = Enthalpy.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPHT Exception # " + prop.L3.ToString());
                    else
                        stmPHT = double.Parse(prop.L5); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return stmPHT;
        }
        static object fStmPHX(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPHX = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 2;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Enthalpy = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[2] = Enthalpy;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = ISFLAG.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L9 = Enthalpy.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPHX Exception # " + prop.L3.ToString());
                    else
                        stmPHX = double.Parse(prop.L8); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }



            return stmPHX;
        }
        
        static object fStmPTS(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPTS = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 1;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Temperature = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[1] = Temperature;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = ISFLAG.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = Temperature.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPTS Exception # " + prop.L3.ToString());
                    else
                        stmPTS = double.Parse(prop.L10); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return stmPTS;
        }
        static object fStmPXH(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPXH = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 3;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = ISFLAG.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = Quality.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPXH Exception # " + prop.L3.ToString());
                    else
                        stmPXH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return stmPXH;
        }
        static object fStmPXT(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmPXT = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 3;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Pressure = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[0] = Pressure;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = ISFLAG.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = Quality.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmPXT Exception # " + prop.L3.ToString());
                    else
                        stmPXT = double.Parse(prop.L5); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }


            return stmPXT;
        }
        static object fStmTXP(List<Expression> p)
        {
            // Should never be hit
            if (p.Count != 3)
                throw new Exception("Wrong number of arguments");

            double stmTXP = 0d;
            short I = 0;
            int KU = 3;
            int ISFLAG = 6;
            int IERNUM = 0;
            double[] F = new double[9];
            double TZero = 32.018;

            double Temperature = (double)p[0];
            double Quality = (double)p[1];
            double EngUnits = (double)p[2];

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0d;

            F[1] = Temperature;
            F[3] = Quality;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KU = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KU = 5;
                TZero = 0.01;
            }

            try
            {
#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg(">Calling EXPROP");
#endif
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KU.ToString().PadLeft(5);
                prop.L2 = ISFLAG.ToString().PadLeft(5);
                prop.L3 = IERNUM.ToString().PadLeft(5);
                prop.L5 = Temperature.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = Quality.ToString("0.0000000000E+00").PadLeft(17);

                prop.WriteFile(prop.Filename);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);

                bool pExit = false;
                ProcessStartInfo psi = new ProcessStartInfo(pathToExe, string.Empty);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                Process cmd = new Process() { StartInfo = psi };
                //cmd.Exited += delegate { pExit = true; };
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();

                //while(pExit)
                //    System.Threading.Thread.Sleep(50);

                while (isFileLocked(pathToFile))
                    System.Threading.Thread.Sleep(10);


                #endregion

#if _DEBUG_
                //EsiApplication.Instance.LOG_ESI_Msg("<Returning from EXPROP");
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

                try
                {
                    ESIpropFile prop = new ESIpropFile();

                    if (int.Parse(prop.L3) != 0)
                        throw new Exception("StmTXP Exception # " + prop.L3.ToString());
                    else
                        stmTXP = double.Parse(prop.L4); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return stmTXP;
        }

        private static bool isFileLocked(string file)
        {
            if (File.Exists(file))
            {
                FileStream stream = null;

                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException ex)
                {
                    return true;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

    }
}
