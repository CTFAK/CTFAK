// Invert effect

sampler TextureSampler : register(s0);

float4 main(float2 texCoord : TEXCOORD0) : COLOR
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
	if ( tex.a != 0.0 )
		tex.rgb = 1.0f - tex.rgb;
    return tex;
}    

technique Invert
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
