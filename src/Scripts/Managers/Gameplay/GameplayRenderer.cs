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

	public override void _Process(double delta)
	{
		if (_gameViewport == null || _sightViewport == null || _worldViewport == null || _shaderMaterial == null) return;

		_shaderMaterial.SetShaderParameter("game_texture", _gameViewport.GetTexture());
		_shaderMaterial.SetShaderParameter("sight_texture", _sightViewport.GetTexture());
		_shaderMaterial.SetShaderParameter("world_texture", _worldViewport.GetTexture());
	}
}
