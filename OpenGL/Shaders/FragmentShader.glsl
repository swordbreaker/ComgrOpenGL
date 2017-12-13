#version 400 core

in vec4 f_color;
out vec4 color;

void main()
{
    color = f_color;
	//color = vec4(0.5, 0.5, 0.5, 1);
    //gl_FragColor = vec4(1, 0, 0, 1);
}