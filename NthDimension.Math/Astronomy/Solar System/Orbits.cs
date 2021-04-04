using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Orbits
	{
		private PlanetPos[,] orbits = new PlanetPos[9,30];
		private int counter = 0;

		public Orbits (params string[] orbitNames)
		{
			foreach (string s in orbitNames){
				for (int i=0; i<30; ++i){
					orbits[counter,i] = new PlanetPos();
					orbits[counter,i].posName = s;
				}
				++counter;
			}
		}

		public PlanetPos this[int i, int j]
		{
			get {return orbits[i,j];}
			set {orbits[i,j] = value;}
		}

		public PlanetPos this[string index, int j]
		{
			get {return this[findString(index), j]; }
			set {orbits[findString(index), j] = value;}
		}

		private int findString (string searchString)
		{
			for (int i=0; i<9; ++i){
				for (int j=0; j<30; ++j)
					if (orbits[i,j].posName == searchString)
						return i;
			}
			return -1;
		}
	}
}
