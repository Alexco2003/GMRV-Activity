#version 330

// Input
// TODO(student): Get values from vertex shader

// Output
layout(location = 0) out vec3 out_color;

in vec3 output_color_vs;

void main()
{
    // TODO(student): Write pixel out color
    out_color = output_color_vs;

}
