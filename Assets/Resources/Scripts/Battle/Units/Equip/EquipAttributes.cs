using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Equip
{
    public class EquipAttributes
    {
        //�����������
        public float integrity;
        //�������
        public int stabb;
        //�������
        public int chop;
        //��������
        public int crush;

        public float weight;
        public string prefabName;
        public string title;

        public EquipAttributes() {

            integrity = 100;
        }
    }
}
