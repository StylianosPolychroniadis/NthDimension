#version 330 core

in vec3 Normal;

out vec3 color;

uniform vec3 LightDirection = normalize(vec3(-1.0, -1.0, -1.0));

void main()
{
	float intensity = clamp(-dot(LightDirection, normalize(Normal)), 0.0, 1.0);
	color = vec3(intensity, intensity, intensity);
}