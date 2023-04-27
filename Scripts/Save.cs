using Godot;
using SimpleBehaviorTreeEditor.AIEditor;
using System;

public class Save : Node2D
{


    public TextEdit textEdit;

    public override void _Ready()
    {
        textEdit = GetNode<TextEdit>("FileName");
    }

    public void Released()
    {
        if (textEdit.Text.Length > 0)
        {
            FileCreator.SaveAI(AIEditor.Instance.currentNode, textEdit.Text);
            Visible = false;
        }
    }

    public void NoReleased()
    {
        Visible = false;
    }
}
