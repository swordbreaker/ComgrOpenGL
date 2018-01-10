#version 400 core

in vec4 f_color;
in vec2 f_uv;
in vec4 f_pos;
in vec3 f_normal;
in vec4 f_lightPos;
in vec4 f_up;

out vec4 color;

uniform sampler2D texture1;
uniform vec4 enviroment;

void main()
{
    vec4 texColor = texture(texture1, f_uv);

    vec3 l = normalize(f_lightPos.xyz - f_pos.xyz);
    float dotNL = dot(l, f_normal);
    vec4 diffuse = max(0, dotNL) * texColor;
    vec3 r = 2 * max(0, dotNL) * f_normal - l;
    
    vec4 specular = pow(max(0, -dot(r, normalize(f_pos.xyz))), 50) * vec4(1);

    // float d = dot(f_up.xyz, f_normal);
    // d = clamp(d, 0, 1);
    // texColor = d * texColor + (1 - d) * vec4(1,1,1,1);


    color = (enviroment * texColor) + diffuse + specular;
}