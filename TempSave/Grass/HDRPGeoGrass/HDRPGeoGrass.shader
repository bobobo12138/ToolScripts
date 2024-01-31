Shader "Unlit/HDRPGeoGrass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopColor("TopColor",Color) = (1, 1, 1, 1)
        _BottomColor("BottomColor",Color) = (1, 1, 1, 1)
        _BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
        _RandomGroupNum("Random Group Num", Range(1,40)) = 1//
        //_ColorLerp("ColorLerp",Range(0, 1)) = 0.5
        [Space(20)]
        _BladeWidth("Blade Width", Float) = 0.05
        _BladeWidthRandom("Blade Width Random", Float) = 0.02
        _BladeHeight("Blade Height", Float) = 0.5//
        _BladeHeightRandom("Blade Height Random", Float) = 0.3
        _BladeForward("Blade Forward Amount", Float) = 0.38     //叶细向前量
        _BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2  //叶细曲率量

        [Space(20)]
        _WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
        _WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
        _WindStrength("Wind Strength", Float) = 1

        [Space(20)]
        _TessellationUniform("Tessellation Uniform", Range(1, 64)) = 1


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
            #pragma hull hull
            #pragma domain domain

            // make fog work
            #pragma multi_compile_fog

            #include "Shaders/CustomTessellation.cginc"
            #include "UnityCG.cginc"


            #define BLADE_SEGMENTS 3
            //struct vertexInput
            //{
            //	float4 vertex : POSITION;
	           // float3 normal : NORMAL;
	           // float4 tangent : TANGENT;
            //    float2 uv : TEXCOORD0;
            //};

            // Add inside the CGINCLUDE block.
            struct geometryOutput
            {
                float2 uv: TEXCOORD0;
	            float4 pos : SV_POSITION;
            };

            //struct vertexOutput
            //{
            //    float2 uv : TEXCOORD0;
            //    float4 vertex : SV_POSITION;
	           // float3 normal : NORMAL;
	           // float4 tangent : TANGENT;
            //};


            sampler2D _MainTex;
            float4 _TopColor;
            float4 _BottomColor;
            //float _ColorLerp;
            float4 _MainTex_ST;
            float _BendRotationRandom;
            int _RandomGroupNum;


            float _BladeHeight;
            float _BladeHeightRandom;	
            float _BladeWidth;
            float _BladeWidthRandom;
            float _BladeForward;
            float _BladeCurve;

            sampler2D _WindDistortionMap;
            float4 _WindDistortionMap_ST;
            float2 _WindFrequency;
            float _WindStrength;

            //vertexOutput vert (vertexInput v)
            //{
            //    vertexOutput o;
            //    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            //    o.vertex = v.vertex;
            //    o.normal = v.normal;
            //    o.tangent = v.tangent;
            //    UNITY_TRANSFER_FOG(o,o.vertex);
            //    return o;
            //}

            ///geo的辅助函数
            geometryOutput VertexOutput(float3 pos, float2 uv)
            {
            	geometryOutput o;
            	o.pos = UnityObjectToClipPos(pos);
                o.uv = uv;
            	return o;
            }

            geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix)
            {
                //草网格生成
            	float3 tangentPoint = float3(width, forward, height);
            
            	float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
            	return VertexOutput(localPosition, uv);
            }

            float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
			}
			float3x3 AngleAxis3x3(float angle, float3 axis)
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
            //[maxvertexcount(BLADE_SEGMENTS*2+1)]
            [maxvertexcount(120)]
            void geo(triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
            {
                //float3 pos = (IN[0]+IN[1]+IN[2])/3;
                float3 pos = IN[2].vertex;
                float3 vNormal = IN[2].normal;
                float4 vTangent = IN[2].tangent;
                float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

                float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
                float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
                float3 wind = normalize(float3(windSample.x , windSample.y, 0));
                float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);//风向旋转矩阵

                //子集循环
                for(int i =0;i< _RandomGroupNum ;i++)
                {
                    float height = (rand(pos.zyx+i) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
                    float width = (rand(pos.xzy+i) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
                    float forward = rand(pos.yyz) * _BladeForward;
                    //切线转局部矩阵
                    float3x3 tangentToLocal = float3x3(
                    	vTangent.x, vBinormal.x, vNormal.x,
                    	vTangent.y, vBinormal.y, vNormal.y,
                    	vTangent.z, vBinormal.z, vNormal.z
                    	);
                    //随风摇摆旋转矩阵
                    float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos+i) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
                    //随机z轴旋转矩阵
                    float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos+i) * 3.14 * 2, float3(0, 0, 1));
                    //切线矩阵乘以旋转矩阵得到transformationMatrix矩阵，之后再transformationMatrix矩阵*摇摆矩阵
                    float3x3 transformationMatrix = mul(mul(mul(tangentToLocal,windRotation), facingRotationMatrix), bendRotationMatrix);
                    float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);

                    //细分循环
                    for (int j = 0; j < BLADE_SEGMENTS; j++)
                    {
                    	float t = j / (float)BLADE_SEGMENTS;

                        //草叶细分
                        float segmentHeight = height * t;
                        float segmentWidth = width * (1 - t);
                        float segmentForward = pow(t, _BladeCurve) * forward;

                        float3x3 transformMatrix = j == 0 ? transformationMatrixFacing : transformationMatrix;

                        //triStream.Append(VertexOutput(pos + float3(rand(pos*i),0,rand(pos*i+1)) + mul(transformationMatrixFacing, float3(width, 0, 0)),  float2(0.5,0)));
                        //triStream.Append(VertexOutput(pos + float3(rand(pos*i),0,rand(pos*i+1))+ mul(transformationMatrixFacing,  float3(-width, 0, 0)), float2(-0.5,0)));
                        //triStream.Append(VertexOutput(pos + float3(rand(pos*i),0,rand(pos*i+1)) + mul(transformationMatrix, float3(0, 0, height)), float2(0,1)));

                        triStream.Append(GenerateGrassVertex(pos + float3(rand(pos*i),0,rand(pos*i+1)),
                        segmentWidth, segmentHeight,segmentForward, float2(0, t), transformMatrix));
                        triStream.Append(GenerateGrassVertex(pos + float3(rand(pos*i),0,rand(pos*i+1)),
                        -segmentWidth, segmentHeight,segmentForward, float2(1, t), transformMatrix));

                    }
                    triStream.Append(GenerateGrassVertex(pos + float3(rand(pos*i),0,rand(pos*i+1)),
                    0, height,forward, float2(0.5, 1), transformationMatrix));

                    triStream.RestartStrip();
                }


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
