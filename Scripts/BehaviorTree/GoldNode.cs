using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{

    public enum ReturnType
    {
        SUCCESS,
        FAILURE,
        RUNNING
    }

    /// <summary>
    /// Base class for any nodes to derive from this one including Task Nodes
    /// <example>
    /// For example:
    /// <code>
    /// public class Task_Run : GoldNode {
    /// 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class GoldNode
    {
        public GoldTreeBase tree;
        public List<GoldNode> children = new List<GoldNode>();
       
        /// <summary>
        /// Unique identifier name so the AILoader can know
        /// where to attach this node to.
        /// </summary>
        public string uniqueIDName;
       
        public GoldNode parent = null;
        public GoldNode() { }
 

        public void AddChildren(List<GoldNode> nodes)
        {
            foreach (GoldNode node in nodes)
            {
                node.parent = this;
                node.tree = tree;
                children.Add(node);
            }
        }

        public void Attach(GoldNode node)
        {
            node.parent = this;
            node.tree = tree;
            children.Add(node);
        }

        public abstract ReturnType Update();
        

       

    }
}
