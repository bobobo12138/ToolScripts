Shader "Unlit/HDRPGeoGrass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopColor("TopColor",Color) = (1, 1, 1, 1)
        _BottomColor("BottomColor",Color) = (1, 1, 1, 1)
        //_ColorLerp("ColorLerp",Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
            	float4 vertex : POSITION;
	            float3 normal : NORMAL;
	            float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            // Add inside the CGINCLUDE block.
            struct geometryOutput
            {
                float2 uv: TEXCOORD0;
	            float4 pos : SV_POSITION;
            };

            struct vertOutput
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
	            float3 normal : NORMAL;
	            float4 tangent : TANGENT;
            };

            sampler2D _MainTex;
            float4 _TopColor;
            float4 _BottomColor;
            //float _ColorLerp;
            float4 _MainTex_ST;

            vertOutput vert (appdata v)
            {
                vertOutput o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.tangent = v.tangent;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            ///geo的辅助函数
            geometryOutput VertexOutput(float3 pos, float2 uv)
            {
            	geometryOutput o;
            	o.pos = UnityObjectToClipPos(pos);
                o.uv = uv;
            	return o;
            }
            float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
			}
			float3x3 angleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3
				(
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
				);
			}
            [maxvertexcount(3)]
            void geo(triangle vertOutput IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
            {
                //float3 pos = (IN[0]+IN[1]+IN[2])/3;
                float3 pos = IN[2].vertex;
                float3 vNormal = IN[2].normal;
                float4 vTangent = IN[2].tangent;
                float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;
                //切线转局部矩阵
                float3x3 tangentToLocal = float3x3(
                	vTangent.x, vBinormal.x, vNormal.x,
                	vTangent.y, vBinormal.y, vNormal.y,
                	vTangent.z, vBinormal.z, vNormal.z
                	);
                //随机z轴旋转矩阵
                float3x3 facingRotationMatrix = angleAxis3x3(rand(pos) * 3.14 * 2, float3(0, 0, 1));
                //切线矩阵乘以旋转矩阵得到最终矩阵
                float3x3 transformationMatrix = mul(tangentToLocal, facingRotationMatrix);

                triStream.Append(VertexOutput(pos +mul(transformationMatrix, float3(0.5, 0, 0)),float2(0.5,0)));
                
                triStream.Append(VertexOutput(pos +mul(transformationMatrix, float3(-0.5, 0, 0)),float2(-0.5,0)));
                
                triStream.Append(VertexOutput(pos +mul(transformationMatrix, float3(0, 0, 1)),float2(0,1)));
            }

            fixed4 frag (geometryOutput i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col*=lerp(_BottomColor,_TopColor,i.uv.y);

                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
