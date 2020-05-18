
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NthStudio.WinForms.Adobe
{
    /// <summary>
    /// Summary description for frmColorPicker.
    /// </summary>
    public class ColorPicker : System.Windows.Forms.Control
    {
        public Button ButtonOK {  get { return m_cmd_OK; } }
        public Button ButtonCancel {  get { return m_cmd_Cancel; } }

        public class ColorChangedEventArgs
        {
            private Color color;
            public Color Color {  get { return color; } }

            public ColorChangedEventArgs(Color newColor)
            {
                color = newColor;
            }
        }
        public delegate void ColorChangedDelegate(ColorChangedEventArgs e);

        public event ColorChangedDelegate OnColorChanged; 

        #region Class Variables

        private AdobeColors.HSL m_hsl;
        private Color m_rgb;
        private AdobeColors.CMYK m_cmyk;

        public enum eDrawStyle
        {
            Hue,
            Saturation,
            Brightness,
            Red,
            Green,
            Blue
        }


        #endregion

        #region Designer Generated Variables

        private System.Windows.Forms.Label m_lbl_SelectColor;
        private System.Windows.Forms.PictureBox m_pbx_BlankBox;
        private System.Windows.Forms.Button m_cmd_OK;
        private System.Windows.Forms.Button m_cmd_Cancel;
        private System.Windows.Forms.TextBox m_txt_Hue;
        private System.Windows.Forms.TextBox m_txt_Sat;
        private System.Windows.Forms.TextBox m_txt_Black;
        private System.Windows.Forms.TextBox m_txt_Red;
        private System.Windows.Forms.TextBox m_txt_Green;
        private System.Windows.Forms.TextBox m_txt_Blue;
        private System.Windows.Forms.TextBox m_txt_Lum;
        private System.Windows.Forms.TextBox m_txt_a;
        private System.Windows.Forms.TextBox m_txt_b;
        private System.Windows.Forms.TextBox m_txt_Cyan;
        private System.Windows.Forms.TextBox m_txt_Magenta;
        private System.Windows.Forms.TextBox m_txt_Yellow;
        private System.Windows.Forms.TextBox m_txt_K;
        private System.Windows.Forms.TextBox m_txt_Hex;
        private System.Windows.Forms.RadioButton m_rbtn_Hue;
        private System.Windows.Forms.RadioButton m_rbtn_Sat;
        private System.Windows.Forms.RadioButton m_rbtn_Black;
        private System.Windows.Forms.RadioButton m_rbtn_Red;
        private System.Windows.Forms.RadioButton m_rbtn_Green;
        private System.Windows.Forms.RadioButton m_rbtn_Blue;
        private System.Windows.Forms.CheckBox m_cbx_WebColorsOnly;
        private System.Windows.Forms.Label m_lbl_HexPound;
        private System.Windows.Forms.RadioButton m_rbtn_L;
        private System.Windows.Forms.RadioButton m_rbtn_a;
        private System.Windows.Forms.RadioButton m_rbtn_b;
        private System.Windows.Forms.Label m_lbl_Cyan;
        private System.Windows.Forms.Label m_lbl_Magenta;
        private System.Windows.Forms.Label m_lbl_Yellow;
        private System.Windows.Forms.Label m_lbl_K;
        private System.Windows.Forms.Label m_lbl_Primary_Color;
        private System.Windows.Forms.Label m_lbl_Secondary_Color;
        private ctrlVerticalColorSlider m_ctrl_ThinBox;
        private ctrl2DColorBox m_ctrl_BigBox;
        private System.Windows.Forms.Label m_lbl_Hue_Symbol;
        private System.Windows.Forms.Label m_lbl_Saturation_Symbol;
        private System.Windows.Forms.Label m_lbl_Black_Symbol;
        private System.Windows.Forms.Label m_lbl_Cyan_Symbol;
        private System.Windows.Forms.Label m_lbl_Magenta_Symbol;
        private System.Windows.Forms.Label m_lbl_Yellow_Symbol;
        private System.Windows.Forms.Label m_lbl_Key_Symbol;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        #region Constructors / Destructors

        public ColorPicker()
            : this(Color.White) {

            m_txt_Red.TextChanged += delegate { changeColorDispatchEvent(); };
            m_txt_Green.TextChanged += delegate { changeColorDispatchEvent(); };
            m_txt_Blue.TextChanged += delegate { changeColorDispatchEvent(); };



        }
        private void changeColorDispatchEvent()
        {
            if (null != OnColorChanged)
                this.OnColorChanged(new ColorChangedEventArgs(Color.FromArgb(255,
                                                                             Int32.Parse(m_txt_Red.Text),
                                                                             Int32.Parse(m_txt_Green.Text),
                                                                             Int32.Parse(m_txt_Blue.Text))));
            
        }
        public ColorPicker(Color starting_color)
        {
            InitializeComponent();

            m_rgb = starting_color;
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Lum.Update();
            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();

            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;

            m_lbl_Primary_Color.BackColor = starting_color;
            m_lbl_Secondary_Color.BackColor = starting_color;

            m_rbtn_Hue.Checked = true;

            this.WriteHexData(m_rgb);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_lbl_SelectColor = new System.Windows.Forms.Label();
            this.m_pbx_BlankBox = new System.Windows.Forms.PictureBox();
            this.m_cmd_OK = new System.Windows.Forms.Button();
            this.m_cmd_Cancel = new System.Windows.Forms.Button();
            this.m_txt_Hue = new System.Windows.Forms.TextBox();
            this.m_txt_Sat = new System.Windows.Forms.TextBox();
            this.m_txt_Black = new System.Windows.Forms.TextBox();
            this.m_txt_Red = new System.Windows.Forms.TextBox();
            this.m_txt_Green = new System.Windows.Forms.TextBox();
            this.m_txt_Blue = new System.Windows.Forms.TextBox();
            this.m_txt_Lum = new System.Windows.Forms.TextBox();
            this.m_txt_a = new System.Windows.Forms.TextBox();
            this.m_txt_b = new System.Windows.Forms.TextBox();
            this.m_txt_Cyan = new System.Windows.Forms.TextBox();
            this.m_txt_Magenta = new System.Windows.Forms.TextBox();
            this.m_txt_Yellow = new System.Windows.Forms.TextBox();
            this.m_txt_K = new System.Windows.Forms.TextBox();
            this.m_txt_Hex = new System.Windows.Forms.TextBox();
            this.m_rbtn_Hue = new System.Windows.Forms.RadioButton();
            this.m_rbtn_Sat = new System.Windows.Forms.RadioButton();
            this.m_rbtn_Black = new System.Windows.Forms.RadioButton();
            this.m_rbtn_Red = new System.Windows.Forms.RadioButton();
            this.m_rbtn_Green = new System.Windows.Forms.RadioButton();
            this.m_rbtn_Blue = new System.Windows.Forms.RadioButton();
            this.m_cbx_WebColorsOnly = new System.Windows.Forms.CheckBox();
            this.m_lbl_HexPound = new System.Windows.Forms.Label();
            this.m_rbtn_L = new System.Windows.Forms.RadioButton();
            this.m_rbtn_a = new System.Windows.Forms.RadioButton();
            this.m_rbtn_b = new System.Windows.Forms.RadioButton();
            this.m_lbl_Cyan = new System.Windows.Forms.Label();
            this.m_lbl_Magenta = new System.Windows.Forms.Label();
            this.m_lbl_Yellow = new System.Windows.Forms.Label();
            this.m_lbl_K = new System.Windows.Forms.Label();
            this.m_lbl_Primary_Color = new System.Windows.Forms.Label();
            this.m_lbl_Secondary_Color = new System.Windows.Forms.Label();
            this.m_ctrl_ThinBox = new ctrlVerticalColorSlider();
            this.m_ctrl_BigBox = new ctrl2DColorBox();
            this.m_lbl_Hue_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Saturation_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Black_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Cyan_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Magenta_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Yellow_Symbol = new System.Windows.Forms.Label();
            this.m_lbl_Key_Symbol = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lbl_SelectColor
            // 
            this.m_lbl_SelectColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_SelectColor.Location = new System.Drawing.Point(10, 10);
            this.m_lbl_SelectColor.Name = "m_lbl_SelectColor";
            this.m_lbl_SelectColor.Size = new System.Drawing.Size(260, 20);
            this.m_lbl_SelectColor.TabIndex = 0;
            this.m_lbl_SelectColor.Text = "Select Color:";
            // 
            // m_pbx_BlankBox
            // 
            this.m_pbx_BlankBox.BackColor = System.Drawing.Color.Black;
            this.m_pbx_BlankBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_pbx_BlankBox.Location = new System.Drawing.Point(316, 30);
            this.m_pbx_BlankBox.Name = "m_pbx_BlankBox";
            this.m_pbx_BlankBox.Size = new System.Drawing.Size(62, 70);
            this.m_pbx_BlankBox.TabIndex = 3;
            this.m_pbx_BlankBox.TabStop = false;
            // 
            // m_cmd_OK
            // 
            this.m_cmd_OK.Location = new System.Drawing.Point(412, 11);
            this.m_cmd_OK.Name = "m_cmd_OK";
            this.m_cmd_OK.Size = new System.Drawing.Size(72, 19);
            this.m_cmd_OK.TabIndex = 4;
            this.m_cmd_OK.Text = "Ok";
            //this.m_cmd_OK.Click += new System.EventHandler(this.m_cmd_OK_Click);
            // 
            // m_cmd_Cancel
            // 
            this.m_cmd_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cmd_Cancel.Location = new System.Drawing.Point(412, 39);
            this.m_cmd_Cancel.Name = "m_cmd_Cancel";
            this.m_cmd_Cancel.Size = new System.Drawing.Size(72, 19);
            this.m_cmd_Cancel.TabIndex = 5;
            this.m_cmd_Cancel.Text = "Cancel";
            //this.m_cmd_Cancel.Click += new System.EventHandler(this.m_cmd_Cancel_Click);
            // 
            // m_txt_Hue
            // 
            this.m_txt_Hue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Hue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Hue.Location = new System.Drawing.Point(351, 115);
            this.m_txt_Hue.Name = "m_txt_Hue";
            this.m_txt_Hue.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Hue.TabIndex = 6;
            this.m_txt_Hue.Text = "";
            this.m_txt_Hue.Leave += new System.EventHandler(this.m_txt_Hue_Leave);
            // 
            // m_txt_Sat
            // 
            this.m_txt_Sat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Sat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Sat.Location = new System.Drawing.Point(351, 140);
            this.m_txt_Sat.Name = "m_txt_Sat";
            this.m_txt_Sat.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Sat.TabIndex = 7;
            this.m_txt_Sat.Text = "";
            this.m_txt_Sat.Leave += new System.EventHandler(this.m_txt_Sat_Leave);
            // 
            // m_txt_Black
            // 
            this.m_txt_Black.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Black.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Black.Location = new System.Drawing.Point(351, 165);
            this.m_txt_Black.Name = "m_txt_Black";
            this.m_txt_Black.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Black.TabIndex = 8;
            this.m_txt_Black.Text = "";
            this.m_txt_Black.Leave += new System.EventHandler(this.m_txt_Black_Leave);
            // 
            // m_txt_Red
            // 
            this.m_txt_Red.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Red.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Red.Location = new System.Drawing.Point(351, 195);
            this.m_txt_Red.Name = "m_txt_Red";
            this.m_txt_Red.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Red.TabIndex = 9;
            this.m_txt_Red.Text = "";
            this.m_txt_Red.Leave += new System.EventHandler(this.m_txt_Red_Leave);
            // 
            // m_txt_Green
            // 
            this.m_txt_Green.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Green.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Green.Location = new System.Drawing.Point(351, 220);
            this.m_txt_Green.Name = "m_txt_Green";
            this.m_txt_Green.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Green.TabIndex = 10;
            this.m_txt_Green.Text = "";
            this.m_txt_Green.Leave += new System.EventHandler(this.m_txt_Green_Leave);
            // 
            // m_txt_Blue
            // 
            this.m_txt_Blue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Blue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Blue.Location = new System.Drawing.Point(351, 245);
            this.m_txt_Blue.Name = "m_txt_Blue";
            this.m_txt_Blue.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Blue.TabIndex = 11;
            this.m_txt_Blue.Text = "";
            this.m_txt_Blue.Leave += new System.EventHandler(this.m_txt_Blue_Leave);
            // 
            // m_txt_Lum
            // 
            this.m_txt_Lum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Lum.Enabled = false;
            this.m_txt_Lum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Lum.Location = new System.Drawing.Point(445, 115);
            this.m_txt_Lum.Name = "m_txt_Lum";
            this.m_txt_Lum.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Lum.TabIndex = 12;
            this.m_txt_Lum.Text = "";
            // 
            // m_txt_a
            // 
            this.m_txt_a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_a.Enabled = false;
            this.m_txt_a.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_a.Location = new System.Drawing.Point(445, 140);
            this.m_txt_a.Name = "m_txt_a";
            this.m_txt_a.Size = new System.Drawing.Size(35, 21);
            this.m_txt_a.TabIndex = 13;
            this.m_txt_a.Text = "";
            // 
            // m_txt_b
            // 
            this.m_txt_b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_b.Enabled = false;
            this.m_txt_b.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_b.Location = new System.Drawing.Point(445, 165);
            this.m_txt_b.Name = "m_txt_b";
            this.m_txt_b.Size = new System.Drawing.Size(35, 21);
            this.m_txt_b.TabIndex = 14;
            this.m_txt_b.Text = "";
            // 
            // m_txt_Cyan
            // 
            this.m_txt_Cyan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Cyan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Cyan.Location = new System.Drawing.Point(445, 195);
            this.m_txt_Cyan.Name = "m_txt_Cyan";
            this.m_txt_Cyan.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Cyan.TabIndex = 15;
            this.m_txt_Cyan.Text = "";
            this.m_txt_Cyan.Leave += new System.EventHandler(this.m_txt_Cyan_Leave);
            // 
            // m_txt_Magenta
            // 
            this.m_txt_Magenta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Magenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Magenta.Location = new System.Drawing.Point(445, 220);
            this.m_txt_Magenta.Name = "m_txt_Magenta";
            this.m_txt_Magenta.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Magenta.TabIndex = 16;
            this.m_txt_Magenta.Text = "";
            this.m_txt_Magenta.Leave += new System.EventHandler(this.m_txt_Magenta_Leave);
            // 
            // m_txt_Yellow
            // 
            this.m_txt_Yellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Yellow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Yellow.Location = new System.Drawing.Point(445, 245);
            this.m_txt_Yellow.Name = "m_txt_Yellow";
            this.m_txt_Yellow.Size = new System.Drawing.Size(35, 21);
            this.m_txt_Yellow.TabIndex = 17;
            this.m_txt_Yellow.Text = "";
            this.m_txt_Yellow.Leave += new System.EventHandler(this.m_txt_Yellow_Leave);
            // 
            // m_txt_K
            // 
            this.m_txt_K.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_K.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_K.Location = new System.Drawing.Point(445, 270);
            this.m_txt_K.Name = "m_txt_K";
            this.m_txt_K.Size = new System.Drawing.Size(35, 21);
            this.m_txt_K.TabIndex = 18;
            this.m_txt_K.Text = "";
            this.m_txt_K.Leave += new System.EventHandler(this.m_txt_K_Leave);
            // 
            // m_txt_Hex
            // 
            this.m_txt_Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txt_Hex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_txt_Hex.Location = new System.Drawing.Point(334, 278);
            this.m_txt_Hex.Name = "m_txt_Hex";
            this.m_txt_Hex.Size = new System.Drawing.Size(56, 21);
            this.m_txt_Hex.TabIndex = 19;
            this.m_txt_Hex.Text = "";
            this.m_txt_Hex.Leave += new System.EventHandler(this.m_txt_Hex_Leave);
            // 
            // m_rbtn_Hue
            // 
            this.m_rbtn_Hue.Location = new System.Drawing.Point(314, 115);
            this.m_rbtn_Hue.Name = "m_rbtn_Hue";
            this.m_rbtn_Hue.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Hue.TabIndex = 20;
            this.m_rbtn_Hue.Text = "H:";
            this.m_rbtn_Hue.CheckedChanged += new System.EventHandler(this.m_rbtn_Hue_CheckedChanged);
            // 
            // m_rbtn_Sat
            // 
            this.m_rbtn_Sat.Location = new System.Drawing.Point(314, 140);
            this.m_rbtn_Sat.Name = "m_rbtn_Sat";
            this.m_rbtn_Sat.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Sat.TabIndex = 21;
            this.m_rbtn_Sat.Text = "S:";
            this.m_rbtn_Sat.CheckedChanged += new System.EventHandler(this.m_rbtn_Sat_CheckedChanged);
            // 
            // m_rbtn_Black
            // 
            this.m_rbtn_Black.Location = new System.Drawing.Point(314, 165);
            this.m_rbtn_Black.Name = "m_rbtn_Black";
            this.m_rbtn_Black.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Black.TabIndex = 22;
            this.m_rbtn_Black.Text = "B:";
            this.m_rbtn_Black.CheckedChanged += new System.EventHandler(this.m_rbtn_Black_CheckedChanged);
            // 
            // m_rbtn_Red
            // 
            this.m_rbtn_Red.Location = new System.Drawing.Point(314, 195);
            this.m_rbtn_Red.Name = "m_rbtn_Red";
            this.m_rbtn_Red.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Red.TabIndex = 23;
            this.m_rbtn_Red.Text = "R:";
            this.m_rbtn_Red.CheckedChanged += new System.EventHandler(this.m_rbtn_Red_CheckedChanged);
            // 
            // m_rbtn_Green
            // 
            this.m_rbtn_Green.Location = new System.Drawing.Point(314, 220);
            this.m_rbtn_Green.Name = "m_rbtn_Green";
            this.m_rbtn_Green.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Green.TabIndex = 24;
            this.m_rbtn_Green.Text = "G:";
            this.m_rbtn_Green.CheckedChanged += new System.EventHandler(this.m_rbtn_Green_CheckedChanged);
            // 
            // m_rbtn_Blue
            // 
            this.m_rbtn_Blue.Location = new System.Drawing.Point(314, 245);
            this.m_rbtn_Blue.Name = "m_rbtn_Blue";
            this.m_rbtn_Blue.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_Blue.TabIndex = 25;
            this.m_rbtn_Blue.Text = "B:";
            this.m_rbtn_Blue.CheckedChanged += new System.EventHandler(this.m_rbtn_Blue_CheckedChanged);
            // 
            // m_cbx_WebColorsOnly
            // 
            this.m_cbx_WebColorsOnly.Enabled = false;
            this.m_cbx_WebColorsOnly.Location = new System.Drawing.Point(10, 296);
            this.m_cbx_WebColorsOnly.Name = "m_cbx_WebColorsOnly";
            this.m_cbx_WebColorsOnly.Size = new System.Drawing.Size(248, 24);
            this.m_cbx_WebColorsOnly.TabIndex = 26;
            this.m_cbx_WebColorsOnly.Text = "Only Web Colors (Not fixed yet)";
            // 
            // m_lbl_HexPound
            // 
            this.m_lbl_HexPound.Location = new System.Drawing.Point(318, 282);
            this.m_lbl_HexPound.Name = "m_lbl_HexPound";
            this.m_lbl_HexPound.Size = new System.Drawing.Size(16, 14);
            this.m_lbl_HexPound.TabIndex = 27;
            this.m_lbl_HexPound.Text = "#";
            // 
            // m_rbtn_L
            // 
            this.m_rbtn_L.Enabled = false;
            this.m_rbtn_L.Location = new System.Drawing.Point(408, 115);
            this.m_rbtn_L.Name = "m_rbtn_L";
            this.m_rbtn_L.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_L.TabIndex = 28;
            this.m_rbtn_L.Text = "L:";
            // 
            // m_rbtn_a
            // 
            this.m_rbtn_a.Enabled = false;
            this.m_rbtn_a.Location = new System.Drawing.Point(408, 140);
            this.m_rbtn_a.Name = "m_rbtn_a";
            this.m_rbtn_a.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_a.TabIndex = 29;
            this.m_rbtn_a.Text = "a:";
            // 
            // m_rbtn_b
            // 
            this.m_rbtn_b.Enabled = false;
            this.m_rbtn_b.Location = new System.Drawing.Point(408, 165);
            this.m_rbtn_b.Name = "m_rbtn_b";
            this.m_rbtn_b.Size = new System.Drawing.Size(35, 24);
            this.m_rbtn_b.TabIndex = 30;
            this.m_rbtn_b.Text = "b:";
            // 
            // m_lbl_Cyan
            // 
            this.m_lbl_Cyan.Location = new System.Drawing.Point(428, 200);
            this.m_lbl_Cyan.Name = "m_lbl_Cyan";
            this.m_lbl_Cyan.Size = new System.Drawing.Size(16, 16);
            this.m_lbl_Cyan.TabIndex = 31;
            this.m_lbl_Cyan.Text = "C:";
            // 
            // m_lbl_Magenta
            // 
            this.m_lbl_Magenta.Location = new System.Drawing.Point(428, 224);
            this.m_lbl_Magenta.Name = "m_lbl_Magenta";
            this.m_lbl_Magenta.Size = new System.Drawing.Size(16, 16);
            this.m_lbl_Magenta.TabIndex = 32;
            this.m_lbl_Magenta.Text = "M:";
            // 
            // m_lbl_Yellow
            // 
            this.m_lbl_Yellow.Location = new System.Drawing.Point(428, 248);
            this.m_lbl_Yellow.Name = "m_lbl_Yellow";
            this.m_lbl_Yellow.Size = new System.Drawing.Size(16, 16);
            this.m_lbl_Yellow.TabIndex = 33;
            this.m_lbl_Yellow.Text = "Y:";
            // 
            // m_lbl_K
            // 
            this.m_lbl_K.Location = new System.Drawing.Point(428, 272);
            this.m_lbl_K.Name = "m_lbl_K";
            this.m_lbl_K.Size = new System.Drawing.Size(16, 16);
            this.m_lbl_K.TabIndex = 34;
            this.m_lbl_K.Text = "K:";
            // 
            // m_lbl_Primary_Color
            // 
            this.m_lbl_Primary_Color.Location = new System.Drawing.Point(317, 31);
            this.m_lbl_Primary_Color.Name = "m_lbl_Primary_Color";
            this.m_lbl_Primary_Color.Size = new System.Drawing.Size(60, 34);
            this.m_lbl_Primary_Color.TabIndex = 36;
            this.m_lbl_Primary_Color.Click += new System.EventHandler(this.m_lbl_Primary_Color_Click);
            // 
            // m_lbl_Secondary_Color
            // 
            this.m_lbl_Secondary_Color.Location = new System.Drawing.Point(317, 65);
            this.m_lbl_Secondary_Color.Name = "m_lbl_Secondary_Color";
            this.m_lbl_Secondary_Color.Size = new System.Drawing.Size(60, 34);
            this.m_lbl_Secondary_Color.TabIndex = 37;
            this.m_lbl_Secondary_Color.Click += new System.EventHandler(this.m_lbl_Secondary_Color_Click);
            // 
            // m_ctrl_ThinBox
            // 
            this.m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Hue;
            this.m_ctrl_ThinBox.Location = new System.Drawing.Point(271, 28);
            this.m_ctrl_ThinBox.Name = "m_ctrl_ThinBox";
            this.m_ctrl_ThinBox.RGB = System.Drawing.Color.Red;
            this.m_ctrl_ThinBox.Size = new System.Drawing.Size(40, 264);
            this.m_ctrl_ThinBox.TabIndex = 38;
            this.m_ctrl_ThinBox.Scroll += new EventHandler(this.m_ctrl_ThinBox_Scroll);
            // 
            // m_ctrl_BigBox
            // 
            this.m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Hue;
            this.m_ctrl_BigBox.Location = new System.Drawing.Point(10, 30);
            this.m_ctrl_BigBox.Name = "m_ctrl_BigBox";
            this.m_ctrl_BigBox.RGB = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(0)), ((System.Byte)(0)));
            this.m_ctrl_BigBox.Size = new System.Drawing.Size(260, 260);
            this.m_ctrl_BigBox.TabIndex = 39;
            this.m_ctrl_BigBox.Scroll += new EventHandler(this.m_ctrl_BigBox_Scroll);
            // 
            // m_lbl_Hue_Symbol
            // 
            this.m_lbl_Hue_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Hue_Symbol.Location = new System.Drawing.Point(387, 115);
            this.m_lbl_Hue_Symbol.Name = "m_lbl_Hue_Symbol";
            this.m_lbl_Hue_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Hue_Symbol.TabIndex = 40;
            this.m_lbl_Hue_Symbol.Text = "°";
            // 
            // m_lbl_Saturation_Symbol
            // 
            this.m_lbl_Saturation_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Saturation_Symbol.Location = new System.Drawing.Point(387, 140);
            this.m_lbl_Saturation_Symbol.Name = "m_lbl_Saturation_Symbol";
            this.m_lbl_Saturation_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Saturation_Symbol.TabIndex = 41;
            this.m_lbl_Saturation_Symbol.Text = "%";
            // 
            // m_lbl_Black_Symbol
            // 
            this.m_lbl_Black_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Black_Symbol.Location = new System.Drawing.Point(387, 165);
            this.m_lbl_Black_Symbol.Name = "m_lbl_Black_Symbol";
            this.m_lbl_Black_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Black_Symbol.TabIndex = 42;
            this.m_lbl_Black_Symbol.Text = "%";
            // 
            // m_lbl_Cyan_Symbol
            // 
            this.m_lbl_Cyan_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Cyan_Symbol.Location = new System.Drawing.Point(481, 195);
            this.m_lbl_Cyan_Symbol.Name = "m_lbl_Cyan_Symbol";
            this.m_lbl_Cyan_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Cyan_Symbol.TabIndex = 43;
            this.m_lbl_Cyan_Symbol.Text = "%";
            // 
            // m_lbl_Magenta_Symbol
            // 
            this.m_lbl_Magenta_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Magenta_Symbol.Location = new System.Drawing.Point(481, 220);
            this.m_lbl_Magenta_Symbol.Name = "m_lbl_Magenta_Symbol";
            this.m_lbl_Magenta_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Magenta_Symbol.TabIndex = 44;
            this.m_lbl_Magenta_Symbol.Text = "%";
            // 
            // m_lbl_Yellow_Symbol
            // 
            this.m_lbl_Yellow_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_lbl_Yellow_Symbol.Location = new System.Drawing.Point(481, 245);
            this.m_lbl_Yellow_Symbol.Name = "m_lbl_Yellow_Symbol";
            this.m_lbl_Yellow_Symbol.Size = new System.Drawing.Size(16, 21);
            this.m_lbl_Yellow_Symbol.TabIndex = 45;
            this.m_lbl_Yellow_Symbol.Text = "%";
            // 
            // m_lbl_Key_Symbol
            // 
            this.m_lbl_Key_Symbol.Location = new System.Drawing.Point(481, 270);
            this.m_lbl_Key_Symbol.Name = "m_lbl_Key_Symbol";
            this.m_lbl_Key_Symbol.TabIndex = 0;
            // 
            // frmColorPicker
            // 
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(504, 321);
            this.Controls.Add(this.m_lbl_Key_Symbol);
            this.Controls.Add(this.m_lbl_Yellow_Symbol);
            this.Controls.Add(this.m_lbl_Magenta_Symbol);
            this.Controls.Add(this.m_lbl_Cyan_Symbol);
            this.Controls.Add(this.m_lbl_Black_Symbol);
            this.Controls.Add(this.m_lbl_Saturation_Symbol);
            this.Controls.Add(this.m_lbl_Hue_Symbol);
            this.Controls.Add(this.m_ctrl_BigBox);
            this.Controls.Add(this.m_ctrl_ThinBox);
            this.Controls.Add(this.m_lbl_Secondary_Color);
            this.Controls.Add(this.m_lbl_Primary_Color);
            this.Controls.Add(this.m_lbl_K);
            this.Controls.Add(this.m_lbl_Yellow);
            this.Controls.Add(this.m_lbl_Magenta);
            this.Controls.Add(this.m_lbl_Cyan);
            this.Controls.Add(this.m_rbtn_b);
            this.Controls.Add(this.m_rbtn_a);
            this.Controls.Add(this.m_rbtn_L);
            this.Controls.Add(this.m_lbl_HexPound);
            this.Controls.Add(this.m_cbx_WebColorsOnly);
            this.Controls.Add(this.m_rbtn_Blue);
            this.Controls.Add(this.m_rbtn_Green);
            this.Controls.Add(this.m_rbtn_Red);
            this.Controls.Add(this.m_rbtn_Black);
            this.Controls.Add(this.m_rbtn_Sat);
            this.Controls.Add(this.m_rbtn_Hue);
            this.Controls.Add(this.m_txt_Hex);
            this.Controls.Add(this.m_txt_K);
            this.Controls.Add(this.m_txt_Yellow);
            this.Controls.Add(this.m_txt_Magenta);
            this.Controls.Add(this.m_txt_Cyan);
            this.Controls.Add(this.m_txt_b);
            this.Controls.Add(this.m_txt_a);
            this.Controls.Add(this.m_txt_Lum);
            this.Controls.Add(this.m_txt_Blue);
            this.Controls.Add(this.m_txt_Green);
            this.Controls.Add(this.m_txt_Red);
            this.Controls.Add(this.m_txt_Black);
            this.Controls.Add(this.m_txt_Sat);
            this.Controls.Add(this.m_txt_Hue);
            this.Controls.Add(this.m_cmd_Cancel);
            this.Controls.Add(this.m_cmd_OK);
            this.Controls.Add(this.m_pbx_BlankBox);
            this.Controls.Add(this.m_lbl_SelectColor);
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;
            this.Name = "frmColorPicker";
            //this.ShowInTaskbar = false;
            //this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            //this.Text = "Color Picker";
            //this.onfoLoad += new System.EventHandler(this.frmColorPicker_Load);
            this.ResumeLayout(false);

        }


        #endregion

        #region Events

        #region General Events

        private void frmColorPicker_Load(object sender, System.EventArgs e)
        {

        }


        //private void m_cmd_OK_Click(object sender, System.EventArgs e)
        //{
        //    this.DialogResult = DialogResult.OK;
        //    this.Close();
        //}


        //private void m_cmd_Cancel_Click(object sender, System.EventArgs e)
        //{
        //    this.DialogResult = DialogResult.Cancel;
        //    this.Close();
        //}


        #endregion

        #region Primary Picture Box (m_ctrl_BigBox)

        private void m_ctrl_BigBox_Scroll(object sender, System.EventArgs e)
        {
            m_hsl = m_ctrl_BigBox.HSL;
            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Black.Update();
            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();

            m_ctrl_ThinBox.HSL = m_hsl;

            m_lbl_Primary_Color.BackColor = m_rgb;
            m_lbl_Primary_Color.Update();

            WriteHexData(m_rgb);
        }


        #endregion

        #region Secondary Picture Box (m_ctrl_ThinBox)

        private void m_ctrl_ThinBox_Scroll(object sender, System.EventArgs e)
        {
            m_hsl = m_ctrl_ThinBox.HSL;
            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Black.Update();
            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();

            m_ctrl_BigBox.HSL = m_hsl;

            m_lbl_Primary_Color.BackColor = m_rgb;
            m_lbl_Primary_Color.Update();

            WriteHexData(m_rgb);
        }


        #endregion

        #region Hex Box (m_txt_Hex)

        private void m_txt_Hex_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Hex.Text.ToUpper();
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            foreach (char letter in text)
            {
                if (!char.IsNumber(letter))
                {
                    if (letter >= 'A' && letter <= 'F')
                        continue;
                    has_illegal_chars = true;
                    break;
                }
            }

            if (has_illegal_chars)
            {
                MessageBox.Show("Hex must be a hex value between 0x000000 and 0xFFFFFF");
                WriteHexData(m_rgb);
                return;
            }

            m_rgb = ParseHexData(text);
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        #endregion

        #region Color Boxes

        private void m_lbl_Primary_Color_Click(object sender, System.EventArgs e)
        {
            m_rgb = m_lbl_Primary_Color.BackColor;
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;

            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Lum.Update();
            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();
        }


        private void m_lbl_Secondary_Color_Click(object sender, System.EventArgs e)
        {
            m_rgb = m_lbl_Secondary_Color.BackColor;
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;

            m_lbl_Primary_Color.BackColor = m_rgb;
            m_lbl_Primary_Color.Update();

            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Lum.Update();
            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();
        }


        #endregion

        #region Radio Buttons

        private void m_rbtn_Hue_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Hue.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Hue;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Hue;
            }
        }


        private void m_rbtn_Sat_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Sat.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Saturation;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Saturation;
            }
        }


        private void m_rbtn_Black_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Black.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Brightness;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Brightness;
            }
        }


        private void m_rbtn_Red_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Red.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Red;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Red;
            }
        }


        private void m_rbtn_Green_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Green.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Green;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Green;
            }
        }


        private void m_rbtn_Blue_CheckedChanged(object sender, System.EventArgs e)
        {
            if (m_rbtn_Blue.Checked)
            {
                m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Blue;
                m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Blue;
            }
        }


        #endregion

        #region Text Boxes

        private void m_txt_Hue_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Hue.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Hue must be a number value between 0 and 360");
                UpdateTextBoxes();
                return;
            }

            int hue = int.Parse(text);

            if (hue < 0)
            {
                MessageBox.Show("An integer between 0 and 360 is required.\nClosest value inserted.");
                m_txt_Hue.Text = "0";
                m_hsl.H = 0.0;
            }
            else if (hue > 360)
            {
                MessageBox.Show("An integer between 0 and 360 is required.\nClosest value inserted.");
                m_txt_Hue.Text = "360";
                m_hsl.H = 1.0;
            }
            else
            {
                m_hsl.H = (double)hue / 360;
            }

            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Sat_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Sat.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Saturation must be a number value between 0 and 100");
                UpdateTextBoxes();
                return;
            }

            int sat = int.Parse(text);

            if (sat < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Sat.Text = "0";
                m_hsl.S = 0.0;
            }
            else if (sat > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Sat.Text = "100";
                m_hsl.S = 1.0;
            }
            else
            {
                m_hsl.S = (double)sat / 100;
            }

            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Black_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Black.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Black must be a number value between 0 and 360");
                UpdateTextBoxes();
                return;
            }

            int lum = int.Parse(text);

            if (lum < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Black.Text = "0";
                m_hsl.L = 0.0;
            }
            else if (lum > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Black.Text = "100";
                m_hsl.L = 1.0;
            }
            else
            {
                m_hsl.L = (double)lum / 100;
            }

            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Red_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Red.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Red must be a number value between 0 and 255");
                UpdateTextBoxes();
                return;
            }

            int red = int.Parse(text);

            if (red < 0)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Sat.Text = "0";
                m_rgb = Color.FromArgb(0, m_rgb.G, m_rgb.B);
            }
            else if (red > 255)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Sat.Text = "255";
                m_rgb = Color.FromArgb(255, m_rgb.G, m_rgb.B);
            }
            else
            {
                m_rgb = Color.FromArgb(red, m_rgb.G, m_rgb.B);
            }

            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Green_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Green.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Green must be a number value between 0 and 255");
                UpdateTextBoxes();
                return;
            }

            int green = int.Parse(text);

            if (green < 0)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Green.Text = "0";
                m_rgb = Color.FromArgb(m_rgb.R, 0, m_rgb.B);
            }
            else if (green > 255)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Green.Text = "255";
                m_rgb = Color.FromArgb(m_rgb.R, 255, m_rgb.B);
            }
            else
            {
                m_rgb = Color.FromArgb(m_rgb.R, green, m_rgb.B);
            }

            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Blue_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Blue.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Blue must be a number value between 0 and 255");
                UpdateTextBoxes();
                return;
            }

            int blue = int.Parse(text);

            if (blue < 0)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Blue.Text = "0";
                m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 0);
            }
            else if (blue > 255)
            {
                MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                m_txt_Blue.Text = "255";
                m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 255);
            }
            else
            {
                m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, blue);
            }

            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Cyan_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Cyan.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Cyan must be a number value between 0 and 100");
                UpdateTextBoxes();
                return;
            }

            int cyan = int.Parse(text);

            if (cyan < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_cmyk.C = 0.0;
            }
            else if (cyan > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_cmyk.C = 1.0;
            }
            else
            {
                m_cmyk.C = (double)cyan / 100;
            }

            m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Magenta_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Magenta.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Magenta must be a number value between 0 and 100");
                UpdateTextBoxes();
                return;
            }

            int magenta = int.Parse(text);

            if (magenta < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Magenta.Text = "0";
                m_cmyk.M = 0.0;
            }
            else if (magenta > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Magenta.Text = "100";
                m_cmyk.M = 1.0;
            }
            else
            {
                m_cmyk.M = (double)magenta / 100;
            }

            m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_Yellow_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_Yellow.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Yellow must be a number value between 0 and 100");
                UpdateTextBoxes();
                return;
            }

            int yellow = int.Parse(text);

            if (yellow < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Yellow.Text = "0";
                m_cmyk.Y = 0.0;
            }
            else if (yellow > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_Yellow.Text = "100";
                m_cmyk.Y = 1.0;
            }
            else
            {
                m_cmyk.Y = (double)yellow / 100;
            }

            m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        private void m_txt_K_Leave(object sender, System.EventArgs e)
        {
            string text = m_txt_K.Text;
            bool has_illegal_chars = false;

            if (text.Length <= 0)
                has_illegal_chars = true;
            else
                foreach (char letter in text)
                {
                    if (!char.IsNumber(letter))
                    {
                        has_illegal_chars = true;
                        break;
                    }
                }

            if (has_illegal_chars)
            {
                MessageBox.Show("Key must be a number value between 0 and 100");
                UpdateTextBoxes();
                return;
            }

            int key = int.Parse(text);

            if (key < 0)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_K.Text = "0";
                m_cmyk.K = 0.0;
            }
            else if (key > 100)
            {
                MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                m_txt_K.Text = "100";
                m_cmyk.K = 1.0;
            }
            else
            {
                m_cmyk.K = (double)key / 100;
            }

            m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
            m_ctrl_BigBox.HSL = m_hsl;
            m_ctrl_ThinBox.HSL = m_hsl;
            m_lbl_Primary_Color.BackColor = m_rgb;

            UpdateTextBoxes();
        }


        #endregion

        #endregion

        #region Private Functions

        private int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                ret_val += 1;

            return ret_val;
        }


        private void WriteHexData(Color rgb)
        {
            string red = Convert.ToString(rgb.R, 16);
            if (red.Length < 2) red = "0" + red;
            string green = Convert.ToString(rgb.G, 16);
            if (green.Length < 2) green = "0" + green;
            string blue = Convert.ToString(rgb.B, 16);
            if (blue.Length < 2) blue = "0" + blue;

            m_txt_Hex.Text = red.ToUpper() + green.ToUpper() + blue.ToUpper();
            m_txt_Hex.Update();
        }


        private Color ParseHexData(string hex_data)
        {
            if (hex_data.Length != 6)
                return Color.Black;

            string r_text, g_text, b_text;
            int r, g, b;

            r_text = hex_data.Substring(0, 2);
            g_text = hex_data.Substring(2, 2);
            b_text = hex_data.Substring(4, 2);

            r = int.Parse(r_text, System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(g_text, System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(b_text, System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }


        private void UpdateTextBoxes()
        {
            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
            m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
            m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
            m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
            m_txt_K.Text = Round(m_cmyk.K * 100).ToString();
            m_txt_Red.Text = m_rgb.R.ToString();
            m_txt_Green.Text = m_rgb.G.ToString();
            m_txt_Blue.Text = m_rgb.B.ToString();

            m_txt_Red.Update();
            m_txt_Green.Update();
            m_txt_Blue.Update();
            m_txt_Hue.Update();
            m_txt_Sat.Update();
            m_txt_Black.Update();
            m_txt_Cyan.Update();
            m_txt_Magenta.Update();
            m_txt_Yellow.Update();
            m_txt_K.Update();

            WriteHexData(m_rgb);
        }


        #endregion

        #region Public Methods

        public Color PrimaryColor
        {
            get
            {
                return m_rgb;
            }
            set
            {
                m_rgb = value;
                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                m_txt_Red.Text = m_rgb.R.ToString();
                m_txt_Green.Text = m_rgb.G.ToString();
                m_txt_Blue.Text = m_rgb.B.ToString();

                m_txt_Hue.Update();
                m_txt_Sat.Update();
                m_txt_Lum.Update();
                m_txt_Red.Update();
                m_txt_Green.Update();
                m_txt_Blue.Update();

                m_ctrl_BigBox.HSL = m_hsl;
                m_ctrl_ThinBox.HSL = m_hsl;

                m_lbl_Primary_Color.BackColor = m_rgb;
            }
        }


        public eDrawStyle DrawStyle
        {
            get
            {
                if (m_rbtn_Hue.Checked)
                    return eDrawStyle.Hue;
                else if (m_rbtn_Sat.Checked)
                    return eDrawStyle.Saturation;
                else if (m_rbtn_Black.Checked)
                    return eDrawStyle.Brightness;
                else if (m_rbtn_Red.Checked)
                    return eDrawStyle.Red;
                else if (m_rbtn_Green.Checked)
                    return eDrawStyle.Green;
                else if (m_rbtn_Blue.Checked)
                    return eDrawStyle.Blue;
                else
                    return eDrawStyle.Hue;
            }
            set
            {
                switch (value)
                {
                    case eDrawStyle.Hue:
                        m_rbtn_Hue.Checked = true;
                        break;
                    case eDrawStyle.Saturation:
                        m_rbtn_Sat.Checked = true;
                        break;
                    case eDrawStyle.Brightness:
                        m_rbtn_Black.Checked = true;
                        break;
                    case eDrawStyle.Red:
                        m_rbtn_Red.Checked = true;
                        break;
                    case eDrawStyle.Green:
                        m_rbtn_Green.Checked = true;
                        break;
                    case eDrawStyle.Blue:
                        m_rbtn_Blue.Checked = true;
                        break;
                    default:
                        m_rbtn_Hue.Checked = true;
                        break;
                }
            }
        }


        #endregion

    }

    /// <summary>
	/// Summary description for ctrl2DColorBox.
	/// </summary>
	public class ctrl2DColorBox : System.Windows.Forms.UserControl
    {
        #region Class Variables

        public enum eDrawStyle
        {
            Hue,
            Saturation,
            Brightness,
            Red,
            Green,
            Blue
        }

        private int m_iMarker_X = 0;
        private int m_iMarker_Y = 0;
        private bool m_bDragging = false;

        //	These variables keep track of how to fill in the content inside the box;
        private eDrawStyle m_eDrawStyle = eDrawStyle.Hue;
        private AdobeColors.HSL m_hsl;
        private Color m_rgb;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        #region Constructors / Destructors

        public ctrl2DColorBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //	Initialize Colors
            m_hsl = new AdobeColors.HSL();
            m_hsl.H = 1.0;
            m_hsl.S = 1.0;
            m_hsl.L = 1.0;
            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_eDrawStyle = eDrawStyle.Hue;
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // ctrl2DColorBox
            // 
            this.Name = "ctrl2DColorBox";
            this.Size = new System.Drawing.Size(260, 260);
            this.Resize += new System.EventHandler(this.ctrl2DColorBox_Resize);
            this.Load += new System.EventHandler(this.ctrl2DColorBox_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ctrl2DColorBox_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctrl2DColorBox_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctrl2DColorBox_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ctrl2DColorBox_MouseDown);

        }
        #endregion

        #region Control Events

        private void ctrl2DColorBox_Load(object sender, System.EventArgs e)
        {
            Redraw_Control();
        }


        private void ctrl2DColorBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                return;

            m_bDragging = true;     //	Begin dragging which notifies MouseMove function that it needs to update the marker

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) x = 0;
            if (x > this.Width - 4) x = this.Width - 4; //	Calculate marker position
            if (y < 0) y = 0;
            if (y > this.Height - 4) y = this.Height - 4;

            if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
                Scroll(this, e);
        }


        private void ctrl2DColorBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!m_bDragging)       //	Only respond when the mouse is dragging the marker.
                return;

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) x = 0;
            if (x > this.Width - 4) x = this.Width - 4; //	Calculate marker position
            if (y < 0) y = 0;
            if (y > this.Height - 4) y = this.Height - 4;

            if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
                Scroll(this, e);
        }


        private void ctrl2DColorBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                return;

            if (!m_bDragging)
                return;

            m_bDragging = false;

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) x = 0;
            if (x > this.Width - 4) x = this.Width - 4; //	Calculate marker position
            if (y < 0) y = 0;
            if (y > this.Height - 4) y = this.Height - 4;

            if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
                Scroll(this, e);
        }


        private void ctrl2DColorBox_Resize(object sender, System.EventArgs e)
        {
            Redraw_Control();
        }


        private void ctrl2DColorBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Redraw_Control();
        }


        #endregion

        #region Events

        public event EventHandler Scroll;

        #endregion

        #region Public Methods

        /// <summary>
        /// The drawstyle of the contol (Hue, Saturation, Brightness, Red, Green or Blue)
        /// </summary>
        public eDrawStyle DrawStyle
        {
            get
            {
                return m_eDrawStyle;
            }
            set
            {
                m_eDrawStyle = value;

                //	Redraw the control based on the new eDrawStyle
                Reset_Marker(true);
                Redraw_Control();
            }
        }


        /// <summary>
        /// The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
        /// </summary>
        public AdobeColors.HSL HSL
        {
            get
            {
                return m_hsl;
            }
            set
            {
                m_hsl = value;
                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);

                //	Redraw the control based on the new color.
                Reset_Marker(true);
                Redraw_Control();
            }
        }


        /// <summary>
        /// The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
        /// </summary>
        public Color RGB
        {
            get
            {
                return m_rgb;
            }
            set
            {
                m_rgb = value;
                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                //	Redraw the control based on the new color.
                Reset_Marker(true);
                Redraw_Control();
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Redraws only the content over the marker
        /// </summary>
        private void ClearMarker()
        {
            Graphics g = this.CreateGraphics();

            //	Determine the area that needs to be redrawn
            int start_x, start_y, end_x, end_y;
            int red = 0; int green = 0; int blue = 0;
            AdobeColors.HSL hsl_start = new AdobeColors.HSL();
            AdobeColors.HSL hsl_end = new AdobeColors.HSL();

            //	Find the markers corners
            start_x = m_iMarker_X - 5;
            start_y = m_iMarker_Y - 5;
            end_x = m_iMarker_X + 5;
            end_y = m_iMarker_Y + 5;
            //	Adjust the area if part of it hangs outside the content area
            if (start_x < 0) start_x = 0;
            if (start_y < 0) start_y = 0;
            if (end_x > this.Width - 4) end_x = this.Width - 4;
            if (end_y > this.Height - 4) end_y = this.Height - 4;

            //	Redraw the content based on the current draw style:
            //	The code get's a little messy from here
            switch (m_eDrawStyle)
            {
                //		  S=0,S=1,S=2,S=3.....S=100
                //	L=100
                //	L=99
                //	L=98		Drawstyle
                //	L=97		   Hue
                //	...
                //	L=0
                case eDrawStyle.Hue:

                    hsl_start.H = m_hsl.H; hsl_end.H = m_hsl.H; //	Hue is constant
                    hsl_start.S = (double)start_x / (this.Width - 4);   //	Because we're drawing horizontal lines, s will not change
                    hsl_end.S = (double)end_x / (this.Width - 4);       //	from line to line

                    for (int i = start_y; i <= end_y; i++)      //	For each horizontal line:
                    {
                        hsl_start.L = 1.0 - (double)i / (this.Height - 4);  //	Brightness (L) WILL change for each horizontal
                        hsl_end.L = hsl_start.L;                            //	line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                //		  H=0,H=1,H=2,H=3.....H=360
                //	L=100
                //	L=99
                //	L=98		Drawstyle
                //	L=97		Saturation
                //	...
                //	L=0
                case eDrawStyle.Saturation:

                    hsl_start.S = m_hsl.S; hsl_end.S = m_hsl.S;         //	Saturation is constant
                    hsl_start.L = 1.0 - (double)start_y / (this.Height - 4);    //	Because we're drawing vertical lines, L will 
                    hsl_end.L = 1.0 - (double)end_y / (this.Height - 4);        //	not change from line to line

                    for (int i = start_x; i <= end_x; i++)              //	For each vertical line:
                    {
                        hsl_start.H = (double)i / (this.Width - 4);         //	Hue (H) WILL change for each vertical
                        hsl_end.H = hsl_start.H;                            //	line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                        g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                    }
                    break;
                //		  H=0,H=1,H=2,H=3.....H=360
                //	S=100
                //	S=99
                //	S=98		Drawstyle
                //	S=97		Brightness
                //	...
                //	S=0
                case eDrawStyle.Brightness:

                    hsl_start.L = m_hsl.L; hsl_end.L = m_hsl.L;         //	Luminance is constant
                    hsl_start.S = 1.0 - (double)start_y / (this.Height - 4);    //	Because we're drawing vertical lines, S will 
                    hsl_end.S = 1.0 - (double)end_y / (this.Height - 4);        //	not change from line to line

                    for (int i = start_x; i <= end_x; i++)              //	For each vertical line:
                    {
                        hsl_start.H = (double)i / (this.Width - 4);         //	Hue (H) WILL change for each vertical
                        hsl_end.H = hsl_start.H;                            //	line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                        g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                    }

                    break;
                //		  B=0,B=1,B=2,B=3.....B=100
                //	G=100
                //	G=99
                //	G=98		Drawstyle
                //	G=97		   Red
                //	...
                //	G=0
                case eDrawStyle.Red:

                    red = m_rgb.R;                                                  //	Red is constant
                    int start_b = Round(255 * (double)start_x / (this.Width - 4));  //	Because we're drawing horizontal lines, B
                    int end_b = Round(255 * (double)end_x / (this.Width - 4));      //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                    {
                        green = Round(255 - (255 * (double)i / (this.Height - 4))); //	green WILL change for each horizontal line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(red, green, start_b), Color.FromArgb(red, green, end_b), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                //		  B=0,B=1,B=2,B=3.....B=100
                //	R=100
                //	R=99
                //	R=98		Drawstyle
                //	R=97		  Green
                //	...
                //	R=0
                case eDrawStyle.Green:

                    green = m_rgb.G; ;                                              //	Green is constant
                    int start_b2 = Round(255 * (double)start_x / (this.Width - 4)); //	Because we're drawing horizontal lines, B
                    int end_b2 = Round(255 * (double)end_x / (this.Width - 4));     //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                    {
                        red = Round(255 - (255 * (double)i / (this.Height - 4)));       //	red WILL change for each horizontal line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(red, green, start_b2), Color.FromArgb(red, green, end_b2), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                //		  R=0,R=1,R=2,R=3.....R=100
                //	G=100
                //	G=99
                //	G=98		Drawstyle
                //	G=97		   Blue
                //	...
                //	G=0
                case eDrawStyle.Blue:

                    blue = m_rgb.B; ;                                               //	Blue is constant
                    int start_r = Round(255 * (double)start_x / (this.Width - 4));  //	Because we're drawing horizontal lines, R
                    int end_r = Round(255 * (double)end_x / (this.Width - 4));      //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                    {
                        green = Round(255 - (255 * (double)i / (this.Height - 4))); //	green WILL change for each horizontal line drawn

                        LinearGradientBrush br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(start_r, green, blue), Color.FromArgb(end_r, green, blue), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
            }
        }


        /// <summary>
        /// Draws the marker (circle) inside the box
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Unconditional"></param>
        private void DrawMarker(int x, int y, bool Unconditional)           //	   *****
        {                                                                   //	  *  |  *
            if (x < 0) x = 0;                                               //	 *   |   *
            if (x > this.Width - 4) x = this.Width - 4;                 //	*    |    *
            if (y < 0) y = 0;                                               //	*    |    *
            if (y > this.Height - 4) y = this.Height - 4;                   //	*----X----*
                                                                            //	*    |    *
            if (m_iMarker_Y == y && m_iMarker_X == x && !Unconditional) //	*    |    *
                return;                                                     //	 *   |   *
                                                                            //	  *  |  *
            ClearMarker();                                                  //	   *****

            m_iMarker_X = x;
            m_iMarker_Y = y;

            Graphics g = this.CreateGraphics();

            Pen pen;
            AdobeColors.HSL _hsl = GetColor(x, y);  //	The selected color determines the color of the marker drawn over
                                                    //	it (black or white)
            if (_hsl.L < (double)200 / 255)
                pen = new Pen(Color.White);                                 //	White marker if selected color is dark
            else if (_hsl.H < (double)26 / 360 || _hsl.H > (double)200 / 360)
                if (_hsl.S > (double)70 / 255)
                    pen = new Pen(Color.White);
                else
                    pen = new Pen(Color.Black);                             //	Else use a black marker for lighter colors
            else
                pen = new Pen(Color.Black);

            g.DrawEllipse(pen, x - 3, y - 3, 10, 10);                       //	Draw the marker : 11 x 11 circle

            DrawBorder();       //	Force the border to be redrawn, just in case the marker has been drawn over it.
        }


        /// <summary>
        /// Draws the border around the control.
        /// </summary>
        private void DrawBorder()
        {
            Graphics g = this.CreateGraphics();

            Pen pencil;

            //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
            //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
            //	inside the gray/white rectangle

            pencil = new Pen(Color.FromArgb(172, 168, 153));    //	The same gray color used by Photoshop
            g.DrawLine(pencil, this.Width - 2, 0, 0, 0);    //	Draw top line
            g.DrawLine(pencil, 0, 0, 0, this.Height - 2);   //	Draw left hand line

            pencil = new Pen(Color.White);
            g.DrawLine(pencil, this.Width - 1, 0, this.Width - 1, this.Height - 1); //	Draw right hand line
            g.DrawLine(pencil, this.Width - 1, this.Height - 1, 0, this.Height - 1);    //	Draw bottome line

            pencil = new Pen(Color.Black);
            g.DrawRectangle(pencil, 1, 1, this.Width - 3, this.Height - 3); //	Draw inner black rectangle
        }


        /// <summary>
        /// Evaluates the DrawStyle of the control and calls the appropriate
        /// drawing function for content
        /// </summary>
        private void DrawContent()
        {
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    Draw_Style_Hue();
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation();
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance();
                    break;
                case eDrawStyle.Red:
                    Draw_Style_Red();
                    break;
                case eDrawStyle.Green:
                    Draw_Style_Green();
                    break;
                case eDrawStyle.Blue:
                    Draw_Style_Blue();
                    break;
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Hue value.
        /// </summary>
        private void Draw_Style_Hue()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL hsl_start = new AdobeColors.HSL();
            AdobeColors.HSL hsl_end = new AdobeColors.HSL();
            hsl_start.H = m_hsl.H;
            hsl_end.H = m_hsl.H;
            hsl_start.S = 0.0;
            hsl_end.S = 1.0;

            for (int i = 0; i < this.Height - 4; i++)               //	For each horizontal line in the control:
            {
                hsl_start.L = 1.0 - (double)i / (this.Height - 4);  //	Calculate luminance at this line (Hue and Saturation are constant)
                hsl_end.L = hsl_start.L;

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, 1), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, this.Width - 4, 1));
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Saturation value.
        /// </summary>
        private void Draw_Style_Saturation()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL hsl_start = new AdobeColors.HSL();
            AdobeColors.HSL hsl_end = new AdobeColors.HSL();
            hsl_start.S = m_hsl.S;
            hsl_end.S = m_hsl.S;
            hsl_start.L = 1.0;
            hsl_end.L = 0.0;

            for (int i = 0; i < this.Width - 4; i++)        //	For each vertical line in the control:
            {
                hsl_start.H = (double)i / (this.Width - 4); //	Calculate Hue at this line (Saturation and Luminance are constant)
                hsl_end.H = hsl_start.H;

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, 1, this.Height - 4), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                g.FillRectangle(br, new Rectangle(i + 2, 2, 1, this.Height - 4));
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Luminance or Brightness value.
        /// </summary>
        private void Draw_Style_Luminance()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL hsl_start = new AdobeColors.HSL();
            AdobeColors.HSL hsl_end = new AdobeColors.HSL();
            hsl_start.L = m_hsl.L;
            hsl_end.L = m_hsl.L;
            hsl_start.S = 1.0;
            hsl_end.S = 0.0;

            for (int i = 0; i < this.Width - 4; i++)        //	For each vertical line in the control:
            {
                hsl_start.H = (double)i / (this.Width - 4); //	Calculate Hue at this line (Saturation and Luminance are constant)
                hsl_end.H = hsl_start.H;

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, 1, this.Height - 4), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                g.FillRectangle(br, new Rectangle(i + 2, 2, 1, this.Height - 4));
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Red value.
        /// </summary>
        private void Draw_Style_Red()
        {
            Graphics g = this.CreateGraphics();

            int red = m_rgb.R; ;

            for (int i = 0; i < this.Height - 4; i++)               //	For each horizontal line in the control:
            {
                //	Calculate Green at this line (Red and Blue are constant)
                int green = Round(255 - (255 * (double)i / (this.Height - 4)));

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, this.Width - 4, 1));
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Green value.
        /// </summary>
        private void Draw_Style_Green()
        {
            Graphics g = this.CreateGraphics();

            int green = m_rgb.G; ;

            for (int i = 0; i < this.Height - 4; i++)   //	For each horizontal line in the control:
            {
                //	Calculate Red at this line (Green and Blue are constant)
                int red = Round(255 - (255 * (double)i / (this.Height - 4)));

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, this.Width - 4, 1));
            }
        }


        /// <summary>
        /// Draws the content of the control filling in all color values with the provided Blue value.
        /// </summary>
        private void Draw_Style_Blue()
        {
            Graphics g = this.CreateGraphics();

            int blue = m_rgb.B; ;

            for (int i = 0; i < this.Height - 4; i++)   //	For each horizontal line in the control:
            {
                //	Calculate Green at this line (Red and Blue are constant)
                int green = Round(255 - (255 * (double)i / (this.Height - 4)));

                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(2, 2, this.Width - 4, 1), Color.FromArgb(0, green, blue), Color.FromArgb(255, green, blue), 0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, this.Width - 4, 1));
            }
        }


        /// <summary>
        /// Calls all the functions neccessary to redraw the entire control.
        /// </summary>
        private void Redraw_Control()
        {
            DrawBorder();

            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    Draw_Style_Hue();
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation();
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance();
                    break;
                case eDrawStyle.Red:
                    Draw_Style_Red();
                    break;
                case eDrawStyle.Green:
                    Draw_Style_Green();
                    break;
                case eDrawStyle.Blue:
                    Draw_Style_Blue();
                    break;
            }

            DrawMarker(m_iMarker_X, m_iMarker_Y, true);
        }


        /// <summary>
        /// Resets the marker position of the slider to match the controls color.  Gives the option of redrawing the slider.
        /// </summary>
        /// <param name="Redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
        private void Reset_Marker(bool Redraw)
        {
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    m_iMarker_X = Round((this.Width - 4) * m_hsl.S);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - m_hsl.L));
                    break;
                case eDrawStyle.Saturation:
                    m_iMarker_X = Round((this.Width - 4) * m_hsl.H);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - m_hsl.L));
                    break;
                case eDrawStyle.Brightness:
                    m_iMarker_X = Round((this.Width - 4) * m_hsl.H);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - m_hsl.S));
                    break;
                case eDrawStyle.Red:
                    m_iMarker_X = Round((this.Width - 4) * (double)m_rgb.B / 255);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - (double)m_rgb.G / 255));
                    break;
                case eDrawStyle.Green:
                    m_iMarker_X = Round((this.Width - 4) * (double)m_rgb.B / 255);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - (double)m_rgb.R / 255));
                    break;
                case eDrawStyle.Blue:
                    m_iMarker_X = Round((this.Width - 4) * (double)m_rgb.R / 255);
                    m_iMarker_Y = Round((this.Height - 4) * (1.0 - (double)m_rgb.G / 255));
                    break;
            }

            if (Redraw)
                DrawMarker(m_iMarker_X, m_iMarker_Y, true);
        }


        /// <summary>
        /// Resets the controls color (both HSL and RGB variables) based on the current marker position
        /// </summary>
        private void ResetHSLRGB()
        {
            int red, green, blue;

            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    m_hsl.S = (double)m_iMarker_X / (this.Width - 4);
                    m_hsl.L = 1.0 - (double)m_iMarker_Y / (this.Height - 4);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Saturation:
                    m_hsl.H = (double)m_iMarker_X / (this.Width - 4);
                    m_hsl.L = 1.0 - (double)m_iMarker_Y / (this.Height - 4);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Brightness:
                    m_hsl.H = (double)m_iMarker_X / (this.Width - 4);
                    m_hsl.S = 1.0 - (double)m_iMarker_Y / (this.Height - 4);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Red:
                    blue = Round(255 * (double)m_iMarker_X / (this.Width - 4));
                    green = Round(255 * (1.0 - (double)m_iMarker_Y / (this.Height - 4)));
                    m_rgb = Color.FromArgb(m_rgb.R, green, blue);
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
                case eDrawStyle.Green:
                    blue = Round(255 * (double)m_iMarker_X / (this.Width - 4));
                    red = Round(255 * (1.0 - (double)m_iMarker_Y / (this.Height - 4)));
                    m_rgb = Color.FromArgb(red, m_rgb.G, blue);
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
                case eDrawStyle.Blue:
                    red = Round(255 * (double)m_iMarker_X / (this.Width - 4));
                    green = Round(255 * (1.0 - (double)m_iMarker_Y / (this.Height - 4)));
                    m_rgb = Color.FromArgb(red, green, m_rgb.B);
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
            }
        }


        /// <summary>
        /// Kindof self explanitory, I really need to look up the .NET function that does this.
        /// </summary>
        /// <param name="val">double value to be rounded to an integer</param>
        /// <returns></returns>
        private int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                ret_val += 1;

            return ret_val;

        }


        /// <summary>
        /// Returns the graphed color at the x,y position on the control
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private AdobeColors.HSL GetColor(int x, int y)
        {

            AdobeColors.HSL _hsl = new AdobeColors.HSL();

            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    _hsl.H = m_hsl.H;
                    _hsl.S = (double)x / (this.Width - 4);
                    _hsl.L = 1.0 - (double)y / (this.Height - 4);
                    break;
                case eDrawStyle.Saturation:
                    _hsl.S = m_hsl.S;
                    _hsl.H = (double)x / (this.Width - 4);
                    _hsl.L = 1.0 - (double)y / (this.Height - 4);
                    break;
                case eDrawStyle.Brightness:
                    _hsl.L = m_hsl.L;
                    _hsl.H = (double)x / (this.Width - 4);
                    _hsl.S = 1.0 - (double)y / (this.Height - 4);
                    break;
                case eDrawStyle.Red:
                    _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(m_rgb.R, Round(255 * (1.0 - (double)y / (this.Height - 4))), Round(255 * (double)x / (this.Width - 4))));
                    break;
                case eDrawStyle.Green:
                    _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(Round(255 * (1.0 - (double)y / (this.Height - 4))), m_rgb.G, Round(255 * (double)x / (this.Width - 4))));
                    break;
                case eDrawStyle.Blue:
                    _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(Round(255 * (double)x / (this.Width - 4)), Round(255 * (1.0 - (double)y / (this.Height - 4))), m_rgb.B));
                    break;
            }

            return _hsl;
        }


        #endregion
    }

    public delegate void EventHandler(object sender, EventArgs e);

    /// <summary>
    /// A vertical slider control that shows a range for a color property (a.k.a. Hue, Saturation, Brightness,
    /// Red, Green, Blue) and sends an event when the slider is changed.
    /// </summary>
    public class ctrlVerticalColorSlider : System.Windows.Forms.UserControl
    {
        #region Class Variables

        public enum eDrawStyle
        {
            Hue,
            Saturation,
            Brightness,
            Red,
            Green,
            Blue
        }


        //	Slider properties
        private int m_iMarker_Start_Y = 0;
        private bool m_bDragging = false;

        //	These variables keep track of how to fill in the content inside the box;
        private eDrawStyle m_eDrawStyle = eDrawStyle.Hue;
        private AdobeColors.HSL m_hsl;
        private Color m_rgb;

        private System.ComponentModel.Container components = null;

        #endregion

        #region Constructors / Destructors

        public ctrlVerticalColorSlider()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //	Initialize Colors
            m_hsl = new AdobeColors.HSL();
            m_hsl.H = 1.0;
            m_hsl.S = 1.0;
            m_hsl.L = 1.0;
            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
            m_eDrawStyle = eDrawStyle.Hue;
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // ctrl1DColorBar
            // 
            this.Name = "ctrl1DColorBar";
            this.Size = new System.Drawing.Size(40, 264);
            this.Resize += new System.EventHandler(this.ctrl1DColorBar_Resize);
            this.Load += new System.EventHandler(this.ctrl1DColorBar_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ctrl1DColorBar_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctrl1DColorBar_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctrl1DColorBar_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ctrl1DColorBar_MouseDown);

        }
        #endregion

        #region Control Events

        private void ctrl1DColorBar_Load(object sender, System.EventArgs e)
        {
            Redraw_Control();
        }


        private void ctrl1DColorBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                return;

            m_bDragging = true;     //	Begin dragging which notifies MouseMove function that it needs to update the marker

            int y;
            y = e.Y;
            y -= 4;                                         //	Calculate slider position
            if (y < 0) y = 0;
            if (y > this.Height - 9) y = this.Height - 9;

            if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawSlider(y, false);   //	Redraw the slider
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls slider(color) has changed
                Scroll(this, e);
        }


        private void ctrl1DColorBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!m_bDragging)       //	Only respond when the mouse is dragging the marker.
                return;

            int y;
            y = e.Y;
            y -= 4;                                         //	Calculate slider position
            if (y < 0) y = 0;
            if (y > this.Height - 9) y = this.Height - 9;

            if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawSlider(y, false);   //	Redraw the slider
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls slider(color) has changed
                Scroll(this, e);
        }


        private void ctrl1DColorBar_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                return;

            m_bDragging = false;

            int y;
            y = e.Y;
            y -= 4;                                         //	Calculate slider position
            if (y < 0) y = 0;
            if (y > this.Height - 9) y = this.Height - 9;

            if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                return;                                     //	or send a scroll notification

            DrawSlider(y, false);   //	Redraw the slider
            ResetHSLRGB();          //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls slider(color) has changed
                Scroll(this, e);
        }


        private void ctrl1DColorBar_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Redraw_Control();
        }


        private void ctrl1DColorBar_Resize(object sender, System.EventArgs e)
        {
            Redraw_Control();
        }


        #endregion

        #region Events

        public event EventHandler Scroll;

        #endregion

        #region Public Methods

        /// <summary>
        /// The drawstyle of the contol (Hue, Saturation, Brightness, Red, Green or Blue)
        /// </summary>
        public eDrawStyle DrawStyle
        {
            get
            {
                return m_eDrawStyle;
            }
            set
            {
                m_eDrawStyle = value;

                //	Redraw the control based on the new eDrawStyle
                Reset_Slider(true);
                Redraw_Control();
            }
        }


        /// <summary>
        /// The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
        /// </summary>
        public AdobeColors.HSL HSL
        {
            get
            {
                return m_hsl;
            }
            set
            {
                m_hsl = value;
                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);

                //	Redraw the control based on the new color.
                Reset_Slider(true);
                DrawContent();
            }
        }


        /// <summary>
        /// The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
        /// </summary>
        public Color RGB
        {
            get
            {
                return m_rgb;
            }
            set
            {
                m_rgb = value;
                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                //	Redraw the control based on the new color.
                Reset_Slider(true);
                DrawContent();
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Redraws the background over the slider area on both sides of the control
        /// </summary>
        private void ClearSlider()
        {
            Graphics g = this.CreateGraphics();
            Brush brush = System.Drawing.SystemBrushes.Control;
            g.FillRectangle(brush, 0, 0, 8, this.Height);               //	clear left hand slider
            g.FillRectangle(brush, this.Width - 8, 0, 8, this.Height);  //	clear right hand slider
        }


        /// <summary>
        /// Draws the slider arrows on both sides of the control.
        /// </summary>
        /// <param name="position">position value of the slider, lowest being at the bottom.  The range
        /// is between 0 and the controls height-9.  The values will be adjusted if too large/small</param>
        /// <param name="Unconditional">If Unconditional is true, the slider is drawn, otherwise some logic 
        /// is performed to determine is drawing is really neccessary.</param>
        private void DrawSlider(int position, bool Unconditional)
        {
            if (position < 0) position = 0;
            if (position > this.Height - 9) position = this.Height - 9;

            if (m_iMarker_Start_Y == position && !Unconditional)    //	If the marker position hasn't changed
                return;                                             //	since the last time it was drawn and we don't HAVE to redraw
                                                                    //	then exit procedure

            m_iMarker_Start_Y = position;   //	Update the controls marker position

            this.ClearSlider();     //	Remove old slider

            Graphics g = this.CreateGraphics();

            Pen pencil = new Pen(Color.FromArgb(116, 114, 106));    //	Same gray color Photoshop uses
            Brush brush = Brushes.White;

            Point[] arrow = new Point[7];               //	 GGG
            arrow[0] = new Point(1, position);          //	G   G
            arrow[1] = new Point(3, position);          //	G    G
            arrow[2] = new Point(7, position + 4);      //	G     G
            arrow[3] = new Point(3, position + 8);      //	G      G
            arrow[4] = new Point(1, position + 8);      //	G     G
            arrow[5] = new Point(0, position + 7);      //	G    G
            arrow[6] = new Point(0, position + 1);      //	G   G
                                                        //	 GGG

            g.FillPolygon(brush, arrow);    //	Fill left arrow with white
            g.DrawPolygon(pencil, arrow);   //	Draw left arrow border with gray

            //	    GGG
            arrow[0] = new Point(this.Width - 2, position);     //	   G   G
            arrow[1] = new Point(this.Width - 4, position);     //	  G    G
            arrow[2] = new Point(this.Width - 8, position + 4); //	 G     G
            arrow[3] = new Point(this.Width - 4, position + 8); //	G      G
            arrow[4] = new Point(this.Width - 2, position + 8); //	 G     G
            arrow[5] = new Point(this.Width - 1, position + 7); //	  G    G
            arrow[6] = new Point(this.Width - 1, position + 1); //	   G   G
                                                                //	    GGG

            g.FillPolygon(brush, arrow);    //	Fill right arrow with white
            g.DrawPolygon(pencil, arrow);   //	Draw right arrow border with gray

        }


        /// <summary>
        /// Draws the border around the control, in this case the border around the content area between
        /// the slider arrows.
        /// </summary>
        private void DrawBorder()
        {
            Graphics g = this.CreateGraphics();

            Pen pencil;

            //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
            //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
            //	inside the gray/white rectangle

            pencil = new Pen(Color.FromArgb(172, 168, 153));    //	The same gray color used by Photoshop
            g.DrawLine(pencil, this.Width - 10, 2, 9, 2);   //	Draw top line
            g.DrawLine(pencil, 9, 2, 9, this.Height - 4);   //	Draw left hand line

            pencil = new Pen(Color.White);
            g.DrawLine(pencil, this.Width - 9, 2, this.Width - 9, this.Height - 3); //	Draw right hand line
            g.DrawLine(pencil, this.Width - 9, this.Height - 3, 9, this.Height - 3);    //	Draw bottome line

            pencil = new Pen(Color.Black);
            g.DrawRectangle(pencil, 10, 3, this.Width - 20, this.Height - 7);   //	Draw inner black rectangle
        }


        /// <summary>
        /// Evaluates the DrawStyle of the control and calls the appropriate
        /// drawing function for content
        /// </summary>
        private void DrawContent()
        {
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    Draw_Style_Hue();
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation();
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance();
                    break;
                case eDrawStyle.Red:
                    Draw_Style_Red();
                    break;
                case eDrawStyle.Green:
                    Draw_Style_Green();
                    break;
                case eDrawStyle.Blue:
                    Draw_Style_Blue();
                    break;
            }
        }


        #region Draw_Style_X - Content drawing functions

        //	The following functions do the real work of the control, drawing the primary content (the area between the slider)
        //	

        /// <summary>
        /// Fills in the content of the control showing all values of Hue (from 0 to 360)
        /// </summary>
        private void Draw_Style_Hue()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL _hsl = new AdobeColors.HSL();
            _hsl.S = 1.0;   //	S and L will both be at 100% for this DrawStyle
            _hsl.L = 1.0;

            for (int i = 0; i < this.Height - 8; i++)   //	i represents the current line of pixels we want to draw horizontally
            {
                _hsl.H = 1.0 - (double)i / (this.Height - 8);           //	H (hue) is based on the current vertical position
                Pen pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4); //	Draw the line and loop back for next line
            }
        }


        /// <summary>
        /// Fills in the content of the control showing all values of Saturation (0 to 100%) for the given
        /// Hue and Luminance.
        /// </summary>
        private void Draw_Style_Saturation()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL _hsl = new AdobeColors.HSL();
            _hsl.H = m_hsl.H;   //	Use the H and L values of the current color (m_hsl)
            _hsl.L = m_hsl.L;

            for (int i = 0; i < this.Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
            {
                _hsl.S = 1.0 - (double)i / (this.Height - 8);           //	S (Saturation) is based on the current vertical position
                Pen pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4); //	Draw the line and loop back for next line
            }
        }


        /// <summary>
        /// Fills in the content of the control showing all values of Luminance (0 to 100%) for the given
        /// Hue and Saturation.
        /// </summary>
        private void Draw_Style_Luminance()
        {
            Graphics g = this.CreateGraphics();

            AdobeColors.HSL _hsl = new AdobeColors.HSL();
            _hsl.H = m_hsl.H;   //	Use the H and S values of the current color (m_hsl)
            _hsl.S = m_hsl.S;

            for (int i = 0; i < this.Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
            {
                _hsl.L = 1.0 - (double)i / (this.Height - 8);           //	L (Luminance) is based on the current vertical position
                Pen pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4); //	Draw the line and loop back for next line
            }
        }


        /// <summary>
        /// Fills in the content of the control showing all values of Red (0 to 255) for the given
        /// Green and Blue.
        /// </summary>
        private void Draw_Style_Red()
        {
            Graphics g = this.CreateGraphics();

            for (int i = 0; i < this.Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
            {
                int red = 255 - Round(255 * (double)i / (this.Height - 8)); //	red is based on the current vertical position
                Pen pen = new Pen(Color.FromArgb(red, m_rgb.G, m_rgb.B));   //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4);         //	Draw the line and loop back for next line
            }
        }


        /// <summary>
        /// Fills in the content of the control showing all values of Green (0 to 255) for the given
        /// Red and Blue.
        /// </summary>
        private void Draw_Style_Green()
        {
            Graphics g = this.CreateGraphics();

            for (int i = 0; i < this.Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
            {
                int green = 255 - Round(255 * (double)i / (this.Height - 8));   //	green is based on the current vertical position
                Pen pen = new Pen(Color.FromArgb(m_rgb.R, green, m_rgb.B)); //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4);         //	Draw the line and loop back for next line
            }
        }


        /// <summary>
        /// Fills in the content of the control showing all values of Blue (0 to 255) for the given
        /// Red and Green.
        /// </summary>
        private void Draw_Style_Blue()
        {
            Graphics g = this.CreateGraphics();

            for (int i = 0; i < this.Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
            {
                int blue = 255 - Round(255 * (double)i / (this.Height - 8));    //	green is based on the current vertical position
                Pen pen = new Pen(Color.FromArgb(m_rgb.R, m_rgb.G, blue));  //	Get the Color for this line

                g.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4);         //	Draw the line and loop back for next line
            }
        }


        #endregion

        /// <summary>
        /// Calls all the functions neccessary to redraw the entire control.
        /// </summary>
        private void Redraw_Control()
        {
            DrawSlider(m_iMarker_Start_Y, true);
            DrawBorder();
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    Draw_Style_Hue();
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation();
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance();
                    break;
                case eDrawStyle.Red:
                    Draw_Style_Red();
                    break;
                case eDrawStyle.Green:
                    Draw_Style_Green();
                    break;
                case eDrawStyle.Blue:
                    Draw_Style_Blue();
                    break;
            }
        }


        /// <summary>
        /// Resets the vertical position of the slider to match the controls color.  Gives the option of redrawing the slider.
        /// </summary>
        /// <param name="Redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
        private void Reset_Slider(bool Redraw)
        {
            //	The position of the marker (slider) changes based on the current drawstyle:
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * m_hsl.H);
                    break;
                case eDrawStyle.Saturation:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * m_hsl.S);
                    break;
                case eDrawStyle.Brightness:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * m_hsl.L);
                    break;
                case eDrawStyle.Red:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * (double)m_rgb.R / 255);
                    break;
                case eDrawStyle.Green:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * (double)m_rgb.G / 255);
                    break;
                case eDrawStyle.Blue:
                    m_iMarker_Start_Y = (this.Height - 8) - Round((this.Height - 8) * (double)m_rgb.B / 255);
                    break;
            }

            if (Redraw)
                DrawSlider(m_iMarker_Start_Y, true);
        }


        /// <summary>
        /// Resets the controls color (both HSL and RGB variables) based on the current slider position
        /// </summary>
        private void ResetHSLRGB()
        {
            switch (m_eDrawStyle)
            {
                case eDrawStyle.Hue:
                    m_hsl.H = 1.0 - (double)m_iMarker_Start_Y / (this.Height - 9);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Saturation:
                    m_hsl.S = 1.0 - (double)m_iMarker_Start_Y / (this.Height - 9);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Brightness:
                    m_hsl.L = 1.0 - (double)m_iMarker_Start_Y / (this.Height - 9);
                    m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                    break;
                case eDrawStyle.Red:
                    m_rgb = Color.FromArgb(255 - Round(255 * (double)m_iMarker_Start_Y / (this.Height - 9)), m_rgb.G, m_rgb.B);
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
                case eDrawStyle.Green:
                    m_rgb = Color.FromArgb(m_rgb.R, 255 - Round(255 * (double)m_iMarker_Start_Y / (this.Height - 9)), m_rgb.B);
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
                case eDrawStyle.Blue:
                    m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 255 - Round(255 * (double)m_iMarker_Start_Y / (this.Height - 9)));
                    m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                    break;
            }
        }


        /// <summary>
        /// Kindof self explanitory, I really need to look up the .NET function that does this.
        /// </summary>
        /// <param name="val">double value to be rounded to an integer</param>
        /// <returns></returns>
        private int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                ret_val += 1;

            return ret_val;

        }


        #endregion
    }
}
