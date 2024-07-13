Shader "Custom/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TileSize ("Tile Size", Vector) = (1, 1, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _MainCoords[4];
            float _LeftCoords[4];
            float _RightCoords[4];
            float _TopCoords[4];
            float _BottomCoords[4];
            float _NeighborExists[8];
            float _IsAirTile;
            float2 _TileSize;

            fixed4 frag (v2f i) : SV_Target
            {
                float radius = 0.3;
                float dist = 1;
                float dist2 = 1;
                i.uv.x = (i.uv.x - _MainCoords[0]) / _MainCoords[2];
                i.uv.y = (i.uv.y - _MainCoords[1]) / _MainCoords[3];
                float2 uvInvX = float2(1 - i.uv.x, i.uv.y);
                float2 uvInvY = float2(i.uv.x, 1 - i.uv.y);
                float2 uvMain = float2(lerp(_MainCoords[0], _MainCoords[0] + _MainCoords[2], i.uv.x), lerp(_MainCoords[1], _MainCoords[1] + _MainCoords[3], i.uv.y));
                float2 uvLeft = float2(lerp(_LeftCoords[0], _LeftCoords[0] + _LeftCoords[2], uvInvX.x), lerp(_LeftCoords[1], _LeftCoords[1] + _LeftCoords[3], uvInvX.y));
                float2 uvRight = float2(lerp(_RightCoords[0], _RightCoords[0] + _RightCoords[2], uvInvX.x), lerp(_RightCoords[1], _RightCoords[1] + _RightCoords[3], uvInvX.y));
                float2 uvTop = float2(lerp(_TopCoords[0], _TopCoords[0] + _TopCoords[2], uvInvY.x), lerp(_TopCoords[1], _TopCoords[1] + _TopCoords[3], uvInvY.y));
                float2 uvBottom = float2(lerp(_BottomCoords[0], _BottomCoords[0] + _BottomCoords[2], uvInvY.x), lerp(_BottomCoords[1], _BottomCoords[1] + _BottomCoords[3], uvInvY.y));
                fixed4 mainTile = tex2D(_MainTex, uvMain);
                fixed4 leftTile = tex2D(_MainTex, uvLeft);
                fixed4 rightTile = tex2D(_MainTex, uvRight);
                fixed4 topTile = tex2D(_MainTex, uvTop);
                fixed4 bottomTile = tex2D(_MainTex, uvBottom);
                if (_IsAirTile) {
                    fixed4 tile1;
                    fixed4 tile2;
                    float dt1;
                    float dt2;
                    if (_NeighborExists[0] > 0 && _NeighborExists[2] > 0) {
                        float L = length(float2(i.uv.x - radius, i.uv.y - 1 + radius));
                        if (L < dist) {
                            dist = L;
                            tile1 = leftTile;
                            tile2 = topTile;
                            dt1 = i.uv.x;
                            dt2 = 1 - i.uv.y;
                        }
                        dist2 = min(dist2, i.uv.x + 1 - i.uv.y);
                    }
                    if (_NeighborExists[2] > 0 && _NeighborExists[1] > 0) {
                        float L = length(float2(i.uv.x - 1 + radius, i.uv.y - 1 + radius));
                        if (L < dist) {
                            dist = L;
                            tile1 = topTile;
                            tile2 = rightTile;
                            dt1 = 1 - i.uv.y;
                            dt2 = 1 - i.uv.x;
                        }
                        dist2 = min(dist2, 1 - i.uv.x + 1 - i.uv.y);
                    }
                    if (_NeighborExists[1] > 0 && _NeighborExists[3] > 0) {
                        float L = length(float2(i.uv.x - 1 + radius, i.uv.y - radius));
                        if (L < dist) {
                            dist = L;
                            tile1 = rightTile;
                            tile2 = bottomTile;
                            dt1 = 1 - i.uv.x;
                            dt2 = i.uv.y;
                        }
                        dist2 = min(dist2, 1 - i.uv.x + i.uv.y);
                    }
                    if (_NeighborExists[3] > 0 && _NeighborExists[0] > 0) {
                        float L = length(float2(i.uv.x - radius, i.uv.y - radius));
                        if (L < dist) {
                            dist = L;
                            tile1 = bottomTile;
                            tile2 = leftTile;
                            dt1 = i.uv.y;
                            dt2 = i.uv.x;
                        }
                        dist2 = min(dist2, i.uv.x + i.uv.y);
                    }
                    clip(-max(radius - dist, dist2 - radius));
                    return (tile1 * dt2 + tile2 * dt1) / (dt1 + dt2);
                }
                else {
                    if (_NeighborExists[0] < 1 && _NeighborExists[2] < 1 && _NeighborExists[7] < 1) {
                        dist = min(dist, length(float2(i.uv.x - radius, i.uv.y - 1 + radius)));
                        dist2 = min(dist2, i.uv.x + 1 - i.uv.y);
                    }
                    if (_NeighborExists[2] < 1 && _NeighborExists[1] < 1 && _NeighborExists[6] < 1) {
                        dist = min(dist, length(float2(i.uv.x - 1 + radius, i.uv.y - 1 + radius)));
                        dist2 = min(dist2, 1 - i.uv.x + 1 - i.uv.y);
                    }
                    if (_NeighborExists[1] < 1 && _NeighborExists[3] < 1 && _NeighborExists[5] < 1) {
                        dist = min(dist, length(float2(i.uv.x - 1 + radius, i.uv.y - radius)));
                        dist2 = min(dist2, 1 - i.uv.x + i.uv.y);
                    }
                    if (_NeighborExists[3] < 1 && _NeighborExists[0] < 1 && _NeighborExists[4] < 1) {
                        dist = min(dist, length(float2(i.uv.x - radius, i.uv.y - radius)));
                        dist2 = min(dist2, i.uv.x + i.uv.y);
                    }
                    clip(max(radius - dist, dist2 - radius));
                    float total = 0.5 + max(0, 0.5 - i.uv.x) * _NeighborExists[0] +
                                max(0, i.uv.x - 0.5) * _NeighborExists[1] +
                                max(0, i.uv.y - 0.5) * _NeighborExists[2] +
                                max(0, 0.5 - i.uv.y) * _NeighborExists[3];
                    return (0.5 * mainTile + leftTile * max(0, 0.5 - i.uv.x) * _NeighborExists[0] +
                                rightTile * max(0, i.uv.x - 0.5) * _NeighborExists[1] +
                                topTile * max(0, i.uv.y - 0.5) * _NeighborExists[2] +
                                bottomTile * max(0, 0.5 - i.uv.y) * _NeighborExists[3]) / total;
                }
            }
            ENDCG
        }
    }
}
