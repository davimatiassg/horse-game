shader_type canvas_item;

uniform float intensity : hint_range(0.0, 5.0) = 1.5;
uniform float threshold : hint_range(0.0, 1.0) = 0.7;
uniform float blur_size : hint_range(0.0, 10.0) = 2.0;

void fragment() {
    vec4 base = texture(TEXTURE, UV);

    // calcula brilho
    float brightness = max(max(base.r, base.g), base.b);

    // só partes brilhantes
    vec4 glow = brightness > threshold ? base : vec4(0.0);

    // blur simples ao redor do sprite
    vec4 blur = vec4(0.0);
    int samples = 6;

    for (int x = -samples; x <= samples; x++) {
        for (int y = -samples; y <= samples; y++) {
            vec2 offset = vec2(float(x), float(y)) * blur_size / 512.0;
            blur += texture(TEXTURE, UV + offset);
        }
    }

    blur /= float((samples * 2 + 1) * (samples * 2 + 1));

    // mistura
    COLOR = base + blur * intensity * step(threshold, brightness);
}