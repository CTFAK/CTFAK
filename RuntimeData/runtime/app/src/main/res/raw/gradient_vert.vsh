attribute vec4 position;
//attribute vec2 texCoord;
attribute vec4 color;

uniform mat4 projectionMatrix;
//uniform int inkEffect;
//uniform float inkParam;

varying vec4 vColor;


void main()
{	
	vColor = color;
    gl_Position = position * projectionMatrix;
}