using BehaviorTree;
using Godot;
using SimpleBehaviorTreeEditor.AIEditor;
using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using File = System.IO.File;
using Object = Godot.Object;

public class AIEditor : Control
{
    public static AIEditor Instance { get; private set; }


    /*IMPORTANT
    * 
    * MUST HAVE AvailableNodes.txt file
    */
    public static string[] availableNodes;

    //Global ID for nodes
    public static int GLOBAL_ID = 0;


    PackedScene dialogPackedScene;
    public string nameOfFile;
    public BehaviorNode currentNode;
    public BehaviorNode selectedNode;
    public static BehaviorNode rootNode;
    public TextureButton addRootBtn;
    public List<Selection> selectionList = new List<Selection>();
    public Label title, selectedNodeLabel;
    public Node2D editText;
    public Node2D dialogs;
    public RichTextLabel explanationText;
    public FileDialog fileDialog;
    private ConfigFile m_config;

    private AIEditor()
    {
        if (Instance != null && Instance != this)
            Instance.QueueFree();
        else
            Instance = this;
    }


    public override void _Ready()
    {       
        m_config = new ConfigFile();

        Instance = this;
        dialogPackedScene = ResourceLoader.Load<PackedScene>("res://Scripts/DialogWithSelection.tscn");

        title = GetNode<Label>("Title");
        selectedNodeLabel = GetNode<Label>("SelectedNode");
        addRootBtn = GetNode<TextureButton>("AddRoot");
        dialogs = GetNode<Node2D>("Dialogs");
        editText = GetNode<Node2D>("EditText");
        fileDialog = GetParent().GetNode<FileDialog>("FileDialog");

        //Create save folder for this system
        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName("MyAI")))
        {
            System.IO.Directory.CreateDirectory("MyAI");
        }

        availableNodes = new string[3];

        availableNodes[0] = "Sequence";
        availableNodes[1] = "Selector";
        availableNodes[2] = "Parallel";

        if (LoadTaskDir())
            FileDialog_DirSelected(fileDialog.CurrentDir);
        if(LoadBlackboardDir())
            FileDialog_BlackboardSelected(fileDialog.CurrentFile);

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
      bool keyReleased = false;
      public override void _Process(float delta)
      {
        if (!IsInstanceValid(AIEditor.Instance.currentNode))
        {
            AIEditor.Instance.addRootBtn.Visible = true;
        }
        
        if (Input.IsKeyPressed((int)KeyList.Control) && Input.IsKeyPressed((int)KeyList.S))
        {
            if (!keyReleased)
            {
                Save();
                keyReleased = true;
            }
        }
        else
        {
            if (keyReleased)
                keyReleased = false;
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
            AIWorld.lockMovement = true;
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
        nameOfFile = Selection.PressedInstance.buttonName.Text;
        FileCreator.ReadAIFile("MyAI/" + Selection.PressedInstance.buttonName.Text);
        title.Text = Selection.PressedInstance.buttonName.Text;
        Godot.File saveGame = new Godot.File();
        saveGame.Open("MyAI/" + Selection.PressedInstance.buttonName.Text, Godot.File.ModeFlags.Read);
        string text = saveGame.GetAsText();
        saveGame.Close();


        foreach (string key in FileCreator.parents.Keys)
        {
            keys.Add(key);
        }

        GLOBAL_ID = 0;
        //Create root first
        string noNumberName = Regex.Replace(keys[0], @"\d+", "");

        if (noNumberName == "Selector")
            CreateSelector();
        else
            CreateSequence();
      
         AttachChildrenID(text, currentNode, keys[0]);   
        
        //Load in the aie content:
        string noExtensionName = nameOfFile.Substring(0, nameOfFile.IndexOf("."));
        FileCreator.AICode = File.ReadAllText("MyAI/" + noExtensionName + ".aie");


    }

    private void AttachChildren(string text, BehaviorNode node, string parentName)
    {

        foreach (string childName in FileCreator.GetChildrenOfParent(text, parentName))
        {
            string noNumberName = Regex.Replace(childName, @"\d+", "");
            BehaviorNode n = node.AddChild(noNumberName);
            n.uniqueName = childName;
            node.code.Clear();    
            
            string content = File.ReadAllText("MyAI/" + AIEditor.Instance.nameOfFile);
            //Load the code
            bool skipAboveText = true;
            string[] lines = content.Split('\n');
            foreach (string s in lines)
            {

                if (skipAboveText && !s.Contains($"[{n.uniqueName}]"))
                    continue;
                if (s.Contains($"[{n.uniqueName}]") && skipAboveText)
                {
                    skipAboveText = false;
                    continue;
                }
                if (s.Contains("[") && s.Contains("]") && !s.Contains($"[{node.uniqueName}]"))
                    break;

                n.code.Add(s);
            }


            AttachChildren(text, n, childName);

        }

        RearrangeNodes();

    }

    private void AttachChildrenID(string text, BehaviorNode node, string parentName)
    {

        foreach (string childName in FileCreator.GetChildrenOfParent(text, parentName))
        {
            string sid  = Regex.Match(childName, @"\d+").Value;
            int id = int.Parse(sid);
            string noNumberName = Regex.Replace(childName, @"\d+", "");
            BehaviorNode n = node.AddChildID(noNumberName, id);
            n.uniqueName = childName;
            node.code.Clear();

            string content = File.ReadAllText("MyAI/" + AIEditor.Instance.nameOfFile);
            //Load the code
            bool skipAboveText = true;
            string[] lines = content.Split('\n');
            foreach (string s in lines)
            {

                if (skipAboveText && !s.Contains($"[{n.uniqueName}]"))
                    continue;
                if (s.Contains($"[{n.uniqueName}]") && skipAboveText)
                {
                    skipAboveText = false;
                    continue;
                }
                if (s.Contains("[") && s.Contains("]") && !s.Contains($"[{node.uniqueName}]"))
                    break;

                n.code.Add(s);
            }


            AttachChildrenID(text, n, childName);

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

    public static void SetTitle(string text)
    {
        Instance.title.Text = text;
    }

    private void FileDialog_DirSelected(string dir)
    {
      
        m_config.SetValue("FileDir", "dirPath", dir);
        m_config.Save("user://filepath.cfg");
        // Process the list of files found in the directory. 
        string[] fileEntries = System.IO.Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories).Select(System.IO.Path.GetFileNameWithoutExtension).Select(p => p.Substring(0)).ToArray();

        availableNodes = new string[fileEntries.Length + 3];

        availableNodes[0] = "Sequence";
        availableNodes[1] = "Selector";
        availableNodes[2] = "Parallel";

        int i = 3;
        foreach (string fileName in fileEntries)
        {
            availableNodes[i] = fileName;
            i++;
        }

    }

    private void FileDialog_BlackboardSelected(string dir)
    {
        m_config.SetValue("FileDir", "keys", dir);
        m_config.Save("user://filepath.cfg");

        bool enumFound = false;
        string enumName = "";

        using (StreamReader reader = new StreamReader(dir))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                // Look for the first occurrence of an enum declaration
                if (!enumFound && line.StartsWith("enum "))
                {
                    enumFound = true;

                    // Get the enum name
                    string[] parts = line.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        enumName = parts[1];
                        GD.Print($"Enum Name: {enumName}");
                    }
                }
                // If we're inside an enum, read its members
                else if (enumFound)
                {
                    // Stop reading when we find the end of the enum block
                    if (line.Contains("}"))
                    {
                        break;
                    }

                    // Ignore empty lines and comments
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                    {
                        // Remove potential trailing commas and comments
                        string member = line.Split(new[] { ',', '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                        if (!string.IsNullOrWhiteSpace(member))
                        {
                            GD.Print($" - {member}");
                        }
                    }
                }
            }
        }
    }

    private void OpenTaskDir()
    {
        fileDialog.PopupCentered();

        LoadTaskDir();
    }

   private bool LoadTaskDir()
   {

        m_config.Load("user://filepath.cfg");

        string dirPath = "";

        foreach (string data in m_config.GetSections())
            dirPath = (string)m_config.GetValue(data, "dirPath");// Fetch the data for each section marked in squared brackets, in this example only [FileDir]
        
        if (dirPath.Empty())
            return false;

        fileDialog.CurrentDir = dirPath;
        
        return true;
   }

    private void CopyCode()
    {
        if(FileCreator.AICode != null)
        OS.Clipboard = FileCreator.AICode;
    }

    private bool LoadBlackboardDir()
    {
        m_config.Load("user://filepath.cfg");
        string dir = "";

        foreach (String data in m_config.GetSections())
        {
            // Fetch the data for each section.
            dir = (String)m_config.GetValue(data, "keys", "");
        }

        if (dir.Empty())
        {
            return false;
        }

        return true;
    }

    private void FileDialogConfirm()
    {
        
    }

}
