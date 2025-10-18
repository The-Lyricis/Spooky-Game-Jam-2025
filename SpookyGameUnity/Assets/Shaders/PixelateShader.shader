Shader "Custom/WhosLilaPixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Range(10, 1000)) = 100
        _PaletteTexture ("Palette Texture (LUT)", 2D) = "white" {}
        
        // Who's Lila 风格控制参数
        _ColorCount ("Color Count", Range(2, 32)) = 8 // 调色板颜色数量
        _Contrast ("Contrast", Range(0.5, 3.0)) = 1.5 // 对比度增强
        _Brightness ("Brightness", Range(-0.5, 0.5)) = 0.0 // 亮度调整
        _Saturation ("Saturation", Range(0.0, 2.0)) = 0.8 // 饱和度（Who's Lila偏低饱和）
        
        // 抖动效果（Dithering）- 复古风格关键
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.5
        _DitherScale ("Dither Scale", Range(1, 8)) = 4
        
        // CRT复古效果
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.2
        _ScanlineCount ("Scanline Count", Range(100, 1000)) = 240
        _NoiseAmount ("Noise Amount", Range(0, 0.2)) = 0.05
        _Vignette ("Vignette", Range(0, 1)) = 0.3
        
        // 颗粒效果
        _GrainSize ("Grain Size", Range(1, 4)) = 2
        _GrainAmount ("Grain Amount", Range(0, 0.3)) = 0.1
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

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

            sampler2D _MainTex;
            half _PixelSize;
            sampler2D _PaletteTexture;
            
            // Who's Lila 风格参数
            half _ColorCount;
            half _Contrast;
            half _Brightness;
            half _Saturation;
            
            // 抖动参数
            half _DitherStrength;
            half _DitherScale;
            
            // CRT效果参数
            half _ScanlineIntensity;
            half _ScanlineCount;
            half _NoiseAmount;
            half _Vignette;
            
            // 颗粒参数
            half _GrainSize;
            half _GrainAmount;

            // 顶点着色器
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // ============================================
            // 工具函数
            // ============================================
            
            // 伪随机函数
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            // Bayer矩阵抖动（经典像素风格）
            float bayer2(float2 pos)
            {
                float2x2 bayerMatrix = float2x2(0, 2, 3, 1);
                return bayerMatrix[pos.x % 2][pos.y % 2] / 4.0;
            }
            
            float bayer4(float2 pos)
            {
                float4x4 bayerMatrix = float4x4(
                    0, 8, 2, 10,
                    12, 4, 14, 6,
                    3, 11, 1, 9,
                    15, 7, 13, 5
                );
                int x = int(pos.x) % 4;
                int y = int(pos.y) % 4;
                return bayerMatrix[y][x] / 16.0;
            }
            
            // RGB转HSV
            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }
            
            // HSV转RGB
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            // ============================================
            // 像素化处理
            // ============================================
            float2 pixelate_uv(float2 uv)
            {
                float2 blockIndex = floor(uv * _PixelSize);
                float2 uv_pixelated = (blockIndex + 0.5) / _PixelSize;
                return uv_pixelated;
            }

            // ============================================
            // 色彩调整（Who's Lila风格）
            // ============================================
            float3 adjust_color(float3 color)
            {
                // 1. 亮度调整
                color += _Brightness;
                
                // 2. 对比度调整（Who's Lila的高对比度特征）
                color = (color - 0.5) * _Contrast + 0.5;
                
                // 3. 饱和度调整（Who's Lila偏低饱和度）
                float3 hsv = rgb2hsv(color);
                hsv.y *= _Saturation;
                color = hsv2rgb(hsv);
                
                return saturate(color);
            }

            // ============================================
            // 颜色量化（限制颜色数量）
            // ============================================
            float3 quantize_color(float3 color, float ditherValue)
            {
                // 应用抖动
                color += (ditherValue - 0.5) * _DitherStrength / _ColorCount;
                
                // 量化到指定颜色数量
                color = floor(color * _ColorCount) / _ColorCount;
                
                return saturate(color);
            }

            // ============================================
            // 调色板映射（使用LUT）
            // ============================================
            float3 map_to_palette(float3 color)
            {
                // 计算亮度作为LUT查找索引
                float gray = dot(color, float3(0.299, 0.587, 0.114));
                float u = saturate(gray);
                
                // 从调色板纹理采样
                float3 paletteColor = tex2D(_PaletteTexture, float2(u, 0.5)).rgb;
                
                return paletteColor;
            }

            // ============================================
            // CRT扫描线效果
            // ============================================
            float scanline(float2 uv)
            {
                float scanlineValue = sin(uv.y * _ScanlineCount * 3.14159);
                return 1.0 - _ScanlineIntensity * (1.0 - scanlineValue * 0.5 + 0.5);
            }

            // ============================================
            // 暗角效果
            // ============================================
            float vignette(float2 uv)
            {
                float2 center = uv - 0.5;
                float dist = length(center);
                return 1.0 - _Vignette * dist * dist * 2.0;
            }

            // ============================================
            // 噪点/颗粒效果
            // ============================================
            float3 add_noise(float3 color, float2 uv)
            {
                // 时间相关的噪点
                float noise = rand(uv + frac(_Time.y)) * 2.0 - 1.0;
                color += noise * _NoiseAmount;
                
                // 颗粒效果
                float2 grainUV = floor(uv * _ScreenParams.xy / _GrainSize);
                float grain = rand(grainUV) * 2.0 - 1.0;
                color += grain * _GrainAmount;
                
                return saturate(color);
            }

            // ============================================
            // 主片段着色器
            // ============================================
            float4 frag(v2f i) : SV_Target
            {
                // 1. 像素化UV
                float2 pixelated_uv = pixelate_uv(i.uv);
                
                // 2. 采样原始颜色
                float3 color = tex2D(_MainTex, pixelated_uv).rgb;
                
                // 3. Who's Lila风格：色彩调整（对比度、饱和度等）
                color = adjust_color(color);
                
                // 4. 计算抖动值（用于颜色量化）
                float2 ditherPos = i.uv * _ScreenParams.xy / _DitherScale;
                float ditherValue = bayer4(ditherPos);
                
                // 5. 颜色量化（限制颜色数量，创造Who's Lila的独特风格）
                color = quantize_color(color, ditherValue);
                
                // 6. 可选：应用调色板映射（如果需要特定调色板）
                // 取消注释下面这行以使用LUT调色板
                // color = map_to_palette(color);
                
                // 7. CRT效果：扫描线
                float scanlineMask = scanline(i.uv);
                color *= scanlineMask;
                
                // 8. 暗角效果
                float vignetteMask = vignette(i.uv);
                color *= vignetteMask;
                
                // 9. 添加噪点和颗粒（增强复古感）
                color = add_noise(color, pixelated_uv);
                
                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
