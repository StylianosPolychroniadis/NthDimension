using System;
using System.Collections.Generic;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public interface IPropertyGridItemCollection : IEnumerable<PropertyGridItem>
    {
        PropertyGridItem this[String name] { get; }
        PropertyGridItem this[int index] { get; }

        PropertyGridItem Add(String name, PropertyEditorBase propertyEditor);
        void Remove(String name);
        void Clear();

        int Count { get; }
    }
}
