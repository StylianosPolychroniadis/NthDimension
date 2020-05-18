using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthStudio.Utilities;
using NthDimension.Forms.Delegates;
using System.Xml.Serialization;
using NthDimension.Forms.Widgets;
using NthStudio.NodeGraph.Xml;
using NthDimension.Forms.Events;
using NthStudio.Properties;
using System.Reflection;
using System.Globalization;

namespace NthStudio.NodeGraph
{
    [TypeConverter(typeof(PropertySorter))]
    [DefaultProperty("Name")]
    public class NodeGraphNode
    {
        public event PaintEventHandler onPostDraw;

        #region Properties
        protected const string FIRST_CATEGORY = "\r Node";


        [Category(FIRST_CATEGORY), PropertyOrder(0)]
        public string Name
        {
            get
            {
                return this.GetName();
            }
        }
        [Category(FIRST_CATEGORY), PropertyOrder(1)]
        public string Title
        {
            get
            {
                return this._mSTitle;
            }
            set
            {
                this._mSTitle = value;

                //if (this is IHeatExchangerNode)
                //    ((IHeatExchangerNode)this).DataTableDaq.TableName = "Heat Exchanger" +
                //                                                      ((IHeatExchangerNode)this).HxsId.ToString();

            }
        }
        //[Browsable(false)]
        //public List<FloatingTextItem> GasPathTextItems = new List<FloatingTextItem>();
        //[Browsable(false)]
        //public List<FloatingTextItem> AirPathTextItems = new List<FloatingTextItem>();
        //[Browsable(false)]
        //public List<FloatingTextItem> FluidPathTextItems = new List<FloatingTextItem>();


        [Browsable(false)]
        public bool CanBeSelected
        {
            get
            {
                return this.m_bCanBeSelected;
            }
            set
            {
                this.m_bCanBeSelected = value;
            }
        }
        [Browsable(false)]
        public int X;
        [Browsable(false)]
        public int Y;
        [Browsable(false)]
        public bool Highlighted;
        [Browsable(false)]
        public Rectangle HitRectangle;

        




        public int Width
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
                this.UpdateHitRectangle();
            }
        }
        [Browsable(false)]
        public int Height
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
                this.UpdateHitRectangle();
            }
        }
        [Browsable(false)]
        public NodeViewState ParentView
        {
            get
            {
                return this.m_oView;
            }
        }
        [Browsable(false)]
        public List<NodeGraphPlug> Plugs
        {
            get
            {
                return this.m_Plugs;
            }
        }
        [Browsable(false)]
        public Rectangle HeaderRectangle { get; set; }
        [Browsable(false)]
        public Rectangle LedTopRectangle { get; set; }
        [Browsable(false)]
        public Rectangle LedBottomRectangle { get; set; }
        [Browsable(false)]
        public Rectangle BackgoundImageRectangle { get; set; }
        [Browsable(false), XmlIgnore]
        public ContextMenuStrip ContextMenu { get; set; }
        [Browsable(false)]
        public bool ShowInletLed { get; set; }
        [Browsable(false)]
        public bool ShowOutletLed { get; set; }

        #endregion

        private int m_Height;
        private int m_Width;
        private bool m_bCanBeSelected;
        private string _mSTitle;

        protected List<NodeGraphPlug> m_Plugs;
        protected NodeViewState m_oView;
        protected string m_sName;



        #region Ctor
        public NodeGraphNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected)
        {
            this.X = p_X;
            this.Y = p_Y;
            this.m_oView = p_View;
            this.m_Width = 140;
            this.m_Height = 64;
            this.m_sName = "Node";
            this.m_bCanBeSelected = p_CanBeSelected;
            this.Highlighted = false;
            this._mSTitle = "";
            this.UpdateHitRectangle();
            this.m_Plugs = new List<NodeGraphPlug>();
        }
        public NodeGraphNode(XmlTreeNode p_Input, NodeViewState p_View)
        {
            this.m_oView = p_View;
            this.m_sName = p_Input.m_attributes["Name"];
            this._mSTitle = p_Input.m_attributes["Title"];
            this.X = int.Parse(p_Input.m_attributes["X"]);
            this.Y = int.Parse(p_Input.m_attributes["Y"]);
            this.Width = int.Parse(p_Input.m_attributes["Width"]);
            this.Height = int.Parse(p_Input.m_attributes["Height"]);
            this.m_bCanBeSelected = bool.Parse(p_Input.m_attributes["CanBeSelected"]);
            this.m_Plugs = new List<NodeGraphPlug>();
        }
        #endregion

        public int GetConnectorIndex(NodeGraphPlug pPlug)
        {
            int result;
            for (int i = 0; i < this.m_Plugs.Count; i++)
            {
                if (this.m_Plugs[i] == pPlug)
                {
                    result = i;
                    return result;
                }
            }
            result = -1;
            return result;
        }
        protected virtual string GetName()
        {
            return this.m_sName;
        }
        public virtual void UpdateHitRectangle()
        {
            this.HitRectangle = new Rectangle(this.X, this.Y, this.Width, this.Height);
        }
        public NodeGraphPlug GetConnectorMouseHit(Point p_ScreenPosition)
        {
            Rectangle rectangle = new Rectangle(p_ScreenPosition, Size.Empty);
            NodeGraphPlug result;
            foreach (NodeGraphPlug current in this.m_Plugs)
            {
                if (rectangle.IntersectsWith(current.GetHitArea()))
                {
                    result = current;
                    return result;
                }
            }
            result = null;
            return result;
        }
        public virtual void Draw(PaintEventArgs e, bool useCustomPoints = false, bool showHeader = true)
        {
            Point point = this.m_oView.ParentPanel.ViewToControl(new Point(this.X, this.Y));
            int x = point.X;
            int y = point.Y;

            Rectangle rect = new Rectangle(point.X, point.Y, (int)((float)this.HitRectangle.Width * this.m_oView.CurrentViewZoom), (int)((float)this.HitRectangle.Height * this.m_oView.CurrentViewZoom));

            this.HeaderRectangle = new Rectangle(rect.X, rect.Y, rect.Width,
                (int)((float)this.m_oView.ParentPanel.NodeHeaderSize * this.m_oView.CurrentViewZoom));

            this.BackgoundImageRectangle =
                this.ParentView.ParentPanel.ViewToControl(new Rectangle(this.X - (int)(0.1f * (float)this.Width) + 4,
                    this.Y - (int)(0.1f * (float)this.Height) + 4, this.Width + (int)(0.2f * (float)this.Width) - 4,
                    this.Height + (int)(0.2f * (float)this.Height) - 4));


            #region Draw Rectangular Node
            if (!useCustomPoints)
            {
                if (this.ParentView.ParentPanel.DrawShadow)
                {
                    e.GC.DrawImage(Resources.NodeShadow, BackgoundImageRectangle);
                }
                if (!this.Highlighted)
                {
                    if (showHeader)
                    {
                        e.GC.FillRectangle(this.m_oView.ParentPanel.NodeHeaderFill, HeaderRectangle);                        
                    }
                    else
                    {
                        rect.Height -= this.HeaderRectangle.Height;
                        rect.Y = this.HeaderRectangle.Height;
                    }

                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeFill, rect);
                    e.GC.DrawRectangle(new NthDimension.Forms.NanoPen(this.m_oView.ParentPanel.NodeOutline), rect);                   
                }
                else
                {
                    if (showHeader)
                    {
                        e.GC.FillRectangle(this.m_oView.ParentPanel.NodeHeaderFill, HeaderRectangle);
                    }
                    else
                    {
                        rect.Height -= this.HeaderRectangle.Height;
                        rect.Y = this.HeaderRectangle.Height;
                    }
                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeFillSelected, rect);
                    e.GC.DrawRectangle(new NthDimension.Forms.NanoPen(this.m_oView.ParentPanel.NodeOutlineSelected), rect);
                }
                }
            #endregion

            #region Draw un-Highlighted Custom Polygon
            else if (!this.Highlighted)
            {
                if (showHeader)
                {
                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeFill, this.HeaderRectangle);
                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeHeaderFill, this.HeaderRectangle);
                }
                this.DrawCustomPolygon(e);
            }
            #endregion

            #region Draw Highlighted Custom Polygon
            else
            {
                if (showHeader)
                {
                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeFillSelected, this.HeaderRectangle);
                    e.GC.FillRectangle(this.m_oView.ParentPanel.NodeHeaderFill, this.HeaderRectangle);
                }
                this.DrawCustomPolygon(e);
            }
            #endregion

            #region Draw Node Leds
            Point pointLedTop = new Point();
            Point pointLedBottom = new Point();

            if (showHeader)
            {
                pointLedTop = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.Width / 2 + 10, this.Y + 4));
                pointLedBottom = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.Width / 2 + 10, this.Y + this.Height - 24));
            }
            else
            {
                pointLedTop = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.Width / 2 + 10, this.Y + 4));
                pointLedBottom = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.Width / 2 + 10, this.Y + this.Height - 24));
            }

            this.LedTopRectangle = new Rectangle(pointLedTop.X,
                                                 pointLedTop.Y,
                                                 (int)(16f * this.m_oView.CurrentViewZoom),
                                                 (int)(16f * this.m_oView.CurrentViewZoom));

            this.LedBottomRectangle = new Rectangle(pointLedBottom.X,
                                                    pointLedBottom.Y,
                                                    (int)(16f * this.m_oView.CurrentViewZoom),
                                                    (int)(16f * this.m_oView.CurrentViewZoom));
            //if(showHeader)
            if (ShowInletLed)
                if (this.IsValid())
                    e.GC.DrawImage(Resources.NodeValid, this.LedTopRectangle);
                else
                    e.GC.DrawImage(Resources.NodeInvalid, this.LedTopRectangle);

            if (ShowOutletLed)
                if (this.IsValid())
                    e.GC.DrawImage(Resources.NodeValid, this.LedBottomRectangle);
                else
                    e.GC.DrawImage(Resources.NodeInvalid, this.LedBottomRectangle);
            #endregion

            #region Draw Header Text and Floating Text
            if (this.m_oView.CurrentViewZoom > this.m_oView.ParentPanel.NodeTitleZoomThreshold)
            {
                #region Draw Header Text

                //showHeader = false;
                if (showHeader)
                {
                    //throw new System.NotImplementedException();
                    {

                        e.GC.DrawString(this.Name, this.m_oView.ParentPanel.NodeScaledTitleFont,
                        new NthDimension.Forms.NanoSolidBrush(this.m_oView.ParentPanel.NodeTextShadow.Color),
                        //new SolidBrush(Color.Black),
                        x + (int)(2f * this.m_oView.CurrentViewZoom) + 1,
                            y + (int)(2f * this.m_oView.CurrentViewZoom) + 1);
                        e.GC.DrawString(this.Name,
                            //this.m_oView.ParentPanel.NodeScaledTitleFont.,
                            new NthDimension.Forms.NanoSolidBrush(this.m_oView.ParentPanel.NodeText.Color),
                            x + (int)(2f * this.m_oView.CurrentViewZoom),
                                y + (int)(2f * this.m_oView.CurrentViewZoom));
                    }
                }

                #endregion

                #region Draw Floating Text Items
                int num = 0;
                List<FloatingTextItem> list = new List<FloatingTextItem>();
                List<FloatingTextItem> list2 = new List<FloatingTextItem>();

                #region Gas Path Items
                //if (this.ParentView.ParentPanel.DisplayLayer == NodeGraphPanel.enuFloatingTextLayers.AllPaths ||
                //    this.ParentView.ParentPanel.DisplayLayer == NodeGraphPanel.enuFloatingTextLayers.GasPath)
                //    foreach (FloatingTextItem current in this.GasPathTextItems)
                //    {
                //        SizeF sizeF = e.GC.MeasureString(current.Text, this.m_oView.ParentPanel.NodeScaledTitleFont);
                //        if (sizeF.Width > (float)num)
                //            num = (int)sizeF.Width;
                //        if (current.TextType == FloatingTextItem.NodeAlignment.Input)
                //            list.Add(current);
                //        if (current.TextType == FloatingTextItem.NodeAlignment.Output)
                //            list2.Add(current);
                //    }
                //else
                //    foreach (FloatingTextItem current in this.GasPathTextItems)
                //    {
                //        if (list.Contains(current))
                //            list.Remove(current);
                //        if (list2.Contains(current))
                //            list2.Remove(current);
                //    }

                #endregion

                #region Draw List A (left)
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].TextType == FloatingTextItem.NodeAlignment.Input)
                        {
                            Point point3 = this.m_oView.ParentPanel.ViewToControl(new Point(this.X, this.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + i * 16));
                            point3.X = point3.X - num - (int)(16f * this.m_oView.CurrentViewZoom);
                            list[i].Point = point3;
                            //e.Graphics.DrawString(list[i].Text, this.m_oView.ParentPanel.NodeScaledTitleFont, this.m_oView.ParentPanel.NodeTextShadow, list[i].Point);
                            throw new System.NotImplementedException();
                            {
                                ////
                                //e.GC.DrawString(list[i].Text,
                                //            this.m_oView.ParentPanel.NodeScaledTitleFont,
                                //            this.m_oView.ParentPanel.NodeTextShadow,
                                //            new Point(list[i].Point.X + 1,
                                //                      list[i].Point.Y + 1));

                                //e.GC.DrawString(list[i].Text,
                                //                this.m_oView.ParentPanel.NodeScaledTitleFont,
                                //                this.m_oView.ParentPanel.NodeText,
                                //                list[i].Point);
                            }
                        }
                    }
                }
                #endregion

                #region Draw List B (right)
                if (list2.Count > 0)
                {
                    for (int i = 0; i < list2.Count; i++)
                    {
                        if (list2[i].TextType == FloatingTextItem.NodeAlignment.Output)
                        {
                            Point point3 = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.HitRectangle.Width, this.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + i * 16));
                            point3.X += (int)(16f * this.m_oView.CurrentViewZoom);
                            list2[i].Point = point3;

                            throw new System.NotImplementedException();
                            {
                                //
                                //e.GC.DrawString(list2[i].Text,
                                //            this.m_oView.ParentPanel.NodeScaledTitleFont,
                                //            this.m_oView.ParentPanel.NodeTextShadow,
                                //            new Point(list2[i].Point.X + 1,
                                //                      list2[i].Point.Y + 1));

                                //e.GC.DrawString(list2[i].Text,
                                //                this.m_oView.ParentPanel.NodeScaledTitleFont,
                                //                this.m_oView.ParentPanel.NodeText,
                                //                list2[i].Point);
                            }
                        }
                    }
                }
                #endregion
                #endregion
            }
            #endregion

            #region Draw Plugs
            for (int j = 0; j < this.m_Plugs.Count; j++)
            {
                this.m_Plugs[j].Draw(e, j);
            }
            #endregion

            #region Draw above-Header Title
            if (this._mSTitle != "")
            {
                string hxsId = string.Empty;

                //if (this is IHeatExchangerNode)
                //    hxsId = "#" + ((IHeatExchangerNode)this).HxsId.ToString() + " : ";

                throw new System.NotImplementedException();
                {
                    ////
                    //e.GC.DrawString(hxsId + this._mSTitle,
                    //            this.m_oView.ParentPanel.NodeScaledTitleFont,
                    //            this.m_oView.ParentPanel.NodeText,
                    //            new Point(x, y - (int)(20f * this.m_oView.CurrentViewZoom)));
                }
            }
            #endregion

            #region Post Draw
            if (this.onPostDraw != null)
            {
                this.onPostDraw(this, e);
            }
            #endregion
        }
        public virtual void DrawCustomPolygon(PaintEventArgs e)
        {
        }
        //private Point GetCustomTextPosition(PaintEventArgs e, FloatingTextItem.NodeAlignment textType, int customTextIndex)
        //{
        //    Point result;
        //    if (textType == FloatingTextItem.NodeAlignment.Input)
        //    {
        //        result = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + 16, this.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + customTextIndex * 16));
        //    }
        //    else
        //    {
        //        SizeF sizeF = e.Graphics.MeasureString(this.Name, this.m_oView.ParentPanel.NodeScaledConnectorFont);
        //        result = this.m_oView.ParentPanel.ViewToControl(new Point(this.X + this.HitRectangle.Width, this.Y + this.m_oView.ParentPanel.NodeHeaderSize + 4 + customTextIndex * 16));
        //        result.X = result.X - (int)(16f * this.m_oView.CurrentViewZoom) - (int)sizeF.Width;
        //    }
        //    return result;
        //}
        public virtual bool IsValid()
        {
            bool result = true;
            foreach (NodeGraphPlug current in this.m_Plugs)
            {
               // if (current.Type == ConnectorType.FluidInletTop)
                {
                    if (!current.CanProcess())
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        //public List GetInputData()
        //{
        //    // Note: NOT USED (Originally used in Reference Node)

        //    List nodeGraphListData = new List();
        //    //foreach (NodeGraphPlug current in this.m_Plugs)
        //    //{
        //    //    if (current.Type == ConnectorType.FluidInletTop)
        //    //    {
        //    //        nodeGraphListData.AddData(current.Process());
        //    //    }
        //    //}
        //    return nodeGraphListData;
        //}
        public virtual NodeGraphData Process()
        {
            return null;
        }
        public virtual NodeGraphData Process(NodeGraphData inputData)
        {
            return null;
        }

        public virtual XmlTreeNode SerializeToXML(XmlTreeNode p_Parent)
        {
            XmlTreeNode xmlTreeNode = new XmlTreeNode(SerializationUtils.GetFullTypeName(this), p_Parent);
            xmlTreeNode.AddParameter("Name", this.Name);
            xmlTreeNode.AddParameter("X", this.X.ToString());
            xmlTreeNode.AddParameter("Y", this.Y.ToString());
            xmlTreeNode.AddParameter("Width", this.Width.ToString());
            xmlTreeNode.AddParameter("Height", this.Height.ToString());
            xmlTreeNode.AddParameter("Title", this.Title.ToString());
            xmlTreeNode.AddParameter("CanBeSelected", this.CanBeSelected.ToString());
            return xmlTreeNode;
        }





        public static NodeGraphNode DeserializeFromXML(XmlTreeNode p_Node, NodeViewState p_View)
        {
            string nodeName = p_Node.m_nodeName;
            object[] args = new object[]
            {
                p_Node,
                p_View
            };
            object obj = Assembly.GetEntryAssembly().CreateInstance(nodeName,
                                                                    false,
                                                                    BindingFlags.CreateInstance,
                                                                    null,
                                                                    args,
                                                                    CultureInfo.GetCultureInfo("en-US"),
                                                                    null);
            // TODO Subclass to Interfaces
            ////if (obj is IHeatExchangerNode)
            ////{
            ////    // Common Hxs Interface
            ////    if (p_Node.m_attributes.ContainsKey("HxsId"))
            ////        ((IHeatExchangerNode)obj).HxsId = int.Parse(p_Node.m_attributes["HxsId"]);
            ////    if (p_Node.m_attributes.ContainsKey("TGCycleInterface"))
            ////        ((IHeatExchangerNode)obj).TGCycleInterface = p_Node.m_attributes["TGCycleInterface"];
            ////    if (p_Node.m_attributes.ContainsKey("GeometryFactor"))
            ////        ((IHeatExchangerNode)obj).GeometryFactor = double.Parse(p_Node.m_attributes["GeometryFactor"]);
            ////    if (p_Node.m_attributes.ContainsKey("SurfaceArea"))
            ////        ((IHeatExchangerNode)obj).SurfaceArea = double.Parse(p_Node.m_attributes["SurfaceArea"]);
            ////    if (p_Node.m_attributes.ContainsKey("CrossSectionalArea"))
            ////        ((IHeatExchangerNode)obj).CrossSectionalArea = double.Parse(p_Node.m_attributes["CrossSectionalArea"]);
            ////    if (p_Node.m_attributes.ContainsKey("GasRecirculationFraction"))
            ////        ((IHeatExchangerNode)obj).GasRecirculationFraction = double.Parse(p_Node.m_attributes["GasRecirculationFraction"]);
            ////    if (p_Node.m_attributes.ContainsKey("GasFlowSplits"))
            ////        ((IHeatExchangerNode)obj).GasFlowSplits = p_Node.m_attributes["GasFlowSplits"];
            ////    if (p_Node.m_attributes.ContainsKey("HxsCoefficientsA"))
            ////        ((IHeatExchangerNode)obj).HxsCoefficientsA = double.Parse(p_Node.m_attributes["HxsCoefficientsA"]);
            ////    if (p_Node.m_attributes.ContainsKey("HxsCoefficientsB"))
            ////        ((IHeatExchangerNode)obj).HxsCoefficientsB = double.Parse(p_Node.m_attributes["HxsCoefficientsB"]);
            ////    if (p_Node.m_attributes.ContainsKey("HxsCoefficientsAA"))
            ////        ((IHeatExchangerNode)obj).HxsCoefficientsAA = double.Parse(p_Node.m_attributes["HxsCoefficientsAA"]);
            ////    if (p_Node.m_attributes.ContainsKey("HxsCoefficientsBB"))
            ////        ((IHeatExchangerNode)obj).HxsCoefficientsBB = double.Parse(p_Node.m_attributes["HxsCoefficientsBB"]);
            ////    if (p_Node.m_attributes.ContainsKey("HxsCoefficientsCC"))
            ////        ((IHeatExchangerNode)obj).HxsCoefficientsCC = double.Parse(p_Node.m_attributes["HxsCoefficientsCC"]);
            ////    // IHeatExchangerGasPath
            ////    if (obj is IHeatExchangerGasComponent)
            ////    {
            ////        if (p_Node.m_attributes.ContainsKey("NoOfGasInlets"))
            ////            ((IHeatExchangerGasComponent)obj).NoOfGasInlets = int.Parse(p_Node.m_attributes["NoOfGasInlets"]);
            ////        if (p_Node.m_attributes.ContainsKey("HxsIdFromGasPath"))
            ////            ((IHeatExchangerGasComponent)obj).HxsIdFromGasPath = p_Node.m_attributes["HxsIdFromGasPath"];

            ////    }
            ////    // IHeatExchangerFluidComponent
            ////    if (obj is IHeatExchangerFluidComponent)
            ////    {
            ////        if (p_Node.m_attributes.ContainsKey("HxsIdFromFluidPath"))
            ////            ((IHeatExchangerFluidComponent)obj).HxsIdFromFluidPath =
            ////                int.Parse(p_Node.m_attributes["HxsIdFromFluidPath"]);

            ////        if (p_Node.m_attributes.ContainsKey("PressureDrop"))
            ////            ((IHeatExchangerFluidComponent)obj).PressureDrop = double.Parse(p_Node.m_attributes["PressureDrop"]);
            ////    }

            ////    if (obj is IHeatExchangerAirComponent)
            ////    {
            ////        if (p_Node.m_attributes.ContainsKey("HxsIdFromAirPath"))
            ////            ((IHeatExchangerAirComponent)obj).HxsIdFromAirPath =
            ////                int.Parse(p_Node.m_attributes["HxsIdFromAirPath"]);
            ////    }

            ////    // Unique to HeatExchangerNode
            ////    if (obj is IHeatExchangerSprays)
            ////    {

            ////        if (p_Node.m_attributes.ContainsKey("UseInletSprays"))
            ////            ((IHeatExchangerSprays)obj).UseInletSprays = bool.Parse(p_Node.m_attributes["UseInletSprays"]);
            ////        if (p_Node.m_attributes.ContainsKey("UseOutletSprays"))
            ////            ((IHeatExchangerSprays)obj).UseOutletSprays = bool.Parse(p_Node.m_attributes["UseOutletSprays"]);
            ////        if (p_Node.m_attributes.ContainsKey("InletSprayFlow"))
            ////            ((IHeatExchangerSprays)obj).InletSprayFlow = double.Parse(p_Node.m_attributes["InletSprayFlow"]);
            ////        if (p_Node.m_attributes.ContainsKey("InletSprayPressure"))
            ////            ((IHeatExchangerSprays)obj).InletSprayPressure = double.Parse(p_Node.m_attributes["InletSprayPressure"]);
            ////        if (p_Node.m_attributes.ContainsKey("InletSprayTemperature"))
            ////            ((IHeatExchangerSprays)obj).InletSprayTemperature = double.Parse(p_Node.m_attributes["InletSprayTemperature"]);
            ////        if (p_Node.m_attributes.ContainsKey("OutletSprayFlow"))
            ////            ((IHeatExchangerSprays)obj).OutletSprayFlow = double.Parse(p_Node.m_attributes["OutletSprayFlow"]);
            ////        if (p_Node.m_attributes.ContainsKey("OutletSprayPressure"))
            ////            ((IHeatExchangerSprays)obj).OutletSprayPressure = double.Parse(p_Node.m_attributes["OutletSprayPressure"]);
            ////        if (p_Node.m_attributes.ContainsKey("OutletSprayTemperature"))
            ////            ((IHeatExchangerSprays)obj).OutletSprayTemperature = double.Parse(p_Node.m_attributes["OutletSprayTemperature"]);

            ////    }
            ////    //EsiApplication.Instance.LOG_ESI_Msg("Failed to deserialize node " + nodeName);
            ////    //return null;        // Something went wrong
            ////}

            //////if (obj is SprayHNode)
            //////{
            //////    if(p_Node.m_attributes.ContainsKey("HxsIdFromGasPath"))
            //////        ((SprayHNode)obj).HxsIdFromGasPath  = int.Parse(p_Node.m_attributes["HxsIdFromGasPath"]);
            //////    return obj as SprayHNode;
            //////}

            //////if (obj is SprayVNode)
            //////{
            //////    if (p_Node.m_attributes.ContainsKey("HxsIdFromGasPath"))
            //////        ((SprayVNode)obj).HxsIdFromGasPath = int.Parse(p_Node.m_attributes["HxsIdFromGasPath"]);
            //////    return obj as SprayVNode;
            //////}

            ////if (obj is AirHeaterNode) return obj as AirHeaterNode;
            ////if (obj is APHNode) return obj as APHNode;
            ////if (obj is FDFanNode) return obj as FDFanNode;
            ////if (obj is FurnaceNode) return obj as FurnaceNode;
            ////if (obj is HeatExchangerNode) return obj as HeatExchangerNode;
            ////if (obj is IDFanNode) return obj as IDFanNode;

            return obj as NodeGraphNode;
        }
        public static NodeGraphNode GenerateInstanceFromName(Assembly p_Assembly, string p_TypeName, object[] p_ConstructorArgs)
        {
            object obj = p_Assembly.CreateInstance(p_TypeName, false, BindingFlags.CreateInstance, null, p_ConstructorArgs, CultureInfo.GetCultureInfo("en-US"), null);
            return obj as NodeGraphNode;
        }

        public virtual void ShowContextMenu(Point pos)
        {
            if (null != ContextMenu)
                //ContextMenu.Show(this.ParentView.ParentPanel, pos);
            throw new System.NotImplementedException("Show(WHUD)");
        }
    }
}
