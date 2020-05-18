// CrowdSimulator - EvadeMovementBehaviour.cs
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
using System.Collections.Generic;
using System.Linq;

namespace CrowdSimulator.Behaviour
{
    public class EvadeMovementBehaviour : IMovementBehaviour
    {
        private Vector2 velocity;

        private Vector2 distance;

        private readonly float speed;

        public EvadeMovementBehaviour()
        {
            velocity = new Vector2(0, 0);

            distance = new Vector2(0, 0);

            speed = 3.0f;
        }

        public Vector2 Move(Human LeMe, IEnumerable<Human> NearestNeighbours)
        {
            if ((LeMe.Node - LeMe.Position).LengthFast > 100.0f)
            {
                LeMe.Node = this.GetNewTarget(LeMe);

                if (LeMe.HumanType == HumanType.Agent)
                {
                    LeMe.MovementBehaviour = new AgentMovementBehaviour();
                }
                else
                {
                    LeMe.MovementBehaviour = new UsualMovementBehaviour();
                }
            }

            this.velocity = LeMe.Node - LeMe.Position;
            this.velocity = Vector2.Multiply(velocity, -1.0f);

            this.velocity = Vector2.Multiply(velocity, 1.0f / this.velocity.LengthFast);
            this.velocity = Vector2.Multiply(velocity, this.speed);

            foreach (var human in NearestNeighbours.Where(Human => Human.Position != LeMe.Position && !(Human.HumanType != HumanType.Agent && LeMe.HumanType == HumanType.Victim)))
            {
                this.distance = human.Position - LeMe.Position;

                var num = this.distance.LengthFast;

                distance = Vector2.Multiply(distance, 1.0f / distance.LengthFast);
                distance = Vector2.Multiply(distance, 1.7f);

                if (!(num <= 15.0f))
                {
                    continue;
                }

                num = 15.0f - num;

                num /= 15.0f;

                this.distance = Vector2.Multiply(distance, num * -1f);
                this.distance = Vector2.Multiply(distance, 3.5f);

                this.velocity += this.distance;

                this.distance = human.Position - LeMe.Position;
            }

            this.velocity = Vector2.Multiply(velocity, 1.0f / this.velocity.LengthFast);
            this.velocity = Vector2.Multiply(velocity, this.speed);

            LeMe.Position = LeMe.Position + this.velocity;

            return LeMe.Position;
        }

        private Vector2 GetNewTarget(Human LeMe)
        {
            return LeMe.RequestNewRandomPosition();
        }
    }
}
