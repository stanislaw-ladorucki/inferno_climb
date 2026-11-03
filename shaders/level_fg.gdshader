shader_type canvas_item;

global uniform sampler2D foreground_gradient;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
	vec4 tint = texture(foreground_gradient, SCREEN_UV);
    COLOR = vec4(tex.rgb * tint.rgb, tex.a);
}
