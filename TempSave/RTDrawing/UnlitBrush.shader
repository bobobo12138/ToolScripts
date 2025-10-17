Shader "Unlit/UnlitBrush"
{
    Properties
    {
        // �������һ����ɫ���ԣ��ñ�ˢ��ɫ������
        _Color ("Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // �ؼ��Ļ��ģʽ
        ZWrite Off
        Cull Off

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

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ���㵽����(0.5, 0.5)�ľ���
                float dist = distance(i.uv, float2(0.5, 0.5));

                // ������� [0, 0.5] ӳ�䵽 [0, 1] ����ת
                float alpha = 1.0 - saturate(dist * 2.0);

                // ʹ�� pow ʹ��Ե�����
                alpha = pow(alpha, 2.0);

                // ���ش�͸���ȵ���ɫ
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}