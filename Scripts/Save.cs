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
            AIEditor.SetTitle(textEdit.Text);
            Visible = false;
        }
        AIWorld.lockMovement = false;
    }

    public void Visibility()
    {
        if (Visible)
        {
            string file = AIEditor.Instance.nameOfFile;
            if (file != null && file.Length > 0)
            {
                string trimmedName = file.Substring(0, file.IndexOf('.'));
                textEdit.Text = trimmedName;
            }
            else
                textEdit.Text = "Name";
        }
    }

    public void NoReleased()
    {
        Visible = false;
        AIWorld.lockMovement = false;
    }
}
