#version 330 core
out vec4 FragColor;

uniform vec4 uColour;
uniform sampler2D uTextureSampler1;

in vec4 oColour;
in vec2 oTexCoords;

void main()
{
    //FragColor = texture(uTextureSampler1, oTexCoords); //Not using textures currently
    FragColor = oColour + texture(uTextureSampler1, oTexCoords) / 20;
}