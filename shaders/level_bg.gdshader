shader_type canvas_item;

global uniform sampler2D background_gradient;
global uniform sampler2D background_gradient_mix;
global uniform float mix_ratio;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
	vec4 tint = mix(texture(background_gradient, SCREEN_UV), texture(background_gradient_mix, SCREEN_UV), mix_ratio);
    COLOR = vec4(tex.rgb * tint.rgb, tex.a);
}
