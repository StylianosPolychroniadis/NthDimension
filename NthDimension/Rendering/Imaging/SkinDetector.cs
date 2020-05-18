using System.Drawing;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Imaging
{
    public class SimpleSkinDetector4 : ISkinDetector
    {
        #region ISkinDetector Members

        public bool IsSkin(System.Drawing.Color color)
        {
            if (((double)color.B / (double)color.G < 1.249) &
                ((double)BitmapUtils.ChannelSum(color) / (double)(3 * color.R) > 0.696) &
                (0.3333 - (double)color.B / (double)BitmapUtils.ChannelSum(color) > 0.014) &
                ((double)color.G / (double)(3 * BitmapUtils.ChannelSum(color)) < 0.108))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region IImageSelector Members

        public bool IsSelectedPoint(System.Drawing.Color color)
        {
            return IsSkin(color);
        }

        #endregion
    }



    /// <summary>
    /// Interface for select pixels on image.
    /// </summary>
    public interface IImageSelector
    {
        bool IsSelectedPoint(Color color);
    }
    /// <summary>
    /// Interface for methods of skin detection.
    /// </summary>
    public interface ISkinDetector : IImageSelector
    {
        bool IsSkin(Color color);
    }

    /// <summary>
    /// Class for selection pixels of image.
    /// </summary>
    public class ImageSelector
    {
        /// <summary>
        /// Source image.
        /// </summary>
        private Bitmap sourceImage;

        public Bitmap SourceImage
        {
            get { return sourceImage; }
            set { sourceImage = value; }
        }

        /// <summary>
        /// Selection method;
        /// </summary>
        private IImageSelector selector;

        public IImageSelector Selector
        {
            get { return selector; }
            set { selector = value; }
        }

        /// <summary>
        /// Color for background of selection.
        /// </summary>
        private Color backColor = Color.FromArgb(0,0,0,0);

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public Bitmap SelectImage
        {
            get
            {
                Bitmap bmp = new Bitmap(sourceImage);
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        if (selector.IsSelectedPoint(pixel) == true)
                        {
                            bmp.SetPixel(i, j, pixel);
                        }
                        else
                        {
                            bmp.SetPixel(i, j, backColor);
                        }
                    }
                }
                return bmp;
            }
        }

        #region Constructors

        public ImageSelector(Bitmap bmp)
        {
            this.sourceImage = bmp;
        }

        public ImageSelector(Bitmap bmp, IImageSelector selector)
        {
            this.sourceImage = bmp;
            this.selector = selector;
        }

        #endregion
    }

    /// <summary>
    /// Class for skin detection.
    /// </summary>
    public class BaseDetector
    {
        /// <summary>
        /// Source image.
        /// </summary>
        private Bitmap sourceImage;

        public Bitmap SourceImage
        {
            get { return sourceImage; }
            set { sourceImage = value; }
        }
        /// <summary>
        /// Detection method.
        /// </summary>
        private ISkinDetector detector;

        public ISkinDetector Detector
        {
            get { return detector; }
            set { detector = value; }
        }

        #region Constructors

        public BaseDetector(Bitmap bmp)
        {
            this.sourceImage = bmp;
        }

        public BaseDetector(Bitmap bmp, ISkinDetector detector)
        {
            this.sourceImage = bmp;
            this.detector = detector;
        }

        #endregion

        public Bitmap SkinDetectionImage
        {
            get
            {
                ImageSelector selector = new ImageSelector(sourceImage, detector);
                return selector.SelectImage;
            }
        }
    }


}
