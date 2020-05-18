#version 130

#variables

#functions

uniform int curLight;

void main(void)
{
	#include vBase.snip
	
	vec4 shifted = gl_Position;
	
	if(shifted.w > 0.0){	
		shifted = shifted / shifted.w;
		shifted.x = (shifted.x+1.0+curLight*2.0)/float(in_no_lights)-1.0;
		shifted.w = 1.0;
	}
	
	
	gl_Position = shifted;
	
	//works only for orthogonal modelview
	//normal = normalize((modelview_matrix * vec4(in_normal, 0)));
}