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
        _mousePositionRelative = GetGlobalMousePosition() - TransformCenterRoot.GetGlobalPosition();
        
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
                var transition = "";
                switch (inputs[0].Name)
                {
                    case "attack_primary":
                        transition = "primary_1";
                        break;
                    case "attack_secondary":
                        transition = "secondary_1";
                        break;
                }

                if (transition != "")
                {
                    RenderAnimationTree.Set("parameters/TransitionHandAttack/transition_request", transition);
                    RenderAnimationTree.Set("parameters/OneShotHandAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
                    MainAttackStart();
                    TempTransitionName = transition;
                }
            }
        }
        else if (IsAttackSequenceAvailable && !TempTransitionDone)
        {
            var inputs = InputBuffer.TakeGroup("attack_main");
            if (inputs.Count > 0)
            {
                var transition = "";
                switch (inputs[0].Name)
                {
                    case "attack_primary":
                        if (TempTransitionName == "primary_1") transition = "primary_2";
                        if (TempTransitionName == "primary_2")
                        {
                            transition = "secondary_1";
                            TempTransitionDone = true;
                        }
                        break;
                    case "attack_secondary":
                        if (TempTransitionName is "weapon/primary_1" or "primary_2")
                        {
                            transition = "secondary_1";
                            TempTransitionDone = true;
                        }
                        break;
                }
                RenderAnimationTree.Set("parameters/TransitionHandAttack/transition_request", transition);
                // RenderAnimationTree.Set("parameters/OneShotHandAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
                TempTransitionName = transition;
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
        
        TransformBodyRoot.Scale = new Vector2(TransformHandRoot.Position.X < TransformCenterRoot.Position.X ? -1 : 1, 1);

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
        
        var baseAngle = _mousePositionRelative.Angle();
        var targetAngle = Mathf.LerpAngle(AttackHandRotationLockStart, baseAngle, AttackHandRotationLerp);
        var normalized = Vector2.Right.Rotated(targetAngle);
        
        TransformHandRoot.Position = new Vector2(normalized.X * TransformHandRootRangeX, normalized.Y * TransformHandRootRangeY);
        TransformHandRoot.Scale = new Vector2(TransformHandRoot.Position.X < 0 ? -1 : 1, 1);
        TransformHandOffset.Rotation = TransformHandRoot.Position.X < 0 ? -targetAngle - Mathf.Pi : targetAngle;
    }

    public override void MainAttackStart()
    {
        base.MainAttackStart();
        AttackHandRotationLockStart = _mousePositionRelative.Angle();
        TempTransitionDone = false;
    }
}
