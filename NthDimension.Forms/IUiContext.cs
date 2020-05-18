using System.Drawing;

namespace NthDimension.Forms
{
    public interface IUiContext
    {
        Point CursorPos
        {
            get;
        }

        bool ShowCursor
        {
            set;
        }

        Size MeasureText(string text, NanoFont font);
        Size MeasureGlyph(byte[] text, NanoFont font);
        float MeasureTextWidth(string text, NanoFont font);

        FontParameters GetFontParemeters(NanoFont font, int id, float height);
        FontParameters CreateFont(NanoFont font);
    }
}
