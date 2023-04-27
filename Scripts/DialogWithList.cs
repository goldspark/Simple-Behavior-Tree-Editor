using Godot;
using System;
using System.Collections.Generic;

public class DialogWithList : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public static DialogWithList Instance;
    public Label title;
    public VBoxContainer container;
    static PackedScene packedSceneLocation = ResourceLoader.Load<PackedScene>("res://Scripts/DialogWithList.tscn");
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        if(IsInstanceValid(Instance))
        {
            Instance.Free();
        }

        Instance = this;

        container = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
        title = GetNode<Label>("Title");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    
    public static void CreateDialogList(string title, Control node, List<Selection> selectionList, string selectionFunction)
    {
        DialogWithList dialog = packedSceneLocation.Instance<DialogWithList>();

        

        node.AddChild(dialog);
        dialog.title.Text = title;

        foreach (Selection selection in selectionList)
        {
            selection.button.Connect("button_up", node, selectionFunction);
            dialog.container.AddChild(selection);
        }
    }

    public static void CreateDialogList(string title, Control node, Node2D functionLocationNode, List<Selection> selectionList, string selectionFunction)
    {
        DialogWithList dialog = packedSceneLocation.Instance<DialogWithList>();



        node.AddChild(dialog);
        dialog.title.Text = title;

        foreach (Selection selection in selectionList)
        {
            selection.button.Connect("button_up", functionLocationNode, selectionFunction);
            dialog.container.AddChild(selection);
        }
    }

    public static void CreateDialogListSelectionFunc(string title, Node2D node, List<Selection> selectionList, string selectionFunction, Action funcToCall)
    {
        DialogWithList dialog = packedSceneLocation.Instance<DialogWithList>();


        node.AddChild(dialog);
        dialog.title.Text = title;
        dialog.RectSize = new Vector2(320, 360);


        foreach (Selection selection in selectionList)
        {
            selection.funcToCall = funcToCall;
            selection.button.Connect("button_up", selection, selectionFunction);
            dialog.container.AddChild(selection);
        }
    }

    public static void CreateDialogListSelectionFunc(string title, Control node, List<Selection> selectionList, string selectionFunction, Action funcToCall)
    {
        DialogWithList dialog = packedSceneLocation.Instance<DialogWithList>();


        node.AddChild(dialog);
        dialog.title.Text = title;
        dialog.RectSize = new Vector2(320, 360);


        foreach (Selection selection in selectionList)
        {
            selection.funcToCall = funcToCall;
            selection.button.Connect("button_up", selection, selectionFunction);
            dialog.container.AddChild(selection);
        }
    }

    public void Exit()
    {
        QueueFree();
    }
}
