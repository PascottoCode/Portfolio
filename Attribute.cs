using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace RPG.Characters
{
    public class Attribute
    {
        public Attribute()
        {
            FlatValue = 0;
            _multiplier = 1;
        }
        [OdinSerialize] public float FlatValue { get; set; }
        public float Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                if (_multiplier == 0)
                {
                    _multiplier = Mathf.Epsilon;
                }
            }
        }

        [OdinSerialize, ShowInInspector, ReadOnly] private float _multiplier = 1;
    }
}