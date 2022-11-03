#version 330 core
in vec3 aPosition;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    // Lying piece of shit
    gl_Position = vec4(aPosition, 1) * uModel * uView * uProjection;
}