#version 130 //330 core
//---------IN------------
in vec3 in_position;
in vec3 in_normal;
//---------UNIFORM------------
uniform vec3 sky_sunp;//sun position in world space
uniform mat3 sky_stars;//rotation matrix for the stars
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

//---------OUT------------
out vec3 g_pos;
out vec3 sun_norm;
out vec3 star_pos;

//---------MAIN------------
void main(){
    mat4 mvp;
    mvp = projection_matrix * modelview_matrix;
    gl_Position = mvp * vec4(in_position, 1.0);
    g_pos = in_position;

    //Sun pos being a constant vector, we can normalize it in the vshader
    //and pass it to the fshader without having to re-normalize it
    sun_norm = normalize(sky_sunp);

    //And we compute an approximate star position using the special rotation matrix
    star_pos = sky_stars * normalize(g_pos);
}
