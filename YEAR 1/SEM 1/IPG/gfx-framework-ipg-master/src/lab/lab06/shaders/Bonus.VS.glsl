#version 330
#extension GL_ARB_separate_shader_objects : enable

// Input
// TODO(student): Get vertex attributes from each location

// totu constant
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 text_coord;
layout(location = 3) in vec3 color;

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
    output_color_vs = normal;

    // TODO(student): Compute gl_Position

    float rand = fract(position.x * 5321.32153 * time + position.y * 321.32131 * time + position.z * 321.321 * time) / 4;
    vec3 pos = position + normal * rand;

    vec4 clipSpacePosition = computeClipSpacePosition(pos);
    gl_Position = clipSpacePosition;

    

}