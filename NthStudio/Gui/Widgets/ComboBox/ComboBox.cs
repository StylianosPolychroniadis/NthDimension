using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets
{
    public partial class ComboBox : Widget
    {

        #region SmallerPicture

        //class SmallerPicture : Widget
        //{
        //    private Picture m_picture;

        //    private bool m_init = false;

        //    public SmallerPicture(string name)
        //    {
        //        this.m_picture = new Picture(name);

        //    }

        //    private void InitializeComponent()
        //    {
        //        this.m_picture.Size = new Size(this.Height - 5, this.Height - 5);
        //        this.m_picture.Location = new Point(this.Width / 2 - m_picture.Width / 2 - 1, this.Height / 2 - m_picture.Height / 2 - 1);
        //        this.Widgets.Add(this.m_picture);

        //        this.m_init = true;
        //    }

        //    protected override void DoPaint(GContext parentGContext)
        //    {
        //        if (!m_init)
        //            this.InitializeComponent();
        //        base.DoPaint(parentGContext);
        //    }
        //}

        #endregion SmallerPicture

        #region Download Simple Dialog

        //class DownloadSimple : DialogSimple
        //{
        //    public DownloadSimple(string title, string text)
        //        : base(title, text, 10, 10) { }



        //}
        #endregion

        public int SelectedIndex = -1;
        public Widget SelectedItem;

        //public delegate int ItemComparer(SceneItem item1, SceneItem item2);

        protected List<Widget> m_items = new List<Widget>();
        protected List<Widget> m_pendingItems = new List<Widget>();
        //protected SceneItem m_SelectedItem;
        ////protected ListView                  m_listPopup;
        //protected SceneList m_list;
        protected Widget m_list;


        //private SmallerPicture m_picScene;
        private Label m_lbCurrentScene;
        private Button m_btnDropDown;

        ////private DownloadSceneDialog m_downloadDetailDialog;
        //private DialogSimple m_downloadSimpleDialog;
        //protected DownloadSceneDialog m_downloadDialog = new DownloadSceneDialog(string.Empty);

        private bool m_init = false;

        public ComboBox()
        {
            //m_globeGlyph = Fonts.GetIconUTF8((int) Entypo.ICON_ADDRESS);
            //m_globeGlyph = Fonts.GetIconUTF8((int)Entypo.ICON_GLOBE);
        }
        public ComboBox(List<Widget> items) : this()
        {
            m_pendingItems.AddRange(items);
        }

        public void InitializeComponent()
        {



            #region DropDown Button
            m_btnDropDown = new Button(" ");
            m_btnDropDown.Size = new Size(30, 30);
            m_btnDropDown.Dock = EDocking.Right;
            m_btnDropDown.Location = new Point(this.Width - m_btnDropDown.Width, 0);
            //m_btnDropDown.Style = UserButton.DrawStyle.Glyph;
            //m_btnDropDown.GlyphIcon = (int)Entypo.ICON_TRIANGLE_DOWN;
            //m_btnDropDown.DrawBackground = false;
            //m_btnDropDown.BGColor = Themes.ThemeBase.Theme.DropDownBackColor;
            //m_btnDropDown.FGColor = Themes.ThemeBase.Theme.DropDownForeColor;
            m_btnDropDown.MouseClickEvent += delegate
            {
                this.handleDropDown();
            };
            //m_btnDropDown.ShowBoundsLines = true;
            #endregion DropDown Button

            this.Widgets.Add(m_btnDropDown);

            #region Scene Label
            m_lbCurrentScene = new Label();
            m_lbCurrentScene.Dock = EDocking.Fill;
            m_lbCurrentScene.FGColor = Color.WhiteSmoke; // TODO:: Move to Theme
            m_lbCurrentScene.TextAlign = ETextAlignment.Top | ETextAlignment.Right;
            m_lbCurrentScene.Text = "Virtual Reality Mode";
            //m_lbCurrentScene.ShowBoundsLines = true;
            m_lbCurrentScene.MouseClickEvent += delegate
            {
                this.handleDropDown();
            };
            #endregion Scene Label

            this.Widgets.Add(m_lbCurrentScene);

            #region Scenes List (DropDown)
            m_list = new Widget();
            m_list.Size = new Size(this.Width, this.Height * 10); // TODO:: Height = Max Items * Item.Height
            m_list.Location = new Point(this.X, this.Y);// + this.Height);
            //m_list.InitializeComponent();
            m_list.ShowBoundsLines = false;
            m_list.Hide();
            #endregion Scenes List

            if (m_pendingItems.Count > 0)
            {
                //foreach (SceneItem si in m_pendingItems)
                //    AddSceneItem(si);

                m_pendingItems.Clear();
            }

            this.m_init = true;
        }

        private void handleDropDown()
        {
            if (!m_list.IsVisible)
            {
                m_list.Show();
                //m_list.BringToFront(((WindowsGame)WindowsGame.Instance).Screen2D);
            }
            else
                m_list.Hide();
        }

        protected override void OnPaintBackground(GContext gc)
        {

            if (!m_init) return;

            base.OnPaintBackground(gc);

            float cornerRadius = Height / 2 - 1;

            #region Draw Rounded Rect


            NVGpaint bg;

            if (this.IsFocused)
            {
                bg = NanoVG.nvgBoxGradient(StudioWindow.vg, X,
                    Height + 1.5f,
                    Width,
                    Height,
                    Height / 2,
                    5,
                    Color.Red.ToNVGColor(),
                    Color.Blue.ToNVGColor());
            }
            else
            {
                bg = NanoVG.nvgBoxGradient(StudioWindow.vg, X,
                    Height + 1.5f,
                    Width,
                    Height,
                    Height / 2,
                    5,
                    Color.Black.ToNVGColor(),
                    Color.White.ToNVGColor());
            }

            NanoVG.nvgBeginPath(StudioWindow.vg);
            NanoVG.nvgRoundedRect(StudioWindow.vg, X, Y, Width, Height, cornerRadius);
            NanoVG.nvgClosePath(StudioWindow.vg);
            NanoVG.nvgFillPaint(StudioWindow.vg, bg);
            NanoVG.nvgFill(StudioWindow.vg);
            #endregion Draw Rounded Rect

            #region Progress Bar
           // if (null == m_SelectedItem)
            {
                #region Globe Glyph
                //gc.DrawGlyph(m_globeGlyph, new NanoSolidBrush(Color.White), 3, 25);
                //gc.DrawString("Virtual Reality Mode", new Rafa.Gui.Drawing.NanoFont(NanoFont.PlayRegular, 12f), new NanoSolidBrush(Color.White), 30, 20);

                //NanoVG.nvgFontSize(StudioWindow.vg, Size.Height * 1.3f);
                //NanoVG.nvgFontFace(StudioWindow.vg, "icons");
                //NanoVG.nvgFillColor(StudioWindow.vg, Color.White.ToNVGColor());


                //NanoVG.nvgTextAlign(StudioWindow.vg, (int)(NVGalign.NVG_ALIGN_CENTER | NVGalign.NVG_ALIGN_MIDDLE));
                //string sts = VRWindows.UITool.cpToUTF8((int)Entypo.ICON_GLOBE);
                //NanoVG.nvgText(WindowsGame.vg, Location.X + Size.Height * 0.55f, Location.Y + Size.Height * 0.55f, sts);
                #endregion Globe Glyph

                this.m_lbCurrentScene.Text = "Virtual Reality Mode";
                //this.m_lbCurrentScene.Invalidate();
            }
            //else
            {
                //if (m_SelectedItem.DownloadingData)
                //{
                //    float pWidth = (m_SelectedItem.DownloadProgress * (this.Width - m_list.ScrollbarWidth)) / 100;

                //    gc.FillRoundedRect(new NanoSolidBrush(Color.FromArgb(255, 100, 134, 165)), 0, 0, (int)pWidth, Height, cornerRadius);
                //}

                //if (m_SelectedItem.SceneIsLoading)
                //{
                //    //float pWidth = (m_SelectedItem.LoadingAssetsProgress * this.Width) / 100;
                //    float pWidth = m_SelectedItem.LoadingAssetsProgress * (this.Width / 100);

                //    gc.FillRoundedRect(new NanoSolidBrush(Color.FromArgb(255, 100, 134, 165)), 0, 0, (int)pWidth, Height, cornerRadius);

                //    string state = pWidth < 20f ? "Preparing" : "Loading";
                //    string loading = string.Format("  {0} {1}  {2}%", state, m_SelectedItem.SceneName, m_SelectedItem.LoadingAssetsProgress.ToString("##0.0"));


                //    this.m_lbCurrentScene.Text = loading;
                //}
            }
            #endregion Progress Bar

        }



        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(e);

            if (m_showDownloadDialog)
            {
                ////if(m_downloadSimpleDialog != null)
                //{
                //    //  if(!m_downloadSimpleDialog.IsHide)
                //    {
                //        if (m_SelectedItem != null)
                //        {
                //            m_showDownloadDialog = false;
                //            this.showDownloadDialog(m_SelectedItem);

                //        }
                //    }
                //}
            }

            //if (downloadList.Count > 0)
            //{
            //    doShowDownloadSceneItem(downloadList[0]);
            //    downloadList.Clear();
            //}
        }


        private bool m_showDownloadDialog = false;

        public override void Show()
        {
            // TODO: Display SceneList
            if (!m_list.IsVisible)
            {
                m_list.Show();
                //m_list.BringToFront(((WindowsGame)WindowsGame.Instance).Screen2D);
            }
            //else
            //    m_list.Hide();

            base.Show();
        }

    }
}
