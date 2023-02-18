#version 330 core

flat out vec4 FragColor;

flat in vec4 Color;

void main()
{
    FragColor = Color;
}