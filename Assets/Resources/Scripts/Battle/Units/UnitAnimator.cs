using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public class UnitAnimator : BindableToUnit
    {
        public void Walking()
        {
            po.go.transform.GetComponent<Animator>().SetBool("walking", true);
        }
        public void Idle()
        {
            po.go.transform.GetComponent<Animator>().SetBool("walking", false);
        }
    }
}