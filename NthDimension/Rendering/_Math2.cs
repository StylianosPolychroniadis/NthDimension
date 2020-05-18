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

namespace NthDimension.Rendering
{
    using System;
    // TODO:: Refactor (Assimilate in normal Extensions Interface)
    public class Math2
    {
        private static Random rand = new Random();

        public static double randomNumber(double min, double max)
        {
            return (max - min) * rand.NextDouble() + min;
        }

        public static bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static double degToRad(double degree)
        {
            return (double)((degree) * (Math.PI / 180.0d));
        }

        public static double radToDeg(double radian)
        {
            return (double)((radian) * (180.0d / Math.PI));
        }

        public static float normalize(float val, float minRange, float maxRange)
        {
            return ((val - minRange) / (maxRange - minRange));
        }
    }
}
