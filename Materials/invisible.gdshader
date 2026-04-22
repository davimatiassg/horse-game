shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(1.0,0.0,0.0,1.0);

uniform float dash_size = 0.04;
uniform float gap_size  = 0.03;

// espessura visual REAL da linha (independente do width do Line2D)
uniform float thinness = 0.05;

void fragment()
{
    //--------------------------------
    // Linha fina no centro
    //--------------------------------

    if (abs(UV.y - 0.5) > thinness)
        discard;


    //--------------------------------
    // Padrão pontilhado
    //--------------------------------

    float period = dash_size + gap_size;

    if (mod(UV.x, period) > dash_size)
        discard;


    COLOR = line_color;
}