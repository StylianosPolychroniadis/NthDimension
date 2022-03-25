﻿using System;
using System.Collections.Generic;
using NthDimension.Algebra;
using NthDimension.Rasterizer;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Utilities;
using NthDimension.Utilities;

namespace NthDimension.Rendering.Drawables.Models
{
    public partial class Terrain
    {
		public enum ECenterType
		{
			CENTER_BASIC,
			CENTER_RASTER,
			CENTER_RADIAL
		}

		public class Center
		{
			public Vector2 pos = new Vector2();
			public float innerRadius;
			public float outerRadius;
			public ECenterType type;
		}

		public class Density : System.IDisposable
		{
			public const int NB_POINTS_CENTERS = 32;
			public const int DENSITY_MAP_SIZE = 128;
			public int Centers { get; set; }

			private Terrain					m_terrain;


			private int m_gridSize;
			private List<Center>			m_centers = new List<Center>();
			private System.Drawing.Bitmap	m_densityMap;
			private Texture					m_densityTexture;
			private float[,]				m_density;



			public float m_innerRadius				= 130f;			// Terrain.Size Max
			public float m_innerRadiusVariance		= 0.20f;            // [ 0.0 ... 1.0  ]
			public float m_innerRadiusDensity		= 0.8f;             // [ 0.0 ... 1.0  ]
			public float m_outerRadiusVariance		= 0.14f;			// [ 0.0 ... 1.0  ]
			public float m_outerRadiusProportion	= 4f;              // [ 0.0 ... 10.0 ]
			public float m_powAttenuation			= 1.2f;             // [ 0.0 ... 2.0  ]
			public float m_elevationRange			= 10f;				// 2 * Terrain.Size Max
			
			

			public int TextureId
			{
				get { return m_densityTexture.identifier; }
			}
				



#if DEBUG_DRAW_CENTERS
		private VertexBuffer<_D3DVECTOR>.Type m_vbCenters = ((object)0);
		private IndexBuffer<ushort>.Type m_ibCenters = ((object)0);
#endif

			public Density(Terrain terrain, /*int gridSize = 7,*/ int centers = 4)
			{
				if (NthDimension.Settings.Instance.game.diagnostics)
					ConsoleUtil.log("\tGenerating Terrain Density...");
				//this.m_gridSize = /*(int)terrain.Width; //*/ 1 << gridSize;                // 1 << 7 = 128
				this.m_terrain = terrain;
				this.Centers = centers;

				this.m_densityMap = new System.Drawing.Bitmap(DENSITY_MAP_SIZE, 
														      DENSITY_MAP_SIZE, 
															  System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				reset();
				if (NthDimension.Settings.Instance.game.diagnostics)
					ConsoleUtil.log($"\t\t-Creating {Centers}x Centers");
				generateCenters();
				if (NthDimension.Settings.Instance.game.diagnostics) 
					ConsoleUtil.log($"\t\t-Creating Density {DENSITY_MAP_SIZE}x{DENSITY_MAP_SIZE}");
				computeDensity();
				if (NthDimension.Settings.Instance.game.diagnostics)
					ConsoleUtil.log("\t\t-Creating Texture");
				
				this.m_densityTexture				= new Texture();
				this.m_densityTexture.type			= Texture.Type.fromBitmap;
				this.m_densityTexture.name			= "terrain_densityMap";
				this.m_densityTexture.identifier	= ApplicationBase.Instance.TextureLoader.textures.Count;
				
				ApplicationBase.Instance.TextureLoader.registerTexture(this.m_densityTexture);
				ApplicationBase.Instance.TextureLoader.loadTexture(this.m_densityTexture, this.m_densityMap, true);

				//this.m_densityMap.Save(@"D:\density.png", System.Drawing.Imaging.ImageFormat.Png);
				if (NthDimension.Settings.Instance.game.diagnostics)
					ConsoleUtil.log("\tTerrain Density Generated!");
			}

			public void Dispose()
			{
				//			if (m_texture != null)		m_texture.Dispose();
				//#if DEBUG_DRAW_CENTERS
				//			if (m_vbCenters != null)	m_vbCenters.Dispose();			
				//			if (m_ibCenters != null)	m_ibCenters.Dispose();
				//#endif
				//			if (m_declaration != null)	m_declaration.Dispose();
			}

			public void update()
			{
				//// on synchronise la texture
				//m_texture.update();
			}

			public void updateCenters()
			{
#if DEBUG_DRAW_CENTERS
			if (m_vbCenters == ((object)0))			
				return;
			
			Vector3[] vertices = m_vbCenters.@lock();
			ushort[] indices = m_ibCenters.@lock();
#else
				if (this.m_centers.Count == 0) return;
#endif

				float angle = ((float)3.141592654f) * 2.0f / (float)NB_POINTS_CENTERS;

				// pour chaque centre
				ushort nb = 0;
				foreach (Center it in m_centers)
				{
					// on calcule donc les positions des points du vertex buffer
					for (int i = 0; i < NB_POINTS_CENTERS; i++)
					{
						float x = it.pos.X + it.innerRadius * (float)System.Math.Cos((float)i * angle);
						float z = it.pos.Y + it.innerRadius * (float)System.Math.Sin((float)i * angle);
#if DEBUG_DRAW_CENTERS
					vertices[i + NB_POINTS_CENTERS * nb].X = x;
					vertices[i + NB_POINTS_CENTERS * nb].Z = z;
#endif
						if (x <= 0.0f) x = 0.0f;
						if (x > m_terrain.Width) x = (float)m_terrain.Width - 1.0f;
						if (z <= 0.0f) z = 0.0f;
						if (z > m_terrain.Length) z = (float)m_terrain.Length - 1.0f;

#if DEBUG_DRAW_CENTERS
					vertices[i + NB_POINTS_CENTERS * nb].y = m_env.getHeightMap.functorMethod().interpolate(x, z) + 0.1f;
#endif
					}

#if DEBUG_DRAW_CENTERS
				// puis les indices de l'index buffer
				for (ushort i = 0; i < NB_POINTS_CENTERS + 4; i++)
				{
					indices[i + (NB_POINTS_CENTERS + 4) * nb] = (i % NB_POINTS_CENTERS) + nb * NB_POINTS_CENTERS;
					if (i >= NB_POINTS_CENTERS)
					{
						i++;
						indices[i + (NB_POINTS_CENTERS + 4) * nb] = NB_POINTS_CENTERS / 2 + nb * NB_POINTS_CENTERS;
						i++;
						indices[i + (NB_POINTS_CENTERS + 4) * nb] = 3 * NB_POINTS_CENTERS / 4 + nb * NB_POINTS_CENTERS;
						i++;
						indices[i + (NB_POINTS_CENTERS + 4) * nb] = NB_POINTS_CENTERS / 4 + nb * NB_POINTS_CENTERS;
					}
				}
#endif
					nb++;
				}

#if DEBUG_DRAW_CENTERS
			m_vbCenters.unlock();
			m_ibCenters.unlock();
#endif

			}

			#region render()
			//////C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
			//////ORIGINAL LINE: void render(Renderer &renderer) const
			////public void render(Renderer renderer)
			////{
			////	//renderer.setShader(m_shader);
			////	//m_shader.setValue("World", renderer.getMatrix());

			////	//m_shader.setValue("Size", getGridSize() - 1.0f);
			////	//renderer.setTexture("density", m_texture);
			////	//_D3DVECTOR color = new _D3DVECTOR(1.0f, 0.1f, 0.2f);
			////	//m_shader.setValue("Color", color);
			////	//m_env.renderLandscape(renderer);

			////	//if (m_vbCenters != null)
			////	//{
			////	//	renderer.setShader(m_shaderCenter);
			////	//	m_shaderCenter.setValue("World", renderer.getMatrix());
			////	//	renderer.setDeclaration(m_declaration);
			////	//	renderer.setVertexBuffer<struct _D3DVECTOR>(0, m_vbCenters, 0);
			////	//	for (uint i = 0; i < m_centers.Count; i++)
			////	//	{
			////	//		renderer.drawIndexedPrimitives<ushort>(_D3DPRIMITIVETYPE.D3DPT_LINESTRIP, m_ibCenters, i * (NB_POINTS_CENTERS + 4), NB_POINTS_CENTERS);
			////	//		renderer.drawIndexedPrimitives<ushort>(_D3DPRIMITIVETYPE.D3DPT_LINELIST, m_ibCenters, i * (NB_POINTS_CENTERS + 4) + NB_POINTS_CENTERS, 1);
			////	//		renderer.drawIndexedPrimitives<ushort>(_D3DPRIMITIVETYPE.D3DPT_LINELIST, m_ibCenters, i * (NB_POINTS_CENTERS + 4) + NB_POINTS_CENTERS + 2, 1);
			////	//	}
			////	//}
			////}
			#endregion

			//public void reset()
			//{
			//	// on efface l'affichage des centres
			//	displayCenters(false);

			//	for (int i = 0; i < m_gridSize; i++)
			//	{
			//		for (int j = 0; j < m_gridSize; j++)
			//		{
			//			m_densityMap.functorMethod(i, j) = 0.0f;
			//		}
			//	}

			//	m_centers.Clear();

			//	for (int i = 0; i < m_gridSize; i++)
			//	{
			//		for (int j = 0; j < m_gridSize; j++)
			//		{
			//			m_textureMap.functorMethod(i, j) = 0;
			//		}
			//	}

			//	update();
			//}

			/* displayCenters
			Affiche ou non les centres, selon l'argument pass� */
			public void displayCenters(bool disp)
			{

				//if (disp) // on demande d'afficher les centres
				//{
				//	// si on n'a pas de centres, on ne fait rien
				//	if (m_centers.Count == 0)
				//	{
				//		return;
				//	}
				//	m_vbCenters = m_media.getRenderer().createVertexBuffer<struct _D3DVECTOR>(m_centers.Count * NB_POINTS_CENTERS);
				//	m_ibCenters = m_media.getRenderer().createIndexBuffer<ushort>(m_centers.Count * (NB_POINTS_CENTERS + 4));

				//	updateCenters();

				//}
				//else // on doit effacer les centres
				//{
				//	if (m_vbCenters != ((object)0))
				//	{
				//		m_vbCenters = null;
				//		m_vbCenters = ((object)0);
				//	}
				//	if (m_ibCenters != ((object)0))
				//	{
				//		m_ibCenters = null;
				//		m_ibCenters = ((object)0);
				//	}
				//}
			}

			///* getters et setters */
			//public int getGridSize()
			//{
			//	return m_gridSize;
			//}

			//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
			//ORIGINAL LINE: const Image<float> & getDensityMap() const
			//public Image<float> getDensityMap()
			//{
			//	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
			//	//ORIGINAL LINE: return m_densityMap;
			//	return new Image<float>(m_densityMap.functorMethod);
			//}

			//public Image<ushort> getTextureMap()
			//{
			//	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
			//	//ORIGINAL LINE: return m_textureMap;
			//	return new Image<ushort>(m_textureMap.functorMethod);
			//}

			//public Image<float> getDensityMap()
			//{
			//	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
			//	//ORIGINAL LINE: return m_densityMap;
			//	return new Image<float>(m_densityMap.functorMethod);
			//}

			public void addCenter(Center toAdd)
			{
				m_centers.Add(toAdd);
			}

			public void addCenter(Vector2 pos, float innerRadius, float outerRadius, ECenterType type)
			{
				Center toAdd = new Center();

				toAdd.pos = pos;
				toAdd.innerRadius = innerRadius;
				toAdd.outerRadius = outerRadius;
				toAdd.type = type;

				addCenter(toAdd);
			}

			public List<Center> getCenters()
			{
				return new List<Center>(m_centers);
			}

			private void computeDensity()
			{
				
				if (null == m_density)
					m_density = new float[m_densityMap.Width, m_densityMap.Height];

				for (int x = 0; x < m_densityMap.Width; x++)
					for (int y = 0; y < m_densityMap.Height; y++)
					{
						//this.m_densityMap.SetPixel(x, y, System.Drawing.Color.FromArgb( 0,0,0,0));
						float d = computeZoneDensity(x, y);
						m_density[x, y] = d;
						
						Color4 col4 = new Color4(1-(d*65535), d * 65535, 1-(d*65535), 1f);
						
						
						this.m_densityMap.SetPixel(x, y, System.Drawing.Color.FromArgb(col4.ToArgb()));
						//ConsoleUtil.log($"Density [{x}(/{m_densityMap.Width}), {y}(/{m_densityMap.Height})] {d} Color {col4}");
					}
				
			}
			private float computeZoneDensity(int x, int y, int DENSITY_RADIUS = 10, float VARIANCE_AMPLIFICATION = 10.0f, float CENTER_IMPORTANCE = 0.7f)
			{
				float mean = 0.0f;
				float meanSqr = 0.0f;
				int nbPoints = 0;

				for (int i = -DENSITY_RADIUS + x; i < DENSITY_RADIUS + x; i++)
				{
					if (i < 0 || i >= m_densityMap.Width) continue;
					for (int j = -DENSITY_RADIUS + y; j < DENSITY_RADIUS + y; j++)
					{
						if (j < 0 || j >= DENSITY_MAP_SIZE) continue;
						Vector2 distFromPosition = new Vector2(x - i, y - j); // ? WARNING:: 

						if (DENSITY_RADIUS * DENSITY_RADIUS >= distFromPosition.LengthSquared)
						{
							nbPoints++;
							float ijHeight = m_terrain.GetHeightAt(i, j);
							mean += ijHeight;
							meanSqr += ijHeight * ijHeight;
						}
					}
				}

				mean /= nbPoints;
				meanSqr /= nbPoints;
				float variance = (meanSqr - (mean * mean)) / m_elevationRange;
				float weight = 0.0f;

				foreach (var center in m_centers)
				{
					Vector2 vDist = center.pos - new Vector2(x, y);
					float dist = vDist.Length;

					if(dist < center.innerRadius)
					{
						float d = dist / center.innerRadius;
						weight = 1.0f - (float)System.Math.Pow(d, m_powAttenuation) * (1.0f - m_innerRadiusDensity);
						break;
					}
					else if(dist > center.outerRadius)
					{
						continue;
					}
					else
					{
						float d = 1.0f - ((dist - center.innerRadius) / (center.outerRadius - center.innerRadius));
						weight = (float)System.Math.Max(weight, System.Math.Pow(d, m_powAttenuation) * m_innerRadiusDensity);
					}
				}

				float density = weight - variance;
				MathFunc.Clamp(density, 0.0f, 1.0f);
				return density;
			}
			private void generateCenters()
			{
				for (int i = 0; i < Centers; i++)
				{
					this.genCenter();
				}
			}
			private void genCenter()
			{
				float innerRadius = (float)MathFunc.RandomNumber(this.m_innerRadius, this.m_innerRadiusVariance);
				float outerRadius = innerRadius * (float)MathFunc.RandomNumber(this.m_outerRadiusProportion, this.m_outerRadiusVariance);
				Vector2 center = new Vector2((float)MathFunc.RandomNumber(0f, this.m_terrain.Width),
											 (float)MathFunc.RandomNumber(0f, this.m_terrain.Length));
				this.addCenter(center, innerRadius, outerRadius, ECenterType.CENTER_BASIC);
			}
			private float getLocalDensity(Vector2 pos)
			{
				float localDensity = 0.0f;
				Vector2 posCenter = new Vector2();

				// on regarde la distance du point actuel avec tous les centres
				foreach (Center it in m_centers)
				{
					posCenter = pos - it.pos;
					float distSq = posCenter.LengthSquared;
					if (distSq < it.innerRadius * it.innerRadius)
					{
						// on a une densit� max sur le point, pas la peine de continuer
						return 1.0f;
					}
					else if (distSq < it.outerRadius * it.outerRadius)
					{
						float tmp = 1 - (distSq - it.innerRadius * it.innerRadius) / (it.outerRadius * it.outerRadius - it.innerRadius * it.innerRadius);
						// si on d�passe une densit� de 1, on sort
						if (tmp >= 1.0f)
						{
							return 1.0f;
						}
						else if (tmp > localDensity)
						{
							localDensity = tmp;
						}
					}
				}

				return localDensity;
			}
			private void reset()
			{
				this.m_densityMap = new System.Drawing.Bitmap(DENSITY_MAP_SIZE, DENSITY_MAP_SIZE);
				this.m_density = new float[m_densityMap.Width, m_densityMap.Height];

				for (int i = 0; i < m_densityMap.Width; i++)
					for (int j = 0; j < m_densityMap.Height; j++)
						m_density[i,j] = 0f;

			}
		}
	}
}