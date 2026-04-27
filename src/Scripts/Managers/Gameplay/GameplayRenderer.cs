using Godot;

namespace Kaleidoscope.Scripts.Managers.Gameplay;

public partial class GameplayRenderer : ColorRect
{
	[ExportCategory("Viewports")]
	[Export] private Viewport _gameViewport;
	[Export] private Viewport _sightViewport;
	[Export] private Viewport _worldViewport;
	[Export] private ShaderMaterial _shaderMaterial;

	public override void _Ready()
	{
		// _gameViewport = GetViewport();
		_shaderMaterial = GetMaterial() as ShaderMaterial;
	}


    
    static readonly StringName GAME_TEXTURE = new StringName("game_texture);
    static readonly StringName SIGHT_TEXTURE = new StringName("sight_texture);
	static readonly StringName WORLD_TEXTURE = new StringName("world_texture);
	
	public override void _Process(double delta)
	{
		if (_gameViewport == null || _sightViewport == null || _worldViewport == null || _shaderMaterial == null) return;

		_shaderMaterial.SetShaderParameter(GAME_TEXTURE, _gameViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(SIGHT_TEXTURE, _sightViewport.GetTexture());
		_shaderMaterial.SetShaderParameter(WORLD_TEXTURE, _worldViewport.GetTexture());
	}
}
