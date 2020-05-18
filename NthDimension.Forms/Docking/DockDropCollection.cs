using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockDropCollection
    {
        #region Properties
        internal DockDropArea DropArea { get; private set; }

        internal DockDropArea InsertBeforeArea { get; private set; }

        internal DockDropArea InsertAfterArea { get; private set; }
        #endregion Properties

        internal DockDropCollection(DockPanel dockPanel, DockGroup group)
        {
            DropArea = new DockDropArea(dockPanel, group, EDockInsertType.None);
            InsertBeforeArea = new DockDropArea(dockPanel, group, EDockInsertType.Before);
            InsertAfterArea = new DockDropArea(dockPanel, group, EDockInsertType.After);
        }
    }
}
