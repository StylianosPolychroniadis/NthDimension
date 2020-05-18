#version 330 core
//---------IN------------
in vec3 vpoint;
in vec3 vnormal;
//---------UNIFORM------------
uniform vec3 sun_pos;//sun position in world space
uniform mat4 mvp;
uniform mat3 rot_stars;//rotation matrix for the stars
//---------OUT------------
out vec3 pos;
out vec3 sun_norm;
out vec3 star_pos;

//---------MAIN------------
void main(){
    gl_Position = mvp * vec4(vpoint, 1.0);
    pos = vpoint;

    //Sun pos being a constant vector, we can normalize it in the vshader
    //and pass it to the fshader without having to re-normalize it
    sun_norm = normalize(sun_pos);

    //And we compute an approximate star position using the special rotation matrix
    star_pos = rot_stars * normalize(pos);
}
