using System;
using Godot;

namespace Chisel
{
    public static class PuzzleInput
    {
        private static readonly Vector2 YFlip = new Vector2(1, -1);

        public static void OnPuzzleTurn(InputEvent e, Action<Vector2> action)
        {
            if (e is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
                action(motion.Relative * YFlip);
        }

        public static void OnScrollIn(Action action)
        {
            if (Input.IsActionPressed("puzzle_zoom_in"))
                action();
        }
        
        public static void OnScrollOut(Action action)
        {
            if (Input.IsActionPressed("puzzle_zoom_out"))
                action();
        }
    }
}