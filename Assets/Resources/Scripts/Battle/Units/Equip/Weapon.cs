using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Equip
{
    public class Weapon : Equip
    {
        protected WeaponAttributes weaponAttr;
        public bool IsReady { get; private set; } = false;
        public bool IsHitTriggered { get; private set; } = false;
        public bool IsHitDone { get; private set; } = false;
        public bool IsMiss { get; private set; } = false;
        private float _timer;
        
        public Weapon (EquipAttributes equipAttributes, WeaponAttributes weaponAttributes) : base(equipAttributes)
        {
            weaponAttr = weaponAttributes;
            prefabPath = "Prefabs/Units/Weapons/";
            switch (weaponAttr.category)
            {
                case WeaponAttributes.Category.one_hand_melee:
                    prefabPath += "Melee/OneHand/";
                    break;
            }
            
        }
        public void Swing()
        {
            if (!IsReady)
            {
                _timer += Time.deltaTime * 1; // magic
                if (_timer > weaponAttr.time)
                {
                    SetReady();
                }
            }
        }

        public Hit Hit(Unit target)
        {
            
            Debug.Log("HIT");
            Hit hit;
            switch (weaponAttr.type)
            {
                case WeaponAttributes.WeaponType.sword:
                    
                    hit = Random.Range(0,100) > 50 ? new Hit(equipAttr.chop, 0, equipAttr.crush, Units.Hit.AttackDirection.back) : new Hit(0, equipAttr.stabb, 0, Units.Hit.AttackDirection.back);
                    break;
                default:
                    hit = new Hit(equipAttr.chop, equipAttr.stabb, equipAttr.crush, Units.Hit.AttackDirection.back);
                    break;
            }
            Abort();
            return hit;
        }
        public void Attack()
        {
            if (IsReady)
            {
                _timer += Time.deltaTime * 1; // magic
                if (_timer > weaponAttr.time)
                {
                    HitDone();
                }
            }
        }
        public void TriggerHit()
        {
            if (IsReady)
            {
                IsHitTriggered = true;
                Debug.Log("WEAPON HIT TRIGGERED");
            }
        }
        public void HandleMiss()
        {
            if(_timer < 0)
            {
                _timer += Time.deltaTime * 1; // magic
            } else
            {
                _timer = 0;
                IsMiss = false;
                Debug.Log("MISS HANDLED");
            }
        }
        public void Miss()
        {
            IsReady = false;
            IsHitTriggered = false;
            IsHitDone = false;
            IsMiss = true;
            _timer = weaponAttr.time * -1;
            Debug.Log("MISS");
        }
        public float Distance => weaponAttr.distance;
        public bool IsMelee => weaponAttr.category == WeaponAttributes.Category.one_hand_melee || weaponAttr.category == WeaponAttributes.Category.two_hand_melee;
        public bool IsShieldInHand => weaponAttr.category == WeaponAttributes.Category.one_hand_melee || weaponAttr.category == WeaponAttributes.Category.one_hand_range;
        private void RestartTimer() => _timer = 0;
        private void SetReady()
        {
            IsReady = true;
            RestartTimer();
            Debug.Log("WEAPON READY");
        }
        private void HitDone()
        {
            IsHitDone = true;
            IsHitTriggered = false;
            Debug.Log("WEAPON HIT DONE");
        }
        private void Abort()
        {
            IsReady = false;
            IsHitTriggered = false;
            IsHitDone = false;
            IsMiss = false;
            RestartTimer();
        }
    }
    
}
