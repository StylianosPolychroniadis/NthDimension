using System;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using NthDimension.Forms.Widgets;
using NthDimension.Forms;

namespace NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors
{
    public partial class ValidatingStringEditor : PropertyEditorBase
    {
        #region Variables
        IValidator _validator;
        BoundPropertyDescriptor _propertyDescriptor;
        bool _isCurrentTextValid;
        Object _lastValue;
        Object _currentValue;
        Object _lastCommittedValue;
        bool _suppressEvents;
        #endregion

        #region Constructor
        public ValidatingStringEditor()
        {
            InitializeComponent();

            _propertyGridItemAssigned += _handlePropertyGridItemAssigned;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public IValidator Validator
        {
            get { return _validator; }
            set
            {
                _validator = value;

                if (_validator != null)
                {
                    RefreshProperty();
                }
                else
                {
                    _currentValue = null;
                    _lastValue = null;
                    _lastCommittedValue = null;
                    _textBox.Text = String.Empty;
                }
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
                //Debug.Assert(_validator != null);

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
        #endregion

        #region Methods
        public override void RefreshProperty()
        {
            if (!_propertyDescriptor.IsEmpty)
            {
                _currentValue = _propertyDescriptor.PropertyDescriptor.GetValue(
                    _propertyDescriptor.PropertyOwner
                    );

                _suppressEvents = true;
                _textBox.Text = _validator.ConvertTo<String>(_currentValue);
                _suppressEvents = false;

                _lastValue = _currentValue;
                _lastCommittedValue = _currentValue;
            }
            else
            {
                _currentValue = null;
                _lastCommittedValue = null;
                _lastValue = null;
            }
        }
        #endregion

        #region Event Handlers
        void _textBox_KeyDown(object sender/*, KeyEventArgs e*/)
        {
            //if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            //{
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;

            //    if (_isCurrentTextValid)
            //    {
            //        _commit();
            //        //SendKeys.Send("{Tab}");
            //    }
            //    else
            //    {
            //        System.Media.SystemSounds.Beep.Play();
            //    }
            //}
            //else if (e.KeyCode == Keys.Escape)
            //{
            //    e.Handled = true;
            //    e.SuppressKeyPress = true;
            //    _revert();
            //}
            //else
            //{
            //    e.Handled = false;
            //}
        }

        void _textBox_TextChanged(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                Object value = _validator.ValidateValue(_textBox.Text);

                if (value == null)
                {
                    _textBox.FGColor = ErrorForeColor;
                    _textBox.BGColor = ErrorBackColor;
                    BGColor = ErrorBackColor;
                    _isCurrentTextValid = false;
                }
                else
                {
                    _textBox.FGColor = Color.FromKnownColor(KnownColor.ControlText);
                    _textBox.BGColor = Color.FromKnownColor(KnownColor.Window);
                    BGColor = Color.FromKnownColor(KnownColor.Window);
                    _isCurrentTextValid = true;

                    // Do not raise the CHANGING event if there is no change
                    if (_valuesEqual(_currentValue, value))
                        return;

                    _lastValue = _currentValue;
                    _currentValue = value;

                    if (!_suppressEvents)
                    {
                        _raisePropertyChangingEvent(
                            new PropertyChangeEventArgs(_lastValue, _currentValue)
                            );
                    }
                }
            }
        }

        void _textBox_Leave(object sender, EventArgs e)
        {
            if (_isCurrentTextValid)
            {
                _commit();
            }
            else
            {
                _revert();
            }
        }

        void _handlePropertyGridItemAssigned(PropertyGridItem item)
        {
            item.IsSelectedChanged += new EventHandler(_item_IsSelectedChanged);
        }

        void _item_IsSelectedChanged(object sender, EventArgs e)
        {
            if (PropertyGridItem.IsSelected)
            {
                _textBox.Focus();
            }
        }
        #endregion

        #region Private Methods
        void _commit()
        {
            //Debug.Assert(_isCurrentTextValid);

            if (!_valuesEqual(_lastCommittedValue, _currentValue))
            {
                PropertyChangeEventArgs e =
                        new PropertyChangeEventArgs(_lastCommittedValue, _currentValue);

                _raisePropertyChangeCommittedEvent(e);

                _lastCommittedValue = _currentValue;
                _lastValue = _currentValue;
            }
        }

        void _revert()
        {
            if (!_isCurrentTextValid ||
                 !_valuesEqual(_lastCommittedValue, _currentValue) ||
                 !_valuesEqual(_lastCommittedValue, _lastValue))
            {
                PropertyChangeRevertedEventArgs e =
                    new PropertyChangeRevertedEventArgs(_lastCommittedValue);

                _raisePropertyChangeRevertedEvent(e);

                _lastValue = _lastCommittedValue;
                _currentValue = _lastCommittedValue;
                _textBox.Text = _validator.ConvertTo<String>(_lastCommittedValue);
            }
        }

        static bool _valuesEqual(Object value1, Object value2)
        {
            if (value1 != null)
            {
                return value2 != null ? value1.Equals(value2) : false;
            }
            else
            {
                return value2 == null;
            }
        }
        #endregion



        // WinForms
        //private TextField _textBox;
        private Label _textBox;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _textBox = new Label();
            //this.SuspendLayout();
            //// 
            //// _textBox
            //// 
            //this._textBox.AcceptsReturn = true;
            _textBox.Anchor = EAnchorStyle.Top | EAnchorStyle.Left | EAnchorStyle.Right;
            //this._textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //this._textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            //this._textBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _textBox.Location = new System.Drawing.Point(0, 3);
            _textBox.Name = "_textBox";
            _textBox.Size = new System.Drawing.Size(15, 15);
            //this._textBox.TabIndex = 0;
            //this._textBox.Tag = "";
            //this._textBox.TextChanged += new System.EventHandler(this._textBox_TextChanged);
            //this._textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._textBox_KeyDown);
            //this._textBox.Leave += new System.EventHandler(this._textBox_Leave);
            //// 
            //// ValidatingStringEditor
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.BackColor = System.Drawing.SystemColors.Window;
            this.Widgets.Add(this._textBox);
            //this.MaximumSize = new System.Drawing.Size(10000, 19);
            //this.MinimumSize = new System.Drawing.Size(0, 19);
            //this.Name = "ValidatingStringEditor";
            //this.Size = new System.Drawing.Size(256, 19);
            //this.ResumeLayout(false);
            //this.PerformLayout();

        }
    }
}
