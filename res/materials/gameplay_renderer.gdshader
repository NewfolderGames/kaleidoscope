shader_type canvas_item;

uniform sampler2D game_texture : source_color, repeat_disable, filter_nearest;
uniform sampler2D sight_texture : source_color, repeat_disable, filter_nearest;
uniform sampler2D world_texture : source_color, repeat_disable, filter_nearest;
uniform float fog_of_war_color_multiplier = 0.8;

void fragment() {
	
	float sight_color_r = texture(sight_texture, SCREEN_UV).r;
	vec3 output_color = texture(game_texture, SCREEN_UV).rgb;
	
	if (sight_color_r < 1.00) {
		
		output_color = texture(world_texture, SCREEN_UV).rgb;
		output_color.r = output_color.r * fog_of_war_color_multiplier;
		output_color.g = output_color.g * fog_of_war_color_multiplier;
		output_color.b = output_color.b * fog_of_war_color_multiplier;
		
	}
	
	COLOR.rgb = output_color.rgb;
	
}
