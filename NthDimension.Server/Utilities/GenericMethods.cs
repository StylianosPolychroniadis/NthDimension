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

using NthDimension.Algebra;
using System;
using System.Globalization;

namespace NthDimension.Server.Utilities
{
    public static class GenericMethods
    {
        private static string splitChar = "|";

        public static NumberFormatInfo getNfi()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalSeparator = ".";
            return nfi;
        }



        #region Algebra Extensions

        public static Vector3 VectorFromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            return new Vector3(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi));
        }

        public static Matrix4 Matrix4FromString(string mString)
        {
            NumberFormatInfo nfi = getNfi();
            string[] fields = mString.Split(splitChar.ToCharArray());
            Matrix4 mMat = new Matrix4(
                Single.Parse(fields[0], nfi),
                Single.Parse(fields[1], nfi),
                Single.Parse(fields[2], nfi),
                Single.Parse(fields[3], nfi),
                Single.Parse(fields[4], nfi),
                Single.Parse(fields[5], nfi),
                Single.Parse(fields[6], nfi),
                Single.Parse(fields[7], nfi),
                Single.Parse(fields[8], nfi),
                Single.Parse(fields[9], nfi),
                Single.Parse(fields[10], nfi),
                Single.Parse(fields[11], nfi),
                Single.Parse(fields[12], nfi),
                Single.Parse(fields[13], nfi),
                Single.Parse(fields[14], nfi),
                Single.Parse(fields[15], nfi)
                );

            return mMat;
        }

        #endregion
    }
}
