using System;
using System.Globalization;
using System.ComponentModel;

namespace NthStudio.Gui.Widgets.PropertyGrid.Validators
{
    internal class DoubleValidator : IValidator
    {

        String _message;
        readonly IFormatProvider _formatProvider;
        readonly TypeConverter _typeConverter;

        public DoubleValidator()
        {
            _message = "";
            _formatProvider = CultureInfo.GetCultureInfo("en-US").NumberFormat;
            _typeConverter = TypeDescriptor.GetConverter(typeof(double));
        }

        public Object ValidateValue(Object o)
        {
            double val;

            if (o is String)
            {
                if (double.TryParse(o.ToString(), NumberStyles.Float,
                    _formatProvider, out val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (!_typeConverter.CanConvertFrom(o.GetType()))
                    return null;

                try
                {
                    return _typeConverter.ConvertFrom(o);
                }
                catch
                {
                    return null;
                }
            }
        }

        public T ConvertTo<T>(Object o) where T : class
        {
            if (typeof(T) == typeof(String))
            {
                Object ret = (((double)o).ToString(_formatProvider));
                return (T)ret;
            }
            else
            {
                try
                {
                    return (T)_typeConverter.ConvertTo(o, typeof(T));
                }
                catch
                {
                    _message = "Could not convert " + o.ToString() + " to " + typeof(T).ToString();
                    return null;
                }
            }
        }

        public string Message
        {
            get { return _message; }
        }

        public Object DefaultValue
        {
            get { return 0.0; }
        }

        public Type ValidatedType
        {
            get { return typeof(double); }
        }

    }
}
