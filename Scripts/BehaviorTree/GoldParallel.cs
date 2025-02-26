using SimpleBehaviorTreeEditor.BehaviorTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Scripts.AI.BehaviorTree
{
    public class GoldParallel : GoldNode
    {
      

        public GoldParallel(GoldTreeBase tree) : base(tree)
        {

        }

        public override ReturnType Update(float delta)
        {
            int numOfSuccess = 0;
            int numOfFailure = 0;


            foreach (GoldNode node in children)
            {
               
                switch (node.Update(delta))
                {
                    case ReturnType.SUCCESS:
                        numOfSuccess++;
                        break;
                        case ReturnType.FAILURE:
                        numOfFailure++; 
                        break;
                    case ReturnType.RUNNING:
                        break;
                    default:
                        throw new InvalidOperationException("Invalid return type for GoldParallel");
                }
            }


            if (numOfSuccess >= 2)
                return ReturnType.SUCCESS;         
            else if (numOfFailure >= 2)
                return ReturnType.FAILURE;

            return ReturnType.RUNNING;
        }
    }
}
