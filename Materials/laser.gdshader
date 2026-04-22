shader_type canvas_item;

uniform sampler2D tex1;
uniform sampler2D tex2;
uniform sampler2D tex3;

uniform float frame_speed = 12.0;

// quantas repetições da textura por comprimento UV da linha
uniform float tile_count = 20.0;

uniform float glow_strength = 2.5;
uniform float rainbow_speed = 1.5;
uniform float pulse_speed = 4.0;

void fragment()
{
    //----------------------------------
    // UV tileado
    //----------------------------------

    vec2 tiled_uv = UV;

    // repete horizontalmente
    tiled_uv.x = fract(UV.x * tile_count);


    //----------------------------------
    // Alternância dos frames
    //----------------------------------

    float t = mod(floor(TIME * frame_speed), 3.0);

    vec4 spark;

    if (t < 1.0)
        spark = texture(tex1, tiled_uv);
    else if (t < 2.0)
        spark = texture(tex2, tiled_uv);
    else
        spark = texture(tex3, tiled_uv);


    //----------------------------------
    // Rainbow glow
    //----------------------------------

    vec3 rainbow = vec3(
        sin(TIME*rainbow_speed)+1.0,
        sin(TIME*rainbow_speed+2.094)+1.0,
        sin(TIME*rainbow_speed+4.188)+1.0
    ) * 0.5;


    float pulse =
        1.2 +
        sin(TIME*7.0 + UV.x*35.0)*0.4 +
        sin(TIME*17.0 + UV.x*80.0)*0.15;


    vec3 emission =
        spark.rgb *
        rainbow *
        glow_strength *
        pulse;

    COLOR.rgb = spark.rgb + emission;
    COLOR.a = spark.a;
}