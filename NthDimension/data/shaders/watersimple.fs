//waterShader.SetUniform("diffuseMap", 0);
//waterShader.SetUniform("normalMap", 1);
//waterShader.SetUniform("refractionMap", 2);
//waterShader.SetUniform("lightPos", light.Position);
//waterShader.SetUniform("fogColor", fogColor);

// As found in GLTerrain

#version 130

precision lowp float;

uniform sampler2D diffuseMap, normalMap, refractionMap;
uniform vec3 lightPos, eyePos, fogColor;
uniform float offset;

in vec3 eyeVector;

void main() {
    vec3 normal = texture(normalMap, -gl_TexCoord[1].st).rbg * 2.0 - 1.0;
    normal += texture(normalMap, vec2(gl_TexCoord[0].s + 0.5, gl_TexCoord[0].t)).rbg * 2.0 - 1.0;
    normal = normalize(normal);

    vec3 eyeVec = normalize(eyeVector);
    vec3 reflected = normalize(-reflect(normalize(lightPos), normal));
    float fresnel = 1.0 - dot(eyeVec, vec3(0.0, 1.0, 0.0));

    vec2 distortion = normal.xz * vec2(10.0);
    vec4 projVertex = vec4(gl_TexCoord[2].x + distortion.x, gl_TexCoord[2].y + distortion.y, gl_TexCoord[2].z, gl_TexCoord[2].w);
    vec2 projTexCoord = projVertex.xy / projVertex.z * 0.5 + 0.5;

    vec4 reflectColor = texture(diffuseMap, projTexCoord);
    vec4 refractColor = texture(refractionMap, projTexCoord);

    float spec = max(dot(eyeVec, reflected), 0.0);

    vec4 color = mix(refractColor, reflectColor, fresnel);
    color *= vec4(0.8, 0.8, 1.0, 1.0);
    color += pow(spec, 128.0);
    //color += pow(reflectColor, vec4(32.0));

    const float density = 1.0;
    float z = gl_FragCoord.z / gl_FragCoord.w / 5000.0;
    float fog = exp2(-density * density * z * z * 1.442695);

    gl_FragColor = vec4(mix(fogColor, color.rgb, clamp(fog, 0.0, 1.0)), 1.0);
}