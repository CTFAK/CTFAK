// Monochrome effect

sampler TextureSampler : register(s0);

float4 main(float2 texCoord : TEXCOORD0) : COLOR
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
	float4 f4 = tex * float4(0.299f, 0.587f, 0.114f, 1.0f);
	float f = f4.r + f4.g + f4.b;
	tex.rgb = f;
    return tex;
}    

technique Mono
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
