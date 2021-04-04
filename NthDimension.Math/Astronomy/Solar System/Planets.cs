using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for Planets.
	/// </summary>
	public class Planets
	{
		private SolarBodyObject[] planets = new SolarBodyObject[12];
		private int counter = 0;

		public Planets (params SolarBodyObject[] initPlanets)
		{
			foreach (SolarBodyObject ap in initPlanets)
				planets[counter++] = ap;
		}

		public void Add (SolarBodyObject planet)
		{
			planets[counter++] = planet;
		}

		public SolarBodyObject this[int index]
		{
			get {return planets[index];}
			set {planets[index] = value;}
		}

		public SolarBodyObject this[string index]
		{
			get {return this[findString(index)];}
			set {planets[findString(index)] = value;}
		}

		private int findString (string searchString)
		{
			for (int i=0; i<planets.Length; i++){
				if (planets[i].name == searchString)
					return i;
			}
			return -1;
		}

		public int GetNumEntries()
		{
			return counter;
		}
	}
}
