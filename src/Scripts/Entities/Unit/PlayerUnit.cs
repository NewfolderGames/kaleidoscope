using System;
using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class PlayerUnit : Unit
{
    [ExportCategory("Movement")]
    [Export] private Vector2 _movementInput;
    
    [ExportCategory("Render States")]
    [Export] private Vector2 _mousePositionRelative;

    public override void _ProcessSelf(double delta)
    {
        _mousePositionRelative = GetGlobalMousePosition() - PositionCenterRoot.GetGlobalPosition();
        
        // Done
        
        base._ProcessSelf(delta);
    }

    public override void _PhysicsProcessSelf(double delta)
    {
        // Input

        if (Input.IsActionJustPressed("attack_primary"))
        {
            InputBuffer.Add("attack_primary", "attack_main", 1, 15);
        }
        if (Input.IsActionJustPressed("attack_secondary"))
        {
            InputBuffer.Add("attack_secondary", "attack_main", 1, 15);
        }
        
        // Attack

        if (!IsAttackMainActive)
        {
            var inputs = InputBuffer.TakeGroup("attack_main");
            if (inputs.Count > 0)
            {
                switch (inputs[0].Name)
                {
                    case "attack_primary":
                        RenderAnimationTree.Set("parameters/TransitionHandAttack/transition_request", "primary_1");
                        break;
                    case "attack_secondary":
                        RenderAnimationTree.Set("parameters/TransitionHandAttack/transition_request", "secondary_1");
                        break;
                }
                RenderAnimationTree.Set("parameters/OneShotHandAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
                MainAttackStart();
            }
        }
        
        // Movement
        
        _movementInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        IsMoving = _movementInput != Vector2.Zero;
        
        AddMovement(_movementInput * MovementSpeed * (float)delta);
        
        // Done
        
        base._PhysicsProcessSelf(delta);
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
        
        if (LockHandRotationWhenAttacking && IsAttackMainActive) return;

        var normalized = _mousePositionRelative.Normalized();
        var normalizedAngle = normalized.Angle();
        
        PositionHandRoot.Position = new Vector2(normalized.X * PositionHandRootRangeX, normalized.Y * PositionHandRootRangeY);
        PositionHandRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        RenderHandOffset.Position = PositionCenterRoot.Position;
        RenderHandOffset.Rotation = _mousePositionRelative.X < 0 ? -normalizedAngle - Mathf.Pi : normalizedAngle;
    }
}