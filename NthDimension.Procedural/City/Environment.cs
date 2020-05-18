using NthDimension.Algebra;
using NthDimension.Rendering;
using NthDimension.Rendering.Drawables;
using NthDimension.Utilities;
using System;
using System.Collections.Generic;


namespace NthDimension.Procedural.City
{
    // TODO:: Transfer to CityDrawable if possible

    public class Environment : Drawable // TODO:: Model???
    {
        // gridSize         = 129
        // gridPrecision    = 7
        public Environment(int gridPrecision, float waterLevel = -2.0f)
        {
            this.m_gridPrecision = gridPrecision;
        }


        // values from C++ runtime debugger
        private int             m_gridPrecision             = 7;
        private int             m_gridSize                  = 129; // 129x129
        private float           m_waterLevel                = -5f;
        private float           m_minHeight                 = -6.34105968f;
        private float           m_maxHeight                 = 12.3116955f;
        //string                  m_heightMap;            // CAUTION:: this was Image<float>
        private DirectBitmap    m_heightsMap;
        private Vector2         pos;

        private List<Vector2>   landscapeVertices;      // = new List<Vector2>();
        private List<int>       landscapeIndices;
        private List<Vector3>   landscapeNormals;
        private List<Vector2>   waterVertices;

        private Material        WaterMaterial;
        private Material        LanscapeMaterial;

        private const int       GRID_PRECISION = 7;

        /*
          private:
	        int m_gridPrecision,
		        m_gridSize;
	        float	m_waterLevel,
			        m_minHeight,
			        m_maxHeight;
	        Image<float> m_heightsMap;

            D3DXVECTOR2 pos;

            VertexBuffer<D3DXVECTOR2>::Type*    m_vbLandscape;
            VertexBuffer<D3DXVECTOR4>::Type*    m_normals;
            IndexBuffer<INDEX_TYPE>::Type*      m_ibLandscape;
            Declaration*                        m_declLandscape;
            VertexBuffer<D3DXVECTOR2>::Type*    m_vbWater;
            Declaration*                        m_declWater;
            Shader*                             m_shaderWater;
            Shader*                             m_shaderLandscape;
         */

        #region TODO Convert to Properties
        private int getGridSize()
        {
            return m_gridSize;
        }
        private int getGridPrecision()
        {
            return m_gridPrecision;
        }
        private DirectBitmap getHeightMap()
        {
            return this.m_heightsMap;
        }
        private float getMinHeight()
        {
            return this.m_minHeight;
        }
        private float getMaxHeight()
        {
            return this.m_maxHeight;
        }
        private void setMinHeight(float minHeight)
        {
            this.m_minHeight = minHeight;
        }
        private void setMaxHeight(float maxHeight)
        {
            this.m_maxHeight = maxHeight;
        }
        private float getWaterLevel()
        {
            return m_waterLevel;
        }
        private void setWaterLevel(float waterLevel)
        {
            m_waterLevel = waterLevel;
        }
        #endregion

        private float computeDX(int x1, int x3, int y)
        {
            //return (m_heightsMap(x1, y) - m_heightsMap(x3, y)) / 2.0f;
            System.Drawing.Color res = new System.Drawing.Color();

            //return (m_heightsMap.GetPixel(x1, y) - m_heightsMap.GetPixel(x3, y)) / 2.0f;
            throw new NotImplementedException();
        }
        private float computeDY(int y1, int y3, int x)
        {
            //return (m_heightsMap(x, y1) - m_heightsMap(x, y3)) / 2.0f;
            // get m_heightMap pixel x1, y and x3, y
            throw new NotImplementedException();
        }
    }
}
