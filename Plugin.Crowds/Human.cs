// CrowdSimulator - Human.cs
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

using System.Collections.Generic;
using System.Linq;
using CrowdSimulator.Behaviour;
using NthDimension.Algebra;

namespace CrowdSimulator
{
    public enum HumanType
    { 
        Normal,
        Agent,
        Victim,
        Dead
    }

    public class Human
    {

        public Human(HumanType HumanType, Vector2 Position, Vector2 Node)
        {
            this.HumanType = HumanType;
            this.Position = Position;
            this.Node = Node;
            this.MovementBehaviour = new UsualMovementBehaviour();
            this.DecisionBehaviour = new UsualDecisionBehaviour();
        }

        public Vector2 Position { get; set; }

        public Vector2 Node { get; set; }

        public Vector2 FieldIndex { get; set; }

        public Vector2 Incident { get; set; }

        public Human Victim { get; set; }

        public HumanType HumanType { get; set; }

        public IMovementBehaviour MovementBehaviour { get; set; }

        public IDecisionBehaviour DecisionBehaviour { get; set; }

        public void Update(List<Human> NearestNeighbour)
        {
            var humanInSight = NearestNeighbour.Where(OnPredicate).ToList();

            this.DecisionBehaviour.CheckSurrounding(this, humanInSight);

            this.Position = this.MovementBehaviour.Move(this, NearestNeighbour);
        }

        private bool OnPredicate(Human Human)
        {
            return Vector2.Dot((this.Node - this.Position), (Human.Position - this.Position)) >= 0.02f && (Human.Position != this.Position);
        }

        public Vector2 RequestNewRandomPosition()
        {
            return Crowd.GetRandomPosition();
        }

        public void Kill()
        {
            this.HumanType = HumanType.Dead;
            this.MovementBehaviour=new DeadMovementBehaviour();
        }
    }
}
