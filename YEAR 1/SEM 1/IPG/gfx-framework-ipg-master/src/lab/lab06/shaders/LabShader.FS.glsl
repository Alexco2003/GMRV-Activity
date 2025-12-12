#version 330

// Input
// TODO(student): Get values from vertex shader

// Output
layout(location = 0) out vec3 out_color;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform float time;
uniform int a;
uniform vec3 b;
uniform vec2 c;
uniform ivec3 d;


in vec3 output_color_vs;

void main()
{
    // TODO(student): Write pixel out color
    out_color = output_color_vs;
}
