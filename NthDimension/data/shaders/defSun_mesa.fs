#version 130
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;
uniform sampler2D Texture4;
uniform sampler2D Texture5;
uniform vec2 in_rendersize;
uniform float shadowQuality;
uniform vec3 defDirection;
uniform vec3 defColor;
uniform mat4 defMatrix;
uniform mat4 defInnerMatrix;
uniform vec3 in_lightambient;
uniform vec3 viewUp;
uniform vec3 viewRight;
uniform vec3 viewDirection;
uniform vec3 viewPosition;
uniform float in_near = 0.1;
uniform float in_far = 4000.0;
out vec4 out_frag_color;
void main ()
{
  float shadow_1;
  float spec_2;
  float depth_3;
  vec2 tmpvar_4;
  tmpvar_4 = (gl_FragCoord.xy / in_rendersize);
  depth_3 = (texture (Texture5, tmpvar_4).x + 0.001);
  depth_3 = (in_near / ((in_far + in_near) - (depth_3 * 
    (in_far - in_near)
  )));
  depth_3 = (depth_3 - 0.001);
  vec2 tmpvar_5;
  tmpvar_5 = (tmpvar_4 - 0.5);
  vec3 tmpvar_6;
  tmpvar_6 = (depth_3 * ((viewDirection + 
    (viewUp * tmpvar_5.y)
  ) + (viewRight * tmpvar_5.x)));
  vec3 tmpvar_7;
  tmpvar_7 = normalize(tmpvar_6);
  vec4 tmpvar_8;
  tmpvar_8 = texture (Texture1, tmpvar_4);
  vec3 tmpvar_9;
  tmpvar_9 = ((tmpvar_8.xyz * 2.0) - 1.0);
  if (((depth_3 == 1.0) || (tmpvar_8.w == 0.0))) {
    discard;
  };
  float tmpvar_10;
  vec3 tmpvar_11;
  tmpvar_11 = -(defDirection);
  tmpvar_10 = clamp (dot (tmpvar_11, tmpvar_9), 0.0, 1.0);
  spec_2 = pow (clamp (dot (tmpvar_11, 
    (tmpvar_7 - (2.0 * (dot (tmpvar_9, tmpvar_7) * tmpvar_9)))
  ), 0.0, 1.0), ((tmpvar_8.w * tmpvar_8.w) * 250.0));
  vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = (viewPosition + tmpvar_6);
  vec2 rnd_13;
  rnd_13 = ((texture (Texture4, (gl_FragCoord.xy / 128.0)).xy * 2.0) - 1.0);
  float bias_14;
  bias_14 = (0.001 / clamp (tmpvar_10, 0.01, 1.0));
  float tmpvar_15;
  vec4 shadowCoordinateWdivide_16;
  vec4 v_sun_map_pos_17;
  vec4 tmpvar_18;
  tmpvar_18 = (defInnerMatrix * tmpvar_12);
  v_sun_map_pos_17 = tmpvar_18;
  vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_18 / tmpvar_18.w);
  shadowCoordinateWdivide_16 = tmpvar_19;
  if ((((
    (((0.0 < tmpvar_19.x) && (1.0 > tmpvar_19.x)) && (0.0 < tmpvar_19.y))
   && 
    (1.0 > tmpvar_19.y)
  ) && (0.0 < tmpvar_19.z)) && (1.0 > tmpvar_19.z))) {
    shadowCoordinateWdivide_16.xy = (tmpvar_19.xy + (0.00075 * rnd_13));
    vec3 coords_20;
    coords_20 = shadowCoordinateWdivide_16.xyz;
    float bias_21;
    bias_21 = bias_14;
    float shadow_24;
    shadow_24 = 0.0;
    for (float y_22 = -(shadowQuality); y_22 <= shadowQuality; y_22 += 1.0) {
      for (float x_23 = -(shadowQuality); x_23 <= shadowQuality; x_23 += 1.0) {
        vec2 tmpvar_25;
        tmpvar_25.x = (x_23 * 0.0015);
        tmpvar_25.y = (y_22 * 0.0015);
        float tmpvar_26;
        tmpvar_26 = (texture (Texture3, (coords_20.xy + tmpvar_25)).x + bias_21);
        float tmpvar_27;
        if ((tmpvar_26 < coords_20.z)) {
          tmpvar_27 = 0.0;
        } else {
          tmpvar_27 = 1.0;
        };
        shadow_24 = (shadow_24 + tmpvar_27);
      };
    };
    float tmpvar_28;
    tmpvar_28 = ((shadowQuality * 2.0) + 1.0);
    shadow_24 = (shadow_24 / (tmpvar_28 * tmpvar_28));
    tmpvar_15 = shadow_24;
  } else {
    v_sun_map_pos_17 = (defMatrix * tmpvar_12);
    shadowCoordinateWdivide_16 = (v_sun_map_pos_17 / v_sun_map_pos_17.w);
    if ((((
      (((0.0 < shadowCoordinateWdivide_16.x) && (1.0 > shadowCoordinateWdivide_16.x)) && (0.0 < shadowCoordinateWdivide_16.y))
     && 
      (1.0 > shadowCoordinateWdivide_16.y)
    ) && (0.0 < shadowCoordinateWdivide_16.z)) && (1.0 > shadowCoordinateWdivide_16.z))) {
      shadowCoordinateWdivide_16.xy = (shadowCoordinateWdivide_16.xy + (0.00075 * rnd_13));
      vec3 coords_29;
      coords_29 = shadowCoordinateWdivide_16.xyz;
      float bias_30;
      bias_30 = (bias_14 * 2.0);
      float shadow_33;
      shadow_33 = 0.0;
      for (float y_31 = -(shadowQuality); y_31 <= shadowQuality; y_31 += 1.0) {
        for (float x_32 = -(shadowQuality); x_32 <= shadowQuality; x_32 += 1.0) {
          vec2 tmpvar_34;
          tmpvar_34.x = (x_32 * 0.0015);
          tmpvar_34.y = (y_31 * 0.0015);
          float tmpvar_35;
          tmpvar_35 = (texture (Texture2, (coords_29.xy + tmpvar_34)).x + bias_30);
          float tmpvar_36;
          if ((tmpvar_35 < coords_29.z)) {
            tmpvar_36 = 0.0;
          } else {
            tmpvar_36 = 1.0;
          };
          shadow_33 = (shadow_33 + tmpvar_36);
        };
      };
      float tmpvar_37;
      tmpvar_37 = ((shadowQuality * 2.0) + 1.0);
      shadow_33 = (shadow_33 / (tmpvar_37 * tmpvar_37));
      tmpvar_15 = shadow_33;
    } else {
      tmpvar_15 = 1.0;
    };
  };
  shadow_1 = (tmpvar_15 * tmpvar_10);
  out_frag_color.xyz = (in_lightambient + (defColor * shadow_1));
  out_frag_color.w = (spec_2 * shadow_1);
}

