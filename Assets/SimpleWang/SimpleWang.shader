Shader "Custom/SimpleWang"
{
	Properties
	{
		_DataTex("Data Texture:", 2D) = "white" {}
		_TilesetTex("Spritesheet:", 2D) = "white" {}
		_TileSize("Tile Size (pixels per side):", Float) = 256
		_TileCount("Amount of tiles in tileset:", Float) = 8
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
				float2 uv_TilesetTex;
			};

			float4 _DataTex_TexelSize;
			float4 _TilesTex_TexelSize;
			float _TileSize;
			float _TileCount;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// gets a pixel from the data texture associated with the current cell
				fixed4 pixel = tex2D(_DataTex, IN.uv_DataTex);

				// get the sprite's index from the data texture
				uint index = pixel.r * 255.0f;
				float2 scaledUV = IN.uv_DataTex * _DataTex_TexelSize.z;
				uint2 cell = floor(scaledUV);
				float2 offsetUV = index + scaledUV - cell;
				offsetUV.x /= _TileCount;

				pixel = tex2D(_TilesetTex, offsetUV);

				o.Albedo = pixel.rgb;
				o.Metallic = 0;
				o.Smoothness = 0;
				o.Alpha = 0;
			}
			ENDCG
		}
		FallBack "Diffuse"
}
