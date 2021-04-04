using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets.CalcGrid
{
    public struct CellRange
    {
        public int r1, c1, r2, c2;
        public CellRange(int row, int col)
            : this(row, col, row, col)
        {
        }
        public CellRange(int row1, int col1, int row2, int col2)
        {
            r1 = row1;
            c1 = col1;
            r2 = row2;
            c2 = col2;
        }

#if _WINFORMS_
        // As used in BluePanel
        public CellRange(System.Windows.Forms.DataGridViewSelectedCellCollection sel)
        {
            // assume invalid range
            r1 = r2 = c1 = c2 = -1;

            // build CellRange using the first and last cells in the 
            // DataGridViewSelectedCellCollection
            if (sel.Count > 0)
            {
                var cell1 = sel[0];
                var cell2 = sel[sel.Count - 1];
                r1 = cell1.RowIndex;
                c1 = cell1.ColumnIndex;
                r2 = cell2.RowIndex;
                c2 = cell2.ColumnIndex;
            }
        }
#endif
        public int TopRow { get { return System.Math.Min(r1, r2); } }
        public int BottomRow { get { return System.Math.Max(r1, r2); } }
        public int LeftCol { get { return System.Math.Min(c1, c2); } }
        public int RightCol { get { return System.Math.Max(c1, c2); } }
        public bool IsValid { get { return r1 > -1 && c1 > -1 && r2 > -1 && c2 > -1; } }
        public bool IsSingleCell { get { return r1 == r2 && c1 == c2; } }
    }
}
