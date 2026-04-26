using Godot;
using Kaleidoscope.Core.Resources;
using Kaleidoscope.Scripts.Managers.Gameplay.Game;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class Unit : CharacterBody2D
{
    [ExportCategory("Position")]
    [Export] protected Node2D PositionRoot;
    [Export] protected Node2D PositionCenterRoot;
    [Export] protected Node2D PositionHandRoot;
    [Export] protected float PositionHandRootRangeX = 24f;
    [Export] protected float PositionHandRootRangeY = 12f;
    
    [ExportCategory("Render")]
    [Export] protected Node2D RenderRoot;
    [Export] protected Node2D RenderBodyRoot;
    [Export] protected Node2D RenderHandOffset;
    
    [ExportCategory("Render - Animation")]
    [Export] protected AnimationTree RenderAnimationTree;
    [Export] protected float RenderAnimationMovingSpeedMultiplier = 1f;
    
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
    [Export] protected bool IsMainAttacking;
    
    [ExportCategory("External")]
    [Export] protected GameManager GameManager;

    public override void _Process(double delta)
    {
        base._Process(delta);
        ProcessBodyRendering(delta);
        ProcessHandRendering(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
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
    
    public void MainAttackStart()
    {
        IsMainAttacking = true;
    }
    
    public void MainAttackEnd()
    {
        IsMainAttacking = false;
    }
}