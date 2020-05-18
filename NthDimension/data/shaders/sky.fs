#version 130
precision lowp float;
uniform vec3 in_lightsun;
in vec2 v_texture;
uniform sampler2D Texture1;
out vec4 out_frag_color;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = in_lightsun;
  out_frag_color = (texture (Texture1, v_texture) * tmpvar_1);
}

