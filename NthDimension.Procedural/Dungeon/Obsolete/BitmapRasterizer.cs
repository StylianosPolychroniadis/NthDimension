using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public static class BitmapRasterizerExtensions
    {
        public static void Copy<TPixel>(this BitmapRasterizer<TPixel> self, TPixel[,] src, Rect srcRect, Point dst,
                    Func<TPixel, bool> transprent = null)
                     where TPixel : struct
        {
            int w = srcRect.MaxX - srcRect.X;
            int h = srcRect.MaxY - srcRect.Y;
            var buf = self.Bitmap;

            if (transprent == null)
                transprent = pix => false;

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    var pix = src[x + srcRect.X, y + srcRect.Y];
                    if (transprent(pix))
                        continue;
                    buf[x + dst.X, y + dst.Y] = src[x + srcRect.X, y + srcRect.Y];
                }
        }
    }

    public class BitmapRasterizer<TPixel> where TPixel : struct
    {
        private const int SEG_COUNT = 10;

        private readonly TPixel[,] buffer;

        private readonly int height;

        private readonly int width;

        private readonly bool[][,] caps;

        public TPixel[,] Bitmap
        {
            get
            {
                return this.buffer;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public BitmapRasterizer(int width, int height):base()
        {
            bool[][,]   array                   = new bool[5][,];
                        array[1]                = new bool[,]
                        {
                            {
                                true,
                                true
                            },
                            {
                                true,
                                true
                            }
                        };
                        array[2] = new bool[,]
                        {
                            {
                                false,
                                true,
                                false
                            },
                            {
                                true,
                                true,
                                true
                            },
                            {
                                false,
                                true,
                                false
                            }
                        };
                        array[3] = new bool[,]
                        {
                            {
                                false,
                                true,
                                true,
                                false
                            },
                            {
                                true,
                                true,
                                true,
                                true
                            },
                            {
                                true,
                                true,
                                true,
                                true
                            },
                            {
                                false,
                                true,
                                true,
                                false
                            }
                        };
                        array[4] = new bool[,]
                        {
                            {
                                false,
                                true,
                                true,
                                true,
                                false
                            },
                            {
                                true,
                                true,
                                true,
                                true,
                                true
                            },
                            {
                                true,
                                true,
                                true,
                                true,
                                true
                            },
                            {
                                true,
                                true,
                                true,
                                true,
                                true
                            },
                            {
                                false,
                                true,
                                true,
                                true,
                                false
                            }
                        };

            this.caps = array;
            
            this.buffer = new TPixel[width, height];
            this.width = width;
            this.height = height;
        }

        public void Clear(TPixel bg)
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.buffer[j, i] = bg;
                }
            }
        }

        private void FillRectInternal(int minX, int minY, int maxX, int maxY, TPixel pix)
        {
            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {
                    this.buffer[j, i] = pix;
                }
            }
        }

        private void FillRectInternal(int minX, int minY, int maxX, int maxY, Func<int, int, TPixel> texMapping)
        {
            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {
                    this.buffer[j, i] = texMapping(j, i);
                }
            }
        }

        public void FillRect(int x, int y, int w, int h, TPixel pix)
        {
            this.FillRectInternal(x, y, x + w, y + h, pix);
        }

        public void FillRect(Rect rect, TPixel pix)
        {
            this.FillRectInternal(rect.X, rect.Y, rect.MaxX, rect.MaxY, pix);
        }

        private void ApplyCap(int x, int y, TPixel pix, int width)
        {
            if (width == 1)
            {
                this.buffer[x, y] = pix;
            }
            if (width <= 5)
            {
                bool[,] array = this.caps[width - 1];
                x -= width >> 1;
                y -= width >> 1;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (array[j, i])
                        {
                            this.buffer[x + j, y + i] = pix;
                        }
                    }
                }
                return;
            }
            int num = width >> 1;
            x -= num;
            y -= num;
            this.FillRectInternal(x, y, x + width, y + width, pix);
        }

        private void ApplyCap(int x, int y, Func<int, int, TPixel> texMapping, int width)
        {
            if (width == 1)
            {
                this.buffer[x, y] = texMapping(x, y);
            }
            if (width <= 5)
            {
                bool[,] array = this.caps[width - 1];
                x -= width >> 1;
                y -= width >> 1;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (array[j, i])
                        {
                            this.buffer[x + j, y + i] = texMapping(x + j, y + i);
                        }
                    }
                }
                return;
            }
            int num = width >> 1;
            x -= num;
            y -= num;
            this.FillRectInternal(x, y, x + width, y + width, texMapping);
        }

        public void DrawLine(Point a, Point b, TPixel pix, int width = 1)
        {
            this.DrawLine(a.X, a.Y, b.X, b.Y, pix, width);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, TPixel pix, int width = 1)
        {
            int num = Math.Abs(x1 - x0);
            int num2 = Math.Abs(y1 - y0);
            int num3 = (x0 < x1) ? 1 : -1;
            int num4 = (y0 < y1) ? 1 : -1;
            int num5 = num - num2;
            while (x0 != x1 || y0 != y1)
            {
                this.ApplyCap(x0, y0, pix, width);
                int num6 = 2 * num5;
                if (num6 > -num2)
                {
                    num5 -= num2;
                    x0 += num3;
                }
                else
                {
                    num5 += num;
                    y0 += num4;
                }
            }
            this.ApplyCap(x0, y0, pix, width);
        }

        public void DrawLine(Point a, Point b, Func<int, int, TPixel> texMapping, int width = 1)
        {
            this.DrawLine(a.X, a.Y, b.X, b.Y, texMapping, width);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Func<int, int, TPixel> texMapping, int width = 1)
        {
            int num = Math.Abs(x1 - x0);
            int num2 = Math.Abs(y1 - y0);
            int num3 = (x0 < x1) ? 1 : -1;
            int num4 = (y0 < y1) ? 1 : -1;
            int num5 = num - num2;
            this.ApplyCap(x0, y0, texMapping, width);
            while (x0 != x1 || y0 != y1)
            {
                int num6 = 2 * num5;
                if (num6 > -num2)
                {
                    num5 -= num2;
                    x0 += num3;
                }
                else
                {
                    num5 += num;
                    y0 += num4;
                }
                this.ApplyCap(x0, y0, texMapping, width);
            }
        }

        private void ScanEdge(int x0, int y0, int x1, int y1, int?[] min, int?[] max)
        {
            int num = Math.Abs(x1 - x0);
            int num2 = Math.Abs(y1 - y0);
            int num3 = (x0 < x1) ? 1 : -1;
            int num4 = (y0 < y1) ? 1 : -1;
            int num5 = num - num2;
            if (!min[y0].HasValue || min[y0] > x0)
            {
                min[y0] = new int?(x0);
            }
            if (!max[y0].HasValue || max[y0] < x0)
            {
                max[y0] = new int?(x0);
            }
            while (x0 != x1 || y0 != y1)
            {
                int num6 = 2 * num5;
                if (num6 >= -num2)
                {
                    num5 -= num2;
                    x0 += num3;
                }
                if (num6 <= num)
                {
                    num5 += num;
                    y0 += num4;
                }
                if (!min[y0].HasValue || min[y0] > x0)
                {
                    min[y0] = new int?(x0);
                }
                if (!max[y0].HasValue || max[y0] < x0)
                {
                    max[y0] = new int?(x0);
                }
            }
        }

        public void FillTriangle(Point a, Point b, Point c, TPixel color)
        {
            int num = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            int num2 = Math.Max(a.Y, Math.Max(b.Y, c.Y)) + 1;
            int num3 = num2 - num;
            int?[] array = new int?[num3];
            int?[] array2 = new int?[num3];
            this.ScanEdge(a.X, a.Y - num, b.X, b.Y - num, array, array2);
            this.ScanEdge(b.X, b.Y - num, c.X, c.Y - num, array, array2);
            this.ScanEdge(c.X, c.Y - num, a.X, a.Y - num, array, array2);
            for (int i = num; i < num2; i++)
            {
                int value = array[i - num].Value;
                int value2 = array2[i - num].Value;
                for (int j = value; j <= value2; j++)
                {
                    this.buffer[j, i] = color;
                }
            }
        }

        public void FillTriangle(Point a, Point b, Point c, Func<int, int, TPixel> texMapping)
        {
            int num = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            int num2 = Math.Max(a.Y, Math.Max(b.Y, c.Y)) + 1;
            int num3 = num2 - num;
            int?[] array = new int?[num3];
            int?[] array2 = new int?[num3];
            this.ScanEdge(a.X, a.Y - num, b.X, b.Y - num, array, array2);
            this.ScanEdge(b.X, b.Y - num, c.X, c.Y - num, array, array2);
            this.ScanEdge(c.X, c.Y - num, a.X, a.Y - num, array, array2);
            for (int i = num; i < num2; i++)
            {
                int value = array[i - num].Value;
                int value2 = array2[i - num].Value;
                for (int j = value; j <= value2; j++)
                {
                    this.buffer[j, i] = texMapping(j, i);
                }
            }
        }

        public void DrawBezier(Point a, Point cp, Point b, TPixel pix, int width = 1)
        {
            this.DrawBezier(a.X, a.Y, cp.X, cp.Y, b.X, b.Y, pix, width);
        }

        public void DrawBezier(int x0, int y0, int x1, int y1, int x2, int y2, TPixel pix, int width = 1)
        {
            double num = (double)x0;
            double num2 = (double)y0;
            for (int i = 0; i < 10; i++)
            {
                double num3 = (double)(i + 1) / 10.0;
                double num4 = 1.0 - num3;
                double num5 = num4 * num4 * (double)x0 + 2.0 * num4 * num3 * (double)x1 + num3 * num3 * (double)x2;
                double num6 = num4 * num4 * (double)y0 + 2.0 * num4 * num3 * (double)y1 + num3 * num3 * (double)y2;
                this.DrawLine((int)num, (int)num2, (int)num5, (int)num6, pix, width);
                num = num5;
                num2 = num6;
            }
        }
    }
}
