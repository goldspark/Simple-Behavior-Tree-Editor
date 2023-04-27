using Godot;
using System;
using System.Runtime.CompilerServices;

public class DialogWithSelection : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public TextureButton button1, button2, button3;
    public Label title, description;
    static PackedScene packedSceneLocation = ResourceLoader.Load<PackedScene>("res://Scripts/DialogWithSelection.tscn");


    public Action method1, method2, method3;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        title = GetNode<Label>("Title");
    }


    //Use this to create dialog
    public static void CreateDialog(Control node, string title, string description, string btn1Title, string btn2Title, string btn3Title, Action button1Func, Action button2Func, Action button3Func)
    {
        

        DialogWithSelection dialog = packedSceneLocation.Instance<DialogWithSelection>();
        dialog.title = dialog.GetNode<Label>("Title");
        dialog.description = dialog.GetNode<Label>("Description");
        dialog.button1 = dialog.GetNode<TextureButton>("Button1");
        dialog.button2 = dialog.GetNode<TextureButton>("Button2");
        dialog.button3 = dialog.GetNode<TextureButton>("Button3");



        dialog.title.Text = title;
        dialog.description.Text = description;
        dialog.button1.Connect("button_up", dialog, "BtnPress1");
        dialog.button2.Connect("button_up", dialog, "BtnPress2");
        dialog.button3.Connect("button_up", dialog, "BtnPress3");

        dialog.button1.GetNode<Label>("Label").Text = btn1Title;
        dialog.button2.GetNode<Label>("Label").Text = btn2Title;
        dialog.button3.GetNode<Label>("Label").Text = btn3Title;

        dialog.method1 = button1Func;
        dialog.method2 = button2Func;
        dialog.method3 = button3Func;

        node.AddChild(dialog);
            
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }


    public void BtnPress1()
    {
        method1();
        Exit();
    }
    public void BtnPress2()
    {
        method2();
        Exit();
    }
    public void BtnPress3()
    {
        method3();
        Exit();
    }

    public void Exit()
    {
        QueueFree();
    }
}
