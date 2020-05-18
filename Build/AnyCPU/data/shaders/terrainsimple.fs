#version 130

const float waterHeight = 0.5;

uniform vec3 fogColor;
uniform sampler2D textre;
uniform vec3 eyePos;
uniform int clip;
in float clipY;

out vec4 out_frag_color;

void main() {
    //if ((clip < 0 && clipY < waterHeight) ||
    //    (clip > 0 && clipY > waterHeight + 5.0)) {
    //    discard;
    //} else {
        float density = 1.0;
        float z = gl_FragCoord.z / gl_FragCoord.w / 5000.0;
        float fog = exp2(-density * density * z * z * 1.442695);

        vec4 tex = texture(textre, gl_TexCoord[0].st);
        out_frag_color = vec4(mix(fogColor, tex.rgb, clamp(fog, 0.0, 1.0)), 1.0);
    //}
}