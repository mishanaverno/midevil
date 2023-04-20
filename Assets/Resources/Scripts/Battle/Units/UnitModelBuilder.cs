using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Battle.Units {
    public class UnitModelBuilder : MonoBehaviour
    {
        private Transform _armature;
        private Color _color;

        public void Build(GameObject head, GameObject rhand, GameObject lhand, GameObject sword, GameObject shield, Color color)
        {
            _armature = transform.GetChild(0);
            _color = color;
            Color(transform.GetChild(1).gameObject);
            Head(head);
            Hands(lhand, rhand);
            Sword(sword);
            Shield(shield);
        }
        public void Build(GameObject head, GameObject rhand,GameObject lhand, GameObject sword, GameObject shield, GameObject banner, Color color)
        {
            Build(head, lhand, rhand, sword, shield, color);
            Banner(banner);
        }
        private void Sword(GameObject prefab)
        {
            Color(Instantiate(prefab, _armature.Find("Hips/Spine/Chest/UpperChest/Shoulder.R/Arm.R/ForeArm.R/Hand.R"), false));
        }
        private void Shield(GameObject prefab)
        {
            Color(Instantiate(prefab, _armature.Find("Hips/Spine/Chest/UpperChest/Shoulder.L/Arm.L/ForeArm.L/Hand.L"), false));
        }
        private void Banner(GameObject prefab)
        {
            Color(Instantiate(prefab, transform, false));
        }
        private void Head(GameObject prefab)
        {
            Color(Instantiate(prefab, _armature.Find("Hips/Spine/Chest/UpperChest/Neck/Head"), false));
        }
        private void Hands(GameObject lprefab, GameObject rprefab)
        {
            Color(Instantiate(lprefab, _armature.Find("Hips/Spine/Chest/UpperChest/Shoulder.L/Arm.L/ForeArm.L/Hand.L"), false));
            Color(Instantiate(rprefab, _armature.Find("Hips/Spine/Chest/UpperChest/Shoulder.R/Arm.R/ForeArm.R/Hand.R"), false));
        }
        private void Color(GameObject model)
        {
            MeshRenderer mesh;
            Material[] materials;
            if(model.TryGetComponent<MeshRenderer>(out mesh))
            {
                materials = mesh.materials;
            }
            else
            {
                materials = model.GetComponent<SkinnedMeshRenderer>().materials;
            }
            
            for (int i = 0; i < materials.Length; i++)
            {

                if (materials[i].name == "FractionColor (Instance)")
                {
                    materials[i].SetColor("_Color", _color);
                }
            }
        }
    }
}
