
using NthDimension.Forms;
using NthDimension.Service;

using System;
using System.Drawing;

namespace NthStudio.IoC
{
    public class BasePluginControl : Widget, IInversionApi
    {
        private int             m_originalHeight        = 30;
        private int             m_originalWidth         = 100;
        private bool            m_init                  = false;

        #region Plugin & UndoRedo Command Pattern API
        System.Collections.Generic.Dictionary<ushort, // TODO:: Replace by Type
                                              NthDimension.Rendering.ApplicationCommandRenderingHandler>
                                                _renderingCommandHandlers;

        public System.Collections.Generic.Dictionary<ushort, // TODO:: Replace by Type
                                                    NthDimension.Rendering.ApplicationCommandRenderingHandler> 
                                                        RenderingCommands
        {
            get { return _renderingCommandHandlers; }
            set { _renderingCommandHandlers = value; }
        }

        #endregion

        #region InversionApi Implementation

        private DateTime _createdOn;
        public DateTime GetCreatedOn()
        {
            return _createdOn;
        }

        public void SetErrorMessage(string msg)
        {
            m_errorMsg = msg;
        }
        public void SetError(int error)
        {
            m_error = error;
        }
        public void SetSystemMessage(string msg)
        {
            m_systemMsg = msg;
        }

        internal string m_errorMsg = string.Empty;
        public string LastErrorMessage
        {
            get
            {
                return this.m_errorMsg;
            }

        }
        private int m_error = -1;
        public int LastError
        {
            get
            {
                return this.m_error;
            }

        }
        private string m_systemMsg = string.Empty;
        public string SystemErrorMessage
        {
            get
            {
                return m_systemMsg;
            }
        }
        #endregion



        //private Label m_titleLabel = new Label();
        //private Controls.Led.LBLed m_titleButton = new LBLed();
        //private enuCollapsedState m_state = enuCollapsedState.Expanded;
        // private VerticalPanel m_parentContainer = new VerticalPanel();

        //public enum enuCollapsedState
        //{
        //    Collapsed,
        //    Expanded
        //}

        #region Ctor
        public BasePluginControl()
        {
            this._createdOn             = DateTime.Now;
            this.Size                   = new Size(m_originalWidth, m_originalHeight);

            ////Size s = this.Size;
            ////s.Height = m_titleLabel.Height;

            ////this.MinimumSize = s;

            ////if (this.Parent is VerticalPanel)
            ////    this.m_parentContainer = this.Parent as VerticalPanel;

            //this.InitializeComponent();

            ////this.Resize += new System.EventHandler(this.OnResize);
            ////this.m_titleButton.Click += new System.EventHandler(this.collapseButton_Click);

        }
        public BasePluginControl(string title)
            : this()
        {
            //m_titleLabel.Text = title;
            //// missing this.Widgets.Add(m_titleLabel); // See InitializeComponent() below
        }
        #endregion

        /// <summary>
        /// Sets the initialized flag to true. DoPaint event calls the function if initialized flag equals false. After overriding this function ALWAYS call base.InitializeComponent();
        /// </summary>
        public virtual void InitializeComponent()
        {
            this.m_init = true;
        }

        /// <summary>
        /// Performs the rendering to the display of the Widget control. 
        /// Calls InitializeComponent() if initialized flag is set to false 
        /// as a lazy loading mechanism to ensure OpenGL instantiation 
        /// before instancing the Widget UI components
        /// </summary>
        /// <param name="parentGContext"></param>
        protected override void DoPaint(GContext parentGContext)
        {

            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);
        }
    }
}
