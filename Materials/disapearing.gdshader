shader_type canvas_item;

uniform sampler2D mask_texture;
uniform vec4 base_color : source_color;

void fragment() {
    float mask = texture(mask_texture, UV).a;

    vec4 color = COLOR;
    color.a *= 1.0-mask;

    COLOR = color;
}