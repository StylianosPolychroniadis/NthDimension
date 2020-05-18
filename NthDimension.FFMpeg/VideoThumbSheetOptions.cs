using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.FFMpeg
{
    public class VideoThumbSheetOptions
    {
        /// <summary>The number of thumbnails in a row</summary>
        public int ThumbColumns { get; set; }
        /// <summary>The number thumbnail rows</summary>
        public int ThumbRows { get; set; }
        /// <summary>The width of a single thumbnail in pixel</summary>
        public int ThumbWidth { get; set; }
        /// <summary>The width of the margin around the whole sheet</summary>
        public int Margin { get; set; }
        /// <summary>The space between the thumbnails (space between header and thumbnails is 2 * Padding)</summary>
        public int Padding { get; set; }

        private Font _headerFont = null;
        /// <summary>The Font of the header text (old Font will be automatically disposed when set)</summary>
        public Font HeaderFont
        {
            get { return _headerFont; }
            set
            {
                if (_headerFont != null) _headerFont.Dispose();
                _headerFont = value;
            }
        }

        private Font _indexFont = null;
        /// <summary>The Font of the time index text on each thumbnail (old Font will be automatically disposed when set)</summary>
        public Font IndexFont
        {
            get { return _indexFont; }
            set
            {
                if (_indexFont != null) _indexFont.Dispose();
                _indexFont = value;
            }
        }

        /// <summary></summary>
        public Color BackgroundColor { get; set; }
        /// <summary>The Font of the header text</summary>
        public Color HeaderColor { get; set; }
        /// <summary>The color of the time index text on each thumbnail</summary>
        public Color IndexColor { get; set; }
        /// <summary>The shadow color of the time index text on each thumbnail</summary>
        public Color IndexShadowColor { get; set; }
        /// <summary>If true, the edge of each thumbnail is overdrawn with a border</summary>
        public bool DrawThumbnailBorder { get; set; }
        /// <summary>The color of the thumbnail borders</summary>
        public Color ThumbBorderColor { get; set; }

        /// <summary>If false, the last key frame is used - can result in irregular gaps between thumbnail time positions but is much faster</summary>
        public bool ForceExactTimePosition { get; set; }

        public VideoThumbSheetOptions(int thumbColumns, int thumbLines)
        {
            this.ThumbColumns = thumbColumns;
            this.ThumbRows = thumbLines;
            this.ThumbWidth = 100;
            this.Margin = 10;
            this.Padding = 3;
            this.HeaderFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
            this.IndexFont = new Font(FontFamily.GenericMonospace, 8, FontStyle.Regular);
            this.BackgroundColor = Color.White;
            this.HeaderColor = Color.Black;
            this.IndexColor = Color.White;
            this.IndexShadowColor = Color.Black;
            this.ThumbBorderColor = Color.Black;
            this.DrawThumbnailBorder = false;
            this.ForceExactTimePosition = true;
        }

        public static VideoThumbSheetOptions Default { get { return new VideoThumbSheetOptions(6, 6); } }
    }
}
