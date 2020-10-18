#version 130

#variables
#functions

out vec3 vLightDir;

void main()
{
	#include vBase.snip  	
	#include vLightCalc.snip

	vLightDir = sunLightStruct.direction;
}