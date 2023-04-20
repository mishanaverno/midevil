using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public class Hit
    {
        public Hit(int chop, int stabb, int crush, AttackDirection direction)
        {
            this.chop = chop;
            this.stabb = stabb;
            this.crush = crush;
            this.direction = direction;
        }

        public enum AttackDirection { front, back, left, right, top }
        public int chop { get; private set; }
        public int stabb { get; private set; }
        public int crush { get; private set; }
        public AttackDirection direction { get; private set; }

    }
}