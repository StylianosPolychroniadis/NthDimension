namespace NthStudio.Gui.Widgets.PropertyGrid
{
    using System;
    public interface IPropertyGridSection
    {
        String SectionName { get; }
        IPropertyGridItemCollection Items { get; }
        PropertyGrid Owner { get; }
    }
}
