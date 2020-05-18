using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockPanelState
    {
        #region Property Region

        public List<DockRegionState> Regions { get; set; }

        #endregion

        #region Constructor Region

        public DockPanelState()
        {
            Regions = new List<DockRegionState>();
        }

        #endregion
    }
}
