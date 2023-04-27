using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree.Examples
{
    public class Task_PathBlocked : GoldNode
    {
        private Object npc;
        public Task_PathBlocked(Object npc) {
            this.npc = npc;
        }

        public override ReturnType Update()
        {

            /**
             * For example in unity check the raycast from the gameobjects position
             * to check whether a path is blocked or not
             */

            return ReturnType.FAILURE;
        }
    }
}
