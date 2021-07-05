varying lowp vec2 texCoordinate;
varying highp vec2 pPos;

uniform lowp sampler2D texture;
uniform lowp float inkParam;
uniform lowp int inkEffect;
uniform lowp vec3 rgbCoeff;

uniform highp vec2 centerpos;
uniform highp vec2 radius;

void main()
{
	lowp vec4 color = texture2D(texture, texCoordinate) * vec4(rgbCoeff, inkParam);

	if(inkEffect == 2)			//INVERT
		color.rgb = vec3(1,1,1)-color.rgb;
	else if(inkEffect == 10)	//MONO
	{
		lowp float mono = 0.3125*color.r + 0.5625*color.g + 0.125*color.b;
		color.rgb = vec3(mono,mono,mono);
	}
	
	highp vec2 eC = vec2(pPos.x-centerpos.x, pPos.y-centerpos.y);
	highp float ellipseFactor = (eC.x*eC.x)/radius.x + (eC.y*eC.y)/radius.y;
	if(ellipseFactor >= 1.0)
		color.a = 0.0;
	gl_FragColor = color;
}