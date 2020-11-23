using NthDimension.Algebra;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Sound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public partial class StudioWindow
    {
        public class SystemSounds : ApplicationSounds
        {
            public override void playBlip(Vector3 source, Vector3 listener)
            {
                if (!StudioWindow.Instance.AudioSources.ContainsKey("blip"))
                    StudioWindow.Instance.AudioSources.Add("blip", new WavSound(File.Open(Path.Combine(GameSettings.AudioFolder, "blip.wav"), FileMode.Open)));

                var audio = StudioWindow.Instance.AudioSources.FirstOrDefault(a => a.Key == "blip").Value;

                audio.SetPositionSource(source);
                audio.SetPositionListener(listener);

                audio.Play();
            }

            public override void playBlip(float sourceX = 0, float sourceY = 0, float sourceZ = 0, float listenerX = 0, float listenerY = 0, float listenerZ = 0)
            {
                this.playBlip(new Vector3(sourceX, sourceY, sourceZ),
                          new Vector3(listenerX, listenerY, listenerZ));
            }
        }

        public SystemSounds AudioPlayer = new SystemSounds();
       
    }
}
