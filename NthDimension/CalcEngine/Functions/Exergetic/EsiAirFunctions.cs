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
    static class EsiAirFunctions
    {
        #region Dll P/Invoke
        [DllImport("C:\\ESI\\LIB\\ESI_AIR.DLL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern void ESISVER(ref double Rx);

        [DllImport("C:\\ESI\\LIB\\ESI_AIR.DLL",
        BestFitMapping = false,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "EXAIR",
        ExactSpelling = true,
        PreserveSig = true,
        SetLastError = false,
        ThrowOnUnmappableChar = true)]
        public static extern void EXAIR(ref int KUSAVx, ref int NFLAGx, ref int Mx, double[] Fx, ref double TZEROx, ref int IERNUMx);
        #endregion

        private static string pathToExe = Path.Combine(Path.Combine(Properties.Settings.Default.rootDisk, @"MAPS\EX-FOSS\HEATRATE\" + Properties.Settings.Default.plantName), "ESIprop.exe");
        private static string pathToFile = Path.Combine(Path.Combine(Properties.Settings.Default.rootDisk, @"MAPS\EX-FOSS\HEATRATE\" + Properties.Settings.Default.plantName), "ESIprop.DAT");

        public static void Register(CalculationEngine ce)
        {
            //ce.RegisterFunction("AirDBWBHH", 4, AirDBWBHH);
            //ce.RegisterFunction("AirDBRHHH", 4, AirDBRHHH);
            //ce.RegisterFunction("AirDBSHHH", 4, AirDBSHHH);
            //ce.RegisterFunction("AirHHSHRH", 4, AirHHSHRH);
            //ce.RegisterFunction("AirDBDPHH", 4, AirDBDPHH);
            //ce.RegisterFunction("AirDBWBRH", 4, AirDBWBRH);
            //ce.RegisterFunction("AirDBDPRH", 4, AirDBDPRH);
            //ce.RegisterFunction("AirDBWBSH", 4, AirDBWBSH);
            //ce.RegisterFunction("AirDBRHSH", 4, AirDBRHSH);
            //ce.RegisterFunction("AirDBDPSH", 4, AirDBDPSH);

            ce.RegisterFunction("AirDBWBHH", 4, fAirDBWBHH);
            ce.RegisterFunction("AirDBRHHH", 4, fAirDBRHHH);
            ce.RegisterFunction("AirDBSHHH", 4, fAirDBSHHH);
            ce.RegisterFunction("AirHHSHRH", 4, fAirHHSHRH);
            ce.RegisterFunction("AirDBDPHH", 4, fAirDBDPHH);
            ce.RegisterFunction("AirDBWBRH", 4, fAirDBWBRH);
            ce.RegisterFunction("AirDBDPRH", 4, fAirDBDPRH);
            ce.RegisterFunction("AirDBWBSH", 4, fAirDBWBSH);
            ce.RegisterFunction("AirDBRHSH", 4, fAirDBRHSH);
            ce.RegisterFunction("AirDBDPSH", 4, fAirDBDPSH);
        }

        static object AirDBWBHH(List<Expression> p)
        {
            if(p.Count!= 4)
                throw new Exception("Wrong number of arguments");

            double      airDBWBHH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 1;
            int         OutPtr          = 5;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double      Pressure        = p[0];
            double      DryBulb         = p[1];
            double      WetBulb         = p[2];
            double      EngUnits        = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBWBHH exception # " + IERNUM);
                else
                    airDBWBHH = F[OutPtr];
            }

            return airDBWBHH;
        }
        static object AirDBRHHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBRHHH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 2;
            int         OutPtr          = 5;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double RelHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[2] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBRHHH exception # " + IERNUM);
                else
                    airDBRHHH = F[OutPtr];
            }

            return airDBRHHH;
        }
        static object AirDBSHHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBSHHH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 3;
            int         OutPtr          = 5;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double SpcHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[3] = SpcHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBSHHH exception # " + IERNUM);
                else
                    airDBSHHH = F[OutPtr];
            }

            return airDBSHHH;
        }
        static object AirHHSHRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airHHSHRH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 4;
            int         OutPtr          = 2;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double SpcHum   = p[1];
            double RelHum   = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[5] = SpcHum;
            F[3] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirHHSHRH exception # " + IERNUM);
                else
                    airHHSHRH = F[OutPtr];
            }

            return airHHSHRH;
        }
        static object AirDBDPHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBDPHH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 5;
            int         OutPtr          = 5;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure     = p[0];
            double DryBulb      = p[1];
            double DewPt        = p[2];
            double EngUnits     = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBDPHH exception # " + IERNUM);
                else
                    airDBDPHH = F[OutPtr];
            }

            return airDBDPHH;
        }
        static object AirDBWBRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBWBRH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 1;
            int         OutPtr          = 2;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double WetBulb = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBWBRH exception # " + IERNUM);
                else
                    airDBWBRH = F[OutPtr];
            }

            return airDBWBRH;
        }
        static object AirDBDPRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBDPRH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 5;
            int         OutPtr          = 2;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb  = p[1];
            double DewPt    = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBDPRH exception # " + IERNUM);
                else
                    airDBDPRH = F[OutPtr];
            }

            return airDBDPRH;
        }
        static object AirDBWBSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBWBSH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 1;
            int         OutPtr          = 3;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb  = p[1];
            double WetBulb  = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBWBSH exception # " + IERNUM);
                else
                    airDBWBSH = F[OutPtr];
            }

            return airDBWBSH;
        }
        static object AirDBRHSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBRHSH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 2;
            int         OutPtr          = 3;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double RelHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[2] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBRHSH exception # " + IERNUM);
                else
                    airDBRHSH = F[OutPtr];
            }

            return airDBRHSH;
        }
        static object AirDBDPSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double      airDBDPSH       = 0d;
            int         KUSAV           = 3;
            int         M               = 3;
            double[]    F               = new double[13];
            double      TZero           = 32.018;
            int         NFLAG           = 5;
            int         OutPtr          = 3;
            int         IERNUM          = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure     = p[0];
            double DryBulb      = p[1];
            double DewPt        = p[2];
            double EngUnits     = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                EXAIR(ref KUSAV, ref NFLAG, ref M, F, ref TZero, ref IERNUM);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (IERNUM != 0)
                    throw new Exception("AirDBDPSH exception # " + IERNUM);
                else
                    airDBDPSH = F[OutPtr];
            }

            return airDBDPSH;
        }

        static object fAirDBWBHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBWBHH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 1;
            int OutPtr = 5;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double WetBulb = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L6 = WetBulb.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBWBHH Exception # " + IERNUM.ToString());
                    else
                        airDBWBHH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airDBWBHH;
        }
        static object fAirDBRHHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBRHHH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 2;
            int OutPtr = 5;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double RelHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[2] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = RelHum.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBRHHH Exception # " + IERNUM.ToString());
                    else
                        airDBRHHH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airDBRHHH;
        }
        static object fAirDBSHHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBSHHH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 3;
            int OutPtr = 5;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double SpcHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[3] = SpcHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L11 = SpcHum.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirHHSHRH Exception # " + IERNUM.ToString());
                    else
                        airDBSHHH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airDBSHHH;
        }
        static object fAirHHSHRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airHHSHRH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 4;
            int OutPtr = 2;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double SpcHum = p[1];
            double RelHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[5] = SpcHum;
            F[3] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L11 = SpcHum.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = RelHum.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirHHSHRH Exception # " + IERNUM.ToString());
                    else
                        airHHSHRH = double.Parse(prop.L8); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airHHSHRH;
        }
        static object fAirDBDPHH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBDPHH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 5;
            int OutPtr = 5;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double DewPt = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L7 = DewPt.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBDPHH Exception # " + IERNUM.ToString());
                    else
                        airDBDPHH = double.Parse(prop.L9); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airDBDPHH;
        }
        static object fAirDBWBRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBWBRH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 1;
            int OutPtr = 2;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double WetBulb = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L6 = WetBulb.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBWBRH Exception # " + IERNUM.ToString());
                    else
                        airDBWBRH = double.Parse(prop.L8); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }

            return airDBWBRH;
        }
        static object fAirDBDPRH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBDPRH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 5;
            int OutPtr = 2;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double DewPt = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L7 = DewPt.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBDPRH Exception # " + IERNUM.ToString());
                    else
                        airDBDPRH = double.Parse(prop.L8); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }


            return airDBDPRH;
        }
        static object fAirDBWBSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBWBSH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 1;
            int OutPtr = 3;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double WetBulb = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[1] = WetBulb;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L6 = WetBulb.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBWBSH Exception # " + IERNUM.ToString());
                    else
                        airDBWBSH = double.Parse(prop.L11); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }


            return airDBWBSH;
        }
        static object fAirDBRHSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBRHSH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 2;
            int OutPtr = 3;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double RelHum = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[2] = RelHum;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L8 = RelHum.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBRHSH Exception # " + IERNUM.ToString());
                    else
                        airDBRHSH = double.Parse(prop.L11); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }


            return airDBRHSH;
        }
        static object fAirDBDPSH(List<Expression> p)
        {
            if (p.Count != 4)
                throw new Exception("Wrong number of arguments");

            double airDBDPSH = 0d;
            int KUSAV = 3;
            int M = 3;
            double[] F = new double[13];
            double TZero = 32.018;
            int NFLAG = 5;
            int OutPtr = 3;
            int IERNUM = 0;

            for (int fi = 0; fi < F.Length; fi++)
                F[fi] = 0;

            double Pressure = p[0];
            double DryBulb = p[1];
            double DewPt = p[2];
            double EngUnits = p[3];

            F[4] = Pressure;
            F[0] = DryBulb;
            F[12] = DewPt;

            if (EngUnits == 2d ||
                EngUnits == 3d)
                KUSAV = 3;

            if (EngUnits == 4d ||
                EngUnits == 5d)
            {
                KUSAV = 5;
                TZero = 0.01;
            }

            try
            {
                #region Call EXPROP.EXE

                if (!File.Exists(pathToExe))
                    throw new FileNotFoundException("File not found", pathToExe);

                if (!File.Exists(pathToFile))
                    throw new FileNotFoundException("File not found", pathToFile);

                ESIpropFile prop = new ESIpropFile();

                prop.L1 = KUSAV.ToString();
                prop.L2 = NFLAG.ToString();
                prop.L3 = IERNUM.ToString();
                prop.L4 = Pressure.ToString("0.0000000000E+00").PadLeft(17);
                prop.L5 = DryBulb.ToString("0.0000000000E+00").PadLeft(17);
                prop.L7 = DewPt.ToString("0.0000000000E+00").PadLeft(17);

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
                        throw new Exception("AirDBDPSH Exception # " + IERNUM.ToString());
                    else
                        airDBDPSH = double.Parse(prop.L11); //F[2];
                }
                catch (Exception rd)
                {
                    throw rd;
                }
            }


            return airDBDPSH;
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
