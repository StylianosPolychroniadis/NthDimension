using NthDimension.Forms.Events;
using NthDimension.Rendering.Utilities;
using NthDimension.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public sealed class SceneItem  //: Widget
    {
        private delegate void FileSizesCollected();
        private event FileSizesCollected                OnFileSizesCollected;
        private static string                               assetUriPath                        = "{0}Release/Scenes/{1}"; //= "{0}/Release/Scenes/{1}";
        
        


        #region Fields

        internal string                                     m_sceneName                         = "(Not Assigned)";

        private bool                                        m_availableDownload                 = false;
        private bool                                        m_availabityRequested               = false;
        private bool                                        m_ready;
        

        protected long                                      m_modelCacheSize                    = -1L;     // Bytes
        protected long                                      m_textureCacheSize                  = -1L;
        protected long                                      m_materialCacheSize                 = -1L;
        protected long                                      m_sceneXmlSize                      = -1L;

        private Uri                                         m_modelCacheUri;
        private Uri                                         m_textureCacheUri;
        private Uri                                         m_materialCacheUri;
        private Uri                                         m_sceneXmlUri;
        // WARNING: Cross-Thread variables
        protected volatile int                              m_progressModelCache                = 0;
        protected volatile int                              m_progressTextureCache              = 0;
        protected volatile int                              m_progressMaterialCache             = 0;
        protected volatile int                              m_progressXmlScene                  = 0;
        protected volatile bool                             m_modelCacheDownloadSuccess         = false;
        protected volatile bool                             m_textureCacheDownloadSuccess       = false;
        protected volatile bool                             m_materialCacheDownloadSuccess      = false;
        protected volatile bool                             m_sceneXmlDownloadSuccess           = false;
        protected volatile bool                             m_checkingForUpdates                = false;

        #region IDownloadCache (NotImplemented) TODO:: Use for pause/resume functionality for file downloads
        //protected IDownloadCache _downloadCache; // TODO:: Implement to add download resume functionality

        //protected FileDownloader modelFileDownloader;
        //protected FileDownloader textureFileDownloader;
        //protected FileDownloader materialFileDownloader;
        //protected FileDownloader sceneFileDownloader;

        //private DialogSimple m_simpleDialog;

        private bool m_cityUnderDevelopment = true;
        #endregion IDownloadCache (NotImplemented) TODO:: Use for pause/resume functionality for file downloads
        #endregion Fields

        #region Properties


        public string                                       SceneName
        {
            get { return m_sceneName; }
        }
        public string                                       SceneIdentifier { get { return Package.SceneIdentifier; } }
        public bool                                         SceneIsLoading;
        public readonly ScenePackage                        Package;

        #region Business Logic (this tries to be Scene specific. Business logic items should move to a higher layer)
        // NOTE:: Used by business logic models
        //public readonly string                              Country;
        //public readonly string                              City;
        //public int                                          Flag                                = -1;
        //private Picture m_flag;
        //private Label m_lbCity;
        //private Label m_lbCountry;

        //internal int                    m_listIndex;
        #endregion
        public bool                                         Ready
        {
            get
            {
                return m_ready;

                //if (m_availabityRequested && !Program.Offline)
                //    return m_available;



                //return false;
            }

            // TODO:: Somehow acquire the file sizes and include in the operation
        }
        public bool                                         AvailableDownload
        {
            get { return m_availableDownload; }
        }
        public bool                                         DownloadingData = false;
        public float                                        DownloadSize = 0f;
        public float                                        ExtractedSize = 0f;

        public bool                                         UnderDevelopment
        {
            get { return m_cityUnderDevelopment; }
        }
        public float                                        DownloadProgress
        {
            get
            {
                float ret = (m_progressModelCache +
                             m_progressTextureCache +
                             m_progressMaterialCache +
                             m_progressXmlScene) / 4f;

                if (m_modelCacheDownloadSuccess &&
                    m_textureCacheDownloadSuccess &&
                    m_materialCacheDownloadSuccess &&
                    m_sceneXmlDownloadSuccess)
                {
                    DownloadingData = false;
                    m_ready = true;

                }



                return ret;
            }
        }

        public float                                        LoadingAssetsProgress;

        


        #endregion Properties

        #region Ctor

        //public SceneItem(string cityName, string country, Size size, ScenePackage filesPackage)
        public SceneItem(ScenePackage filesPackage, string name)
        {
            this.m_sceneName        = name;
            this.Package            = filesPackage;

            #region Creates Widgets for (Business Logic)

            // Business Logic variables
            //this.City = cityName;
            //this.Country = country.ToUpper();
            ////this.Size = size;

            #region Widgets
            Point pLabel = new Point(5, 0);

            try
            {
                #region Downloads and stores locally .png image files
                //string imageSquare = string.Format("_{0}.png", country);
                //string imageRound = string.Format("_{0}_r.png", country);
                //string savePathSquare = Path.Combine(DirectoryUtil.AppData_MySoci_Temporary, imageSquare).ToString();
                //string savePathRound = Path.Combine(DirectoryUtil.AppData_MySoci_Temporary, imageRound).ToString();

                //try
                //{
                //    if (!File.Exists(savePathSquare))
                //        using (Bitmap b =
                //            (Bitmap)(Properties.Resources.ResourceManager.GetObject(string.Format("_{0}", country))))
                //            if (null != b)
                //                b.Save(savePathSquare, ImageFormat.Png);
                //}
                //catch { }

                //try
                //{
                //    if (!File.Exists(savePathRound))
                //        using (Bitmap b =
                //            (Bitmap)(Properties.Resources.ResourceManager.GetObject(string.Format("_{0}_r", country))))
                //            if (null != b)
                //                b.Save(savePathRound, ImageFormat.Png);
                //}
                //catch { }

                //this.m_flag = new Picture(savePathSquare);
                //this.m_flag.Size = new Size(this.Height - 2, this.Height - 2);
                //this.m_flag.Location = new Point(1, 1);
                //this.m_flag.BGColor = Color.FromArgb(0, 0, 0, 0);
                //this.m_flag.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
                //{
                //    this.onSelectMe(mea);
                //};
                //this.m_flag.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
                //{
                //    Parent.Parent.OnMouseWheel(e);
                //};
                //this.Widgets.Add(this.m_flag);
                //this.m_flag.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
                //this.m_flag.MouseLeaveEvent += delegate { this.BGColor = Color.White; };
                //pLabel = new Point(1 + m_flag.Width + 5, 0);
                #endregion Downloads and stores locally .png image files
            }
            catch
            {
            }

            
            //this.m_lbCountry = new Label(country.ToUpper());
            //this.m_lbCountry.Size = new Size(35, this.Height);
            //this.m_lbCountry.Location = new Point(this.Width - this.m_lbCountry.Width - 20, 0);
            //this.m_lbCountry.Font = new Rafa.Gui.Drawing.NanoFont(Themes.ThemeBase.Theme.ThemeFont, 8f);
            //this.m_lbCountry.BGColor = Color.FromArgb(0, 0, 0, 0);
            //this.m_lbCountry.FGColor = Color.Black;
            //this.m_lbCountry.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
            //{
            //    this.onSelectMe(mea);
            //};
            //this.m_lbCountry.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    Parent.Parent.OnMouseWheel(e);
            //};
            //this.Widgets.Add(this.m_lbCountry);
            //this.m_lbCountry.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.m_lbCountry.MouseLeaveEvent += delegate { this.BGColor = Color.White; };

            //this.m_lbCity = new Label(this.m_sceneName);
            //this.m_lbCity.Size = this.Size;
            //this.m_lbCity.Location = pLabel;
            //this.m_lbCity.Font = new Rafa.Gui.Drawing.NanoFont(Themes.ThemeBase.Theme.ThemeFont, 8f);
            //this.m_lbCity.BGColor = Color.FromArgb(0, 0, 0, 0);
            //this.m_lbCity.FGColor = Color.Black;
            //this.m_lbCity.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
            //{
            //    this.onSelectMe(mea);
            //};
            //this.m_lbCity.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    Parent.Parent.OnMouseWheel(e);
            //};
            //this.Widgets.Add(m_lbCity);

            //this.m_lbCity.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.m_lbCity.MouseLeaveEvent += delegate { this.BGColor = Color.White; };




            //this.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.MouseLeaveEvent += delegate { this.BGColor = Color.White; };
            #endregion

            this.OnFileSizesCollected += delegate
            {
                this.m_checkingForUpdates = false;
                //this.m_lbCity.Text = this.City;
            };


            new System.Threading.Tasks.Task(() => getFileSizesAsync()).Start();

            #endregion Creates Widgets for (Business Logic)
        }


        private void getFileSizesAsync()
        {
            this.m_checkingForUpdates = true;
            this.m_availabityRequested = true;

            if (!String.IsNullOrEmpty(Package.ModelsFile) &&
               !String.IsNullOrEmpty(Package.TexturesFile) &&
               !String.IsNullOrEmpty(Package.MaterialsFile) &&
               !String.IsNullOrEmpty(Package.SceneFile))
            {
                try
                {
                    //long texSize = File.Exists(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //       Package.TexturesFile)) ? FileSizeUtil.GetFileSize(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //       Package.TexturesFile)) : 0L;
                    //long modSize = File.Exists(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.ModelsFile)) ? FileSizeUtil.GetFileSize(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.ModelsFile)) : 0L;
                    //long matSize = File.Exists(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.MaterialsFile)) ? FileSizeUtil.GetFileSize(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.MaterialsFile)) : 0L;
                    //long xmlSize = File.Exists(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.SceneFile)) ? FileSizeUtil.GetFileSize(Path.Combine(DirectoryUtil.Documents_MySoci_Cache,
                    //    Package.SceneFile)) : 0L;

                    //if (Program.Offline && texSize > 1 && modSize > 1 && matSize > 1 && xmlSize > 1)
                    //{
                    //    DownloadingData = false;
                    //    m_ready = true;
                    //    return;
                    //}

                    //string website = Program.WebsiteUrl;

                    //if (!website.EndsWith("/"))
                    //    website += "/";

                    //m_modelCacheUri = new Uri(string.Format(assetUriPath, website, this.Package.ModelsFile));
                    //m_textureCacheUri = new Uri(string.Format(assetUriPath, website, this.Package.TexturesFile));
                    //m_materialCacheUri = new Uri(string.Format(assetUriPath, website, this.Package.MaterialsFile));
                    //m_sceneXmlUri = new Uri(string.Format(assetUriPath, website, this.Package.SceneFile));

                    //m_modelCacheSize = FileSizeUtil.GetFileSize(m_modelCacheUri);
                    //m_textureCacheSize = FileSizeUtil.GetFileSize(m_textureCacheUri);
                    //m_materialCacheSize = FileSizeUtil.GetFileSize(m_materialCacheUri);
                    //m_sceneXmlSize = FileSizeUtil.GetFileSize(m_sceneXmlUri);

                    //if (m_modelCacheSize > 0 && m_textureCacheSize > 0 && m_materialCacheSize > 0 && m_sceneXmlSize > 0)
                    //{
                    //    m_cityUnderDevelopment = false;
                    //    m_availableDownload = true;
                    //}

                    //m_ready = (texSize == m_textureCacheSize &&
                    //                        modSize == m_modelCacheSize &&
                    //                        matSize == m_materialCacheSize &&
                    //                        xmlSize == m_sceneXmlSize);

                    //if (OnFileSizesCollected != null)
                    //    this.OnFileSizesCollected();

                }
                catch (System.Net.WebException hE)
                {
                    ConsoleUtil.errorlog(string.Format("Scene Item {0} check for updates ", this.SceneName), string.Format("{0}\n{1}", hE.Message, hE.StackTrace));

                    m_availableDownload = false;
                    m_cityUnderDevelopment = true;

                    if (OnFileSizesCollected != null)
                        this.OnFileSizesCollected();

                }
                catch (Exception fE)
                {
                    ConsoleUtil.errorlog(string.Format("Scene Item {0} check for updates ", this.SceneName), string.Format("{0}\n{1}", fE.Message, fE.StackTrace));

                    m_availableDownload = false;
                    m_cityUnderDevelopment = true;
                    if (OnFileSizesCollected != null)
                        this.OnFileSizesCollected();
                }

            }
            else
            {
                m_cityUnderDevelopment = true;
                m_availableDownload = false;

                if (OnFileSizesCollected != null)
                    this.OnFileSizesCollected();
            }
        }

        private void onSelectMe(MouseEventArgs mea)
        {
            throw new NotImplementedException();
            //if (!checkDiskSpace()) return;
            //((SceneList)Parent.Parent).SelectedItem = this;

            //if (!DownloadingData && !Ready)
            //{
            //    if (m_availableDownload)
            //    {
            //        this.DownloadSize = (m_textureCacheSize + m_modelCacheSize + m_materialCacheSize + m_sceneXmlSize) / 1024f / 1024f;
            //        this.ExtractedSize = (m_textureCacheSize + m_modelCacheSize + m_materialCacheSize + m_sceneXmlSize) / 1024f / 1024f;

            //        ((ScreenUI)(((WindowsGame)(WindowsGame.Instance)).Screen2D)).TopBar.SceneListDropDown.ShowDownloadSceneDialog(this);
            //    }
            //}

            string tmpTexturesDir = Path.Combine(DirectoryUtil.Documents,
                                                 NthDimension.Rendering.Configuration.GameSettings.TextureTempFolder);

            string tmpMaterialsDir = Path.Combine(DirectoryUtil.Documents,
                                                  NthDimension.Rendering.Configuration.GameSettings.MaterialTempFolder);

            if (!Directory.Exists(tmpTexturesDir))
                Directory.CreateDirectory(tmpTexturesDir);


            if (!Directory.Exists(tmpMaterialsDir))
                Directory.CreateDirectory(tmpMaterialsDir);

            //this.OnMouseClick(mea);
        }
        private void rebindHandlers()
        {
            throw new NotImplementedException();
            //this.m_flag.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
            //{
            //    this.onSelectMe(mea);
            //};
            //this.m_flag.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    Parent.Parent.OnMouseWheel(e);
            //};
            //this.m_flag.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.m_flag.MouseLeaveEvent += delegate { this.BGColor = Color.White; };

            //this.m_lbCountry.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
            //{
            //    this.onSelectMe(mea);
            //};
            //this.m_lbCountry.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    Parent.Parent.OnMouseWheel(e);
            //};
            //this.m_lbCountry.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.m_lbCountry.MouseLeaveEvent += delegate { this.BGColor = Color.White; };

            //this.m_lbCity.MouseClickEvent += delegate (object sender, MouseEventArgs mea)
            //{
            //    this.onSelectMe(mea);
            //};
            //this.m_lbCity.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    Parent.Parent.OnMouseWheel(e);
            //};
            //this.m_lbCity.MouseEnterEvent += delegate { this.BGColor = Themes.ThemeBase.Theme.TopBarBGColorStart; };
            //this.m_lbCity.MouseLeaveEvent += delegate { this.BGColor = Color.White; };
        }

        private void displayDownloadDialog()
        {

        }

        

        //protected override void DoPaint(PaintEventArgs e)
        //{
        //    if (this.m_flag.PictureIndex != -1)
        //        this.Flag = this.m_flag.PictureIndex;

        //    if (!Ready)
        //    {
        //        //e.GC.FillRectangle(new SolidBrush(Color.FromArgb(128, 32, 32, 32)), this.ClientRect);
        //        m_lbCity.Text = City;
        //        m_lbCity.FGColor = m_lbCountry.FGColor = Color.DarkGray;
        //    }
        //    else
        //    {
        //        m_lbCity.Text = City;
        //        m_lbCity.FGColor = m_lbCountry.FGColor = Color.Black;
        //    }

        //    if (DownloadingData && !m_checkingForUpdates)
        //    {
        //        string dcity = string.Format("{0} - Downloading {1}%", City, (int)DownloadProgress);
        //        m_lbCity.Text = dcity;
        //    }
        //    else if (!DownloadingData && m_checkingForUpdates)
        //    {
        //        this.m_lbCity.Text = string.Format("Updating {0}...", this.City);
        //    }
        //    else
        //        if (m_lbCity.Text != City)
        //        m_lbCity.Text = City;



        //    base.DoPaint(e);
        //}

        //protected override void OnPaintBackground(GContext gc)
        //{

        //    base.OnPaintBackground(gc);

        //    if (DownloadingData)
        //    {
        //        float pWidth = (DownloadProgress * (this.Width - ((SceneList)Parent.Parent).ScrollbarWidth)) / 100;
        //        gc.FillRectangle(new SolidBrush(Color.FromArgb(255, 100, 134, 165)), new Rectangle(0, 0, (int)pWidth, Height));
        //    }
        //}

        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    rebindHandlers();
        //}

        #region Threads
        public void downloadModels()
        {
            //modelFileDownloader = new FileDownloader();
            //modelFileDownloader.DownloadProgressChanged += delegate (object sender, DownloadFileProgressChangedArgs args)
            //{
            //    m_progressModelCache = args.ProgressPercentage;

            //    if (args.TotalBytesToReceive != m_modelCacheSize)
            //        ConsoleUtil.log(string.Format("(!) {0} Size Mismatch http:{1} downloader{2}", Package.ModelsFile, m_modelCacheSize, args.TotalBytesToReceive));
            //};
            //modelFileDownloader.DownloadFileCompleted += delegate (object sender, DownloadFileCompletedArgs eventArgs)
            //{
            //    m_modelCacheDownloadSuccess = eventArgs.State == CompletedState.Succeeded;

            //    if (!m_modelCacheDownloadSuccess)
            //        File.Delete(Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.ModelsFile));
            //};

            //try
            //{
            //    modelFileDownloader.DownloadFileAsync(m_modelCacheUri,
            //        Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.ModelsFile));
            //}
            //catch (Exception pE)
            //{
            //    ConsoleUtil.errorlog(string.Format("Downloading {0} ", this.Package.ModelsFile), pE.Message);
            //}
        }
        public void downloadTextures()
        {
            //textureFileDownloader = new FileDownloader();
            //textureFileDownloader.DownloadProgressChanged += delegate (object sender, DownloadFileProgressChangedArgs args)
            //{
            //    m_progressTextureCache = args.ProgressPercentage;

            //    if (args.TotalBytesToReceive != m_textureCacheSize)
            //        ConsoleUtil.log(string.Format("(!) {0} Size Mismatch http:{1} downloader{2}", Package.TexturesFile, m_textureCacheSize, args.TotalBytesToReceive));
            //};
            //textureFileDownloader.DownloadFileCompleted += delegate (object sender, DownloadFileCompletedArgs eventArgs)
            //{
            //    m_textureCacheDownloadSuccess = eventArgs.State == CompletedState.Succeeded;

            //    if (!m_textureCacheDownloadSuccess)
            //        File.Delete(Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.TexturesFile));
            //};
            //try
            //{
            //    textureFileDownloader.DownloadFileAsync(m_textureCacheUri, Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.TexturesFile));
            //}
            //catch (Exception pE)
            //{
            //    ConsoleUtil.errorlog(string.Format("Downloading {0} ", this.Package.TexturesFile), pE.Message);
            //}
        }
        public void downloadMaterials()
        {
            //materialFileDownloader = new FileDownloader();
            //materialFileDownloader.DownloadProgressChanged += delegate (object sender, DownloadFileProgressChangedArgs args)
            //{
            //    m_progressMaterialCache = args.ProgressPercentage;

            //    if (args.TotalBytesToReceive != m_materialCacheSize)
            //        ConsoleUtil.log(string.Format("(!) {0} Size Mismatch http:{1} downloader{2}", Package.MaterialsFile, m_materialCacheSize, args.TotalBytesToReceive));

            //};
            //materialFileDownloader.DownloadFileCompleted += delegate (object sender, DownloadFileCompletedArgs eventArgs)
            //{
            //    m_materialCacheDownloadSuccess = eventArgs.State == CompletedState.Succeeded;

            //    if (!m_materialCacheDownloadSuccess)
            //        File.Delete(Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.MaterialsFile));
            //};
            //try
            //{
            //    materialFileDownloader.DownloadFileAsync(m_materialCacheUri, Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.MaterialsFile));
            //}
            //catch (Exception pE)
            //{
            //    ConsoleUtil.errorlog(string.Format("Downloading {0} ", this.Package.MaterialsFile), pE.Message);
            //}
        }
        public void downloadSceneXml()
        {
            //sceneFileDownloader = new FileDownloader();
            //sceneFileDownloader.DownloadProgressChanged += delegate (object sender, DownloadFileProgressChangedArgs args)
            //{
            //    m_progressXmlScene = args.ProgressPercentage;

            //    if (args.TotalBytesToReceive != m_sceneXmlSize)
            //        ConsoleUtil.log(string.Format("(!) {0} Size Mismatch http:{1} downloader{2}", Package.SceneFile, m_sceneXmlSize, args.TotalBytesToReceive));
            //};
            //sceneFileDownloader.DownloadFileCompleted += delegate (object sender, DownloadFileCompletedArgs eventArgs)
            //{
            //    m_sceneXmlDownloadSuccess = eventArgs.State == CompletedState.Succeeded;

            //    if (!m_sceneXmlDownloadSuccess)
            //        File.Delete(Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.SceneFile));
            //};
            //try
            //{
            //    sceneFileDownloader.DownloadFileAsync(m_sceneXmlUri, Path.Combine(DirectoryUtil.Documents_MySoci_Cache, Package.SceneFile));
            //}
            //catch (Exception pE)
            //{
            //    ConsoleUtil.errorlog(string.Format("Downloading {0} ", this.Package.SceneFile), pE.Message);
            //}
        }
        #endregion

        private bool checkDiskSpace()
        {
            //string driveLetter = Path.GetPathRoot(DirectoryUtil.Documents_MySoci_Cache);
            //long downloadSize = m_modelCacheSize +
            //                    m_textureCacheSize +
            //                    m_materialCacheSize +
            //                    m_sceneXmlSize;
            //long freeSpace = HardwareMonitor.GetTotalFreeSpace(driveLetter);
            //if (downloadSize > freeSpace)
            //{
            //    string smsg = string.Format("{0} MB free disk space required on {1} drive for {2}",
            //        downloadSize / 1024 / 1024,
            //        driveLetter.ToUpper(),
            //        this.City);

            //    m_simpleDialog = new DialogSimple("Not enough disk space", smsg, 10f, 10f);

            //    m_simpleDialog.Size = new Size((int)(m_simpleDialog.CaptionTextSize.Width * 1.2f), 100);

            //    m_simpleDialog.Show(((WindowsGame)WindowsGame.Instance).Screen2D);
            //    return false;
            //}
            return true;
        }
    }
}
#endregion