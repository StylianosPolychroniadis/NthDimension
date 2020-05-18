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

namespace NthDimension.Rendering.Culling
{
    using NthDimension.Algebra;
    public class Boundary
    {
        public enuCollisionBoundryType type = enuCollisionBoundryType.AxisBox;
        public Vector3 center = new Vector3();
        public Vector3 bounds = new Vector3();
        public Vector3 Orientation = new Vector3();

        // cache for prims
        [System.Xml.Serialization.XmlIgnoreAttribute]
        BoundingAABB box = BoundingAABB.Empty;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        BoundingSphere sphere = BoundingSphere.Empty;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        BoundingCylinderXY cylinder = BoundingCylinderXY.Empty;

        public BoundingAABB Bounds()
        {
            switch (type)
            {
                case enuCollisionBoundryType.Sphere:
                    return BoundingAABB.CreateFromSphere(getSphere());

                case enuCollisionBoundryType.Cylinder:
                    return BoundingAABB.CreateFromCylinderXY(getCylinder());
            }

            return getBox();
        }

        BoundingAABB getBox()
        {
            if (box == BoundingAABB.Empty)
                box = new BoundingAABB(center + -bounds, center + bounds);
            return box;
        }

        BoundingSphere getSphere()
        {
            if (sphere == BoundingSphere.Empty)
                sphere = new BoundingSphere(center, bounds.X);
            return sphere;
        }

        BoundingCylinderXY getCylinder()
        {
            if (cylinder == BoundingCylinderXY.Empty)
                cylinder = new BoundingCylinderXY(center, bounds.X, bounds.Z);

            return cylinder;
        }
    }
}
