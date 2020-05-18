using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document
{
    /// <summary>
	/// This interface is used to describe a span inside a text sequence
	/// </summary>
	public class AbstractSegment : ISegment
    {
        protected int offset = -1;
        protected int length = -1;

        #region ICSharpCode.TextEditor.Document.ISegment interface implementation

        public virtual int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        public virtual int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        #endregion

        public override string ToString()
        {
            return String.Format("[AbstractSegment: Offset = {0}, Length = {1}]",
                                 Offset,
                                 Length);
        }


    }
}
