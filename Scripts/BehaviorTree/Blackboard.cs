using System.Collections.Generic;


namespace SimpleBehaviorTreeEditor.Scripts.BehaviorTree
{
    /**
     * Dummy Vector class just for data example. You can safely remove this
     */
    public class Vec{

    }


    //Modify this to include all sorts of keys you might want for your game
    public enum Keys
    {
        Position,
        IsHappy
    }

    /*
     * Use this to store and retreive data for your AI
     * Modify it for your own use
     */
    public class Blackboard
    {
        

        private Dictionary<Keys, Vec> m_Vectors = new Dictionary<Keys, Vec>();
        private Dictionary<Keys, bool> m_Booleans = new Dictionary<Keys, bool>();


        public void SetVector(Keys key, Vec value)
        {
            m_Vectors[key] = value;
        }

        public void SetBoolean(Keys key, bool value)
        {
            m_Booleans[key] = value;
        }

        public Vec GetVector(Keys key)
        {
            return m_Vectors[key];
        }
        public bool GetBoolean(Keys key)
        {
            return m_Booleans[key];
        }
    }
}
