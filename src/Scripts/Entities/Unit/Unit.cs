using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class Unit : CharacterBody2D
{
    [ExportCategory("Position Root")]
    [Export] protected Node2D PositionRoot;
    [Export] protected Node2D PositionCenterRoot;
    
    [ExportCategory("Render")]
    [Export] protected Node2D RenderRoot;
    [Export] protected Node2D RenderHandRoot;
    [Export] protected float RenderHandRangeX = 24f;
    [Export] protected float RenderHandRangeY = 12f;
    [Export] protected Node2D RenderHandCenterOffset;
    [Export] protected Node2D RenderHandOffset;
    [Export] protected Node2D RenderBodyRoot;
    
    [ExportCategory("Movement")]
    [Export] protected Vector2 Movement;
    [Export] protected Vector2 MovementPrev;
    [Export] protected Vector2 MovementSpeedBase = new (5f, 5f);

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

    public virtual void Move()
    {
        MoveAndCollide(Movement);
    }

    public virtual void ChangeMovement(Vector2 movement)
    {
        MovementPrev = Movement;
        Movement = movement;
    }
    
    public virtual void ProcessHandRendering(double delta)
    {
        
    }
    
    public virtual void ProcessBodyRendering(double delta)
    {
        
    }
}