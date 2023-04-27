using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.BehaviorTree.Examples
{
    internal class SentryTurretAI : GoldTreeBase
    {

        private Object referenceToTheNPChavingThisBehavior = null;

        public SentryTurretAI(RootType rootType) : base(rootType)
        {
            rootType = RootType.SELECTOR;
        }

        public override void Start()
        {
            AILoader.LoadBHTFile(root, "C://DemoFolder//SomeAIFile.txt");
        }
    }
}
