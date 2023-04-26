#version 330 core
out vec4 FragColor;

uniform vec4 uColour;
uniform sampler2D uTextureSampler1;

in vec2 oTexCoords;

void main()
{
    FragColor = texture(uTextureSampler1, oTexCoords) + uColour;
}