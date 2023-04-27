using Godot;
using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
public class BehaviorNode : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public int id = 0;

    public Label title, description;
    static PackedScene packedSceneLocation = ResourceLoader.Load<PackedScene>("res://Scripts/BehaviorNode.tscn");

    


    int numberOfChildren = 0;

    public BehaviorNode parent;
    public List<BehaviorNode> children = new List<BehaviorNode>();

    //The node this behavior node represents
    public string GoldNodeName;
    public static List<Selection> listOfAvailableNodes = new List<Selection>();
    Node2D dialogs;

    public static Vector2 size = new Vector2(256, 256);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        dialogs = GetNode<Node2D>("Dialogs");
    }

    public static BehaviorNode CreateNode(Vector2 pos, string title, string description, BehaviorNode parent)
    {
        AIEditor.GLOBAL_ID++;
        
        BehaviorNode dialog = packedSceneLocation.Instance<BehaviorNode>();
        dialog.title = dialog.GetNode<Label>("Label");
        dialog.description = dialog.GetNode<Label>("Description");
        
        dialog.title.Text = title;
        dialog.description.Text = description;
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
        behaviorNode.description = behaviorNode.GetNode<Label>("Description");
        behaviorNode.id = AIEditor.GLOBAL_ID;
        behaviorNode.title.Text = title;
        behaviorNode.description.Text = description;
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
        behaviorNode.description = behaviorNode.GetNode<Label>("Description");
        behaviorNode.id = AIEditor.GLOBAL_ID;
        behaviorNode.title.Text = title;
        behaviorNode.description.Text = description;
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
    private void ReAranageChildren()
    {
        float width = numberOfChildren * size.x;
        

        if (children.Count > 1)
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
        LoadNodesToSelect();

    }

    public void AddSelectionChild()
    {
        if (Selection.PressedInstance != null)
        {
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



    public void NodePressed()
    {
        if (parent != null)
        {
            foreach(BehaviorNode node in children)
            {
                node.Visible = !node.Visible;
                node.NodePressed();
            }
        }
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
       
        for (int i = 0; i < children.Count; i++)
        {  
            children[i].Remove();
        }

        children.Clear();
        QueueFree();

    }

   
}
