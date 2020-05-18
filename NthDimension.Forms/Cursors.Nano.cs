using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace NthDimension.Forms
{
    public class NanoCursor
    {
        const uint XC_TYPE_IMG = 0xfffd0002;

        class toc
        {
            public uint type;
            public uint subtype;
            public uint pos;

            public toc(BinaryReader sr)
            {
                type = sr.ReadUInt32();
                subtype = sr.ReadUInt32();
                pos = sr.ReadUInt32();
            }
        }

        public List<SingleCursor> Cursors = new List<SingleCursor>();

        static NanoCursor loadFromStream(Stream s)
        {
            var tocList = new List<toc>();
            var tmp = new NanoCursor();

            using (var sr = new BinaryReader(s))
            {
                //byte[] data;
                //magic: CARD32 ’Xcur’ (0x58, 0x63, 0x75, 0x72)
                if (new string(sr.ReadChars(4)) != "Xcur")
                {
                    Debug.WriteLine("NanoCursor Load error: Wrong magic");
                    return null;
                }
                //header: CARD32 bytes in this header
                uint headerLength = sr.ReadUInt32();
                //version: CARD32 file version number
                uint version = sr.ReadUInt32();
                //ntoc: CARD32 number of toc entries
                uint nbToc = sr.ReadUInt32();
                //toc: LISTofTOC table of contents
                for (uint i = 0; i < nbToc; i++)
                {
                    tocList.Add(new toc(sr));
                }

                foreach (toc t in tocList)
                {
                    if (t.type != XC_TYPE_IMG)
                        continue;

                    sr.BaseStream.Seek(t.pos, SeekOrigin.Begin);
                    tmp.Cursors.Add(imageLoad(sr));
                }
            }
            return tmp;
        }

        public static NanoCursor Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: ", path);
            var strm = new FileStream(path, FileMode.Open, FileAccess.Read);

            return Load(strm);
        }

        public static NanoCursor Load(Stream strm)
        {
            return loadFromStream(strm);
        }

        static SingleCursor imageLoad(BinaryReader sr)
        {
            var tmp = new SingleCursor();
            //			header: 36 Image headers are 36 bytes
            uint header = sr.ReadUInt32();
            //			type: 0xfffd0002 Image type is 0xfffd0002
            uint type = sr.ReadUInt32();
            //			subtype: CARD32 Image subtype is the nominal size
            uint subtype = sr.ReadUInt32();
            //			version: 1
            uint version = sr.ReadUInt32();
            //			width: CARD32 Must be less than or equal to 0x7fff
            tmp.Width = sr.ReadUInt32();
            //			height: CARD32 Must be less than or equal to 0x7fff
            tmp.Height = sr.ReadUInt32();
            //			xhot: CARD32 Must be less than or equal to width
            tmp.Xhot = sr.ReadUInt32();
            //			yhot: CARD32 Must be less than or equal to height
            tmp.Yhot = sr.ReadUInt32();
            //			delay: CARD32 Delay between animation frames in milliseconds
            tmp.Delay = sr.ReadUInt32();
            //			pixels: LISTofCARD32 Packed ARGB format pixels
            tmp.data = sr.ReadBytes((int)(tmp.Width * tmp.Height * 4));

            return tmp;
        }
    }
}
