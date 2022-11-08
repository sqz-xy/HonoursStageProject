#version 330 core
in vec3 aPosition;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec4 oColour;

void main()
{
    const float brightnessMultiplier = 1.5;
    vec4 position = vec4(aPosition, 1) * uModel * uView * uProjection;
    vec4 colour = vec4(aPosition.y, aPosition.y, aPosition.y, 1);
    
    oColour = colour * brightnessMultiplier;
    gl_Position = position;
}