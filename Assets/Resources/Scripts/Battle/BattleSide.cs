using Game.DataObjects;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle.Units;

namespace Battle
{
    public class BattleSide
    {
        public readonly int id;
        public List<GroupData> groupsData = new();
        public List<Group> groups = new();
        public enum Side {unknown, offence, deffence};
        public Side side;
        public BattleSide(Side side)
        {
            this.side = side;
            id = Counters.Side;
        }
    }
}
