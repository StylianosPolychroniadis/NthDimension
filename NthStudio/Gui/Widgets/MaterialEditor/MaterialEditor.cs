using System;

using NthDimension.Forms;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering;

namespace NthStudio.Gui.Widgets
{
    public class MaterialEditor : Panel
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

        private Material material;
        public Material Material
        {
            get { return material; }
            set
            {
                this.material = value;
                
                this.labelMaterial.Text = string.Format("{0}", this.material.name, this.material.pointer, this.material.identifier);
                this.labelShader.Text = string.Format("{0}", this.material.shader.Name, this.material.shader.Identifier, this.material.shader.Handle);
                //this.labelMaterialTextures.Text = string.Format("", this.material.Textures.Length, 0);
                
               
                if (!string.IsNullOrEmpty(this.Material.shader.VertexShader))
                {
                    this.shaderVertexTab.Source = this.Material.shader.VertexShader;
                    try
                    {
                        if (null != this.Material.shader.Pointer &&
                            !string.IsNullOrEmpty(this.Material.shader.Pointer[0]))



                            this.shaderVertexTab.Title = string.Format("{0} [Vertex]", this.Material.shader.Pointer[0]);
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(this.Material.shader.FragmentShader))
                {
                    this.shaderFragmentTab.Source = this.Material.shader.FragmentShader;
                    try
                    {
                        if (null != this.Material.shader.Pointer && 
                            !string.IsNullOrEmpty(this.Material.shader.Pointer[1]))
                            this.shaderFragmentTab.Title = string.Format("{0} [Fragment]", this.Material.shader.Pointer[1]);
                    }
                    catch { }
                }

                //var mat = Material.shader;
                //var nat = Material.shadowshader;


#if MATERIAL_V3
                if(null != this.Material.shader.GeometryShader)
                {
                    this.shaderGeometryTab.Source = this.Material.shader.GeometryShader;
                    if(null != this.Material.shader.Pointer &&
                        !string.IsNullOrEmpty(this.Material.shader.Pointer[2]))
                        this.shaderGeometryTab.Title = string.Format("Geometry {0}", this.Material.shader.Pointer[2]);
                }
                if(null != this.Material.shader.ComputeShader)
                {
                    this.shaderComputeTab.Source = this.Material.shader.ComputeShader;
                    if(null != this.Material.shader.Pointer &&
                        !string.IsNullOrEmpty(this.Material.shader.Pointer[3]))
                        this.shaderComputeTab.Title = string.Format("Compute {0}", this.Material.shader.Pointer[3]);
                }
#endif
            }

        }

        private Label labelMaterial;
        private Label labelShader;
        private SplitterBox panel;
        private Panel panelTexture;


        private Widgets.TabbedDocs shaderSource;
        private ShaderTextTab shaderVertexTab;
        private ShaderTextTab shaderFragmentTab;
        private ShaderTextTab shaderGeometryTab;
        private ShaderTextTab shaderComputeTab;

        public MaterialEditor()
        {
            this.InitializeComponent();
        }
        public MaterialEditor(Material material)
            :this()
        {
            this.Material = material;

            RunAfterNextPaintUpdate(delayedPopulateThumbnails);
        }

        private void delayedPopulateThumbnails()
        {
            if (null != Material.Textures)
            {
                if (Material.Textures[0].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[0], "Base 0")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[1].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[1], "Base 1")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[2].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[2], "Base 2")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[3].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[3], "Base 3")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[4].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[4], "Normal")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[5].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[5], "Emissive")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[6].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[6], "Reflection")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[7].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[7], "Emit Map")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[8].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[8], "Spec Map")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[9].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[9], "Env Map")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[10].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[10], "Environment")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[11].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[11], "DefInfo")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[12].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[12], "Video")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[13].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[13], "Aux 0")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[14].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[14], "Aux 1")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[15].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[15], "Aux 2")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
                if (Material.Textures[16].loaded)
                {
                    MaterialTextureThumbnail tt = new MaterialTextureThumbnail(Material.Textures[16], "Aux 3")
                    {
                        Dock = EDocking.Left
                    };
                    this.panelTexture.Widgets.Add(tt);
                }
            }
        }

        private void InitializeComponent()
        {
            Panel statusStrip = new Panel();
            statusStrip.Size = new System.Drawing.Size(this.Width, 20);
            statusStrip.Dock = EDocking.Bottom;
            statusStrip.BGColor = System.Drawing.Color.FromArgb(24,24,24);
            this.Widgets.Add(statusStrip);

            this.labelMaterial = new Label();
            this.labelMaterial.Size = new System.Drawing.Size(200, 18);
            this.labelMaterial.Dock = EDocking.Left;
            this.labelMaterial.TextAlign = ETextAlignment.Left;
            this.labelMaterial.BGColor = System.Drawing.Color.Transparent;
            this.labelMaterial.FGColor = System.Drawing.Color.OrangeRed;
            statusStrip.Widgets.Add(labelMaterial);

            this.labelShader = new Label();
            this.labelShader.Size = new System.Drawing.Size(200, 18);
            this.labelShader.Dock = EDocking.Right;
            this.labelShader.TextAlign = ETextAlignment.Right;
            this.labelShader.BGColor = System.Drawing.Color.Transparent;
            this.labelShader.FGColor = System.Drawing.Color.LightCyan;
            statusStrip.Widgets.Add(labelShader);

            //this.labelMaterialTextures = new Label();
            //this.labelMaterialTextures.Size = new System.Drawing.Size(this.Width, 18);
            //this.labelMaterialTextures.Dock = EDocking.Top;
            //this.Widgets.Add(labelMaterialTextures);

            this.panel = new NthDimension.Forms.Widgets.SplitterBox(ESplitterType.VerticalScroll);
            this.panel.Size = this.Size;
            this.panel.Dock = EDocking.Fill;
            this.panel.SplitterBarLocation = 0.25f;
            this.Widgets.Add(panel);

            var paneTop = this.panel.Panel0;
            var paneBottom = this.panel.Panel1;

            this.panelTexture = new Panel();
            this.panelTexture.Size = paneTop.Size;
            this.panelTexture.Dock = EDocking.Fill;
            paneTop.Widgets.Add(panelTexture);



            this.shaderSource = new Widgets.TabbedDocs();
            this.shaderSource.Size = paneBottom.Size;
            this.shaderSource.Dock = EDocking.Fill;

            paneBottom.Widgets.Add(shaderSource);

            this.shaderVertexTab = new ShaderTextTab();
            this.shaderVertexTab.Title = "Vertex";

            this.shaderSource.Items.Add(shaderVertexTab);

            this.shaderFragmentTab = new ShaderTextTab();
            this.shaderFragmentTab.Title = "Fragment";

            this.shaderSource.Items.Add(shaderFragmentTab);

            this.shaderGeometryTab = new ShaderTextTab();
            this.shaderGeometryTab.Title = "Geometry";

            this.shaderSource.Items.Add(shaderGeometryTab);

            this.shaderComputeTab = new ShaderTextTab();
            this.shaderComputeTab.Title = "Compute";

            this.shaderSource.Items.Add(shaderComputeTab);
        }
    }
}
