#version 330 core
out vec4 FragColor;

uniform vec4 uColour;
uniform vec3 uViewPos;
uniform sampler2D uTextureSampler1;

in vec4 oColour;
in vec2 oTexCoords;
in vec3 oNormal;
in vec3 oFragPos;

void main()
{

    // Lighting

    float ambientStrength = 0.1;
    vec3 lightColour = vec3(1, 1, 1);
    vec3 lightPos = vec3(0, 10, 0);
    
    // Ambient
    vec3 ambient = ambientStrength * lightColour;

    // Diffuse
    vec3 normal = normalize(oNormal);
    vec3 lightDir = normalize(lightPos - oFragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * lightColour;
    
    // Specular
    float specularStrength = 0.5;
    vec3 viewDir = normalize(uViewPos - oFragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColour;

    vec3 result = (ambient + diffuse + specular) * vec3(1, 1, 1);
    //FragColor = vec4(result, 1.0) * texture(uTextureSampler1, oTexCoords) + oColour;
    
    //FragColor = texture(uTextureSampler1, oTexCoords); //Not using textures currently
    FragColor = oColour + texture(uTextureSampler1, oTexCoords) / 20;
}