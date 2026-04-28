using Godot;

namespace Kaleidoscope.Scripts.Managers.Gameplay;

public partial class GameplayRenderer : ColorRect
{
	[ExportCategory("Viewports")]
	[Export] private Viewport _gameViewport;
	[Export] private Viewport _sightViewport;
	[Export] private Viewport _worldViewport;
	[Export] private ShaderMaterial _shaderMaterial;

	static readonly StringName GAME_TEXTURE = new("game_texture");
	static readonly StringName SIGHT_TEXTURE = new("sight_texture");
	static readonly StringName WORLD_TEXTURE = new("world_texture");
	
	public override void _Ready()
	{
		_shaderMaterial = GetMaterial() as ShaderMaterial;
	}
	
	public override void _Process(double delta)
	{
		if (_gameViewport == null || _sightViewport == null || _worldViewport == null || _shaderMaterial == null) return;

		_shaderMaterial.SetShaderParameter(GAME_TEXTURE, _gameViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(SIGHT_TEXTURE, _sightViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(WORLD_TEXTURE, _worldViewport.GetTexture());
	}
}
