using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NthDimension.Utilities;

namespace NthDimension.Rendering.Imaging
{
    public class TGAFile
    {
        public static Image Read(string file)
        {
            FileInfo f = new FileInfo(file);
            if (!f.Exists)
                throw new System.ArgumentException("File " + f.FullName + " does not exist");

            FileStream fs = f.OpenRead();
            Image img = ReadTGA(fs);
            fs.Close();
            return img;
        }

        public static Image Read(FileInfo file)
        {
            if (!file.Exists)
                throw new System.ArgumentException("File " + file.FullName + " does not exist");
            FileStream fs = file.OpenRead();
            Image img = ReadTGA(fs);
            fs.Close();
            return img;
        }

        public static Image Read(Stream stream)
        {
            return ReadTGA(stream);
        }

        internal static int GetBits(byte b, int offset, int count)
        {
            return (b >> offset) & ((1 << count) - 1);
        }

        protected static Bitmap ReadTGA(Stream stream)
        {
            int IDLen = stream.ReadByte();
            int colorType = stream.ReadByte();
            int imageType = stream.ReadByte();

            UInt16 entryOffset = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            UInt16 mapSize = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            int mapBPP = stream.ReadByte();

            UInt16 xOrigin = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            UInt16 yOrigin = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            UInt16 width = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            UInt16 height = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
            int bpp = stream.ReadByte();
            int descriptor = stream.ReadByte();

            byte[] IDField;
            if (IDLen > 0)
            {
                IDField = new byte[IDLen];
                stream.Read(IDField, 0, IDLen);
            }

            byte[] ColorMap = null;
            if (mapSize > 0)
            {
                ColorMap = new byte[mapSize * (mapBPP / 8)];
                stream.Read(ColorMap, 0, ColorMap.Length);
            }

            Bitmap bitmap = null;

            if (colorType == 1)
            {
                if (imageType != 1 || ColorMap == null)
                    throw new System.NotImplementedException("Uncompressed images only");

                if (mapBPP > 24)
                    bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                else
                    bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                Color[] pallet = new Color[mapSize];
                int offset = 0;
                for (int i = 0; i < mapSize; i++)
                {
                    byte A = 255;
                    if (mapBPP > 24)
                        A = ColorMap[offset + 3];

                    pallet[i] = Color.FromArgb(A, ColorMap[offset + 2], ColorMap[offset + 1], ColorMap[offset]);

                    offset += mapBPP / 8;
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = 0;
                        if (mapSize > 256)
                            index = (UInt16)BinUtils.ReadObject(stream, typeof(UInt16));
                        else
                            index = (byte)BinUtils.ReadObject(stream, typeof(byte));

                        bitmap.SetPixel(x, (height - 1) - y, pallet[index]);
                    }
                }
            }
            else if (colorType == 0)
            {
                if (imageType != 2 && imageType != 10)
                    throw new System.NotImplementedException("Uncompressed images only");

                if (bpp > 24)
                    bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                else
                    bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                if (imageType == 2) // raw RGB
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            byte[] pixel;
                            if (bpp > 24)
                                pixel = new byte[4];
                            else
                                pixel = new byte[3];

                            stream.Read(pixel, 0, pixel.Length);

                            byte A = 255;
                            if (bpp > 24)
                                A = pixel[3];

                            bitmap.SetPixel(x, (height - 1) - y, Color.FromArgb(A, pixel[2], pixel[1], pixel[0]));
                        }
                    }
                }
                else // rle
                {
                    int totalPixels = height * width;
                    int pixelCount = 0;
                    int x = 0;
                    int y = 0;
                    while (pixelCount < totalPixels)
                    {
                        byte packetHeader = 0;
                        packetHeader = (byte)stream.ReadByte();

                        int type = GetBits(packetHeader, 7, 1);
                        int count = GetBits(packetHeader, 0, 7) + 1;

                        if (count == 0)
                            continue;

                        if (type > 0) // RLE packet
                        {
                            byte[] pixel;
                            if (bpp > 24)
                                pixel = new byte[4];
                            else
                                pixel = new byte[3];

                            stream.Read(pixel, 0, pixel.Length);

                            byte A = 255;
                            if (bpp > 24)
                                A = pixel[3];

                            Color color = Color.FromArgb(A, pixel[2], pixel[1], pixel[0]);
                            pixelCount += count;
                            for (int i = 0; i < count; i++)
                            {
                                bitmap.SetPixel(x, (height - 1) - y, color);
                                x++;
                                if (x >= width)
                                {
                                    x = 0;
                                    y++;
                                }
                            }
                        }
                        else // raw packet
                        {
                            pixelCount += count;

                            for (int i = 0; i < count; i++)
                            {
                                byte[] pixel;
                                if (bpp > 24)
                                    pixel = new byte[4];
                                else
                                    pixel = new byte[3];

                                stream.Read(pixel, 0, pixel.Length);

                                byte A = 255;
                                if (bpp > 24)
                                    A = pixel[3];

                                bitmap.SetPixel(x, (height - 1) - y, Color.FromArgb(A, pixel[2], pixel[1], pixel[0]));

                                x++;
                                if (x >= width)
                                {
                                    x = 0;
                                    y++;
                                }
                            }
                        }
                    }
                }
            }
            else
                throw new System.InvalidOperationException("Unknown image type:" + colorType.ToString());

            return bitmap;
        }
    }
}
