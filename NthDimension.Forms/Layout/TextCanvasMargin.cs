using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Layout
{
    public class TextCanvasMargin : AbstractMargin
    {
        protected TextCanvas iTextCanvas;

        public TextCanvas TextCanvas
        {
            get
            {
                return iTextCanvas;
            }
        }

        public IDocument Document
        {
            get
            {
                return iTextCanvas.Document;
            }
        }

        public ITextEditorProperties TextEditorProperties
        {
            get
            {
                return iTextCanvas.Document.TextEditorProperties;
            }
        }

        public TextCanvasMargin(TextCanvas pTextCanvas)
        {
            this.iTextCanvas = pTextCanvas;
        }
    }
}
