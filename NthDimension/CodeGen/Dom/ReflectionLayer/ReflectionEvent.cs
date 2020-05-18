namespace NthDimension.CodeGen.Dom.ReflectionLayer
{
    using System;
    using System.Reflection;

    internal class ReflectionEvent : DefaultEvent
    {
        public ReflectionEvent(EventInfo eventInfo, IClass declaringType) : base(declaringType, eventInfo.Name)
        {
            this.ReturnType = ReflectionReturnType.Create(this, eventInfo.EventHandlerType, false);

            // get modifiers
            MethodInfo methodBase = null;
            try
            {
                methodBase = eventInfo.GetAddMethod(true);
            }
            catch (Exception) { }

            if (methodBase == null)
            {
                try
                {
                    methodBase = eventInfo.GetRemoveMethod(true);
                }
                catch (Exception) { }
            }

            ModifierEnum modifiers = ModifierEnum.None;
            if (methodBase != null)
            {
                if (methodBase.IsStatic)
                {
                    modifiers |= ModifierEnum.Static;
                }

                if (methodBase.IsAssembly)
                {
                    modifiers |= ModifierEnum.Internal;
                }

                if (methodBase.IsPrivate)
                { // I assume that private is used most and public last (at least should be)
                    modifiers |= ModifierEnum.Private;
                }
                else if (methodBase.IsFamily || methodBase.IsFamilyOrAssembly)
                {
                    modifiers |= ModifierEnum.Protected;
                }
                else if (methodBase.IsPublic)
                {
                    modifiers |= ModifierEnum.Public;
                }
                else
                {
                    modifiers |= ModifierEnum.Internal;
                }
            }
            else
            {
                // assume public property, if no methodBase could be get.
                modifiers = ModifierEnum.Public;
            }
            this.Modifiers = modifiers;

        }
    }
}
