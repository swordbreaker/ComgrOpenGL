#version 400 core

in vec3 pos;
in vec4 v_color;

in vec3 v_normal;
uniform mat4 viewModel;
out vec4 f_color;

vec4 lightPos = vec4(1,1,5,1);

void main()
{
    vec4 newPos = viewModel * vec4(pos,1);
    // vec4 normal = vec4(v_normal,0);

    // vec4 l = normalize(lightPos - newPos);
    // float dotNL = dot(l, normal);
    // vec4 diffuse = max(0, dotNL) * v_color;
    // vec4 r = 2 * dotNL * normal - l;
    // vec4 specular = pow(max(0, dotNL) - dot(r, normalize(newPos)), 50) * vec4(1);

    gl_Position = newPos;
    f_color = v_color;

	//f_color = vec4(1,0, 0.0, 1.0);
}