shader_type canvas_item;

// Converts RGB to HSV color space
vec3 rgb_to_hsv(vec3 color_rgb) {
    vec4 k = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(color_rgb.bg, k.wz), vec4(color_rgb.gb, k.xy), step(color_rgb.b, color_rgb.g));
    vec4 q = mix(vec4(p.xyw, color_rgb.r), vec4(color_rgb.r, p.yzx), step(p.x, color_rgb.r));

    float d = q.x - min(q.w, q.y);
    float e = 1e-10;

    return vec3(
        abs(q.z + (q.w - q.y) / (6.0 * d + e)),
        d / (q.x + e),
        q.x
    );
}

// Converts HSV back to RGB
vec3 hsv_to_rgb(vec3 color_hsv) {
    vec3 rgb = clamp(
        abs(mod(color_hsv.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0,
        0.0,
        1.0
    );
    return color_hsv.z * mix(vec3(1.0), rgb, color_hsv.y);
}
void fragment() {
    vec4 tex = texture(TEXTURE, UV);
	vec3 tex_hsv = rgb_to_hsv(tex.rgb);
	vec3 tinted_hsv = rgb_to_hsv(mix(vec3(1.0, 0.0, 1.0), vec3(0.0, 0.0, 1.0), SCREEN_UV.y));
    COLOR = vec4(hsv_to_rgb(vec3(tinted_hsv.x, tinted_hsv.y, tex_hsv.z)), tex.a);
}
