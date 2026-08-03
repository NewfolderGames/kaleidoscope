using System.Collections.Generic;
using Godot;

namespace Kaleidoscope.Scripts.Effects;

public partial class ControllableLine2D : Line2D
{
	[Export] private int _maxPoints = 0;
	[Export] public bool KeepPushing;
	[Export] public Node2D Target;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Target == null) return;

		if (!KeepPushing && Points.Length > 0)
		{
			RemovePoint(Points.Length - 1);
			if (Points.Length > _maxPoints) RemovePoint(Points.Length - 1);
		}
		AddPoint(Target.GetGlobalPosition() - GetGlobalPosition(), 0);
	}
}
