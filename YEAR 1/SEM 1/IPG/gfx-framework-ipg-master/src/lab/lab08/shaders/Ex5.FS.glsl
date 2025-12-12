#version 330

// Input
in vec2 texture_coord;

// Uniform properties
uniform sampler2D texture_1;
uniform sampler2D texture_2;
// TODO(student): Declare various other uniforms

// Output
layout(location = 0) out vec4 out_color;


void main()
{
    // TODO(student): Calculate the out_color using the texture() function.
    vec4 color1 = texture2D(texture_1, texture_coord);
    float heightSample = texture2D(texture_2, texture_coord).r;

    // TODO(student): Use the "discard" directive to terminate execution
    // based on the value of the alpha channel
    if (color1.a < 0.5)
        discard;

    vec3 heightColor = vec3(heightSample);

    out_color = vec4(mix(color1.rgb, heightColor, heightSample), color1.a);
    
}
