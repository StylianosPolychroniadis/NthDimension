using System.IO;
using System.Drawing;
using NthDimension.Forms;

namespace NthStudio.Utilities
{
    public class ResourceUtil
    {
        public const string RootNamespace = "NthStudio";
        public static Bitmap GetImageResourceByName(ImageList imgList, int index, string imageName)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            //string resourceName = RootNamespace + ".data.icons." + Path.GetFileNameWithoutExtension(imageName);
            string resourceName = RootNamespace + ".data.icons." + imageName;

            Bitmap bmp = null;
            Stream f = asm.GetManifestResourceStream(resourceName);
            using (Stream myStream = f)
            {
                bmp = new Bitmap(myStream);
            }

            imgList.Images.Add(bmp);
            imgList.Images.SetKeyName(index, imageName);

            return bmp;
        }
    }
}
