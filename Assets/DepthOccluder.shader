Shader "Custom/DepthOccluder"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        Pass
        {
            ColorMask 0   // Invisible (0 RGB write)
            ZWrite On     // Writes depth to GPU buffer
            ZTest LEqual
        }
    }
}