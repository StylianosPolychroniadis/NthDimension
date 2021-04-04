using NthDimension.Math.Geography.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NthDimension.Math.Geography
{
	public class SatelliteWebData
	{
		public struct webdata
		{
			public string webUrl;

			public string localFileName;
		}

		public const int maxNumSats = 1000;

		private static int numSats;

		public string satDataSource;

		public List<string> satRaw = new List<string>();

		public static SatInfo[] satDat = new SatInfo[1000];

		public static bool satDataReady = false;

		public string tleErrorString;

		public bool checkSatGroupFiles()
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string path = Path.Combine(directoryName, "data\\SatData\\SatGroups");
			string[] files = Directory.GetFiles(path, "SatGroup??.*");
			files.Count();
			string[] array = files;
			string[] array2 = array;
			foreach (string path2 in array2)
			{
				string[] array3 = File.ReadAllLines(path2);
				if (array3.Count() < 2)
				{
					continue;
				}
				for (int j = 1; j < array3.Count(); j++)
				{
					if (array3[j].Trim().Length > 2)
					{
						SatInfo tleData = getTleData(array3[j].Trim());
						checkForBadTle(tleData.satName, tleData.line1, tleData.line2);
					}
				}
			}
			return true;
		}

		public string findWorkingSatellite()
		{
			bool flag = false;
			string result = null;
			string text = null;
			SatInfo satInfo = default(SatInfo);
			satInfo.satName = null;
			satInfo.line1 = null;
			satInfo.line2 = null;
			try
			{
				using (StreamReader streamReader = new StreamReader("data\\SatData\\AllSats.txt"))
				{
					text = streamReader.ReadToEnd();
				}
			}
			catch (Exception)
			{
			}
			string[] array = text.Split(new string[2]
			{
				"\r\n",
				"\n"
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains("ISS "))
				{
					for (int j = 0; j < getNumSats(); j++)
					{
						if (satDat[j].satName.Trim() == array[i].Trim())
						{
							satInfo.satName = satDat[j].satName.Trim();
							satInfo.line1 = satDat[j].line1;
							satInfo.line2 = satDat[j].line2;
							break;
						}
					}
					if ((satInfo.satName != null) & (satInfo.line1 != null) & (satInfo.line2 != null))
					{
						flag = !checkForBadTle(satInfo.satName.Trim(), satInfo.line1.Trim(), satInfo.line2.Trim());
					}
					if (flag)
					{
						result = satInfo.satName;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				int num = 0;
				if (num < array.Length)
				{
					for (int k = 0; k < getNumSats(); k++)
					{
						if (satDat[k].satName.Trim() == array[num].Trim())
						{
							satInfo.satName = satDat[k].satName.Trim();
							satInfo.line1 = satDat[k].line1;
							satInfo.line2 = satDat[k].line2;
							break;
						}
					}
					if ((satInfo.satName != null) & (satInfo.line1 != null) & (satInfo.line2 != null))
					{
						flag = !checkForBadTle(satInfo.satName.Trim(), satInfo.line1.Trim(), satInfo.line2.Trim());
					}
					if (flag)
					{
						result = satInfo.satName;
					}
				}
			}
			return result;
		}

		public bool startupTleCheck()
		{
			bool flag = false;
			bool result = false;
			checkSatGroupFiles();
			string[] files = Directory.GetFiles("data/SatData/SatGroups/", "*.txt");
			if (files.Length != 0)
			{
				string[] array = files;
				string[] array2 = array;
				foreach (string path in array2)
				{
					StreamReader streamReader = new StreamReader(path);
					int num = 0;
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.Trim();
						if (num == 1 && !flag && text.Length > 2)
						{
							flag = true;
							break;
						}
						num++;
					}
					streamReader.Close();
				}
			}
			if (!flag)
			{
				string text2 = "DEFAULT";
				string text3 = findWorkingSatellite();
				StreamWriter streamWriter = new StreamWriter("data\\SatData\\SatGroups\\SatGroup0.txt");
				streamWriter.WriteLine(text2.Trim());
				streamWriter.WriteLine(text3.Trim());
				streamWriter.Close();
				result = true;
			}
			return result;
		}

		public bool initializeProgramData()
		{
			Directory.CreateDirectory("data/");
			Directory.CreateDirectory("data/Images/");
			Directory.CreateDirectory("data/SatData/");
			Directory.CreateDirectory("data/SatData/ManualSatData/");
			Directory.CreateDirectory("data/SatData/SatGroups/");
			Directory.CreateDirectory("data/SatData/TleErrors/");
			return true;
		}

		public double getEpochAge(NoradTle tle)
		{
			string text = "";
			string text2 = "";
			text = ((Convert.ToInt16(tle.Epoch.Substring(0, 2)) < 57) ? ("20" + tle.Epoch.Substring(0, 2)) : ("19" + tle.Epoch.Substring(0, 2)));
			text2 = tle.Epoch.Substring(2);
			DateTime value = new DateTime(Convert.ToInt16(text), 1, 1).AddDays(Convert.ToDouble(text2) - 1.0);
			return DateTime.Now.Subtract(value).TotalDays;
		}

		public bool checkForBadTle(string tleLine0, string tleLine1, string tleLine2)
		{
			bool flag = false;
			NoradTle noradTle = new NoradTle("ISS(ZARYA)", "1 25544U 98067A   14126.92299264  .00007994  00000-0  14853-3 0  2319", "2 25544  51.6500 304.1061 0002752 337.8472 139.1065 15.49877967884923");
			try
			{
				noradTle = new NoradTle(tleLine0, tleLine1, tleLine2);
			}
			catch
			{
				flag = true;
				tleErrorString = " [Convert tle strings to tle structure error]";
			}
			if (!flag && !double.IsNegativeInfinity(checkTleForDecay(noradTle)))
			{
				flag = true;
				tleErrorString = " [Decay: approx. epoch date + " + (checkTleForDecay(noradTle) / 1440.0).ToString("F1") + " Days]";
			}
			if (!flag)
			{
				try
				{
					Convert.ToDouble(noradTle.BStarDrag);
				}
				catch
				{
					flag = true;
					tleErrorString = " [Convert tle BStarDrag string to double error]";
				}
			}
			if (!flag)
			{
				try
				{
					Convert.ToDouble(noradTle.MeanMotionDt);
				}
				catch
				{
					flag = true;
					tleErrorString = " [Convert MeanMotionDt string to double error]";
				}
			}
			if (!flag)
			{
				try
				{
					Convert.ToDouble(noradTle.Epoch);
				}
				catch
				{
					flag = true;
					tleErrorString = " [Convert Epoch string to double error]";
				}
			}
			return flag;
		}

		public double checkTleForDecay(NoradTle tle)
		{
			Orbit orbit = new Orbit(tle);
			try
			{
				orbit = new Orbit(tle);
			}
			catch
			{
			}
			bool flag = false;
			double epochMinutes = getEpochAge(tle) * 24.0 * 60.0;
			for (double num2 = epochMinutes; num2 < epochMinutes + 4320.0; num2 += 60.0)
			{
				try
				{
					orbit.GetPosition(num2);
				}
				catch (DecayException)
				{
					flag = true;
					return epochMinutes;
				}
				if (!flag)
				{
					epochMinutes = double.NegativeInfinity;
				}
			}
			return epochMinutes;
		}

		public SatInfo getTleData(string satName)
		{
			SatInfo result = default(SatInfo);
			result.satName = satName;
			result.line1 = null;
			result.line2 = null;
			for (int i = 0; i < satDat.Length; i++)
			{
				if (satDat[i].satName.Trim() == satName.Trim())
				{
					result.satName = satDat[i].satName.Trim();
					result.line1 = satDat[i].line1;
					result.line2 = satDat[i].line2;
					break;
				}
			}
			return result;
		}

		public string getSatNumber(string satName)
		{
			bool flag = false;
			int satIdx = 0;
			string strLine = "";
			string strLine2 = "";
			for (satIdx = 0; satIdx < getNumSats(); satIdx++)
			{
				if (satDat[satIdx].satName.Trim() == satName.Trim())
				{
					flag = true;
					satDat[satIdx].satName.Trim();
					strLine = satDat[satIdx].line1.Trim();
					strLine2 = satDat[satIdx].line2.Trim();
				}
			}
			if (flag)
			{
				NoradTle noradTle = new NoradTle(satName, strLine, strLine2);
				return noradTle.NoradNumber;
			}
			return "0";
		}

		public string getFirstWebDataSourceFileName()
		{
			webdata[] webDataSources = getWebDataSources();
			return webDataSources[0].localFileName;
		}

		public webdata[] getWebDataSources()
		{
			bool flag = false;
			int num = 0;
			string text = "";
			webdata[] array = null;
			try
			{
				using (StreamReader streamReader = new StreamReader("data/SatData/WebDataSources.txt"))
				{
					while ((text = streamReader.ReadLine()) != null)
					{
						if (text.Substring(0, 1) != "-")
						{
							num++;
						}
					}
					streamReader.Close();
				}
			}
			catch (Exception)
			{
				num = 0;
			}
			if (num > 0)
			{
				array = new webdata[num];
			}
			int num2 = 0;
			try
			{
				using (StreamReader streamReader2 = new StreamReader("data/SatData/WebDataSources.txt"))
				{
					while ((text = streamReader2.ReadLine()) != null)
					{
						if (text.Substring(0, 1) != "-")
						{
							string webUrl = text.Substring(0, text.IndexOf(","));
							array[num2].webUrl = webUrl;
							string text2 = text.Substring(text.IndexOf(",") + 1);
							array[num2].localFileName = "data/SatData/" + text2.Trim();
							num2++;
						}
					}
					streamReader2.Close();
				}
			}
			catch (Exception)
			{
				flag = true;
			}
			return array;
		}

		private bool writeAllSatsFile()
		{
			string text = "";
			string[] array = new string[numSats];
			for (int i = 0; i < numSats; i++)
			{
				if (!array.Contains(satDat[i].satName.Trim()))
				{
					array[i] = satDat[i].satName.Trim();
				}
			}
			Array.Sort(array);
			text = null;
			for (int j = 0; j < array.Length; j++)
			{
				text = text + array[j] + "\r\n";
			}
			string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string path = Path.Combine(directoryName, "data\\SatData\\AllSats.txt");
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.WriteLine(text.Trim());
			streamWriter.Close();
			return true;
		}

		public AzimuthElevation getAzElAbsoluteTime(string satName, ObserverData obsDat, DateTime dt)
		{
			bool flag = false;
			string strLine = null;
			string strLine2 = null;
			AzimuthElevation result = default(AzimuthElevation);
			result.satName = " ";
			result.azimuth = 0.0;
			result.elevation = 0.0;
			result.altitude = 0.0;
			result.latitude = 0.0;
			result.longitude = 0.0;
			result.period = TimeSpan.Zero;
			result.epoch = DateTime.MinValue;
			result.range = 0.0;
			result.rangeRate = 0.0;
			result.satNumber = " ";
			for (int i = 0; i < getNumSats(); i++)
			{
				if (satDat[i].satName.Trim() == satName.Trim())
				{
					flag = true;
					result.satName = satDat[i].satName.Trim();
					strLine = satDat[i].line1;
					strLine2 = satDat[i].line2;
				}
			}
			if (flag)
			{
				NoradTle noradTle = new NoradTle(result.satName, strLine, strLine2);
				Orbit orbit = new Orbit(noradTle);
				double minutesPastEpoch = Convert.ToDouble(orbit.TPlusEpoch(dt).TotalMinutes);
				GeoVector position = orbit.GetPosition(minutesPastEpoch);
				GeoSite geoSite = new GeoSite(Convert.ToDouble(obsDat.obsLatitude), Convert.ToDouble(obsDat.obsLongitude), Convert.ToDouble(obsDat.obsAltitude) / 1000.0);
				GeoVector.CoordTopo lookAngle = geoSite.GetLookAngle(position);
				result.azimuth = Constants.Rad2Deg(lookAngle.Azimuth);
				result.elevation = Constants.Rad2Deg(lookAngle.Elevation);
				result.altitude = position.ToGeo().Altitude;
				result.latitude = position.ToGeo().Latitude;
				result.longitude = position.ToGeo().Longitude;
				result.period = orbit.Period;
				result.epoch = orbit.EpochTime;
				result.range = lookAngle.Range;
				result.rangeRate = lookAngle.RangeRate;
				result.satNumber = noradTle.NoradNumber;
			}
			return result;
		}

		public AzimuthElevation getAzEl(string satName, ObserverData obsDat, double mpeOffset)
		{
			bool flag = false;
			string strLine = null;
			string strLine2 = null;
			AzimuthElevation result = default(AzimuthElevation);
			result.satName = "NULL ";
			result.azimuth = 0.0;
			result.elevation = 0.0;
			result.altitude = 0.0;
			result.latitude = 0.0;
			result.longitude = 0.0;
			result.period = TimeSpan.Zero;
			result.epoch = DateTime.MinValue;
			result.range = 0.0;
			result.rangeRate = 0.0;
			result.satNumber = " ";
			for (int i = 0; i < getNumSats(); i++)
			{
				if (satDat[i].satName.Trim() == satName.Trim())
				{
					flag = true;
					result.satName = satDat[i].satName;
					strLine = satDat[i].line1;
					strLine2 = satDat[i].line2;
				}
			}
			if (flag)
			{
				NoradTle noradTle = new NoradTle(result.satName, strLine, strLine2);
				Orbit orbit = new Orbit(noradTle);
				DateTime gmt = DateTime.Now.AddMinutes(mpeOffset).ToUniversalTime();
				double minutesPastEpoch = Convert.ToDouble(orbit.TPlusEpoch(gmt).TotalMinutes);
				GeoVector position = orbit.GetPosition(minutesPastEpoch);
				GeoSite geoSite = new GeoSite(Convert.ToDouble(obsDat.obsLatitude), Convert.ToDouble(obsDat.obsLongitude), Convert.ToDouble(obsDat.obsAltitude) / 1000.0);
				GeoVector.CoordTopo lookAngle = geoSite.GetLookAngle(position);
				result.azimuth = Constants.Rad2Deg(lookAngle.Azimuth);
				result.elevation = Constants.Rad2Deg(lookAngle.Elevation);
				result.altitude = position.ToGeo().Altitude;
				result.latitude = position.ToGeo().Latitude;
				result.longitude = position.ToGeo().Longitude;
				result.period = orbit.Period;
				result.epoch = orbit.EpochTime;
				result.range = lookAngle.Range;
				result.rangeRate = lookAngle.RangeRate;
				result.satNumber = noradTle.NoradNumber;
			}
			return result;
		}

		public bool getWebSatData(bool webUpdate)
		{
			bool result = false;
			int num = 0;
			satRaw.Clear();
			webdata[] webDataSources = getWebDataSources();
			if (webUpdate)
			{
				for (num = 0; num < webDataSources.Length; num++)
				{
					satRaw.Clear();
					if (downloadSatData(webDataSources[num].webUrl))
					{
						writeRawSatData(webDataSources[num].localFileName);
					}
				}
			}
			for (num = 0; num < webDataSources.Length; num++)
			{
				if (!readRawSatData(webDataSources[num].localFileName))
				{
					satRaw.Clear();
					if (downloadSatData(webDataSources[num].webUrl))
					{
						writeRawSatData(webDataSources[num].localFileName);
					}
				}
			}
			satRaw.Clear();
			for (num = 0; num < webDataSources.Length; num++)
			{
				readRawSatData(webDataSources[num].localFileName);
			}
			readManualTleFiles();
			parseRawSatData();
			writeAllSatsFile();
			return result;
		}

		public int getNumSats()
		{
			return numSats;
		}

		public bool readManualTleFiles()
		{
			int num = 0;
			bool result = false;
			string text = null;
			string text2 = null;
			string text3 = null;
			string path = "data/SatData/ManualSatData/";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] files = Directory.GetFiles(path, "*.tle");
			StreamWriter streamWriter = new StreamWriter("data/SatData/ProblemManualTleData.txt", false);
			if (files.Length != 0)
			{
				List<string> list = new List<string>();
				string[] array = files;
				string[] array2 = array;
				foreach (string path2 in array2)
				{
					StreamReader streamReader = new StreamReader(path2);
					string text4;
					while ((text4 = streamReader.ReadLine()) != null)
					{
						text4 = text4.Trim();
						if (text4.Length > 3)
						{
							list.Add(text4);
							satRaw.Add(text4);
							num++;
						}
						if (num > 2)
						{
							num = 0;
							numSats++;
						}
					}
					streamReader.Close();
				}
				if (list.Count >= 3)
				{
					try
					{
						StreamWriter streamWriter2 = new StreamWriter("data/SatData/ManualSatData.txt");
						foreach (string item in list)
						{
							if (num == 0)
							{
								text = item;
							}
							if (num == 1)
							{
								text2 = item;
							}
							if (num == 2)
							{
								text3 = item;
							}
							num++;
							if (num == 3)
							{
								num = 0;
								if (checkForBadTle(text, text2, text3))
								{
									result = true;
									streamWriter.WriteLine(text.Trim() + tleErrorString);
									streamWriter.WriteLine(text2.Trim());
									streamWriter.WriteLine(text3.Trim());
								}
								else
								{
									result = false;
									streamWriter2.WriteLine(text);
									streamWriter2.WriteLine(text2);
									streamWriter2.WriteLine(text3);
								}
							}
						}
						streamWriter2.Close();
					}
					catch (Exception)
					{
						result = true;
					}
				}
			}
			else
			{
				FileInfo fileInfo = new FileInfo("data/SatData/ManualSatData.txt");
				fileInfo.Delete();
				result = true;
			}
			streamWriter.Close();
			if (new FileInfo("data/SatData/ProblemManualTleData.txt").Length == 0)
			{
				FileInfo fileInfo2 = new FileInfo("data/SatData/ProblemManualTleData.txt");
				fileInfo2.Delete();
			}
			return result;
		}

		private bool writeRawSatData(string fileName)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			string text = null;
			string text2 = null;
			string text3 = null;
			StreamWriter streamWriter = new StreamWriter(fileName);
			StreamWriter streamWriter2 = new StreamWriter("data/SatData/ProblemTleData.txt", false);
			try
			{
				foreach (string item in satRaw)
				{
					if (num2 == 0)
					{
						text = item;
					}
					if (num2 == 1)
					{
						text2 = item;
					}
					if (num2 == 2)
					{
						text3 = item;
					}
					num2++;
					if (num2 == 3)
					{
						num2 = 0;
						bool flag2 = checkForBadTle(text, text2, text3);
						flag = false;
						if (!flag2)
						{
							bool flag3 = false;
							int num3 = 0;
							while (!flag3 && num3 < num)
							{
								if (text.Trim() == satDat[num3].satName)
								{
									flag3 = true;
								}
								num3++;
							}
							if (!flag3)
							{
								streamWriter.WriteLine(text);
								streamWriter.WriteLine(text2);
								streamWriter.WriteLine(text3);
							}
						}
						else
						{
							streamWriter2.WriteLine(text.Trim() + tleErrorString);
							streamWriter2.WriteLine(text2.Trim());
							streamWriter2.WriteLine(text3.Trim());
						}
					}
				}
			}
			catch (Exception)
			{
				streamWriter2.Close();
				streamWriter.Close();
				return false;
			}
			streamWriter2.Close();
			streamWriter.Close();
			return true;
		}

		private bool readRawSatData(string fileName)
		{
			try
			{
				using (StreamReader streamReader = new StreamReader(fileName))
				{
					string item;
					while ((item = streamReader.ReadLine()) != null)
					{
						satRaw.Add(item);
					}
					streamReader.Close();
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private bool parseRawSatData()
		{
			bool flag = false;
			bool result = true;
			int num = 0;
			int num2 = 0;
			string text = null;
			string text2 = null;
			string text3 = null;
			StreamWriter streamWriter = new StreamWriter("data/SatData/ProblemTleDataParse.txt", false);
			try
			{
				foreach (string item in satRaw)
				{
					if (num2 == 0)
					{
						text = item;
					}
					if (num2 == 1)
					{
						text2 = item;
					}
					if (num2 == 2)
					{
						text3 = item;
					}
					num2++;
					if (num2 == 3)
					{
						num2 = 0;
						bool flag2 = checkForBadTle(text, text2, text3);
						flag = false;
						if (!flag2)
						{
							bool flag3 = false;
							int num3 = 0;
							while (!flag3 && num3 < num)
							{
								if (text.Trim() == satDat[num3].satName.Trim())
								{
									flag3 = true;
								}
								num3++;
							}
							if (!flag3)
							{
								NoradTle tle = new NoradTle(text, text2, text3);
								double epochAge = getEpochAge(tle);
								satDat[num].satName = text;
								satDat[num].line1 = text2;
								satDat[num].line2 = text3;
								num = (numSats = num + 1);
							}
						}
						else
						{
							streamWriter.WriteLine(text.Trim() + tleErrorString);
							streamWriter.WriteLine(text2.Trim());
							streamWriter.WriteLine(text3.Trim());
						}
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			streamWriter.Close();
			if (new FileInfo("data/SatData/ProblemTleDataParse.txt").Length == 0)
			{
				FileInfo fileInfo = new FileInfo("data/SatData/ProblemTleDataParse.txt");
				fileInfo.Delete();
			}
			return result;
		}

		private bool downloadSatData(string satDataURL)
		{
			satDataReady = false;
			WebClient webClient = new WebClient();
			try
			{
				using (MemoryStream stream = new MemoryStream(webClient.DownloadData(satDataURL)))
				{
					StreamReader streamReader = new StreamReader(stream);
					string item;
					while ((item = streamReader.ReadLine()) != null)
					{
						satRaw.Add(item);
					}
					streamReader.Close();
				}
			}
			catch (Exception)
			{
				return false;
			}
			satDataReady = true;
			return true;
		}
	}
}
