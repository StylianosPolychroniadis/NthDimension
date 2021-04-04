#version 130 //330 core

//#variables
//#functions

//---------IN------------
in vec3             g_pos;
in vec3             sun_norm;
in vec3             star_pos;
//---------UNIFORM------------ 
uniform sampler2D   baseTexture;        // Tint1    //the color of the sky on the half-sphere where the baseTextureThree is. (sky_time x height)
uniform sampler2D   baseTextureTwo;     // Tint2    //the color of the sky on the opposite half-sphere. (sky_time x height)
uniform sampler2D   baseTextureThree;   // Sun      //sun texture (radius x sky_time)
uniform sampler2D   baseTextureFour;    // Moon     // moon texture (circular)
uniform sampler2D   auxTexture;         // Clouds1  //light clouds texture (spherical UV projection)
uniform sampler2D   auxTextureTwo;      // Clouds2  //heavy clouds texture (spherical UV projection)
uniform float       sky_clouds;         // mixing factor (0.5 to 1.0)
uniform float       sky_time;
//---------OUT------------
out vec4            out_frag_color;    //out vec3 color;


//---------NOISE GENERATION------------
//Noise generation baseTextured on a simple hash, to ensure that if a given point on the dome
//(after taking into account the rotation of the sky) is a star, it remains a star all night long
float Hash( float n ){
        return fract( (1.0 + sin(n)) * 415.92653);
}
float Noise3d( vec3 x ){
    float xhash = Hash(round(400*x.x) * 37.0);
    float yhash = Hash(round(400*x.y) * 57.0);
    float zhash = Hash(round(400*x.z) * 67.0);
    return fract(xhash + yhash + zhash);
}

//---------MAIN------------
void main(){
      
    if(g_pos.y < 0)
        discard;
    
    vec3 pos_norm = normalize(g_pos);
    float dist = dot(sun_norm,pos_norm);

    //We read the tint texture according to the position of the sun and the weather factor
    vec3 color_wo_sun = texture(baseTextureTwo, vec2((sun_norm.y + 1.0) / 2.0,max(0.01,pos_norm.y))).rgb;
    vec3 color_w_sun = texture(baseTexture, vec2((sun_norm.y + 1.0) / 2.0,max(0.01,pos_norm.y))).rgb;
    vec3 color;
    color = sky_clouds*mix(color_wo_sun,color_w_sun,dist*0.5+0.5);

    //Computing u and v for the clouds textures (spherical projection)
    float u = 0.5 + atan(pos_norm.z,pos_norm.x)/(2*3.14159265);
    float v = - 0.5 + asin(pos_norm.y)/3.14159265;

    //Cloud color
    //color depending on the weather (shade of grey) *  (day or night) ?
    vec3 cloud_color = vec3(min(sky_clouds*3.0/2.0,1.0))*(sun_norm.y > 0 ? 0.95 : 0.95+sun_norm.y*1.8);
  
    //Reading from the clouds maps
    //mixing according to the sky_clouds (1.0 -> auxTexture (sunny), 0.5 -> auxTextureTwo (rainy))
    //+ time translation along the u-axis (horizontal) for the clouds movement
    float transparency = mix(texture(auxTextureTwo,vec2(u+sky_time,v)).r,texture(auxTexture,vec2(u+sky_time,v)).r,(sky_clouds-0.5)*2.0);

    // Stars
    if(sun_norm.y<0.1){//Night or dawn
        float threshold = 0.99;
        //We generate a random value between 0 and 1
        float star_intensity = Noise3d(normalize(star_pos));
        //And we apply a threshold to keep only the brightest areas
        if (star_intensity >= threshold){
            //We compute the star intensity
            star_intensity = pow((star_intensity - threshold)/(1.0 - threshold), 6.0)*(-sun_norm.y+0.1);
            color += vec3(star_intensity);
        }
    }

    //Sun
    float radius = length(pos_norm-sun_norm);
    if(radius < 0.05){//We are in the area of the sky which is covered by the sun
        float sky_time = clamp(sun_norm.y,0.01,1);
        radius = radius/0.05;
        if(radius < 1.0-0.001){//< we need a small bias to avoid flickering on the border of the texture
            //We read the alpha value from a texture where x = radius and y=height in the sky (~time)
            vec4 sun_color = texture(baseTextureThree,vec2(radius,sky_time));
            color = mix(color,sun_color.rgb,sun_color.a);
        }
    }

    //Moon
    float radius_moon = length(pos_norm+sun_norm);//the moon is at position -sun_pos
    if(radius_moon < 0.03){//We are in the area of the sky which is covered by the sky_moon
        //We define a local plane tangent to the skydome at -sun_norm
        //We work in model space (everything normalized)
        vec3 n1 = normalize(cross(-sun_norm,vec3(0,1,0)));
        vec3 n2 = normalize(cross(-sun_norm,n1));
        //We project pos_norm on this plane
        float x = dot(pos_norm,n1);
        float y = dot(pos_norm,n2);
        //x,y are two sine, ranging approx from 0 to sqrt(2)*0.03. We scale them to [-1,1], then we will translate to [0,1]
        float scale = 23.57*0.5;
        //we need a compensation term because we made projection on the plane and not on the real sphere + other approximations.
        float compensation = 1.4;
        //And we read in the texture of the sky_moon. The projection we did previously allows us to have an undeformed sky_moon
        //(for the baseTextureThree we didn't care as there are no details on it)
        color = mix(color,texture(baseTextureFour,vec2(x,y)*scale*compensation+vec2(0.5)).rgb,clamp(-sun_norm.y*3,0,1));
    }

    //Final mix
    //mixing with the cloud color allows us to hide things behind clouds (baseTextureThree, stars, sky_moon)
    color = mix(color,cloud_color,clamp((2-sky_clouds)*transparency,0,1));
    out_frag_color.rgb = color;
}
