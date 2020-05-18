using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

// NOTE:: For use with cityGen Heightmaps

namespace NthDimension.Utilities
{
    /*
    There's no need for LockBits or SetPixel. Use the above class for direct access to bitmap data.

    With this class, it is possible to set raw bitmap data as 32-bit data. Notice that it is PARGB, 
    which is premultiplied alpha. See Alpha Compositing on Wikipedia for more information on how this 
    works and examples on the MSDN article for BLENDFUNCTION to find out how to calculate the alpha 
    properly.

    If premultiplication might overcomplicate things, use PixelFormat.Format32bppArgb instead. A 
    performance hit occurs when it's drawn, because it's internally being converted to 
    PixelFormat.Format32bppPArgb. If the image doesn't have to change prior to being drawn, 
    the work can be done before premultiplication, drawn to a PixelFormat.Format32bppArgb buffer, 
    and further used from there.

    Access to standard Bitmap members is exposed via the Bitmap property. Bitmap data is directly 
    accessed using the Bits property.
    Using byte instead of int for raw pixel data

    Change both instances of Int32 to byte, and then change this line:

    Bits = new Int32[width * height];

    To this:

    Bits = new byte[width * height * 4];

    When bytes are used, the format is Alpha/Red/Green/Blue in that order. Each pixel takes 4 bytes of 
    data, one for each channel. The GetPixel and SetPixel functions will need to be reworked 
    accordingly or removed.
    Benefits to using the above class

        Memory allocation for merely manipulating the data is unnecessary; changes made to the raw data 
        are immediately applied to the bitmap.
        There are no additional objects to manage. This implements IDisposable just like Bitmap.
        It does not require an unsafe block.

    Considerations

        Pinned memory cannot be moved. It's a required side effect in order for this kind of memory 
        access to work. This reduces the efficiency of the garbage collector (MSDN Article). Do it only 
        with bitmaps where performance is required, and be sure to Dispose them when you're done so the 
        memory can be unpinned.

    Access via the Graphics object

    Because the Bitmap property is actually a .NET Bitmap object, it's straightforward to perform 
    operations using the Graphics class.

    var dbm = new DirectBitmap(200, 200);
    using (var g = Graphics.FromImage(dbm.Bitmap))
    {
        g.DrawRectangle(Pens.Black, new Rectangle(50, 50, 100, 100));
    }

    Performance comparison

    The question asks about performance, so here's a table that should show the relative performance 
    between the three different methods proposed in the answers. This was done using a .NET Standard 2 
    based application and NUnit.

    * Time to fill the entire bitmap with red pixels *
    - Not including the time to create and dispose the bitmap
    - Best out of 100 runs taken
    - Lower is better
    - Time is measured in Stopwatch ticks to emphasize magnitude rather than actual time elapsed
    - Tests were performed on an Intel Core i7-4790 based workstation

                  Bitmap size
    Method        4x4   16x16   64x64   256x256   1024x1024   4096x4096
    DirectBitmap  <1    2       28      668       8219        178639
    LockBits      2     3       33      670       9612        197115
    SetPixel      45    371     5920    97477     1563171     25811013

    * Test details *

    - LockBits test: Bitmap.LockBits is only called once and the benchmark
                     includes Bitmap.UnlockBits. It is expected that this
                     is the absolute best case, adding more lock/unlock calls
                     will increase the time required to complete the operation.



    */
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
