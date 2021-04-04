using System;
using System.Collections;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for DeepSpaceData.
	/// </summary>
	public class DeepSpaceData
	{
		public ArrayList            stars               = new ArrayList();
		public ArrayList            messier             = new ArrayList();
		public ArrayList            constellation       = new ArrayList();
		public ArrayList            constellationNames  = new ArrayList();

		public static DeepSpaceData GetInstance(){
			if( instance == null )
				instance = new DeepSpaceData();
			return instance;
		}

		private DeepSpaceData()
		{
			Assembly a = Assembly.GetExecutingAssembly();
			Stream txtStream = a.GetManifestResourceStream("NthDimension.Math.Astronomy.Resources.HYG.txt");
			StreamReader sr = new StreamReader(txtStream);

			string delimStr = ";";
			char[] delimiter = delimStr.ToCharArray();
			NumberFormatInfo provider = new NumberFormatInfo( );
			provider.NumberDecimalSeparator = ".";
			while (sr.Peek() >= 0) 
			{
				string[] split = sr.ReadLine().Split(delimiter, 20);
				stars.Add (new Star(split[0], split[1], Convert.ToDouble(split[2], provider)*15,
					                Convert.ToDouble(split[3], provider), 
					                Convert.ToDouble(split[4], provider), split[5]));
			}
			stars.TrimToSize();

			a = Assembly.GetExecutingAssembly();
			txtStream = a.GetManifestResourceStream("NthDimension.Math.Astronomy.Resources.Messier.txt");
			sr = new StreamReader(txtStream);

			while (sr.Peek() >= 0) 
			{
				string[] split = sr.ReadLine().Split(delimiter, 20);
				messier.Add (new Messier(split[0], Convert.ToDouble(split[1], provider)*15,
					           Convert.ToDouble(split[2], provider), split[3], split[4]));
			}
			messier.TrimToSize();

			a = Assembly.GetExecutingAssembly();
			txtStream = a.GetManifestResourceStream("NthDimension.Math.Astronomy.Resources.Constellations.txt");
			sr = new StreamReader(txtStream);

			while (sr.Peek() >= 0) 
			{
				string str = sr.ReadLine();
				if (str[0] != 'C')
				{
					string[] split = str.Split(delimiter, 20);
					constellation.Add (new ConstellationLine(Convert.ToDouble(split[0], provider)*15, Convert.ToDouble(split[1], provider),
						Convert.ToDouble(split[2], provider)*15, Convert.ToDouble(split[3], provider)));
				}
				else
				{
					string[] split = str.Split(delimiter, 4);
					SkyPos sp = new SkyPos();
					sp.RA = Convert.ToDouble(split[2], provider)*15;
					sp.decl = Convert.ToDouble(split[3], provider);
					constellationNames.Add(new ConstellationName(split[1], sp));
				}
			}
			constellation.TrimToSize();
			constellationNames.TrimToSize();
		}

		private static DeepSpaceData instance;
	}
}
