#version 130
uniform int in_no_lights;
uniform int curLight;
uniform vec2 in_rendersize;
out vec4 out_frag_color;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = (gl_FragCoord.xy / in_rendersize);
  float tmpvar_2;
  float tmpvar_3;
  tmpvar_3 = float(in_no_lights);
  tmpvar_2 = (float(curLight) / tmpvar_3);
  float tmpvar_4;
  tmpvar_4 = (float((curLight + 1)) / tmpvar_3);
  if (((tmpvar_1.x > tmpvar_4) || (tmpvar_1.x < tmpvar_2))) {
    discard;
  } else {
    vec4 tmpvar_5;
    tmpvar_5.xyz = vec3(1.0, 1.0, 1.0);
    tmpvar_5.w = gl_FragCoord.z;
    out_frag_color = tmpvar_5;
  };
}

