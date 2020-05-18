using System;
using System.Collections.Generic;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public interface IPropertyGridSectionCollection : IEnumerable<IPropertyGridSection>
    {
        IPropertyGridSection this[String name] { get; }
        IPropertyGridSection this[int index] { get; }

        IPropertyGridSection Add(String name);
        void Remove(String name);
        void Clear();

        int Count { get; }
    }
}
