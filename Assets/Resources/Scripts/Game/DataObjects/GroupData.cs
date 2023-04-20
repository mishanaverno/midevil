using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.DataObjects
{
    public class GroupData
    {
        public int id;
        public string title;
        public List<UnitData> units;
        public List<int> ints;
        public UnitData sgt;
        public bool player;
        public Fraction fraction;
        // group equip
        public string armor = "";
        public string helm = "";
        public string shield = "";
        public string primary_weapon = "";
        public string banner = "";

        public GroupData()
        {
        }

        public GroupData(UnitData sgt, bool player, Fraction fraction)
        {
            id = Counters.Group;
            this.sgt = sgt;
            this.player = player;
            units = new List<UnitData>();
            title = "Group " + id;
            this.fraction = fraction;
        }
    }
}
