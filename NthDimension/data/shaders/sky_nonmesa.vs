#version 130

precision lowp float;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;

uniform vec3 in_light;

in vec3 in_normal;
in vec3 in_position;
in vec2 in_texture;
in vec3 in_tangent;

out vec4 g_pos;
out vec2 v_texture;

void main(void)
{
  //works only for orthogonal modelview
  //normal = normalize((modelview_matrix * vec4(in_normal, 0)).xyz);

	g_pos = rotation_matrix * model_matrix * vec4(in_position, 1.0);
	gl_Position = projection_matrix * modelview_matrix * g_pos;
  
	v_texture = in_texture;
}