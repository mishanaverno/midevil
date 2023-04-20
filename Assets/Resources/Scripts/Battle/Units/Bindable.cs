using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle.Units;

namespace Battle
{
    abstract public class BindableToUnit
    {
        protected Unit po;

        public void Bind(Unit unit)
        {
            po = unit;
            OnBind();
        }
        public virtual void OnBind()
        {

        }
    }
}
