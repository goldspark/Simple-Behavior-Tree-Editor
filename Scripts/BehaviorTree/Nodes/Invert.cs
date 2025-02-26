using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Scripts.AI.BehaviorTree.Nodes
{
    public class Invert : GoldNode
    {
        public Invert(GoldTreeBase tree) : base(tree)
        {

        }

        public override ReturnType Update(float delta)
        {
            foreach (GoldNode child in children)
            {
                switch (child.Update(delta))
                {
                    case ReturnType.SUCCESS:
                        return ReturnType.FAILURE;
                    case ReturnType.FAILURE:
                        return ReturnType.SUCCESS;
                    default:
                        continue;
                }
            }

            return ReturnType.RUNNING;
        }
    }
}
