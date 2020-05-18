using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rasterizer.NanoVG;
using NthDimension.Utilities;
using NthStudio.Gui.Displays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NthStudio.Gui.Displays.NanoGContext;

namespace NthStudio.Gui.Widgets
{
    public class TextInput : Widget
    {

        //public event EchoMyMessage OnEchoMessage;

        private string _defaultText = "Type your message here";
        private int maximumCharacters = 60;

        private int m_lastBlinkCheck;
        private bool showCarret;
        private int charPos = 0;

        private Font defaultFont;
        private readonly Color defaultColor;
        private readonly Color inactiveColor;
        private RectangleF closeBtn;
        private PointF mouse;

        public string Text { get; set; }

        public TextInput()
        {
            this.Text = _defaultText;
            this.Font = new NanoFont(NanoFont.DefaultRegular, 8f);

        }

        public void SetDefaultText(string text)
        {
            _defaultText = text;
        }

        public virtual float MeasureTextWidth(string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(StudioWindow.vg, font.Id))
                throw new Exception(String.Format("Font '{0}', not created", font.ToString()));

            //NanoVG.nvgSave(WindowsGame.vg);

            NanoVG.nvgFontSize(StudioWindow.vg, font.Height);
            NanoVG.nvgFontFace(StudioWindow.vg, font.Id);

            float textWidth = NanoVG.nvgTextBounds(StudioWindow.vg, 0, 0, text);

            //NanoVG.nvgRestore(WindowsGame.vg);

            return textWidth;
        }

        #region Input Handling
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyData == Keys.Enter)
            {
                Guid usr = Guid.Empty;

                //Guid.TryParse(((ChatTabItem)Parent).UserId, out usr);


                //if (null != OnEchoMessage)
                //    this.OnEchoMessage(this.Text);

                this.Text = string.Empty;
                this.charPos = 0;
            }
            else
                this.Text = StringUtil.KeyEvent(Text, ref this.charPos, (int)e.KeyData);

            e.Handled = true;
        }
        protected override void OnKeyPress(KeyPressedEventArgs e)
        {
            base.OnKeyPress(e);

            if (this.Text.Length < maximumCharacters)
                this.Text = StringUtil.CharEvent(this.Text, ref charPos, e.KeyChar);


            e.Handled = true;
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.FGColor = defaultColor;
            this.showCarret = true;

            if (this.Text == _defaultText)
                this.Text = string.Empty;

        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.FGColor = inactiveColor;
            this.showCarret = false;

            if (this.Text == string.Empty)
            {
                this.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
                this.Text = _defaultText;
            }

        }
        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);


            closeBtn = new RectangleF(Size.Width - Size.Height * 0.8f,
                this.Location.Y,
                Size.Width,
                Size.Height);

            if (closeBtn.Contains(e.X, e.Y))
            {
                this.Text = string.Empty;
                this.charPos = this.Text.Length;
            }

            if (!this.IsFocused)
                this.Focus();

            if (this.Text == "Send to everyone")
            {
                this.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
                this.Text = string.Empty;
                this.charPos = this.Text.Length;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mouse = new PointF(e.X, e.Y);
        }
        #endregion Input Handling

        #region Draw
        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);
            NVGcontext vg = StudioWindow.vg;

            if (!NanoVG.FontCreated(vg, Font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", Font.InternalName));

            if (Width < 0 || Height < 0)
                return;

            //int nx = Location.X + Parent.Location.X + Parent.Parent.Location.X + Parent.Parent.Parent.Location.X;// + 160;// x + topWinPos.X;
            //int ny = Location.Y + Parent.Location.Y + Parent.Parent.Location.Y + Parent.Parent.Parent.Location.Y;//39;// y + topWinPos.Y;

            int nx = e.GC.TopWinPos.X;// x + topWinPos.X;
            int ny = Location.Y + e.GC.TopWinPos.Y;// y + topWinPos.Y;

            drawSearchBoxBase(vg, nx, ny, Width, Height);

            this.FGColor = Color.Black;//ThemeBase.Theme.WindowDialoguesFontColor;//Color.Black;
            NanoVG.nvgFontSize(vg, Font.Height);
            NanoVG.nvgFontFace(vg, Font.InternalName);
            NanoVG.nvgFillColor(vg, e.GC.ColorToNVGcolor(this.FGColor)); // NanoVG.RGBA(255, 255, 255, 64));
            NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));

            NanoVG.nvgText(vg,
                           //nx + this.Size.Height * 1.05f + Width / 2 - ThemeBase.Theme.ScrollBarSize.Width,
                           nx + this.Size.Height * 1.05f - /*ThemeBase.Theme.ScrollBarSize.Width*/13,
                           ny + Height * 0.55f,
                           //nx, ny,
                           Text);
            //NanoVG.nvgText(vg, 80, 0, Text);

            #region Carret
            //if (this.IsFocused && this.showCarret) // Draw Caret
            if (this.showCarret)
            {
                if (IsFocused &&
                    (int)(WHUD.MainTimer() * 2) % 2 != this.m_lastBlinkCheck)
                {
                    this.drawCarret(vg);
                    this.m_lastBlinkCheck = (int)(WHUD.MainTimer() * 2) % 2;
                }



            }

            #endregion

            //base.OnPaint(e);

        }
        private void drawSearchBoxBase(NVGcontext vg, float x, float y, float w, float h)
        {
            float cornerRadius = h / 2 - 1;

            NVGpaint bg = NanoVG.nvgBoxGradient(vg, x,
                h + 1.5f,
                w,
                h,
                h / 2,
                5,
                new Color4f(255, 255, 255, 255).ToNVGColor(),
                new Color4f(255, 255, 255, 255).ToNVGColor());
            //NanoVG.nvgRGBA(255, 255, 255, 64), NanoVG.nvgRGBA(235, 235, 235, 64));

            NanoVG.nvgBeginPath(vg);
            //NanoVG.nvgRect(vg, x, y, w, h);
            NanoVG.nvgRoundedRect(vg,
                                  //w / 2 + x - ThemeBase.Theme.ScrollBarSize.Width, //TO DO:: scrollBar.Size.Width
                                  x - ThemeBase.Theme.ScrollBarSize.Width, //TO DO:: scrollBar.Size.Width
                                  y,
                                  0,//w / 2,
                                  h,
                                  cornerRadius);

            NanoVG.nvgClosePath(vg);

            NanoVG.nvgFillPaint(vg, bg);
            NanoVG.nvgFill(vg);

            //WindowDialogues.Chat.ChatItem.

            #region Magnifying Glass Icon - TODO // Switch to DrawGlyph
            //NanoVG.nvgFontSize(vg, h * 0.8f); //ICONS SIZE
            //NanoVG.nvgFontFace(vg, "icons");
            ////NanoVG.nvgFillColor(vg, this.focused ? Theme.LightTheme.SearchBoxGradientIconSearchFocusedColor : Theme.LightTheme.SearchBoxGradientIconSearchUnfocusedColor);
            //NanoVG.nvgFillColor(vg, ThemeBase.Theme.WindowDialoguesChatBoxIconsColor);//ThemeOld.LightThemeOld.SearchBoxGradientIconSearchFocusedColor);
            //NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_CENTER | NVGalign.NVG_ALIGN_MIDDLE));
            //string sts = VRWindows.UITool.cpToUTF8((int)Entypo.ICON_CHAT);
            //NanoVG.nvgText(vg,
            //               0 + x + h * 0.55f, //TO DO:: scrollBar.Size.Width
            //               y + h * 0.55f,
            //               sts);
            #endregion

            #region Clear Icon  - TODO // Switch to DrawGlyph
            NanoVG.nvgFontSize(vg, h * 0.8f); //ICONS SIZE
            NanoVG.nvgFontFace(vg, "icons");
            //NanoVG.nvgFillColor(vg, this.focused ? theme.searchBoxGradientIconClearFocusedColor : theme.searchBoxGradientIconClearUnfocusedColor);
            NanoVG.nvgFillColor(vg, HoveredWidget == this /*&& this.IsFocused*/ && closeBtn.Contains(mouse) ? new NVGcolor()
            { r = 0.8f, g = 0.1f, b = 0.1f, a = 1f }
                                                                                                                : ThemeBase.Theme.WindowDialoguesChatBoxIconsColor);//ThemeOld.LightThemeOld.SearchBoxGradientIconSearchFocusedColor);
            NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_CENTER | NVGalign.NVG_ALIGN_MIDDLE));
            NanoVG.nvgText(vg, x + w - h * 0.55f, y + h * 0.55f, NanoFont.UnicodeToUTF8((int)Entypo.ICON_CIRCLED_CROSS));
            #endregion


        }
        private void drawCarret(NVGcontext ctx)
        {
            NanoVG.nvgBeginPath(ctx);


            float posX = Text.Length == 0 ? this.Size.Height * 1.05f : this.Size.Height * 1.05f + MeasureTextWidth(Text.Substring(0, charPos), Font);

            NanoVG.nvgRect(ctx, this.Location.X + posX + 1f, this.Location.Y + 2, 1, Size.Height - 2);
            NanoVG.nvgClosePath(ctx);

            NanoVG.nvgFillColor(ctx, NanoVG.nvgRGBA(255, 192, 0, 255));
            NanoVG.nvgFill(ctx);
        }

        #endregion Draw
    }
}
