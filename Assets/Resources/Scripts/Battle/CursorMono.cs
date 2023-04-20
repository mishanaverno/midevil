using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CursorMono : MonoBehaviour
    {
        private Transform _arrow;
        private MeshRenderer _arrowMesh;
        private List<MeshRenderer> _pointsMesh = new();
        private GameObject _point;
        private Transform _points;
        private bool _hide = false;
        private float _hideSpeed = 700;
        private float _showTimer = 0, _baseShowTimer = 2;
        void Awake()
        {
            _arrow = transform.Find("arrow");
            _arrowMesh = _arrow.gameObject.GetComponent<MeshRenderer>();
            _point = transform.Find("point").gameObject;
            _points = transform.Find("points");
        }
        public void CreatePointInLocal(Vector3 offset)
        {

            GameObject point = Instantiate(_point, _points, false);
            _pointsMesh.Add(point.transform.GetComponent<MeshRenderer>());
            point.transform.localPosition = offset;
        }
        public void ClearPoints()
        {
            for (int i = 0; i < _points.childCount; i++)
            {
                Destroy(_points.GetChild(i).gameObject);
                _pointsMesh = new();
            }
        }
        public void RotateArrow(Quaternion quaternion)
        {
            _arrow.rotation = quaternion;
            _points.rotation = quaternion;
        }
        public void ResetArrow()
        {
            _arrow.localRotation = Quaternion.Euler(Vector3.forward);
            _points.localRotation = Quaternion.Euler(Vector3.forward);
        }

        public void Hide()
        {
            Debug.Log("HIDE0");
            _hide = true;
        }
        public void Show()
        {
            _hide = false;
            Color32 color = _arrowMesh.material.color;
            color.a = 255;
            _arrowMesh.material.color = color;
            _pointsMesh.ForEach(delegate (MeshRenderer mesh)
            {
                mesh.material.color = color;
            });
            _showTimer = _baseShowTimer;
        }
        public void Update()
        {
            if (_hide)
            {
                Color32 color = _arrowMesh.material.color;
                float speed = Time.deltaTime * _hideSpeed;
                if (((float)color.a - speed) <= 0)
                {
                    color.a = 0;
                    _hide = false;
                }
                else
                {
                    color.a -= (byte)(speed);
                }
                _arrowMesh.material.color = color;
                _pointsMesh.ForEach(delegate (MeshRenderer mesh)
                {
                    mesh.material.color = color;
                });
            }
            if (_showTimer > 0)
            {
                _showTimer -= Time.deltaTime;
                if(_showTimer <= 0)
                {
                    Hide();
                }
            }
        }
    }
}
