shader_type canvas_item;

global uniform sampler2D background_gradient;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
	vec4 tint = texture(background_gradient, SCREEN_UV);
    COLOR = vec4(tex.rgb * tint.rgb, tex.a);
}
