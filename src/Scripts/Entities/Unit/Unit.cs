using System;
using System.Collections.Generic;
using Godot;
using Kaleidoscope.Core.Resources;
using Kaleidoscope.Core.System.Input;
using Kaleidoscope.Scripts.Managers.Gameplay.Game;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class Unit : CharacterBody2D
{
	[ExportCategory("Unit")]
	[Export] public string Team { get; protected set;  }

	[ExportCategory("Position")]
	[Export] protected Node2D TransformRoot;
	[Export] protected Node2D TransformCenterRoot;
	[Export] protected Node2D TransformBodyRoot;
	[Export] protected Node2D TransformHandRoot;
	[Export] protected float TransformHandRootRangeX = 24f;
	[Export] protected float TransformHandRootRangeY = 12f;
	[Export] protected Node2D TransformHandOffset;

	[ExportCategory("Health")]
	[Export] protected long Health = 10;
	[Export] protected long HealthMax = 10;
	[Export] protected long Shield;
	[Export] protected long ShieldMax;

	[ExportCategory("Render")]
	[Export] protected Node2D RenderRoot;
	[Export] protected Node2D RenderForegroundRoot;
	[Export] protected Node2D RenderBodyRoot;

	[ExportCategory("Render - Animation")]
	[Export] protected AnimationTree RenderAnimationTree;
	[Export] protected AnimationPlayer RenderAnimationPlayer;
	[Export] protected float RenderAnimationMovingSpeedMultiplier = 1f;
	protected AnimationNodeStateMachinePlayback RenderAnimationTreeWeaponStateMachinePlayback;

	[ExportCategory("Render - Effects")]
	[Export] protected Color RenderEffectHitColor = Color.Color8(255, 255, 255);
	[Export] protected int RenderEffectHitDurationLeft;
	[Export] protected int RenderEffectHitDuration = 5;

	[ExportCategory("Input")]
	[Export] protected bool AllowInput;
	protected readonly InputBuffer InputBuffer = new();

	[ExportCategory("Movement")]
	[Export] private Vector2 _movement;
	[Export] protected Vector2 MovementPrev;
	[Export] protected Vector2 MovementNext;
	[Export] protected Vector2 MovementInput;
	[Export] protected Vector2 MovementDesired;
	[Export] protected float MovementSpeedBase = 5f;
	[Export] protected float MovementSpeed = 5f;
	protected bool IsMoving => MovementDesired != Vector2.Zero;
	protected bool IsMovementInputting => MovementInput != Vector2.Zero;

	[ExportCategory("Weapon")]
	[Export] protected Weapon Weapon;
	[Export] protected float WeaponDamageMultiplierBase = 1f;
	[Export] protected float WeaponDamageSweetSpotMultiplierBase = 1f;

	[ExportCategory("Attack")]
	[Export] protected bool IsAttackMainActive;
	[Export] public string AttackUuid { get; protected set; } = "";

	[ExportCategory("Attack - Sequence")]
	[Export] protected bool IsAttackSequenceAvailable;
	[Export] protected string AttackSequence;
	[Export] protected int AttackSequenceNumber;
	[Export] protected int AttackSequenceHitCount;

	[ExportCategory("Attack - Extension")]
	[Export] protected int AttackExtensionFramesLeft;
	[Export] protected int AttackExtensionFramesMax = 30;

	[ExportCategory("Attack - Rotation")]
	[Export] protected float AttackHandRotationLockStart;
	[Export] protected float AttackHandRotationLockMultiplier;
	[Export] protected int AttackHandRotationLockRecoverFramesStart;
	[Export] protected int AttackHandRotationLockRecoverFramesCurrent;
	[Export] protected Curve AttackHandRotationRecoverCurve;

	[ExportCategory("Attack - Collision")]
	[Export] protected Area2D AttackCollisionArea2D;
	[Export] public bool IsAttackCollisionSweetSpotActive { get; protected set; }

	[ExportCategory("External")]
	[Export] protected GameManager GameManager;

	[ExportCategory("Misc")]
	[Export] protected int FrameCounter;

	[ExportCategory("Debug")]
	[Export] protected bool DebugEnabled;
	[Export] protected Label DebugAttackSequenceLabel;

	protected Dictionary<Rid, (string, int)> HitboxWeaponCollisions = new();

	protected string TempTransitionName;
	protected bool TempTransitionDone;

	protected float AttackHandRotationLerp
	{
		get
		{
			if (IsAttackMainActive)
				return AttackHandRotationLockMultiplier;
			if (AttackHandRotationLockRecoverFramesCurrent == 0)
				return 1;
			return AttackHandRotationLockMultiplier + (1 - AttackHandRotationLockMultiplier) * AttackHandRotationRecoverCurve.Sample((AttackHandRotationLockRecoverFramesStart - AttackHandRotationLockRecoverFramesCurrent) / (float)AttackHandRotationLockRecoverFramesStart);
		}
	}

	// Godot

	public override void _Ready()
	{
		RenderAnimationTreeWeaponStateMachinePlayback = (AnimationNodeStateMachinePlayback)RenderAnimationTree.Get("parameters/WeaponStateMachine/playback");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// Before process

		// Process

		_ProcessSelf(delta);

		// Process After

		ProcessHandRendering(delta);
		ProcessBodyRendering(delta);
		ProcessAfterRendering(delta);

		// Debug

		if (DebugEnabled)
		{
			DebugAttackSequenceLabel.Text = AttackSequence;
		}
	}

	public virtual void _ProcessSelf(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Before process

		FrameCounter++;

		if (!IsAttackMainActive) AttackHandRotationLockRecoverFramesCurrent--;

		InputBuffer.Process();

		MovementNext = Vector2.Zero;
		MovementDesired = Vector2.Zero;

		if (RenderEffectHitDurationLeft > 0)
		{
			RenderEffectHitDurationLeft--;
		}

		if (AttackExtensionFramesLeft > 0)
		{
			AttackExtensionFramesLeft--;
			RenderAnimationTree.Set("parameters/WeaponTimeScale/scale", 0);
			if (AttackExtensionFramesLeft <= 0)
			{
				AttackExtensionFramesLeft = 0;
				RenderAnimationTree.Set("parameters/WeaponTimeScale/scale", 1);
			}
		}

		// Process

		_PhysicsProcessSelf(delta);

		// After process

		Move();
	}

	public virtual void _PhysicsProcessSelf(double delta)
	{
	}

	// Movement

	private void Move()
	{
		MovementPrev = _movement;
		_movement = MovementNext + MovementDesired;

		MoveAndCollide(_movement);
	}

	public virtual void ChangeMovement(Vector2 movement)
	{
		MovementNext = movement;
	}

	public virtual void AddMovement(Vector2 movement)
	{
		MovementNext += movement;
	}

	public virtual void ChangeDesiredMovement(Vector2 movement)
	{
		MovementDesired = movement;
	}

	public virtual void AddDesiredMovement(Vector2 movement)
	{
		MovementDesired += movement;
	}

	// Rendering

	public virtual void ProcessHandRendering(double delta)
	{

	}

	public virtual void ProcessBodyRendering(double delta)
	{

	}

	public virtual void ProcessAfterRendering(double delta)
	{
		if (RenderEffectHitDurationLeft > 0)
		{
			RenderForegroundRoot.SetModulate(RenderEffectHitColor);
		}
		else
		{
			RenderForegroundRoot.SetModulate(Color.Color8(255, 255, 255));
		}
	}

	// Attack

	public virtual void MainAttack(string sequence)
	{
		// Sequence

		IsAttackSequenceAvailable = false;
		AttackSequence = sequence;
		if (!IsAttackMainActive) AttackSequenceNumber = 0;
		AttackSequenceNumber++;
		AttackSequenceHitCount = 0;

		// Extension

		AttackExtensionFramesLeft = 0;

		// Rotation Lock

		AttackHandRotationLockRecoverFramesStart = 30;
		AttackHandRotationLockRecoverFramesCurrent = 30;

		// Animation

		RenderAnimationTree.Set("parameters/WeaponTimeScale/scale", 1);
		RenderAnimationTreeWeaponStateMachinePlayback.Next();

		// Lock

		IsAttackMainActive = true;

		// UUID

		AttackUuid = Guid.NewGuid().ToString();
	}

	public virtual void MainAttackEnd()
	{
		IsAttackMainActive = false;
	}

	public void ActivateAttackCollision()
	{
		AttackCollisionArea2D.SetProcessMode(ProcessModeEnum.Always);
	}

	public void DisableAttackCollision()
	{
		AttackCollisionArea2D.SetProcessMode(ProcessModeEnum.Disabled);
	}

	public void ActivateAttackCollisionSweetSpot()
	{
		IsAttackCollisionSweetSpotActive = true;
	}

	public void DisableAttackCollisionSweetSpot()
	{
		IsAttackCollisionSweetSpotActive = true;
	}

	public void AttackSequenceAvailableStart()
	{
		IsAttackSequenceAvailable = true;
	}

	public void AttackSequenceAvailableEnd()
	{
		IsAttackSequenceAvailable = false;
	}

	public virtual void AttackSequenceHit()
	{
		AttackSequenceHitCount++;
		AttackExtensionFramesLeft += 2;
		if (AttackExtensionFramesLeft > AttackExtensionFramesMax) AttackExtensionFramesLeft = AttackExtensionFramesMax;
	}

	// Health

	public void Damage(long amount)
	{
		Health -= amount;
		RenderEffectHitDurationLeft = RenderEffectHitDuration;
		if (Health <= 0) Kill();
	}

	public void Kill()
	{
		QueueFree();
	}

	// Collision

	public void _OnBodyCollisionEnter(Rid areaRid, Area2D area, int areaShapeIndex, int localShapeIndex)
	{
		if (area is UnitWeaponCollision cast && cast.Team != Team)
		{
			// Add to hit registry

			if (HitboxWeaponCollisions.TryAdd(cast.GetRid(), (cast.AttackUuid, 1)))
			{
				cast.AttackSequenceHit();
				Damage(1);
			}
			else if (HitboxWeaponCollisions[cast.GetRid()].Item1 != cast.AttackUuid)
			{
				HitboxWeaponCollisions[cast.GetRid()] = (cast.AttackUuid, 1);
				cast.AttackSequenceHit();
				Damage(1);
			}
			else
			{
				HitboxWeaponCollisions[cast.GetRid()] = (cast.AttackUuid, HitboxWeaponCollisions[cast.GetRid()].Item2 + 1);
			}

			// Notify hit

			GD.Print(cast.AttackUuid, cast.Team, Team);
		}
	}

	public void _OnWeaponCollisionEnter(Rid areaRid, Area2D area, int areaShapeIndex, int localShapeIndex, UnitWeaponCollision weaponCollision)
	{

	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		GD.Print("Unit disposed");
	}
}
