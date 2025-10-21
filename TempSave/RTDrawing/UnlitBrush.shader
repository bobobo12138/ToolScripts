Shader "Unlit/UnlitBrush_Min" // �ĸ�����
{
    Properties
    {
        _Color ("Brush Target Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        // --- ���ǹؼ��Ķ� ---
        // ����GPUʹ�á���Сֵ������
        BlendOp Min
        // ����GPUԴ��Ŀ�궼��Ҫ�����κ�alpha��ֱ�ӱȽ�
        // Final = min(SrcColor * 1, DstColor * 1)
        Blend One One 
        // --- �����Ķ� ---

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
                alpha = pow(alpha, 0.1);

                // --- ���ǹؼ��Ķ� ---
                // ���ǲ������ (Color, alpha)
                // �������һ���� "��ɫ" �� "Ŀ��ɫ" ֮���ֵ����ɫ
                // alpha = 0 (��Ե),   ��� (1,1,1) (��ɫ)
                // alpha = 1 (����),   ��� _Color.rgb (��ɫ)
                // lerp(a, b, t) = a * (1-t) + b * t
                fixed3 outputColor = lerp(fixed3(1,1,1), _Color.rgb, alpha);

                // Alphaͨ����Ϊ1����Ϊ���� Blend One One ģʽ�²���Ҫ
                return fixed4(outputColor, 1.0); 
                // --- �����Ķ� ---
            }
            ENDCG
        }
    }
}