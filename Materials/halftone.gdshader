shader_type spatial;
render_mode unshaded, blend_mix, cull_back, depth_draw_opaque;

/**
 * Maximum number of dots in one row / column.
 */
uniform int max_dots = 100;

/**
 * Bigger number means bigger dots.
 */
uniform float dot_size = 1.0;

/**
 * Brightness multiplier.
 */
uniform float brightness_mult = 1.0;

/**
 * Invert colors.
 */
uniform bool invert = false;

/**
 * Colors
 */
uniform vec4 primary_color : source_color = vec4(0.1, 0.1, 0.2, 1.0);
uniform vec4 secondary_color : source_color = vec4(0.9, 0.9, 0.8, 1.0);

/**
 * Texture input (equivalent to TEXTURE)
 */
uniform sampler2D albedo_texture : source_color;


// RGB → HSV
vec3 rgb2hsv(vec3 c) {
	vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
	vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y)/(6.0*d + e)), d/(q.x + e), q.x);
}

void fragment() {
	float f_max_dots = float(max_dots);

	// Equivalente ao TEXTURE_PIXEL_SIZE
	vec2 tex_size = vec2(textureSize(albedo_texture, 0));
	vec2 tex_pixel_size = 1.0 / tex_size;

	vec2 ratio = vec2(1.0, tex_pixel_size.x / tex_pixel_size.y);

	vec2 pixelated_uv = floor(UV * f_max_dots * ratio) / (f_max_dots * ratio);

	float dots = length(fract(UV * f_max_dots * ratio) - vec2(0.5)) / dot_size;

	dots = mix(dots, 1.0 - dots, float(invert));

	float brightness = rgb2hsv(texture(albedo_texture, pixelated_uv).rgb).z;

	dots += brightness * brightness_mult;
	dots = pow(dots, 5.0);
	dots = clamp(dots, 0.0, 1.0);

	vec4 dots_color = mix(primary_color, secondary_color, dots);

	float alpha = texture(albedo_texture, UV).a;
	dots_color.a *= alpha;

	ALBEDO = dots_color.rgb;
	ALPHA = dots_color.a;
}