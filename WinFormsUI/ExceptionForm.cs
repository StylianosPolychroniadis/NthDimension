using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NthDimension;
using NthDimension.Utilities;

namespace NthDimension
{

    /// <summary>
    /// Form will be displayed for all unhandled exceptions in the application.
    /// </summary>
    public partial class ExceptionForm : Form {

        /// <summary>
        /// WINAPI method used to hide caret in the textbox control.
        /// </summary>
        /// <param name="hWnd">Handle of the control.</param>
        /// <returns>Operation result.</returns>
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="e">Exception that will be displayed in the exception details.</param>
        public ExceptionForm(Exception e) {
            InitializeComponent();
            LocalizeUIStrings();
            rtbErrorDetails.GotFocus +=new EventHandler(rtbErrorDetails_GotFocus);
            rtbErrorDetails.SelectionChanged +=new EventHandler(rtbErrorDetails_SelectionChanged);
            rtbErrorDetails.MouseDown +=new MouseEventHandler(rtbErrorDetails_MouseDown);
            if (e != null) {
                rtbErrorDetails.Text = e.ToString();
                ckbErrorDetails.Enabled = true;
            } else {
                ckbErrorDetails.Enabled = false;
            }
        }

        /// <summary>
        /// If Localization is available and initialized, all strings
        /// in the form are translated.
        /// </summary>
        private void LocalizeUIStrings() {
            if (Localization.IsInitialized()) {
                btnOK.Text = Localization.GetString("MorphoMain.forms.ExceptionForm.btnOK.Text"); // "OK"
                ckbLogError.Text = Localization.GetString("MorphoMain.forms.ExceptionForm.ckbLogError.Text"); // "Log error"                
                ckbErrorDetails.Text = Localization.GetString("MorphoMain.forms.ExceptionForm.ckbErrorDetails.Text"); // "Show details"
                lbErrorDetails.Text = Localization.GetString("MorphoMain.forms.ExceptionForm.lbErrorDetails"); // "Error details:"
                Text = Localization.GetString("MorphoMain.forms.ExceptionForm.Text"); // "Unexpected error"
                lbErrorInfo.Text = Localization.GetString("MorphoMain.forms.ExceptionForm.lbErrorInfo.Text"); // "Unexpected error occured in Hexa Platform application. If the error persists,"
                                                                                                              // "try to reinstall application. Application will shut down now and the error will be"
                                                                                                              // "logged to the file."
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExceptionForm() : this(null) {
        }

        /// <summary>
        /// Hide caret from the error details textbox when selection is changed.
        /// </summary>        
        private void rtbErrorDetails_SelectionChanged(object sender, EventArgs e) {
            HideCaret(rtbErrorDetails.Handle);
        }

        /// <summary>
        /// Hide caret from the error details textbox when is got the focus.
        /// </summary>        
        private void rtbErrorDetails_GotFocus(object sender, EventArgs e) {
            HideCaret(rtbErrorDetails.Handle);
        }

        /// <summary>
        /// Hide caret from the error details textbox for mouseDown event.
        /// </summary>
        private void rtbErrorDetails_MouseDown(object sender, MouseEventArgs e) {
            HideCaret(rtbErrorDetails.Handle);
        }

        /// <summary>
        /// If ckbErrorDetails is checked, error details are displayed in the textbox, 
        /// otherwise the details are hidden.
        /// </summary>        
        private void ckbErrorDetails_CheckedChanged(object sender, EventArgs e) {
            int rtbHeight = rtbErrorDetails.Height + rtbErrorDetails.Margin.Top + rtbErrorDetails.Margin.Bottom;
            if (ckbErrorDetails.Checked) {
                this.Size = new Size(Size.Width, Size.Height + rtbHeight);
                rtbErrorDetails.Visible = true;
                lbErrorDetails.Visible = true;
            } else {
                rtbErrorDetails.Visible = false;
                lbErrorDetails.Visible = false;
                this.Size = new Size(Size.Width, Size.Height - rtbHeight);
            }
        }

        /// <summary>
        /// Close form and set the dialog result on the OK click.
        /// </summary>        
        private void btnOK_Click(object sender, EventArgs e) {
            LogError = ckbLogError.Checked;
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Property used to determine if ckbLogError was checked by user.
        /// </summary>
        public bool LogError {
            get;
            private set;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogError = true;
        }        
    }
}
