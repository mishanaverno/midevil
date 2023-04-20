using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public class UnitMono : MonoBehaviour
    {
        protected Unit po;
        private void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // controller action
            po.controller.Action();
            // ai action
            po.ai.Action();
        }
        public void Bind(Unit po)
        {
            this.po = po;
        }

    }
}
