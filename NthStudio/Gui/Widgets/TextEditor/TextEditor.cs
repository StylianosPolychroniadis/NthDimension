using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor
{
    public class TextEditor : TextEditorBase
    {
        // From WinForms (NthDimension 2016) Code Editor
        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do\n{\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };
        // End From WinForms


        TextArea iTextArea;

        #region Constructor

        public TextEditor()
        {
            PaintBackGround = false;
            Document = (new DocumentFactory()).CreateDocument();
            Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("C#");

            // El TextArea ha de crearse después de crear el Documento.
            iTextArea = new TextArea(this);
            iTextArea.Dock = EDocking.Fill;

            Widgets.Add(iTextArea);

            Document.UpdateCommited += CommitUpdateRequested;
            Document.DocumentChanged += Document_DocumentChanged;

            //OptionsChanged();
            iTextArea.OptionsChanged();
        }
        #endregion Constructor

        public bool EnableUndo
        {
            get
            {
                return Document.UndoStack.CanUndo;
            }
        }

        public bool EnableRedo
        {
            get
            {
                return Document.UndoStack.CanRedo;
            }
        }

        public override TextArea ActiveTextArea
        {
            get
            {
                return iTextArea;
            }
        }

        void Document_DocumentChanged(object sender, DocumentEventArgs e)
        {
            if (Document.FoldingManager.FoldingStrategy != null)
                Document.FoldingManager.UpdateFoldings(null, null);
        }

        public override void OptionsChanged()
        {
            iTextArea.OptionsChanged();
            /*if (secondaryTextArea != null) {
				secondaryTextArea.OptionsChanged();
			}*/
        }

        public void Undo()
        {
            if (Document.ReadOnly)
            {
                return;
            }
            if (Document.UndoStack.CanUndo)
            {
                BeginUpdate();
                Document.UndoStack.Undo();

                Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
                this.iTextArea.TextCanvas.UpdateMatchingBracket();
                /*if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}*/
                EndUpdate();
            }
        }

        public void Redo()
        {
            if (Document.ReadOnly)
            {
                return;
            }
            if (Document.UndoStack.CanRedo)
            {
                BeginUpdate();
                Document.UndoStack.Redo();

                Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
                this.iTextArea.TextCanvas.UpdateMatchingBracket();
                /*if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}*/
                EndUpdate();
            }
        }

        public virtual void SetHighlighting(string name)
        {
            Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(name);
        }

        #region Update Methods

        public override void EndUpdate()
        {
            base.EndUpdate();
            Document.CommitUpdate();
            if (!IsInUpdate)
            {
                ActiveTextArea.Caret.OnEndUpdate();
            }
        }

        void CommitUpdateRequested(object sender, EventArgs e)
        {
            if (IsInUpdate)
            {
                return;
            }
            foreach (TextAreaUpdate update in Document.UpdateQueue)
            {
                switch (update.TextAreaUpdateType)
                {
                    case TextAreaUpdateType.PositionToEnd:
                        iTextArea.TextCanvas.UpdateToEnd(update.Position.Y);
                        /*if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						}*/
                        break;
                    case TextAreaUpdateType.PositionToLineEnd:
                    case TextAreaUpdateType.SingleLine:
                        iTextArea.TextCanvas.UpdateLine(update.Position.Y);
                        /*if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y);
						}*/
                        break;
                    case TextAreaUpdateType.SinglePosition:
                        iTextArea.TextCanvas.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
                        /*if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						}*/
                        break;
                    case TextAreaUpdateType.LinesBetween:
                        iTextArea.TextCanvas.UpdateLines(update.Position.X, update.Position.Y);
                        /*if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						}*/
                        break;
                    case TextAreaUpdateType.WholeTextArea:
                        iTextArea.TextCanvas.Invalidate();
                        /*if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.Invalidate();
						}*/
                        break;
                }
            }
            Document.UpdateQueue.Clear();
            // OLD_CODE
            //			this.primaryTextArea.TextArea.Update();
            //			if (this.secondaryTextArea != null) {
            //				this.secondaryTextArea.TextArea.Update();
            //			}
        }
        #endregion Update Methods


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                /*if (printDocument != null) {
					printDocument.BeginPrint -= new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  -= new PrintPageEventHandler(this.PrintPage);
					printDocument = null;
				}*/
                Document.UndoStack.ClearAll();
                Document.UpdateCommited -= new EventHandler(CommitUpdateRequested);
            }
            base.Dispose(disposing);
        }

    }
}
