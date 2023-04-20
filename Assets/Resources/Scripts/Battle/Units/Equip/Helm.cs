using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Battle.Units.Equip
{
    public class Helm : Equip
    {
        public Helm(EquipAttributes attributes) : base(attributes)
        {
            prefabPath = "Prefabs/Units/Armor/Head";
        }
        public override GameObject GetPrefab()
        {
            return Utils.LoadPrefab(prefabPath + "/head-" + equipAttr.prefabName);
        }
    }
}
