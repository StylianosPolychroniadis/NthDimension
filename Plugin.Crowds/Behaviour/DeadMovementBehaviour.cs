// CrowdSimulator - DeadMovementBehaviour.cs
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

namespace CrowdSimulator.Behaviour
{
    public class DeadMovementBehaviour : IMovementBehaviour
    {
        public Vector2 Move(Human LeMe, IEnumerable<Human> NearestNeighbours)
        {
            return LeMe.Position;
        }
    }
}
