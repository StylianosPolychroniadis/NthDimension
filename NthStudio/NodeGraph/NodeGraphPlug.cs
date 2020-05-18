using NthDimension.Forms;
using NthDimension.Forms.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.NodeGraph
{
    public class NodePlugEventArgs : EventArgs
    {
        private NodeGraphPlug m_plug;

        public NodePlugEventArgs(NodeGraphPlug e)
        {
            m_plug = e;
        }

        public NodeGraphPlug NodeGraphPlug
        {
            get { return m_plug; }
        }
    }
    public class NodeGraphPlug : INodeGraphPlug
    {
        // Oct-28-15    Added OnPlugLinked event to acquire FromHxsId               S.Pol


        #region Events and Delegates

        public delegate void OnPlugLinked(NodePlugEventArgs e);

        public event OnPlugLinked PlugLinked;
        #endregion

        #region Members
        private string m_Name;
        private NodeGraphNode m_oParentNode;
        private NodeViewState m_oView;
        private enuOPERATORS m_oPlugType;
        private int m_iPlugIndex;
        private NodeGraphDataType m_oDataType;
        private LinkVisualStyle m_LinkVisualStyle;
        #endregion

        #region Ctor
        public NodeGraphPlug(string p_Name, NodeGraphNode p_parent, enuOPERATORS pPlugType, int pPlugIndex, LinkVisualStyle style)
        {
            this.m_Name = p_Name;
            this.m_oParentNode = p_parent;
            this.m_oView = p_parent.ParentView;
            this.m_oPlugType = pPlugType;
            this.m_iPlugIndex = pPlugIndex;
            this.m_LinkVisualStyle = style;
            this.m_oDataType = p_parent.ParentView.KnownDataTypes["Generic"];
        }
        public NodeGraphPlug(string p_Name, NodeGraphNode p_parent, enuOPERATORS pPlugType, int pPlugIndex, string p_NodeGraphDataTypeName, LinkVisualStyle style)
        {
            this.m_Name = p_Name;
            this.m_oParentNode = p_parent;
            this.m_oView = p_parent.ParentView;
            this.m_oPlugType = pPlugType;
            this.m_iPlugIndex = pPlugIndex;
            this.m_LinkVisualStyle = style;

            this.m_oDataType = p_parent.ParentView.KnownDataTypes[p_NodeGraphDataTypeName];
        }
        #endregion

        public bool CanProcess()
        {
            bool result;
            //if (this.m_oPlugType == ConnectorType.FluidOutletBottom)
            //{
            //    result = true;
            //}
            //else if (this.m_oPlugType == ConnectorType.ReadInputConnector)
            //{
            //    result = true;
            //}
            //else if (this.m_oPlugType == ConnectorType.WriteOutputConnector)
            //{
            //    result = true;
            //}
            //else
            {
                NodeGraphPlug link = this.m_oView.ParentPanel.GetLink(this);
                result = (link != null);
            }
            return result;
        }

        public Rectangle GetPlugArea()
        {
            Rectangle result = default(Rectangle);

            #region Top Fluid Plug
            //if (this.m_oPlugType == ConnectorType.FluidInletTop)
            //{
            //    //Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    //result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //    Point point =
            //        this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y));

            //    return new Rectangle(point.X,
            //        point.Y + (this.ParentNode.HeaderRectangle.Height * (int)this.m_oView.CurrentViewZoom),
            //        (int)(6f * this.m_oView.CurrentViewZoom),
            //        (int)(8f * this.m_oView.CurrentViewZoom));
            //}
            #endregion

            #region Bottom Fluid Plug
            //if (this.m_oPlugType == ConnectorType.FluidOutletBottom)
            //{
            //    ////Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    ////result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //    Point point =
            //        this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y + this.m_oParentNode.Height));

            //    return new Rectangle(point.X,
            //        ////point.Y - (int)(1f * this.m_oView.CurrentViewZoom),
            //        point.Y - (int)(2f * this.m_oView.CurrentViewZoom),
            //        (int)(6f * this.m_oView.CurrentViewZoom),
            //        (int)(8f * this.m_oView.CurrentViewZoom));
            //}
            #endregion


            #region Spray Inlet Bottom

            //if (this.m_oPlugType == ConnectorType.SprayInletBottom)
            //{
            //    int pX = (this.m_oParentNode.X + this.m_oParentNode.Width / 2);// -(int)(10 * this.m_oView.CurrentViewZoom);
            //    int pY = this.m_oParentNode.Y + this.m_oParentNode.Height;

            //    Point point =
            //        this.m_oView.ParentPanel.ViewToControl(new Point(pX, pY));

            //    return new Rectangle(point.X,
            //                         //point.Y - (int)(1f * this.m_oView.CurrentViewZoom),
            //                         point.Y - (int)(2f * this.m_oView.CurrentViewZoom),
            //                         (int)(6f * this.m_oView.CurrentViewZoom),
            //                         (int)(8f * this.m_oView.CurrentViewZoom));
            //}

            #endregion

            #region Normal Inlet
            if (this.m_oPlugType == enuOPERATORS.Inlet)
            {
                Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
                result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            }
            #endregion

            #region Normal Outlet
            if (this.m_oPlugType == enuOPERATORS.Outlet)
            {
                Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
                result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            }
            #endregion

            #region Bool //Gas/ Inlet
            //if (this.m_oPlugType == ConnectorType._GasInlet)
            if (this.m_oPlugType == enuOPERATORS.Inlet)
            {
                Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
                result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
                //Point point =
                //    this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y));

                return new Rectangle(point.X,
                    point.Y - 13,
                    6,
                    12);
            }
            #endregion

            #region Gas Outlet
            //if (this.m_oPlugType == ConnectorType._GasOutlet)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //    //Point point =
            //    //    this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y + this.m_oParentNode.Height));

            //    //return new Rectangle(point.X,
            //    //    point.Y + 1,
            //    //    6,
            //    //    12);
            //}
            #endregion

            #region Arithmetic Input & Output
            //if (this.m_oPlugType == ConnectorType.ReadInputConnector)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //}
            //if (this.m_oPlugType == ConnectorType.WriteOutputConnector)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //}
            #endregion

            ////#region Fluid Outlet
            ////if (this.m_oPlugType == ConnectorType._GasOutlet)
            ////{
            ////    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            ////    result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            ////}
            ////#endregion

            PlugRectangle = result;

            return result;
        }
        public Rectangle GetHitArea()
        {
            //Rectangle result = default(Rectangle);
            int connectorHitZoneBleed = this.m_oView.ParentPanel.ConnectorHitZoneBleed;

            // TODO Add Sprays


            #region Top Fluid Plug
            //if (this.m_oPlugType == ConnectorType.FluidInletTop)
            //{
            //    Point point =
            //         this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y));

            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed,
            //         point.Y + this.ParentNode.HeaderRectangle.Height - 2 * connectorHitZoneBleed,
            //         (int)(6f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed,
            //         (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion

            #region Bottom Fluid Plug
            //if (this.m_oPlugType == ConnectorType.FluidOutletBottom)
            //{

            //    //Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    //result = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //    Point point =
            //        this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.Width / 2, this.m_oParentNode.Y + this.m_oParentNode.Height));

            //    Rectangle rect = new Rectangle(point.X,
            //         //point.Y - (int)(1f * this.m_oView.CurrentViewZoom),
            //         point.Y - (int)(2f * this.m_oView.CurrentViewZoom),
            //         (int)(6f * this.m_oView.CurrentViewZoom),
            //         (int)(8f * this.m_oView.CurrentViewZoom));



            //    //result = new Rectangle(point.X - 2 * connectorHitZoneBleed,
            //    //    //point.Y + (int)(1f * this.m_oView.CurrentViewZoom) - connectorHitZoneBleed,
            //    //    //point.Y - (int)(2f * this.m_oView.CurrentViewZoom) - connectorHitZoneBleed,
            //    //    point.Y  - 2 * connectorHitZoneBleed,
            //    //    //(int)(6f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed,
            //    //    point.X +  2 * connectorHitZoneBleed,
            //    //    point.Y +  2 * connectorHitZoneBleed);



            //    return this.PlugHitRectangle = new Rectangle(rect.X - 2 * connectorHitZoneBleed,
            //                            rect.Y - rect.Height - 2 * connectorHitZoneBleed,
            //                            rect.Width + 2 * connectorHitZoneBleed,
            //                            rect.Y + 2 * rect.Height + 2 * connectorHitZoneBleed);


            //}
            #endregion

            #region Fluid Inlet
            //if (this.m_oPlugType == ConnectorType.FluidInlet)// || this.m_oPlugType == ConnectorType.FluidInlet)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion

            #region Fluit Outlet
            //if (this.m_oPlugType == ConnectorType.FluidOutlet || this.m_oPlugType == ConnectorType._GasOutlet)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion

            #region Air Inlet
            //if (this.m_oPlugType == ConnectorType.AirInlet)// || this.m_oPlugType == ConnectorType.FluidInlet)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion

            #region Air Outlet
            //if (this.m_oPlugType == ConnectorType.AirOutlet)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion

            #region Inlet
            if (this.m_oPlugType == enuOPERATORS.Inlet)// || this.m_oPlugType == ConnectorType.FluidInlet)
            {
                Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
                return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            }
            #endregion

            #region Outlet
            if (this.m_oPlugType == enuOPERATORS.Outlet)
            {
                Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
                return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            }
            #endregion

            #region Numeric Inlet
            //if (this.m_oPlugType == ConnectorType.ReadInputConnector)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X, point.Y, (int)(12f * this.m_oView.CurrentViewZoom), (int)(8f * this.m_oView.CurrentViewZoom));
            //}
            #endregion
            #region Numeric Outlet
            //if (this.m_oPlugType == ConnectorType.WriteOutputConnector)
            //{
            //    Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + (this.m_oParentNode.HitRectangle.Width - 12), this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 6 + this.m_iPlugIndex * 16));
            //    return this.PlugHitRectangle = new Rectangle(point.X - connectorHitZoneBleed, point.Y - connectorHitZoneBleed, (int)(12f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed, (int)(8f * this.m_oView.CurrentViewZoom) + 2 * connectorHitZoneBleed);
            //}
            #endregion



            return this.PlugHitRectangle;
        }
        public Point GetPlugTextPosition(PaintEventArgs e)
        {
            Point result;

            //if (this.m_oPlugType == ConnectorType.FluidInletTop)// || this.m_oPlugType == ConnectorType.FluidOutletBottom || this.m_oPlugType == ConnectorType.SprayInletBottom)
            //{
            //    Rectangle r = this.GetPlugArea();

            //    throw new System.NotImplementedException();
            //    {
            //        ////
            //        //SizeF sizeF = e.Graphics.MeasureString(this.Name, this.m_oView.ParentPanel.NodeScaledConnectorFont);
                   
            //    }
            //        SizeF sizeF_ToDELETE = new SizeF();
            //        e.GC.DrawRectangle(new NanoPen(Color.Red) /*{ DashStyle = DashStyle.DashDotDot }*/, r);

            //    return result = new Point(r.X - (int)sizeF_ToDELETE.Width / 2, r.Y + r.Height / 2 - (int)sizeF_ToDELETE.Height / 2);
            //}
            //else if (this.m_oPlugType == ConnectorType.FluidOutletBottom)// || this.m_oPlugType == ConnectorType.FluidOutletBottom || this.m_oPlugType == ConnectorType.SprayInletBottom)
            //{
            //    Rectangle r = this.GetPlugArea();
            //    throw new System.NotImplementedException();
            //    {
            //        //

            //        //SizeF sizeF = e.Graphics.MeasureString(this.Name, this.m_oView.ParentPanel.NodeScaledConnectorFont);
            //    }
            //    SizeF sizeF_ToDELETE = new SizeF();
            //    e.GC.DrawRectangle(new NanoPen(Color.Red) /*{ DashStyle = DashStyle.DashDotDot }*/, r);

            //    return result = new Point(r.X - (int)sizeF_ToDELETE.Width / 2, r.Y - r.Height / 2 - (int)sizeF_ToDELETE.Height / 2);
            //}

            //else 
                if (
                //this.m_oPlugType == ConnectorType._GasInlet ||
                //    this.m_oPlugType == ConnectorType.FluidInlet ||
                //    this.m_oPlugType == ConnectorType.SprayInlet
                this.m_oPlugType == enuOPERATORS.Inlet)     // ToDo:: Split
            {
                result = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + 16, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + this.m_iPlugIndex * 16));
            }
            else
            {
                //throw new System.NotImplementedException();
                //{
                //    //

                //    //SizeF sizeF = e.Graphics.MeasureString(this.Name, this.m_oView.ParentPanel.NodeScaledConnectorFont);
                //}
                //SizeF sizeF_ToDELETE = new SizeF();
                result = this.m_oView.ParentPanel.ViewToControl(new Point(this.m_oParentNode.X + this.m_oParentNode.HitRectangle.Width, this.m_oParentNode.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + this.m_iPlugIndex * 16));
                //result.X = result.X - (int)(16f * this.m_oView.CurrentViewZoom) - (int)sizeF_ToDELETE.Width;
            }
            return result;
        }
        public void RaisePlugLinked(NodeGraphPlug fromPlug)
        {
            if (null != this.PlugLinked)
                this.PlugLinked(new NodePlugEventArgs(fromPlug));
        }

        public virtual void Draw(PaintEventArgs e, int ConnectorIndex)
        {
            Rectangle area = this.GetPlugArea();

            Pen pen;
            SolidBrush brush;

            if (this.m_oView.ParentPanel.UseLinkColoring)
            {
                pen = this.m_oDataType.ConnectorOutlinePen;
                brush = this.m_oDataType.ConnectorFillBrush;
            }
            else
            {
                pen = this.m_oView.ParentPanel.ConnectorOutline;
                brush = this.m_oView.ParentPanel.ConnectorFill;
            }


            e.GC.FillRectangle(brush, area);
            e.GC.DrawRectangle(new NthDimension.Forms.NanoPen(pen.Color), area);

            if (this.m_oView.CurrentViewZoom > this.m_oView.ParentPanel.NodeConnectorTextZoomTreshold)
            {
                Point connectorTextPosition = this.GetPlugTextPosition(e);

                SolidBrush brsh = new SolidBrush(pen.Color);

                ////if (this.PlugType == ConnectorType.FluidInletTop ||
                ////    this.PlugType == ConnectorType.FluidOutletBottom)
                ////    //brsh = this.m_oView.ParentPanel.NodeText;


                //if (this.PlugType != ConnectorType.FluidInletTop &&
                //    this.PlugType != ConnectorType.FluidOutletBottom &&
                //    this.PlugType != ConnectorType.SprayInletBottom)
                //{
                //    throw new System.NotImplementedException();
                //    {
                //        //

                //        //e.GC.DrawString(this.Name,
                //        //                  new NanoFont(this.m_oView.ParentPanel.NodeScaledConnectorFont.FontFamily.Name,
                //        //                               this.m_oView.ParentPanel.NodeScaledConnectorFont.SizeInPoints),
                //        //                  new NanoSolidBrush(Color.Black),
                //        //                  (float)connectorTextPosition.X + 1,
                //        //                  (float)connectorTextPosition.Y + 1);

                //        //e.GC.DrawString(this.Name,
                //        //                      new NanoFont(this.m_oView.ParentPanel.NodeScaledConnectorFont.FontFamily.Name,
                //        //                                   this.m_oView.ParentPanel.NodeScaledConnectorFont.SizeInPoints),
                //        //                      new NanoSolidBrush(brsh.Color),
                //        //                      (float)connectorTextPosition.X,
                //        //                      (float)connectorTextPosition.Y);
                //    }
                //}
                //else
                //{
                //    brsh = brush;

                //    if (this.PlugType == ConnectorType.FluidInletTop ||
                //       this.PlugType == ConnectorType.FluidOutletBottom ||
                //        this.PlugType == ConnectorType.SprayInletBottom)
                //    {
                //        throw new System.NotImplementedException();
                //        {
                //            //

                //            //e.GC.DrawString(this.Name,
                //            //             this.m_oView.ParentPanel.NodeScaledConnectorFont,
                //            //             new NanoSolidBrush(this.m_oView.ParentPanel.NodeTextShadow.Color),//new SolidBrush(Color.Black), 
                //            //             (float)connectorTextPosition.X + 1,
                //            //             (float)connectorTextPosition.Y + 9 * this.View.CurrentViewZoom);

                //            //e.GC.DrawString(this.Name,
                //            //                 this.m_oView.ParentPanel.NodeScaledConnectorFont,
                //            //                 new NanoSolidBrush(brsh.Color),
                //            //                 (float)connectorTextPosition.X,
                //            //                 (float)connectorTextPosition.Y + 10 * this.View.CurrentViewZoom);
                //        }
                //    }

                //    //if(this.PlugType == ConnectorType.FluidOutletBottom)
                //    //{
                //    //    e.Graphics.DrawString(this.Name,
                //    //                     this.m_oView.ParentPanel.NodeScaledConnectorFont,
                //    //                     this.m_oView.ParentPanel.NodeTextShadow,//new SolidBrush(Color.Black),
                //    //                     (float)connectorTextPosition.X + 1,
                //    //                     (float)connectorTextPosition.Y - 9 * this.View.CurrentViewZoom);

                //    //    e.Graphics.DrawString(this.Name,
                //    //                     this.m_oView.ParentPanel.NodeScaledConnectorFont,
                //    //                     brsh,
                //    //                     (float)connectorTextPosition.X,
                //    //                     (float)connectorTextPosition.Y - 10 * this.View.CurrentViewZoom);
                //    //}
                //}
            }
        }
        public virtual NodeGraphData Process()
        {
            NodeGraphData result;
            //if (this.m_oPlugType == ConnectorType.FluidOutletBottom)
            //{
            //    result = this.m_oParentNode.Process();
            //}
            //else if (this.m_oPlugType == ConnectorType._GasOutlet)
            //{
            //    result = this.m_oParentNode.Process();
            //}
            //else if (this.m_oPlugType == ConnectorType.ReadInputConnector)
            //{
            //    result = this.m_oParentNode.Process();
            //}
            //else if (this.m_oPlugType == ConnectorType.WriteOutputConnector)
            //{
            //    result = this.m_oParentNode.Process();
            //}
            //else
            {
                NodeGraphPlug link = this.m_oView.ParentPanel.GetLink(this);
                if (link == null)
                {
                    result = new NodeGraphDataInvalid(this.m_oParentNode, "Plug:" + this.Name + " NOT LINKED");
                }
                else
                {
                    result = link.Process();
                }
            }
            return result;
        }

        #region Properties

        public Rectangle PlugRectangle { get; set; }
        public Rectangle PlugHitRectangle { get; set; }
        public NodeGraphNode Parent
        {
            get
            {
                return this.m_oParentNode;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public enuOPERATORS Type
        {
            get
            {
                return this.m_oPlugType;
            }
        }
        public NodeGraphNode ParentNode
        {
            get
            {
                return this.m_oParentNode;
            }
            set
            {
                this.m_oParentNode = value;
            }
        }
        public NodeViewState View
        {
            get
            {
                return this.m_oView;
            }
            set
            {
                this.m_oView = value;
            }
        }
        public enuOPERATORS PlugType
        {
            get
            {
                return this.m_oPlugType;
            }
            set
            {
                this.m_oPlugType = value;
            }
        }
        public int PlugIndex
        {
            get
            {
                return this.m_iPlugIndex;
            }
            set
            {
                this.m_iPlugIndex = value;
            }
        }
        public LinkVisualStyle LinkVisualStyle
        {
            get
            {
                return this.m_LinkVisualStyle;
            }
            set
            {
                this.m_LinkVisualStyle = value;
            }
        }

        internal NodeGraphDataType DataType
        {
            get
            {
                return this.m_oDataType;
            }
            set
            {
                this.m_oDataType = value;
            }
        }
        string INodeGraphPlug.Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        NodeGraphDataType INodeGraphPlug.DataType
        {
            get
            {
                return this.m_oDataType;
            }
            set
            {
                this.m_oDataType = value;
            }
        }
        #endregion
    }
}
