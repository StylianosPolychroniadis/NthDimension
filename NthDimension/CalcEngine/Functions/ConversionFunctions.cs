using System;
using System.Collections.Generic;

using NthDimension.CalcEngine.Expressions;


namespace NthDimension.CalcEngine.Functions
{
    static class ConversionFunctions
    {


        public static void Register(CalculationEngine ce)
        {
            ce.RegisterFunction("ConvPsig_Psia",    2, ConvPsig_Psia);
            ce.RegisterFunction("ConvInHga_Psia",   1, ConvInHga_Psia);
            ce.RegisterFunction("ConvKlbmhr_Lbmhr" ,1 , ConvKlbmhr_Lbmhr);
            ce.RegisterFunction("ConvGpm_Lbmhr" ,   1 , ConvGpm_Lbmhr);
            ce.RegisterFunction("ConvPpm_Fraction", 1, ConvPpm_Fraction);
            ce.RegisterFunction("MassFlowInlet", 1, NodeFlowIn);
            ce.RegisterFunction("MassFlowOutlet", 1, NodeFlowOut);
        }


        static object ConvPsig_Psia(List<Expression> p)
        {
            if(p.Count != 2)
                throw new Exception("Wrong number of arguments");

            double res = 0d;

            try
            {
                res = ((double)p[0] + (double)p[1]);
            }
            catch (Exception e)
            {
                
                throw e;
            }

            return res;
        }
        static object ConvInHga_Psia(List<Expression> p)
        {
            if (p.Count != 1)
                throw new Exception("Wrong number of arguments");

            double res = 0d;

            try
            {
                res = (double) p[0]*14.6959d/29.92d;
            }
            catch (Exception e)
            {

                throw e;
            }


            return res;
        }
        static object ConvKlbmhr_Lbmhr(List<Expression> p)
        {
            if (p.Count != 1)
                throw new Exception("Wrong number of arguments");

            double res = 0d;

            try
            {
                res = p[0]*1000;
            }
            catch (Exception e)
            {

                throw e;
            }

            return res;
        }       
        static object ConvGpm_Lbmhr(List<Expression> p)
        {
            if (p.Count != 1)
                throw new Exception("Wrong number of arguments");

            double res = 0d;

            try
            {
                res = (double)p[0] * 501.7d;
            }
            catch (Exception e)
            {

                throw e;
            }

            return res;
        }
        static object ConvPpm_Fraction(List<Expression> p)
        {
            if (p.Count != 1)
                throw new Exception("Wrong number of arguments");

            double res = 0d;

            try
            {
                res = (double) p[0]*Math.Pow(10, -6);
            }
            catch (Exception e)
            {

                throw e;
            }

            return res;
        }

        static object NodeFlowIn(List<Expression> p)
        {
            double massFlowIn = 0d;

            int nodeIndex = (int)p[0];

            throw new NotImplementedException("From BluePanel.net");
            //for (int i = 0; i < EsiApplication.Instance.NodeGraph.View.NodeCollection.Count; i++ )
            //{
            //    if (EsiApplication.Instance.NodeGraph.View.NodeCollection[i] is IHeatExchangerNode &&
            //        ((IHeatExchangerNode)EsiApplication.Instance.NodeGraph.View.NodeCollection[i]).HxsId == nodeIndex)
            //    {
            //        #region Fluid
            //        if (EsiApplication.Instance.NodeGraph.View.NodeCollection[i] is IHeatExchangerFluidComponent)
            //        {
            //            if (((IHeatExchangerNode) EsiApplication.Instance.NodeGraph.View.NodeCollection[i]).HxsId ==
            //                nodeIndex)
            //                massFlowIn =
            //                    ((IHeatExchangerFluidComponent) EsiApplication.Instance.NodeGraph.View.NodeCollection[i])
            //                        .MassFlowIn;
            //        }
            //        #endregion

                    
            //    }
            //}
            return massFlowIn;
        }
        static object NodeFlowOut(List<Expression> p)
        {
            double massFlowIn = 0d;

            int nodeIndex = (int)p[0];

            throw new NotImplementedException("From BluePanel.net");

            //for (int i = 0; i < EsiApplication.Instance.NodeGraph.View.NodeCollection.Count; i++)
            //{
            //    if (EsiApplication.Instance.NodeGraph.View.NodeCollection[i] is IHeatExchangerNode &&
            //        ((IHeatExchangerNode)EsiApplication.Instance.NodeGraph.View.NodeCollection[i]).HxsId == nodeIndex)
            //    {
            //        #region Fluid
            //        if (EsiApplication.Instance.NodeGraph.View.NodeCollection[i] is IHeatExchangerFluidComponent)
            //        {
            //            if (((IHeatExchangerNode)EsiApplication.Instance.NodeGraph.View.NodeCollection[i]).HxsId ==
            //                nodeIndex)
            //                massFlowIn =
            //                    ((IHeatExchangerFluidComponent)EsiApplication.Instance.NodeGraph.View.NodeCollection[i])
            //                        .MassFlowOut;
            //        }
            //        #endregion


            //    }
            //}
            return massFlowIn;
        }

    
    }
}
