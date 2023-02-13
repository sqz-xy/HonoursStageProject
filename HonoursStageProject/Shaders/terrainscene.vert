#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexture;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec4 oColour;
out vec2 oTexCoords;
out vec3 oNormal;
out vec3 oFragPos;

void main()
{
    oTexCoords = aTexture;
    
    // Relief
    
    const float brightnessMultiplier = 1.5;
    vec4 position = vec4(aPosition, 1) * uModel * uView * uProjection;
    vec4 colour = vec4(0, aPosition.y, 0, 1) / 20;
    
    oFragPos = vec3(uModel * vec4(aPosition, 1.0));
    oNormal = mat3(transpose(inverse(uModel))) * aNormal;
    oColour = colour * brightnessMultiplier;
    gl_Position = position;
}