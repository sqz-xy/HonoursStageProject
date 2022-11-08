#version 330 core
in vec3 aPosition;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec4 oColour;

void main()
{
    // Lying piece of shit
    vec4 position = vec4(aPosition, 1) * uModel * uView * uProjection;
    
    oColour = vec4(0, aPosition.y, 0, 1);
    
    gl_Position = position;
}