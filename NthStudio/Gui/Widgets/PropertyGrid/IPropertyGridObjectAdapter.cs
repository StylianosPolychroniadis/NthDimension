using System;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public interface IPropertyGridObjectAdapter
    {
        Object SelectedObject
        {
            get;
            set;
        }

        PropertyGrid TargetPropertyGrid
        {
            get;
            set;
        }
    }
}
