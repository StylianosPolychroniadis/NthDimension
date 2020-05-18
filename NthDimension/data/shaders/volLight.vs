#version 130

precision lowp float;

uniform vec3 in_light;

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;
in vec2 in_texture;

uniform vec3 defDirection;
uniform vec3 defColor;
uniform mat4 defMatrix;
uniform mat4 defInnerMatrix;
uniform mat4 defInvPMatrix;

uniform mat4 invMVPMatrix;

out vec3 g_pos_far;

// Added Apr-27-18 for Volumetric Lights
out float dist_close;
out float dist_far;
const vec3 box[8] = vec3[](
    vec3(1.0,1.0,1.0),
    vec3(1.0,1.0,-1.0),
    vec3(1.0,-1.0,1.0),
    vec3(1.0,-1.0,-1.0),
    vec3(-1.0,1.0,1.0),
    vec3(-1.0,1.0,-1.0),
    vec3(-1.0,-1.0,1.0),
    vec3(-1.0,-1.0,-1.0));
// Added Apr-27-18 for Volumetric Lights
void calcVolLight()
{
	dist_close = 1;
    dist_far = 0;
    for(int i = 0; i < 8; i++)
    {
        //calculate global position
        vec4 g_pos = vec4(box[i].xyz,1);
        g_pos = vec4(defInvPMatrix * g_pos);


        g_pos.xyz /= g_pos.w;
        g_pos.w = 1;
        
        //calculate onsceen position
        vec4 screenpos = vec4(projection_matrix * modelview_matrix * g_pos);
        
        //save
        if(screenpos.z > 0)
        {
            screenpos.xyz /= screenpos.w;
            
            if(screenpos.z < dist_close)
                dist_close = screenpos.z;
                
            if(screenpos.z > dist_far)
                dist_far = screenpos.z;
        }
        else
        {
            dist_close = 0;
        }
    }
}
	
void main(void)
{
	vec4 g_pos = vec4(in_position.xyz,1.0);
	g_pos = vec4(defInvPMatrix * g_pos);
	g_pos.xyz /= g_pos.w;
	g_pos.w = 1.0;
	
	vec4 screenpos = vec4(projection_matrix * modelview_matrix * g_pos);
	gl_Position = screenpos;
	
	screenpos /= screenpos.w;

	g_pos = vec4(invMVPMatrix * vec4(screenpos.xy,1.0,1.0));
	g_pos /= g_pos.w;
	
	g_pos_far = g_pos.xyz;
	
	calcVolLight();
}

