using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class AIEntityManager
    {
        private static AIEntityManager _instance;
        private Dictionary<int, AIEntity> _entities;

        private AIEntityManager()
        {
            _entities = new Dictionary<int, AIEntity>();
        }

        public static AIEntityManager Get()
        {
            if (_instance == null)
                _instance = new AIEntityManager();
            return _instance;
        }

        public void AddEntity(AIEntity newEntity)
        { 
            if(_entities.ContainsKey(newEntity.GetID()))
                _entities.Remove(newEntity.GetID());
            _entities.Add(newEntity.GetID(), newEntity);
        }

        public Dictionary<int, AIEntity> Entities()
        { 
            return _entities; 
        }

        public AIEntity GetEntity(int id)
        {
            if (_entities.ContainsKey(id))
            {
                return _entities[id];
            }
            return null;
        }

        public int Size()
        {
            return _entities.Count;
        }

        public void RemoveEntity(int id)
        {
            if (_entities.ContainsKey(id))
                return;
            _entities.Remove(id);
        }

        public void ClearAll()
        {
            _entities.Clear();
        }

    }
}
