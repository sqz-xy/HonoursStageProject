#version 330 core
out vec4 FragColor;

uniform vec4 uColour;

in vec4 oColour;

void main()
{
    FragColor = oColour;
}