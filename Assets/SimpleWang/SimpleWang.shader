Shader "Custom/SimpleWang"
{
	Properties
	{
		_PatternTex("Pattern texture:", 2D) = "white" {}
		_TilesetTex("Tileset:", 2D) = "white" {}
		_RowCount("Number of rows:", Float) = 2
		_ColCount("Number of columns:", Float) = 4
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			sampler2D _PatternTex;
			sampler2D _TilesetTex;

			struct Input
			{
				float2 uv_PatternTex;
			};

			float4 _PatternTex_TexelSize;
			float _RowCount;
			float _ColCount;
			fixed4 _Color;
			half _Glossiness;
			half _Metallic;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Align the current uv texel to the nearest centered texel.
				float2 alignedUV = IN.uv_PatternTex; 
				alignedUV -= IN.uv_PatternTex % (0.5 / float2(_PatternTex_TexelSize.z, _PatternTex_TexelSize.z)); 

				// Sample the pattern texture.
				fixed4 pixel = tex2D(_PatternTex, alignedUV);

				// Get the tile's row and column index stored as red and green values respecitvely in the pattern texture.
				uint row = (_RowCount - 1) - pixel.r * 255.0f; // (flip the y-axis)
				uint col = pixel.g * 255.0f;

				// Sample the tileset texture at the offset uv texel.
				float2 scaledUV = IN.uv_PatternTex * _PatternTex_TexelSize.z;
				uint2 cell = floor(scaledUV);
				float2 offsetUV = scaledUV - cell;
				offsetUV += float2(col, row);
				offsetUV /= float2(_ColCount, _RowCount);
				pixel = tex2D(_TilesetTex, offsetUV);

				// Apply the sample color to the surface output.
				o.Albedo = pixel.rgb * _Color;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = pixel.a;
			}
			ENDCG
		}
		FallBack "Diffuse"
}
