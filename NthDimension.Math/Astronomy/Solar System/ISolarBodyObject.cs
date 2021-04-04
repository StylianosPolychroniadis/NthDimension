using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Properties of a Solar Body
	/// </summary>
	interface ISolarBodyObject
	{
		void OrbitalElements();

		double N  {get; set;}
		double i  {get; set;}
		double w  {get; set;}
		double a  {get; set;}
		double ec {get; set;}
		double M  {get; set;}

		double d	 {get; set;}
		double T	 {get; set;}
		double d0	 {get; set;}
		string name  {get; set;}

		double dist  {get; set;}
		double diam  {get; set;}
		double elong {get; set;}
		double FV {get; set;}
		double phase {get; set;}
		double magnitude {get; set;}
	}
}
