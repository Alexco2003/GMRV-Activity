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

out vec3 output_color_vs;

void main()
{
  
    // TODO(student): Send output to fragment shader
    output_color_vs = color;

    // TODO(student): Compute gl_Position

    vec4 worldPos = View * Model * vec4(position, 1.0);

    worldPos.y += sin(time);


    gl_Position = Projection * worldPos;

    

}