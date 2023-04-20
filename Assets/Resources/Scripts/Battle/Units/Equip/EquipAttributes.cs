using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Equip
{
    public class EquipAttributes
    {
        //целостность
        public float integrity;
        //колющий
        public int stabb;
        //рубящий
        public int chop;
        //дробящий
        public int crush;

        public float weight;
        public string prefabName;
        public string title;

        public EquipAttributes() {

            integrity = 100;
        }
    }
}
