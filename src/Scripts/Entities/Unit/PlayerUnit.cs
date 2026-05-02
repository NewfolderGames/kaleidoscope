using System;
using Godot;

namespace Kaleidoscope.Scripts.Entities.Unit;

public partial class PlayerUnit : Unit
{
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
        
        if (!IsAttackMainActive || IsAttackSequenceAvailable)
        {
            var inputs = InputBuffer.TakeGroup("attack_main");
            if (inputs.Count > 0)
            {
                var sequence = !IsAttackMainActive ? "" : AttackSequence;
                switch (inputs[0].Name)
                {
                    case "attack_primary":
	                    sequence += "a";
                        break;
                    case "attack_secondary":
	                    sequence += "b";
                        break;
                }
                if (sequence != "") MainAttack(sequence);
            }
        }
        
        // Movement
        
        MovementInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        AddDesiredMovement(MovementInput * MovementSpeed * (float)delta);
        
        // Done
        
        base._PhysicsProcessSelf(delta);
    }

    public override void ProcessBodyRendering(double delta)
    {
        base.ProcessBodyRendering(delta);
        
        TransformBodyRoot.Scale = new Vector2(TransformHandRoot.Position.X < TransformCenterRoot.Position.X ? -1 : 1, 1);
        
        if (IsMovementInputting)
        {
            var isMovingAway = Mathf.Sign(_mousePositionRelative.X) != Mathf.Sign(MovementInput.X);
            var speed = (MovementSpeedBase > 0 ? MovementSpeed / MovementSpeedBase : 1) * RenderAnimationMovingSpeedMultiplier * (isMovingAway ? -1 : 1);
            RenderAnimationTree.Set("parameters/BodyStateMachine/moving/TimeScale/scale", speed);
            RenderAnimationTree.Set("parameters/LegsStateMachine/moving/TimeScale/scale", speed);
            RenderAnimationTree.Set("parameters/WeaponTree/BaseStateMachine/moving/TimeScale/scale", speed);
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

    public override void MainAttack(string sequence)
    {
        if (!IsAttackMainActive) AttackHandRotationLockStart = _mousePositionRelative.Angle();
        
        base.MainAttack(sequence);
    }
}
