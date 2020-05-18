using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using NthDimension.Forms;

namespace NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors
{
    /// <summary>
    /// PropertyEditor for all non-flag enums.
    /// </summary>
    [IsDefaultPropertyEditorOf(typeof(Enum))]
    public partial class EnumEditor : PropertyEditorBase
    {
        #region EnumItemSource class
        class EnumItemSource : CustomSourceListEditor.IItemSource
        {
            static Dictionary<Type, Array> _typeMap =
                new Dictionary<Type, Array>();

            Array _values;

            public EnumItemSource(Type type)
            {
                if (!_typeMap.ContainsKey(type))
                {
                    _typeMap.Add(type, Enum.GetValues(type));
                }

                _values = _typeMap[type];
            }

            public event EventHandler SourceChanged
            {
                add { }
                remove { }
            }

            public IEnumerable Items
            {
                get
                {
                    return _values;
                }
            }
        }
        #endregion

        #region Construtor
        public EnumEditor()
        {
            InitializeComponent();

            _customSourceListEditor.PropertyDescriptorChanged +=
                _customSourceListEditor_PropertyDescriptorChanged;

            _propertyGridItemAssigned += new Action<PropertyGridItem>(_handlePropertyGridItemAssigned);
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

        #region Properties
        [Browsable(false)]
        public override BoundPropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _customSourceListEditor.PropertyDescriptor;
            }
            set
            {
                if (!value.IsEmpty)
                {
                    //Debug.Assert(value.PropertyDescriptor.PropertyType.IsEnum);

                    _customSourceListEditor.ItemSource =
                        new EnumItemSource(value.PropertyDescriptor.PropertyType);

                    _customSourceListEditor.PropertyDescriptor = value;
                }
            }
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
        #endregion




        // WinForms
        private CustomSourceListEditor _customSourceListEditor;
        private void InitializeComponent()
        {
            _customSourceListEditor = new CustomSourceListEditor();
            //this.SuspendLayout();
            //// 
            //// _customSourceListEditor
            //// 
            _customSourceListEditor.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //this._customSourceListEditor.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _customSourceListEditor.ItemNameProvider = null;
            _customSourceListEditor.ItemSource = null;
            _customSourceListEditor.Location = new System.Drawing.Point(0, 0);
            _customSourceListEditor.Name = "_customSourceListEditor";
            _customSourceListEditor.Size = new System.Drawing.Size(15, 19);
            //this._customSourceListEditor.TabIndex = 0;
            _customSourceListEditor.PropertyChanging += new System.EventHandler<PropertyChangeEventArgs>(this._customSourceListEditor_PropertyChanging);
            _customSourceListEditor.PropertyChangeCommitted += new System.EventHandler<PropertyChangeEventArgs>(this._customSourceListEditor_PropertyChangeCommitted);
            _customSourceListEditor.PropertyDescriptorChanged += new System.EventHandler<PropertyDescriptorChangedEventArgs>(this._customSourceListEditor_PropertyDescriptorChanged);
            _customSourceListEditor.PropertyChangeReverted += new System.EventHandler<PropertyChangeRevertedEventArgs>(this._customSourceListEditor_PropertyChangeReverted);
            //// 
            //// EnumEditor
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Widgets.Add(this._customSourceListEditor);
            //this.Name = "EnumEditor";
            //this.Size = new System.Drawing.Size(256, 19);
            //this.ResumeLayout(false);

        }
    }
}
