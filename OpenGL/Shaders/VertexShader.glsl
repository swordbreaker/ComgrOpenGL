#version 400 core

in vec3 pos;
in vec4 v_color;
uniform mat4 viewModel;
out vec4 f_color;

void main()
{
    gl_Position = viewModel * vec4(pos,1);
	f_color = v_color;
}