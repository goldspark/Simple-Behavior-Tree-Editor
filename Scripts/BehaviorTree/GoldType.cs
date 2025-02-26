using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldTypes
{
    public class GoldEnum
    {
        public string Name;
        private readonly Dictionary<string, int> _map;
        public GoldEnum(List<string> names)
        {
            _map = new Dictionary<string, int>();
            int i = 0;
            foreach (string name in names)
            {
                _map[name] = i++;
            }
        }

        public bool Contains(string name)
        {
            return _map.ContainsKey(name);
        }

        public void AddKey(string name)
        {
            _map.Add(name, _map.Count);
        }

        public int this[string s] => _map.ContainsKey(s) ? _map[s] : throw new KeyNotFoundException($"Field '{s}' is not defined in enum '{Name}'.");

        public IEnumerable<string> GetFields() { return _map.Keys; }
    }
}
