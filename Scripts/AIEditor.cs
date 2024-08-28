using Godot;
using SimpleBehaviorTreeEditor.AIEditor;
using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

public class AIEditor : Control
{

    /*IMPORTANT
    * 
    * MUST HAVE AvailableNodes.txt file
    */
    public static string[] availableNodes;

    //Global ID for nodes
    public static int GLOBAL_ID = 0;


    PackedScene dialogPackedScene;

    public static AIEditor Instance;

    public BehaviorNode currentNode;
    public BehaviorNode selectedNode;
    public static BehaviorNode rootNode;
    public TextureButton addRootBtn;
    public List<Selection> selectionList = new List<Selection>();
    public Node2D editText;
    public Node2D dialogs;
    public RichTextLabel explanationText;
    public FileDialog fileDialog;


    public override void _Ready()
    {
        Instance = this;
        dialogPackedScene = ResourceLoader.Load<PackedScene>("res://Scripts/DialogWithSelection.tscn");


        addRootBtn = GetNode<TextureButton>("AddRoot");
        dialogs = GetNode<Node2D>("Dialogs");
        editText = GetNode<Node2D>("EditText");
        fileDialog = GetParent().GetNode<FileDialog>("FileDialog");

        //Create save folder for this system
        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName("MyAI"))){
            System.IO.Directory.CreateDirectory("MyAI");
        }
        ////////////////////////////////////
        ///


        availableNodes = new string[2];

        availableNodes[0] = "Sequence";
        availableNodes[1] = "Selector";


    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
      public override void _Process(float delta)
      {
        if (!IsInstanceValid(AIEditor.Instance.currentNode))
        {
            AIEditor.Instance.addRootBtn.Visible = true;
        }

       
    }

    private void LoadSavedAIFiles()
    {
        selectionList.Clear();

        string myAIDirectoryPath = "MyAI/";
        string[] files = System.IO.Directory.GetFiles(myAIDirectoryPath);
        foreach (string file in files)
        {
            Selection selection = ResourceLoader.Load<PackedScene>("res://Scripts/Selection.tscn").Instance<Selection>();
            selection.buttonName = selection.GetNode<Godot.Label>("Button/Label");
            selection.button = selection.GetNode<TextureButton>("Button");

            
            selection.buttonName.Text = System.IO.Path.GetFileName(file);
            selectionList.Add(selection);
        }
    }


  

    public void Save()
    {
        if (currentNode != null)
        {
            //FileCreator.SaveAI(currentNode, "MyCustomAI");
            editText.Visible = true;

        }
    }

    public void Load()
    {

        LoadSavedAIFiles();

        DialogWithList.CreateDialogListSelectionFunc("Select AI to edit", this, selectionList, "SelectionPressed", Accept);


    }
    public void SelectionPressed()
    {

        DialogWithSelection.CreateDialog(AIEditor.Instance, "Are you sure?", "Selecting this file will load this AI behavior on the screen.", "Yes", "No", "Cancel", SelectionYes, SelectionNo, CancelSelection);
    }
    public void SelectionNo()
    {

    }
    
    private void Accept()
    {
       SelectionPressed();
    }

    //Load AI
    public void SelectionYes()
    {
        if(rootNode != null)
            rootNode.Remove();
        LoadAIFile();
        DialogWithList.Instance.QueueFree();

    }

    List<string> keys = new List<string>();

    private void LoadAIFile()
    {
        FileCreator.ReadAIFile("MyAI/" + Selection.PressedInstance.buttonName.Text);
        Godot.File saveGame = new Godot.File();
        saveGame.OpenEncryptedWithPass("MyAI/" + Selection.PressedInstance.buttonName.Text, Godot.File.ModeFlags.Read, "Ligmasin");
        string text = saveGame.GetAsText();
        saveGame.Close();


        foreach (string key in FileCreator.parents.Keys)
        {
            keys.Add(key);
        }

        //Create root first
        string noNumberName = Regex.Replace(keys[0], @"\d+", "");

        if (noNumberName == "Selector")
            CreateSelector();
        else
            CreateSequence();

      
         AttachChildren(text, currentNode, keys[0]);
 
        
    }

    private void AttachChildren(string text, BehaviorNode node, string parentName)
    {
       
       foreach (string childName in FileCreator.GetChildrenOfParent(text, parentName))
       {
            string noNumberName = Regex.Replace(childName, @"\d+", "");
            BehaviorNode n = node.AddChild(noNumberName);
            AttachChildren(text, n, childName);
            
       }

        RearrangeNodes();

    }
   
    public static void RearrangeNodes()
    {
        rootNode.ReAranageChildren();

        for (int i = 0; i < rootNode.children.Count; i++)
        {
            rootNode.children[i].ReAranageChildren();
            RearrangeChildren(rootNode.children[i]);
        }
    }

    private static void RearrangeChildren(BehaviorNode child)
    {
        if (child.children != null && child.children.Count < 1)
            return;

        for(int i = 0; i < child.children.Count; i++)
        {
            child.children[i].ReAranageChildren();
            RearrangeChildren(child.children[i]);
        }
    }


    public void CancelSelection()
    {

    }

    public void AddRootPressed()
    {
        DialogWithSelection.CreateDialog(this, "SELECT ROOT NODE", "SELECTOR will return on first success, SEQUENCE will return on first failure. Recommended as a m_root: SELECTOR", 
            "Selector", "Sequence", "Cancel", CreateSelector, CreateSequence, Cancel);
    }

    public void ExitPressed()
    {
        GetParent().GetParent<Node2D>().QueueFree();
        
    }
    //Control pozicija je doljnji desni ugao

    public void CreateSelector()
    {
        currentNode = BehaviorNode.CreateNode(new Vector2((1280 / 2) + (BehaviorNode.size.x * 0.5f) / 2, (720 / 2) + (BehaviorNode.size.y * 0.5f) / 2), "Selector", "ROOT", null);
        currentNode.GoldNodeName = "Selector";
        addRootBtn.Visible = false;
    }
    public void CreateSequence()
    {
        currentNode = BehaviorNode.CreateNode(new Vector2((1280 / 2) + (BehaviorNode.size.x * 0.5f) / 2, (720 / 2) + (BehaviorNode.size.y * 0.5f) / 2), "Sequence", "ROOT", null);
        currentNode.GoldNodeName = "Sequence";
        addRootBtn.Visible = false;

    }
    public void Cancel()
    {

    }

    private void FileDialog_DirSelected(string dir)
    {
        


        // Process the list of files found in the directory. 
        string[] fileEntries = System.IO.Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories).Select(System.IO.Path.GetFileNameWithoutExtension).Select(p => p.Substring(0)).ToArray();



        availableNodes = new string[fileEntries.Length + 2];

        availableNodes[0] = "Sequence";
        availableNodes[1] = "Selector";

        int i = 2;
        foreach (string fileName in fileEntries)
        {
            availableNodes[i] = fileName;
            i++;
        }

        GD.Print(fileDialog.CurrentDir.ToString());

    }

    private void OpenTaskDir()
    {
        fileDialog.PopupCentered();
    }

   


}
