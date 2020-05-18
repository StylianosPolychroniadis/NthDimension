#version 130
precision lowp float;

out vec4 g_pos;


void main() 
{	
	//gl_TexCoord[0] = gl_MultiTexCoord0;
	//gl_Position = invariant(); // TODO:: invariants is declaration for variables, not a func but means keep the same

	g_pos = gl_Position;
	gl_TexCoord[0] = gl_MultiTexCoord0;
} 
