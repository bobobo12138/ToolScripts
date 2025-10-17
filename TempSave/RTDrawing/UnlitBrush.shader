Shader "Unlit/UnlitBrush"
{
    Properties
    {
        // 可以添加一个颜色属性，让笔刷颜色可配置
        _Color ("Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // 关键的混合模式
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
                // 计算到中心(0.5, 0.5)的距离
                float dist = distance(i.uv, float2(0.5, 0.5));

                // 将距离从 [0, 0.5] 映射到 [0, 1] 并反转
                float alpha = 1.0 - saturate(dist * 2.0);

                // 使用 pow 使边缘更柔和
                alpha = pow(alpha, 2.0);

                // 返回带透明度的颜色
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}