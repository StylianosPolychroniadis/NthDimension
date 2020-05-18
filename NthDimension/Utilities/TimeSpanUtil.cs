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

namespace NthDimension.Utilities
{
    using System;

    public static class TimeSpanUtil
    {
        #region Pretifier (ie Just Now, 5 Min ago, etc)
        private static string GetSecondsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One second";
            return string.Format("{0} seconds", v);
        }
        private static string GetMinutesText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One minute";
            return string.Format("{0} minutes", v);
        }
        private static string GetHoursText(int v)
        {
            string ret = string.Empty;

            if (v <= 0) ret = string.Empty;
            if (v == 1) ret = "One hour";
            if (v > 1) ret = "hours";

            ret = string.Format("{0} {1}", v, ret);

            return ret;
        }
        private static string GetDaysText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One day";
            return string.Format("{0} days", v);
        }
        private static string GetWeeksText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One week";
            return string.Format("{0} weeks", v);
        }
        private static string GetMonthsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One month";
            return string.Format("{0} months", v);
        }
        private static string GetYearsText(int v)
        {
            if (v <= 0) return null;
            if (v == 1) return "One year";
            return string.Format("{0} years", v);
        }

        private static readonly Func<int, string>[] TextFuncArray = new Func<int, string>[] { GetYearsText, GetMonthsText, GetWeeksText, GetDaysText, GetHoursText, GetMinutesText, GetSecondsText };
        private static readonly long[] SignificanceArray = new long[] {
                3600 * 24 * 365, // year
                3600 * 24 * 30, //month
                3600 * 24 * 7, // week
                3600 * 24, // day
                3600, //hour
                60, //min
                1 // sec
            };

        /// <summary>
        /// Transform a TimeSpan into user readable Text
        /// </summary>
        /// <param name="ts">the TimeSpan object</param>
        /// <param name="partCount">how many interval parts you want? (e.g.  2years and 5 days  is 2 parts)</param>
        /// <param name="neglectionFactor">determine the significance level of a part.</param>
        /// <returns>beautiful string</returns>
        public static string Prettify(this TimeSpan ts, int partCount = 2, double neglectionFactor = 0.2)
        {
            int[] tsPartsArray = new int[7];

            int d = (int)ts.TotalDays;
            tsPartsArray[0] = d / 365;
            d %= 365;
            tsPartsArray[1] = (int)(d / 30.41);
            d %= 30;
            tsPartsArray[2] = (int)(d / 7.5);
            d %= 7;
            tsPartsArray[3] = d;

            tsPartsArray[4] = ts.Hours;
            tsPartsArray[5] = ts.Minutes;
            tsPartsArray[6] = ts.Seconds;

            long totalSignificance = 1;
            int partIdx = 0;
            string[] parts = new string[partCount];
            for (int i = 0; i < tsPartsArray.Length; i++)
            {
                if (tsPartsArray[i] > 0)
                {
                    long significance = SignificanceArray[i] * tsPartsArray[i];

                    if ((significance / (double)totalSignificance) < neglectionFactor)
                    {
                        break;
                    }

                    totalSignificance += significance;

                    parts[partIdx] = TextFuncArray[i](tsPartsArray[i]);
                    if (++partIdx >= parts.Length) // no new parts are needed
                        break;

                }
            }

            if (partIdx == 0)
                return "Just Now";
            if (partIdx == 1)
                return string.Format("{0} ago", parts[0]);

            return string.Format("{0} ago", string.Join(" ", parts, 0, partIdx - 1) + " " + parts[partIdx - 1]);
        }

        #endregion

    }
}
