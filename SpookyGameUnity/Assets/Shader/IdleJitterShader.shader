Shader "Custom/IdleJitterShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Vertex Jitter)]
        _JitterStrengthX ("Jitter Strength X", Range(0, 0.1)) = 0.01
        _JitterStrengthY ("Jitter Strength Y", Range(0, 0.1)) = 0.01
        _JitterFrequencyX ("Jitter Frequency X", Range(1, 50)) = 10
        _JitterFrequencyY ("Jitter Frequency Y", Range(1, 50)) = 12
        _JitterSpeed ("Jitter Speed", Range(0.1, 10)) = 1
        
        [Header(UV Jitter - Optional)]
        _UVJitterStrength ("UV Jitter Strength", Range(0, 0.05)) = 0.005
        _UVJitterFrequency ("UV Jitter Frequency", Range(1, 30)) = 15
        
        [Header(Color Jitter - Optional)]
        _ColorJitterStrength ("Color Jitter Strength", Range(0, 0.2)) = 0.02
        _ColorJitterSpeed ("Color Jitter Speed", Range(0.5, 5)) = 2
        
        [Header(Render Settings)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 10
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
        [Toggle] _ZWrite ("ZWrite", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull [_Cull]
        Lighting Off
        ZWrite [_ZWrite]
        Blend [_SrcBlend] [_DstBlend]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            // 顶点抖动参数
            half _JitterStrengthX;
            half _JitterStrengthY;
            half _JitterFrequencyX;
            half _JitterFrequencyY;
            half _JitterSpeed;
            
            // UV抖动参数
            half _UVJitterStrength;
            half _UVJitterFrequency;
            
            // 颜色抖动参数
            half _ColorJitterStrength;
            half _ColorJitterSpeed;
            
            // ============================================
            // 哈希函数 - 生成伪随机值
            // ============================================
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }
            
            // ============================================
            // 噪声函数 - 更平滑的随机值
            // ============================================
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f); // 平滑插值
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // ============================================
            // 顶点着色器
            // ============================================
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // 获取当前时间
                float time = _Time.y * _JitterSpeed;
                
                // 计算种子值 - 基于顶点原始位置
                // 这确保每个顶点有不同的抖动相位
                float seed = v.vertex.x * 123.456 + v.vertex.y * 789.012;
                
                // 计算X方向位移
                // 使用sin函数 + 种子 + 时间
                float displX = _JitterStrengthX * sin(_JitterFrequencyX * time + seed);
                
                // 计算Y方向位移
                // 使用cos函数确保X和Y方向不完全同步
                float displY = _JitterStrengthY * cos(_JitterFrequencyY * time + seed * 1.5);
                
                // 应用位移到顶点
                v.vertex.xy += float2(displX, displY);
                
                // 可选：添加更复杂的噪声位移
                // 使用噪声函数创造更有机的抖动
                float noiseX = noise(float2(time * 0.5 + v.vertex.x, v.vertex.y)) - 0.5;
                float noiseY = noise(float2(v.vertex.x, time * 0.5 + v.vertex.y)) - 0.5;
                v.vertex.xy += float2(noiseX, noiseY) * _JitterStrengthX * 0.3;
                
                // 转换到裁剪空间
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // 传递纹理坐标和颜色
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                
                return o;
            }
            
            // ============================================
            // 片段着色器
            // ============================================
            fixed4 frag(v2f i) : SV_Target
            {
                // UV抖动效果（可选）
                float2 uv = i.texcoord;
                
                if (_UVJitterStrength > 0.001)
                {
                    float time = _Time.y * 2.0;
                    
                    // 基于噪声的UV偏移
                    float uvOffsetX = (noise(float2(time * _UVJitterFrequency, uv.y * 10.0)) - 0.5) * _UVJitterStrength;
                    float uvOffsetY = (noise(float2(uv.x * 10.0, time * _UVJitterFrequency)) - 0.5) * _UVJitterStrength;
                    
                    uv += float2(uvOffsetX, uvOffsetY);
                }
                
                // 采样纹理
                fixed4 texColor = tex2D(_MainTex, uv);
                fixed4 finalColor = texColor * i.color;
                
                // 颜色抖动效果（可选）- 模拟老旧屏幕或恐怖效果
                if (_ColorJitterStrength > 0.001)
                {
                    float time = _Time.y * _ColorJitterSpeed;
                    
                    // 轻微的RGB通道偏移
                    float colorShift = sin(time * 10.0 + uv.x * 5.0) * _ColorJitterStrength;
                    finalColor.r += colorShift;
                    finalColor.g += sin(time * 12.0 + uv.y * 5.0) * _ColorJitterStrength;
                    finalColor.b -= colorShift * 0.5;
                    
                    // 整体亮度抖动
                    float brightness = 1.0 + sin(time * 20.0) * _ColorJitterStrength * 0.5;
                    finalColor.rgb *= brightness;
                }
                
                // 预乘Alpha
                finalColor.rgb *= finalColor.a;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Sprites/Default"
}

