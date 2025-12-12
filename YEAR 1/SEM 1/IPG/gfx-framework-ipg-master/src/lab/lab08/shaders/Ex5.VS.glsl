#version 330

// Input
layout(location = 0) in vec3 v_position;
layout(location = 1) in vec3 v_normal;
layout(location = 2) in vec2 v_texture_coord;
layout(location = 3) in vec3 v_color;

// Uniform properties
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

uniform sampler2D texture_2;

// Output
out vec2 texture_coord;


void main()
{
    // Pass texture coordinates to fragment shader
    texture_coord = v_texture_coord;

    float heightSample = texture2D(texture_2, v_texture_coord).r;
    float scale = 10;
    float displacement = (heightSample - 0.4) * scale;

    vec3 displaced_pos = v_position;
    displaced_pos.y += displacement;

    gl_Position = Projection * View * Model * vec4(displaced_pos, 1.0);
}