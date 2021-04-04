using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public struct PlanetPos
	{
		public double x, y, z;
		public string posName;

		public PlanetPos(double xx, double yy, double zz)
		{
			x = xx;	y = yy;	z = zz;	posName = "";
		}

		public void Rotate (double angle_x, double angle_z)
		{
			double r = System.Math.Sqrt(x*x+y*y+z*z);
			
			double a=0, al=0;
			al = System.Math.Atan(System.Math.Abs(y)/System.Math.Abs(x))*180/System.Math.PI;

			if (x>0 && y==0) a = 0;
			if (x==0 && y>0) a = 90;
			if (x<0 && y==0) a = 180;
			if (x==0 && y<0) a = 270;

			if (x>0 && y>0) a = al;
			if (x<0 && y>0) a = 180-al;
			if (x<0 && y<0) a = 180+al;
			if (x>0 && y<0) a = 360-al;

			a += angle_z;
			a = a%360;
			double tk = System.Math.Tan(a*System.Math.PI/180);
			if (a>=90 && a<270) x = -System.Math.Sqrt(r*r/(1+tk*tk));
			else x = System.Math.Sqrt(r*r/(1+tk*tk));    
			y = x*tk; 

			y = y*System.Math.Cos(angle_x*System.Math.PI/180);
			z = z*System.Math.Sin(angle_x*System.Math.PI/180);
			y += z;
		}
	}
}
