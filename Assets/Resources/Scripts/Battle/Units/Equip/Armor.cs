using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Equip
{
    public class Armor : Equip
    {
        private string armorName;
        private string glovesName;
        public Armor(EquipAttributes attributes) : base(attributes)
        {
            prefabPath = "Prefabs/Units/Armor/";
            string[] splited = equipAttr.prefabName.Split("|", 2);
            armorName = splited[0];
            glovesName = splited.Length == 2 ? splited[1] : "base";
        }
        public GameObject GetRHand()
        {
            return Utils.LoadPrefab(prefabPath + "Hands/r-hand-" + glovesName);
        }
        public GameObject GetLHand()
        {
            return Utils.LoadPrefab(prefabPath + "Hands/l-hand-" + glovesName);
        }
        public override GameObject GetPrefab()
        {
            return Utils.LoadPrefab(prefabPath + "Body/body-" + armorName);
        }
    }
}
