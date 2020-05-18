using NthStudio.Gui.Widgets.PropertyGrid.PropertyEditors;
using NthStudio.Gui.Widgets.PropertyGrid.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public static class DefaultPropertyEditorMap
    {
        #region variables
        static Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        #endregion

        static DefaultPropertyEditorMap()
        {
            Type attributeType = typeof(IsDefaultPropertyEditorOfAttribute);

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in a.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    Object[] attributes = type.GetCustomAttributes(attributeType, true);

                    if (attributes.Length == 0)
                        continue;

                    foreach (Object attribute in attributes)
                    {
                        IsDefaultPropertyEditorOfAttribute dpAttribute =
                            attribute as IsDefaultPropertyEditorOfAttribute;

                        if (dpAttribute != null)
                        {
                            _map.Add(dpAttribute.TargetType, type);
                        }
                    }
                }
            }
        }

        public static PropertyEditorBase GetEditor(Type propertyType)
        {
            Type editorType;

            for (Type t = propertyType; t != typeof(Object); t = t.BaseType)
            {
                if (_map.TryGetValue(t, out editorType))
                {
                    Object o = TypeDescriptor.CreateInstance(null, editorType, null, null);

                    return (PropertyEditorBase)o;
                }
            }

            ValidatingStringEditor defaultEditor = new ValidatingStringEditor();
            defaultEditor.Validator = DefaultValidator.CreateFor(propertyType);

            return defaultEditor;
        }
    }
}
