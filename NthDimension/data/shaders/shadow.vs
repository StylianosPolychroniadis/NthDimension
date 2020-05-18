#version 130
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;
uniform int in_no_lights;
in vec3 in_normal;
in vec3 in_position;
in vec2 in_texture;
in vec3 in_tangent;
out vec4 g_pos;
out vec3 v_normal;
out vec2 v_texture;
out vec3 v_tangent;
out vec3 v_bnormal;
out vec3 vertexPos;
uniform int curLight;
void main ()
{
  vec4 shifted_1;
  vertexPos = in_position;
  vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = in_position;
  g_pos = ((model_matrix * rotation_matrix) * tmpvar_2);
  gl_Position = ((projection_matrix * modelview_matrix) * g_pos);
  vec4 tmpvar_3;
  tmpvar_3.w = 0.0;
  tmpvar_3.xyz = in_normal;
  vec3 tmpvar_4;
  tmpvar_4 = normalize((rotation_matrix * tmpvar_3).xyz);
  v_normal = tmpvar_4;
  vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = in_tangent;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((rotation_matrix * tmpvar_5).xyz);
  v_tangent = tmpvar_6;
  v_bnormal = normalize(((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)));
  v_texture = in_texture;
  shifted_1 = gl_Position;
  if ((gl_Position.w > 0.0)) {
    shifted_1.xyz = (gl_Position.xyz / gl_Position.w);
    shifted_1.x = (((
      (shifted_1.x + 1.0)
     + 
      float((curLight * 2))
    ) / float(in_no_lights)) - 1.0);
    shifted_1.w = 1.0;
  };
  gl_Position = shifted_1;
}

