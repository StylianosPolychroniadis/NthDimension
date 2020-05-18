// =============================================================
// Dual-Layer Clouds Shader 
// ************************
// Copyright (c) 2006 Renaud Bédard (Zaknafein)
// http://www.tvprojects.org/?cat=63&page=1&ex=1 
// E-mail : renaud.bedard@gmail.com
// =============================================================

// -------------------------------------------------------------
// Compilation Switches
// -------------------------------------------------------------
// Trades the costly normalize() instrinsic for a cubemap lookup
#define NORMALIZE_WITH_CUBEMAP

// -------------------------------------------------------------
// Semantics
// -------------------------------------------------------------
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

// -------------------------------------------------------------
// Textures & Samplers
// -------------------------------------------------------------
// -- The two cloud textures layers
texture texLayer0 : TEXTURE0;
sampler sampLayer0 = sampler_state {
	Texture = (texLayer0);
	MagFilter = Anisotropic;
	MinFilter = Anisotropic;
	MipFilter = Anisotropic;
};
texture texLayer1 : TEXTURE1;
sampler sampLayer1 = sampler_state {
	Texture = (texLayer1);
	MagFilter = Anisotropic;
	MinFilter = Anisotropic;
	MipFilter = Anisotropic;
};

// -- Cube normalization map, if enabled
#ifdef NORMALIZE_WITH_CUBEMAP
	textureCUBE texCubeNormalizer;
	samplerCUBE sampCubeNormalizer = sampler_state {
		Texture = (texCubeNormalizer);
		MagFilter = Linear;
		MinFilter = Linear;
		MipFilter = None;
  		AddressU = Clamp;
  		AddressV = Clamp;
  		AddressW = Clamp;
	};	
#endif

// -------------------------------------------------------------
// Parameters
// -------------------------------------------------------------
float2 _cloudsSize, _layersOpacity;
float2 _cloudsTranslation[2];
float3 _cloudsColor;

// -------------------------------------------------------------
// Input/Output channels
// -------------------------------------------------------------
struct VS_INPUT {
	float4 rawPos : POSITION;		// Vertex position in object space
	float2 texCoord : TEXCOORD;		// Texture coordinates
	float3 normal : NORMAL;			// Vertex normal
};
struct VS_OUTPUT {
	float4 homogenousPos : POSITION;	// Transformed position
	float2 tcLayer[2] : TEXCOORD0;		// Texture coordinates for the two layers
	float3 normal : TEXCOORD3;			// Interpolated normal
};
#define	PS_INPUT VS_OUTPUT		// What comes out of VS goes into PS!

// -------------------------------------------------------------
// Vertex Shader function
// -------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT IN) {
	VS_OUTPUT OUT;
    
    // Basic transformation of vertex into clip-space
    OUT.homogenousPos = mul(IN.rawPos, matWorldViewProj);
    
    // Texture coordinates for both layers
    for(int i=0; i<2; i++)
		OUT.tcLayer[i] = IN.texCoord * _cloudsSize[i] + _cloudsTranslation[i];
		
	// Opacity factor to prevent ugly smuches on the edges
	OUT.normal = -IN.normal;
        	
	return OUT;
}

// -------------------------------------------------------------
// Switch-enabled normalization
// -------------------------------------------------------------
float3 normalizeVector(float3 vec) {
	#ifdef NORMALIZE_WITH_CUBEMAP
		float3 retVec = texCUBE(sampCubeNormalizer, vec).rgb;
		retVec = retVec * 2.0f - 1.0f;
		return retVec;
	#else
		return normalize(vec);
	#endif
}

// -------------------------------------------------------------
// Pixel Shader function
// -------------------------------------------------------------
float4 PS(PS_INPUT IN) : COLOR {
	float layerAlpha[2];
	layerAlpha[0] = tex2D(sampLayer0, IN.tcLayer[0]).a * _layersOpacity[0];
	layerAlpha[1] = tex2D(sampLayer1, IN.tcLayer[1]).a * _layersOpacity[1];
	float transparencyFactor = saturate(normalizeVector(IN.normal).y * 1.5f);
	
	return float4(_cloudsColor, saturate((layerAlpha[0] + layerAlpha[1]) * transparencyFactor));
}

// -------------------------------------------------------------
// Technique
// -------------------------------------------------------------
technique TShader {
    pass P0 {
		// States
		AlphablendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		AlphaTestEnable = false;
    
        // Compile Shaders
		VertexShader = compile vs_2_0 VS();
		PixelShader  = compile ps_2_0 PS();		
    }
}