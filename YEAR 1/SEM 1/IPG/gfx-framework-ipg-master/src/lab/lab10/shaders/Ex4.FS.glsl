#version 430

// Input
layout(location = 0) in vec2 texture_coord;

// Uniform properties
uniform sampler2D texture_1;

// Output
layout(location = 0) out vec4 out_color;

void main()
{
    // TODO(student): Apply the texture
    vec4 texture_color = texture2D(texture_1, texture_coord);

    if (texture_color.a < 0.5)
        discard;

    out_color = texture_color;
}
