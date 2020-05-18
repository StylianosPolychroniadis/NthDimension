namespace NthDimension.CodeGen.Dom.Refactoring
{
    using System;
    using NRefactory.Ast;
    using NRefactory.PrettyPrinter;

    public class VBNetCodeGenerator : NRefactoryCodeGenerator
    {
        internal static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();

        public override IOutputAstVisitor CreateOutputVisitor()
        {
            VBNetOutputVisitor v = new VBNetOutputVisitor();
            VBNetPrettyPrintOptions pOpt = v.Options;

            pOpt.IndentationChar = this.Options.IndentString[0];
            pOpt.IndentSize = this.Options.IndentString.Length;
            pOpt.TabSize = this.Options.IndentString.Length;

            return v;
        }

        public override PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
        {
            string propertyName = GetPropertyName(field.Name);
            if (string.Equals(propertyName, field.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (HostCallback.RenameMember(field, "m_" + field.Name))
                {
                    field = new DefaultField(field.ReturnType, "m_" + field.Name,
                                             field.Modifiers, field.Region, field.DeclaringType);
                }
            }
            return base.CreateProperty(field, createGetter, createSetter);
        }
    }
}
