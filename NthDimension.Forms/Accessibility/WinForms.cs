using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Accessibility
{

    #region CreateParams
    //
    // Summary:
    //     Encapsulates the information needed when creating a control.
    public class CreateParams
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.CreateParams class.
        public CreateParams() { }

        //
        // Summary:
        //     Gets or sets the name of the Windows class to derive the control from.
        //
        // Returns:
        //     The name of the Windows class to derive the control from.
        public string ClassName { get; set; }
        //
        // Summary:
        //     Gets or sets the control's initial text.
        //
        // Returns:
        //     The control's initial text.
        public string Caption { get; set; }
        //
        // Summary:
        //     Gets or sets a bitwise combination of window style values.
        //
        // Returns:
        //     A bitwise combination of the window style values.
        public int Style { get; set; }
        //
        // Summary:
        //     Gets or sets a bitwise combination of extended window style values.
        //
        // Returns:
        //     A bitwise combination of the extended window style values.
        public int ExStyle { get; set; }
        //
        // Summary:
        //     Gets or sets a bitwise combination of class style values.
        //
        // Returns:
        //     A bitwise combination of the class style values.
        public int ClassStyle { get; set; }
        //
        // Summary:
        //     Gets or sets the initial left position of the control.
        //
        // Returns:
        //     The numeric value that represents the initial left position of the control.
        public int X { get; set; }
        //
        // Summary:
        //     Gets or sets the top position of the initial location of the control.
        //
        // Returns:
        //     The numeric value that represents the top position of the initial location of
        //     the control.
        public int Y { get; set; }
        //
        // Summary:
        //     Gets or sets the initial width of the control.
        //
        // Returns:
        //     The numeric value that represents the initial width of the control.
        public int Width { get; set; }
        //
        // Summary:
        //     Gets or sets the initial height of the control.
        //
        // Returns:
        //     The numeric value that represents the initial height of the control.
        public int Height { get; set; }
        //
        // Summary:
        //     Gets or sets the control's parent.
        //
        // Returns:
        //     An System.IntPtr that contains the window handle of the control's parent.
        public IntPtr Parent { get; set; }
        //
        // Summary:
        //     Gets or sets additional parameter information needed to create the control.
        //
        // Returns:
        //     The System.Object that holds additional parameter information needed to create
        //     the control.
        public object Param { get; set; }

        //
        // Returns:
        //     A string that represents the current object.
        public override string ToString() { return string.Empty; }
    }
    #endregion


}
