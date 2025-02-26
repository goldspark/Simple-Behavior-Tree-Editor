using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using File = System.IO.File;

namespace SimpleBehaviorTreeEditor.AIEditor
{
    public static class FileCreator
    {
        private static List<string> code = new List<string>();
        public static Dictionary<string, string> parents = new Dictionary<string, string>();
        public static string AICode = null;

        public static void SaveAI(BehaviorNode node, string name)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append("[Tree]\n");
            SaveNodes(stringBuilder, node);
            foreach(var s in code)
                stringBuilder.Append(s + "\n");

            File.WriteAllText("MyAI/" + name + ".ai", stringBuilder.ToString());
      

            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            string base64String = Convert.ToBase64String(bytes);
            //Convert.FromBase64String(base64String);
            //Encoding.UTF8.GetString(bytes);
            File.WriteAllText("MyAI/" + name + ".aie", base64String);
            AICode = base64String;
        }

        public static List<string> GetChildrenOfParent(string text, string parentName)
        {
            List<string> result = new List<string>();
            string[] lines = text.Split('\n');

            foreach(string line in lines)
            {
                if (line.Contains($"(Parent={parentName})"))
                {
                    int start = line.IndexOf("(Parent=") + 8;
                    string childName = line.Substring(0, start - 8);
                    result.Add(childName);
                }
            }

            return result;
        }

        private static void SaveNodes(StringBuilder stringBuilder, BehaviorNode node)
        {

            BehaviorNode currentNode = node;

            stringBuilder.Append(currentNode.uniqueName);
            if (currentNode.code.Count > 0)
            {
                code.Add($"[{currentNode.uniqueName}]");
                code.AddRange(currentNode.code);
            }

            if(currentNode.parent != null)
                stringBuilder.Append("(Parent=" + currentNode.parent.uniqueName + ")\n");

            if (currentNode.parent == null)
                stringBuilder.AppendLine();

            for(int i = 0; i < currentNode.children.Count; i++)
            {
                SaveNodes(stringBuilder, currentNode.children[i]);
            }
            
        }

       
        public static void ReadAIFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }
            

            StringReader reader = new StringReader(File.ReadAllText(filename));
            string line;


            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("[Tree]"))
                {
                    continue;
                }
                else if (line.Contains("(Parent="))
                {
                    int startIndex = line.IndexOf("(Parent=") + 8;
                    int endIndex = line.IndexOf(")", startIndex);
                    string parentName = line.Substring(startIndex, endIndex - startIndex);
                    string childName = line.Substring(0, line.IndexOf("(Parent="));
                    parents[childName] = parentName;
                }
                else
                {
                    parents[line] = null;
                }
            }

            reader.Close();
        }
    }

    /*
     * All in all parents have a child as a Key and parent is an item in a dictionary
     * So TKey is a child and TValue is a parent
     * 
     */
}
