#version 130

#variables

in vec3 vLightDir;
in vec3 vertexNormal;

uniform float terrain_minHeight;
uniform float terrain_maxHeight;
uniform vec2  terrain_uvScale	= vec2(0.01, 0.01);		// TODO:: clear instanciation
uniform vec3  terrain_lightDir	= vec3(-1, -1, -1);		// TODO:: clear instantiation

//baseTexture  -	grass
//baseTexture2 -	snow
//baseTexture3 -	dirt
//baseTexture4 -	rock

#functions

void main()
{	
	#include base.snip

	vec3 t_pos		= g_pos.xyz;	
	vec3 color		= vec3(0,1,0);
	vec3 tNormal	= vertexNormal;
	vec2 uv			= vec2(t_pos.x * terrain_uvScale.x, t_pos.z * terrain_uvScale.y);

	float intensity = clamp(-dot(terrain_lightDir, tNormal), 0.0, 1.0);	// v_normal is already normalized
		
	vec3 grassSlope = texture(baseTexture, uv).rgb;
	vec3 snowSlope = texture(baseTextureTwo, uv).rgb;
	vec3 dirtSlope = texture(baseTextureThree, uv).rgb;
	vec3 rockSlope = texture(baseTextureFour, uv).rgb;

	vec3 newNormal = normalize(tNormal);
	float slope = 1.0 - newNormal.y;
	//normalize t_poss to [0, 1]
	float posFactor = (t_pos.y - terrain_minHeight) / (terrain_maxHeight - terrain_minHeight);

	//flat surfaces are dirt-like, more rock-like as slope increases (slope factor)
	vec3 slopeColor = slope * rockSlope + (1 - slope) * dirtSlope;
	
	//grass near on bottom, snow near top (t_pos factor)
	vec3 locColor = posFactor * snowSlope + (1 - posFactor) * grassSlope;

	//quadratic bias towards extremes (top and bottom have more weight)
	//that is, tops/bottoms rely more heavily on t_pos weight than slope weight
	float extremeX = abs(.5 - posFactor) * 2;
	float extremeFactor = (extremeX + 1) * (extremeX - 1) + 1;
	color = extremeFactor * locColor + (1 - extremeFactor) * slopeColor;
	
	//low flat areas have grass
	if (slope < .3 && posFactor < .4){
		//normalize
		float slopeFactor = abs(slope - .3) / .3;
		color = slopeFactor * grassSlope + (1 - slopeFactor) * color;
	}

	//high flat areas have snow
	else if (slope < .3 && posFactor >= .8){
		//normalize
		float slopeFactor = abs(slope - .3) / .3;
		color = slopeFactor * snowSlope + (1 - slopeFactor) * color;
	}

	//middle ground
	else if (slope < .3){
		//normalize
		float slopeFactor = abs(slope - .3) / .3;
		float posFactor2 = (posFactor - .4)/ .4;

		color = slopeFactor * (posFactor2 * snowSlope + (1-posFactor2) * grassSlope) + (1 - slopeFactor) * color;		
	}
	else
	{
		color = vec3(0,1,0);
		intensity = 1;
	}

	color = color * intensity;	// factor in phong	
	out_frag_color = vec4(color, 0);
}