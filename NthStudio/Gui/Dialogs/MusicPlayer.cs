using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthStudio.Gui.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Dialogs
{
    public class MusicPlayer : DialogBase
    {
        TextInput   filePath;
        Button      fileBrowse = new Button("Browse");
        public MusicPlayer()
        {
            this.Size = new System.Drawing.Size(350, 200);

            fileBrowse.Size = new System.Drawing.Size(60, 25);
            
            filePath = new TextInput();
            filePath.Size = new System.Drawing.Size(this.Width - fileBrowse.Width, 25);
            filePath.Location = new System.Drawing.Point(0, 0);
            filePath.SetDefaultText("Select an mp3 file");

            fileBrowse.Location = new System.Drawing.Point(filePath.Width, 0);
            fileBrowse.MouseClickEvent += delegate
            {
                // TODO Class Mp3Sound
            };

            Panel top = new Panel();
            top.Size = new System.Drawing.Size(this.Width, 100);
            top.Dock = EDocking.Fill;
            top.Widgets.Add(filePath);
            top.Widgets.Add(fileBrowse);

            this.Widgets.Add(top);


        }

        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
