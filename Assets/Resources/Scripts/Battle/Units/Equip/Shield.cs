using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Battle.Units.Equip
{
    public class Shield : Equip
    {
        public bool IsTriggered { get; private set; } = false;
        public bool IsdReady { get; private set; } = false;
        public float ShieldTimer { get; private set; } = 0;
        public float ShieldUpTime { get; private set; }
        public Shield(EquipAttributes attributes) : base(attributes)
        {
            prefabPath = "Prefabs/Units/Armor/Shield";
            ShieldUpTime = 1.5f; // magic
        }
        public override GameObject GetPrefab()
        {
            return Utils.LoadPrefab(prefabPath + "/shield-" + equipAttr.prefabName);
        }
        public void TriggerShieldUp()
        {
            IsTriggered = true;
            Debug.Log("SHIELD TRIGGERED");
        }
        public void Abort()
        {
            IsdReady = false;
            IsTriggered = false;
            ShieldTimer = 0;
        }
        public void SheldUp()
        {
            ShieldTimer += Time.deltaTime * 1; // magic; to do: calculate from skill value
            if (ShieldTimer >= ShieldUpTime)
            {
                Debug.Log("SHIELD READY");
                IsdReady = true;
                IsTriggered = false;
            }
        }
        public void ReceiveHit(Hit hit)
        {
            Attr.integrity -= (hit.chop - Attr.chop);
        }
    }
}
