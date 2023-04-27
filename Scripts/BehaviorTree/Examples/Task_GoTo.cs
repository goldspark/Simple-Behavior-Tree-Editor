using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree.Examples
{
    public class Task_GoTo : GoldNode
    {
        private Object npc;

        public Task_GoTo(Object npc) {
            this.npc = npc;
        }

        public override ReturnType Update()
        {
            /**
             * Add move function here
             * 
             */

            return ReturnType.SUCCESS;
        }
    }
}
