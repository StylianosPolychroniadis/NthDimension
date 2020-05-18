#version 130
precision lowp float;
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;
in vec3 in_position;
in vec2 in_texture;
out vec4 g_pos;
out vec2 v_texture;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = in_position;
  g_pos = ((rotation_matrix * model_matrix) * tmpvar_1);
  gl_Position = ((projection_matrix * modelview_matrix) * g_pos);
  v_texture = in_texture;
}
