using SimpleBehaviorTreeEditor.AIEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                foreach (string childName in FileCreator.GetChildrenOfParent(text, parents[i].uniqueIDName))
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

        public static void LoadBHTFile(GoldNode root, string filePath)
        {
            parents.Clear();

            string myAIDirectoryPath = filePath;
            string text = System.IO.File.ReadAllText(myAIDirectoryPath);

            //Parse AI file
            FileCreator.ReadAIFile(myAIDirectoryPath);

            foreach (string nodeName in FileCreator.parents.Keys)
            {
                //Remove number id from the name of the node
                string outputString = Regex.Replace(nodeName, @"\d+", "");


                if (FileCreator.parents[nodeName] == null)
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
                else
                {
                    bool exists = false;
                    for (int i = 0; i < parents.Count; i++)
                    {
                        if (FileCreator.parents[nodeName] == parents[i].uniqueIDName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        parents.Add(CreateNodeByName(FileCreator.parents[nodeName], outputString));
                    }

                }
            }

            AttachChildren(text);

        }


    }
}
