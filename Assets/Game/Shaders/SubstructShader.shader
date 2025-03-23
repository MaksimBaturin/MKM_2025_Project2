Shader "Custom/SubtractShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SubtractTex ("Subtract Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        sampler2D _MainTex;
        sampler2D _SubtractTex;
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainColor = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 subtractColor = tex2D (_SubtractTex, IN.uv_MainTex);
            
            o.Albedo = mainColor.rgb - subtractColor.rgb;
            o.Alpha = mainColor.a;
        }
        ENDCG
    }
    
    Fallback "Diffuse"
}