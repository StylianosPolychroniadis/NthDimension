using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthStudio.Gui.Widgets;
using NthStudio.Gui.Widgets.PropertyGrid;
using NthStudio.Gui.Widgets.TabStrip;
using NthStudio.Gui.Widgets.TextEditor;
using System;


namespace NthStudio.Gui.Dialogs
{
    public class ServerPanel : DialogBase
    {        
        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private bool                    m_init = false;
        private SplitterBox             g_verticalSplit;

        private SplitterBox             g_horizontalSplit;
        private Panel                   g_topInfoServer;
        private Label                   g_serverLabel;
        private TextField               g_serverNameText;

        private TabStrip                g_tabStrip;
        private TabStripItem            g_tabUsers;
        private TabStripItem            g_tabFileSystem;
        private TabStripItem            g_tabStatistics;
        private TabStripItem            g_tabLog;
        private TreeView                g_treeServerBrowser;
        private TreeNode                g_rootNode;
        private TreeNode                g_treeNodeUsers;

        //class serverprops
        //{
        //    string m_name { get; set; }
        //    System.Net.IPEndPoint m_ip { get; set; }

        //    public serverprops(string name, System.Net.IPEndPoint ip)
        //    {
        //        m_name = name;
        //        m_ip = ip;
        //    }

        //    public string                       Name { get { return m_name; } }
        //    public System.Net.IPEndPoint        IP {  get { return m_ip; } }
        //}

        public ServerPanel()
        {
            this.Title = string.Format("Session Id {1}", StudioWindow.Instance.Server.ToString(), StudioWindow.Instance.Server.GetHashCode().ToString());
            g_rootNode = new TreeNode(string.Format("{0}", StudioWindow.Instance.Server.ToString()));
            this.Size = new System.Drawing.Size(550, 350);
        }

        private void InitializeComponent()
        {
            if (m_init) return;

            g_verticalSplit = new SplitterBox(ESplitterType.HorizontalScroll);
            g_verticalSplit.Dock = EDocking.Fill;
            g_verticalSplit.SplitterSize = 1;
            g_verticalSplit.SplitterBarLocation = .35f;
            this.Widgets.Add(g_verticalSplit);

            SplitterBox g_horizontalSplit = new SplitterBox(ESplitterType.VerticalScroll);
            g_horizontalSplit.Dock = EDocking.Fill;
            g_horizontalSplit.SplitterSize = 1;
            g_horizontalSplit.SplitterBarLocation = .1f /*0.35f*/;

            g_verticalSplit.Panel0.Widgets.Add(g_horizontalSplit);

            #region Top Left
            g_topInfoServer = new Panel();
            g_topInfoServer.Size = new System.Drawing.Size(200, 300);   // Requires veticalSplitter added and docked in parent            
            //g_topInfoServer.Dock = EDocking.Fill;
            g_horizontalSplit.Panel0.Widgets.Add(g_topInfoServer);
            

            System.Drawing.Size sizeTextLine =  new System.Drawing.Size(200, 25); ;
            System.Drawing.Point point = new System.Drawing.Point(0, 0);// Parent.Location;

            g_serverLabel = new Label("Server: ");
            var serv = ((StudioWindow)StudioWindow.Instance).Server;
            g_serverLabel.Text = "Offline";
          
            
            g_serverLabel.Size = sizeTextLine;
            //g_serverLabel.Location = new System.Drawing.Point(0 + point.X, 0 + point.Y);
            g_serverLabel.BGColor = System.Drawing.Color.LightGray;
            g_serverLabel.FGColor = System.Drawing.Color.OrangeRed;
            //g_serverLabel.TextColor =
            //g_serverLabel.Anchor = EAnchorStyle.All;
            g_serverLabel.Dock = EDocking.Top;
            g_topInfoServer.Widgets.Add(g_serverLabel);

            g_serverNameText = new TextField();
            g_serverNameText.Text = "localhost";
            //g_serverNameText.Anchor = EAnchorStyle.All;
            //g_serverNameText.Size = sizeTextLine;
            //g_serverNameText.Location = new System.Drawing.Point(0 + point.X, g_serverLabel.Top + g_serverLabel.Height + point.Y);
            g_serverNameText.Dock = EDocking.Top;
            g_topInfoServer.Widgets.Add(g_serverNameText);
            #endregion

            #region Bottom Left

            //PropertyGrid pg = new PropertyGrid();
            //PropertyGridItem t = new PropertyGridItem();

            Panel p = new Panel();
            p.Size = new System.Drawing.Size(300, 150);
            p.Location = new System.Drawing.Point(0, 0);

            Button bServer = new Button("Start Server");
            bServer.Size = new System.Drawing.Size(200, 25);
            bServer.Location = new System.Drawing.Point(0,50);            
            //bServer.Font.FontSize = 12;
            bServer.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //bServer.ShowBoundsLines = true;
            /*g_horizontalSplit.Panel1.Widgets.Add(bServer);*/
            p.Widgets.Add(bServer);

            //Label lbIp = new Label();
            //lbIp.Text = "Network Interface IPv4";
            //lbIp.Font.FontSize = 10;
            //lbIp.Size = new System.Drawing.Size(200, 25);
            //lbIp.BGColor = System.Drawing.Color.DarkGray;
            //lbIp.FGColor = System.Drawing.Color.White;
            //lbIp.Location = new System.Drawing.Point(0, 25);
            //lbIp.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //lbIp.TextAlign = ETextAlignment.Top | ETextAlignment.Left;
            ////lbIp.ShowBoundsLines = true;
            ///*g_horizontalSplit.Panel1.Widgets.Add(lbIp);*/
            //p.Widgets.Add(lbIp);

            Panel p_txtIp = new Panel(); p_txtIp.Size = new System.Drawing.Size(200, 20);
            TextInput txtIp = new TextInput();
            txtIp.BGColor = System.Drawing.Color.FromArgb(255, 64,64,64);
            txtIp.FGColor = System.Drawing.Color.WhiteSmoke;
            txtIp.Font.FontSize = 14;
            txtIp.SetDefaultText("IPv4 NIC");
            txtIp.Size = new System.Drawing.Size(200, 20);
            txtIp.Location = new System.Drawing.Point(0, 0);
            txtIp.Text = "IPv4 NIC";
            txtIp.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            p_txtIp.Widgets.Add(txtIp);
            p.Widgets.Add(p_txtIp);
            txtIp.GotFocusEvent += delegate { txtIp.BGColor = System.Drawing.Color.FromArgb(255, 128, 128, 128); };
            txtIp.LostFocusEvent += delegate { txtIp.BGColor = System.Drawing.Color.FromArgb(255, 64, 64, 64); };

                Panel p_txtPrt = new Panel(); p_txtPrt.Size = new System.Drawing.Size(200, 20);
            TextInput txtPrt = new TextInput();
            txtPrt.BGColor = System.Drawing.Color.FromArgb(255, 64, 64, 64);
            txtPrt.FGColor = System.Drawing.Color.WhiteSmoke;
            txtPrt.Font.FontSize = 14;
            txtPrt.SetDefaultText("TCP/UDP Port");
            txtPrt.Size = new System.Drawing.Size(200, 20);
            txtPrt.Location = new System.Drawing.Point(0, 0);
            txtPrt.Text = "35565";
            txtPrt.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;


          
            p_txtPrt.Widgets.Add(txtPrt);
            p_txtPrt.Location = new System.Drawing.Point(0, 20);            
            p.Widgets.Add(p_txtPrt);
            txtPrt.GotFocusEvent += delegate { txtIp.BGColor = System.Drawing.Color.FromArgb(255, 128, 128, 128); };
            txtPrt.LostFocusEvent += delegate { txtIp.BGColor = System.Drawing.Color.FromArgb(255, 64, 64, 64); };


            //Label lbPort = new Label();
            //lbPort.Text = "Label2";
            //lbPort.Font.FontSize = 10;
            //lbPort.Size = new System.Drawing.Size(200, 25);
            //lbPort.BGColor = System.Drawing.Color.DarkGray;
            //lbPort.FGColor = System.Drawing.Color.White;
            //lbPort.Location = new System.Drawing.Point(0, 80);
            //lbPort.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //lbPort.TextAlign = ETextAlignment.Top | ETextAlignment.Left;
            ////lbIp.ShowBoundsLines = true;
            ///*g_horizontalSplit.Panel1.Widgets.Add(lbPort);*/
            //p.Widgets.Add(lbPort);


            p.Dock = EDocking.Fill;
            g_horizontalSplit.Panel1.Widgets.Add(p);
            #endregion Bottom Left

            #region Right 
            Panel tree = new Panel();
            {
                tree.Size = new System.Drawing.Size(600, 600);

                g_tabUsers = new TabStripItem(); g_tabUsers.Title = "Details";
                g_tabLog = new TabStripItem(); g_tabLog.Title = "Events";
                g_tabStatistics = new TabStripItem(); g_tabStatistics.Title = "Statistics";
                g_tabFileSystem = new TabStripItem(); g_tabFileSystem.Title = "Files";

                g_tabStrip = new TabStrip();
                g_tabStrip.Size = g_verticalSplit.Panel1.Size;
                g_tabStrip.Items.Add(g_tabUsers);
                g_tabStrip.Items.Add(g_tabLog); 
                g_tabStrip.Items.Add(g_tabStatistics);
                g_tabStrip.Items.Add(g_tabFileSystem);
                g_tabStrip.Dock = EDocking.Fill;
                tree.Widgets.Add(g_tabStrip);

                g_treeServerBrowser = new TreeView();
                g_treeServerBrowser.Dock = EDocking.Fill;

                g_tabUsers.Widgets.Add(g_treeServerBrowser);



                TreeNode m_treeNodeScenes       = new TreeNode("Scenes");
                TreeNode m_treeNodeAvatars      = new TreeNode("Avatars");
                g_treeNodeUsers                 = new TreeNode("Users");
                TreeNode m_treeNodeUsersOnline  = new TreeNode("Online");

                g_treeNodeUsers.Nodes.Add(m_treeNodeUsersOnline);

                g_rootNode.Nodes.Add(m_treeNodeScenes);
                g_rootNode.Nodes.Add(m_treeNodeAvatars);
                g_rootNode.Nodes.Add(g_treeNodeUsers);

                g_treeServerBrowser.Nodes.Add(g_rootNode);

                tree.Dock = EDocking.Fill;
                g_verticalSplit.Panel1.Widgets.Add(tree);
            }

            Panel events = new Panel();
            {
                events.Size = new System.Drawing.Size(600,600);
                Panel toolbar = new Panel();
                toolbar.Size = new System.Drawing.Size(200, 25);
                toolbar.Dock = EDocking.Top;
                events.Widgets.Add(toolbar);
                TextEditor console = new TextEditor();
                console.Size = new System.Drawing.Size(600, 600);
                console.Font.FontSize = 8;
                console.Dock = EDocking.Fill;
                console.ShowVRuler = false;
                console.ShowHRuler = false;
                console.ShowLineNumbers = false;
                console.ShowInvalidLines = false;
                console.ShowEOLMarkers = false;
                console.IsIconBarVisible = true;
                events.Widgets.Add(console);
                events.Dock = EDocking.Fill;
                g_tabLog.Widgets.Add(events);
                console.Text += string.Format(">");
            }
            
            #endregion Right

            this.m_init = true;
        }

        protected override void DoPaint(GContext parentGContext)
        {
            

            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);
        }

    }
}
