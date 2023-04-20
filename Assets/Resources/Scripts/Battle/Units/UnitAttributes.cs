using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public class UnitAttributes
    {
        public float reactionDelay;
        public float maxHelth;

        public UnitAttributes()
        {
        }

        public UnitAttributes(float reactionDelay, float maxHelth)
        {
            this.reactionDelay = reactionDelay;
            this.maxHelth = maxHelth;
        }
    }
}
