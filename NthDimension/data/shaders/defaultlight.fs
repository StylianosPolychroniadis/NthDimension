#version 130
in vec3 v_normal;
in vec2 v_texture;
uniform sampler2D baseTexture;
uniform sampler2D definfoTexture;
uniform sampler2D emitTexture;
out vec4 out_frag_color;
uniform float in_alpha;
uniform bool use_emit;
uniform vec3 in_emitcolor;
uniform vec2 in_rendersize;
uniform sampler2D lightMap;
in vec3 viewDir;
uniform float fresnelExp;
uniform float fresnelStr;
void main ()
{
  float fresnel_1;
  vec3 emit_2;
  vec2 tmpvar_3;
  tmpvar_3 = (gl_FragCoord.xy / in_rendersize);
  vec4 tmpvar_4;
  tmpvar_4 = texture (baseTexture, v_texture);
  vec4 tmpvar_5;
  tmpvar_5 = texture (definfoTexture, v_texture);
  emit_2 = vec3(0.0, 0.0, 0.0);
  if (use_emit) {
    emit_2 = texture (emitTexture, v_texture).rgb;
	emit_2.rgb *= tmpvar_4.rgb;
	emit_2.rgb *= in_emitcolor;     // Warning: This requires ALWAYS a tint value in the material definition
  };
  vec4 tmpvar_6;
  tmpvar_6 = texture (lightMap, tmpvar_3);
  vec3 tmpvar_7;
  tmpvar_7 = ((tmpvar_6.xyz * tmpvar_6.w) * ((tmpvar_5.x * tmpvar_5.x) * 10.0));
  fresnel_1 = 1.0;
  if ((fresnelExp > 0.0)) {
    fresnel_1 = (pow (clamp (
      (dot (viewDir, v_normal) + 1.0)
    , 0.0, 1.0), fresnelExp) * fresnelStr);
    fresnel_1 += 1.0;
  };
  if(use_emit){
    out_frag_color.xyz = (((tmpvar_6.xyz * tmpvar_4.xyz) + (
    ((tmpvar_7 * tmpvar_6.xyz) * (0.3 / ((tmpvar_6.x + tmpvar_6.y) + tmpvar_6.z))) * fresnel_1)) + emit_2);
  } else {
     out_frag_color.xyz = (((tmpvar_6.xyz * tmpvar_4.xyz) + (
    ((tmpvar_7 * tmpvar_6.xyz) * (0.3 / ((tmpvar_6.x + tmpvar_6.y) + tmpvar_6.z))) * fresnel_1)));
  };
  float alpha;
  alpha = in_alpha;
  out_frag_color.w = alpha; //1
}