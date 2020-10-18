#version 330 core

in vec3 Position;
in vec3 Normal;

out vec3 color;

uniform vec3 LightDirection = normalize(vec3(-1.0, -1.0, -1.0));
uniform vec2 UVScale = vec2(1.0, 1.0);
uniform sampler2D grassSampler;

void main()
{
	vec2 uv = vec2(Position.x * UVScale.x, Position.z * UVScale.y);
	float intensity = clamp(-dot(LightDirection, normalize(Normal)), 0.0, 1.0);
	color = texture(grassSampler, uv).rgb * intensity;
}