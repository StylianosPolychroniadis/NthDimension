#version 130
precision lowp float;
uniform sampler2D Texture1;
uniform sampler2D Texture2;

in vec2 v_texture;

out vec4 out_frag_color;
uniform vec2 in_screensize;

uniform float totStrength = 2.0;
uniform float rad = 2.0;
uniform float falloff = 0.2;
uniform float strength = 1.5;
uniform float totHighlightStr = 0.5;

uniform float in_time;

uniform float in_near = 0.1; //Z-in_near
uniform float in_far = 100; //100.0; //Z-in_far

vec4 readSample(in vec2 coord)
{
	vec4 sample = texture(Texture1,coord);
	vec3 normal = sample.xyz * 2.0 - 1.0;
	normal.z *= -1.0;

	float depth = sample.a * in_far;
	//float depth = (2.0 * in_near) / (in_far + in_near - sample.a * (in_far-in_near)) * in_far;
	return vec4(normal,depth);
}

void main() {
	vec3 pSphere[16] = vec3[](vec3(0.53812504, 0.18565957, -0.43192),vec3(0.13790712, 0.24864247, 0.44301823),vec3(0.33715037, 0.56794053, -0.005789503),vec3(-0.6999805, -0.04511441, -0.0019965635),vec3(0.06896307, -0.15983082, -0.85477847),vec3(0.056099437, 0.006954967, -0.1843352),vec3(-0.014653638, 0.14027752, 0.0762037),vec3(0.010019933, -0.1924225, -0.034443386),vec3(-0.35775623, -0.5301969, -0.43581226),vec3(-0.3169221, 0.106360726, 0.015860917),vec3(0.010350345, -0.58698344, 0.0046293875),vec3(-0.08972908, -0.49408212, 0.3287904),vec3(0.7119986, -0.0154690035, -0.09183723),vec3(-0.053382345, 0.059675813, -0.5411899),vec3(0.035267662, -0.063188605, 0.54602677),vec3(-0.47761092, 0.2847911, -0.0271716));
	int samples = 8; //samples
	
	float ratio = in_screensize.x/in_screensize.y;

	// these are the random vectors inside a unit sphere 
	vec2 dynamicPos = gl_FragCoord.xy/128+in_time*vec2(7.0,7.0);
   
	// grab a normal for reflecting(randomizing) the sample rays later on
	vec3 fres = normalize((texture(Texture2,dynamicPos).rgb * 2.0 - 1.0)*vec3(0.5,0.5,1.0));
	
	//grab curent pixel and convert to screenspace
	vec4 curpixel = readSample(v_texture);	
	vec3 screenspace_coords = vec3((v_texture * 2 -1)*curpixel.a,curpixel.a);
	screenspace_coords.x *= ratio;
				
	vec2 samplepos, texture_coords;
	vec3 occnorm, ray, randvec, ss_sample_coords, raydir;
	float normdiff, occdepth, depthdifference, dist, fallof;
    float bl = 0.0;
	float highlight = 0.0;
	
	if(curpixel.a > 99.0){
		out_frag_color = vec4(1.0,0.0,0.0,0.5);
	} else {
		for (int i = 0 ; i < samples; ++i)
		{	
			ray = reflect(pSphere[i],fres);

			
			// if the ray is outside the hemisphere then change direction
			ray = sign(dot(ray,curpixel.xyz) )*ray;
			//ray = ray*length(ray);
			
			//get screenspace coords /convert them
			ss_sample_coords = screenspace_coords + ray * rad;
			texture_coords = ss_sample_coords.xy;
			texture_coords.x /= ratio; 
			texture_coords = texture_coords/ss_sample_coords.z * 0.5 + 0.5;
									
			// get the depth of the sample fragment get differece to sample
			vec4 sample = readSample(texture_coords);
			
			if(sample.a < 99.0)
			{
				// calculate the difference between the normals as a weight
				normdiff = (1-dot(sample.xyz,curpixel.xyz));
				
				if(normdiff > 0.03)
				{
					depthdifference = ss_sample_coords.z-sample.a;

					fallof = (1.0-smoothstep(falloff,strength,depthdifference));
					
					// calculate the cavity for highlights
					highlight += dot(-ray, sample.xyz)*fallof;
					
					// the falloff equation, starts at falloff and is kind of 1/x^2 falling
					bl += step(falloff,depthdifference)*fallof;
				}	
			}
		}
		
		// output the result
		float ao = (totStrength*bl+highlight*totHighlightStr)/samples;
		//ao = highlight*totHighlightStr/samples;
	   
		curpixel.z *= -1.0;
		out_frag_color = vec4(curpixel.xyz * 0.5 +0.5,clamp(0.5-ao,0.01,1.0));
		//out_frag_color = curpixel;
	}
}