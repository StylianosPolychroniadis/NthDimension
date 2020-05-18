using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Forms;
using NthDimension.Forms.Events;


namespace Plugin.CADTransforms.Controls
{
    internal class TransformationSpace : Widget
    {
        Panel control;
        NthStudio.Gui.Widgets.ComboBox combo;
        NthStudio.Gui.Widgets.ToolButton btnWorld;
        NthStudio.Gui.Widgets.ToolButton btnLocal;
        bool m_init;

        public TransformationSpace()
        {
            m_init = false;
            this.Size = new System.Drawing.Size(180, 30);

        }


        private void InitializeComponent()
        {
            control = new Panel();
            control.Dock = EDocking.Fill;
            this.Widgets.Add(control);

            combo = new NthStudio.Gui.Widgets.ComboBox(new List<Widget>
            {
                new Label("World") { Size = new System.Drawing.Size(20,20) },
                new Label("Local") { Size = new System.Drawing.Size(20,20) }
            });
            combo.Size = new System.Drawing.Size(80, 25);
            combo.Location = new System.Drawing.Point(0, 0);
            control.Widgets.Add(combo);

            m_init = true;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(e);


        }
    }
}
