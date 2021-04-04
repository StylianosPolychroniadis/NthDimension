using System;
using System.Drawing;
using System.Collections;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	/// 

	public class PlanetData
	{
		public Planets planets = new Planets(
			new Sun("Sun"), new Moon("Moon"), new Mercury("Mercury"), new Venus("Venus"), 
			new Earth("Earth"), new Mars("Mars"), new Jupiter("Jupiter"), new Saturn("Saturn"), 
			new Uranus("Uranus"), new Neptune("Neptune"), new Pluto("Pluto"), new EarthShadow("Earth shadow"));

		public Orbits orbits = new Orbits(orbitNames);
		public Orbits copyOrb = new Orbits(orbitNames);

		public static PlanetData GetInstance()
		{
			if( instance == null )
				instance = new PlanetData();
			return instance;
		}

		public void PlanetPositions()
		{
			planets["Sun"].ResetPlanet();

			planets["Sun"].OrbitalElements();
			planets["Jupiter"].OrbitalElements();
			planets["Saturn"].OrbitalElements();
			planets["Uranus"].OrbitalElements();
			
			planets["Sun"].GeocentricPos();
			planets["Earth"].OrbitalElements();
			planets["Earth"].HeliocentricPos();
			planets["Earth shadow"].OrbitalElements();
			planets["Earth shadow"].GeocentricPos();

			planets["Moon"].OrbitalElements();
			planets["Moon"].HeliocentricPos();
			planets["Moon"].Perturbations();
			planets["Moon"].GeocentricPos();
			planets["Moon"].TopocentricPos();

			planets["Mercury"].OrbitalElements();
			planets["Mercury"].HeliocentricPos();
			planets["Mercury"].GeocentricPos();

			planets["Venus"].OrbitalElements();
			planets["Venus"].HeliocentricPos();
			planets["Venus"].GeocentricPos();

			planets["Mars"].OrbitalElements();
			planets["Mars"].HeliocentricPos();
			planets["Mars"].GeocentricPos();

			planets["Jupiter"].HeliocentricPos();
			planets["Jupiter"].Perturbations();
			planets["Jupiter"].GeocentricPos();

			planets["Saturn"].HeliocentricPos();
			planets["Saturn"].Perturbations();
			planets["Saturn"].GeocentricPos();

			planets["Uranus"].HeliocentricPos();
			planets["Uranus"].Perturbations();
			planets["Uranus"].GeocentricPos();

			planets["Neptune"].OrbitalElements();
			planets["Neptune"].HeliocentricPos();
			planets["Neptune"].GeocentricPos();

			planets["Pluto"].OrbitalElements();
			planets["Pluto"].HeliocentricPos();
			planets["Pluto"].GeocentricPos();

			planets["Sun"].Ephemerides();
			planets["Moon"].Ephemerides();
			foreach (string s in orbitNames)
				planets[s].Ephemerides();

			double RES = 0;
			double RZ = 6378.140;
			double RS = 696000;
			double dS = planets["Sun"].dist * 1.49597870E8;
			double dM = planets["Moon"].dist * 1.49597870E8;
			((EarthShadow)planets["Earth shadow"]).DP = RES = (RZ+RS)*(dS+dM)/dS - RS;
			planets["Earth shadow"].diam = 7200*System.Math.Atan(RES/dM)*180/System.Math.PI;
			((EarthShadow)planets["Earth shadow"]).DU = RS - ((RS-RZ)*(dS+dM))/dS;
		}

		public void PlanetOrb()
		{
			DateTime dt = location.v_mainDT;
			foreach (string s in orbitNames){
				for (short i=0; i<30; ++i){
					PlanetPositions();
					location.v_mainDT = location.v_mainDT.AddDays (planets[s].T/27.4);
					PlanetPos pp = new PlanetPos();
					pp.x = planets[s].position.x;
					pp.y = planets[s].position.y;
					pp.z = planets[s].position.z;
					pp.posName = s;
					orbits[s,i] = pp;
				}
				if (s == "Saturn")	location.v_mainDT = new DateTime(1950,1,1,0,0,0);
				if (s == "Uranus")	location.v_mainDT = new DateTime(1920,1,1,0,0,0);
				if (s == "Neptune")	location.v_mainDT = new DateTime(1900,1,1,0,0,0);
				if (s == "Pluto")	location.v_mainDT = dt;
			}
		}

		public void RotateOrbit(int angle_x, int angle_z)
		{
			foreach (string s in orbitNames){
				for (short i=0; i<30; ++i){
					PlanetPos pp = new PlanetPos();
					pp = orbits[s,i];
					pp.Rotate(angle_x, angle_z);
					copyOrb[s,i] = pp;
				}
			}
		}

		private PlanetData(){}

		private LocationST location = LocationST.GetInstance();
		private static PlanetData instance;
		private static string[] orbitNames = {"Mercury","Venus","Earth","Mars","Jupiter",
												 "Saturn","Uranus","Neptune","Pluto"};
	}
}
