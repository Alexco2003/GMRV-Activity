#version 430

// Input and output topologies
layout(triangles) in;
layout(triangle_strip, max_vertices = 170) out;

// Uniform properties
uniform mat4 View;
uniform mat4 Projection;
// TODO(student): Declare other uniforms here

// Output
layout(location = 0) out vec3 color;

void EmitPoint(vec3 position, vec3 c)
{
    color = c;
    gl_Position = Projection * View * vec4(position, 1);
    EmitVertex();
}

void main()
{
    vec3 p1 = gl_in[0].gl_Position.xyz;
    vec3 p2 = gl_in[1].gl_Position.xyz;
    vec3 p3 = gl_in[2].gl_Position.xyz;
    // TODO(student): Emit the vertices of the triangle for which
    // information is received in the input attributes. Use EmitPoint()
    // and set a color information associated with each vertex.

    vec3 color_red = vec3(1.0, 0.0, 0.0);
    vec3 color_orange = vec3(1.0, 0.5, 0.0);
    vec3 color_yellow = vec3(1.0, 1.0, 0.0);
    
    vec3 center = (p1 + p2 + p3) / 3.0;

    vec3 p1Inv = vec3(p1.x, 2.0*center.y - p1.y, p1.z);
    vec3 p2Inv = vec3(p2.x, 2.0*center.y - p2.y, p2.z);
    vec3 p3Inv = vec3(p3.x, 2.0*center.y - p3.y, p3.z);

    vec3 in1 = center + (p1 - center) * 0.5;
    vec3 in2 = center + (p2 - center) * 0.5;
    vec3 in3 = center + (p3 - center) * 0.5;

    EmitPoint(p1Inv, color_red);
    EmitPoint(in1, color_orange);
    EmitPoint(in3, color_orange);
    EndPrimitive();

    EmitPoint(p3Inv, color_red);
    EmitPoint(in2, color_orange);
    EmitPoint(in1, color_orange);
    EndPrimitive();

    EmitPoint(p2Inv, color_red);
    EmitPoint(in3, color_orange);
    EmitPoint(in2, color_orange);
    EndPrimitive();
    
    EmitPoint(in1, color_yellow);
    EmitPoint(in2, color_yellow);
    EmitPoint(in3, color_yellow);
    EndPrimitive();


}
