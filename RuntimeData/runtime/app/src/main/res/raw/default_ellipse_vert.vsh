attribute vec4 position;
attribute vec2 texCoord;
//attribute vec4 color;

varying vec2 texCoordinate;
varying vec2 pPos;

//uniform low sampler2D texture;
uniform mat4 projectionMatrix;
//uniform int inkEffect;
//uniform float inkParam;

//uniform vec2 centerpos;
//uniform vec2 radius;

void main()
{	
	texCoordinate = texCoord;
	pPos = position.xy;
    gl_Position = position * projectionMatrix;
}