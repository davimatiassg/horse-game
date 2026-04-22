shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(0.0, 0.8, 1.0, 1.0);
uniform float speed = 10.0;
uniform float amplitude = 0.1;
uniform float frequency = 5.0;

void fragment() {
    // Cria um efeito de onda baseado no tempo e na posição UV
    float wave = sin(UV.x * frequency + TIME * speed) * amplitude;
    float dist = abs(UV.y - 0.5 + wave);
    
    // Define a espessura do "raio"
    float mask = smoothstep(0.1, 0.0, dist);
    
    COLOR = line_color;
    COLOR.a *= mask;
}