// CrowdSimulator - Field.cs
// 
// Copyright (c) 2012, Dominik Gander
// Copyright (c) 2012, Pascal Minder
// 
//  Permission to use, copy, modify, and distribute this software for any
//  purpose without fee is hereby granted, provided that the above
//  copyright notice and this permission notice appear in all copies.
// 
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NthStudio.Game.Crowd
{
    public class Field
    {
        private const int XFields = 6;
        
        private const int YFields = 6;

        private List<Human>[,] humans;

        private readonly int xTile;

        private readonly int yTile;

        public Field(int Width, int Height)
        {
            this.xTile = Width / XFields;
            this.yTile = Height / YFields;

            this.CreateArray();
        }

        public void Update(IEnumerable<Human> Humans)
        {
            this.CreateArray();

            //Parallel.ForEach(Humans, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, h =>
                foreach (var h in Humans)
                {
                    try
                    {
                        var x = Math.Min(2, Math.Max(0, (int)Math.Floor(h.Position.X / this.xTile)));
                        var y = Math.Min(2, Math.Max(0, (int)Math.Floor(h.Position.Y / this.yTile)));

                        this.humans[x, y].Add(h);

                        h.FieldIndex = new Vector2(x, y);
                    }
                    catch { }
                }
            //);
        }

        public List<Human> GetNearestNeighbour(Vector2 FieldIndex, Vector2 Position)
        {
            int[,] leftUp = {{0, -1}, {-1, -1}, {-1, 0}};
            int[,] leftDown = {{-1, 0}, {-1, 1}, {0, 1}};
            int[,] rightUp = {{0, -1}, {1, -1}, {1, 0}};
            int[,] rightDown = {{1, 0}, {1, 1}, {0, 1}};

            var leftRight = 0;
            var upDown = 0;

            var neighbour = new List<Human>();

            if((Position.X % this.xTile) > (this.xTile / 2))
            {
                leftRight = 1;
            }

            if ((Position.Y % this.yTile) > (this.yTile / 2))
            {
                upDown = 1;
            }

            switch (leftRight)
            {
                case 0:
                {
                    switch (upDown)
                    {
                        case 0:
                        {
                            neighbour.AddRange(this.GetFields(FieldIndex, leftUp));
                        } break;

                        case 1:
                        {
                            neighbour.AddRange(this.GetFields(FieldIndex, leftDown));
                        } break;
                    }
                }break;

                case 1:
                {
                    switch (upDown)
                    {
                        case 0:
                        {
                            neighbour.AddRange(this.GetFields(FieldIndex, rightUp));
                        } break;

                        case 1:
                        {
                            neighbour.AddRange(this.GetFields(FieldIndex, rightDown));
                        } break;
                    }
                }break;
            }

            neighbour.AddRange(this.humans[(int)FieldIndex.X, (int)FieldIndex.Y]);

            return neighbour;
        }

        private IEnumerable<Human> GetFields(Vector2 Field, int[,] Offsets)
        {
            var neightbour = new List<Human>();

            for (int i = 0; i < (Offsets.GetLength(0) - 1); i++)
            {
                int x = (int)Field.X + Offsets[i, 0];
                int y = (int)Field.Y + Offsets[i, 1];

                if (!(x >= 0 && x < XFields))
                {
                    continue;
                }

                if (!(y >= 0 && y < YFields))
                {
                    continue;
                }

                neightbour.AddRange(this.humans[x, y]);
            }

            return neightbour;
        }

        private void CreateArray()
        {
            this.humans = new List<Human>[XFields, YFields];

            for (var x = 0; x < XFields; x++)
            {
                for (var y = 0; y < YFields; y++)
                {
                    this.humans[x, y] = new List<Human>();
                }
            }
        }
    }
}
