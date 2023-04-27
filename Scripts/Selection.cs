using Godot;
using System;

public class Selection : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Label buttonName;
    public TextureButton button;
    public int index = 0;
    public static Selection PressedInstance;
    public Action funcToCall;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
       
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

   
    public void SelectionPressed()
    {
        PressedInstance = this;
        if (funcToCall != null)
        {
            funcToCall();
        }
    }


}
