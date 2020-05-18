#version 130
struct Light {
  bool actve;
  bool textre;
  vec3 position;
  vec3 direction;
  vec3 color;
  mat4 view_matrix;
};
struct SunLight {
  bool actve;
  vec3 direction;
  vec3 color;
  mat4 view_matrix;
  mat4 inner_view_matrix;
};
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;
uniform vec3 in_eyepos;
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
out vec4 v_sun_map_pos;
out vec4 v_inner_sun_map_pos;
uniform Light lightStructs[10];
uniform SunLight sunLightStruct;
out vec4 v_s_map_pos[10];
out vec3 viewDir;
void main ()
{
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
  for (int i_1 = 0; i_1 < 10; i_1++) {
    if (((i_1 < in_no_lights) && lightStructs[i_1].actve)) {
      v_s_map_pos[i_1] = (lightStructs[i_1].view_matrix * g_pos);
    };
  };
  v_sun_map_pos = (sunLightStruct.view_matrix * g_pos);
  v_inner_sun_map_pos = (sunLightStruct.inner_view_matrix * g_pos);
  viewDir = normalize((g_pos.xyz - in_eyepos));
}

