/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

#define UNSAFE 

namespace NthDimension.Rendering.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    public class CavityGenerator
    {
        private Bitmap                  NormalMap;
        private Bitmap                  ProcessingBitmap;
        private int                     processedPixels         = 0;
        private bool                    SwichX;
        private bool                    SwichY;
        private string                  SaveToFile;

        private BackgroundWorker        backgroundWorker;
        private Action<int>             reportProgressCallback;

        public CavityGenerator(Bitmap normalMap, string saveFile, bool switchX = false, bool switchY = false, Action<int> progressCallback = null)
        {
#if !UNSAFE
            throw new Exception("Unsafe execution is set to disabled! Please enable and try again");
#endif
            this.NormalMap = normalMap;
            this.SaveToFile = saveFile;
            this.SwichX = switchX;
            this.SwichY = switchY;
            this.reportProgressCallback = progressCallback;
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
#if UNSAFE
            this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
#endif
            this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
        }

#if UNSAFE
        private unsafe void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap bitmap = this.NormalMap;//new Bitmap(this.customPicturebox.Image);
            this.ProcessingBitmap = this.NormalMap;//new Bitmap(this.customPicturebox.Image);
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                e.Result = false;
                throw new Exception("Pixel Format is not Format32bppArgb");
            }
            Rectangle rect = new Rectangle(0, 0, this.ProcessingBitmap.Width, this.ProcessingBitmap.Height);
            BitmapData bmpDataR = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            BitmapData bmpDataW = this.ProcessingBitmap.LockBits(rect, ImageLockMode.WriteOnly, this.ProcessingBitmap.PixelFormat);
            Math.Abs(bmpDataR.Stride);
            int arg_CA_0 = bmpDataR.Height;
            int totalPixels = bmpDataR.Width * bmpDataR.Height;
            int lineW = bmpDataR.Width;
            int segmentSize = totalPixels / Environment.ProcessorCount;
            this.processedPixels = 0;
            Action<object> action = delegate (object readerStart)
            {
                bmpDataW.Scan0.ToPointer();
                PixelData* ptr = (PixelData*)bmpDataW.Scan0.ToPointer();
                PixelData* ptr2;
                PixelData* ptr3;
                if (!this.SwichX)
                {
                    ptr2 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() + sizeof(PixelData));
                    ptr3 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() - sizeof(PixelData));
                }
                else
                {
                    ptr2 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() - sizeof(PixelData));
                    ptr3 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() + sizeof(PixelData));
                }
                PixelData* ptr4;
                PixelData* ptr5;
                if (!this.SwichY)
                {
                    ptr4 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() + lineW * sizeof(PixelData));
                    ptr5 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() - lineW * sizeof(PixelData));
                }
                else
                {
                    ptr4 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() - lineW * sizeof(PixelData));
                    ptr5 = (PixelData*)((byte*)bmpDataR.Scan0.ToPointer() + lineW * sizeof(PixelData));
                }
                int num = segmentSize * (int)readerStart;
                ptr += num;
                ptr2 += num;
                ptr3 += num;
                ptr4 += num;
                ptr5 += num;
                for (int k = 0; k < segmentSize; k++)
                {
                    this.processedPixels++;
                    int num2 = 128;
                    if ((k + 1) % lineW != 0 && k % lineW > 0)
                    {
                        if (ptr3->Red != 0)
                        {
                            num2 = (int)(ptr2->Red * 128 / ptr3->Red);
                        }
                        else
                        {
                            num2 = 255;
                        }
                    }
                    if (k + lineW + num < totalPixels && k - lineW > 0)
                    {
                        if (ptr4->Green != 0)
                        {
                            num2 = num2 / 2 + (int)(ptr5->Green * 64 / ptr4->Green);
                        }
                        else
                        {
                            num2 = num2 / 2 + 128;
                        }
                    }
                    byte b;
                    if (num2 < 256 && num2 >= 0)
                    {
                        b = (byte)num2;
                    }
                    else if (num2 >= 256)
                    {
                        b = 255;
                    }
                    else
                    {
                        b = 0;
                    }
                    ptr->Red = b;
                    ptr->Green = b;
                    ptr->Blue = b;
                    ptr++;
                    ptr2++;
                    ptr4++;
                    ptr3++;
                    ptr5++;
                }
            };
            Task[] array = new Task[Environment.ProcessorCount];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Task.Factory.StartNew(action, i);
            }
            bool flag = true;
            while (flag)
            {
                Thread.Sleep(100);
                this.backgroundWorker.ReportProgress(this.processedPixels * 100 / totalPixels);
                flag = false;
                Task[] array2 = array;
                for (int j = 0; j < array2.Length; j++)
                {
                    Task task = array2[j];
                    if (task.Status == TaskStatus.Running)
                    {
                        flag = true;
                    }
                }
            }
            bitmap.UnlockBits(bmpDataR);
            this.ProcessingBitmap.UnlockBits(bmpDataW);
        }
#endif

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
                return; // Raise 100% when the file has been saved

            if (null != this.reportProgressCallback)
                this.reportProgressCallback(e.ProgressPercentage);
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            this.ProcessingBitmap.Save(this.SaveToFile);

            if (null != this.reportProgressCallback)            // Done!
                this.reportProgressCallback(100);
        }

        public Bitmap CreateCavityMap()
        {

            return ProcessingBitmap;
        }

    }
}
