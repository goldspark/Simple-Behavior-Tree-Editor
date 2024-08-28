using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{
    public class GoldSelector : GoldNode
    {
        public GoldSelector() : base(null) { }

        public override ReturnType Update()
        {
          

            foreach (GoldNode child in children)
            {
                switch(child.Update())
                {
                    case ReturnType.SUCCESS:
                        return ReturnType.SUCCESS;
                    case ReturnType.FAILURE:
                        continue;
                    case ReturnType.RUNNING:
                        return ReturnType.RUNNING;
                    default:
                        return ReturnType.RUNNING;
                }
            }


            return ReturnType.FAILURE;
        }
    }
}
