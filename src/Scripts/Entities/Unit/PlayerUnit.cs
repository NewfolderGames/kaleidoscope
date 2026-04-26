using System;
using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class PlayerUnit : Unit
{
    [ExportCategory("Movement")]
    [Export] private Vector2 _movementInput;
    
    [ExportCategory("Render States")]
    [Export] private Vector2 _mousePositionRelative;

    public override void _Process(double delta)
    {
        _mousePositionRelative = GetGlobalMousePosition() - PositionCenterRoot.GetGlobalPosition() ;
        
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Attack
        
        if (Input.IsActionJustPressed("attack_primary"))
        {
            RenderAnimationTree.Set("parameters/OneShotHandAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }
        
        // Movement
        
        _movementInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        IsMoving = _movementInput != Vector2.Zero;
        
        AddMovement(_movementInput * MovementSpeed * (float)delta);
        base._PhysicsProcess(delta);
    }

    public override void ProcessBodyRendering(double delta)
    {
        base.ProcessBodyRendering(delta);
        
        RenderBodyRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        if (IsMoving)
        {
            var isMovingAway = Mathf.Sign(_mousePositionRelative.X) != Mathf.Sign(_movementInput.X);
            var speed = (MovementSpeedBase > 0 ? MovementSpeed / MovementSpeedBase : 1) * RenderAnimationMovingSpeedMultiplier * (isMovingAway ? -1 : 1);
            RenderAnimationTree.Set("parameters/TimeScaleBodyMoving/scale", speed);
            RenderAnimationTree.Set("parameters/TimeScaleLegsMoving/scale", speed);
            RenderAnimationTree.Set("parameters/TransitionBody/transition_request", "moving");
            RenderAnimationTree.Set("parameters/TransitionLegs/transition_request", "moving");
        }
        else
        {
            RenderAnimationTree.Set("parameters/TransitionBody/transition_request", "idle");
            RenderAnimationTree.Set("parameters/TransitionLegs/transition_request", "idle");
        }
    }
    
    public override void ProcessHandRendering(double delta)
    {
        base.ProcessHandRendering(delta);

        var normalized = _mousePositionRelative.Normalized();
        var normalizedAngle = normalized.Angle();
        
        PositionHandRoot.Position = new Vector2(normalized.X * PositionHandRootRangeX, normalized.Y * PositionHandRootRangeY);
        PositionHandRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        RenderHandOffset.Position = PositionCenterRoot.Position;
        RenderHandOffset.Rotation = _mousePositionRelative.X < 0 ? -normalizedAngle - Mathf.Pi : normalizedAngle;
    }
}