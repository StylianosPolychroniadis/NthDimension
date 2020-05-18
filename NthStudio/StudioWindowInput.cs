using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    //internal class StudioWindowInput : NthDimension.Rendering.ApplicationUserInput
    //{

    //    public StudioWindowInput(NthDimension.Rendering.Scenegraph.SceneGame scene)
    //        : base(scene)
    //    {

    //    }

    //    public override void update()
    //    {
    //        base.update();


    //    }
    //}
    public partial class StudioWindow
    {
        private DateTime lastCtrlF1 = DateTime.Now;
        private void CtrlF1()
        {
            OpenTK.Input.KeyboardState key = OpenTK.Input.Keyboard.GetState();
            if (key.IsKeyDown(OpenTK.Input.Key.F1) &&
                (key.IsKeyDown(OpenTK.Input.Key.LControl) ||
                key.IsKeyDown(OpenTK.Input.Key.RControl)))
            {
                if (lastCtrlF1.AddMilliseconds(200) < DateTime.Now)
                {
                    DrawPerformance = !DrawPerformance;
                    lastCtrlF1 = DateTime.Now;
                }
            }
        }
    }
}
