using System.Collections.Generic;
using NthDimension.Algebra;
using NthDimension.Graphics;
using NthDimension.Graphics.Drawables;
using NthDimension.Graphics.Frames;
using NthDimension.Graphics.Manipulators;
using NthDimension.Graphics.Modelling;
using NthDimension.Rasterizer.GL1;
using NthDimension.Graphics.Renderer;
using NthDimension.Graphics.Scenegraph;

namespace TexLib
{
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System.Diagnostics;
    using System.Drawing;
    using Img = System.Drawing.Imaging;

    /// <summary>
    /// The TexUtil class is released under the MIT-license.
    /// /Olof Bjarnason
    /// </summary>
    

    //public class TextureFont : SceneElement, IDrawable, ISpatial, ITransformable
    //{
    //    //public StateChangeStartEventHandler OnStateChangeStart;
    //    //public StateChangeEndHandler OnStateChangeEnd;

    //    private string m_msg = "Font";
    //    private TransformationFrame m_transformation;
    //    protected readonly RendererBase _renderer;

    //    private Vector3 m_position = new Vector3();
    //    private Vector3 m_barycentro = new Vector3();
    //    private bool    m_ispicking = false;
    //    private bool    m_preview = false;
    //    private int m_idx = -1;

    //    public string Text
    //    {
    //        get { return m_msg; }
    //        set { m_msg = value; }
    //    }

    //    public bool Preview { get { return m_preview; } set { m_preview = value; } }

    //    /// <summary>
    //    /// Create a TextureFont object. The sent-in textureId should refer to a
    //    /// texture bitmap containing a 16x16 grid of fixed-width characters,
    //    /// representing the ASCII table. A 32 bit texture is assumed, aswell as
    //    /// all GL state necessary to turn on texturing. The dimension of the
    //    /// texture bitmap may be anything from 128x128 to 512x256 or any other
    //    /// order-by-two-squared-dimensions.
    //    /// </summary>
    //    public TextureFont(RendererBase renderer, Bitmap bitmap, TextureUnit unit, SceneElement parent)
    //        :base(parent)
    //    {
    //        this._renderer = renderer;
    //        this.m_transformation = new TransformationFrame(renderer);
    //        ((SceneElement)this).Name = "Font";
    //        this.m_idx = RendererGL.TexUtil.CreateTextureFromBitmap(bitmap);


    //    }

    //    /// <summary>
    //    /// Draw an ASCII string around coordinate (0,0,0) in the XY-plane of the
    //    /// model space coordinate system. The height of the text is 1.0.
    //    /// The width may be computed by calling ComputeWidth(string).
    //    /// This call modifies the currently bound
    //    /// 2D-texture, but no other GL state.
    //    /// </summary>
    //    public void WriteString(string text)
    //    {
    //        GL.BindTexture(TextureTarget.Texture2D, textureId);
    //        //GL.PushMatrix();
    //        double width = ComputeWidth(text);
    //        //GL.Translate(-width / 2.0, -0.5, 0);
    //        GL.Begin(BeginMode.Quads);
    //        double xpos = 0;
    //        foreach (var ch in text)
    //        {
    //            WriteCharacter(ch, xpos);
    //            xpos += AdvanceWidth;
    //        }
    //        GL.End();
    //        //GL.PopMatrix();
    //    }

    //    /// <summary>
    //    /// Determines the distance from character center to adjacent character center, horizontally, in
    //    /// one written text string. Model space coordinates.
    //    /// </summary>
    //    public double AdvanceWidth = 0.75;

    //    /// <summary>
    //    /// Determines the width of the cut-out to do for each character when rendering. This is necessary
    //    /// to avoid artefacts stemming from filtering (zooming/rotating). Make sure your font contains some
    //    /// "white space" around each character so they won't be clipped due to this!
    //    /// </summary>
    //    public double CharacterBoundingBoxWidth = 0.8;

    //    /// <summary>
    //    /// Determines the height of the cut-out to do for each character when rendering. This is necessary
    //    /// to avoid artefacts stemming from filtering (zooming/rotating). Make sure your font contains some
    //    /// "white space" around each character so they won't be clipped due to this!
    //    /// </summary>
    //    public double CharacterBoundingBoxHeight = 0.8;//{ get { return 1.0 - borderY * 2; } set { borderY = (1.0 - value) / 2.0; } }

    //    /// <summary>
    //    /// Computes the expected width of text string given. The height is always 1.0.
    //    /// Model space coordinates.
    //    /// </summary>
    //    public double ComputeWidth(string text)
    //    {
    //        return text.Length * AdvanceWidth;
    //    }

        

    //    /// <summary>
    //    /// This is a convenience function to write a text string using a simple coordinate system defined to be 0..100 in x and 0..100 in y.
    //    /// For example, writing the text at 50,50 means it will be centered onscreen. The height is given in percent of the height of the viewport.
    //    /// No GL state except the currently bound texture is modified. This method is not as flexible nor as fast
    //    /// as the WriteString() method, but it is easier to use.
    //    /// </summary>
    //    public void WriteStringAt(
    //      string text,
    //      double heightPercent,
    //      double xPercent,
    //      double yPercent,
    //      double degreesCounterClockwise)
    //    {
    //        GL.MatrixMode(MatrixMode.Projection);
    //        GL.PushMatrix();
    //        GL.LoadIdentity();
    //        GL.Ortho(0, 100, 0, 100, -1, 1);
    //        GL.MatrixMode(MatrixMode.Modelview);
    //        GL.PushMatrix();
    //        GL.LoadIdentity();
    //        GL.Translate(xPercent, yPercent, 0);
    //        double aspectRatio = ComputeAspectRatio();
    //        GL.Scale(aspectRatio * heightPercent, heightPercent, heightPercent);
    //        GL.Rotate(degreesCounterClockwise, 0, 0, 1);
    //        WriteString(text);
    //        GL.PopMatrix();
    //        GL.MatrixMode(MatrixMode.Projection);
    //        GL.PopMatrix();
    //        GL.MatrixMode(MatrixMode.Modelview);
    //    }

    //    private static double ComputeAspectRatio()
    //    {
    //        int[] viewport = new int[4];
    //        GL.GetInteger(GetPName.Viewport, viewport);
    //        int w = viewport[2];
    //        int h = viewport[3];
    //        double aspectRatio = (float)h / (float)w;
    //        return aspectRatio;
    //    }

    //    private void WriteCharacter(char ch, double xpos)
    //    {
    //        byte ascii;
    //        unchecked { ascii = (byte)ch; }

    //        int row = ascii >> 4;
    //        int col = ascii & 0x0F;

    //        double centerx = (col + 0.5) * Sixteenth;
    //        double centery = (row + 0.5) * Sixteenth;
    //        double halfHeight = CharacterBoundingBoxHeight * Sixteenth / 2.0;
    //        double halfWidth = CharacterBoundingBoxWidth * Sixteenth / 2.0;
    //        double left = centerx - halfWidth;
    //        double right = centerx + halfWidth;
    //        double top = centery - halfHeight;
    //        double bottom = centery + halfHeight;

    //        GL.TexCoord2(left, top); GL.Vertex2(xpos, 1);
    //        GL.TexCoord2(right, top); GL.Vertex2(xpos + 1, 1);
    //        GL.TexCoord2(right, bottom); GL.Vertex2(xpos + 1, 0);
    //        GL.TexCoord2(left, bottom); GL.Vertex2(xpos, 0);
    //    }

    //    private int textureId;
    //    private const double Sixteenth = 1.0 / 16.0;

    //    public void Render(SYSCON.Graphics.Renderer.RendererBase renderer)
    //    {
    //        RendererGL.TexUtil.InitTexturing();

    //        GL.MatrixMode(MatrixMode.Texture);

    //        WriteString(m_msg);

    //        GL.MatrixMode(MatrixMode.Modelview);

    //        if(m_preview)
    //            WriteStringAt(m_msg, 10d, 10d, 10d, 0d);

    //        RendererGL.TexUtil.EndTexturing();
    //    }

    //    public void ResetRenderer(SYSCON.Graphics.Renderer.RendererBase renderer)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public SYSCON.Graphics.enuRasterizerMode RasterizerMode
    //    {
    //        get
    //        {
    //            return enuRasterizerMode.Direct;
    //        }
    //        set
    //        {
    //            throw new System.NotImplementedException();
    //        }
    //    }

        
        
    //    public void PushObjectSpace()
    //    {

    //        _renderer.PushMatrix();
    //        //_renderer.PushMatrix();

    //        Transformation.Transform();
    //    }
    //    public void PopObjectSpace()
    //    {
    //        _renderer.PopMatrix();
    //        //_renderer.PopMatrix();
    //    }

    //    public TransformationFrame Transformation
    //    {
    //        get { return m_transformation; }
    //        internal set { m_transformation = value; }
    //    }

    //    public void TranslateLocal(Vector3 translate)
    //    {
    //        if(!Enabled)
    //            return;

    //        // Relies on Model
    //        //if (null != this.OnStateChangeStart)
    //        //    this.OnStateChangeStart(this);

    //        Matrix4 translation = Matrix4.CreateTranslation(translate);
    //        this.Transformation.TranslationMatrix *= translation;
    //        this.Transformation.Transform();

    //        foreach (SceneElement se in this.Children)
    //        {
    //            recursiveTranslate(se, translation);
    //        }
    //    }
    //    private void recursiveTranslate(SceneElement element, Matrix4 translation)
    //    {
    //        if (element is ISpatial && element.Enabled)
    //        {
    //            ((ISpatial)element).Transformation.TranslationMatrix += translation;
    //            ((ISpatial)element).Transformation.Transform();
    //        }

    //        foreach (SceneElement child in element.Children)
    //        {
    //            if (child.Enabled)
    //                recursiveTranslate(child, translation);
    //        }
    //    }
    //    public void RotateLocal(Matrix4 rotate)
    //    {
            
    //    }
    //    public void ScaleLocal(Vector3 scale)
    //    {
            
    //    }

    //    #region Properties
    //    public Vector3 Position { get { return m_position; } }
    //    public Vector3 BaryCentro { get { return m_barycentro; } }
    //    public bool IsSolid { get { return false; } }
    //    public bool IsPicking { get { return m_ispicking; } set { m_ispicking = value; } }
        
    //    #endregion
    //}

    //public static class TextureManager
    //{
    //    public static Dictionary<int, string> TextureList = new Dictionary<int, string>();
    //}

    //public class IndexedBitmap
    //{
    //    public Bitmap Bitmap;
    //    public int Index;
    //    public IndexedBitmap(Bitmap b)
    //    {
    //        Bitmap = b;
    //    }
    //}
}
