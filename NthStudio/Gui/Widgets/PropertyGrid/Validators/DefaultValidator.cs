using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace NthStudio.Gui.Widgets.PropertyGrid.Validators
{
    public delegate Object DefaultValueFactoryDelegate();

    public class DefaultValidator : IValidator
    {
        static Dictionary<Type, Type> _defaultValidators;
        static Dictionary<Type, DefaultValueFactoryDelegate> _defaultValueFactories;

        TypeConverter _typeConverter;
        String _message = "";
        Type _validatedType;
        public string Message
        {
            get { return _message; }
        }

        public Type ValidatedType
        {
            get { return _validatedType; }
        }

        public Object DefaultValue
        {
            get
            {
                DefaultValueFactoryDelegate factory;

                if (_defaultValueFactories.TryGetValue(_validatedType, out factory))
                {
                    return factory();
                }

                return TypeDescriptor.CreateInstance(null, _validatedType, null, null);
            }
        }

        DefaultValidator(Type t)
        {
            //Debug.Assert(t != null);

            _typeConverter = TypeDescriptor.GetConverter(t);
            _validatedType = t;
        }


        public Object ValidateValue(Object o)
        {
            if (_typeConverter.CanConvertFrom(o.GetType()))
            {
                try
                {
                    Object ret = _typeConverter.ConvertFrom(o);

                    _message = "";
                    return ret;
                }
                catch
                {
                    _message = "Cannot convert " + o.ToString() + " to " + _validatedType.FullName;
                    return null;
                }
            }
            else
            {
                _message = "Cannot convert " + o.ToString() + " to " + _validatedType.FullName;
                return null;
            }
        }

        public T ConvertTo<T>(Object o) where T : class
        {
            try
            {
                return (T)_typeConverter.ConvertTo(o, typeof(T));
            }
            catch
            {
                _message = "Cannot convert object to type " + typeof(T).ToString();
                return null;
            }
        }

        public static IValidator CreateFor(Type type)
        {
            Type vType;

            if (_defaultValidators.TryGetValue(type, out vType))
            {
                return (IValidator)TypeDescriptor.CreateInstance(
                    null, vType, null, null
                    );
            }

            return new DefaultValidator(type);
        }

        public static void RegisterDefaultValidator(Type type, Type validatorType)
        {
            //Debug.Assert(ContainsInterface(validatorType.GetInterfaces(),
            //    typeof(IValidator)));
            //Debug.Assert(!_defaultValidators.ContainsKey(type));

            _defaultValidators[type] = validatorType;
        }

        public static void RegisterDefaultValueFactory(Type type,
            DefaultValueFactoryDelegate factory)
        {
            //Debug.Assert(!_defaultValueFactories.ContainsKey(type));

            _defaultValueFactories.Add(type, factory);
        }

        static DefaultValidator()
        {
            _defaultValidators = new Dictionary<Type, Type>();
            _defaultValueFactories = new Dictionary<Type, DefaultValueFactoryDelegate>();

            RegisterDefaultValidator(typeof(float), typeof(FloatValidator));
            RegisterDefaultValueFactory(typeof(String), delegate { return ""; });
        }

        static bool ContainsInterface(Type[] types, Type iface)
        {
            foreach (Type type in types)
            {
                if (type == iface)
                    return true;
            }

            return false;
        }
    }
}
