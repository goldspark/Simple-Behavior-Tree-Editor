using Godot;
using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using File = System.IO.File;
public class BehaviorNode : Control, IEquatable<BehaviorNode>, IComparable<BehaviorNode>
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public int id = 0;
    public string uniqueName;
    public Position2D lineConnectPos, lineStart, bottomLineStart, bottomLineEnd;
    public Label title;
    public TouchScreenButton btnShowHide, codeExit, removeNode;
    public Sprite background;
    public bool isHidden = false;
    public bool IsReparenting = false;
    public List<string> code = new List<string>();
    static PackedScene packedSceneLocation = ResourceLoader.Load<PackedScene>("res://Scripts/BehaviorNode.tscn");

    int numberOfChildren = 0;

    public BehaviorNode parent;
    public List<BehaviorNode> children = new List<BehaviorNode>();

    //The node this behavior node represents
    public string GoldNodeName;
    public static List<Selection> listOfAvailableNodes = new List<Selection>();
    Node2D dialogs;

   

    public static Vector2 size = new Vector2(256, 256);

    public Node2D codeEditor;
    bool isSelected = false;
    bool hideChildren = false;
    public TextEdit codeEditorContainer;
    public TextEdit codeEditorText;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        codeEditor = GetNode<Node2D>("CodeEditor");
        codeExit = GetNode<TouchScreenButton>("CodeEditor/CodeExit");
        codeEditorContainer = GetNode<TextEdit>("CodeEditor/CodeEditContainer");
        codeEditorText = GetNode<TextEdit>("CodeEditor/CodeEdit");
        lineConnectPos = GetNode<Position2D>("Position2D");
        lineStart = GetNode<Position2D>("LineStart");
        removeNode = GetNode<TouchScreenButton>("Exit");
        bottomLineStart = GetNode<Position2D>("BottomLineStart");
        bottomLineEnd = GetNode<Position2D>("BottomLineEnd");
        dialogs = GetNode<Node2D>("Dialogs");
        background = GetNode<Sprite>("BG");
        btnShowHide = GetNode<TouchScreenButton>("ShowHide");


        if (!uniqueName.Empty() && !uniqueName.All<char>(char.IsDigit))
            uniqueName = title.Text;
        if(uniqueName.Empty())
            uniqueName = title.Text + "" + id;

        if (parent == null)
        {
            btnShowHide.QueueFree();
            AIEditor.rootNode = this;
        }

      
        
    }

    public override void _Process(float delta)
    {
        if (children.Count > 0 && AIWorld.isUpdating)
            children.Sort();

        if (AIWorld.isCodeOpen)
            removeNode.Visible = false;
        else
            removeNode.Visible = true;
    }


    public static BehaviorNode CreateNode(Vector2 pos, string title, string description, BehaviorNode parent)
    {
        AIEditor.GLOBAL_ID++;
        
        BehaviorNode dialog = packedSceneLocation.Instance<BehaviorNode>();
        dialog.title = dialog.GetNode<Label>("Label");
        
        dialog.title.Text = title;
        dialog.parent = parent;
        dialog.id = AIEditor.GLOBAL_ID;
      
        AIEditor.Instance.GetParent().GetParent<Node2D>().AddChild(dialog);

        dialog.RectPosition = pos - (size * 0.5f);
        dialog.RectScale = new Vector2(0.5f, 0.5f);
        dialog.GoldNodeName = title;
       

        return dialog;
        
    }

    public void CreateChild(Vector2 pos, string title, string description, BehaviorNode parent)
    {
        AIEditor.GLOBAL_ID++;

        BehaviorNode behaviorNode = packedSceneLocation.Instance<BehaviorNode>();
        behaviorNode.title = behaviorNode.GetNode<Label>("Label");
        behaviorNode.id = AIEditor.GLOBAL_ID;
        behaviorNode.title.Text = title;
        behaviorNode.parent = parent;


        AIEditor.Instance.GetParent().GetParent<Node2D>().AddChild(behaviorNode);

        behaviorNode.RectPosition = pos - (size * 0.5f);
        behaviorNode.RectScale = new Vector2(0.5f, 0.5f);
        behaviorNode.GoldNodeName = title;
        children.Add(behaviorNode);
        ReAranageChildren();
    }

    public BehaviorNode CreateChildR(Vector2 pos, string title, string description, BehaviorNode parent)
    {
        AIEditor.GLOBAL_ID++;

        BehaviorNode behaviorNode = packedSceneLocation.Instance<BehaviorNode>();
        behaviorNode.title = behaviorNode.GetNode<Label>("Label");
        behaviorNode.id = AIEditor.GLOBAL_ID;
        behaviorNode.title.Text = title;
        behaviorNode.parent = parent;


        AIEditor.Instance.GetParent().GetParent<Node2D>().AddChild(behaviorNode);

        behaviorNode.RectPosition = pos - (size * 0.5f);
        behaviorNode.RectScale = new Vector2(0.5f, 0.5f);
        behaviorNode.GoldNodeName = title;
        children.Add(behaviorNode);
        ReAranageChildren();
        
        return behaviorNode;
    }

    public BehaviorNode CreateChildRID(Vector2 pos, string title, string description, BehaviorNode parent, int id)
    {
        AIEditor.GLOBAL_ID++;

        BehaviorNode behaviorNode = packedSceneLocation.Instance<BehaviorNode>();
        behaviorNode.title = behaviorNode.GetNode<Label>("Label");
        behaviorNode.id = id;
        behaviorNode.title.Text = title;
        behaviorNode.parent = parent;


        AIEditor.Instance.GetParent().GetParent<Node2D>().AddChild(behaviorNode);

        behaviorNode.RectPosition = pos - (size * 0.5f);
        behaviorNode.RectScale = new Vector2(0.5f, 0.5f);
        behaviorNode.GoldNodeName = title;
        children.Add(behaviorNode);
        ReAranageChildren();

        return behaviorNode;
    }


    float marginY = 20f;
    public void ReAranageChildren()
    {
        float width = numberOfChildren * size.x;

        
        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                float x = RectPosition.x - (width * 0.5f) + (i * (size.x)) + (size.x * 0.5f); // have to multiply with 0.5f because its scale
                children[i].RectPosition = new Vector2(x, children[i].RectPosition.y);                
            }
        }
    }

    public void AddChildNode()
    {
        AIWorld.lockMovement = true;
        LoadNodesToSelect();

    }

    public void AddSelectionChild()
    {
        if (Selection.PressedInstance != null)
        {
            AIWorld.lockMovement = false;
            numberOfChildren++;

            Vector2 startPos = new Vector2(this.RectPosition.x + (size.x * 0.5f * numberOfChildren), this.RectPosition.y + size.y + marginY);

            CreateChild(startPos, Selection.PressedInstance.buttonName.Text, "A child node", this);

            foreach (BehaviorNode node in children)
            {
                node.Visible = true;
            }
            Selection.PressedInstance = null;
        }
    }

    public BehaviorNode AddChild(string name)
    {
        BehaviorNode n;
        numberOfChildren++;

        Vector2 startPos = new Vector2(this.RectPosition.x + (size.x * 0.5f * numberOfChildren), this.RectPosition.y + size.y + marginY);

        n = CreateChildR(startPos, name, "A child node", this);

        foreach (BehaviorNode node in children)
        {
            node.Visible = true;
        }

        return n;
    }

    public BehaviorNode AddChildID(string name, int id)
    {
        BehaviorNode n;
        numberOfChildren++;

        Vector2 startPos = new Vector2(this.RectPosition.x + (size.x * 0.5f * numberOfChildren), this.RectPosition.y + size.y + marginY);

        n = CreateChildRID(startPos, name, "A child node", this, id);

        foreach (BehaviorNode node in children)
        {
            node.Visible = true;
        }

        return n;
    }

    public void Hide()
    {
        hideChildren = !hideChildren;

        if (hideChildren)
        {
            btnShowHide.Normal = ResourceLoader.Load<Texture>("res://HUD/hide.png");
        }
        else
        {
            btnShowHide.Normal = ResourceLoader.Load<Texture>("res://HUD/show.png");
        }

        foreach (BehaviorNode child in children)
            child.HideChildren();

    }

    public void HideChildren()
    {
        isHidden = !isHidden;
        Visible = !Visible;

        if (parent != null)
        {
            foreach(BehaviorNode child in children)        
                child.HideChildren();    
        }
    }

    public void HighlightChildren(BehaviorNode parent)
    {
        foreach( BehaviorNode child in parent.children)
        {
            child.background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card_selected_children.png");
        }
    }

    public void UnhighlightChildren(BehaviorNode parent)
    {
        foreach (BehaviorNode child in parent.children)
        {
            child.background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card.png");
        }
    }

    public void NodeSelected()
    {
        if(AIEditor.Instance.selectedNode != null && AIEditor.Instance.selectedNode.IsReparenting)
        {
            for (int i = 0; i < AIEditor.Instance.selectedNode.parent.children.Count; i++)
            {
                var child = AIEditor.Instance.selectedNode.parent.children[i];
                if (child.id == AIEditor.Instance.selectedNode.id)
                {
                    AIEditor.Instance.selectedNode.parent.children.RemoveAt(i);
                    break;
                }
            }

            AIEditor.Instance.selectedNode.parent = this;
            children.Add(AIEditor.Instance.selectedNode);
            AIEditor.Instance.selectedNode.IsReparenting = false;
        }
        


        isSelected = true;
        AIEditor.Instance.selectedNodeLabel.Text = uniqueName;

        if (AIEditor.Instance.selectedNode != null)
        {
            if(AIEditor.Instance.selectedNode.id != id)
            {
                UnhighlightChildren(AIEditor.Instance.selectedNode);
                AIEditor.Instance.selectedNode.background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card.png");
                background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card_selected.png");
                AIEditor.Instance.selectedNode = this;
                HighlightChildren(AIEditor.Instance.selectedNode);
            }
        }
        else
        {
            background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card_selected.png");
            AIEditor.Instance.selectedNode = this;
            HighlightChildren(AIEditor.Instance.selectedNode);
        }

        
    }

    public void InitReparent(bool isReparenting)
    {
        IsReparenting = isReparenting;

        if(isReparenting)
            background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card_reparent.png");
        else
            background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card_selected.png");
    }

    public void DeselectNode()
    {
        AIEditor.Instance.selectedNode.background.Texture = ResourceLoader.Load<Texture>("res://HUD/node_card.png");
        UnhighlightChildren(AIEditor.Instance.selectedNode);
        AIEditor.Instance.selectedNode = null;
        isSelected = false;
    }

    public void NodeReleased()
    {
        isSelected = false;
    }


    public void LoadNodesToSelect()
    {
        listOfAvailableNodes.Clear();

       

        foreach (string availableNode in AIEditor.availableNodes)
        {
            Selection selection = ResourceLoader.Load<PackedScene>("res://Scripts/Selection.tscn").Instance<Selection>();
            selection.buttonName = selection.GetNode<Label>("Button/Label");
            selection.button = selection.GetNode<TextureButton>("Button");


            selection.buttonName.Text = System.IO.Path.GetFileName(availableNode);
            listOfAvailableNodes.Add(selection);
        }
        DialogWithList.CreateDialogListSelectionFunc("Behaviors", dialogs, listOfAvailableNodes, "SelectionPressed", AddSelectionChild);
        DialogWithList.Instance.RectScale = new Vector2(1.5f, 1.5f);
    }

   public void Remove()
   {
        if (parent != null)
        {
            parent.children.Remove(this);
            parent.numberOfChildren--;
            parent.ReAranageChildren();
        }

        if (AIEditor.Instance.selectedNode != null && AIEditor.Instance.selectedNode.id == id)
            AIEditor.Instance.selectedNode = null;

        if (AIEditor.rootNode != null && AIEditor.rootNode.id == id)
            AIEditor.rootNode = null;

        RemoveRecursive();

        if(parent != null)
        if (parent.children.Count > 0)
        {
            ReAranageChildren();
        }
    }

    public void OpenCodeEditor()
    {
        codeEditor.Visible = true;
        AIWorld.isCodeOpen = true;
        AIWorld.Instance.StopMovement();
        if (code.Count > 0)
        {
            foreach (var s in code)
                codeEditorText.Text += s + "\n";
        }

    }

    public void CloseCodeEditor()
    {
        codeEditor.Visible = false;
        code.Clear();
        AIWorld.isCodeOpen = false;
        AIWorld.Instance.ResumeMovement();

        string[] lines = codeEditorText.Text.Split('\n');
        foreach (string s in lines)
            code.Add(s);

        codeEditorText.Text = "";
    }

    private void RemoveRecursive()
    {
        for (int i = 0; i < children.Count; i++)
        {
            numberOfChildren = 0;
            if (AIEditor.Instance.selectedNode != null && AIEditor.Instance.selectedNode.id == children[i].id)
                AIEditor.Instance.selectedNode = null;
            if (AIEditor.rootNode != null && AIEditor.rootNode.id == children[i].id)
                AIEditor.rootNode = null;
            children[i].RemoveRecursive();
        }

        children.Clear();
        QueueFree();


        if (parent != null)
            parent.CallDeferred("ReAranageChildren");

    }


    public void Swap(BehaviorNode n1, BehaviorNode n2)
    {
        BehaviorNode temp = n1;
        n1 = n2;
        n2 = temp;
    }

    public void AddVariables()
    {

    }

    public bool Equals(BehaviorNode other)
    {
        if(other == null) return false;

        return this.id.Equals(other.id);
    }

    public int CompareTo(BehaviorNode other)
    {
        int res = 1;
        if (lineConnectPos.GlobalPosition.x < other.lineConnectPos.GlobalPosition.x)
            res = -1;

        return res;
    }
}
