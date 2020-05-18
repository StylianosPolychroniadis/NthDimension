#version 130
precision mediump float;
in vec3 in_position;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = in_position;
  gl_Position = tmpvar_1;
}

