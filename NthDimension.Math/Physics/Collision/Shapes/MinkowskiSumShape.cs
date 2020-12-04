﻿/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/


#region Using Statements
using System;
using System.Collections.Generic;

using NthDimension.Physics.Dynamics;
using NthDimension.Physics.Collision.Shapes;
using NthDimension.Physics.LinearMath;

#endregion

namespace NthDimension.Physics.Collision.Shapes
{
    public class MinkowskiSumShape : Shape
    {
        JVector shifted;
        List<Shape> shapes = new List<Shape>();

        public MinkowskiSumShape(IEnumerable<Shape> shapes)
        {
            AddShapes(shapes);
        }

        public void AddShapes(IEnumerable<Shape> shapes)
        {
            foreach (Shape shape in shapes)
            {
                if (shape is Multishape) throw new Exception("Multishapes not supported by MinkowskiSumShape.");
                this.shapes.Add(shape);
            }

            this.UpdateShape();
        }

        public void AddShape(Shape shape)
        {
            if (shape is Multishape) throw new Exception("Multishapes not supported by MinkowskiSumShape.");
            shapes.Add(shape);

            this.UpdateShape();
        }

        public bool Remove(Shape shape)
        {
            if (shapes.Count == 1) throw new Exception("There must be at least one shape.");
            bool result = shapes.Remove(shape);
            UpdateShape();
            return result;
        }

        public JVector Shift()
        {
            return -1 * this.shifted;
        }

        public override void CalculateMassInertia()
        {
            this.mass = Shape.CalculateMassInertia(this, out shifted, out inertia);
        }

        public override void SupportMapping(ref JVector direction, out JVector result)
        {
            if (float.IsNaN(direction.X) ||
                float.IsNaN(direction.Y) ||
                float.IsNaN(direction.Z))
            {
                result = JVector.Zero;
            }
            JVector temp1, temp2 = JVector.Zero;

            for (int i = 0; i < shapes.Count; i++)
            {
                shapes[i].SupportMapping(ref direction, out temp1);
                JVector.Add(ref temp1, ref temp2, out temp2);
            }

            JVector.Subtract(ref temp2, ref shifted, out result);
        }

    }
}
