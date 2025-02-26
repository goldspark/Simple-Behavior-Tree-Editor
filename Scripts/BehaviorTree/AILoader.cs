using SpaceGame.Scripts.AI.BehaviorTree;
using SpaceGame.Scripts.AI.BehaviorTree.Nodes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        public class StringPair
        {
            public StringPair(string nodeName, string varName)
            {
                NodeName = nodeName;
                VariableName = varName;
            }
            public string NodeName;
            public string VariableName;

            public override string ToString()
            {
                return $"StringPair: NodeName:{NodeName}, VariableName:{VariableName}";
            }

            public override bool Equals(object obj)
            {
                StringPair other = obj as StringPair;
                return (NodeName == other.NodeName) && (VariableName == other.VariableName);
            }

            public override int GetHashCode()
            {
                int hash1 = NodeName.GetHashCode();
                int hash2 = VariableName.GetHashCode();
                return hash1 + hash2;
            }
        }

        private static StringPair s_VarLoader = new StringPair("", "");

        private static string parentTag = "(Parent=";
        //Key - uniqueIdName Value - variable name
        private static Dictionary<StringPair, float> Floats = new Dictionary<StringPair, float>();
        private static Dictionary<StringPair, int> Integers = new Dictionary<StringPair, int>();
        private static Dictionary<StringPair, bool> Booleans = new Dictionary<StringPair, bool>();
        private static Dictionary<StringPair, string> Strings = new Dictionary<StringPair, string>();



        public static Dictionary<string, string> parentsD = new Dictionary<string, string>();
        private static List<GoldNode> parents = new List<GoldNode>();

        public static GoldNode LoadBHT(GoldTreeBase tree, string fileContent, bool isBase64 = true)
        {
            GoldNode root = null;

            parents.Clear();

            if (isBase64)
            {
                byte[] b = System.Convert.FromBase64String(fileContent);
                fileContent = Encoding.UTF8.GetString(b);
            }

            //Parse AI file
            ReadAIFile(fileContent);

            foreach (string nodeName in parentsD.Keys)
            {
                //Remove number id from the name of the node
                string outputString = Regex.Replace(nodeName, @"\d+", "");

                //First extract root node
                if (parentsD[nodeName] == null)
                {

                    if (outputString == "Selector")
                    {
                        root = new GoldSelector(tree);
                        root.uniqueIDName = nodeName;
                    }
                    else if (outputString == "Sequence")
                    {
                        root = new GoldSequence(tree);
                        root.uniqueIDName = nodeName;
                    }

                    parents.Add(root);

                }
                else //Then find every node inside [] of the txt file
                {
                    string nodeNameNoNumber = Regex.Replace(parentsD[nodeName], @"\d+", "");


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
                        parents.Add(CreateNodeByName(tree, fileContent, parentsD[nodeName], nodeNameNoNumber));
                    }

                }
            }

            AttachChildren(tree, fileContent);
            return root;
        }

        public static float LoadFloat(string nodeUniqueId, string varName)
        {
            s_VarLoader.NodeName = nodeUniqueId;
            s_VarLoader.VariableName = varName;
            return Floats[s_VarLoader];
        }

        public static bool LoadBool(string nodeUniqueId, string varName)
        {
            s_VarLoader.NodeName = nodeUniqueId;
            s_VarLoader.VariableName = varName;
            return Booleans[s_VarLoader];
        }

        public static string LoadString(string nodeUniqueId, string varName)
        {
            s_VarLoader.NodeName = nodeUniqueId;
            s_VarLoader.VariableName = varName;
            if (Strings.ContainsKey(s_VarLoader))
                return Strings[s_VarLoader];
            return null;
        }

        public static int LoadInt(string nodeUniqueId, string varName)
        {
            s_VarLoader.NodeName = nodeUniqueId;
            s_VarLoader.VariableName = varName;
            return Integers[s_VarLoader];
        }


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
        private static GoldNode CreateNodeByName(GoldTreeBase tree, string fileContent, string uniqueIdentifierName, string name)
        {
            GoldNode node;
            switch (name)
            {
                case "Selector":
                    node = new GoldSelector(tree);
                    break;
                case "Sequence":
                    node = new GoldSequence(tree);
                    break;
                case "Parallel":
                    node = new GoldParallel(tree);
                    break;
                case "Invert":
                    node = new Invert(tree);
                    break;
                case "Wait":
                    node = new Wait(tree);
                    break;
                default:
                    node = new GoldSelector(tree);
                    break;
            }
            node.uniqueIDName = uniqueIdentifierName;
            LoadBB(tree, uniqueIdentifierName, fileContent);
            node.InitVarsFromLoader();
            return node;
        }

        private static void AttachChildren(GoldTreeBase tree, string text)
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
                        parents[i].Attach(CreateNodeByName(tree, text, childName, outputString));
                    }

                }
            }
        }

        private static void ReadAIFile(string fileContent)
        {

            StringReader reader = new StringReader(fileContent);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("[Tree]"))
                {
                    continue;
                }
                else if (line.Contains(parentTag))
                {
                    int startIndex = line.IndexOf(parentTag) + 8;
                    int endIndex = line.IndexOf(")", startIndex);
                    string parentName = line.Substring(startIndex, endIndex - startIndex);
                    string childName = line.Substring(0, line.IndexOf(parentTag));
                    parentsD[childName] = parentName;
                }
                else
                {
                    parentsD[line] = null;
                }
            }
            reader.Close();
        }

        private static List<string> GetChildrenOfParent(string text, string parentName)
        {
            List<string> result = new List<string>();
            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains($"(Parent={parentName})"))
                {
                    int start = line.IndexOf(parentTag) + 8;
                    string childName = line.Substring(0, start - 8);
                    result.Add(childName);
                }
            }

            return result;
        }

        private static void LoadBB(GoldTreeBase tree, string nodeName, string content)
        {
            bool skipAboveText = true;
            string[] lines = content.Split('\n');
            int i = 0;
            foreach (string s in lines)
            {
                if (skipAboveText && !s.Contains($"[{nodeName}]"))
                    continue;
                if (s.Contains($"[{nodeName}]") && skipAboveText)
                {
                    skipAboveText = false;
                    continue;
                }
                if (s.Contains("[") && s.Contains("]") && !s.Contains($"[{nodeName}]"))
                    break;
                AnalyzeType(tree, nodeName, s);

            }

        }

        private static string GetVarValue(string s)
        {
            int start = s.IndexOf('=') + 1;
            string val = s.Remove(0, start);
            string newVal = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] == ';')
                    break;
                if (val[i] == ' ')
                    continue;
                if (val[i] == '"')
                    continue;

                newVal += val[i];
            }
            return newVal;
        }


        private static string GetVarName(string s, string type)
        {
            string name = s.Remove(0, type.Length);
            string newName = "";

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ' ')
                    continue;
                if (name[i] == '=')
                    break;

                newName += name[i];

            }

            return newName;
        }

        private static void AnalyzeType(GoldTreeBase tree, string nodeName, string line)
        {
            string[] types =
            {
                "Key.AIEntity",
                "Key.Int",
                "Key.Float",
                "Key.String",
                "Key.Bool",
                "BB.Condition"
            };

            string[] vartypes =
            {
                "AIEntity",
                "Int",
                "Float",
                "String",
                "Bool"
            };

            foreach (string t in vartypes)
                if (line.Contains(t) && !line.Contains("Key."))
                {
                    switch (t)
                    {
                        case "AIEntity":
                            break;
                        case "Int":
                            {
                                string name = GetVarName(line, t);
                                int value = Convert.ToInt32(GetVarValue(line));
                                StringPair pair = new StringPair(nodeName, name);
                                if (Integers.ContainsKey(pair))
                                    Integers.Remove(pair);
                                Integers.Add(pair, value);
                            }
                            break;
                        case "Float":
                            {
                                string name = GetVarName(line, t);
                                float value = (float)Convert.ToDouble(GetVarValue(line));
                                StringPair pair = new StringPair(nodeName, name);
                                if (Floats.ContainsKey(pair))
                                    Floats.Remove(pair);
                                Floats.Add(pair, value);
                            }
                            break;
                        case "String":
                            {
                                string name = GetVarName(line, t);
                                string value = GetVarValue(line);
                                StringPair pair = new StringPair(nodeName, name);
                                if (Strings.ContainsKey(pair))
                                    Strings.Remove(pair);
                                Strings.Add(pair, value);
                            }
                            break;
                        case "Bool":
                            {
                                string name = GetVarName(line, t);
                                bool value = Convert.ToBoolean(GetVarValue(line));
                                StringPair pair = new StringPair(nodeName, name);
                                if (Booleans.ContainsKey(pair))
                                    Booleans.Remove(pair);
                                Booleans.Add(pair, value);
                            }
                            break;
                        default:
                            break;
                    }
                }

            foreach (string t in types)
                if (line.Contains(t))
                {
                    switch (t)
                    {
                        case "Key.AIEntity":
                            break;
                        case "Key.Int":
                            {
                                string name = GetVarName(line, t);
                                int value = Convert.ToInt32(GetVarValue(line));
                                if (!tree.GetBB().Keys.Contains(name))
                                    tree.GetBB().Keys.AddKey(name);
                                tree.GetBB().SetInt(name, value);
                            }
                            break;
                        case "Key.Float":
                            {
                                string name = GetVarName(line, t);
                                float value = (float)Convert.ToDouble(GetVarValue(line));

                                if (!tree.GetBB().Keys.Contains(name))
                                    tree.GetBB().Keys.AddKey(name);
                                tree.GetBB().SetFloat(name, value);
                            }
                            break;
                        case "Key.String":
                            {
                                string name = GetVarName(line, t);
                                string value = GetVarValue(line);

                                if (!tree.GetBB().Keys.Contains(name))
                                    tree.GetBB().Keys.AddKey(name);
                                tree.GetBB().SetString(name, value);
                            }
                            break;
                        case "Key.Bool":
                            {
                                string name = GetVarName(line, t);
                                bool value = Convert.ToBoolean(GetVarValue(line));

                                if (!tree.GetBB().Keys.Contains(name))
                                    tree.GetBB().Keys.AddKey(name);
                                tree.GetBB().SetBool(name, value);
                            }
                            break;
                        case "BB.Condition":
                            {
                                string value = GetVarValue(line);
                                if (tree.GetBB().Keys.Contains(value))
                                    tree.GetBB().Conditions.Remove(value);
                                tree.GetBB().Conditions.Add(value, value);
                            }
                            break;
                        default:
                            break;
                    }
                }

        }

    }





}

