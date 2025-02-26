using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{
    public class GoldSequence : GoldNode
    {
        public int currentIndex = 0;

        public GoldSequence(GoldTreeBase tree) : base(tree) { }

        public override ReturnType Update(float delta)
        {

            if(tree.Interrupt && parent != null)
            {
                currentIndex = 0;
                tree.Interrupt = false;             
                return ReturnType.FAILURE;
            }

            for (int i = currentIndex; i < children.Count; i++)
            { 
                var result = children[i].Update(delta);
                switch (result)
                {
                    case ReturnType.SUCCESS:
                        currentIndex++;
                        continue;
                    case ReturnType.FAILURE:
                        currentIndex = 0;
                        return ReturnType.FAILURE;
                    case ReturnType.RUNNING:
                        return ReturnType.RUNNING;
                    default:
                        throw new InvalidOperationException("GoldSequence: Unrecognized ReturnType");
                }
            }
            currentIndex = 0;
            return ReturnType.SUCCESS;
        }


    }
}
