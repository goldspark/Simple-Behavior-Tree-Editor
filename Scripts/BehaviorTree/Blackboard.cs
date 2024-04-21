using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.Scripts.BehaviorTree
{
    /**
     * Dummy Vector class just for data example. You can safely remove this
     */
    public class Vec{

    }

    /*
     * Use this to emulate Blackboards like in Unreal Engine
     * Modify it for your own use
     */
    public class Blackboard
    {
        //Modify this to include all sorts of keys you might want for your game
        public enum Keys
        {
            Position,
            Rotation,
            Waypoints
        }

        public Dictionary<Keys, Vec> Positions = new Dictionary<Keys, Vec>();
        public Dictionary<Keys, float> Rotations = new Dictionary<Keys, float>();
        public Dictionary<Keys, List<Vec>> Waypoints = new Dictionary<Keys, List<Vec>>();


    }
}
