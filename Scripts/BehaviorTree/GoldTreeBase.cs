using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{

    public enum RootType
    {
        SELECTOR,
        SEQUENCE
    }

    /// <summary>
    /// NPCs should derive from this class as it's a base
    /// class for NPC behavior.
    /// <example>
    /// Example:
    /// public class SentryTurretAI : GoldTreeBase{
    /// 
    /// }
    /// </example>
    ///
    /// </summary>
    public abstract class GoldTreeBase
    {

        public GoldNode root;

        public GoldTreeBase(RootType rootType)
        {
            Initialize(rootType);
        }



        private void Initialize(RootType rootType)
        {
            if(rootType == RootType.SELECTOR)
            {
                root = new GoldSelector();
            }
            Start();

            root.tree = this;

            World.npcs.Add(this);
        }

        public abstract void Start();

        /// <summary>
        /// Add tree nodes 
        /// </summary>
        /// <param name="nodes">Add nodes</param>
        public void AddChildren(List<GoldNode> nodes)
        {
            foreach (GoldNode node in nodes)
            {
                root.children.Add(node);
            }
        }

        public void Update()
        {
            root.Update();
        }


    }
}
