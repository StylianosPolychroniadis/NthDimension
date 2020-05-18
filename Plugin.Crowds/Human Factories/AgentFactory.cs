// CrowdSimulator - AgentFactory.cs
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
using CrowdSimulator.Behaviour;

namespace CrowdSimulator.Human_Factories
{
    public class AgentFactory : HumanFactory
    {
        readonly Random rnd = new Random();

        public Human CreateHuman(List<Human> Humans)
        {
            var human = this.CreateHuman();
            int listIndex;

            do
            {
                listIndex = rnd.Next(0, Humans.Count - 1);
            } while (!(Humans[listIndex].HumanType != HumanType.Agent && Humans[listIndex].HumanType != HumanType.Victim));

            human.HumanType = HumanType.Agent;
            human.MovementBehaviour = new AgentMovementBehaviour();
            human.DecisionBehaviour = new AgentDecisionBehaviour();
            human.Victim = Humans[listIndex];
            Humans[listIndex].HumanType = HumanType.Victim;

            return human;
        }
    }
}
