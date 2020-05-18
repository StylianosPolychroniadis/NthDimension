using System;
using System.ComponentModel;
using System.Collections;
using NthDimension.Forms;

namespace NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors
{
    [IsDefaultPropertyEditorOf(typeof(bool))]
    public partial class BoolEditor : PropertyEditorBase
    {
        #region BoolSource class
        class BoolSource : CustomSourceListEditor.IItemSource
        {
            object[] _values = new object[] { false, true };

            public event EventHandler SourceChanged
            {
                add { }
                remove { }
            }

            public IEnumerable Items
            {
                get { return _values; }
            }
        }
        #endregion

        #region Variables
        static BoolSource _boolSource = new BoolSource();
        #endregion

        #region Properties
        [Browsable(false)]
        public override BoundPropertyDescriptor PropertyDescriptor
        {
            get { return _customSourceListEditor.PropertyDescriptor; }
            set
            {
                _customSourceListEditor.PropertyDescriptor = value;
            }
        }
        #endregion

        #region Constructor
        public BoolEditor()
        {
            InitializeComponent();
            _customSourceListEditor.ItemSource = _boolSource;

            _propertyGridItemAssigned += new Action<PropertyGridItem>(_handlePropertyGridItemAssigned);
        }
        #endregion

        #region Methods
        public override void RefreshProperty()
        {
            _customSourceListEditor.RefreshProperty();
        }
        #endregion

        #region Event Handlers
        void _customSourceListEditor_PropertyChangeCommitted(object sender, PropertyChangeEventArgs e)
        {
            _raisePropertyChangeCommittedEvent(e);
        }

        void _customSourceListEditor_PropertyChangeReverted(object sender, PropertyChangeRevertedEventArgs e)
        {
            _raisePropertyChangeRevertedEvent(e);
        }

        void _customSourceListEditor_PropertyChanging(object sender, PropertyChangeEventArgs e)
        {
            _raisePropertyChangingEvent(e);
        }

        void _customSourceListEditor_PropertyDescriptorChanged(object sender, PropertyDescriptorChangedEventArgs e)
        {
            _raisePropertyDescriptorChangedEvent(e);
        }

        void _customSourceListEditor_SelectedItemRemovedFromSource(object sender, ItemEventArgs e)
        {
            throw new Exception("This should not happen.");
        }

        void _handlePropertyGridItemAssigned(PropertyGridItem gridItem)
        {
            gridItem.IsSelectedChanged += new EventHandler(_gridItem_IsSelectedChanged);
        }

        void _gridItem_IsSelectedChanged(object sender, EventArgs e)
        {
            if (PropertyGridItem.IsSelected)
            {
                _customSourceListEditor.Focus();
            }
        }
        #endregion



        // WinForms
        private CustomSourceListEditor _customSourceListEditor;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _customSourceListEditor = new CustomSourceListEditor();
            //this.SuspendLayout();
            //// 
            //// _customSourceListEditor
            //// 
            _customSourceListEditor.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //this._customSourceListEditor.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _customSourceListEditor.ItemNameProvider = null;
            _customSourceListEditor.ItemSource = null;
            _customSourceListEditor.Location = new System.Drawing.Point(0, 0);
            _customSourceListEditor.Name = "_customSourceListEditor";
            _customSourceListEditor.Size = new System.Drawing.Size(15, 19);
            //this._customSourceListEditor.TabIndex = 0;
            _customSourceListEditor.PropertyChanging += new System.EventHandler<PropertyChangeEventArgs>(this._customSourceListEditor_PropertyChanging);
            _customSourceListEditor.SelectedItemRemovedFromSource += new System.EventHandler<PropertyEditors.ItemEventArgs>(this._customSourceListEditor_SelectedItemRemovedFromSource);
            _customSourceListEditor.PropertyChangeCommitted += new System.EventHandler<PropertyChangeEventArgs>(this._customSourceListEditor_PropertyChangeCommitted);
            _customSourceListEditor.PropertyDescriptorChanged += new System.EventHandler<PropertyDescriptorChangedEventArgs>(this._customSourceListEditor_PropertyDescriptorChanged);
            _customSourceListEditor.PropertyChangeReverted += new System.EventHandler<PropertyChangeRevertedEventArgs>(this._customSourceListEditor_PropertyChangeReverted);
            //// 
            //// BoolEditor
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Widgets.Add(this._customSourceListEditor);
            //this.DoubleBuffered = true;
            //this.Name = "BoolEditor";
            //this.Size = new System.Drawing.Size(256, 19);
            //this.ResumeLayout(false);

        }

    }
}
