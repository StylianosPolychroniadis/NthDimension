#version 130

precision lowp float;

uniform vec3 in_lightambient;
uniform vec3 in_lightsun;

in vec3 normal;
in vec2 v_texture;
uniform sampler2D Texture1;

out vec4 out_frag_color;

void main(void)
{
  vec4 Color = texture(Texture1, v_texture);
  //out_frag_color = Color*vec4(in_lightsun * 2,1);	// High light intensity, white in sky too bright with Bloom
  out_frag_color = Color*vec4(in_lightsun,1.0);
}