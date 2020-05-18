//using NthDimension.Forms;
//using NthStudio.Plugins;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NthStudio.Gui
//{
//    public partial class PluginStoreRefreshDialog : Window
//    {
//        public PluginStoreRefreshDialog()
//        {
//            InitializeComponent();
//            PluginStoreManager.OnPluginStoreRefreshStarting += new PluginStoreRefreshOnStartHandler(PluginStoreManager_OnPluginStoreRefreshStarting);
//            PluginStoreManager.OnPluginStoreRefreshProgress += new PluginStoreRefreshHandler(PluginStoreManager_OnPluginStoreRefreshProgress);
//            PluginStoreManager.OnPluginStoreRefreshCompleted += new PluginStoreRefreshOnCompleteHandler(PluginStoreManager_OnPluginStoreRefreshCompleted);
//        }

//        private void InitializeComponent()
//        {

//        }

//        public delegate void Action();
//        public delegate void PluginStoreUpdateAction(PluginStoreRefreshProgressEventArgs e);

//        public void Update(PluginStoreRefreshProgressEventArgs e)
//        {
//            if (IsVisible)
//            {
//                //progressBar.Maximum = e.Total;
//                //progressBar.Value = e.Current;
//            }

//            if (IsVisible == true && e.Current >= 100)
//                //if (this.InvokeRequired)
//                //    this.Invoke(new MethodInvoker(delegate () { this.Text = "Done"; }));
//                //else
//                    this.Title = "Done";
//        }

//        void PluginStoreManager_OnPluginStoreRefreshCompleted()
//        {
//            //if (this.InvokeRequired)
//            //{
//            //    this.Invoke(new Action(this.Close));
//            //}
//            //else
//            {
//                this.Hide();
//            }
//        }

//        void PluginStoreManager_OnPluginStoreRefreshProgress(PluginStoreRefreshProgressEventArgs e)
//        {
//            //if (this.InvokeRequired)
//            //    this.Invoke(new PluginStoreUpdateAction(this.Update), new object[] { e });
//            //else
//                this.Update(e);
//        }

//        void PluginStoreManager_OnPluginStoreRefreshStarting()
//        {
//        }

//        public PluginStore RefreshPluginStore(/*IWin32Window*/ WHUD parent)
//        {
//            this.Show(parent);
//            return PluginStoreManager.RefreshPuginStore();
//        }

//        //protected override void OnClosing(CancelEventArgs e)
//        //{
//        //    PluginStoreManager.OnPluginStoreRefreshStarting -= new PluginStoreRefreshOnStartHandler(PluginStoreManager_OnPluginStoreRefreshStarting);
//        //    PluginStoreManager.OnPluginStoreRefreshProgress -= new PluginStoreRefreshHandler(PluginStoreManager_OnPluginStoreRefreshProgress);
//        //    PluginStoreManager.OnPluginStoreRefreshCompleted -= new PluginStoreRefreshOnCompleteHandler(PluginStoreManager_OnPluginStoreRefreshCompleted);
//        //    base.OnClosing(e);
//        //}
//    }
//}
