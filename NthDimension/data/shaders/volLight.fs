#version 130

#variables

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in float dist_close;
in float dist_far;
float samples = 30.0;

in vec3 g_pos_far;


#functions


void main() 
{
#include defBase.snip code:off


    screenpos = gl_FragCoord.xy/in_rendersize; //screenpos();
    screenHelper = screenpos - 0.5;


    //get depth buffer for culling
    depth = texture(Texture5,screenpos).r;
    depth += 0.001;
        //make it linear
    depth = (1.0 * in_near) / (in_far + in_near - depth * (in_far-in_near));
    depth -= 0.001;


    out_frag_color.rgb = vec3(0,0,0);
    
    viewVec = viewDirection + viewUp * screenHelper.y + viewRight * screenHelper.x;
    
    //wtf only half?
    viewVec *= 2;
    depth /= 2;
    
    //some randomness
    rnd = texture(Texture4,gl_FragCoord.xy/128).xy -0.5;
    
    //step through layers
    float stepsize = (dist_far - dist_close) / samples;
    for(float i = dist_close + stepsize * rnd.x; i < dist_far && i < depth; i += stepsize)
    {
        g_pos = viewPosition + viewVec * i;
        
        out_frag_color.rgb += calcShadow(vec4(g_pos,1), vec2(0,0), 0)/samples;
    }
    
    //color it yay
    out_frag_color.rgb *= defColor;
    out_frag_color.a = 1.0;
}