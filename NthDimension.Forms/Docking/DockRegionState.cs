using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockRegionState
    {
        #region Property Region

        public EDockArea Area { get; set; }

        public Size Size { get; set; }

        public List<DockGroupState> Groups { get; set; }

        #endregion

        #region Constructor Region

        public DockRegionState()
        {
            Groups = new List<DockGroupState>();
        }

        public DockRegionState(EDockArea area)
            : this()
        {
            Area = area;
        }

        public DockRegionState(EDockArea area, Size size)
            : this(area)
        {
            Size = size;
        }

        #endregion
    }
}
