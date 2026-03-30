shader_type canvas_item;

uniform float flash_amount : hint_range(0.0, 1.0) = 0.0;
uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float outline_size = 1.0;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    float alpha = tex.a;

    vec2 size = TEXTURE_PIXEL_SIZE * outline_size;

    float outline = 0.0;

    outline += texture(TEXTURE, UV + vec2(size.x, 0)).a;
    outline += texture(TEXTURE, UV + vec2(-size.x, 0)).a;
    outline += texture(TEXTURE, UV + vec2(0, size.y)).a;
    outline += texture(TEXTURE, UV + vec2(0, -size.y)).a;


    if (alpha < 0.0001 && outline > 0.0) {
        COLOR = outline_color;
    } else {
		vec3 final_color = mix(tex.rgb, vec3(1.0), flash_amount);
        COLOR = vec4(final_color, tex.a);
    }
}