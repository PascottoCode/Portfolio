using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class Flags
    {
        private Dictionary<Flag, (int stacks, float magnitude)> _enumDictionary = new Dictionary<Flag, (int stacks, float magnitude)>();
        private Dictionary<string, (int stacks, float magnitude)> _stringDictionary = new Dictionary<string, (int stacks, float magnitude)>();

        public bool HasEntry(Flag flag)
        {
            return _enumDictionary.ContainsKey(flag);
        }
        public bool HasEntry(string flag)
        {
            return _stringDictionary.ContainsKey(flag);
        }
        public int GetEntryStacks(Flag flag)
        {
            return _enumDictionary.ContainsKey(flag) ? _enumDictionary[flag].stacks : 0;
        }
        public int GetEntryStacks(string flag)
        {
            return _stringDictionary.ContainsKey(flag) ? _stringDictionary[flag].stacks : 0;
        }

        public float GetEntryMagnitude(Flag flag)
        {
            return _enumDictionary.ContainsKey(flag) ? _enumDictionary[flag].magnitude : 0;
        }
        public float GetEntryMagnitude(string flag)
        {
            return _stringDictionary.ContainsKey(flag) ? _stringDictionary[flag].magnitude : 0;
        }

        public float GetEntryValue(Flag flag)
        {
            return _enumDictionary.ContainsKey(flag) ? _enumDictionary[flag].stacks * _enumDictionary[flag].magnitude : 0;
        }
        public float GetEntryValue(string flag)
        {
            return _stringDictionary.ContainsKey(flag) ? _stringDictionary[flag].stacks * _stringDictionary[flag].magnitude : 0;
        }

        public void AddEntry(Flag flag, float magnitude)
        {
            if (_enumDictionary.ContainsKey(flag))
            {
                _enumDictionary[flag] = (_enumDictionary[flag].stacks + 1, _enumDictionary[flag].magnitude);
                return;
            }
            
            _enumDictionary.Add(flag, (1, magnitude));
        }
        public void AddEntry(string flag, float magnitude)
        {
            if (_stringDictionary.ContainsKey(flag))
            {
                _stringDictionary[flag] = (_stringDictionary[flag].stacks + 1, _stringDictionary[flag].magnitude);
                return;
            }
            
            _stringDictionary.Add(flag, (1, magnitude));
        }

        public void RemoveEntry(Flag flag, float magnitude)
        {
            if (!_enumDictionary.ContainsKey(flag)) { return; }
            
            _enumDictionary[flag] = (_enumDictionary[flag].stacks - 1, _enumDictionary[flag].magnitude);
            if (_enumDictionary[flag].stacks <= 0)
            {
                _enumDictionary.Remove(flag);
            }
        }
        public void RemoveEntry(string flag, float magnitude)
        {
            if (!_stringDictionary.ContainsKey(flag)) { return; }
            
            _stringDictionary[flag] = (_stringDictionary[flag].stacks - 1, _stringDictionary[flag].magnitude);
            if (_stringDictionary[flag].stacks <= 0)
            {
                _stringDictionary.Remove(flag);
            }
        }
    }
}