Shader "Custom/SimpleWang"
{
	Properties
	{
		_DataTex("Data Texture:", 2D) = "white" {}
		_TilesetTex("Spritesheet:", 2D) = "white" {}
		_TileSize("Tile Size (pixels per side):", Float) = 256
		_RowCount("Number of rows:", Float) = 2
		_ColCount("Number of columns:", Float) = 4
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			sampler2D _DataTex;
			sampler2D _TilesetTex;

			struct Input
			{
				float2 uv_DataTex;
			};

			float4 _DataTex_TexelSize;
			float4 _TilesTex_TexelSize;
			float _RowCount;
			float _ColCount;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Align the current uv texel to the nearest centered texel.
				float2 alignedUV = IN.uv_DataTex; 
				alignedUV -= IN.uv_DataTex % (0.5 / float2(_DataTex_TexelSize.z, _DataTex_TexelSize.z)); 

				// Sample the data texture.
				fixed4 pixel = tex2D(_DataTex, alignedUV);

				// Get the tile's row and column index stored as red and green values respecitvely in the data texture.
				uint row = (_RowCount - 1) - pixel.r * 255.0f; // (flip the y-axis)
				uint col = pixel.g * 255.0f;

				// Sample the tileset texture at the offset uv texel.
				float2 scaledUV = IN.uv_DataTex * _DataTex_TexelSize.z;
				uint2 cell = floor(scaledUV);
				float2 offsetUV = scaledUV - cell;
				offsetUV += float2(col, row);
				offsetUV /= float2(_ColCount, _RowCount);
				pixel = tex2Dgrad(_TilesetTex, offsetUV, float2(0.0, 0.0), float2(0.0, 0.0));

				// Apply the sample color to the surface output.
				o.Albedo = pixel.rgb;
				o.Metallic = 0;
				o.Smoothness = 0;
				o.Alpha = 0;
			}
			ENDCG
		}
		FallBack "Diffuse"
}
