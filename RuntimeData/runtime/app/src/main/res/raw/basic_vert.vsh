attribute vec4 position;
attribute vec2 texCoord;
//attribute vec4 color;

uniform mat4 projectionMatrix;

//uniform sampler2D texture;
varying vec2 texCoordinate;

void main()
{	
	texCoordinate = texCoord;
    gl_Position = position * projectionMatrix;
}