// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestLight"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		//_SpecularTint ("Specular", Color) = (0.5, 0.5, 0.5)
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
	}
    SubShader
    {
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM

			//#include "UnityCG.cginc"

			//#include "UnityStandardBRDF.cginc"
			//#include "UnityStandardUtils.cginc"

			#include "UnityPBSLighting.cginc"

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Tint;
			//float4 _SpecularTint;
			float _Metallic;
			float _Smoothness;

			struct VertexData
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			Interpolators MyVertexProgram(VertexData v)
			{
				//Interpolators i;
				//i.position = UnityObjectToClipPos(v.position);
				//i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//i.normal = v.normal;

				//i.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));

				//i.normal = mul((float3x3)unity_ObjectToWorld, v.normal);

				//i.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				//i.normal = normalize(i.normal);

				//i.normal = mul(transpose((float3x3)unity_WorldToObject), v.normal);
				//i.normal = normalize(i.normal);

				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return i;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_Target
			{
				//i.normal = normalize(i.normal);
				//return float4(i.normal * 0.5 + 0.5, 1);

				i.normal = normalize(i.normal);

				//return max(0, dot(float3(0, 1, 0), i.normal));

				//return saturate(dot(float3(0, 1, 0), i.normal));

				//return DotClamped(float3(0, 1, 0), i.normal);

				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
				//albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b));

				float3 specularTint;// = albedo * _Metallic;
				float oneMinusReflectivity;// = 1 - _Metallic;
				//albedo = EnergyConservationBetweenDiffuseAndSpecular(albedo, specularTint, oneMinusReflectivity);
				//albedo *= oneMinusReflectivity;

				albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

				float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);
				//return float4(diffuse, 1);

				//float3 reflectionDir = reflect(-lightDir, i.normal);
				float3 halfVector = normalize(lightDir + viewDir);
				float3 specular = specularTint * lightColor * pow(DotClamped(halfVector, i.normal), _Smoothness * 100);
				//return float4(diffuse + specular, 1);

				UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);

				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				return UNITY_BRDF_PBS(albedo, specularTint, oneMinusReflectivity, _Smoothness, i.normal, viewDir, light, indirectLight);
			}

			ENDCG
		}
	}
}
