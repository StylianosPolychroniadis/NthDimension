#version 130

out float clipY;
void main () {
    float clipY = gl_Vertex.y;
    gl_Position = gl_ModelViewProjectionMatrix * vec4(gl_Vertex);
    gl_TexCoord[0] = gl_MultiTexCoord0;
}