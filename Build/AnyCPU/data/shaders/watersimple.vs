// As found in GLTerrain

#version 130

precision lowp float;

out vec3 eyeVector;

uniform vec3 lightPos, eyePos;
uniform float offset;

void main () {
    eyeVector = eyePos - vec3(gl_Vertex);
    vec4 vertex = gl_ModelViewProjectionMatrix * gl_Vertex;

    gl_Position = vertex;
    gl_TexCoord[0] = vec4(gl_MultiTexCoord0.x + offset, gl_MultiTexCoord0.y, gl_MultiTexCoord0.zw);
    gl_TexCoord[1] = vec4(gl_MultiTexCoord0.x + offset, gl_MultiTexCoord0.y + offset, gl_MultiTexCoord0.zw);
    gl_TexCoord[2] = vertex;
}