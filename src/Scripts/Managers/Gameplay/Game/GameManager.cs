
using Godot;

namespace Kaleidoscope.Scripts.Managers.Gameplay.Game;

public partial class GameManager : Node2D
{
	[ExportCategory("Cameras")]
	[Export] private Camera2D _sightWorkerCamera;
	[Export] private Camera2D _worldWorkerCamera;

	public override void _Ready()
	{
		base._Ready();
		if (_sightWorkerCamera != null) GetNode<RemoteTransform2D>("GameCameraRoot/GameCamera/SightWorkerRemoteTransform").RemotePath = _sightWorkerCamera.GetPath();
		if (_worldWorkerCamera != null) GetNode<RemoteTransform2D>("GameCameraRoot/GameCamera/WorldWorkerRemoteTransform").RemotePath = _worldWorkerCamera.GetPath();
	}
}
