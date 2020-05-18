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
using CrowdSimulator.Human_Factories;
using NthDimension.Algebra;

namespace CrowdSimulator
{
    public class Crowd
    {
        private readonly List<Human> humans;

        private readonly Field field;

        //private readonly Bitmap bitmap;

        private static int width;
        
        private static int height;

        private int frameCount;

        //private readonly Graphics graphics;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public Crowd(int worldWidth, int worldLength)
        {
            //this.bitmap = Bitmap;
            this.humans = new List<Human>();
            this.field = new Field(worldWidth, worldLength);
            height = worldLength;
            width = worldWidth;
            //this.graphics = Graphics.FromImage(bitmap);
        }

        public void Init(int Humans, int Assassins)
        {
            var agentFactory = new AgentFactory();
            var humanFactory = new HumanFactory();

            for (int i = 0; i < Humans; i++)
            {
                humans.Add(humanFactory.CreateHuman());
            }

            for (int i = 0; i < Assassins; i++)
            {
                humans.Add(agentFactory.CreateHuman(humans));
            }

            field.Update(this.humans);
        }

        public void Update()
        {
            frameCount++;

            foreach (var h in humans)
            {
                h.Update(this.GetNearestNeighbour(h));
            }

            if (frameCount >= 10)
            {
                field.Update(this.humans);
                frameCount = 0;
            }

            this.Draw();
        }

        private void Draw()
        {
            //graphics.Clear(Color.White);

            //foreach (var h in humans)
            //{
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

            //}
        }

        private List<Human> GetNearestNeighbour(Human LeMe)
        {
            return field.GetNearestNeighbour(LeMe.FieldIndex, LeMe.Position);
        }

        public static Vector2 GetRandomPosition()
        {
            return new Vector2(Rnd.Next(0, width), Rnd.Next(0, height));
        }
    }
}
