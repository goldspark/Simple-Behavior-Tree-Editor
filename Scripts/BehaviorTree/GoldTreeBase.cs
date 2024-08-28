using SimpleBehaviorTreeEditor.Scripts.BehaviorTree;
using System.Collections.Generic;


namespace SimpleBehaviorTreeEditor.BehaviorTree
{

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
        public GoldNode m_root;
        private Blackboard m_blackboard;

        public GoldTreeBase()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_blackboard = new Blackboard();
            Start();
            // Can remove this if you are not using stock AIWorld
            if (m_root != null)
            {
                AIWorld.npcs.Add(this); 
            }
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
                m_root.children.Add(node);
            }
        }

        public void Update()
        {
            m_root.Update();
        }

        public Blackboard GetBB() { return m_blackboard; }


    }
}
