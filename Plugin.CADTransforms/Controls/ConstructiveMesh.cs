using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Forms;
using NthDimension.Forms.Events;

namespace Plugin.CADTransforms.Controls
{
    internal class ConstructivePolygon : Widget
    {
        Panel control;

        NthStudio.Gui.Widgets.ToolButton btnSelectAll;
        NthStudio.Gui.Widgets.ToolButton btnSelectNone;
        NthStudio.Gui.Widgets.ToolButton btnVertices;
        NthStudio.Gui.Widgets.ToolButton btnEdges;
        NthStudio.Gui.Widgets.ToolButton btnQuads;
        NthStudio.Gui.Widgets.ToolButton btnExtrude;
        NthStudio.Gui.Widgets.ToolButton btnCamfer;
        bool m_init;
    }
}
