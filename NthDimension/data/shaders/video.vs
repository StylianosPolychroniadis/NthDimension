#version 130
precision lowp float;

in vec3 in_position;
in vec2 in_texture;
in vec3 in_normal;
in vec3 in_tangent;

uniform vec3 in_eyepos;
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;

out float v_depth;

out vec2 v_texture;

out vec3 v_eyedirection;
out vec3 v_normal;
out vec3 v_tangent;
out vec3 v_bnormal;

out vec4 g_pos;

void main(void) {
    //v_texture = in_texture;
    //gl_Position = vec4(in_position, 1.0);
	
	v_texture = in_texture;
    g_pos = model_matrix * rotation_matrix * vec4(in_position, 1.0);
	
	gl_Position = projection_matrix * modelview_matrix * g_pos;
	
	v_eyedirection = normalize(g_pos.xyz - in_eyepos);
	
	v_normal = normalize((rotation_matrix * vec4(in_normal, 0.0)).xyz);
	v_tangent = normalize((rotation_matrix * vec4(in_tangent, 0.0)).xyz);
	v_bnormal = normalize(cross(v_normal, v_tangent));
}