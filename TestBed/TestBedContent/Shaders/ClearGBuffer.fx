float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

struct PixelShaderOutput
{
    float4 Color : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth : COLOR2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, 1);

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output;

	output.Color = 0.0f;
	output.Color.a = 0.0f;
	
	//When transforming 0.5f into [-1,1] we will get 0.0f
	output.Normal.rgb = 0.5f;

	//No specular power
	output.Normal.a = 0.0f;

	//Max Depth
	output.Depth = 1.0f;

	return output;
}

technique ClearBuffer
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
