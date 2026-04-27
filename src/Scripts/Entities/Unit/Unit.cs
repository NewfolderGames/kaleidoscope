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

    [ExportCategory("Input")]
    [Export] protected bool AllowInput;
    protected readonly InputBuffer InputBuffer = new();
    
    [ExportCategory("Movement")]
    [Export] protected bool IsMoving;
    [Export] private Vector2 _movement;
    [Export] protected Vector2 MovementPrev;
    [Export] protected Vector2 MovementNext;
    [Export] protected float MovementSpeedBase = 5f;
    [Export] protected float MovementSpeed = 5f;
    
    [ExportCategory("Weapon")]
    [Export] protected Weapon Weapon;
    [Export] protected bool weaponSweetSpotActive;
    [Export] protected float weaponDamageMultiplerBase = 1f;
    [Export] protected float weaponDamageSweetSpotMultiplierBase = 1f;

    [ExportCategory("Attack")]
    [Export] protected bool IsAttackMainActive;
    [Export] protected bool IsAttackSequenceAvailable;
    [Export] protected float AttackHandRotationLockStart;
    [Export] protected float AttackHandRotationLockMultiplier;
    [Export] protected int AttackHandRotationLockRecoverFramesStart;
    [Export] protected int AttackHandRotationLockRecoverFramesCurrent;
    [Export] protected Curve AttackHandRotationRecoverCurve;
    
    [ExportCategory("External")]
    [Export] protected GameManager GameManager;

    [ExportCategory("Misc")]
    [Export] protected int FrameCounter;

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
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Before process
        
        // Process
        
        _ProcessSelf(delta);
    }

    public virtual void _ProcessSelf(double delta)
    {
        ProcessHandRendering(delta);
        ProcessBodyRendering(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
     
        // Before process

        FrameCounter++;
        
        if (!IsAttackMainActive) AttackHandRotationLockRecoverFramesCurrent--;
        
        InputBuffer.Process();
        
        // Process
        
        _PhysicsProcessSelf(delta);
    }

    public virtual void _PhysicsProcessSelf(double delta)
    {
        Move();
    }

    private void Move()
    {
        MovementPrev = _movement;
        _movement = MovementNext;
        MoveAndCollide(MovementNext);
        MovementNext = Vector2.Zero;
    }

    public virtual void ChangeMovement(Vector2 movement)
    {
        MovementNext = movement;
    }
    
    public virtual void AddMovement(Vector2 movement)
    {
        MovementNext += movement;
    }
    
    public virtual void ProcessHandRendering(double delta)
    {
        
    }
    
    public virtual void ProcessBodyRendering(double delta)
    {
        
    }
    
    public void SetWeaponSweetSpotActive(bool active)
    {
        weaponSweetSpotActive = active;
    }
    
    public virtual void MainAttackStart()
    {
        IsAttackMainActive = true;
        AttackHandRotationLockRecoverFramesStart = 30;
        AttackHandRotationLockRecoverFramesCurrent = 30;
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