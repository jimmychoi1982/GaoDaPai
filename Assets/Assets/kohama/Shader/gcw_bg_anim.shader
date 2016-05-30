// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32891,y:32560,varname:node_3138,prsc:2|emission-5725-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32231,y:32550,ptovrint:False,ptlb:Base_Color,ptin:_Base_Color,varname:_Base_Color,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4717,x:31715,y:32703,ptovrint:True,ptlb:Star_tex,ptin:_Star_tex,varname:_Star_tex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1231-OUT;n:type:ShaderForge.SFN_Tex2d,id:6356,x:31742,y:32992,ptovrint:True,ptlb:sig_tex,ptin:_sig_tex,varname:_sig_tex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8218-OUT;n:type:ShaderForge.SFN_TexCoord,id:630,x:30910,y:33137,varname:node_630,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:5334,x:30896,y:32737,varname:node_5334,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:2866,x:30896,y:32576,varname:node_2866,prsc:0|A-5521-OUT,B-6737-TSL;n:type:ShaderForge.SFN_Time,id:6737,x:30604,y:32861,varname:Time_01,prsc:1;n:type:ShaderForge.SFN_Append,id:1231,x:31290,y:32660,varname:node_1231,prsc:1|A-1153-OUT,B-5334-V;n:type:ShaderForge.SFN_Add,id:1153,x:31086,y:32660,varname:node_1153,prsc:2|A-2866-OUT,B-5334-U;n:type:ShaderForge.SFN_ValueProperty,id:5521,x:30693,y:32576,ptovrint:True,ptlb:tex01_speed,ptin:_tex01_speed,varname:_tex01_speed,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8743,x:30898,y:33001,ptovrint:True,ptlb:tex02_U_sp,ptin:_tex02_U_sp,varname:_tex02_U_sp,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:8218,x:31526,y:32984,varname:node_8218,prsc:1|A-5694-OUT,B-3454-OUT;n:type:ShaderForge.SFN_Multiply,id:8480,x:31120,y:32952,varname:node_8480,prsc:2|A-6737-TSL,B-8743-OUT;n:type:ShaderForge.SFN_Add,id:5694,x:31322,y:32984,varname:node_5694,prsc:2|A-8480-OUT,B-630-U;n:type:ShaderForge.SFN_Multiply,id:335,x:31132,y:33261,varname:node_335,prsc:2|A-6737-TSL,B-2275-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2275,x:30910,y:33295,ptovrint:True,ptlb:tex02_V_sp,ptin:_tex02_V_sp,varname:_tex02_V_sp,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:3454,x:31322,y:33101,varname:node_3454,prsc:2|A-630-V,B-335-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8568,x:31975,y:33137,ptovrint:True,ptlb:StarEm_pow,ptin:_StarEm_pow,varname:_StarEm_pow,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:6046,x:31970,y:32971,varname:node_6046,prsc:2|A-4717-A,B-6356-R;n:type:ShaderForge.SFN_Add,id:7030,x:32418,y:32827,varname:last_tex,prsc:1|A-4717-RGB,B-8436-OUT;n:type:ShaderForge.SFN_Multiply,id:8436,x:32217,y:32996,varname:node_8436,prsc:2|A-6046-OUT,B-8568-OUT;n:type:ShaderForge.SFN_Multiply,id:5725,x:32615,y:32658,varname:node_5725,prsc:2|A-5329-OUT,B-7030-OUT;n:type:ShaderForge.SFN_Multiply,id:5329,x:32451,y:32454,varname:node_5329,prsc:2|A-2802-OUT,B-7241-RGB;n:type:ShaderForge.SFN_Vector1,id:2802,x:32229,y:32450,varname:node_2802,prsc:2,v1:2;proporder:7241-4717-6356-5521-8743-2275-8568;pass:END;sub:END;*/

Shader "Custom/env/gcw_bg_anim" {
    Properties {
        _Base_Color ("Base_Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Star_tex ("Star_tex", 2D) = "white" {}
        _sig_tex ("sig_tex", 2D) = "white" {}
        _tex01_speed ("tex01_speed", Float ) = 0
        _tex02_U_sp ("tex02_U_sp", Float ) = 0
        _tex02_V_sp ("tex02_V_sp", Float ) = 0
        _StarEm_pow ("StarEm_pow", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform half4 _Base_Color;
            uniform sampler2D _Star_tex; uniform float4 _Star_tex_ST;
            uniform sampler2D _sig_tex; uniform float4 _sig_tex_ST;
            uniform half _tex01_speed;
            uniform half _tex02_U_sp;
            uniform half _tex02_V_sp;
            uniform half _StarEm_pow;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                half4 Time_01 = _Time + _TimeEditor;
                half2 node_1231 = float2(((_tex01_speed*Time_01.r)+i.uv0.r),i.uv0.g);
                half4 _Star_tex_var = tex2D(_Star_tex,TRANSFORM_TEX(node_1231, _Star_tex));
                half2 node_8218 = float2(((Time_01.r*_tex02_U_sp)+i.uv0.r),(i.uv0.g+(Time_01.r*_tex02_V_sp)));
                half4 _sig_tex_var = tex2D(_sig_tex,TRANSFORM_TEX(node_8218, _sig_tex));
                float3 emissive = ((2.0*_Base_Color.rgb)*(_Star_tex_var.rgb+((_Star_tex_var.a*_sig_tex_var.r)*_StarEm_pow)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
