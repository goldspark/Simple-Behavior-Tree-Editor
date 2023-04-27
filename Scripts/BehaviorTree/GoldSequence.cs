using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{
    public class GoldSequence : GoldNode
    {

        public GoldSequence() { }

        public override ReturnType Update()
        {


            foreach (GoldNode child in children)
            {
                switch (child.Update())
                {
                    case ReturnType.SUCCESS:
                        continue;
                    case ReturnType.FAILURE:
                        return ReturnType.FAILURE;
                    case ReturnType.RUNNING:
                        continue;
                    default:
                        return ReturnType.RUNNING;
                }
            }


            return ReturnType.SUCCESS;
        }
    }
}
