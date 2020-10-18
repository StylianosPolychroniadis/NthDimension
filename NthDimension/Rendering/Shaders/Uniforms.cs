/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using ProtoBuf;

namespace NthDimension.Rendering.Shaders
{
    // TODO:: Revise and modify as required. Include 3rd person valid projMatrix
    //[ProtoContract]
    public enum Uniform
    {

        //projection_rev_matrix,              // NOT USED
        rotation_matrix,
        rotation_matrix2,
        model_matrix2,
        model_matrix,
        modelview_matrix,
        projection_matrix,

        in_eyepos,
        in_time,
        in_pass,                            // NOT USED
        in_waterlevel,                      // NOT USED // TODO!!!:: Per Material [ Addition To Material Format ]  
        in_vector,
        in_screensize,
        in_rendersize,
        in_lightambient,                    // TODO!!!:: Per Material [ Addition To Material Format ]  
        in_lightsun,                        // TODO!!!:: Per Material [ Addition To Material Format ]  
        
        //shadow_quality,

        in_particlepos,
        in_particlesize,

        in_color,
        in_mod,

        use_emit,
        emit_a_base,
        emit_a_normal,
        in_emitcolor,

        use_spec,
        spec_a_base,
        spec_a_normal,
        in_speccolor,
        in_specexp,

        use_env,
        env_a_base,
        env_a_normal,
        env_tint,

        useTexture,
        use_alpha,
        ref_size,
        blur_size,
        fresnel_str,

        in_near,
        in_far,

        in_hudsize,
        in_hudpos,
        in_hudcolor,
        in_hudvalue,

        in_no_lights,
        curLight,
        uni_no_bones,

        defPosition,
        defDirection,
        defColor,           // TODO!!!:: Per Material [ Addition To Material Format ]  


        defMatrix,
        defInnerMatrix,
        defInvPMatrix,

        invPMatrix,
        invMMatrix,
        invMVPMatrix,

        viewUp,
        viewRight,
        viewDirection,
        viewPosition,

        fresnelExp,
        fresnelStr,

        shadowQuality,

        // ShadowMap v.2.0
        shadowView,
        shadowProj,

        // Cascaded Shadow
        splitNo,

        // Skin Color
        in_skinColor,

        terrain_minHeight,                  // float
        terrain_maxHeight,                  // float
        terrain_uvScale,                    // vec2
        terrain_lightDir                    // vec3
    }
}
