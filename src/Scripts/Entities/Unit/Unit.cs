using Godot;
using Kaleidoscope.Core.Resources;
using Kaleidoscope.Core.System.Input;
using Kaleidoscope.Scripts.Managers.Gameplay.Game;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class Unit : CharacterBody2D
{
	[ExportCategory("Position")]
	[Export] protected Node2D TransformRoot;
	[Export] protected Node2D TransformCenterRoot;
	[Export] protected Node2D TransformBodyRoot;
	[Export] protected Node2D TransformHandRoot;
	[Export] protected float TransformHandRootRangeX = 24f;
	[Export] protected float TransformHandRootRangeY = 12f;
	[Export] protected Node2D TransformHandOffset;
	
	[ExportCategory("Render")]
	[Export] protected Node2D RenderRoot;
	[Export] protected Node2D RenderBodyRoot;
	
	[ExportCategory("Render - Animation")]
	[Export] protected AnimationTree RenderAnimationTree;
	[Export] protected float RenderAnimationMovingSpeedMultiplier = 1f;
	protected AnimationNodeStateMachinePlayback RenderAnimationTreeWeaponStateMachinePlayback;

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
	[Export] protected bool WeaponSweetSpotActive;
	[Export] protected float WeaponDamageMultiplierBase = 1f;
	[Export] protected float WeaponDamageSweetSpotMultiplierBase = 1f;

	[ExportCategory("Attack")]
	[Export] protected bool IsAttackMainActive;
	
	[ExportCategory("Attack - Sequence")]
	[Export] protected bool IsAttackSequenceAvailable;
	[Export] protected string AttackSequence;
	[Export] protected int AttackSequenceNumber;
	
	[ExportCategory("Attack - Rotation")]
	[Export] protected float AttackHandRotationLockStart;
	[Export] protected float AttackHandRotationLockMultiplier;
	[Export] protected int AttackHandRotationLockRecoverFramesStart;
	[Export] protected int AttackHandRotationLockRecoverFramesCurrent;
	[Export] protected Curve AttackHandRotationRecoverCurve;
	
	[ExportCategory("External")]
	[Export] protected GameManager GameManager;

	[ExportCategory("Misc")]
	[Export] protected int FrameCounter;

	[ExportCategory("Debug")]
	[Export] protected bool DebugEnabled;
	[Export] protected Label DebugAttackSequenceLabel;

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
		
		// Process
		
		_PhysicsProcessSelf(delta);
		
		// After process
		
		Move();
	}

	public virtual void _PhysicsProcessSelf(double delta)
	{
		
	}

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
	
	public virtual void ProcessHandRendering(double delta)
	{
		
	}
	
	public virtual void ProcessBodyRendering(double delta)
	{
		
	}
	
	public void SetWeaponSweetSpotActive(bool active)
	{
		WeaponSweetSpotActive = active;
	}
	
	public virtual void MainAttack(string sequence)
	{
		// Sequence

		IsAttackSequenceAvailable = false;
		AttackSequence = sequence;
		if (!IsAttackMainActive) AttackSequenceNumber = 0;
		AttackSequenceNumber++;
		
		// Rotation Lock
		
		AttackHandRotationLockRecoverFramesStart = 30;
		AttackHandRotationLockRecoverFramesCurrent = 30;
		
		// Animation

		RenderAnimationTreeWeaponStateMachinePlayback.Next();
		
		// Lock
		
		IsAttackMainActive = true;
	}
	
	public virtual void MainAttackEnd()
	{
		IsAttackMainActive = false;
	}

	public void AttackSequenceAvailableStart()
	{
		IsAttackSequenceAvailable = true;
	}
	
	public void AttackSequenceAvailableEnd()
	{
		IsAttackSequenceAvailable = false;
	}
}
