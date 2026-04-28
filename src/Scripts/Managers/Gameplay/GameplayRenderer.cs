using Godot;

namespace Kaleidoscope.Scripts.Managers.Gameplay;

public partial class GameplayRenderer : ColorRect
{
	[ExportCategory("Viewports")]
	[Export] private Viewport _gameViewport;
	[Export] private Viewport _sightViewport;
	[Export] private Viewport _worldViewport;
	
	[ExportCategory("Fog of War")]
	[Export] private bool _isFogOfWarEnabled;
	[Export] private float _fogOfWarColorMultiplier = 0.8f;
	
	[ExportCategory("Materials")]
	[Export] private ShaderMaterial _shaderMaterial;

	static readonly StringName GAME_TEXTURE = new("game_texture");
	static readonly StringName SIGHT_TEXTURE = new("sight_texture");
	static readonly StringName WORLD_TEXTURE = new("world_texture");
	
	static readonly StringName IS_FOG_OF_WAR_ENABLED = new("is_fog_of_war_enabled");
	static readonly StringName FOG_OF_WAR_COLOR_MULTIPLIER = new("fog_of_war_color_multiplier");
	
	public override void _Ready()
	{
		_shaderMaterial ??= GetMaterial() as ShaderMaterial;
	}
	
	public override void _Process(double delta)
	{
		if (_shaderMaterial == null) return;
		
		_shaderMaterial.SetShaderParameter(GAME_TEXTURE, _gameViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(SIGHT_TEXTURE, _sightViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(WORLD_TEXTURE, _worldViewport.GetTexture());
		
		_shaderMaterial.SetShaderParameter(IS_FOG_OF_WAR_ENABLED, _isFogOfWarEnabled);
		_shaderMaterial.SetShaderParameter(FOG_OF_WAR_COLOR_MULTIPLIER, _fogOfWarColorMultiplier);
	}
}
