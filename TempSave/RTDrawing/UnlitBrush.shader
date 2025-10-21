Shader "Unlit/UnlitBrush_Min" // 改个名字
{
    Properties
    {
        _Color ("Brush Target Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        // --- 这是关键改动 ---
        // 告诉GPU使用“最小值”操作
        BlendOp Min
        // 告诉GPU源和目标都不要乘以任何alpha，直接比较
        // Final = min(SrcColor * 1, DstColor * 1)
        Blend One One 
        // --- 结束改动 ---

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
                alpha = pow(alpha, 0.1);

                // --- 这是关键改动 ---
                // 我们不再输出 (Color, alpha)
                // 而是输出一个在 "白色" 和 "目标色" 之间插值的颜色
                // alpha = 0 (边缘),   输出 (1,1,1) (白色)
                // alpha = 1 (中心),   输出 _Color.rgb (黑色)
                // lerp(a, b, t) = a * (1-t) + b * t
                fixed3 outputColor = lerp(fixed3(1,1,1), _Color.rgb, alpha);

                // Alpha通道设为1，因为它在 Blend One One 模式下不重要
                return fixed4(outputColor, 1.0); 
                // --- 结束改动 ---
            }
            ENDCG
        }
    }
}