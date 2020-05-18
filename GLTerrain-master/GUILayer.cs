using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public enum GUIObjectType {
        None,
        Label,
        Frame,
        Button,
        Texture
    }

    public enum Layer {
        Layer0,
        Layer1,
        Layer2,
        Layer3
    }

    public delegate void ClickedDelegate(int x, int y, MouseButton button);

    public class GUILayer : IDisposable {

        public static Font DefaultFont = new Font("Arial", 10.0f);

        private Texture canvas = null;
        private Geometry fullScreenQuad = null;
        private Dictionary<string, GUIObject> objects = new Dictionary<string, GUIObject>();
        private Matrix4 orthoMatrix = Matrix4.Identity;
        private bool isDirty = true;

        public GUILayer(Viewport view) {
            ResizeView(view);

            fullScreenQuad = new Geometry(BeginMode.Quads,
                new VertexBuffer(new Vertex[] {
                new Vertex() {X = 0, Y = view.Height, Z = 0},
                new Vertex() {X = view.Width, Y = view.Height, Z = 0},
                new Vertex() {X = view.Width, Y = 0, Z = 0},
                new Vertex() {X = 0, Y = 0, Z = 0}
            }, new TexCoord[] {
                new TexCoord() {U = 0, V = 1.0f},
                new TexCoord() {U = 1.0f, V = 1.0f},
                new TexCoord() {U = 1.0f, V = 0},
                new TexCoord() {U = 0, V = 0}
            }));
        }

        public void AddLabel(string name, string text, Font font, int x, int y, Layer layer, Color color) {
            objects.Add(name, 
                new GUIObject() { 
                Name = name, 
                Text = text,
                Font = font,
                ForeColor = color,
                Location = new Point(x, y),
                Layer = layer,
                Type = GUIObjectType.Label
            });
        }

        public void AddLabel(string name, string text, int x, int y, Layer layer, Color color) {
            AddLabel(name, text, DefaultFont, x, y, layer, color);
        }

        public void AddFrame(string name, int x, int y, int width, int height, Layer layer, Color color) {
            objects.Add(name, new GUIObject() {
                Name = name,
                ForeColor = color,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Layer = layer,
                Type = GUIObjectType.Frame
            });
        }

        public void AddTexture(string name, int x, int y, int width, int height, Layer layer, Texture tex) {
            objects.Add(name, new GUIObject() {
                Name = name,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Layer = layer,
                UserData = tex,
                Type = GUIObjectType.Texture
            });
        }

        public void Refresh() {
            isDirty = true;
        }

        public bool UpdateObjectText(string name, string newText) {
            if (objects.ContainsKey(name)) {
                objects[name].Text = newText;
                isDirty = true;
                return true;
            }
            return false;
        }

        public void ResizeView(Viewport newView) {
            orthoMatrix = Matrix4.CreateOrthographicOffCenter(
                0, newView.Width, newView.Height, 0, -100, 100);

            if (canvas != null) {
                canvas.Dispose();
            }

            using (Bitmap blankBitmap = new Bitmap(newView.Width, newView.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
                canvas = new Texture(blankBitmap, true, false);
            }

            isDirty = true;
        }

        public void Update(int mouseX, int mouseY, MouseButton buttons) {
            foreach (KeyValuePair<string, GUIObject> kvp in objects) {
                switch (kvp.Value.Type) {
                    case GUIObjectType.Button:
                        if (kvp.Value.OnClick != null) {
                            kvp.Value.OnClick(mouseX, mouseY, buttons);
                        }
                        break;
                }
            }
        }

        public void Draw() {
            // load ortho projection
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();

            GL.LoadMatrix(ref orthoMatrix);

            GL.PushAttrib(AttribMask.CurrentBit | AttribMask.DepthBufferBit | AttribMask.LightingBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Lighting);
                GL.LoadIdentity();
                GL.Disable(EnableCap.DepthTest);

                if (isDirty) {
                    UpdateTexture();
                }

                canvas.Bind();
                fullScreenQuad.Draw();
                canvas.Unbind();
            }
            GL.PopMatrix();
            GL.PopAttrib();

            // pop ortho projection and set mode back to modelview
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void UpdateTexture() {
            using (Graphics graphics = canvas.GetImageContext()) {
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                List<KeyValuePair<string, GUIObject>> guiObjects = 
                    new List<KeyValuePair<string, GUIObject>>(objects);

                guiObjects.Sort(new Comparison<KeyValuePair<string, GUIObject>>(
                    delegate(KeyValuePair<string, GUIObject> obj1, KeyValuePair<string, GUIObject> obj2) {
                        return (obj1.Value.Layer < obj2.Value.Layer) ? -1 : 
                               (obj1.Value.Layer == obj2.Value.Layer ? 0 : 1);
                    }));

                foreach (KeyValuePair<string, GUIObject> kvp in guiObjects) {
                    switch (kvp.Value.Type) {
                        case GUIObjectType.Label:
                            DrawLabel(graphics, kvp.Value);
                            break;
                        case GUIObjectType.Frame:
                            DrawFrame(graphics, kvp.Value);
                            break;
                        case GUIObjectType.Button:
                            // DrawButton(graphics, obj);
                            break;
                        case GUIObjectType.Texture:
                            DrawTexture(graphics, kvp.Value);
                            break;
                    }
                }
            }

            canvas.RefreshTexture();
            isDirty = false;
        }

        private void DrawLabel(Graphics g, GUIObject obj) {
            using (SolidBrush brush = new SolidBrush(obj.ForeColor)) {
                g.DrawString(obj.Text, obj.Font, brush, new PointF(obj.Location.X, obj.Location.Y));
            }
        }

        private void DrawFrame(Graphics g, GUIObject obj) {
            using (SolidBrush brush = new SolidBrush(obj.ForeColor)) {
                g.FillRectangle(brush, obj.Location.X, obj.Location.Y, obj.Size.Width, obj.Size.Height);
            }
        }

        private void DrawTexture(Graphics g, GUIObject obj)  {
            Texture tex = obj.UserData as Texture;
            if (tex != null && tex.IsDynamic) {
                Bitmap b = tex.GetImageData();
                g.DrawImage(b, obj.Location.X, obj.Location.Y, obj.Size.Width, obj.Size.Height);
            }
        }

        private class GUIObject {
            public string Text { get; set; }
            public string Name { get; set; }
            public Font Font { get; set; }
            public Layer Layer { get; set; }
            public Color ForeColor { get; set; }
            public Point Location { get; set; }
            public Size Size { get; set; }
            public GUIObjectType Type { get; set; }
            public ClickedDelegate OnClick { get; set; }
            public object UserData { get; set; }
        }

        public void Dispose() {
            canvas.Dispose();
            fullScreenQuad.Dispose();
        }
    }
}
