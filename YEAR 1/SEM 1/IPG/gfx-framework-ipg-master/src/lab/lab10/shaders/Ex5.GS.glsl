#version 430

// Input and output topologies
layout(triangles) in;
layout(line_strip, max_vertices = 6) out;

// Input
layout(location = 1) in vec2 v_texture_coord[];

// Uniform properties
uniform mat4 View;
uniform mat4 Projection;
// TODO(student): Declare other uniforms here
uniform float time;

// Output
layout(location = 0) out vec2 texture_coord;

void EmitLine(vec3 a_world, vec3 b_world, vec2 tex)
{
    gl_Position = Projection * View * vec4(a_world, 1.0);
	texture_coord = tex;
    EmitVertex();

    gl_Position = Projection * View * vec4(b_world, 1.0);
	texture_coord = tex;
    EmitVertex();

    EndPrimitive();
}


  

void main()
{

	vec3 p1 = gl_in[0].gl_Position.xyz;
    vec3 p2 = gl_in[1].gl_Position.xyz;
    vec3 p3 = gl_in[2].gl_Position.xyz;

	vec3 v12 = normalize(p2 - p1);
	vec3 v13 = normalize(p3 - p1);
	vec3 normal = cross(v12, v13);


	for (int i = 0; i < 3; i++) 
	{
		vec3 world_position = gl_in[i].gl_Position.xyz;
		vec3 tip_world = world_position + normal * 0.05;
		EmitLine(world_position, tip_world, v_texture_coord[i]);
		
	}
	
	

	
	



}
