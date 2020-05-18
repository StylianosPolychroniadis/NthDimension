using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using System;
using System.Reflection;

namespace NthDimension.Forms
{
    /// <summary>
    /// Description of Cursors.
    /// </summary>
    public static class Cursors
    {
        public static event MouseCursorChangedHandler MouseCursorChanged;

        #region Static-Constructor

        static Cursors()
        {
            string resStr = String.Empty;

            try
            {
                var assb = Assembly.GetExecutingAssembly();

                //Load cursors
                resStr = "NthDimension.Forms.Cursors.hand";
                Hand = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.cross";
                Cross = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.arrow";
                Arrow = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.right_ptr";
                ArrowRight = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.top_left_corner";
                NW = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.top_right_corner";
                NE = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.bottom_left_corner";
                SW = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.bottom_right_corner";
                SE = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.sb_h_double_arrow";
                HSplit = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.sb_v_double_arrow";
                VSplit = NanoCursor.Load(assb.GetManifestResourceStream(resStr));
                resStr = "NthDimension.Forms.Cursors.ibeam";
                Text = NanoCursor.Load(assb.GetManifestResourceStream(resStr));



                Default = Arrow;
            }
            catch (Exception ex)
            {
                throw new Exception("Resource not found: " + resStr, ex);
            }
        }
        #endregion Static-Constructor

        static NanoCursor Cursor_ = Default;
        public static NanoCursor Cursor
        {
            get
            {
                if (Cursor_ == null)
                    Cursor = Default;
                return Cursor_;
            }
            set
            {
                if (value == null || value == Cursor_)
                    return;
                Cursor_ = value;

                if (MouseCursorChanged != null)
                    MouseCursorChanged(null, new MouseCursorChangedEventArgs(value));
            }
        }

        public static NanoCursor Hand;
        /// <summary>
        /// Asigned to 'Cursors.arrow'
        /// </summary>
        public static NanoCursor Default;
        /// <summary>
        /// Asigned to 'Cursors.cross'
        /// </summary>
        public static NanoCursor Cross;
        /// <summary>
        /// Asigned to 'Cursors.arrow'
        /// </summary>
        public static NanoCursor Arrow;
        /// <summary>
        /// Asigned to 'Cursors.arrow_right'
        /// </summary>
        public static NanoCursor ArrowRight;
        /// <summary>
        /// Asigned to 'Cursors.ibeam'
        /// </summary>
        public static NanoCursor Text;
        /// <summary>
        /// Asigned to 'Cursors.bottom_left_corner'
        /// </summary>
        public static NanoCursor SW;
        /// <summary>
        /// Asigned to 'Cursors.bottom_right_corner'
        /// </summary>
        public static NanoCursor SE;
        /// <summary>
        /// Asigned to 'Cursors.top_left_corner'
        /// </summary>
        public static NanoCursor NW;
        /// <summary>
        /// Asigned to 'Cursors.top_right_corner'
        /// </summary>
        public static NanoCursor NE;
        /// <summary>
        /// 
        /// </summary>
        public static NanoCursor N;
        /// <summary>
        /// 
        /// </summary>
        public static NanoCursor S;
        /// <summary>
        /// Asigned to ScrollBarV 'Cursors.sb_v_double_arrow'
        /// </summary>
        public static NanoCursor VSplit;
        /// <summary>
        /// Asigned to ScrollBarH 'Cursors.sb_h_double_arrow'
        /// </summary>
        public static NanoCursor HSplit;

        //
        public static NanoCursor AeroHand;
        public static NanoCursor AeroSoci;
        public static NanoCursor SociArrow;
  
    }
}
