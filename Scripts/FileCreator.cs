﻿using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.AIEditor
{
    public static class FileCreator
    {

        public static Dictionary<string, string> parents = new Dictionary<string, string>();


        public static void SaveAI(BehaviorNode node, string name)
        {
            Godot.File saveGame = new Godot.File();
            StringBuilder stringBuilder = new StringBuilder();

            saveGame.Open("MyAI/" + name + ".ai", Godot.File.ModeFlags.Write);

            stringBuilder.Append("[Children]\n");
            SaveChildren(stringBuilder, node);

            saveGame.StoreString(stringBuilder.ToString());
            saveGame.Close();
        }

        public static List<string> GetChildrenOfParent(string text, string parentName)
        {
            List<string> result = new List<string>();
            string[] lines = text.Split('\n');

            foreach(string line in lines)
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

        private static void SaveChildren(StringBuilder stringBuilder, BehaviorNode node)
        {
            BehaviorNode currentNode = node;

            
            stringBuilder.Append(currentNode.title.Text + "" + currentNode.id);
            

            if(currentNode.parent != null)
            {
                stringBuilder.Append("[Parent=" + currentNode.parent.title.Text + "" +currentNode.parent.id +"]\n");
            }

            if(currentNode.parent == null)
                stringBuilder.AppendLine();

            for(int i = 0; i < currentNode.children.Count; i++)
            {
                SaveChildren(stringBuilder, currentNode.children[i]);
            }
            
        }

       
        public static void ReadAIFile(string filename)
        {
            StreamReader reader = new StreamReader(filename);
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
                    parents[childName] = parentName;
                }
                else
                {
                    parents[line] = null;
                }
            }

            //GD.Print("Children:");
            foreach (string childName in parents.Keys)
            {
                if (parents[childName] == null)
                {
                    GD.Print(childName);
                }
            }

            //GD.Print("\nChildren with parents:");
            foreach (string childName in parents.Keys)
            {
                if (parents[childName] != null)
                {
                    //GD.Print(childName + " has parent: " + parents[childName]);
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
