using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree
{
    public class GoldSelector : GoldNode
    {
        public int currentIndex = 0;
        public GoldSelector(GoldTreeBase tree) : base(tree) { }

        public override ReturnType Update(float delta)
        {
            if (tree.Interrupt && parent != null)
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
                        currentIndex = 0;
                        return ReturnType.SUCCESS;
                    case ReturnType.FAILURE:
                        currentIndex++;
                        continue;
                    case ReturnType.RUNNING:
                        return ReturnType.RUNNING;
                    default:
                        return ReturnType.RUNNING;
                }
            }
            currentIndex = 0;
            return ReturnType.FAILURE;
        }

    }
}
