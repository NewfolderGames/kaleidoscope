using System;
using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class PlayerUnit : Unit
{
    [ExportCategory("Render States")]
    [Export] private Vector2 _mousePositionRelative;

    public override void _Process(double delta)
    {
        var mousePosition = GetViewport().GetMousePosition();
        var centerPosition = GetViewport().GetCamera2D().GetCanvasTransform().AffineInverse() * (PositionCenterRoot.GetGlobalPosition() + GetViewportRect().Size);
        _mousePositionRelative = mousePosition - centerPosition;
        
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
        
        var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        var movement = inputDir * MovementSpeed * (float)delta;
        
        IsMoving = movement != Vector2.Zero;
        
        AddMovement(movement);
        base._PhysicsProcess(delta);
    }

    public override void ProcessBodyRendering(double delta)
    {
        base.ProcessBodyRendering(delta);
        
        RenderBodyRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        if (IsMoving)
        {
            var speed = (MovementSpeedBase > 0 ? MovementSpeed / MovementSpeedBase : 1) * RenderAnimationMovingSpeedMultiplier;
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

        PositionHandRoot.Position = new Vector2(normalized.X * PositionHandRootRangeX, normalized.Y * PositionHandRootRangeY);
        PositionHandRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        var normalizedAngle = normalized.Angle();
        PositionHandOffset.Position = PositionCenterRoot.Position;
        PositionHandOffset.Rotation = _mousePositionRelative.X < 0 ? -normalizedAngle - Mathf.Pi : normalizedAngle;
    }
}