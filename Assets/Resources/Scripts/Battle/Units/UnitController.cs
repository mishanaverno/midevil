using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Battle.Units
{
    public class UnitController : BindableToUnit
    {
        //movement
        private float _rotateSpeed = 250f;
        private Vector3 _targetPosition;
        private Vector3 _targetRotation;
        private bool _needToChangePosition = false;
        private bool _isMoving = false;
        private bool _isNearTargetPosition = false;
        private float _reactionDelay;
        private float _presition = 0.5f;
        private bool _strictPosition = true;

        protected NavMeshAgent agent;
        protected Transform transform;

        public Vector3 TargetRotation => _targetRotation;
        public Vector3 TargetPosition => _targetPosition;

        public override void OnBind()
        {
            agent = po.go.GetComponent<NavMeshAgent>();
            transform = po.go.transform;
        }

        public void StandTargetPosition()
        {
            if (BattleMono.BattleStarted)
            {
                _needToChangePosition = true;
            }
            else
            {
                transform.position = _targetPosition;
                transform.rotation = Quaternion.Euler(_targetRotation);
            }
            
        }
        public void SetTarget(Vector3 position, Vector3 rotation, float precision)
        {
            SetTarget(position, rotation);
            _strictPosition = false;
            _presition = precision;
        }
        public void SetTarget(Vector3 position, Vector3 rotation)
        {
            Clear();
            _targetPosition = BattleMono.CameraMono.GetFixedPosition(position);
            _targetRotation = rotation;
        }

        public virtual void Action()
        {
            if (!po.IsKnoked)
            {
                //on need to change position
                if (_needToChangePosition)
                {
                    if (ReactionDelay() < 0)
                    {
                        MoveToTarget();
                    }
                }
                // on moving
                if (_isMoving)
                {
                    float dist = Vector3.Distance(transform.position, _targetPosition);
                    if ((!_strictPosition && dist < _presition) || dist < 0.5f)
                    {
                        //Debug.Log("ALMOST REACHED POSITION" + agent.isStopped + agent.updateRotation+ _isNearTargetPosition);
                        _isNearTargetPosition = true;
                        _isMoving = false;
                        agent.updateRotation = false;

                        if (!_strictPosition)
                        {
                            agent.isStopped = true;
                        }
                        po.animator.Idle();
                    }
                }
                // on near position
                if (_isNearTargetPosition)
                {
                    if (transform.rotation.eulerAngles != _targetRotation)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime * _rotateSpeed);
                    }
                    else
                    {
                        //Debug.Log("REACHED POSITION" + agent.isStopped + agent.updateRotation + _isNearTargetPosition);
                        _isNearTargetPosition = false;

                    }

                }
            }
        }

        private void MoveToTarget()
        {
            
            _needToChangePosition = false;
            agent.isStopped = false;
            agent.updateRotation = true;
            _isMoving = true;
            agent.destination = _targetPosition;
            po.animator.Walking();
            //Debug.Log("STARTED MOVING" + agent.isStopped + agent.updateRotation + _isNearTargetPosition);
        }
        public void Clear()
        {
            float distance = Vector3.Distance(transform.position, po.Group.Sgt.Position);
            _reactionDelay = po.data.attributes.reactionDelay + (distance / 5);
            _strictPosition = true;
            _isMoving = false;
            _isNearTargetPosition = false;
            _needToChangePosition = false;
        }
    
        private float ReactionDelay()
        {
            if (_needToChangePosition)
            {
                _reactionDelay -= Time.deltaTime;
            }
            return _reactionDelay;
        }
        public void Cancel()
        {
            Clear();
            agent.isStopped = false;
            agent.updateRotation = true;
            po.animator.Idle();
        }

        public void Attack(Unit target)
        {
            po.PrimaryWeapon.Hit(target);
        }
    }
}
