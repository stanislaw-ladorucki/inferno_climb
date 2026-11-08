shader_type canvas_item;

uniform float progress;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
	if (UV.y < 1.0 - progress) {
		tex.rgb *= 0.5;
	}
    COLOR = vec4(tex.rgb, tex.a);
}
