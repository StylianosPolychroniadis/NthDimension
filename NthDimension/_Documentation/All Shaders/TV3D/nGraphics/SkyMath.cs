using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTV3D65;

namespace nDimension.nGraphics
{
    class SkyMaths
    {
        #region Constants
        const float HALF_PI = (float)(Math.PI / 2d);
        const float TWO_PI = (float)(Math.PI * 2d);

        /// <summary>
        /// Zenith x value
        /// </summary>
        static float[,] xZenithCoeff = new float[3, 4] 
            { { 0.00165f, -0.00375f,  0.00209f, 0.0f},
			  {-0.02903f,  0.06377f, -0.03202f, 0.00394f},
			  { 0.11693f, -0.21196f,  0.06052f, 0.25886f} };

        /// <summary>
        /// Zenith y value
        /// </summary>
        static float[,] yZenithCoeff = new float[3, 4]
            { { 0.00275f, -0.00610f,  0.00317f, 0.0f},
			  {-0.04214f,  0.08970f, -0.04153f, 0.00516f},
			  { 0.15346f, -0.26756f,  0.06670f, 0.26688f} };

        /// <summary>
        /// Distribution coefficients for the x distribution function
        /// </summary>
        static float[,] xDistribCoeff = new float[5, 2]
            { {-0.0193f, -0.2592f},
			  {-0.0665f,  0.0008f},
			  {-0.0004f,  0.2125f},
			  {-0.0641f, -0.8989f},
			  {-0.0033f,  0.0452f} };

        /// <summary>
        /// Distribution coefficients for the y distribution function
        /// </summary>
        static float[,] yDistribCoeff = new float[5, 2]
            { {-0.0167f, -0.2608f},
              {-0.0950f,  0.0092f},
			  {-0.0079f,  0.2102f},
			  {-0.0441f, -1.6537f},
			  {-0.0109f,  0.0529f} };

        /// <summary>
        /// Distribution coefficients for the Y distribution function
        /// </summary>
        static float[,] YDistribCoeff = new float[5, 2]
            { { 0.1787f, -1.4630f},
			  {-0.3554f,  0.4275f},
			  {-0.0227f,  5.3251f},
			  { 0.1206f, -2.5771f},
			  {-0.0670f,  0.3703f} };

        /// <summary>
        /// XYZ to RGB conversion matrix (rec.709 HDTV XYZ to RGB, D65 white point)
        /// </summary>
        static float[,] XYZtoRGBconv = new float[3, 3] 
            { {  3.24079f,  -1.53715f, -0.49853f},
              {-0.969256f,  1.875991f,  0.041556f},
			  { 0.055648f, -0.204043f,  1.057311f} };
        #endregion

        #region Structures
        public struct xyYColor
        {
            public float x, y, Y;

            public TV_3DVECTOR AsVector3
            {
                get
                {
                    return new TV_3DVECTOR(x, y, Y);
                }
            }
        }

        private struct XYZColor
        {
            public float X, Y, Z;

            public XYZColor(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }

            public float[] AsFloat3
            {
                get
                {
                    return new float[3] { X, Y, Z };
                }
            }
        }

        public struct xyYCoeffs
        {
            public float[] x;
            public float[] y;
            public float[] Y;
        }


        public struct AltAzAngles
        {
            public AltAzAngles(float altitude, float azimuth)
            {
                this.altitude = altitude;
                this.azimuth = azimuth;
            }

            public AltAzAngles Invert()
            {
                return new AltAzAngles(-this.altitude, -this.azimuth);
            }

            public float altitude;
            public float azimuth;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the sky Zenith colors for the specified sunTheta, at AngleBetween = 0
        /// </summary>
        public static xyYColor SkyZenithColor(float turbidity, float sunTheta)
        {
            xyYColor zenith;

            // Zenith luminance
            float chi = (4f / 9f - turbidity / 120f) * ((float)Math.PI - 2f * sunTheta);
            zenith.Y = (4.0453f * turbidity - 4.971f) * (float)Math.Tan(chi) - 0.2155f * turbidity + 2.4192f;
            if (zenith.Y < 0f) zenith.Y = -zenith.Y;

            // Zenith chromacity	
            zenith.x = Chromaticity(xZenithCoeff, turbidity, sunTheta);
            zenith.y = Chromaticity(yZenithCoeff, turbidity, sunTheta);

            return zenith;
        }

        /// <summary>
        /// Calculates the maximum luminance for the supplied turbidity and sun theta
        /// </summary>
        public static float MaximumLuminance(float turbidity, float sunTheta, xyYColor zenith, xyYCoeffs coeffs)
        {
            float theta = sunTheta;
            if (sunTheta >= HALF_PI) theta = HALF_PI - 0.01f;
            return Distribution(coeffs.Y, theta, zenith.Y, 0f) * 1.5f;
        }

        /// <summary>
        /// Calculates the RGB atmospheric color (fog + lightning use this in sunrise/sunset)
        /// </summary>
        public static TV_COLOR AtmosphereColor(float turbidity, float sunTheta, xyYColor zenith, xyYCoeffs coeffs)
        {
            float theta = Math.Min(sunTheta + 0.15f, HALF_PI - 0.01f);
            xyYColor skyColor;

            // Sky color distribution (using the Perez Function)
            skyColor.x = Distribution(coeffs.x, theta, zenith.x, 0.2f);
            skyColor.y = Distribution(coeffs.y, theta, zenith.y, 0.2f);
            skyColor.Y = 0.5f;

            TV_COLOR ret = xyYtoRGB(skyColor);
            return ret;
        }

        /// <summary>
        /// Calculates distribution coefficients in relation to turbidity
        /// </summary>
        public static xyYCoeffs DistributionCoefficients(float turbidity)
        {
            xyYCoeffs ret;
            ret.x = new float[5]; ret.y = new float[5]; ret.Y = new float[5];

            for (int i = 0; i < 5; i++)
            {
                ret.x[i] = xDistribCoeff[i, 0] * turbidity + xDistribCoeff[i, 1];
                ret.y[i] = yDistribCoeff[i, 0] * turbidity + yDistribCoeff[i, 1];
                ret.Y[i] = YDistribCoeff[i, 0] * turbidity + YDistribCoeff[i, 1];
            }

            return ret;
        }

        /// <summary>
        /// Calculates accurate theta polar coordinates from a normalized vector
        /// </summary>
        public static float VectorToTheta(TV_3DVECTOR vec)
        {
            return HALF_PI - (float)Math.Atan2(vec.y, Math.Sqrt(vec.x * vec.x + vec.z * vec.z));
        }

        /// <summary>
        /// Convert directly from xyY to RGB
        /// </summary>
        public static TV_COLOR xyYtoRGB(xyYColor xyY)
        {
            float Yony = xyY.Y / xyY.y;
            XYZColor XYZ = new XYZColor(xyY.x * Yony, xyY.Y, (1.0f - xyY.x - xyY.y) * Yony);

            float[] XYZf3 = XYZ.AsFloat3;
            float[] ret = new float[3];
            for (int i = 0; i < 3; i++)
            {
                ret[i] = 0f;
                for (int j = 0; j < 3; j++)
                    ret[i] += XYZf3[j] * XYZtoRGBconv[i, j];
            }

            return new TV_COLOR(ret[0], ret[1], ret[2], 1.0f);
        }

        /// <summary>
        /// Some other sun position calculation, heavier but gets theta's from 0 to pi.
        /// Based on <![CDATA[http://kogs-www.informatik.uni-hamburg.de/~wiemker/]]>
        /// Has been heavily optimized and chopped down, it's not entirely accurate.
        /// </summary>
        public static AltAzAngles CalculateSunPosition(float julianDate, float latitude)
        {
            AltAzAngles angles;
            float gamma = 4.93073839645544f;

            float meanAnomaly = 6.2398418f + 0.0172019696455f * julianDate;
            float eccAnomaly = 2f * (float)Math.Atan(1.016862146d * Math.Tan(meanAnomaly / 2d));
            eccAnomaly = meanAnomaly + 0.016720f * (float)Math.Sin(eccAnomaly);
            float trueAnomaly = 2f * (float)Math.Atan(1.016862146d * Math.Tan(eccAnomaly / 2d));
            float lambda = gamma + trueAnomaly;

            float dec = (float)Math.Asin(Math.Sin(lambda) * 0.39778375791855d);
            float ra = (float)Math.Atan(Math.Tan(lambda) * 0.917479d);
            if (Math.Cos(lambda) < 0d) ra += (float)Math.PI;

            float gha = 1.7457f + 6.300388098526f * julianDate;
            float latSun = dec;
            float lonSun = ra - gha;

            // To prevent over-calculation
            float cosLonSun = (float)Math.Cos(lonSun);
            float sinLonSun = (float)Math.Sin(lonSun);
            float cosLatSun = (float)Math.Cos(latSun);
            float sinLatSun = (float)Math.Sin(latSun);
            float sinLat = (float)Math.Sin(latitude);
            float cosLat = (float)Math.Cos(latitude);

            angles.altitude = (float)Math.Asin(sinLat * sinLatSun + cosLat * cosLatSun * cosLonSun);
            float west = cosLatSun * sinLonSun;
            float south = -cosLat * sinLatSun + sinLat * cosLatSun * cosLonSun;
            angles.azimuth = (float)Math.Atan(west / south);

            if (south >= 0f) angles.azimuth = (float)Math.PI - angles.azimuth;
            if (south < 0f) angles.azimuth = -angles.azimuth;
            if (angles.azimuth < 0f) angles.azimuth += TWO_PI;

            return angles;
        }

        // Emulates the HLSL instrinsic "saturate" (ASM -> _sat)
        public static float Saturate(float value)
        {
            if (value > 1f) value = 1f;
            if (value < 0f) value = 0f;
            return value;
        }

        // Emulates the HLSL instrinsic "saturate" (ASM -> _sat)
        public static TV_COLOR Saturate(TV_COLOR value)
        {
            value.r = Saturate(value.r);
            value.g = Saturate(value.g);
            value.b = Saturate(value.b);
            return value;
        }

        // A custom LERP function
        public static float Lerp(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Perez Function
        /// </summary>
        static float PerezFunction(float A, float B, float C, float D, float E, float theta, float gamma)
        {
            float cosGamma = (float)Math.Cos(gamma);
            return (float)((1.0f + A * Math.Exp(B / Math.Cos(theta))) * (1.0f + C * Math.Exp(D * gamma) + E * cosGamma * cosGamma));
        }

        /// <summary>
        /// Calculates distribution using two Perez function calls
        /// </summary>
        static float Distribution(float[] coeffs, float theta, float zenith, float gamma)
        {
            float A = coeffs[0], B = coeffs[1], C = coeffs[2], D = coeffs[3], E = coeffs[4];
            return (zenith * PerezFunction(A, B, C, D, E, theta, gamma) / PerezFunction(A, B, C, D, E, 0f, theta));
        }

        /// <summary>
        /// Calculates chromaticity (zenith)
        /// </summary>
        static float Chromaticity(float[,] ZC, float turbidity, float sunTheta)
        {
            // Theta, Theta² and Theta³
            float sunThetaSquared = sunTheta * sunTheta;
            float sunThetaCubed = sunThetaSquared * sunTheta;

            // Turbidity²
            float turbiditySquared = turbidity * turbidity;

            // Vectors will help us with the computation
            float[] turbidityVector = { turbiditySquared, turbidity, 1.0f };
            float[] sunThetaVector = { sunThetaCubed, sunThetaSquared, sunTheta, 1.0f };

            return MulChromaticityMatrices(turbidityVector, ZC, sunThetaVector);
        }

        /// <summary>
        /// So I don't have to use TV libraries to do it with tons of ref's... D:
        /// </summary>
        static float MulChromaticityMatrices(float[] lv, float[,] mat, float[] cv)
        {
            float[] inter = new float[4];
            for (int i = 0; i < 4; i++)
            {
                inter[i] = 0f;
                for (int j = 0; j < 3; j++)
                    inter[i] += lv[j] * mat[j, i];
            }

            float ret = 0f;
            for (int i = 0; i < 4; i++)
            {
                ret += inter[i] * cv[i];
            }

            return ret;
        }
        #endregion
    }
}
