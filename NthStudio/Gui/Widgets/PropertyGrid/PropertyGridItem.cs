using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public class PropertyGridItem : Widget, IDisposable
    {
        #region Variables
        bool _disposed;
        bool _isSelected;
        Panel _editorPanel;
        Label _nameLabel;
        String _description;
        IPropertyGridSection _owner;
        PropertyEditorBase _propertyEditor;
        #endregion

        #region Constructor / Finalizer
        internal PropertyGridItem(Panel editorPanel, Label nameLabel, PropertyEditorBase editor)
        {
            //Debug.Assert(editorPanel != null);
            //Debug.Assert(nameLabel != null);
            //Debug.Assert(editor != null);

            _editorPanel = editorPanel;
            _nameLabel = nameLabel;
            _propertyEditor = editor;
        }

        ~PropertyGridItem()
        {
            Dispose(false);
        }
        #endregion

        #region Events
        public event EventHandler ItemDisposed;
        public event EventHandler IsSelectedChanged;
        #endregion

        #region Properties
        public String Name
        {
            get { return _nameLabel.Text; }
            set { _nameLabel.Text = value; }
        }

        public Panel EditorPanel
        {
            get { return _editorPanel; }
        }

        public PropertyEditorBase PropertyEditor
        {
            get { return _propertyEditor; }
        }

        public String Description
        {
            get { return _description; }
            internal set { _description = value; }
        }

        public IPropertyGridSection Owner
        {
            get { return _owner; }
            internal set { _owner = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            internal set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    if (IsSelectedChanged != null)
                    {
                        IsSelectedChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (ItemDisposed != null)
                    {
                        ItemDisposed(this, new EventArgs());
                        ItemDisposed = null;
                    }
                }

                _disposed = true;
            }
        }
        #endregion
    }
}
