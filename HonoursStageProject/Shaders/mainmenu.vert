#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexture;

out vec2 oTexCoords;

void main()
{
    oTexCoords = aTexture;
    gl_Position = vec4(aPosition, 1.0);
}