// SYSCON Technologies Nth-Dimension Platform
// GLSL Fragment Shader
// Name: glass saturation splash fragment shader
// Date:
// Rev.:
// Rev. 1.2 - Fixed shader by modifying i = 1 to i = 0 at for loop in main

#version 130
precision lowp float;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform vec2 in_vector;
in vec2 v_texture;

out vec4 out_frag_color;

vec4 sample(vec2 coo, float perc)
{
	vec4 color = texture(Texture1, coo);
	return color;	
}

vec2 scale(vec2 coo, float factor)
{
	return (coo - vec2(0.5,0.5)) * factor + vec2(0.5,0.5);
}

void main() {
	out_frag_color = texture(Texture1, v_texture);
}
