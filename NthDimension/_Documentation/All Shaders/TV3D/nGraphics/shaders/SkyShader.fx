// =============================================================
// Sky Hemisphere Rendering Shader
// *******************************
// Based on "A Practical Analytic Model for Daylight"
// http://www.cs.utah.edu/vissim/papers/sunsky/sunsky.pdf
// and Stephan Heigl's C++ vertex coloring implementation
// http://www.eisscholle.de/index.php?d=devel/openmountains&sub=daylight
// (full credits on the demo's TVProjects website)
// 
// Copyright (c) 2006 Renaud Bédard (Zaknafein)
// http://www.tvprojects.org/?cat=63&page=1&ex=1 
// E-mail : renaud.bedard@gmail.com
// =============================================================

// -------------------------------------------------------------
// Semantics
// -------------------------------------------------------------
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

// -------------------------------------------------------------
// Textures & Samplers
// -------------------------------------------------------------
// The stars texture, instead of another dome...!
texture texStars : TEXTURE0;
sampler sampStars = sampler_state {
	Texture = (texStars);
	MagFilter = Anisotropic;
	MinFilter = Anisotropic;
	MipFilter = Anisotropic;
};

// -------------------------------------------------------------
// Parameters
// -------------------------------------------------------------
float3 _sunVector;
float3 _zenithColor;
float _xDistribCoeffs[5], _yDistribCoeffs[5], _YDistribCoeffs[5];
float _sunTheta;
float _invGammaCorrection, _invNegMaxLum, _invPowLumFactor;
float _starsIntensity, _nightDarkness;

// -------------------------------------------------------------
// Constants
// -------------------------------------------------------------
// -- XYZ to RGB conversion matrix (rec.709 HDTV XYZ to RGB, D65 white point)
const float3x3 XYZtoRGBconv = { {  3.24079f,  -1.53715f, -0.49853f},
								{-0.969256f,  1.875991f,  0.041556f},
								{ 0.055648f, -0.204043f,  1.057311f} };
								
// -------------------------------------------------------------
// Input/Output channels
// -------------------------------------------------------------
struct VS_INPUT {
	float4 rawPos : POSITION;		// Vertex position in object space
	float2 texCoord : TEXCOORD;		// Texture coordinates
};
struct VS_OUTPUT {
	float4 homogenousPos : POSITION;	// Transformed position
	float2 texCoord : TEXCOORD0;		// Interpolated texture coordinates	
	float3 vertexColor : TEXCOORD1;		// Interpolated vertex color
};
#define	PS_INPUT VS_OUTPUT		// What comes out of VS goes into PS!

// -- Angle between two normalized, zero-centered vectors
float AngleBetween(float3 point1, float3 point2) {
	return acos(dot(point1, point2));
}

// -- Perez Function
float PerezFunction(float A, float B, float C, float D, float E, float theta, float gamma) {
	float cosGamma = cos(gamma);
	return (1.0f + A * exp(B / cos(theta))) * (1.0f + C * exp(D * gamma) + E * cosGamma * cosGamma);
}

// -- Calculate distribution
float Distribution(float coeffs[5], float theta, float gamma, float zenith) {
	float A = coeffs[0], B = coeffs[1], C = coeffs[2], D = coeffs[3], E = coeffs[4];
	return (zenith * PerezFunction(A, B, C, D, E, theta, gamma) / PerezFunction(A, B, C, D, E, 0.0f, _sunTheta));
}

// -- Convert directly from xyY to RGB
float3 xyYtoRGB(float3 xyY) {
	float Yony = xyY.z / xyY.y;
	float3 XYZ = float3(xyY.x * Yony,
						xyY.z, 
						(1.0f - xyY.x - xyY.y) * Yony);
						
	return mul(XYZtoRGBconv, XYZ);
}

// -- Calculate color based on the sun normalized position vector
float3 CalculateColor(float3 vertexVector) {
	float gamma = AngleBetween(vertexVector, _sunVector);
	float theta = AngleBetween(float3(0.0f, 1.0f, 0.0f), vertexVector);

	float3 skyColor;	// xyY format
	
	// Sky color distribution (using the Perez Function)
	skyColor[0] = Distribution(_xDistribCoeffs, theta, gamma, _zenithColor[0]);
	skyColor[1] = Distribution(_yDistribCoeffs, theta, gamma, _zenithColor[1]);
	skyColor[2] = Distribution(_YDistribCoeffs, theta, gamma, _zenithColor[2]);

	// Expononentially scale the luminosity
	skyColor[2] = pow(1.0f - exp(_invNegMaxLum * skyColor[2]), _invPowLumFactor);

	// Convert to RGB and return
	return xyYtoRGB(skyColor);
}

// -------------------------------------------------------------
// Vertex Shader function
// -------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT IN) {
	VS_OUTPUT OUT;
    
    // Basic transformation of vertex into clip-space
    OUT.homogenousPos = mul(IN.rawPos, matWorldViewProj);
    
    // Texture coordinate, straight out
    OUT.texCoord = IN.texCoord;
        
    // Calculate the color for this vertex
	OUT.vertexColor = CalculateColor(normalize(IN.rawPos));
	
	// Gamma correction
	OUT.vertexColor = pow(OUT.vertexColor, _invGammaCorrection);
	
	// Correct it for the night... kind of a hack, but meh.
	OUT.vertexColor *= _nightDarkness;
		
	return OUT;
}

// -------------------------------------------------------------
// Pixel Shader function
// -------------------------------------------------------------
float4 PS(PS_INPUT IN) : COLOR {
	float starsAlpha = tex2D(sampStars, IN.texCoord).a;
	return float4(IN.vertexColor.rgb + starsAlpha * _starsIntensity, 1.0f);
}

// -------------------------------------------------------------
// Technique
// -------------------------------------------------------------
technique TShader {
    pass P0 {
        // Compile Shaders
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}