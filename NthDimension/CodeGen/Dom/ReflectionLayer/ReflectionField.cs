﻿namespace NthDimension.CodeGen.Dom.ReflectionLayer
{
    using System.Reflection;

    internal class ReflectionField : DefaultField
    {
        public ReflectionField(FieldInfo fieldInfo, IClass declaringType) : base(declaringType, fieldInfo.Name)
        {
            this.ReturnType = ReflectionReturnType.Create(this, fieldInfo.FieldType, false);

            ModifierEnum modifiers = ModifierEnum.None;
            if (fieldInfo.IsInitOnly)
            {
                modifiers |= ModifierEnum.Readonly;
            }

            if (fieldInfo.IsStatic)
            {
                modifiers |= ModifierEnum.Static;
            }

            if (fieldInfo.IsAssembly)
            {
                modifiers |= ModifierEnum.Internal;
            }

            if (fieldInfo.IsPrivate)
            { // I assume that private is used most and public last (at least should be)
                modifiers |= ModifierEnum.Private;
            }
            else if (fieldInfo.IsFamily || fieldInfo.IsFamilyOrAssembly)
            {
                modifiers |= ModifierEnum.Protected;
            }
            else if (fieldInfo.IsPublic)
            {
                modifiers |= ModifierEnum.Public;
            }
            else
            {
                modifiers |= ModifierEnum.Internal;
            }

            if (fieldInfo.IsLiteral)
            {
                modifiers |= ModifierEnum.Const;
            }
            this.Modifiers = modifiers;
        }
    }
}
