using Godot;
using System;

public class MovableCamera : Camera2D
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
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && eventMouseMotion.ButtonMask == (int)ButtonList.Middle)
        {
            Position -= eventMouseMotion.Relative * Zoom;
        }
    }




}
