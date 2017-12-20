#version 400 core

in vec3 pos;
in vec4 v_color;

in vec3 v_normal;
in vec2 v_uv;
uniform mat4 p;
uniform mat4 m;
out vec4 f_color;
out vec2 f_uv;
out vec4 f_pos;
out vec3 f_normal;
out vec4 f_lightPos;

vec3 lightPos = vec3(0,0,5);

void main()
{
    f_pos = m * vec4(pos,1);
    f_normal = (m * vec4(v_normal,0)).xyz;
    gl_Position = p * f_pos;
    f_color = v_color;
    f_uv = v_uv;
    f_lightPos = vec4(lightPos, 1);
}