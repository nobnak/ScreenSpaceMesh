Shader "Custom/Grid" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _Color;
			
			struct appdata {
				float4 vertex : POSITION;
			};
			struct vs2ps {
				float4 vertex : POSITION;
			};
			
			vs2ps vert(appdata IN) {
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				return o;
			}
			
			float4 frag(vs2ps IN) : COLOR {
				return _Color;
			}
			ENDCG
		}
	} 
}
