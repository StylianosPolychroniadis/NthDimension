using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using NthDimension.Forms.Widgets;
using NthDimension.Forms;
using NthDimension.Forms.Events;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public class ColorChangedEventArgs : EventArgs
    {
        public readonly Color OldColor;
        public readonly Color NewColor;

        public ColorChangedEventArgs(Color oldColor, Color newColor)
        {
            OldColor = oldColor;
            NewColor = newColor;
        }
    }

    public partial class PropertyGrid : Widget
    {
        List<IPropertyGridSection> pendingSections = new List<IPropertyGridSection>();

        #region SectionCollection struct
        struct SectionCollection : IPropertyGridSectionCollection
        {
            PropertyGrid _owner;

            public SectionCollection(PropertyGrid owner)
            {
                _owner = owner;
            }

            public IPropertyGridSection this[string name]
            {
                get
                {
                    return _owner._sections.Find(
                        delegate (PropertyGridSection section)
                        {
                            return section.SectionName == name;
                        }
                        );
                }
            }

            public IPropertyGridSection this[int index]
            {
                get
                {
                    return _owner._sections[index];
                }
            }

            public IPropertyGridSection Add(String name)
            {
                return _owner._addSection(name);
            }

            public void Remove(String name)
            {
                _owner._removeSection(name);
            }

            public void Clear()
            {
                _owner._clearSections();
            }

            public int Count
            {
                get { return _owner._sections.Count; }
            }

            public IEnumerator<IPropertyGridSection> GetEnumerator()
            {
                foreach (PropertyGridSection section in _owner._sections)
                {
                    yield return section;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _owner._sections.GetEnumerator();
            }
        }
        #endregion

        #region Variables
        List<PropertyGridSection> _sections = new List<PropertyGridSection>();
        Color _sectionBackColor = Color.FromArgb(233, 236, 250);
        Color _errorForeColor = Color.Black;
        Color _errorBackColor = Color.Salmon;
        bool _updatingSelection;
        #endregion

        #region Events
        public event EventHandler<ColorChangedEventArgs> SectionBackColorChanged;
        public event EventHandler<ColorChangedEventArgs> ErrorForeColorChanged;
        public event EventHandler<ColorChangedEventArgs> ErrorBackColorChanged;
        #endregion

        #region Properties
        [Browsable(true)]
        [Description("The background color of all contained sections.")]
        [Category("Appearance")]
        public Color SectionBackColor
        {
            get { return _sectionBackColor; }
            set
            {
                if (_sectionBackColor != value)
                {
                    Color oldColor = _sectionBackColor;
                    _sectionBackColor = value;

                    if (SectionBackColorChanged != null)
                    {
                        SectionBackColorChanged(this,
                            new ColorChangedEventArgs(oldColor, value)
                            );
                    }
                }
            }
        }

        [Browsable(true)]
        [Description("The background color of PropertyEditors when a invalid value is encountered.")]
        [Category("Appearance")]
        public Color ErrorBackColor
        {
            get { return _errorBackColor; }
            set
            {
                if (_errorBackColor != value)
                {
                    Color oldColor = _errorBackColor;
                    _errorBackColor = value;

                    if (ErrorBackColorChanged != null)
                    {
                        ErrorBackColorChanged(this,
                            new ColorChangedEventArgs(oldColor, value)
                            );
                    }
                }
            }
        }

        [Browsable(true)]
        [Description("The text color of PropertyEditors when a invalid value is encountered.")]
        [Category("Appearance")]
        public Color ErrorForeColor
        {
            get { return _errorForeColor; }
            set
            {
                if (_errorForeColor != value)
                {
                    Color oldColor = _errorForeColor;
                    _errorForeColor = value;

                    if (ErrorForeColorChanged != null)
                    {
                        ErrorForeColorChanged(this,
                            new ColorChangedEventArgs(oldColor, value)
                            );
                    }
                }
            }
        }

        [Browsable(false)]
        public IPropertyGridSectionCollection Items
        {
            get { return new SectionCollection(this); }
        }
        #endregion

        #region Constructor
        public PropertyGrid()
        {
            InitializeComponent();
        }
        #endregion

        #region EventHandler
        void _section_SizeChanged(object sender, EventArgs e)
        {
            _updateSectionPositions();
        }

        void _section_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!_updatingSelection)
            {
                _updatingSelection = true;

                _helpTextTitleLabel.Text = "";
                _helpTextLabel.Text = "";

                PropertyGridSection selectedSection = (PropertyGridSection)sender;

                foreach (PropertyGridSection section in _sections)
                {
                    if (selectedSection != section)
                    {
                        section.ResetSelectedItem();
                    }
                    else
                    {
                        PropertyGridItem selectedItem = section.SelectedItem;

                        if (selectedItem != null)
                        {
                            if (selectedItem.Description != null)
                            {
                                _helpTextTitleLabel.Text = selectedItem.Name;
                                _helpTextLabel.Text = selectedItem.Description;
                            }

                            _scrollToGridItem(selectedItem);
                        }
                    }
                }

                _updatingSelection = false;
            }
        }

        void _section_SplitterMoving(object sender/*, SplitterCancelEventArgs e*/)
        {
            foreach (PropertyGridSection section in _sections)
            {
                //section.SplitterDistance = e.SplitX;
            }
        }

        void _splitContainer_Panel1_SizeChanged(object sender, EventArgs e)
        {
            _updateSectionPositions();
        }

        private void _sectionPanelScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _sectionPanel.Location = new Point(_sectionPanel.X, -e.NewValue);
        }
       
        #endregion

        #region Private Methods
        PropertyGridSection _addSection(String sectionName)
        {
            //Debug.Assert(Items[sectionName] == null);

            PropertyGridSection section = new PropertyGridSection();
            section.SectionName = sectionName;

            int y = _calculateSectionsHeight();

            _sections.Add(section);

            section.Size = new Size(this.Width, 20); // ADDED by hand

            _sectionPanel.Widgets.Add(section);

            section.Location = new Point(0, y);
            
            section.Size = new Size(_sectionPanel.Width, section.Size.Height); // NOTE:: Height has not been properly assigned
            section.Anchor = EAnchorStyle.Left | EAnchorStyle.Right | EAnchorStyle.Top;
            //section.SizeChanged += new EventHandler(_section_SizeChanged);
            //section.SplitterMoving += new SplitterCancelEventHandler(_section_SplitterMoving);
            section.SelectedItemChanged += new EventHandler(_section_SelectedItemChanged);
            section.Owner = this;

            _updateSectionPositions();

            return section;
        }

        void _removeSection(String name)
        {
            PropertyGridSection section = (PropertyGridSection)Items[name];

            //Debug.Assert(section != null && _sections.Contains(section));

            //section.SizeChanged -= _section_SizeChanged;
            
            _sections.Remove(section);

            section.Dispose();

            _updateSectionPositions();
        }

        void _clearSections()
        {
            while (_sections.Count != 0)
            {
                _removeSection(_sections[_sections.Count - 1].SectionName);
            }
        }

        void _updateSectionPositions()
        {
            int height = _calculateSectionsHeight();

            if (_sectionPanel.Height != height)
            {
                _sectionPanel.Size = new Size(_sectionPanel.Size.Width, height); // Note: Width has not been properly set
            }

            if (_sectionPanel.Height > _splitContainer.Panel0.Height)
            {
                if (!_sectionPanelScrollBar.IsVisible)
                {
                    _sectionPanelScrollBar.Show();
                    _sectionPanelScrollBar.Enabled = true;
                }

                if (_sectionPanel.Width != _splitContainer.Panel0.Width - _sectionPanelScrollBar.Width)
                {                    
                    _sectionPanel.Size = new Size(_splitContainer.Panel0.Width - _sectionPanelScrollBar.Width, _sectionPanel.Size.Height); // Note Height has not been properly set
                }

                int max = _sectionPanel.Height;
                int largeChange = _splitContainer.Panel0.Height;

                _sectionPanelScrollBar.Minimum = 0;
                _sectionPanelScrollBar.Maximum = max;
                _sectionPanelScrollBar.LargeChange = largeChange;
                _sectionPanelScrollBar.SmallChange = 1;

                if (_sectionPanelScrollBar.Value > max - largeChange + 1)
                {
                    _sectionPanelScrollBar.Value = max - largeChange + 1;
                }

                if (_sectionPanel.Top != -_sectionPanelScrollBar.Value)
                {
                    _sectionPanel.Location = new Point(_sectionPanel.X, -_sectionPanelScrollBar.Value);
                }
            }
            else
            {
                if (_sectionPanelScrollBar.IsVisible)
                {
                    _sectionPanelScrollBar.Hide();
                    _sectionPanelScrollBar.Enabled = false;
                }

                if (_sectionPanel.Width != _splitContainer.Panel0.Width)
                {
                    _sectionPanel.Size = new Size(_splitContainer.Panel0.Width, _sectionPanel.Height); // NOTE Height has not been properly set
                }

                if (_sectionPanel.Top != 0)
                {                    
                    _sectionPanel.Location = new Point(_sectionPanel.X, 0); // Note X has not been propely set
                }
            }

            int y = 0;

            foreach (PropertyGridSection section in _sections)
            {
                if (section.Top != y)
                {
                    section.Location = new Point(section.X, y); // Note X has not been properly set
                }

                if (section.Width != _sectionPanel.Width)
                {                    
                    section.Size = new Size(_sectionPanel.Width, section.Height); // NOTE Height has not been properly set
                }

                y += section.Height;
            }
        }

        void _scrollToGridItem(PropertyGridItem item)
        {

            Rectangle displayRect =
                    //_splitContainer.Panel1.RectangleToScreen(                 // NOTE Not using PointToScreen
                    new Rectangle(
                        0,
                        0,
                        _splitContainer.Panel1.Width,
                        _splitContainer.Panel1.Height
                        );
            //);

            Rectangle editorPanelRect =
                    //item.EditorPanel.RectangleToScreen(                   // NOTE Not using PointToScreen
                    new Rectangle(Point.Empty, item.EditorPanel.Size);
                    //);

            if (editorPanelRect.Height >= displayRect.Height)
                return;

            if (editorPanelRect.Bottom > displayRect.Bottom)
            {
                int offset =
                    (displayRect.Bottom - editorPanelRect.Height) -
                    editorPanelRect.Top;

                _sectionPanelScrollBar.Value -= offset;
                
                int newY = _sectionPanel.Y + offset;
                _sectionPanel.Location = new Point(_sectionPanel.X, newY); // Note X not properly set
            }
            else
            if (editorPanelRect.Top < displayRect.Top)
            {
                int offset = displayRect.Top - editorPanelRect.Top;

                _sectionPanelScrollBar.Value -= offset;
                int newY = _sectionPanel.Y + offset;
                _sectionPanel.Location = new Point(_sectionPanel.X, newY); // Note X not properly set
            }
        }

        int _calculateSectionsHeight()
        {
            int h = 0;

            foreach (PropertyGridSection section in _sections)
            {
                h += section.Height;
            }

            return h;
        }
        #endregion

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            _sectionPanelScrollBar.Minimum = 0;
            _sectionPanelScrollBar.Maximum = _sectionPanel.Widgets.Count * 20;
            _sectionPanelScrollBar.SmallChange = 10;
            _sectionPanelScrollBar.LargeChange = 100;

            if (!_sectionPanelScrollBar.IsHide)
            {
                Rectangle corner = new Rectangle(this.Width - 20, _sectionPanelScrollBar.Bottom,
                    _sectionPanelScrollBar.Width, this.Height);
                if (e.ClipRect.IntersectsWith(corner))
                    e.GC.FillRectangle(new SolidBrush(BGColor),
                        corner);
            }

            base.DoPaint(e);


        }

        //WINFORMS
        private SplitterBox _splitContainer;
        private Label _helpTextLabel;
        private Label _helpTextTitleLabel;
        
        private Panel _sectionPanel;
        //private Widget _sectionPanelCanvas;
        private ScrollBarV _sectionPanelScrollBar;
        private bool m_init = false;
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //SuspendLayout();
            this._splitContainer = new SplitterBox(ESplitterType.VerticalScroll);            
            this._sectionPanelScrollBar = new ScrollBarV();
            this._sectionPanel = new Panel();
            //this._sectionPanelCanvas = new Widget();
            this._helpTextLabel = new Label();
            this._helpTextTitleLabel = new Label();

            _sectionPanelScrollBar.Size = new Size(0, 13);
            _sectionPanelScrollBar.ThumbColor = Color.FromArgb(255, 220, 220, 220);
            _sectionPanelScrollBar.Dock = EDocking.Right;
            _sectionPanelScrollBar.Scroll += _sectionPanelScrollBar_Scroll;
            _sectionPanelScrollBar.Name = "_sectionPanelScrollBar";

            _sectionPanel.Location = new Point(0,0);
            _sectionPanel.Size = new Size(200, 200);
            _sectionPanel.Anchor = EAnchorStyle.Top;
            _sectionPanel.Dock = EDocking.Fill;

            //_sectionPanelCanvas.Location = new Point(0, 0);
            //_sectionPanelCanvas.Size = new Size(200, 200);
            //_sectionPanelCanvas.Anchor = EAnchorStyle.Top;
            //_sectionPanelCanvas.Dock = EDocking.Fill;
            //_sectionPanelCanvas.Widgets.Add(_sectionPanel);

            _splitContainer.Size = new Size(this.Width, this.Height);
            _splitContainer.Anchor = EAnchorStyle.Top | EAnchorStyle.Bottom | EAnchorStyle.Left | EAnchorStyle.Right;
            _splitContainer.Location = new Point(0, 0);
            _splitContainer.SplitterBarLocation = 0.5f;

            _splitContainer.Panel0.Widgets.Add(_sectionPanelScrollBar);
            _splitContainer.Panel0.Widgets.Add(_sectionPanel);
            

            _helpTextTitleLabel = new Label("PropertyName");
            _helpTextTitleLabel.Size = new Size(this.Width, 20);
            _helpTextTitleLabel.Location = new Point(0, 0);
            _helpTextTitleLabel.Anchor = EAnchorStyle.All;

            _helpTextLabel = new Label("Help Text");
            _helpTextLabel.Size = new Size(this.Width, 20);
            _helpTextLabel.Location = new Point(0, 20);
            _helpTextLabel.Anchor = EAnchorStyle.All;

            _splitContainer.Panel1.Widgets.Add(_helpTextTitleLabel);
            _splitContainer.Panel1.Widgets.Add(_helpTextLabel);
            

            this.Widgets.Add(_splitContainer);
            //ResumeLayout();

            m_init = true;
        }

        
    }
}
