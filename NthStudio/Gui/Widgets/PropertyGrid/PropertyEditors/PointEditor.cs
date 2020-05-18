using System;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using NthDimension.Forms;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors
{
    [IsDefaultPropertyEditorOf(typeof(Point))]
    public partial class PointEditor : PropertyEditorBase
    {
        #region Variables
        Point _currentValue;
        Point _lastValue;
        Point _lastCommittedValue;
        BoundPropertyDescriptor _propertyDescriptor;
        IValidator _validator = Validators.DefaultValidator.CreateFor(typeof(int));
        bool _xIsValid;
        bool _yIsValid;
        Color _errorForeColor = Color.Black;
        Color _errorBackColor = Color.Salmon;
        bool _suppressEvents;
        #endregion

        #region Properties
        [Browsable(false)]
        public override BoundPropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _propertyDescriptor;
            }
            set
            {
                if (_propertyDescriptor != value)
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

        #region Constructor
        public PointEditor()
        {
            InitializeComponent();

            _propertyGridItemAssigned += _pointEditor_propertyGridItemAssigned;
        }
        #endregion

        #region Methods
        public override void RefreshProperty()
        {
            if (!_propertyDescriptor.IsEmpty)
            {
                _currentValue =
                    (Point)_propertyDescriptor.PropertyDescriptor.GetValue(
                        _propertyDescriptor.PropertyOwner
                    );

                _lastValue = _currentValue;
                _lastCommittedValue = _currentValue;

                _suppressEvents = true;

                _xTextBox.Text = _currentValue.X.ToString();
                _yTextBox.Text = _currentValue.Y.ToString();

                _suppressEvents = false;
            }
            else
            {
                _currentValue = Point.Empty;
                _lastValue = Point.Empty;
                _lastCommittedValue = Point.Empty;

                _xTextBox.Text = "";
                _yTextBox.Text = "";
            }
        }
        #endregion

        #region Event Handlers
        void _pointEditor_propertyGridItemAssigned(PropertyGridItem gridItem)
        {
            gridItem.IsSelectedChanged += _gridItem_IsSelectedChanged;

            BGColor = gridItem.Owner.Owner.SectionBackColor;
        }

        void _gridItem_IsSelectedChanged(object sender, EventArgs e)
        {
            if (PropertyGridItem.IsSelected)
            {
                if (!_yTextBox.IsFocused)
                {
                    _xTextBox.Focus();
                }
            }
        }

        void _xTextBox_TextChanged(object sender, EventArgs e)
        {
            Object value = _validator.ValidateValue(_xTextBox.Text);

            if (value == null)
            {
                _xIsValid = false;
                _xTextBox.FGColor = ErrorForeColor;
                _xTextBox.BGColor = ErrorBackColor;
                _xLabel.BGColor = ErrorBackColor;
            }
            else
            {
                _xIsValid = true;
                _xTextBox.FGColor = Color.FromKnownColor(KnownColor.WindowText);
                _xTextBox.BGColor = Color.FromKnownColor(KnownColor.Window);
                _xLabel.BGColor = Color.FromKnownColor(KnownColor.Window);

                int newX = (int)value;

                if (newX != _currentValue.X)
                {
                    _lastValue = _currentValue;
                    _currentValue.X = newX;

                    if (!_suppressEvents)
                    {
                        _raisePropertyChangingEvent(
                            new PropertyChangeEventArgs(_lastValue, _currentValue)
                            );
                    }
                }
            }
        }

        void _yTextBox_TextChanged(object sender, EventArgs e)
        {
            Object value = _validator.ValidateValue(_yTextBox.Text);

            if (value == null)
            {
                _yIsValid = false;
                _yTextBox.FGColor = ErrorForeColor;
                _yTextBox.BGColor = ErrorBackColor;
                _yLabel.BGColor = ErrorBackColor;
            }
            else
            {
                _yIsValid = true;
                _yTextBox.FGColor = Color.FromKnownColor(KnownColor.WindowText);
                _yTextBox.BGColor = Color.FromKnownColor(KnownColor.Window);
                _yLabel.BGColor = Color.FromKnownColor(KnownColor.Window);

                int newY = (int)value;

                if (newY != _currentValue.Y)
                {
                    _lastValue = _currentValue;
                    _currentValue.Y = newY;

                    if (!_suppressEvents)
                    {
                        _raisePropertyChangingEvent(
                            new PropertyChangeEventArgs(_lastValue, _currentValue)
                            );
                    }
                }
            }
        }

        void _pointEditor_Leave(object sender, EventArgs e)
        {
            if (_xIsValid && _yIsValid)
            {
                _commit();
            }
            else
            {
                _revert();
            }
        }

        void _xTextBox_KeyDown(object sender/*, KeyEventArgs e*/)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Escape:
            //        _revert();
            //        e.Handled = true;
            //        e.SuppressKeyPress = true;
            //        break;

            //    case Keys.Enter:
            //        if (_xIsValid && _yIsValid)
            //        {
            //            _commit();
            //            //// Go to the next editor
            //            //SendKeys.Send("{Tab}");
            //        }
            //        else
            //        {
            //            System.Media.SystemSounds.Beep.Play();
            //        }

            //        e.Handled = true;
            //        e.SuppressKeyPress = true;
            //        break;
            //}
        }

        void _yTextBox_KeyDown(object sender/*, KeyEventArgs e*/)
        {
            //_xTextBox_KeyDown(sender, e);
        }

        void _xLabel_Click(object sender, EventArgs e)
        {
            _xTextBox.Focus();
            //_xTextBox.SelectAll();
        }

        void _yLabel_Click(object sender, EventArgs e)
        {
            _yTextBox.Focus();
            //_yTextBox.SelectAll();
        }

        void PointEditor_Click(object sender, EventArgs e)
        {
            _xTextBox.Focus();
            //_xTextBox.SelectAll();
        }
        #endregion

        #region Private Methods
        void _commit()
        {
            //Debug.Assert(_xIsValid && _yIsValid);

            if (_currentValue != _lastCommittedValue)
            {
                _raisePropertyChangeCommittedEvent(
                    new PropertyChangeEventArgs(_lastCommittedValue, _currentValue)
                    );

                _lastCommittedValue = _currentValue;
                _lastValue = _currentValue;
            }
        }

        void _revert()
        {
            if (_currentValue != _lastCommittedValue)
            {
                _raisePropertyChangeRevertedEvent(
                    new PropertyChangeRevertedEventArgs(_lastCommittedValue)
                    );

                _currentValue = _lastCommittedValue;
                _lastValue = _lastCommittedValue;

                _suppressEvents = true;

                _xTextBox.Text = _currentValue.X.ToString();
                _yTextBox.Text = _currentValue.Y.ToString();

                _suppressEvents = false;
            }
        }
        #endregion



        // WinForms
        private Label _xLabel;
        private Label _yLabel;
        private Label _xTextBox;
        private Label _yTextBox;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            

            this._xLabel            = new Label("X");
            this._yLabel            = new Label("Y");
            this._xTextBox          = new Label();
            this._yTextBox          = new Label();
            //this.SuspendLayout();
            //// 
            //// _xLabel
            //// 
            //
            _xLabel.BGColor = System.Drawing.SystemColors.Window;
            //this._xLabel.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
            _xLabel.Location = new Point(0, 0);
            _xLabel.Name = "_xLabel";
            _xLabel.Size = new System.Drawing.Size(15, 19);
            //this._xLabel.TabIndex = 0;
            _xLabel.Text = "X";
            _xLabel.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            _xLabel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            this._xLabel.MouseClickEvent += (this._xLabel_Click);

            
            _xTextBox.Location = new System.Drawing.Point(_xLabel.X + _xLabel.Width + 1, 0);
            //this._xTextBox.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
            _xTextBox.Name = "_xTextBox";
            _xTextBox.Size = new System.Drawing.Size(15, 19);
            _xTextBox.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            _xTextBox.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //this._xTextBox.TabIndex = 2;
            //this._xTextBox.WordWrap = false;
            //_xTextBox.TextChanged += new System.EventHandler(this._xTextBox_TextChanged);
            //this._xTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._xTextBox_KeyDown);


            //// 
            //// _yLabel
            //// 
            //
            _yLabel.BGColor = System.Drawing.SystemColors.Window;
            //this._yLabel.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
            _yLabel.Location = new System.Drawing.Point(_xTextBox.X + _xTextBox.Width + 1, 0);
            _yLabel.Name = "_yLabel";
            _yLabel.Size = new System.Drawing.Size(15, 19);
            //this._yLabel.TabIndex = 1;
            _yLabel.Text = "Y";
            _yLabel.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            _yLabel.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            _yLabel.MouseClickEvent += (this._yLabel_Click);

            //// 
            //// _yTextBox
            //// 
            //this._yTextBox.AcceptsReturn = true;
            
            //this._yTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            _yTextBox.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            //this._yTextBox.Font = new NanoFont(NanoFont.DefaultRegular, 8f);
            _yTextBox.Location = new System.Drawing.Point(_yLabel.X + _yLabel.Width + 1, 0);
            _yTextBox.Name = "_yTextBox";
            _yTextBox.Size = new System.Drawing.Size(15, 19);
            _yTextBox.Anchor = EAnchorStyle.Left | EAnchorStyle.Top | EAnchorStyle.Right;
            //this._yTextBox.TabIndex = 3;
            //this._yTextBox.WordWrap = false;
            //this._yTextBox.TextChanged += new System.EventHandler(this._yTextBox_TextChanged);
            //this._yTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._yTextBox_KeyDown);
            //// 
            //// PointEditor
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Widgets.Add(this._yTextBox);
            this.Widgets.Add(this._xTextBox);
            this.Widgets.Add(this._yLabel);
            this.Widgets.Add(this._xLabel);
            //this.MaximumSize = new System.Drawing.Size(10000, 39);
            //this.MinimumSize = new System.Drawing.Size(0, 39);
            //this.Name = "PointEditor";
            
            MouseClickEvent += (this.PointEditor_Click);
            //this.Leave += new System.EventHandler(this._pointEditor_Leave);
            //this.ResumeLayout(false);
            //this.PerformLayout();

        }
    }
}
