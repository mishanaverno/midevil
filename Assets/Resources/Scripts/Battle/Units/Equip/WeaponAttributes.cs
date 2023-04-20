using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Equip
{
    public class WeaponAttributes
    {
        public enum Category { one_hand_melee, two_hand_melee, one_hand_range, two_hand_range };
        public enum WeaponType { sword };
        public float distance; 
        public float time = 3; //magic | sec

        public WeaponType type;
        public Category category;

        public WeaponAttributes() { }
    }
}
