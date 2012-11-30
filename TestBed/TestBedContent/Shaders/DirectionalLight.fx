//Direction of the light
float3 lightDirection;

//Colour of the light
float3 Colour;

//Position of the camera - Used for spec light.
float3 cameraPosition;

//This is used to compute the world-position
float4x4 InvertViewProjection;

//Diffues colour, and specularIntensity in the alpha channel
texture colourMap;

//Normals and specularPower in the alpha channel
texture normalMap;

//Depth map
texture depthMap;

sampler colourSampler = sampler_state
{
    Texture = (colourMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler normalSampler = sampler_state
{
    Texture = (normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};

sampler depthSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

float2 halfPixel;

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = float4(input.Position, 1);

    //Align texture coords
    output.TexCoord = input.TexCoord + halfPixel;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //get normal data from the normalMap
    float4 normalData = tex2D(normalSampler, input.TexCoord);

    //transform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;

    //get spec power in range of [0,255]
    float specularPower = normalData.a * 255;

    //get specular intensity from the colourMap
    float specularIntensity = tex2D(colourSampler, input.TexCoord).a;

    //Get the depth value for this pixel
    float depthVal = tex2D(depthSampler, input.TexCoord).r;

    //Compute screen-space position [-1,1] on height and width
    float4 position;
    position.x = input.TexCoord.x * 2.0f - 1.0f;
    position.y = -(input.TexCoord.y * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;

    //Transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;

    //Surface to light vector
    float3 lightVector = -normalize(lightDirection);

    //Compute diffuse light
    float NdL = max(0, dot(normal, lightVector));
    float3 diffuseLight = NdL * Colour.rgb;

    //Reflection Vector
    float3 reflectionVector = normalize(reflect(lightVector, normal));

    //Camera to surface vector
    float3 directionToCamera = normalize(cameraPosition - position);

    //Compute specular light
    float specularLight = specularIntensity * pow(saturate(dot(reflectionVector, directionToCamera)), specularPower);

    //Output two lights
    return float4(diffuseLight.rgb, specularLight);
}

technique DirectionalLight
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
