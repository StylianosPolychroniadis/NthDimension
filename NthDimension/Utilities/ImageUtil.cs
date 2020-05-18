/* LICENSE
 * Copyright (C) 2017 - 2018 SYSCON Software, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@rafasoftware.com) http://www.rafasoftware.com
 * 
 * This file is part of MySoci.Net Social Network
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NthDimension.Utilities
{

    public static class ImageUtil
    {
        const string errorMessage = "Could not recognize image format.";

        private static Dictionary<byte[], Func<BinaryReader, Size>> imageFormatDecoders =
            new Dictionary<byte[], Func<BinaryReader, Size>>()
            {
                {new byte[] {0x42, 0x4D}, DecodeBitmap},
                {new byte[] {0x47, 0x49, 0x46, 0x38, 0x37, 0x61}, DecodeGif},
                {new byte[] {0x47, 0x49, 0x46, 0x38, 0x39, 0x61}, DecodeGif},
                {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}, DecodePng},
                {new byte[] {0xff, 0xd8}, DecodeJfif},
            };

        private static int[] POT = new int[13]
        {
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192
        };

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>
        public static Size GetDimensions(string path)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            {
                try
                {
                    return GetDimensions(binaryReader);
                }
                catch (ArgumentException e)
                {
                    if (e.Message.StartsWith(errorMessage))
                    {
                        throw new ArgumentException(errorMessage, "path", e);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// This function has been found on the internet -> added troubleshooting Jpeg error loading
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Size GetDimensions0(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))

            {
                using (Image tif = Image.FromStream(stream: file,
                                                    useEmbeddedColorManagement: false,
                                                    validateImageData: false))

                {
                    float width = tif.PhysicalDimension.Width;
                    float height = tif.PhysicalDimension.Height;
                    float hresolution = tif.HorizontalResolution;
                    float vresolution = tif.VerticalResolution;

                    return new Size((int)width, (int)height);












                }
            }
            throw new Exception("Failed to get Image Size");
            // Note: Buggy code, requires further JFIF header handling 0xC0 0xC1 0xC2 etc SOF1 SOF2 
            //using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            //{
            //    try
            //    {
            //        return GetDimensions(binaryReader);
            //    }
            //    catch (ArgumentException e)
            //    {
            //        if (e.Message.StartsWith(errorMessage))
            //        {
            //            throw new ArgumentException(errorMessage, "path", e);
            //        }
            //        else
            //        {
            //            throw e;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>    
        public static Size GetDimensions(BinaryReader binaryReader)
        {
            int maxMagicBytesLength = imageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

            byte[] magicBytes = new byte[maxMagicBytesLength];

            for (int i = 0; i < maxMagicBytesLength; i += 1)
            {
                magicBytes[i] = binaryReader.ReadByte();

                foreach (var kvPair in imageFormatDecoders)
                {
                    if (magicBytes.StartsWith(kvPair.Key))
                    {
                        return kvPair.Value(binaryReader);
                    }
                }
            }

            throw new ArgumentException(errorMessage, "binaryReader");
        }

        private static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
        {
            for (int i = 0; i < thatBytes.Length; i += 1)
            {
                if (thisBytes[i] != thatBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static short ReadLittleEndianInt16(this BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(short)];
            for (int i = 0; i < sizeof(short); i += 1)
            {
                bytes[sizeof(short) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        private static int ReadLittleEndianInt32(this BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i += 1)
            {
                bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        private static Size DecodeBitmap(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(16);
            int width = binaryReader.ReadInt32();
            int height = binaryReader.ReadInt32();
            return new Size(width, height);
        }

        private static Size DecodeGif(BinaryReader binaryReader)
        {
            int width = binaryReader.ReadInt16();
            int height = binaryReader.ReadInt16();
            return new Size(width, height);
        }

        private static Size DecodePng(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            int width = binaryReader.ReadLittleEndianInt32();
            int height = binaryReader.ReadLittleEndianInt32();
            return new Size(width, height);
        }

        private static Size DecodeJfif(BinaryReader binaryReader)
        {
            while (binaryReader.ReadByte() == 0xff)
            {
                byte marker = binaryReader.ReadByte();
                short chunkLength = binaryReader.ReadLittleEndianInt16();

                if (marker == 0xc0)
                {
                    binaryReader.ReadByte();

                    int height = binaryReader.ReadLittleEndianInt16();
                    int width = binaryReader.ReadLittleEndianInt16();
                    return new Size(width, height);
                }

                binaryReader.ReadBytes(chunkLength - 2);
            }

            throw new ArgumentException(errorMessage);
        }

        public static Bitmap Crop(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, System.Drawing.Imaging.PixelFormat.DontCare);
        }

        public static Size GetPOTSize(Size original)
        {
            Size ret = new Size(32, 32);

            if (original.Width < 32)
                ret.Width = 32;

            if (original.Height < 32)
                original.Height = 32;

            if (original.Width > 8192)
                ret.Width = 8192;

            if (original.Height > 8192)
                ret.Height = 8192;

            for (int wi = 0; wi < POT.Length - 1; wi++)
            {
                if (POT.Contains(original.Width))
                {
                    ret.Width = original.Width;
                    break;
                }
                else if (original.Width > POT[wi] &&
                    original.Width < POT[wi + 1])
                    ret.Width = POT[wi + 1];
            }

            for (int wi = 0; wi < POT.Length - 1; wi++)
            {
                if (POT.Contains(original.Height))
                {
                    ret.Height = original.Height;
                    break;
                }
                else if (original.Height >= POT[wi] &&
                    original.Height <= POT[wi + 1])
                    ret.Height = POT[wi + 1];
            }

            return ret;
        }


        //private static ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

        public static Bitmap Superimpose(string imagePath, Size pot)
        {
            Bitmap potBmp = new Bitmap(pot.Width, pot.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Image srcBmp = FromFile(imagePath);


            #region Obsolete (checks to see if image is part of resources and skips it - not needed
            //foreach (DictionaryEntry entry in resourceSet)
            //{
            //    string resourceKey = entry.Key.ToString();
            //    object resource = entry.Value;

            //    if (resource is Bitmap && resourceKey == Path.GetFileNameWithoutExtension(imagePath))
            //        return (Bitmap)srcBmp;
            //}
            #endregion

            Rectangle destRect = new Rectangle(pot.Width / 2 - srcBmp.Width / 2, pot.Height / 2 - srcBmp.Height / 2, srcBmp.Width, srcBmp.Height);

            using (var g = System.Drawing.Graphics.FromImage(potBmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(srcBmp, destRect);

            }

            return potBmp;
        }

        public static Image FromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }

    }

}
