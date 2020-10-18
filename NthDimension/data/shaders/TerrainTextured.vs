﻿#version 330 core

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;

uniform mat4 WorldTransform;
uniform mat4 ViewProjectionTransform;

out vec3 Position;
out vec3 Normal;

void main()
{
	vec4 worldPosition = WorldTransform * vec4(vertexPosition, 1.0);
	Position = worldPosition.xyz;

	gl_Position = ViewProjectionTransform * worldPosition;
	Normal = (WorldTransform * vec4(vertexNormal, 0.0)).xyz;
}