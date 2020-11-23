
using System.Drawing;
using System.Linq;

using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.Utilities;
using System.Collections.Generic;
using NthDimension.Forms.Events;
using System;

namespace NthStudio.Gui
{
    public class AssetsViewer : DialogBase
    {
        #region Singleton
        protected static AssetsViewer _instance;
        public static AssetsViewer Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Singleton

        #region Tree Nodes Classes
        class MeshesNode : TreeNode
        {
            class MeshNode : TreeNode
            {
                MeshVbo m_mesh;

                public MeshNode(MeshVbo mesh)
                {
                    m_mesh = mesh;

                    Text = string.Format("[{0}] {1}", mesh.Identifier, mesh.Name);
                }
            }

            public MeshesNode()
            {
                Text = "Meshes";

                foreach (MeshVbo m in StudioWindow.Instance.MeshLoader.meshes)
                    Nodes.Add(new MeshNode(m));
            }
        }
        class TexturesNode : TreeNode
        {
            class FramebuffersNode : TreeNode { public FramebuffersNode(string name) : base(name) { } }
            class FramebufferTexture : TextureNode
            {
                Texture ftexture;

                TextureViewer texView;

                public FramebufferTexture(Texture texture)
                    : base(texture)
                {
                    ftexture = texture;
                    Text = string.Format("[{0}] {1}", texture.identifier, texture.name);

                    //var fb = StudioWindow.Instance.FramebufferCreator.getFrameBuffer(texture.name);

                    //if (null != fb)
                    //{
                    //    this.Nodes.Add(new TreeNode(fb.ColorTexture));
                    //    this.Nodes.Add(new TreeNode(fb.DepthTexture));
                    //
                }

                public override void OnMouseClick(MouseEventArgs e)
                {
                    //ConsoleUtil.log(string.Format("TextureViewer id {0} name {1}", ftexture.texture, ftexture.name));
                    //texView = new TextureViewer(ftexture);
                    //texView.Show(((StudioWindow)StudioWindow.Instance).Screen2D);

                    AssetsViewer.Instance.PreviewTexture(ftexture.identifier);
                    
                }

            }
            class TextureNode : TreeNode
            {
                private Texture m_texture;
                TextureViewer texView;
                public TextureNode(Texture texture)
                {
                    
                    m_texture = texture;
                    string text = string.IsNullOrEmpty(texture.pointer) ? texture.name : texture.pointer;
                    Text = string.Format("[{0}] {1}", m_texture.identifier, text);                  
                }

                public override void OnMouseClick(MouseEventArgs e)
                {
                    AssetsViewer.Instance.PreviewTexture(m_texture.texture);
                }

                //public override void OnMouseDoubleClick(MouseEventArgs e)
                //{

                    

                //    ConsoleUtil.log(string.Format("TextureViewer id {0} name {1}", m_texture.texture, m_texture.name));
                //    texView = new TextureViewer(m_texture);
                //    texView.Show(((StudioWindow)StudioWindow.Instance).Screen2D);
                //}
            }

            public TexturesNode()
            {
                Text = "Textures";

                FramebuffersNode framebuffers = new FramebuffersNode("Framebuffers");

                foreach (Texture t in StudioWindow.Instance.TextureLoader.textures)
                    if (string.IsNullOrEmpty(t.pointer))
                        framebuffers.Nodes.Add(new FramebufferTexture(t));
                    else
                    {
                        TextureNode tn = new TextureNode(t);
                        Nodes.Add(tn);
                    }

                Nodes.Add(framebuffers);
            }

            public int TextureCount
            {
                get
                {
                    var list = Nodes.OfType<FramebuffersNode>().ToList<FramebuffersNode>();

                    int listCount = 0;

                    if(list.Count > 0)
                        listCount = list[0].Nodes.Count;
                    
                    int hasFramebuffers = 0;

                    if (listCount > 0)
                        hasFramebuffers = 1;

                    return Nodes.Count - hasFramebuffers + listCount;
                }
            }
        }
        class FramebuffersNode : TreeNode
        {
            class FramebufferNode : TreeNode
            {
                private Framebuffer m_framebuffer;
                public FramebufferNode(Framebuffer framebuffer)
                {
                    m_framebuffer = framebuffer;
                    Text = string.Format("[{0}] {1}", m_framebuffer.FboHandle, m_framebuffer.Name);

                    if (m_framebuffer.ColorTexture > 0)
                        Nodes.Add(new TreeNode(string.Format("[{0}] Color", m_framebuffer.ColorTexture)));

                    if (m_framebuffer.DepthTexture > 0)
                        Nodes.Add(new TreeNode(string.Format("[{0}] Depth", m_framebuffer.DepthTexture)));
                    if (m_framebuffer.DepthTexture1 > 0)
                        Nodes.Add(new TreeNode(string.Format("[{0}] DepthOne", m_framebuffer.DepthTexture1)));
                    if (m_framebuffer.DepthTexture2 > 0)
                        Nodes.Add(new TreeNode(string.Format("[{0}] DepthTwo", m_framebuffer.DepthTexture2)));
                    if (m_framebuffer.DepthTexture3 > 0)
                        Nodes.Add(new TreeNode(string.Format("[{0}] DepthThree", m_framebuffer.DepthTexture3)));

                }
            }

            public FramebuffersNode()
            {
                Text = "Framebuffers";

                foreach (Framebuffer f in StudioWindow.Instance.FramebufferCreator.Framebuffers)
                    Nodes.Add(new FramebufferNode(f));
            }
        }
        class ShadersNode : TreeNode
        {
            class FragmentShaderNode : TreeNode
            {
                private string m_source;
                public string Source {  get { return m_source; } }

                public FragmentShaderNode(string fragment)
                {
                    this.Text = "Fragment";
                    m_source = fragment;
                }

                public override void OnMouseClick(MouseEventArgs e)
                {
                    AssetsViewer.Instance.PreviewText(m_source);
                }
            }
            class VertexShaderNode : TreeNode
            {
                private string m_source;
                public string Source { get { return m_source; } }

                public VertexShaderNode(string vertex)
                {
                    this.Text = "Vertex";
                    m_source = vertex;
                }

                public override void OnMouseClick(MouseEventArgs e)
                {
                    AssetsViewer.Instance.PreviewText(m_source);
                }
            }
            class UniformNode : TreeNode
            {
                int         m_location;
                string      m_uniformName;

                public UniformNode(string uniformName, int uniformLocation)
                {
                    
                    m_uniformName = uniformName;
                    m_location = uniformLocation;

                    Text = string.Format("{1} (location {0})", m_location, m_uniformName);
                }
            }
            class ShaderNode : TreeNode
            {
                Shader m_shader;

                public ShaderNode(Shader shader)
                {
                    m_shader = shader;
                    
                    this.Text = string.Format("[{0}] {1} ({2})", m_shader.Identifier, m_shader.Name, m_shader.type.ToString());

                    this.ForeColor = (m_shader.Loaded ? Color.ForestGreen : Color.DarkRed);

                    this.Nodes.Add(new VertexShaderNode(m_shader.VertexShader));
                    this.Nodes.Add(new FragmentShaderNode(m_shader.FragmentShader));

                    TreeNode uniforms = new TreeNode("Uniforms");
                    if(null != m_shader.Uniforms)
                        foreach (string uniform in m_shader.Uniforms)
                        {
                            int loc = ApplicationBase.Instance.Renderer.GetUniformLocation(shader.Handle, uniform);
                            if(loc >= 0)
                                uniforms.Nodes.Add(new UniformNode(uniform, loc));
                        }

                    if(uniforms.Nodes.Count > 0)
                        this.Nodes.Add(uniforms);
                }

                public bool Compiled
                {
                    get
                    {
                        return m_shader.Loaded;
                    }
                }
            }

            public ShadersNode()
            {
                Text = "Shaders";

                foreach (Shader s in StudioWindow.Instance.ShaderLoader.shaders)
                {
                    int shIdx = Nodes.Add(new ShaderNode(s));
                    if(!s.Loaded)
                    { this.FGColor = Color.OrangeRed; }
                }
            }

           
        }
        class MaterialsNode : TreeNode
        {
            class MaterialNode : TreeNode
            {
                Material m_material;
                public MaterialNode(Material material)
                {
                    m_material = material;

                    Text = string.Format("[{0}] {1}", m_material.identifier, string.IsNullOrEmpty(m_material.name) ? m_material.pointer : m_material.name);

                    TreeNode textures = new TreeNode("Textures");
                    TreeNode texturesFramebuffers = new TreeNode("Framebuffers");

                    foreach (var t in m_material.Textures)
                    {
                        if(!string.IsNullOrEmpty(t.name) && 
                           !string.IsNullOrEmpty(t.pointer))
                            if (string.IsNullOrEmpty(t.pointer))
                                texturesFramebuffers.Nodes.Add(new TreeNode(string.Format("[{0}] {1}", t.identifier, t.name)));
                            else
                                textures.Nodes.Add(new TreeNode(string.Format("[{0}] {1}", t.identifier, t.pointer)));
                    }

                    if(texturesFramebuffers.Nodes.Count > 0)
                        textures.Nodes.Add(texturesFramebuffers);
                    
                    TreeNode shaders = new TreeNode("Shaders");

                    if (!string.IsNullOrEmpty(m_material.shader.Name))
                        shaders.Nodes.Add(new TreeNode(string.Format("Diffuse: {0}", m_material.shader.Name)));

                    if (!string.IsNullOrEmpty(m_material.definfoshader.Name))
                        shaders.Nodes.Add(new TreeNode(string.Format("Deferred: {0}", m_material.definfoshader.Name)));

                    if (!string.IsNullOrEmpty(m_material.shadowshader.Name))
                        shaders.Nodes.Add(new TreeNode(string.Format("Shadow: {0}", m_material.shadowshader.Name)));

                    if (!string.IsNullOrEmpty(m_material.ssnshader.Name))
                        shaders.Nodes.Add(new TreeNode(string.Format("SSNormals: {0}", m_material.ssnshader.Name)));

                    if (!string.IsNullOrEmpty(m_material.selectionshader.Name))
                        shaders.Nodes.Add(new TreeNode(string.Format("Selection: {0}", m_material.selectionshader.Name)));

                    
                    if(textures.Nodes.Count > 0)
                        Nodes.Add(textures);
                    if(shaders.Nodes.Count > 0)
                        Nodes.Add(shaders);
                    
                }
            }

            public MaterialsNode()
            {
                Text = "Materials";

                foreach (Material m in StudioWindow.Instance.MaterialLoader.materials)
                    Nodes.Add(new MaterialNode(m));
            }
        }
        class TemplatesNode : TreeNode
        {
            class TemplateNode : TreeNode
            {
                Template m_template;

                public TemplateNode(Template template)
                {
                    m_template = template;
                    Text = string.Format("[{0}] {1}",  m_template.identifier, m_template.name);

                    TreeNode meshes = Nodes.Add("Meshes");

                    foreach (string s in m_template.meshes)
                        meshes.Nodes.Add(new TreeNode(s));

                    TreeNode materials = Nodes.Add("Materials");

                    foreach (string m in m_template.materials)
                        materials.Nodes.Add(new TreeNode(m));

                    Nodes.Add(meshes);
                    Nodes.Add(materials);
                }
            }

            public TemplatesNode()
            {
                Text = "Templates";

                foreach (Template t in StudioWindow.Instance.TemplateLoader.templates)
                    Nodes.Add(new TemplateNode(t));
            }
        }
        #endregion

        #region Widgets
        private Panel m_statusStrip;
        private Label m_statusLabel;
        private SplitterBox m_splitDisplayH;
        private Panel m_previewPanel;
        private Widgets.Picture m_previewPicture;
        private Widgets.TextEditor.TextEditor m_previewText;

        private TreeView            m_treeAssets;
        private MeshesNode          m_meshAssets;
        private TexturesNode        m_textureAssets;
        private FramebuffersNode    m_framebufferAssets;
        private ShadersNode         m_shaderAssets;
        private MaterialsNode       m_materialAssets;
        private TemplatesNode       m_templateAssets;

        private TreeView            m_treeCacheFiles;
        private TreeNode            m_meshCacheFiles;
        private TreeNode            m_textureCacheFiles;
        private TreeNode            m_shaderCacheFiles;
        private TreeNode            m_materialCacheFiles;
        private TreeNode            m_templateCacheFiles;

        TreeNode sun;
        #endregion

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }


        public AssetsViewer()
        {
            _instance = this;
            Title   = "Assets Viewer";
            Size    = new Size(900, 600);
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BGColor = Color.DarkGray;

            #region Status Bar
            m_statusStrip = new Panel();
            m_statusStrip.Size = new Size(200, 20);
            m_statusStrip.Dock = EDocking.Bottom;
            {
                m_statusLabel = new Label(string.Empty);
                m_statusLabel.Size = new Size(100, 18);
                m_statusLabel.Location = new Point(5, 0);
                m_statusLabel.Font = new NanoFont(NanoFont.DefaultRegular, 10f);
                m_statusStrip.Widgets.Add(m_statusLabel);
            }
            #endregion

            #region MainSplitter
            m_splitDisplayH = new SplitterBox(ESplitterType.HorizontalScroll);
            m_splitDisplayH.Name = "SplitBoxH";
            m_splitDisplayH.SplitterBarLocation = 0.2f;
            m_splitDisplayH.Size = new Size(400, 400);
            m_splitDisplayH.Dock = EDocking.Fill;
            m_splitDisplayH.PaintBackGround = true;

            #region Panel0 (Tree)

            #region panelAssets (tree)
            Panel panelAssets = new Panel();
            panelAssets.Dock = EDocking.Fill;
            panelAssets.PaintBackGround = false;
            panelAssets.Widgets.Add(new Label("Assets")
                                    {
                                        Size = new Size(400, 20),
                                        Font = new NanoFont(NanoFont.DefaultRegular, 10f),
                                        Dock = EDocking.Top,
                                        BGColor = Color.LightSlateGray,
                                        PaintBackGround = true
                                    });

            #region tree
            m_treeAssets = new TreeView();
            m_treeAssets.Dock = EDocking.Fill;
            m_treeAssets.BGColor = Color.FromArgb(10, 72, 75, 85);
            m_treeAssets.FGColor = Color.LightGray;
            m_treeAssets.FullRowSelect = true;

            #region Available Nodes
            TreeNode m_available = new TreeNode();
            m_treeAssets.Nodes.Add(m_available);
            m_available.Text = "Available Nodes";

            m_meshAssets = new MeshesNode();
            m_available.Nodes.Add(m_meshAssets);
            m_meshAssets.Text = string.Format("Meshes ({0})", m_meshAssets.Nodes.Count);

            m_textureAssets = new TexturesNode();
            m_available.Nodes.Add(m_textureAssets);
            m_textureAssets.Text = string.Format("Textures ({0})", m_textureAssets.TextureCount); // -1 IF textures contains Framebuffers
           
            //m_textureAssets.MouseClickEvent += delegate (object sender, NthDimension.Forms.Events.MouseEventArgs e)
            //{
            //    ////this.OnMouseDoubleClick(e);
            //    //using (TextureViewer texView = new TextureViewer(texture))
            //    //{

            //    //    texView.Show();
            //    //};

            //    ConsoleUtil.log("Node Click");
            //};

            m_framebufferAssets = new FramebuffersNode();
            m_available.Nodes.Add(m_framebufferAssets);
            m_framebufferAssets.Text = string.Format("Framebuffers ({0})", m_framebufferAssets.Nodes.Count);

            m_shaderAssets = new ShadersNode();
            m_available.Nodes.Add(m_shaderAssets);
            m_shaderAssets.Text = string.Format("Shaders ({0})", m_shaderAssets.Nodes.Count);

            m_materialAssets = new MaterialsNode();
            m_available.Nodes.Add(m_materialAssets);
            m_materialAssets.Text = string.Format("Materials ({0})", m_materialAssets.Nodes.Count);

            m_templateAssets = new TemplatesNode();
            m_available.Nodes.Add(m_templateAssets);
            m_templateAssets.Text = string.Format("Templates ({0})", m_templateAssets.Nodes.Count);
            #endregion Available nodes

            #region Scene Nodes
            TreeNode scene = new TreeNode();
            m_treeAssets.Nodes.Add(scene);
            scene.Text = "Scene Nodes";

            TreeNode scene_sunlights = new TreeNode();
            scene.Nodes.Add(scene_sunlights);
            scene_sunlights.Text = "Light (Sun)";

            foreach(var s in StudioWindow.Instance.Scene.DirectionalLights)
            {
                /*TreeNode */sun = new TreeNode() { Tag = s, Text = string.Format("{0}", s.Name) };
                scene_sunlights.Nodes.Add(sun);
                sun.Tag = s;

                string scolor = s.Color.ToString();

                TreeNode color = new TreeNode() { Tag = s, Text = string.Format("Color: {0}", scolor) };
                sun.Nodes.Add(color);
                color.MouseClickEvent += delegate(object sender, NthDimension.Forms.Events.MouseEventArgs mea) {
                    Debug.ColorPickerDialog col = new Debug.ColorPickerDialog() { PrimaryColor = Color.FromArgb((int)(s.Color.W * 255f), (int)(s.Color.X * 255f), (int)(s.Color.Y * 255f) , (int)(s.Color.Z * 255f)) };
                    col.OnColorChanged += delegate (Debug.ColorPickerDialog.ColorChangedEventArgs e)
                    {
                        ((NthDimension.Rendering.Drawables.Lights.LightDirectional)sun.Tag).Color = new NthDimension.Algebra.Vector4(                                                                                      
                                                                                      e.ColorNew.R / 255f,
                                                                                      e.ColorNew.G / 255f,
                                                                                      e.ColorNew.B / 255f, 
                                                                                      e.ColorNew.A / 255f);
                    };
                    System.Windows.Forms.DialogResult dr = col.ShowDialog();

                    if (dr == System.Windows.Forms.DialogResult.Cancel) return;
                    if(dr == System.Windows.Forms.DialogResult.OK)
                    {
                        //NthDimension.Algebra.Vector4 vec4 = new NthDimension.Algebra.Vector4();
                        //((NthDimension.Rendering.Drawables.Lights.LightSun)((TreeNode)sender).Tag).Color = new NthDimension.Algebra.Vector4();
                        ((NthDimension.Rendering.Drawables.Lights.LightDirectional)sun.Tag).Color =  new NthDimension.Algebra.Vector4(col.PrimaryColor.A / 255,
                                                                                      col.PrimaryColor.R / 255,
                                                                                      col.PrimaryColor.G / 255,
                                                                                      col.PrimaryColor.B / 255);
                    }
                    
                };

                string acolor = s.lightAmbient.ToString();
                TreeNode ambient = new TreeNode() { Tag = s, Text = string.Format("Ambient: {0}", acolor) };
                sun.Nodes.Add(ambient);
            }

            TreeNode scene_spotlights = new TreeNode();
            scene.Nodes.Add(scene_spotlights);
            scene_spotlights.Text = "Light (Spot)";

            foreach (var s in StudioWindow.Instance.Scene.Spotlights)
            {

            }

            TreeNode scene_skybox = new TreeNode();
            scene.Nodes.Add(scene_skybox);
            scene_skybox.Text = "Skybox";

            TreeNode scene_model = new TreeNode();
            scene.Nodes.Add(scene_model);
            scene_model.Text = "Models";

            TreeNode scene_model_static = new TreeNode() { Text = "Static" };
            scene_model.Nodes.Add(scene_model_static);

            TreeNode scene_model_animated = new TreeNode() { Text = "Animated" };
            scene_model.Nodes.Add(scene_model_animated);


            #endregion
            #endregion tree

            panelAssets.Widgets.Add(m_treeAssets);            
            #endregion panelAssets (tree)

            #endregion Panel0

            #region Panel1

            #region panelCacheFiles (tree)
            Panel panelCacheFiles = new Panel();
            panelCacheFiles.Dock = EDocking.Fill;
            panelCacheFiles.PaintBackGround = false;
            panelCacheFiles.Widgets.Add(new Label("Cache Files")
                                        {
                                            Size = new Size(400, 20),
                                            Font = new NanoFont(NanoFont.DefaultRegular, 10f),
                                            Dock = EDocking.Top,
                                            BGColor = Color.LightSlateGray,
                                            PaintBackGround = true
                                        });

            #region tree
            m_treeCacheFiles = new TreeView();
            m_treeCacheFiles.Dock = EDocking.Fill;
            m_treeCacheFiles.BGColor = Color.FromArgb(10, 72, 75, 85);
            m_treeCacheFiles.FGColor = Color.LightGray;
            m_treeCacheFiles.FullRowSelect = true;

            m_meshCacheFiles = new TreeNode("Mesh");
            m_treeCacheFiles.Nodes.Add(m_meshCacheFiles);

            m_textureCacheFiles = new TreeNode("Texture");
            m_treeCacheFiles.Nodes.Add(m_textureCacheFiles);

            m_shaderCacheFiles = new TreeNode("Shader");
            m_treeCacheFiles.Nodes.Add(m_shaderCacheFiles);

            m_materialCacheFiles = new TreeNode("Material");
            m_treeCacheFiles.Nodes.Add(m_materialCacheFiles);

            m_templateCacheFiles = new TreeNode("Templates");
            m_treeCacheFiles.Nodes.Add(m_templateCacheFiles);
            #endregion

            panelCacheFiles.Widgets.Add(m_treeCacheFiles);
            #endregion

            #endregion Panel1


            SplitterBox assetsCacheSplit = new SplitterBox(ESplitterType.VerticalScroll);
            assetsCacheSplit.Dock = EDocking.Fill;
            assetsCacheSplit.SplitterBarLocation = 0.75f;

            assetsCacheSplit.Panel0.Widgets.Add(panelAssets);
            assetsCacheSplit.Panel1.Widgets.Add(panelCacheFiles);

            m_splitDisplayH.Panel0.Widgets.Add(assetsCacheSplit);

            m_previewPanel = new Panel()
            {
                BGColor = Color.LightSlateGray,
                Dock = EDocking.Fill
            };

            

            m_splitDisplayH.Panel1.Widgets.Add(m_previewPanel);
            m_splitDisplayH.SplitterBarLocation = 0.35f;
            #endregion

            Widgets.Add(m_statusStrip);
            Widgets.Add(m_splitDisplayH);

            //Each Node handles the Mouse Click events. The lines below resulted in firring the events twice
            //m_treeAssets.NodeMouseDoubleClick += delegate (object sender, NthDimension.Forms.Events.TreeNodeMouseClickEventArgs e)
                    
            //{
            //    if (e.Node.IsExpanded) e.Node.Collapse(true); else e.Node.Expand();
            //    e.Node.OnMouseDoubleClick(e);
            //};   
        }



        private void m_tree_SelectionChanged()
        {
            // Not Used
            // m_treeAssets.OnSelectionChanged += m_tree_SelectionChanged;     
        }

        public void PreviewTexture(int textureHandle)
        {
            this.m_previewPanel.Widgets.Clear();
            this.m_previewPicture = new Widgets.Picture(textureHandle);
            this.m_previewPicture.Dock = EDocking.Fill;
            this.m_previewPanel.Widgets.Add(m_previewPicture);
        }
        public void PreviewText(string text)
        {
            this.m_previewPanel.Widgets.Clear();
            this.m_previewText = new Widgets.TextEditor.TextEditor();
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                m_previewText.Font = new NanoFont(NanoFont.DefaultRegular, 11f);

            m_previewText.Dock = EDocking.Fill;
            m_previewText.IsReadOnly = true;
            m_previewText.Text = text;

            this.m_previewPanel.Widgets.Add(m_previewText);

        }


    }

}
