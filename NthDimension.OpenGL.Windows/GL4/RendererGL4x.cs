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

namespace NthDimension.Rasterizer.Windows
{
    /// <summary>
    /// Not to be used for production yet
    /// </summary>
    public class RendererGL4x : RendererGL3x
    {
        public void BeginQuery(OpenTK.Graphics.OpenGL4.QueryTarget target, int id)
        {
            OpenTK.Graphics.OpenGL4.GL.BeginQuery(target, id);
        }
        public void DeleteQuery(int ids)
        {
            OpenTK.Graphics.OpenGL4.GL.DeleteQuery(ids);
        }
        public void DeleteQueries(int n, int[] ids)
        {
            OpenTK.Graphics.OpenGL4.GL.DeleteQueries(n, ids);
        }
        public int GenQuery()
        {
            return OpenTK.Graphics.OpenGL4.GL.GenQuery();
        }
        public void GenQueries(int n, int[] ids)
        {
            OpenTK.Graphics.OpenGL4.GL.GenQueries(n, ids);
        }
    }
}
