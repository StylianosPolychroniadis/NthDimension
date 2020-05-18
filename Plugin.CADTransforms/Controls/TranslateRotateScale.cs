using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Forms;
using NthDimension.Forms.Events;


namespace Plugin.CADTransforms.Controls
{
    internal class TranslateRotateScale : Widget
    {
        Panel control;
        
        NthStudio.Gui.Widgets.ToolButton btnTranslate;
        NthStudio.Gui.Widgets.ToolButton btnRotate;
        NthStudio.Gui.Widgets.ToolButton btnScale;
        bool m_init;
    }
}
