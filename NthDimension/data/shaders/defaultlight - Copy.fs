#version 130

#variables

#functions

in vec3 viewDir;

uniform float fresnelExp;
uniform float fresnelStr;


vec4 getback(vec2 coord,float blur_size) {
	float PI = 3.14159265;
	int samples = 5; 	//samples on the first ring (5-10)
	//int rings = 2; 		//ring count (2-6) // original value
	int rings = 6;
	
	int i;
	vec4 base_color = vec4(0.5,0.5,0.5,1.0);
	float stepp = PI*2.0 / float(samples);
	float pw;
	float ph;
	float ringcoord;
	vec2 size = blur_size/in_rendersize/rings;
	vec4 col;	
	
	for (int i = 1 ; i <= rings; ++i)
	{
		for (int j = 0 ; j < samples; ++j)
		{	
			ringcoord = float(j)*stepp;
			pw = cos(ringcoord)*float(i);
			ph = sin(ringcoord)*float(i);
			col += texture(backColorTexture, vec2(coord.s+pw*size.s,coord.t+ph*size.t) );
		}
	}
	
	return col/float(samples)/float(rings);
}

void main(void)
{
	#include base.snip
	
	//#include defReflections.snip
		
	vec3 emit = vec3(0.0,0.0,0.0);
	if(use_emit){
		emit = texture (emitTexture, v_texture).rgb;//in_emitcolor;
		emit.rgb *= base.rgb;
        emit.rgb *= in_emitcolor;
	}
	
	#include defLighting.snip
	
	float fresnel = 1.0;
	if(fresnelExp > 0.0)
	{
		fresnel = clamp(dot(viewDir,v_normal)+1.0,0.0,1.0);
		fresnel = pow(fresnel,fresnelExp);
		fresnel *= fresnelStr;
		fresnel += 1.0;
	}
	
	float lightBrightness =  0.3 / (all_lights.r + all_lights.g + all_lights.b);
	vec3 env1 = vec3(env.x * 0.3, env.y*0.3, env.z*0.3);
    
    if(use_emit){
	out_frag_color.rgb = all_lights*base.rgb+all_spec*all_lights*lightBrightness*fresnel+emit;
    } else {
        out_frag_color.rgb = all_lights*base.rgb+all_spec*all_lights*lightBrightness*fresnel;
    };
	out_frag_color.a = 1.0;
}