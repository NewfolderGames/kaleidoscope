using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class UnitWeaponCollision : Area2D
{
	[Export] protected Unit Unit;
	[Export] protected Array<CollisionShape2D> SweetSpotCollisionShapes;

	public string Team => Unit.Team;
	public string AttackUuid => Unit.AttackUuid;
	public bool IsAttackCollisionSweetSpotActive => Unit.IsAttackCollisionSweetSpotActive;

	public override void _Ready()
	{
		base._Ready();
		Unit = GetNode("../..") as Unit;
	}

	public void AttackSequenceHit()
	{
		Unit.AttackSequenceHit();
	}
}
