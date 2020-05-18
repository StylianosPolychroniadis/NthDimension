using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using NthDimension.Forms;
using NthDimension.Forms.Widgets;
using NthDimension.Forms.Events;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    internal partial class PropertyGridSection : Widget, IPropertyGridSection
    {
        const int DEFAULT_ITEM_HEIGHT = 20;

        #region ItemEx class
        struct ItemEx  
        {
            public ItemEx(PropertyGridItem item, Label nameLabel)
            {
                Item = item;
                NameLabel = nameLabel;
            }

            public readonly PropertyGridItem Item;
            public readonly Label NameLabel;
        }
        #endregion

        #region ItemCollection struct
        struct ItemCollection : IPropertyGridItemCollection
        {
            PropertyGridSection _owner;

            public ItemCollection(PropertyGridSection owner)
            {
                _owner = owner;
            }

            public PropertyGridItem this[string name]
            {
                get
                {
                    int index = _owner._items.FindIndex(
                        delegate (ItemEx item)
                        {
                            return item.Item.Name == name;
                        }
                    );

                    return index >= 0 ? _owner._items[index].Item : null;
                }
            }

            public PropertyGridItem this[int index]
            {
                get { return _owner._items[index].Item; }
            }

            public PropertyGridItem Add(String name, PropertyEditorBase propertyEditor)
            {
                return _owner._addItem(name, propertyEditor);
            }

            public void Remove(String name)
            {
                PropertyGridItem item = this[name];
                _owner._removeItem(item);
            }

            public void Clear()
            {
                _owner._clearItems();
            }

            public IEnumerator<PropertyGridItem> GetEnumerator()
            {
                foreach (ItemEx itemEx in _owner._items)
                {
                    yield return itemEx.Item;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (ItemEx itemEx in _owner._items)
                {
                    yield return itemEx.Item;
                }
            }

            public int Count
            {
                get { return _owner._items.Count; }
            }
        }
        #endregion

        #region Variables
        bool _isExpanded = true;
        List<ItemEx> _items = new List<ItemEx>();
        PropertyGridItem _selectedItem;
        bool _splitterMoving;
        PropertyGrid _owner;
        bool _updatingSelectedItem;
        #endregion

        #region Events
        //internal event SplitterCancelEventHandler SplitterMoving;
        internal event EventHandler SelectedItemChanged;
        internal event EventHandler ExpandStateChanged;
        #endregion

        #region Constructor
        internal PropertyGridSection()
        {
            InitializeComponent();

            //Disposed += delegate { if (Disposing) _clearItems(); };
        }
        #endregion

        #region Properties
        public String SectionName
        {
            get { return _titelLabel.Text; }
            set { _titelLabel.Text = value; }
        }

        //public int SplitterDistance
        //{
        //    get { return _splitContainer.SplitterBarLocation ; }// .SplitterDistance; }
        //    set
        //    {
        //        if (!_splitterMoving)
        //        {
        //            _splitContainer.SplitterDistance = value;
        //        }
        //    }
        //}

        public IPropertyGridItemCollection Items
        {
            get { return new ItemCollection(this); }
        }

        public PropertyGrid Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != null)
                {
                    _owner.SectionBackColorChanged -= _owner_SectionBackColorChanged;
                }

                _owner = value;

                if (_owner != null)
                {
                    _owner.SectionBackColorChanged += _owner_SectionBackColorChanged;
                }

                _updateSectionBackColor();
            }
        }

        internal PropertyGridItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            private set
            {
                if (value != _selectedItem && !_updatingSelectedItem)
                {
                    _updatingSelectedItem = true;
                    //Debug.Assert(value == null ||
                        //_items.FindAll(
                        //    delegate (ItemEx item)
                        //    {
                        //        return item.Item == value;
                        //    }
                        //    ).Count == 1
                        //);

                    if (_selectedItem != null)
                    {
                        _selectedItem.IsSelected = false;
                    }

                    _selectedItem = value;

                    if (_selectedItem != null)
                    {
                        _selectedItem.IsSelected = true;
                    }

                    _updateLabelSelection();

                    if (SelectedItemChanged != null)
                    {
                        SelectedItemChanged(this, EventArgs.Empty);
                    }
                    _updatingSelectedItem = false;
                }
            }
        }
        #endregion

        #region Internal Methods
        internal void ResetSelectedItem()
        {
            SelectedItem = null;
        }
        #endregion

        #region EventHandler
        void _expandButton_Click(object sender, EventArgs e)
        {
            _isExpanded = !_isExpanded;

            _updateExpandState();

            if (ExpandStateChanged != null)
            {
                ExpandStateChanged(this, EventArgs.Empty);
            }

            SelectedItem = null;
        }

        void _nameTextBox_Click(object sender, EventArgs e)
        {
            PropertyGridItem item =
                _items.Find(
                    delegate (ItemEx itemEx)
                    {
                        return itemEx.NameLabel == sender;
                    }
                    ).Item;

            SelectedItem = item;
        }

        void _titelLabel_Click(object sender, EventArgs e)
        {
            SelectedItem = null;
        }

        void _splitContainer_SplitterMoving(object sender /*, SplitterCancelEventArgs e*/)
        {
            _splitterMoving = true;

            //if (SplitterMoving != null)
            {
                //SplitterMoving(sender, e);
            }
        }

        void _splitContainer_SplitterMoved(object sender /*, SplitterEventArgs e*/)
        {
            _splitterMoving = false;
        }

        void _owner_SectionBackColorChanged(object sender, ColorChangedEventArgs e)
        {
            _updateSectionBackColor();
        }

        void _editorPanel_SizeChanged(object sender, EventArgs e)
        {
            _updatePositions();
            _updateExpandState();
        }
        #endregion

        #region Private Methods
        PropertyGridItem _addItem(String name, PropertyEditorBase propertyEditor)
        {
            //Debug.Assert(Items[Name] == null);
            //Debug.Assert(!String.IsNullOrEmpty(name));
            //Debug.Assert(propertyEditor != null);

            int y = _calculateItemHeight();

            Label nameLabel = new Label();

            nameLabel.Location = new Point(0, y);
            nameLabel.Size = new Size(_splitContainer.Panel1.Width, DEFAULT_ITEM_HEIGHT - 1);
            nameLabel.Text = name;
            nameLabel.Anchor =  EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            nameLabel.BGColor = Color.White;
            nameLabel.TextAlign = ETextAlignment.Top | ETextAlignment.Left;
            nameLabel.MouseClickEvent += (_nameTextBox_Click);
            nameLabel.Font = NanoFont.DefaultRegular;
            //nameLabel.ShowBoundsLines = true;
            

            _splitContainer.Panel0.Widgets.Add(nameLabel);

            ToolTip nameLabelToolTip = new ToolTip();
            //nameLabelToolTip.SetToolTip(nameLabel, nameLabel.Text);

            Panel editorPanel = new Panel();

            editorPanel.Location = new Point(0, y);
            editorPanel.Size = new Size(_splitContainer.Panel1.Width, DEFAULT_ITEM_HEIGHT - 1);
            editorPanel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            editorPanel.BGColor = Color.White;
            editorPanel.Name = "editorPanel";
            //editorPanel.ShowBoundsLines = true;
            

            _splitContainer.Panel1.Widgets.Add(editorPanel);

            PropertyGridItem item = new PropertyGridItem(editorPanel, nameLabel, propertyEditor);

            item.Owner = this;

            propertyEditor.Size = new Size(600, 19);
            editorPanel.Widgets.Add(propertyEditor);

            propertyEditor.PropertyGridItem = item;

            propertyEditor.MouseEnterEvent += delegate (Object sender, EventArgs e)
            {
                SelectedItem = item;
            };

            propertyEditor.MouseLeaveEvent += delegate (Object sender, EventArgs e)
            {
                SelectedItem = null;
            };

            _items.Add(new ItemEx(item, nameLabel));

            _updateExpandState();

            return item;
        }

        void _removeItem(PropertyGridItem item)
        {
            //Debug.Assert(Items[item.Name] != null);

            int index = _items.FindIndex(
                delegate (ItemEx itemEx) { return item == itemEx.Item; }
                );

            _splitContainer.Panel0.Widgets.Remove(_items[index].NameLabel);
            _items[index].NameLabel.Dispose();

            _splitContainer.Panel1.Widgets.Remove(item.EditorPanel);
            item.EditorPanel.Dispose();
            item.Dispose();

            _items.RemoveAt(index);

            _updatePositions();
            _updateExpandState();

            if (item == SelectedItem)
            {
                SelectedItem = null;
            }
        }

        void _clearItems()
        {
            while (_items.Count != 0)
            {
                _removeItem(_items[_items.Count - 1].Item);
            }
        }

        void _updateExpandState()
        {
            if (_isExpanded)
            {
                
                Size = new Size(Size.Width, _calculateExpandedHeight());
                _expandButton.Text = "-"; //_expandButton.Image = Platinum.Properties.Resources.minus;                
            }
            else
            {
                Size = new Size(Size.Width, _titelLabel.Height);
                _expandButton.Text = "+"; //_expandButton.Image = Platinum.Properties.Resources.plus;
            }
        }

        int _calculateExpandedHeight()
        {
            return _titelLabel.Height + _calculateItemHeight();
        }

        int _calculateItemHeight()
        {
            int h = 0;

            foreach (ItemEx item in _items)
            {
                h += item.Item.EditorPanel.Height + 1;
            }

            return h;
        }

        void _updateLabelSelection()
        {
            foreach (ItemEx item in _items)
            {
                if (item.Item == _selectedItem)
                {
                    item.NameLabel.BGColor = Color.FromKnownColor(KnownColor.MenuHighlight);
                    item.NameLabel.FGColor = Color.FromKnownColor(KnownColor.HighlightText);
                }
                else
                {
                    item.NameLabel.BGColor = Color.White;
                    item.NameLabel.FGColor = Color.Black;
                }
            }
        }

        void _updatePositions()
        {
            int y = 0;

            foreach (ItemEx item in _items)
            {
                item.NameLabel.Location             = new Point(item.NameLabel.Location.X,  y);
                item.Item.EditorPanel.Location      = new Point(item.Item.EditorPanel.Location.X, y);

                y += item.Item.EditorPanel.Height + 1;
            }
        }

        void _updateSectionBackColor()
        {
            if (_owner != null)
            {
                _sidePanel.BGColor                  = Color.FromArgb(255, 96, 96, 96);//  _owner.SectionBackColor;
                _titelLabel.BGColor                 = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(250)))));//_owner.SectionBackColor;
                _splitContainer.BGColor             = _owner.SectionBackColor;
                _splitContainer.Panel0.BGColor      = _owner.SectionBackColor;
                _splitContainer.Panel1.BGColor      = _owner.SectionBackColor;
            }
        }
        #endregion

        //protected override void DoPaint(PaintEventArgs e)
        //{
           

        //    base.DoPaint(e);
        //}


        // WINFORMS
        private Label _titelLabel;
        private Panel _sidePanel;
        private SplitterBox _splitContainer;
        private Button _expandButton;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _titelLabel = new Label();
            _sidePanel = new Panel();
            _expandButton = new Button(string.Empty);
            _splitContainer = new SplitterBox(ESplitterType.HorizontalScroll);

       
            _titelLabel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            _titelLabel.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(250)))));
            //this._titelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _titelLabel.FGColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            _titelLabel.Location = new System.Drawing.Point(19, 0);
            _titelLabel.Name = "_titelLabel";
            _titelLabel.Size = new System.Drawing.Size(344, 19);
            //this._titelLabel.TabIndex = 0;
            _titelLabel.Text = "Titel";
            _titelLabel.TextAlign =  ETextAlignment.Left | ETextAlignment.CenterV;
            _titelLabel.MouseClickEvent += this._titelLabel_Click;

            _sidePanel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Bottom;
            _sidePanel.BGColor = Color.Red;//  System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(136)))), ((int)(((byte)(150)))));
            _sidePanel.Widgets.Add(this._expandButton);
            _sidePanel.Location = new System.Drawing.Point(0, 0);
            _sidePanel.Name = "_sidePanel";
            _sidePanel.Size = new System.Drawing.Size(19, 19);

            //this._expandButton.FlatAppearance.BorderSize = 0;
            //this._expandButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(250)))));
            //this._expandButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(250)))));
            //this._expandButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            //this._expandButton.Image = global::Platinum.Properties.Resources.minus;
            _expandButton.Location = new System.Drawing.Point(0, 0);
            _expandButton.Name = "_expandButton";
            _expandButton.Size = new System.Drawing.Size(19, 19);
            _expandButton.TextAlign = ETextAlignment.Center;
            _expandButton.Text = "-";
            _expandButton.MouseClickEvent += this._expandButton_Click;


            _splitContainer.Anchor = EAnchorStyle.All;
            _splitContainer.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(250)))));
            _splitContainer.Location = new System.Drawing.Point(19, 19);
            _splitContainer.Name = "_splitContainer";
            _splitContainer.Size = new System.Drawing.Size(344, 380);
            _splitContainer.SplitterBarLocation = 0.25f;
            _splitContainer.SplitterSize = 1;



            //this._splitContainer.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this._splitContainer_SplitterMoving);
            //this._splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this._splitContainer_SplitterMoved);

            this.Widgets.Add(this._splitContainer);
            this.Widgets.Add(this._sidePanel);
            this.Widgets.Add(this._titelLabel);

            this.Name = "PropertyGridSection";
        }

    }
}
