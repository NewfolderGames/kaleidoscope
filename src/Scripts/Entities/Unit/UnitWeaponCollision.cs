using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class UnitWeaponCollision : Area2D
{
    [Export] protected Unit Unit;

    public string Team => Unit.Team;

    public override void _Ready()
    {
        base._Ready();
        GD.Print(Unit);  // This should be printed, but it doesn't???
    }
}
