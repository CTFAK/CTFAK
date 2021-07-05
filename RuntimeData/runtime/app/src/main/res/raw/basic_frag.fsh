varying lowp vec2 texCoordinate;
uniform lowp sampler2D texture;

void main()
{
	gl_FragColor = texture2D(texture, texCoordinate);
}
