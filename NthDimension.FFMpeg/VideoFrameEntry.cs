using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.FFMpeg
{
    public class VideoFrameEntry
    {
        public TimeSpan Position { get; set; }
        public Bitmap Picture { get; set; }

        public VideoFrameEntry() { }
        public VideoFrameEntry(TimeSpan pos, Bitmap pic)
        {
            this.Position = pos;
            this.Picture = pic;
        }
    }
}
