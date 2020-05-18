using System;
using System.Diagnostics;

namespace NthStudio.Gui.Widgets.PropertyGrid.Validators
{
    public class RangedValueValidator<T> : IValidator where T : IComparable<T>
    {

        readonly T _min;
        readonly T _max;
        readonly IValidator _validator;
        String _message = "";

        public string Message
        {
            get { return _message; }
        }

        public Object DefaultValue
        {
            get { return _min; }
        }

        public Type ValidatedType
        {
            get { return _validator.ValidatedType; }
        }


        public RangedValueValidator(T min, T max)
        {
            _min = min;
            _max = max;
            _validator = DefaultValidator.CreateFor(typeof(T));
        }

        public RangedValueValidator(T min, T max, IValidator wrappedValidator)
        {
            //Debug.Assert(wrappedValidator != null);

            _min = min;
            _max = max;
            _validator = wrappedValidator;
        }


        public Object ValidateValue(Object o)
        {
            _message = "Invalid value";

            Object value = _validator.ValidateValue(o);

            if (value == null)
                return null;

            if (!(value is T))
                return null;

            T casted = (T)value;

            if (casted.CompareTo(_min) >= 0 && casted.CompareTo(_max) <= 0)
            {
                _message = "";
                return value;
            }
            else
            {
                _message = "Value " + o.ToString() + " is out of range [ "
                    + _min.ToString() + " : " + _max.ToString() + " ]";

                return null;
            }
        }

        public U ConvertTo<U>(Object o) where U : class
        {
            return _validator.ConvertTo<U>(o);
        }
        
    }
}
