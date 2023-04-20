using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CameraMono : MonoBehaviour
    {
        private float _minZ = 9;
        private float _maxZ = 60;
        private float _speed = 500;
        private float _sensivity = 200;
        private Plane _plane = new Plane(Vector3.up, 0);
        private Camera _camera;
        private Transform _target;
        private Vector3 _targetOffset = Vector3.zero;
        // Start is called before the first frame update
        void Awake()
        {
            _camera = GetComponent<Camera>();
            _target = transform.parent.Find("Camera Target");
        }

        // Update is called once per frame
        void Update()
        {
            

            if (RightCLickHold)
            {
                float rotatate = Input.GetAxis("Mouse X");
                if (rotatate != 0) {
                    _camera.transform.RotateAround(_target.position, new Vector3(0, 1, 0), rotatate * Time.deltaTime * _speed);
                    UpdateOffset();
                }
            }
            if (ZoomIn || ZoomOut)
            {
                transform.Translate(GetZoomInput(), Space.Self);
                UpdateOffset();
            }
            if (true)
            {
                transform.position = Vector3.Lerp(transform.position, _target.position + _targetOffset, Time.deltaTime * _speed);
                transform.LookAt(_target);
            }
        }

        public Vector3 GetMousePosition()
        {
            float distance;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                return GetFixedPosition(new Vector3(point.x, 0, point.z));
            }
            return new Vector3(0, 0, 0);
        }
        public float GetTerrainHeight(float x, float z)
        {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Ground");
            Ray ray = new Ray(new Vector3(x, 100, z), Vector3.down);
            if (Physics.Raycast(ray, out hit, 400, mask))
            {
                return hit.point.y;
            }
            else
            {
                return 0;
            }
        }
        public Vector3 GetFixedPosition(Vector3 position)
        {
            return new Vector3(position.x, GetTerrainHeight(position.x, position.z), position.z);
        }

        private Vector3 GetZoomInput(){
            return new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * _sensivity * Time.deltaTime * transform.position.y);
        }
        public void SetTarget(Transform target)
        {
            _target = target;
            UpdateOffset();
        }
        private void UpdateOffset()
        {
            _targetOffset = transform.position - _target.position;
        }
        private bool HorizontalMoving => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        private bool Zooming => Input.GetAxis("Mouse ScrollWheel") != 0;
        private bool ZoomIn => Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.y > _minZ;
        private bool ZoomOut => Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y < _maxZ;
        public bool RightClick => Input.GetMouseButtonUp(1);
        public bool RightClickDown => Input.GetMouseButtonDown(1);
        public bool RightCLickHold => Input.GetMouseButton(1);
    }
}
