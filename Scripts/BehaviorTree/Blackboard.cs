using BehaviorTree;
using GoldTypes;
using System;
using System.Collections.Generic;


namespace SimpleBehaviorTreeEditor.Scripts.BehaviorTree
{

    /*
     * Use this to store and retreive data for your AI
     * Modify it for your own use
     */
    public class Blackboard
    {

        public GoldEnum Keys = new GoldEnum(new List<string> { });

        public Dictionary<string, string> Conditions = new Dictionary<string, string>(); //Necessary, used for nodes which are scripted to check if condition is set before starting

        private Dictionary<int, AIEntity> Entities = new Dictionary<int, AIEntity>();
        private Dictionary<int, int> Integers = new Dictionary<int, int>();
        private Dictionary<int, bool> Booleans = new Dictionary<int, bool>();
        private Dictionary<int, float> Floats = new Dictionary<int, float>();
        private Dictionary<int, string> Strings = new Dictionary<int, string>();


        public void SetEntity(string key, AIEntity entity)
        {
            if (Entities.ContainsKey(Keys[key]))
            {
                Entities[Keys[key]] = entity;
                return;
            }
            Entities.Add(Keys[key], entity); 
        }

        public void SetInt(string key, int value)
        {
            if (Integers.ContainsKey(Keys[key]))
            {
                Integers[Keys[key]] = value;
                return;
            }
            Integers.Add(Keys[key], value);
        }

        public void SetBool(string key, bool value)
        {
            if (Booleans.ContainsKey(Keys[key]))
            {
                Booleans[Keys[key]] = value;
                return;
            }
            Booleans.Add(Keys[key], value);
        }

        public void SetString(string key, string value)
        {
            if (Strings.ContainsKey(Keys[key]))
            {
                Strings[Keys[key]] = value;
                return;
            }
            Strings.Add(Keys[key], value);
        }

        public void SetFloat(string key, float value)
        {
            if (Floats.ContainsKey(Keys[key]))
            {
                Floats[Keys[key]] = value;
                return;
            }
            Floats.Add(Keys[key], value);
        }


        public AIEntity GetEntity(string key)
        {
            if (!Entities.TryGetValue(Keys[key], out var entity))
                entity = null;
            return entity; 
        }

        public int GetIntegers(string key)
        {
            return Integers[Keys[key]];
        }

        public bool GetBoolean(string key)
        {
            return Booleans[Keys[key]];
        }

        public float GetFloat(string key)
        {
            return Floats[Keys[key]];
        }

        public string GetString(string key)
        {
            return Strings[Keys[key]];
        }

    }
}
