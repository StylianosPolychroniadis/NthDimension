namespace NthDimension.CodeGen.Dom.Refactoring
{
    using System;
    using NRefactory.Ast;
    using NRefactory.PrettyPrinter;

    public abstract class NRefactoryCodeGenerator : CodeGenerator
    {
        public abstract IOutputAstVisitor CreateOutputVisitor();

        public override string GenerateCode(AbstractNode node, string indentation)
        {
            IOutputAstVisitor visitor = CreateOutputVisitor();
            int indentCount = 0;
            foreach (char c in indentation)
            {
                if (c == '\t')
                    indentCount += 4;
                else
                    indentCount += 1;
            }
            visitor.OutputFormatter.IndentationLevel = indentCount / 4;
            if (node is Statement)
                visitor.OutputFormatter.Indent();
            node.AcceptVisitor(visitor, null);
            string text = visitor.Text;
            if (node is Statement && !text.EndsWith("\n"))
                text += Environment.NewLine;
            return text;
        }
    }
}
