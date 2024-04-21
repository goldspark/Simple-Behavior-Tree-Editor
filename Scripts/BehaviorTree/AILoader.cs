using Godot;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace SimpleBehaviorTreeEditor.BehaviorTree
{

    /// <summary>
    /// Used to load saved AI file
    /// made by using Behavior Tree Editor.
    /// 
    /// Simply just use LoadBHTFile function at the start of "Start()" function of GoldTreeBase derived classes.
    /// </summary>
    public static class AILoader
    {

        private static List<GoldNode> parents = new List<GoldNode>();
        public static Dictionary<string, string> parentsD = new Dictionary<string, string>();


        /// <summary>
        /// Creates specific node according to its name.
        /// You have to add more names of the nodes in order for this to work.
        /// <example>
        /// For example:
        /// <code>
        /// case "MyCustomNodeName":
        ///  node = new CustomNode(this);
        ///  break;
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="uniqueIdentifierName"> Used by AILoader to know where to attach this node to</param>
        /// <param name="name"> Name without the ID in the name</param>
        /// <returns></returns>
        private static GoldNode CreateNodeByName(string uniqueIdentifierName, string name)
        {
            GoldNode node;
            switch (name)
            {
                case "Selector":
                    node = new GoldSelector();
                    break;
                case "Sequence":
                    node = new GoldSequence();
                    break;
                default:
                    node = new GoldSelector();
                    break;
            }
            node.uniqueIDName = uniqueIdentifierName;

            return node;
        }

        private static void AttachChildren(string text)
        {
            for (int i = 0; i < parents.Count; i++)
            {
                //Get children of this parent
                foreach (string childName in GetChildrenOfParent(text, parents[i].uniqueIDName))
                {


                    //Check whether this child already exists in parent class if not create new
                    bool foundParentChild = false;
                    for (int j = 0; j < parents.Count; j++)
                    {

                        if (childName == parents[j].uniqueIDName)
                        {
                            parents[i].Attach(parents[j]);
                            foundParentChild = true;
                            break;
                        }
                    }

                    if (!foundParentChild)
                    {
                        string outputString = Regex.Replace(childName, @"\d+", "");
                        parents[i].Attach(CreateNodeByName(childName, outputString));
                    }

                }
            }
        }

        public static GoldNode LoadBHTFile(string filePath)
        {
            GoldNode root = null;

            parents.Clear();

            string myAIDirectoryPath = filePath;

            Godot.File saveGame = new Godot.File();
            saveGame.OpenEncryptedWithPass(filePath, Godot.File.ModeFlags.Read, "Ligmasin");
            if (!saveGame.FileExists(filePath))
            {
                return null;
            }

            string text = saveGame.GetAsText();
            saveGame.Close();

            //Parse AI file
            ReadAIFile(myAIDirectoryPath);

            foreach (string nodeName in parentsD.Keys)
            {
                //Remove number id from the name of the node
                string outputString = Regex.Replace(nodeName, @"\d+", "");

                //First extract root node
                if (parentsD[nodeName] == null)
                {

                    if (outputString == "Selector")
                    {
                        root = new GoldSelector();
                        root.uniqueIDName = nodeName;
                    }
                    else if (outputString == "Sequence")
                    {
                        root = new GoldSequence();
                        root.uniqueIDName = nodeName;
                    }

                    parents.Add(root);

                }
                else //Then find every node inside [] of the txt file
                {
                    string outputString2 = Regex.Replace(parentsD[nodeName], @"\d+", "");


                    bool exists = false;
                    for (int i = 0; i < parents.Count; i++)
                    {
                        if (parentsD[nodeName] == parents[i].uniqueIDName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        parents.Add(CreateNodeByName(parentsD[nodeName], outputString2));
                    }

                }
            }

            AttachChildren(text);

            return root;
        }

        private static void ReadAIFile(string filename)
        {
            Godot.File saveGame = new Godot.File();
            saveGame.OpenEncryptedWithPass(filename, Godot.File.ModeFlags.Read, "Ligmasin");
            if (!saveGame.FileExists(filename))
            {
                GD.Print("File does not exist");
                return;
            }


            StringReader reader = new StringReader(saveGame.GetAsText());
            string line;


            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("[Children]"))
                {
                    continue;
                }
                else if (line.Contains("[Parent="))
                {
                    int startIndex = line.IndexOf("[Parent=") + 8;
                    int endIndex = line.IndexOf("]", startIndex);
                    string parentName = line.Substring(startIndex, endIndex - startIndex);
                    string childName = line.Substring(0, line.IndexOf("[Parent="));
                    parentsD[childName] = parentName;
                }
                else
                {
                    parentsD[line] = null;
                }
            }

            saveGame.Close();
            reader.Close();
        }

        private static List<string> GetChildrenOfParent(string text, string parentName)
        {
            List<string> result = new List<string>();
            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains($"[Parent={parentName}]"))
                {
                    int start = line.IndexOf("[Parent=") + 8;
                    string childName = line.Substring(0, start - 8);
                    result.Add(childName);
                }
            }

            return result;
        }
    }





}

