using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Dialogs
{
    public class ConsoleView : DialogBase
    {
        private static ConsoleView      _instance;

        public static ConsoleView       Instance
        {
                get{ return             _instance; }
        }

        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    }
}
