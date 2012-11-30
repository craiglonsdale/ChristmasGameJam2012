float4x4 World;
float4x4 View;
float4x4 Projection;


//Colour of the light
float3 Colour;

//Camera Position for specular light
float3 cameraPosition;

//Use this to compute the world position
float4x4 InvertViewProjection;

//The position of the point light
float3 lightPosition;

//Radius of the lights range
float lightRadius;

float lightIntensity = 1.0f;

//Diffuse colour and specularIntensity in the alpha channel
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
    Mipfilter = LINEAR;
};

sampler depthSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

sampler normalSampler = sampler_state
{
    Texture = (normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    float4 ScreenPosition : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    //Process geom coords to screen position
    float4 worldPosition = mul(float4(input.Position, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.ScreenPosition = output.Position;

    return output;
}

float2 halfPixel;

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Obtain screen position
    input.ScreenPosition.xy /= input.ScreenPosition.w;

    //Obtain TexCoords in relation to current pixel
    //Screen Coords are in [-1,1] by [1, -1]
    //Texture Coords need to be in [0,1] by [0,1]
    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x, -input.ScreenPosition.y) + 1);

    //Align texels to pixels
    texCoord += halfPixel;

    //Get normal data from normalMap and transform into [-1,1] range
    float4 normalData = tex2D(normalSampler, texCoord);
    float3 normal = 2.0f * normalData.xyz - 1.0f;

    //Solve specular power
    float specularPower = normalData.a * 255;

    //Solve spcular intensity
    float specularIntensity = tex2D(colourSampler, texCoord).a;

    //Read in depth
    float depthVal = tex2D(depthSampler, texCoord).r;

    //Compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;

    //Transform into world space
    position = mul(position, InvertViewProjection);
    position /= position.w;

    //Surface to light vector
    float3 lightVector = lightPosition - position;

    //Compute attenuation based on distance from light source (linear) and normalize
    float attenuation = saturate(1.0f - length(lightVector)/lightRadius);
    lightVector = normalize(lightVector);

    //Compute diffuse light
    float NdL = max(0, dot(normal, lightVector));
    float3 diffuseLight = NdL * Colour.rgb;

    //Reflection Vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));

    //Camera to surface vector
    float3 directionToCamera = normalize(cameraPosition - position);

    //Compute SPecular light
    float specularLight = specularIntensity * pow(saturate(dot(reflectionVector, directionToCamera)), specularPower);

    //Now sort out what attenuation has done
    return attenuation * lightIntensity * float4(diffuseLight.rgb, specularLight);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
