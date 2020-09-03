Shader "AperiodicTiling/Standard"
{
	Properties
	{
		_PatternTex("Pattern texture", 2D) = "white" {}
		_TilesetTex("Tileset texture", 2D) = "white" {}
		_TilesetNormalTex("Tileset normal texture", 2D) = "bump" {}
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_LOD("Level of detail (LOD)", Range(0,1)) = 0.28
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
			sampler2D _TilesetNormalTex;

			struct Input
			{
				float2 uv_PatternTex;
			};

			float4 _PatternTex_TexelSize;
			float _LOD;

			fixed4 _Color;
			half _Glossiness;
			half _Metallic;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Align the current uv texel to the nearest centered texel.
				float2 alignedUV = IN.uv_PatternTex; 
				alignedUV -= IN.uv_PatternTex % (0.5 / float2(_PatternTex_TexelSize.z, _PatternTex_TexelSize.z)); 

				// Sample the pattern texture.
				fixed4 tile = tex2D(_PatternTex, alignedUV);

				// Get the tile's row and column index stored in the pattern texture as green and blue values respecitvely.
				uint row = tile.g * 255.0f;
				uint col = tile.b * 255.0f;

				// The amount of rows and columns in the tileset.
				float rowCount = 4;
				float colCount = 4;

				// Offset the uv texel in to sample the correct tile in the tileset.
				float2 scaledUV = IN.uv_PatternTex * _PatternTex_TexelSize.z;
				uint2 cell = floor(scaledUV);
				float2 offsetUV = scaledUV - cell;
				offsetUV += float2(col, row);
				offsetUV /= float2(colCount, rowCount);

				// Generate correct mipmaps.
				float2 derivativeUV = scaledUV * _LOD;
				float2 texDdx = ddx(derivativeUV);
				float2 texDdy = ddy(derivativeUV);

				// Sample the tileset texture and normal texture at the offset uv texel.
				fixed4 color = tex2Dgrad(_TilesetTex, offsetUV, texDdx, texDdy);
				fixed4 normal = tex2Dgrad(_TilesetNormalTex, offsetUV, texDdx, texDdy);

				// Apply the sample color to the surface output.
				o.Albedo = color.rgb * _Color;
				o.Normal = UnpackNormal(normal);
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = color.a;
			}
			ENDCG
		}
		FallBack "Diffuse"
}
