using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using NthDimension.Forms;

namespace NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors
{
    public class ItemEventArgs : EventArgs
    {
        public readonly Object Item;

        public ItemEventArgs(Object item)
        {
            Item = item;
        }
    }

    public partial class CustomSourceListEditor : PropertyEditorBase
    {

        public interface IItemSource
        {
            event EventHandler SourceChanged;
            IEnumerable Items { get; }
        }



        public delegate String ItemNameProviderDelegate(Object item);

        public event EventHandler<ItemEventArgs> SelectedItemRemovedFromSource;



        IItemSource _itemSource;
        ItemNameProviderDelegate _nameProvider;
        List<Object> _items = new List<Object>();
        BoundPropertyDescriptor _propertyDescriptor;
        Object _lastCommittedValue;
        Object _lastValue;
        bool _suppressEvents;


        [Browsable(false)]
        public IItemSource ItemSource
        {
            get { return _itemSource; }
            set
            {
                if (_itemSource != null)
                {
                    _itemSource.SourceChanged -= _updateItemList;
                }

                _itemSource = value;

                if (_itemSource != null)
                {
                    _itemSource.SourceChanged += _updateItemList;
                }

                _updateItemList(null, EventArgs.Empty);

                if (_propertyDescriptor.PropertyOwner != null)
                {
                    //Debug.Assert(_propertyDescriptor.PropertyDescriptor != null);
                    _selectItem(
                        _propertyDescriptor.PropertyDescriptor.GetValue(
                            _propertyDescriptor.PropertyOwner)
                            );
                }
            }
        }

        [Browsable(false)]
        public ItemNameProviderDelegate ItemNameProvider
        {
            get { return _nameProvider; }
            set
            {
                _nameProvider = value;
                _updateItemList(null, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public override BoundPropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _propertyDescriptor;
            }
            set
            {
                if (value != _propertyDescriptor)
                {
                    _propertyDescriptor = value;
                    RefreshProperty();

                    _raisePropertyDescriptorChangedEvent(
                        new PropertyDescriptorChangedEventArgs(value)
                    );
                }
            }
        }

        public CustomSourceListEditor()
        {
            InitializeComponent();
        }

        public override void RefreshProperty()
        {
            _suppressEvents = true;
            if (!_propertyDescriptor.IsEmpty)
            {
                _lastCommittedValue =
                    _propertyDescriptor.PropertyDescriptor.GetValue(
                        _propertyDescriptor.PropertyOwner
                        );

                _lastValue = _lastCommittedValue;


                _selectItem(_lastCommittedValue);

                //Debug.Assert(_comboBox.SelectedIndex >= 0);
            }
            else
            {
                _comboBox.SelectedIndex = -1;
            }
            _suppressEvents = false;
        }

        void _comboBox_DrawItem(object sender /*, DrawItemEventArgs e*/)
        {
            //if (_comboBox.Items.Count == 0)
            //    return;

            //if ((e.State & DrawItemState.ComboBoxEdit) == 0)
            //{
            //    if (e.Index < 0)
            //        return;

            //    e.DrawBackground();

            //    using (SolidBrush brush = new SolidBrush(e.ForeColor))
            //    {
            //        e.Graphics.DrawString(_comboBox.Items[e.Index].ToString(),
            //            e.Font, brush, 0, e.Index * _comboBox.ItemHeight);
            //    }

            //    e.DrawFocusRectangle();
            //}
            //else
            //{
            //    if (_comboBox.SelectedItem == null)
            //        return;

            //    e.DrawBackground();

            //    using (SolidBrush brush = new SolidBrush(e.ForeColor))
            //    {
            //        e.Graphics.DrawString(_comboBox.SelectedItem.ToString(),
            //            e.Font, brush, e.Bounds);
            //    }

            //    e.DrawFocusRectangle();
            //}
        }

        void _textLabel_MouseEnter(object sender, EventArgs e)
        {
            //_textLabel.Visible = false;
            _textLabel.Hide();
        }

        void _comboBox_MouseLeave(object sender, EventArgs e)
        {
            if (!_comboBox.IsFocused)
            {
                //_textLabel.Visible = true;
                _textLabel.Show();
            }
        }

        void _comboBox_DropDownClosed(object sender, EventArgs e)
        {
            _revertValue();
        }

        void _comboBox_Enter(object sender, EventArgs e)
        {
            //_textLabel.Visible = false;
            _textLabel.Hide();
        }

        void _comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBox.SelectedIndex >= 0)
            {
                _textLabel.Text = _items[_comboBox.SelectedIndex].ToString();

                if (_lastValue.Equals(_items[_comboBox.SelectedIndex]))
                    return;

                if (!_suppressEvents)
                {
                    _raisePropertyChangingEvent(
                        new PropertyChangeEventArgs(
                            _lastValue,
                            _items[_comboBox.SelectedIndex]
                            )
                        );
                }

                _lastValue = _items[_comboBox.SelectedIndex];
            }
            else
            {
                _textLabel.Text = "";
            }
        }

        void _comboBox_Leave(object sender, EventArgs e)
        {
            if (!_textLabel.IsFocused)
            {
                //_textLabel.Visible = true;
                _textLabel.Show();
            }
        }

        void _comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _commitValue();
        }

        void _comboBox_KeyDown(object sender/*, KeyEventArgs e*/)
        {
            //if (e.KeyCode == Keys.Enter)
            {
                //SendKeys.Send("{Tab}");
            }
        }


        void _updateItemList(Object sender, EventArgs e)
        {
            Object selectedItem = _comboBox.SelectedIndex >= 0 ?
                _items[_comboBox.SelectedIndex] : null;

            //_comboBox.Items.Clear();

            if (_itemSource != null)
            {
                _items.Clear();
                foreach (Object item in _itemSource.Items)
                {
                    _items.Add(item);
                }

                if (_nameProvider != null)
                {
                    foreach (Object item in _items)
                    {
                        //_comboBox.Items.Add(_nameProvider(item));
                    }
                }
                else
                {
                    foreach (Object item in _items)
                    {
                        //_comboBox.Items.Add(item.ToString());
                    }
                }
            }

            if (selectedItem == null)
                return;

            bool selectedItemContainedNow = false;

            foreach (Object item in _items)
            {
                if (item.Equals(selectedItem))
                {
                    selectedItemContainedNow = true;
                }
            }

            if (!selectedItemContainedNow)
            {
                if (SelectedItemRemovedFromSource != null)
                {
                    SelectedItemRemovedFromSource(this,
                        new ItemEventArgs(selectedItem)
                        );
                }

                if (_comboBox.Widgets.Count > 0)
                {
                    _comboBox.SelectedIndex = 0;
                }
                else
                {
                    _comboBox.SelectedIndex = -1;
                }
            }
            else
            {
                _selectItem(selectedItem);
            }
        }

        void _selectItem(Object item)
        {
            int index = _items.FindIndex(delegate (Object o) { return o.Equals(item); });
            _comboBox.SelectedIndex = index;
        }

        void _commitValue()
        {
            if (!_lastCommittedValue.Equals(_items[_comboBox.SelectedIndex]))
            {
                if (!_suppressEvents)
                {
                    _raisePropertyChangeCommittedEvent(
                        new PropertyChangeEventArgs(
                            _lastCommittedValue,
                            _items[_comboBox.SelectedIndex]
                            )
                        );
                }

                _lastCommittedValue = _items[_comboBox.SelectedIndex];
                _lastValue = _lastCommittedValue;
            }
            else
            {
                if (!_lastCommittedValue.Equals(_lastValue))
                {
                    _revertValue();
                }
            }
        }

        void _revertValue()
        {
            if (!_lastCommittedValue.Equals(_items[_comboBox.SelectedIndex]) ||
                 !_lastValue.Equals(_items[_comboBox.SelectedIndex]))
            {
                if (!_suppressEvents)
                {
                    _raisePropertyChangeRevertedEvent(
                        new PropertyChangeRevertedEventArgs(_lastCommittedValue)
                        );
                }

                _lastValue = _lastCommittedValue;

                _selectItem(_lastCommittedValue);
            }
        }






        // WinForms

        Label _textLabel;
        ComboBox _comboBox;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _comboBox = new ComboBox();
            _textLabel = new Label();
            //this.SuspendLayout();
            //// 
            //// _comboBox
            //// 
            _comboBox.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //this._comboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            //this._comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //this._comboBox.Font = new System.Drawing.Font("Segoe UI", 8F);
            //this._comboBox.FormattingEnabled = true;
            //this._comboBox.ItemHeight = 13;
            _comboBox.Location = new System.Drawing.Point(0, 0);
            _comboBox.Name = "_comboBox";
            _comboBox.Size = new System.Drawing.Size(15, 19);
            //this._comboBox.TabIndex = 0;
            //_comboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this._comboBox_DrawItem);
            //_comboBox.SelectionChangeCommitted += new System.EventHandler(this._comboBox_SelectionChangeCommitted);
            //this._comboBox.SelectedIndexChanged += new System.EventHandler(this._comboBox_SelectedIndexChanged);
            //this._comboBox.Leave += new System.EventHandler(this._comboBox_Leave);
            //this._comboBox.Enter += new System.EventHandler(this._comboBox_Enter);
            //this._comboBox.MouseLeave += new System.EventHandler(this._comboBox_MouseLeave);
            //this._comboBox.DropDownClosed += new System.EventHandler(this._comboBox_DropDownClosed);
            //this._comboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._comboBox_KeyDown);
            //// 
            //// _textLabel
            //// 
            _textLabel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            
            this._textLabel.BGColor = System.Drawing.SystemColors.Window;
            //this._textLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this._textLabel.Location = new System.Drawing.Point(0, 0);
            this._textLabel.Name = "_textLabel";
            this._textLabel.Padding = new NthDimension.Forms.Layout.Spacing(3, 2, 0, 0);
            this._textLabel.Size = new System.Drawing.Size(15, 19);
            //this._textLabel.TabIndex = 1;
            _textLabel.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV; 
            //this._textLabel.UseCompatibleTextRendering = true;
            this._textLabel.MouseEnterEvent += (this._textLabel_MouseEnter);
            //// 
            //// CustomSourceListEditor
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Widgets.Add(this._textLabel);
            this.Widgets.Add(this._comboBox);
            //this.DoubleBuffered = true;
            //this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.Name = "CustomSourceListEditor";
            //this.Size = new System.Drawing.Size(256, 19);
            //this.ResumeLayout(false);

        }

    }
}
