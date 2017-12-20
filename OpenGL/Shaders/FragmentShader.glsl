#version 400 core

in vec4 f_color;
in vec2 f_uv;
in vec4 f_pos;
in vec3 f_normal;
in vec4 f_lightPos;

out vec4 color;

uniform sampler2D texture1;
uniform vec4 enviroment;

void main()
{
    vec4 texColor = texture(texture1, f_uv);

    //color = texColor + f_color;

    vec3 l = normalize(f_lightPos.xyz - f_pos.xyz);
    float dotNL = dot(l, f_normal);
    vec4 diffuse = max(0, dotNL) * f_color;
    vec3 r = 2 * max(0, dotNL) * f_normal - l;
    
    //vec4 specular = pow(dot(r, normalize(f_pos.xyz)), 50) * vec4(1);
    vec4 specular = pow(max(0, -dot(r, normalize(f_pos.xyz))), 50) * vec4(1);

    //var Is = (float)Math.Pow(Math.Max(0, -Vector3.Dot(r, Vector3.Normalize(p3D))), 50) * new Vector3(1f, 1f, 1f);

    color = (enviroment * f_color) + texColor * diffuse + specular;

    // vec4 normal = vec4(v_normal,0);

    // vec4 l = normalize(lightPos - newPos);
    // float dotNL = dot(l, normal);
    // vec4 diffuse = max(0, dotNL) * v_color;
    // vec4 r = 2 * dotNL * normal - l;
    // vec4 specular = pow(max(0, dotNL) - dot(r, normalize(newPos)), 50) * vec4(1);

	//color = vec4(0.5, 0.5, 0.5, 1);
    //gl_FragColor = vec4(1, 0, 0, 1);

    //return (_scene.EnviromentLight * color) + matColor * Id + Is;
}