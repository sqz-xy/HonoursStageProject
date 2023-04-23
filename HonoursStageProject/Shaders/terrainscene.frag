#version 330 core
out vec4 FragColor;

uniform vec4 uColour;
uniform vec3 uViewPos;
uniform sampler2D uTextureSampler1;

uniform float uMaxVal;
uniform float uMinVal;

in vec4 oColour;
in vec2 oTexCoords;
in vec3 oNormal;
in vec3 oFragPos;

void main()
{
    // Lighting
    float ambientStrength = 0.1;
    
    // Yellow ish colour
    vec3 lightColour = vec3(1, 1, 0.8f);
    vec3 lightPos = vec3(0, 30, 0);
    
    // Ambient
    vec3 ambient = ambientStrength * lightColour;

    // Diffuse
    vec3 normal = normalize(oNormal);
    vec3 lightDir = normalize(lightPos - oFragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * lightColour;
    
    vec3 result = (ambient + diffuse) * vec3(1, 1, 1);
    vec4 colour = vec4(1, 1, 1, 1);
    
    // Height value normalised between 0 and 1;
    float normHeight = (oFragPos.y - uMinVal) / (uMaxVal - uMinVal);
    
    if (normHeight > 0.51f)
        colour = vec4(0, normHeight, 0, normHeight);
    else if (normHeight < 0.50f)
        colour = vec4(0, 0, normHeight, normHeight);
    else
        colour = vec4(0, normHeight, normHeight, normHeight);
    
    colour *= 0.5f;
    
    // Add lighting and terrain colour
    FragColor = colour + (vec4(result, 1.0f) * 0.1f);
}