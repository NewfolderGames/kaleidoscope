using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class UnitBodyCollision : Area2D
{
	[Export] protected Unit Unit;

	public string Team => Unit.Team;
}
