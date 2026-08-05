
using Godot;
using Godot.Collections;

namespace Kaleidoscope.Scripts.Managers.Gameplay.Game;

public partial class GameManager : Node2D
{
	[ExportCategory("Cameras")]
	[Export] private Camera2D _gameCamera;
	[Export] private Camera2D _sightWorkerCamera;
	[Export] private Camera2D _worldWorkerCamera;

	[ExportCategory("Camera - States")]
	[Export] private float _cameraShake;
	[Export] private float _cameraShakeDuration;
	[Export] private float _cameraShakeMax = 10f;

	[ExportCategory("Effects")]
	[Export] private Dictionary<string, GpuParticles2D> _effects = new();

	public override void _Ready()
	{
		base._Ready();

		if (_sightWorkerCamera != null) GetNode<RemoteTransform2D>("GameCameraRoot/GameCamera/SightWorkerRemoteTransform").RemotePath = _sightWorkerCamera.GetPath();
		if (_worldWorkerCamera != null) GetNode<RemoteTransform2D>("GameCameraRoot/GameCamera/WorldWorkerRemoteTransform").RemotePath = _worldWorkerCamera.GetPath();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_cameraShakeDuration > 0f)
		{
			_cameraShakeDuration -= (float)delta;
			if (_cameraShakeDuration <= 0f) _cameraShakeDuration = 0f;
		}

		_cameraShake = Mathf.Lerp(0f, _cameraShake, _cameraShakeDuration);
		_gameCamera.Position = new Vector2(Mathf.Cos(Mathf.Pi * 2 * GD.Randf()) * _cameraShake, Mathf.Sin(Mathf.Pi * 2 * GD.Randf()) * _cameraShake);
	}

	public void AddCameraShake(float shake, float duration)
	{
		_cameraShake = Mathf.Clamp(_cameraShake + shake, 0f, _cameraShakeMax);
		_cameraShakeDuration =  Mathf.Clamp(_cameraShakeDuration + duration, 0f, 1f);
	}

	public void SpawnEffectAt(string effectName, Vector2 position)
	{
		if (!_effects.TryGetValue(effectName, out var effect)) return;
		var duplicate = effect.Duplicate() as GpuParticles2D;
		if (duplicate == null) return;
		effect.AddSibling(duplicate);
		duplicate.GlobalPosition = position;
		duplicate.Restart();
		duplicate.Emitting = true;
	}
}
