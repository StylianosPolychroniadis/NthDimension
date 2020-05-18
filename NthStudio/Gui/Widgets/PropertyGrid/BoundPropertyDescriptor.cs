namespace NthStudio.Gui.Widgets.PropertyGrid
{
    using System;
    using System.ComponentModel;

    public struct BoundPropertyDescriptor
    {
        public PropertyDescriptor PropertyDescriptor;
        public Object PropertyOwner;

        public bool IsEmpty
        {
            get { return PropertyDescriptor == null || PropertyOwner == null; }
        }

        public static bool operator ==(BoundPropertyDescriptor d1,
                                       BoundPropertyDescriptor d2)
        {
            return d1.PropertyOwner == d2.PropertyOwner &&
                   d1.PropertyDescriptor == d2.PropertyDescriptor;
        }

        public static bool operator !=(BoundPropertyDescriptor d1,
                                       BoundPropertyDescriptor d2)
        {
            return !(d1 == d2);
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundPropertyDescriptor)
            {
                return this == (BoundPropertyDescriptor)obj;
            }

            return false;
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }
    }
}
