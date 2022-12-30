using Godot;
using System;
using Chisel;

public class CameraPivot : Spatial
{
    
    private Vector2 _mouseMotion = Vector2.Zero;
    private float _wheelMotion;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {

        var yAngle = ((2 * Mathf.Pi) / 1000) * _mouseMotion.y;
        var maxY = 1.5f;
        var minY = -1.5f;
        var currentRotation = Rotation.x;
        var newRotation = Mathf.Clamp(currentRotation + yAngle, minY, maxY);
        var yChange = newRotation - currentRotation;
        RotateX(yChange);
        _mouseMotion *= 0.9f;
        
        ScaleObjectLocal(Vector3.One * (1 + _wheelMotion));
        _wheelMotion *= 0.9f;

    }

    public override void _Input(InputEvent e) {
        PuzzleInput.OnPuzzleTurn(e, (motion) =>
        {
            _mouseMotion = motion;
        });
        
        PuzzleInput.OnScrollIn(() =>
        {
            _wheelMotion = -0.01f;
        });
        PuzzleInput.OnScrollOut(() =>
        {
            _wheelMotion = 0.01f;
        });
    }
    
}
