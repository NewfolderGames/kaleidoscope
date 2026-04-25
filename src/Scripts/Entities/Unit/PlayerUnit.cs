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
        var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        var movement = inputDir * MovementSpeedBase;
        
        ChangeMovement(movement);
        base._PhysicsProcess(delta);
    }

    public override void ProcessBodyRendering(double delta)
    {
        base.ProcessBodyRendering(delta);
        
        RenderBodyRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);
    }
    
    public override void ProcessHandRendering(double delta)
    {
        base.ProcessHandRendering(delta);

        var normalized = _mousePositionRelative.Normalized();

        RenderHandRoot.Position = new Vector2(normalized.X * RenderHandRangeX, normalized.Y * RenderHandRangeY);
        RenderHandRoot.Scale = new Vector2(_mousePositionRelative.X < 0 ? -1 : 1, 1);

        var normalizedAngle = normalized.Angle();
        RenderHandCenterOffset.Position = PositionCenterRoot.Position;
        RenderHandCenterOffset.Rotation = _mousePositionRelative.X < 0 ? -normalizedAngle - Mathf.Pi : normalizedAngle;
        
        RenderHandOffsetBobbleProgress += (float)delta * RenderHandOffsetBobbleSpeed;
        RenderHandOffset.Position = new Vector2(0, Mathf.Sin(RenderHandOffsetBobbleProgress * Mathf.Pi * 2) * RenderHandOffsetBobbleRange);
    }
}