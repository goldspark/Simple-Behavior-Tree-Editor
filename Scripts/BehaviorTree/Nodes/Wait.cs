using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Scripts.AI.BehaviorTree.Nodes
{
    public class Wait : GoldNode
    {
        float setWaitTime = 2f;
        float waitTime;
        public Wait(GoldTreeBase tree) : base(tree)
        {
        }

        public override void InitVarsFromLoader()
        {
            setWaitTime = GetBB().GetFloat("waitTime");
        }

        public override ReturnType Update(float delta)
        {

            waitTime -= delta;
            if (waitTime < 0)
            {
                waitTime = setWaitTime;
                return ReturnType.SUCCESS;
            }
            
            return ReturnType.RUNNING;
        }

    }
}
