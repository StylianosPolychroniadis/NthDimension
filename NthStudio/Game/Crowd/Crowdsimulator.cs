// CrowdSimulator - Crowd.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using NthStudio.Game.Crowd.Human_Factories;
using NthDimension.Algebra;
using System.Threading.Tasks;

namespace NthStudio.Game.Crowd
{
    public class CrowdSimulator
    {
        private readonly List<Human> humans;

        private readonly Field field;

        //private readonly Bitmap bitmap;

        private static int widthPositive;
        
        private static int lengthPositive;

        private static int widthNegative;

        private static int lengthNegative;

        private int frameCount;

        //private readonly Graphics graphics;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public CrowdSimulator(int minWidth, int maxWidth, int minLength, int maxLength)
        {
            //this.bitmap = Bitmap;
            this.humans = new List<Human>();
            this.field = new Field(maxWidth - minLength, maxLength - minLength);
            lengthPositive = maxLength;
            widthPositive = maxWidth;
            lengthNegative = minLength;
            widthNegative = minWidth;
            //this.graphics = Graphics.FromImage(bitmap);
        }

        public void Init(int Humans, int Assassins)
        {
            humans.Clear();

            var agentFactory = new AgentFactory();
            var humanFactory = new HumanFactory();

            for (int i = 0; i < Humans; i++)
            {
                Human human = humanFactory.CreateHuman();
                humans.Add(human);
            }

            for (int i = 0; i < Assassins; i++)
            {
                Human assasin = agentFactory.CreateHuman(humans);
                humans.Add(assasin);
            }

            field.Update(this.humans);
        }

        public void Update()
        {
            frameCount++;

            Parallel.ForEach(humans, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, h => 
            {
                h.Update(this.GetNearestNeighbour(h));
            }); 

            if (frameCount >= 10)
            {
                frameCount = 0;
                try
                {
                    field.Update(this.humans);
                }
                catch { }                
            }

            //this.Draw();
        }

        public void Draw()
        {
            //graphics.Clear(Color.White);

            foreach (var h in humans)
            {
                #region 2D Colors
                //    if (h.HumanType == HumanType.Normal)
                //    {
                //        graphics.FillEllipse(Brushes.Black, h.Position.X - 2, h.Position.Y - 2, 4, 4);    
                //    }
                //    else if (h.HumanType == HumanType.Agent)
                //    {
                //        graphics.FillEllipse(Brushes.Red, h.Position.X - 2, h.Position.Y - 2, 4, 4);
                //    }
                //    else if (h.HumanType==HumanType.Dead)
                //    {
                //        graphics.FillEllipse(Brushes.GreenYellow, h.Position.X - 2, h.Position.Y - 2, 4, 4);
                //    }
                //    else if (h.HumanType == HumanType.Victim)
                //    {
                //        graphics.FillRectangle(Brushes.DeepSkyBlue, h.Position.X - 2, h.Position.Y - 2, 4, 4);
                //    }
                #endregion

                h.DrawCrowdInfo2D();
            }
        }

        private List<Human> GetNearestNeighbour(Human LeMe)
        {
            return field.GetNearestNeighbour(LeMe.FieldIndex, LeMe.Position);
        }

        public static Vector2 GetRandomPosition()
        {
            float x = Rnd.Next(widthNegative, widthPositive);
            float z = Rnd.Next(lengthNegative, lengthPositive);

            return new Vector2(x, z);
        }
    }
}
