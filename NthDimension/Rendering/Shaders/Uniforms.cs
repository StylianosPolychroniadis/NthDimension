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
    public enum Uniform
    {
        //  3 x [CONFLICTS]
        // 10 x [TODO]
        //  5 x [TBC]
        //  2 x [TBD]
        //  3 x [NOT USED]

        model_matrix,                       // The Model space matrix (local rotation * transformation * scale)
        model_matrix2,                      // Auxiliary Model space matrix                                                 // [TODO: Eliminate]
        modelview_matrix,                   // The ModelView matrix
        projection_matrix,                  // The Modelviewprojection matrix        
        invPMatrix,                         // Inverse Projection matrix
        invMMatrix,                         // Inverse Model matrix
        invMVPMatrix,                       // Inverse ModelViewProjection 

        rotation_matrix,                    // The rotation matrix in model space (local)
        rotation_matrix2,                   // Auxiliary rotation matrix                                                    // [TBC: Still used? Eliminate?]                

        viewUp,                             // View vector pointing Up
        viewRight,                          // View vector pointing Right

        in_near,                            // Frustum near view
        in_far,                             // Frustum far view

        viewDirection,                      // View Direction
        viewPosition,                       // View Position                                                                // [CONFLICTS in_eyepos]
        in_eyepos,                          // The Eye (camera) position                                                    // [CONFLICTS viewPosition]
                
        in_screensize,                      // The virtual screen size
        in_rendersize,                      // The screen size
        //shadow_quality,                   // disabled                                                                     // [TBC Disabled?]
        shadowQuality,                      // Shadowmap resolution factor                                                  // [TODO: Refactor in_shadowQuality]
        in_time,                            // Frame Time
        in_pass,                            // Render pass index                                                            // [NOT USED TBC]
        in_lightambient,                    // Ambient light color                                                          // TODO:: Update according to weather model
        in_lightsun,                        // Sun light color                                                              // TODO:: Update according to weather model      
        in_waterlevel,                      // Water plane height                                                           // [TODO: Implement water shader] TODO:: Update according to weather model        
        in_vector,                          // Arbitrary input vector                                                       // [TBD: Arbitrary]

                                                                     // [TBC/TODO] [REFACTOR in_shadowQuality]

        in_particlepos,                     // Particle position
        in_particlesize,                    // Particle size

        in_color,                           // Arbitrary color input
        in_mod,                             //                                                                              // [TBD: Use appropriate term and TODO: refactor]
        in_alpha,

        use_emit,                           // Emissive feature on/off
        emit_a_base,                        // Emissive albedo texture
        emit_a_normal,                      // Emissive normal texture
        in_emitcolor,                       // Emissive color

        use_spec,                           // Specular feature on/off
        spec_a_base,                        // Specular albedo texture
        spec_a_normal,                      // Specular normal texture
        in_speccolor,                       // Specular color
        in_specexp,                         // Specular exposure

        use_env,                            // Environment mapping feature on/off
        env_a_base,                         // The environment-map texture rendered on an off-screen framebuffer
        env_a_normal,                       // The environment-map normal texture rendered on an off-screen framebuffer     // [NOT USED]
        env_tint,                           // Environment mapping tint color

        useTexture,                         // Use texture feature switch on/off
        use_alpha,                          // Use alpha component feature switch on/off
        ref_size,                           // Reflection Size
        rer_size,                           // Refraction Size                                                              // [TODO]
        blur_size,                          // Blur size
        fresnel_str,                        // Fresnel strength                                                             // [CONFLICTS 'fresnelStr']

       

        in_hudsize,                         // UI object size
        in_hudpos,                          // UI object position
        in_hudcolor,                        // UI object color
        in_hudvalue,                        // UI object value

        in_no_lights,                       // Total number of lights                                                       // [TODO find a way to eliminate]
        curLight,                           // Current light                                                                // [TODO find a way to eliminate]
        uni_no_bones,
            
        defPosition,                        // Deferred lighting position
        defDirection,                       // Deferred lighting direction
        defColor,                           // Deferred color


        defMatrix,                          // Deferred far view matrix                                                     // [TODO: Implement PSSM correctly]
        defInnerMatrix,                     // Deferred near view matrix
        defInvPMatrix,                      // Deferred inverse Projection matrix
        


        fresnelExp,
        fresnelStr,

        

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
        terrain_lightDir,                   // vec3
        terrain_densityMap,                 // float
        terrain_densityFactor

        //projection_rev_matrix,            // NOT USED
    }
}
