using System;

using NthDimension.Forms;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering;

namespace NthStudio.Gui
{
    // Note:: MaterialEditor is more advanced
    public class MaterialViewer : NthDimension.Forms.Dialogs.DialogBase
    {
        class ShaderTextTab : Widgets.TabStrip.TabStripItem
        {
            private string m_source;
            public string Source { get { return m_source; } set { m_source = value; shaderSource.Text = value; } }

            private Widgets.TextEditor.TextEditor shaderSource;

            public ShaderTextTab()
            {
                this.SizeChangedEvent += delegate
                {
                    // Do not do anything here - remove
                };

                this.shaderSource = new Widgets.TextEditor.TextEditor();
                this.shaderSource.Dock = EDocking.Fill;
                this.Widgets.Add(this.shaderSource);
            }
            public ShaderTextTab(string source)
                : this()
            {
                this.Source = source;
            }
        }

        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Material material;
        public Material Material
        {
            get { return material; }
            set
            {
                this.material = value;
                this.shaderVertexTab.Source = this.Material.shader.VertexShader;
                this.shaderFragmentTab.Source = this.Material.shader.FragmentShader;
#if MATERIAL_V3
                this.shaderGeometryTab.Source = this.Material.shader.GeometryShader;
                this.shaderComputeTab.Source = this.Material.shader.ComputeShader;
#endif
            }

        }

        private SplitterBox panel;
        private Panel panelTexture;


        private Widgets.TabbedDocs shaderSource;
        private ShaderTextTab shaderVertexTab;
        private ShaderTextTab shaderFragmentTab;
        private ShaderTextTab shaderGeometryTab;
        private ShaderTextTab shaderComputeTab;


        public MaterialViewer()
            : base()
        {

            this.panel = new NthDimension.Forms.Widgets.SplitterBox(ESplitterType.VerticalScroll);
            this.panel.Size = this.Size;
            this.panel.Dock = EDocking.Fill;
            this.panel.SplitterBarLocation = 0.25f;
            this.Widgets.Add(panel);

            var paneTop = this.panel.Panel0;
            var paneBottom = this.panel.Panel1;

            this.panelTexture = new Panel();
            this.panelTexture.Size = paneTop.Size;
            this.panelTexture.Dock = EDocking.Top;

            paneTop.Widgets.Add(panelTexture);

            this.shaderSource = new Widgets.TabbedDocs();
            this.shaderSource.Size = paneBottom.Size;
            this.shaderSource.Dock = EDocking.Fill;

            paneBottom.Widgets.Add(shaderSource);

            this.shaderVertexTab = new ShaderTextTab();
            this.shaderVertexTab.Text = "Vertex";

            this.shaderSource.Items.Add(shaderVertexTab);

            this.shaderFragmentTab = new ShaderTextTab();
            this.shaderFragmentTab.Text = "Fragment";

            this.shaderSource.Items.Add(shaderFragmentTab);

            this.shaderGeometryTab = new ShaderTextTab();
            this.shaderGeometryTab.Text = "Geometry";

            this.shaderSource.Items.Add(shaderGeometryTab);

            this.shaderComputeTab = new ShaderTextTab();
            this.shaderComputeTab.Text = "Compute";

            this.shaderSource.Items.Add(shaderComputeTab);


        }
        public MaterialViewer(NthDimension.Rendering.Material material)
            : this()
        {
            this.Material = material;
        }
        public override void OnShowDialog()
        {
            base.OnShowDialog();

            //RunAfterNextPaintUpdate(LoadMaterialUI);
        }

    }
}
