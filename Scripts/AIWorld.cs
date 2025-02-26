using Godot;
using System;

public class AIWorld : Node2D
{
    public static AIWorld Instance;

    bool mouseHeld = false;
    Vector2 offset = Vector2.Zero;
    public static bool isUpdating = false;
    public static bool lockMovement = false;
    public static bool isCodeOpen = false;
    private bool _isReparentingInProgress = false;
    Camera2D gdCamera;

    public override void _Ready()
    {
        Instance = this;

        gdCamera = GetNode<Camera2D>("MovableCamera");
    }

    public override void _Process(float delta)
    {
        if (lockMovement)
            return;

        if (Input.IsMouseButtonPressed((int)ButtonList.Right) && AIEditor.Instance.selectedNode != null)
            AIEditor.Instance.selectedNode.DeselectNode();


        if (Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            if (AIEditor.Instance.selectedNode != null && !mouseHeld)
                offset = AIEditor.Instance.selectedNode.RectGlobalPosition - GetGlobalMousePosition();

            mouseHeld = true;
        }
        else
            mouseHeld = false;
 

        if (AIEditor.Instance.selectedNode != null && mouseHeld)
        {
            isUpdating = true;
            Vector2 oldParentPos = AIEditor.Instance.selectedNode.RectGlobalPosition;
            AIEditor.Instance.selectedNode.RectGlobalPosition = offset + GetGlobalMousePosition();
            MoveChildren(AIEditor.Instance.selectedNode, oldParentPos);
        }
        else
            isUpdating = false;

        Update();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("reparent") && AIEditor.Instance.selectedNode != null && AIEditor.Instance.selectedNode.parent != null)
        {
            GD.Print("Reparenting");
            _isReparentingInProgress = !_isReparentingInProgress;
            AIEditor.Instance.selectedNode.InitReparent(_isReparentingInProgress);

        }
    }

    public void MoveChildren(BehaviorNode parent, Vector2 oldParentPos)
    {
        if (parent.children.Count < 1)
            return;

        foreach (BehaviorNode child in parent.children)
        {
            Vector2 relativePos = child.RectGlobalPosition - oldParentPos;
            Vector2 oldPos = child.RectGlobalPosition;
            child.RectGlobalPosition = parent.RectGlobalPosition + relativePos;
            MoveChildren(child, oldPos);
        }
    }

    public void StopMovement()
    {
        gdCamera.Set("_stopMovement", true);
    }

    public void ResumeMovement()
    {
        gdCamera.Set("_stopMovement", false);
    }

    public void DrawParentToChild(BehaviorNode parent)
    {
        if (parent == null || parent.children.Count < 1)
            return;

        foreach (BehaviorNode node in parent.children)
        {
            if (node.isHidden)
                continue;
            
            Vector2 d = (parent.bottomLineEnd.GlobalPosition - parent.bottomLineStart.GlobalPosition);
            Vector2 v = node.lineConnectPos.GlobalPosition - parent.bottomLineStart.GlobalPosition;
            float t = d.Dot(v)/d.LengthSquared();

            Vector2 drawTo = parent.bottomLineStart.GlobalPosition + (d * t);

            if (t < 0.0f)
                drawTo = parent.bottomLineStart.GlobalPosition;
            if (t > 1.0f)
                drawTo = parent.bottomLineEnd.GlobalPosition;

            DrawLine(node.lineConnectPos.GlobalPosition, drawTo, Colors.Green);
            DrawParentToChild(node);
        }
    }



    public override void _Draw()
    {
        DrawParentToChild(AIEditor.rootNode);
    }

}
