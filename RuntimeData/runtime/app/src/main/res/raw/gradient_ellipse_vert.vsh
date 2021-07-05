attribute vec4 position;
attribute vec2 texCoord;
attribute vec4 color;

uniform mat4 projectionMatrix;
uniform int inkEffect;
uniform float inkParam;

uniform vec2 centerpos;
uniform vec2 radius;

varying vec4 vColor;
varying vec2 pPos;

void main()
{	
	vColor = color;
	pPos = position.xy;
    gl_Position = position * projectionMatrix;
}
