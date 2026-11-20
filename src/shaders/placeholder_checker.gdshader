// PLACEHOLDER (Task 1.2): two-tone checkerboard so movement is visible while the camera follows
// Kit. Replaced by real TileMapLayer ground in Task 1.5. One cell = one 64 px tile.
shader_type canvas_item;

uniform vec4 color_a : source_color = vec4(0.48, 0.70, 0.36, 1.0);
uniform vec4 color_b : source_color = vec4(0.43, 0.64, 0.32, 1.0);
uniform float cell_size = 64.0;

varying vec2 local_pos;

void vertex() {
	local_pos = VERTEX;
}

void fragment() {
	vec2 c = floor(local_pos / cell_size);
	COLOR = mod(c.x + c.y, 2.0) < 0.5 ? color_a : color_b;
}
