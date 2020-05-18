using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Scenegraph
{
    public class ModelSelection : EventArgs
    {
        public Model Model;

        public ModelSelection(Model model)
        {
            this.Model = model;
        }
    }

    public delegate void UnhoveredEvent(ModelSelection e);
    public delegate void HoveredEvent(ModelSelection e);

    public interface IClickable
    {
        event HoveredEvent          OnSelected;
        event UnhoveredEvent        OnUnselected;

        bool ClickEnabled { get; set; }

        Culling.BoundingAABB        Bounds                      { get; set; }
        Vector2[]                   ScreenBounds                { get; set; }

        void                        Hover();
        void                        UnHover();

        void                        Select();
        void                        Unselect();
    }
}
