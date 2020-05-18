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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using NthDimension.Rendering.Serialization;


namespace NthDimension.Rendering
{
    [Serializable, ProtoContract]
    public struct Texture
    {
        [ProtoContract]
        public enum Type
        {
            [ProtoMember(10)] fromPng,
            [ProtoMember(20)] fromFramebuffer,
            [ProtoMember(30)] fromDds,
            [ProtoMember(40)] fromVideo
        };

        [ProtoMember(40)] public int            texture;
        [ProtoMember(50)] public string         name;
        [ProtoMember(60)] public string         pointer;
        [ProtoMember(70)] public Type           type;

        [ProtoMember(80)] public bool           loaded;
        [ProtoMember(90)] public int            identifier;
        [ProtoMember(100)] public bool          multisampling;
        [ProtoMember(110)] public byte[]        cacheBitmap;
        [ProtoMember(111)] public byte[]        cacheDDS;
        public Bitmap                           bitmap;
        [ProtoMember(130)] public int           wrapS;
        [ProtoMember(140)] public int           wrapT;

        public bool                             Mipmaped;       // TODO:: Decide if we keep it
        public int                              Mipmaps;        // TODO:: Decide if we keep it
        public long                             Size
        {
            get
            {
                if (null == cacheBitmap) return 0L;
                return cacheBitmap.Length;
            }
        }
        public long                             SizeBitmap
        {

            get
            {
                Byte[] bmp = GenericMethods.ObjectToByteArray<Bitmap>(bitmap);
                return bmp.Length * sizeof(Byte);
            }
        }
        public override string ToString()
        {
            return name;
        }

        internal Texture nameOnly()
        {
            Texture tmpTexture = new Texture();

            tmpTexture.name = name;

            return tmpTexture;
        }

        public void cacheTexture(ref ListTexture mList)
        {
            if (!loaded)
                return;

            Texture tmpTex = new Texture();

            tmpTex.name = name;
            tmpTex.identifier = identifier;
            tmpTex.pointer = pointer;

            if (type == Type.fromPng)
            {
                tmpTex.name = name;
                tmpTex.cacheBitmap = cacheBitmap;
                tmpTex.multisampling = multisampling;

                mList.Add(tmpTex);
            }
            if (type == Type.fromDds)
            {
                // TODO:: Serialize DDS Raw Bytes

                //tmpTex.name = name;
                //tmpTex.cacheDDS = cacheDDS;
                //tmpTex.multisampling = multisampling;
                //tmpTex.type = Type.fromDds;
                //mList.Add(tmpTex);


                return;
            }


        }

        public Bitmap CacheBitmap
        {
            get
            {
                MemoryStream ms = new MemoryStream(cacheBitmap);
                Image returnImage = Image.FromStream(ms);
                return (Bitmap)returnImage;
            }
            set
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    value.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Convert Image to byte[]
                    cacheBitmap = ms.ToArray();
                }
            }
        }

        //public byte[] CacheDDS
        //{
        //    get
        //    {
        //        MemoryStream ms = new MemoryStream(cacheDDS);

        //    }
        //}

    }
}
