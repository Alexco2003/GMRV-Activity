#version 330
#extension GL_ARB_separate_shader_objects : enable

// Input
// TODO(student): Get vertex attributes from each location

// totu constant
layout(location = 0) in vec3 position;
layout(location = 3) in vec3 normal;
layout(location = 2) in vec2 text_coord;
layout(location = 1) in vec3 color;

// Uniform properties

// global global
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

uniform float time;
uniform int a;
uniform vec3 b;
uniform vec2 c;
uniform ivec3 d;
uniform vec3[100] e;

// Output
// TODO(student): Output values to fragment shader
vec4 computeClipSpacePosition(const in vec3 objectSpacePosition)
{
    return Projection * View * Model * vec4(objectSpacePosition, 1);
}

out vec3 output_color_vs;

void main()
{

    // TODO(student): Send output to fragment shader
    //time = 2.0;
    //position.x += 3;
    output_color_vs = color;
    
    // TODO(student): Compute gl_Position
    // vec3 pos = position + vec3(0, sin(time), 0);
    
    float rand = fract(position.x * 5321.32153 * time + position.y * 321.32131 * time + position.z * 321.321 * time);
    vec3 pos = position + normal * rand;

    float s = 1.0 + 0.25 * sin(time * 3);
    vec3 scaledPos = vec3(position * s);

    vec4 clipSpacePosition = computeClipSpacePosition(scaledPos);
    gl_Position = clipSpacePosition;

    float m = 0.5 + 0.5 * sin(time * 3);
    output_color_vs = color * m;
}