using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets.CalcGrid
{
//    public class CalcGridEngine : NthDimension.CalcEngine.CalculationEngine
//    {
//        public CellRange GetRange(string cell)
//        {
//            int index = 0;

//            // parse column
//            int col = -1;
//            bool absCol = false;
//            for (; index < cell.Length; index++)
//            {
//                var c = cell[index];
//                if (c == '$' && !absCol)
//                {
//                    absCol = true;
//                    continue;
//                }
//                if (!char.IsLetter(c))
//                {
//                    break;
//                }
//                if (col < 0) col = 0;
//                col = 26 * col + (char.ToUpper(c) - 'A' + 1);
//            }

//            // parse row
//            int row = -1;
//            bool absRow = false;
//            for (; index < cell.Length; index++)
//            {
//                var c = cell[index];
//                if (c == '$' && !absRow)
//                {
//                    absRow = true;
//                    continue;
//                }
//                if (!char.IsDigit(c))
//                {
//                    break;
//                }
//                if (row < 0) row = 0;
//                row = 10 * row + (c - '0');
//            }

//            // sanity
//            if (index < cell.Length)
//            {
//                throw new Exception("Invalid cell reference.");
//            }

//            // done
//            return new CellRange(row - 1, col - 1);
//        }
//        public CellRange MergeRanges(CellRange rng1, CellRange rng2)
//        {
//            return new CellRange(
//                Math.Min(rng1.TopRow, rng2.TopRow),
//                Math.Min(rng1.LeftCol, rng2.LeftCol),
//                Math.Max(rng1.BottomRow, rng2.BottomRow),
//                Math.Max(rng1.RightCol, rng2.RightCol));
//        }

//#if _WINFORMS_
//        // As used in BluePanel.Net
//        class CellRangeReference : BluePanel.Net.CalcEngine.Expressions.IValueObject, IEnumerable
//        {
//            // ** fields
//            EsiSpreadsheet _grid;
//            CellRange _rng;
//            bool _evaluating;

//            // ** ctor
//            public CellRangeReference(EsiSpreadsheet grid, CellRange rng)
//            {
//                _grid = grid;
//                _rng = rng;
//            }

//            // ** IValueObject
//            public object GetValue()
//            {
//                return GetValue(_rng);
//            }

//            // ** IEnumerable
//            public IEnumerator GetEnumerator()
//            {
//                for (int r = _rng.TopRow; r <= _rng.BottomRow; r++)
//                {
//                    for (int c = _rng.LeftCol; c <= _rng.RightCol; c++)
//                    {
//                        var rng = new CellRange(r, c);
//                        yield return GetValue(rng);
//                    }
//                }
//            }

//            // ** implementation
//            object GetValue(CellRange rng)
//            {
//                if (_evaluating)
//                {
//                    throw new Exception("Circular Reference");
//                }
//                try
//                {
//                    _evaluating = true;

//                    return _grid.Evaluate(rng.r1, rng.c1);
//                }
//                finally
//                {
//                    _evaluating = false;
//                }
//            }
//        }
//#endif


//    }
}
