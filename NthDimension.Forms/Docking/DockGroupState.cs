using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockGroupState
    {
        #region Property Region

        public List<string> Contents { get; set; }

        #endregion

        #region Constructor Region

        public DockGroupState()
        {
            Contents = new List<string>();
        }

        #endregion
    }
}
