using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer.NanoVG;
using NthDimension.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rasterizer;

namespace NthStudio.Gui.Widgets
{
    public class Picture : Widget
    {
        public enum enuRectangle
        {
            Normal,
            Rounded
        }

        protected string m_pictureFile;
        protected int m_picture = -1;
        private int m_fboTexture;
        private bool m_fboDepth = false;

        private bool m_init;

        private bool fromFbo;
        private bool skipRender;

        public bool Scissored = false;

        private bool m_superimpose;

        private NVGimageFlags imageflags;
        public NVGimageFlags ImageFlags
        {
            get { return imageflags; }
        }

        public int PictureIndex
        {
            get { return m_picture; }
        }
        public string PictureFile
        {
            get { return m_pictureFile; }
        }

        public Picture(string picture, bool superimpose = true, NVGimageFlags flags = NVGimageFlags.NVG_IMAGE_FLIPY)
        {

            this.skipRender = string.IsNullOrEmpty(picture);
            this.m_pictureFile = picture;
            this.m_superimpose = superimpose;
            this.imageflags = flags;
            //this.InitializeComponent();
        }
        public Picture(int glIndex, bool useFbo = false, NVGimageFlags flags = NVGimageFlags.NVG_IMAGE_FLIPY)
        {
            this.m_picture = glIndex;
            fromFbo = useFbo;
            
            if (useFbo)
                m_fboTexture = glIndex;

            this.imageflags = flags;
        }
        ~Picture()
        {
            if (m_picture > 0)
            {
                //NanoVG.nvgDeleteImage(WindowsGame.vg, this.m_picture);
                //WindowsGame.Instance.checkGlError(string.Format("Delete UI Texture {0}", this.m_picture));
                //this.m_picture = -1;
            }
        }

        public enuRectangle RectangleMode = enuRectangle.Rounded;

        private void InitializeComponent()
        {
            if (!fromFbo)
            {
                if (!string.IsNullOrEmpty(m_pictureFile))
                {
                    if (!string.IsNullOrEmpty(m_pictureFile) && File.Exists(m_pictureFile))
                        loadPictureFromFile(m_pictureFile);
                }
                else if (this.m_picture > 0)
                    loadPictureFromTexture(this.m_picture, this.Width, this.Height);
                else
                    skipRender = true;
            }
            else
            {
                loadPictureFromFramebuffer(this.m_picture);
            }


            m_init = true;
        }

        public void Reload()
        {
            this.m_picture = -1;
            this.m_init = false;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            if (skipRender)
            {
                return;
            }

            if (fromFbo)
            {
                NanoVG.nvgBeginPath(StudioWindow.vg);
                NanoVG.nvgRect(StudioWindow.vg, Parent.X, Parent.Y, this.Width, this.Height);
                NanoVG.nvgClosePath(StudioWindow.vg);

                

                NVGpaint fbPaint = NanoVG.nvgImagePattern(StudioWindow.vg, Parent.X, Parent.Y, this.Width, this.Height, this.m_picture, 0, 1.0f);
                NanoVG.nvgFillPaint(StudioWindow.vg, fbPaint);
                NanoVG.nvgFill(StudioWindow.vg);
            }
            else
            {
                if (m_pictureFile == string.Empty && m_picture == 0)
                {
                    ConsoleUtil.errorlog("Picture.DoPaint()", " Picture filename cannot be empty");
                    return;
                }

                switch (RectangleMode)
                {
                    case enuRectangle.Normal:
                        if (!Scissored) e.GC.DrawImage(this.m_picture, this.Location.X, this.Location.Y, this.Width, this.Height);
                        else e.GC.DrawImageScissored(this.m_picture, this.Location.X, this.Location.Y, this.Width, this.Height);
                        break;
                    case enuRectangle.Rounded:
                        if (!Scissored) e.GC.DrawImageRounded(this.m_picture, this.Location.X, this.Location.Y, this.Width, this.Height, 5f);
                        else e.GC.DrawImageRoundedScissored(this.m_picture, this.Location.X, this.Location.Y, this.Width, this.Height, 5f);

                        break;
                }
            }
        }

        private void loadPictureFromFile(string path, bool noDelete = true) // ToDo switch to noDelete = false and TEST TEST TEST -> false should be final
        {
            this.m_pictureFile = path;



            if (File.Exists(m_pictureFile))
            {
                #region Aug-14-18 resizing to POT
                try
                {
                    Size original = ImageUtil.GetDimensions(m_pictureFile);
                    Size pot = ImageUtil.GetPOTSize(original);

                    if (pot != original)
                    {
                        ConsoleUtil.errorlog("NPOT", string.Format("{0} -> Size {1} should be {2}", Path.GetFileName(m_pictureFile), original, pot));
                        //ImageUtil.Superimpose(m_pictureFile, pot).Save(m_pictureFile);
                    }
                }
                catch (System.Exception iE)
                {
                    ConsoleUtil.errorlog("ImageUtil.GetDimensions", string.Format("{0}\n{1}", iE.Message, iE.StackTrace));
                }


                #endregion Aug-14-18


                this.m_picture = NanoVG.nvgCreateImage(ref StudioWindow.vg, m_pictureFile, (int)NVGimageFlagsGL.NVG_IMAGE_NODELETE);

                if (this.m_picture == -1)
                    throw new System.Exception("Image failed to load. OpenGL did not create a texture");
            }
            else
            {
                ConsoleUtil.log(string.Format("(!) File not found {0} using DEFAULT PICTURE", m_pictureFile));


                //if (!File.Exists(m_pictureFile))
                //    Properties.Resources.UserProfile.Save(m_pictureFile);

                //skipRender = true;
                m_init = false;
                return;
            }



            if (-1 == this.m_picture || 0 == this.m_picture)
            {
                ConsoleUtil.errorlog("PictureCircle ", string.Format(" Failed to load image {0}", m_pictureFile));
                skipRender = true;
                return;
            }
        }
        private void loadPictureFromFramebuffer(int textureId, bool isDepth = false)
        {
            this.m_picture = NanoVG.CreateImage(StudioWindow.vg, textureId, StudioWindow.Instance.Width, StudioWindow.Instance.Height, (int)imageflags);
            
        }

        private void loadPictureFromTexture(int textureId, int width, int height)
        {
            this.m_picture = NanoVG.CreateImage(StudioWindow.vg, textureId, width, height, (int)imageflags);
        }
    }
}
