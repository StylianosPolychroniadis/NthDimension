using System;
using System.Collections.Generic;
using System.Linq;
using NthDimension.CalcEngine.Expressions;

namespace NthDimension.CalcEngine.Functions
{
    static class EsiOpcFunctions
    {
        public static void Register(CalculationEngine ce)
        {
            ce.RegisterFunction("GetTagDefault",    1, GetTagDefault);
            ce.RegisterFunction("GetTagCurrent",    1, GetTag);
            ce.RegisterFunction("tag",        1, GetTagAvg);
            ce.RegisterFunction("avg",      2, int.MaxValue, SignalAvg);
            ce.RegisterFunction("GetAvgInterval",   0, GetAvgInterval);
        }

        static object GetTag(List<Expression> p)
        {
            if (p.Count < 1)
                throw new Exception("Wrong number of argumennts");

            double      getTagValue     = 0d;
            string      tagName         = p[0];

            if(p.Count == 1)
            {
                

                getTagValue = 
               double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagValue.ToString());
            }

            return getTagValue;
        }

        private static object GetTagAvg(List<Expression> p)
        {
            if (p.Count != 1)
                throw new Exception("Wrong number of argumennts");

            double getTagValue = 0d;
            string tagName = p[0];

            tagName = tagName.ToLower();


            switch (EsiApplication.Instance.RunningAverageNextRun)
            {
                case enuRunningAverageIntervals.Min1:
                    getTagValue =
                        double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagAVG_1_Min);
                    break;
                case enuRunningAverageIntervals.Min5:
                    getTagValue =
                        double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagAVG_5_Min);
                    break;
                case enuRunningAverageIntervals.Min15:
                    getTagValue =
                        double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagAVG_15_Min);
                    break;
                case enuRunningAverageIntervals.Min30:
                    getTagValue =
                        double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagAVG_30_Min);
                    break;
                case enuRunningAverageIntervals.Min60:
                    getTagValue =
                        double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].TagAVG_60_Min);
                    break;
            }



            return getTagValue;
        }

        static object GetTagDefault(List<Expression> p)
        {
            if (p.Count < 1)
                throw new Exception("Wrong number of argumennts");

            double getTagValue = 0d;
            string tagName = p[0].ToString().ToLower();

            if (p.Count == 1)
            {


                getTagValue =
               double.Parse(EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagName].DefaultValue.ToString());
            }

            return getTagValue;
        }
        static object GetAvgInterval(List<Expression> p)
        {
            int ret = 0;

            switch (EsiApplication.Instance.RunningAverageNextRun)
            {
                case enuRunningAverageIntervals.Default:
                    break;
                    case enuRunningAverageIntervals.Min1:
                    ret = 1;
                    break;
                    case enuRunningAverageIntervals.Min5:
                    ret = 5;
                    break;
                    case enuRunningAverageIntervals.Min15:
                    ret = 15;
                    break;
                    case enuRunningAverageIntervals.Min30:
                    ret = 30;
                    break;
                    case enuRunningAverageIntervals.Min60:
                    ret = 60;
                    break;
            }

            return ret;
        }

        static object SignalAvg(List<Expression> p)
        {
            if (p.Count < 2)
                throw new Exception("Wrong number of argumennts");

            double res = 0d;

            List<ValidableTag> signals = new List<ValidableTag>();
            List<ValidableTag> invalid = new List<ValidableTag>();

            for (int i = 0; i < p.Count; i++)
                signals.Add(new ValidableTag(p[i]));

            foreach (ValidableTag vt in signals)
            {
                if (!vt.IsValid)
                    invalid.Add(vt);
            }

            List<ValidableTag> valid = signals.Except(invalid).ToList();

            foreach (ValidableTag rv in valid)
            {
                res += rv.Value;
            }

            return res / valid.Count;
        }

        class ValidableTag
        {
            public ValidableTag(string tagname)
            {
                SYSCON.OPC.TagData td = EsiApplication.Instance.OpcPanel.OpcClient.InputTable[tagname];

                if (!td.Integrity)
                    throw new Exception("Integrity for tag " + tagname + " is disabled");

                double valueToCheck = 0d;

                switch (EsiApplication.Instance.RunningAverageNextRun)
                {
                    case enuRunningAverageIntervals.Default:
                        valueToCheck = td.DefaultValue;
                        break;
                    case enuRunningAverageIntervals.Min1:
                        valueToCheck = double.Parse(td.TagAVG_1_Min);
                        break;
                    case enuRunningAverageIntervals.Min5:
                        valueToCheck = double.Parse(td.TagAVG_5_Min);
                        break;
                    case enuRunningAverageIntervals.Min15:
                        valueToCheck = double.Parse(td.TagAVG_15_Min);
                        break;
                    case enuRunningAverageIntervals.Min30:
                        valueToCheck = double.Parse(td.TagAVG_30_Min);
                        break;
                    case enuRunningAverageIntervals.Min60:
                        valueToCheck = double.Parse(td.TagAVG_60_Min);
                        break;
                }

                if (valueToCheck >= td.IntegrityMin && valueToCheck <= td.IntegrityMax)
                {
                    IsValid = true;
                    Value = valueToCheck;
                }
                else
                {
                    IsValid = false;
                    Value = valueToCheck;
                }
            }

            public bool IsValid { get; set; }
            public double Value { get; set; }
        }
    }
}
