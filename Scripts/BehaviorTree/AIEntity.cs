using SimpleBehaviorTreeEditor.BehaviorTree;
using System;

namespace BehaviorTree
{
    public class AIEntity
    {
        public object Owner;
        private GoldTreeBase _tree;
        private int _entityID = 0;
        private static int _nextAvailableID = 0;

        public AIEntity(GoldTreeBase tree, object owner)
        {
            Owner = owner ?? throw new ArgumentNullException("AIEntity:Null owner", "Owner cannot be null"); ;
            SetID();
            AIEntityManager.Get().AddEntity(this);
            if (tree != null)
            {
                _tree = tree;
                _tree.owner = this;
            }
        }

        public GoldTreeBase BHT()
        {
            return _tree;
        }
        public void Update(float delta)
        {
            if (_tree != null)
                _tree.Update(delta);
        }

        public void SetID()
        {
            _entityID = _nextAvailableID++;
        }

        public int GetID()
        {
            return _entityID;
        }

    }
}
