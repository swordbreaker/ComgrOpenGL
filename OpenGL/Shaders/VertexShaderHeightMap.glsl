#version 400 core

in vec3 pos;
in vec4 v_color;

in vec2 v_uv;
uniform mat4 p;
uniform mat4 m;
out vec4 f_color;
out vec2 f_uv;
out vec4 f_pos;
out vec3 f_normal;
out vec4 f_lightPos;
out vec4 f_up;

uniform sampler2D heightMap;
uniform float scale;

vec3 lightPos = vec3(0,0,5);

vec3 calcNormal(const in vec2 pos);

void main()
{
    float h = texture(heightMap, v_uv).r * scale;
    vec3 newPos = vec3(pos.x, pos.y, pos.z + h);

    vec3 normal = calcNormal(v_uv);

    f_pos = m * vec4(newPos,1);
    f_up = m * vec4(0,1,0,0);
    f_normal = (m * vec4(normal,0)).xyz;
    gl_Position = p * f_pos;
    f_color = v_color;
    f_uv = v_uv;
    f_lightPos = vec4(lightPos, 1);
}

vec3 calcNormal(const in vec2 pos)
{
    float x = pos.x;
    float y = pos.y;

    vec2 p1 = vec2(x + 0.1, y);
    vec2 p2 = vec2(x - 0.1, y);
    vec3 v1 = vec3(0.2, 0, texture(heightMap, p1).r * scale - texture(heightMap, p2).r * scale);

    vec2 p3 = vec2(x, y + 0.1);
    vec2 p4 = vec2(x, y - 0.1);
    vec3 v2 = vec3(0, 0.2, texture(heightMap, p3).r * scale - texture(heightMap, p4).r * scale);

    return normalize(cross(v1, v2));
}