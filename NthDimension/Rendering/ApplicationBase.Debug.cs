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

namespace NthDimension.Rendering
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Geometry;

    public partial class ApplicationBase
    {
        #region Debugging Helpers

        public virtual void debug_aid_frustum(Vector3 n1, Vector3 n2, Vector3 n3, Vector3 n4, Vector3 f1, Vector3 f2, Vector3 f3, Vector3 f4) { }

        //public void drawLine(Vector3 start, Vector3 end, Color4 color)
        //{
        //    Renderer.Immediate_DrawLine(start, end, color);
        //    Renderer.Flush();
        //}
        #endregion

//#if DEBUG
        public System.Collections.Generic.List<MeshVbo> DrawnMeshes = new System.Collections.Generic.List<MeshVbo>();
        public virtual void DrawMeshDrawTimes()
        {

        }
        public virtual void DrawDrawablesDrawTimes()
        {

        }

        private static int CompareByDrawTime(MeshVbo meshA, MeshVbo meshB)
        {
            return meshA.DrawTimeAllPasses.CompareTo(meshB.DrawTimeAllPasses);
        }
//#endif
    }
}
