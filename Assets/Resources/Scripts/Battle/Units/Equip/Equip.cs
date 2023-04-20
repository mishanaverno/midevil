using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Battle.Units.Equip
{
    public class Equip : BindableToUnit
    {
        protected EquipAttributes equipAttr;
        protected string prefabPath;
        public Equip(EquipAttributes attributes)
        {
            this.equipAttr = attributes;
        }
        public virtual GameObject GetPrefab()
        {
            return Utils.LoadPrefab(prefabPath + equipAttr.prefabName);
        }
        public EquipAttributes Attr => equipAttr;
    }
}
