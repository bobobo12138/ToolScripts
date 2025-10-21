Shader "Unlit/FadeToWhite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // _FadeAmount ԽС����ʧ��Խ��
        _FadeAmount ("Fade Amount", Range(0, 0.1)) = 0.01 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off
        Cull Off
        Blend Off // ������ֱ�Ӹ��ǣ����ǻ��

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half _FadeAmount; // ��������C#�Ĳ���

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. ��ȡRT�ϵĵ�ǰ��ɫ
                fixed4 currentColor = tex2D(_MainTex, i.uv);
                
                // 2. ����Ŀ����ɫ����ɫ��
                fixed4 targetColor = fixed4(1,1,1,1);
                
                // 3. ����ǰ��ɫ��Ŀ����ɫ��������һ���
                fixed4 finalColor = lerp(currentColor, targetColor, _FadeAmount);
                finalColor +=_FadeAmount/2;
                finalColor = saturate(finalColor);

                return finalColor;
            }
            ENDCG
        }
    }
}