shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float outline_size = 1.0;

void fragment() {
    vec4 tex = texture(screen_texture, SCREEN_UV, 0.0);
	
	
    float alpha = tex.a;

    vec2 size = TEXTURE_PIXEL_SIZE * outline_size;

    float outline = 0.0;

    outline += texture(screen_texture, SCREEN_UV + vec2(size.x, 0)).a;
    outline += texture(screen_texture, SCREEN_UV + vec2(-size.x, 0)).a;
    outline += texture(screen_texture, SCREEN_UV + vec2(0, size.y)).a;
    outline += texture(screen_texture, SCREEN_UV + vec2(0, -size.y)).a;


    if (abs(alpha) < 0.0001 && outline > 0.0) {
        COLOR = outline_color;
    } else {
        COLOR = tex;
    }
}