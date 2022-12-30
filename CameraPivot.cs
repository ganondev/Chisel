using Godot;
using System;
using Chisel;

public class CameraPivot : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {

        var yAngle = ((2 * Mathf.Pi) / 1000) * mouseMotion.y;
        var maxY = 1.5f;
        var minY = -1.5f;
        var currentRotation = Rotation.x;
        var newRotation = Mathf.Clamp(currentRotation + yAngle, minY, maxY);
        var yChange = newRotation - currentRotation;
        RotateX(yChange);

        mouseMotion *= 0.9f;
        
        // wheel
        ScaleObjectLocal(Vector3.One * (1 + wheelMotion));
        wheelMotion *= 0.9f;

    }
    
    private Vector2 mouseMotion = Vector2.Zero;
    private float wheelMotion = 0f;
	
    public override void _Input(InputEvent e) {
        PuzzleInput.OnPuzzleTurn(e, (motion) =>
        {
            mouseMotion = motion;
        });
        
        PuzzleInput.OnScrollIn(() =>
        {
            wheelMotion = -0.01f;
        });
        PuzzleInput.OnScrollOut(() =>
        {
            wheelMotion = 0.01f;
        });
    }
    
}
