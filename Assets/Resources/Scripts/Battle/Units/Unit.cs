using Battle.Units.Equip;
using Game.DataObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Battle.Units
{
    public class Unit
    {
        protected Group group;
        public readonly UnitData data;
        public readonly UnitController controller;
        public readonly UnitAnimator animator;
        public readonly GameObject go;
        public readonly AI ai;
        protected Armor armor;
        protected Helm helm;
        protected Weapon primaryWeapon;
        protected Shield shield;
        public float Helth { get; private set; }
        public bool IsDead => Helth <= 0;
        public bool IsKnoked { get; private set; }
        public Unit(
            Group group,
            AI ai,
            UnitData data,
            bool isSgt = false
        )
        {
            IsKnoked = false;
            Helth = data.attributes.maxHelth;
            this.group = group;
            // animator
            this.animator = new UnitAnimator();
            this.animator.Bind(this);
            //ai
            this.ai = ai;
            this.ai.Bind(this);
            //data
            this.data = data;
            //equip
            this.armor = EquipDB.Instance.ReadArmor(getArmorName());
            this.helm = EquipDB.Instance.ReadHelm(getHelmName());
            this.shield = EquipDB.Instance.ReadShield(getShieldName());
            this.primaryWeapon = EquipDB.Instance.ReadWeapon(getPrimaryWeaponName());
            
            //gameobject
            GameObject banner = Utils.LoadPrefab("Prefabs/Units/banner"); //magic
            this.go = Object.Instantiate(armor.GetPrefab(), BattleMono.Units);
            go.transform.name = "Unit:" + data.id;
            go.AddComponent<UnitMono>().Bind(this);
            if (isSgt)
            {
                this.go.GetComponent<UnitModelBuilder>().Build(helm.GetPrefab(), armor.GetRHand(), armor.GetLHand(), primaryWeapon.GetPrefab(), shield.GetPrefab(), banner, group.Fraction.color);
            }
            else
            {
                this.go.GetComponent<UnitModelBuilder>().Build(helm.GetPrefab(), armor.GetRHand(), armor.GetLHand(), primaryWeapon.GetPrefab(), shield.GetPrefab(), group.Fraction.color);
            }
            // controller
            controller = group.data.player ? new PlayerController() : new UnitController();
            controller.Bind(this);
        }
        public Unit GetClosestEnemy()
        {
            Group found = GetClosestEnemyGroup();
            Unit result = found.Sgt;
            float resultDistance = float.PositiveInfinity;
            found.GetUnits.ForEach(delegate (Unit unit)
            {
                if (unit.IsKnoked) return;
                float distance = Vector3.Distance(unit.Position, Position);
                if (distance < resultDistance)
                {
                    resultDistance = distance;
                    result = unit;
                }
            });
            result.OnEnemyFocus(this);
            return result;
        }
        public Group GetClosestEnemyGroup()
        {
            Group result = group;
            float resultDistance = float.PositiveInfinity;
            BattleMono.Instance.GetAllEnemyGroups(group).ForEach(delegate (Group gr)
            {
                if (gr.Sgt.IsKnoked) return;
                float distance = Vector3.Distance(gr.Sgt.Position, group.Sgt.Position);
                if (distance < resultDistance)
                {
                    //Debug.Log("GOTCHA");
                    resultDistance = distance;
                    result = gr;
                }
            });
            /*Debug.Log("Groups: " + BattleMono.Instance.GetAllEnemyGroups(group).Count);
            Debug.Log("My group: " + group.id);
            Debug.Log("Enemy group: " + result.id);*/
            return result;
        }
        public void ReceivHit(Unit unit, Hit hit)
        {
            if (IsKnoked) return;
            Debug.Log(data.name + " said: HIT Received From: " + unit.data.name);
            float critRand = Random.Range(0, 100);
            bool critical = critRand >= 70f ? true : false; //magic critical = true - hit in head
            
            float damage = 0;
            Equip.Equip part = critical ? helm : armor;
            damage += (hit.stabb - part.Attr.stabb) * 2;
            damage += (hit.crush - (part.Attr.crush / 2));
            damage += (hit.chop - part.Attr.chop);
            part.Attr.integrity -= hit.chop / 2;
            Helth -= critical ? damage * 2 : damage;
            Debug.Log(data.name + " said: I recived (" + (critical ? "critical " : "") + critRand+ ") " + damage + " I have " + Helth + " HP left ");
            if ( Helth < data.attributes.maxHelth / 2)
            {
                Debug.Log(data.name + " said: I'am in knokdown");
                IsKnoked = true;
                Object.Destroy(go);
            }
            if (IsDead)
            {
                Debug.Log(data.name + " said: I'am dead");
                Object.Destroy(go);
                if (Id == group.Sgt.Id)
                {
                    BattleMono.Instance.CheckEnd();
                }
            }
        } 
        public void OnEnemyFocus(Unit unit)
        {
            if (IsKeepSHieldInHand)
            {
                //shield
                if (unit.primaryWeapon.IsMelee && Vector3.Distance(Position, unit.Position) < unit.PrimaryWeapon.Distance * 2)
                {

                }
            }
        }
        public Vector3 Position => go.transform.position;
        public Vector3 Direction => go.transform.rotation.eulerAngles;
        public Transform Transform => go.transform;
        public Group Group => group;
        public int Id => data.id;
        public Weapon PrimaryWeapon => primaryWeapon;

        protected float X => go.transform.position.x;
        protected float Z => go.transform.position.z;
        private string getArmorName() => data.armor != "" ? data.armor : group.data.armor;
        private string getHelmName() => data.helm != "" ? data.helm : group.data.helm;
        private string getShieldName() => data.shield != "" ? data.shield : group.data.shield;
        private string getPrimaryWeaponName() => data.primary_weapon != "" ? data.primary_weapon : group.data.primary_weapon;
        public bool HasShield => shield != null;
        public bool IsKeepSHieldInHand => HasShield && primaryWeapon.IsShieldInHand;
    }
}
