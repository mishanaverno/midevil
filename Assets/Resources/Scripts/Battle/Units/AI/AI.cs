using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public abstract class AI : BindableToUnit
    {
        public abstract void Action();
    }
}
