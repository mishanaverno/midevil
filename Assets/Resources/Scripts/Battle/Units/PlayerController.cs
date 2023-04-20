using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units
{
    public class PlayerController : UnitController
    {
        private bool righClickHolded = false;
        private float timeHolded = 0f;
        private float speed = 6f;
        private float angularSpeed = 180;
        private Unit _target;
        public override void Action()
        {
            /*if (BattleMono.CameraMono.RightClick)
            {
                if (timeHolded > 0.3f)
                {
                    CursorLookAt();
                }
                po.controller.SetTarget(BattleMono.Cursor.position, BattleMono.Cursor.rotation.eulerAngles);
                po.controller.StandTargetPosition();
                po.Group.ChangePosition(); 

                RefresRightClickHold();
            }
            if (righClickHolded)
            {
                //GameMono.Cursor.LookAt(GameMono.Camera.GetMousePosition());
                BattleMono.CursorMono.RotateArrow(GetRotationToCursor());
                timeHolded += Time.deltaTime;
            }
            if (BattleMono.CameraMono.RightClickDown)
            {
                righClickHolded = true;
                Vector3 position = BattleMono.CameraMono.GetMousePosition();
                BattleMono.Cursor.position = position;
            }*/
            if (BattleMono.BattleStarted)
            {
                float translate = Input.GetAxis("Vertical");

                if (translate != 0)
                {
                    po.go.transform.Translate(0, 0, Time.deltaTime * speed * translate);
                    po.animator.Walking();
                }
                else
                {
                    po.animator.Idle();
                }

                if (Input.GetKeyUp(KeyCode.Q))
                {
                    BattleMono.Cursor.SetPositionAndRotation(po.go.transform.position, po.go.transform.rotation);
                    po.controller.SetTarget(po.Transform.position, po.Transform.rotation.eulerAngles);
                    po.Group.ChangePosition();
                    BattleMono.CursorMono.Show();
                }
                if (Input.GetKeyUp(KeyCode.LeftAlt))
                {
                    BattleMono.CursorMono.Show();
                }
                // mouse buttons
                if (Input.GetMouseButtonDown(0))
                {
                    _target = po.GetClosestEnemy();
                    Debug.Log("SWING");
                    Debug.DrawLine(po.Position, _target.Position, Color.gray, 1, true);
                }

                if (Input.GetMouseButton(0) && _target.Group.data.id != po.Group.data.id)
                {
                    _target = po.GetClosestEnemy();
                    po.PrimaryWeapon.Swing();
                    Vector3 targetDirection = (_target.Position - po.Position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    if (Quaternion.Angle(po.Transform.rotation, targetRotation) > 0.01f)
                    {
                        po.Transform.rotation = Quaternion.RotateTowards(po.Transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
                    }
                    float rotatate = Input.GetAxis("Horizontal");
                    if (rotatate != 0) { po.go.transform.Translate(new Vector3(rotatate * Time.deltaTime * speed, 0, 0)); }

                }
                else
                {
                    if (po.PrimaryWeapon.IsMiss)
                    {
                        po.PrimaryWeapon.HandleMiss();
                    }
                    float rotatate = Input.GetAxis("Horizontal");
                    if (rotatate != 0) { po.go.transform.Rotate(0, Time.deltaTime * angularSpeed * rotatate, 0, Space.Self); }
                }
                if (po.PrimaryWeapon.IsReady)
                {
                    if (Input.GetMouseButtonUp(0) && _target.Group.data.id != po.Group.data.id)
                    {
                        po.PrimaryWeapon.TriggerHit();
                    }
                    if (po.PrimaryWeapon.IsHitTriggered)
                    {
                        po.PrimaryWeapon.Attack();
                    }
                    if (po.PrimaryWeapon.IsHitDone)
                    {
                        Vector3 targetDirection = (_target.Position - po.Position).normalized;
                        float angle = Vector3.Angle(targetDirection, po.Transform.forward);
                        Debug.Log("DIST: " + Vector3.Distance(po.Position, _target.Position) + " WDIST: " + po.PrimaryWeapon.Distance + " ANGLE: " + angle);
                        if (Vector3.Distance(po.Position, _target.Position) < po.PrimaryWeapon.Distance && angle < 23)
                        {
                            Debug.DrawLine(po.Position, _target.Position, Color.red, 3, true);
                            _target.ReceivHit(po, po.PrimaryWeapon.Hit(_target));
                        }
                        else
                        {

                            po.PrimaryWeapon.Miss();
                        }
                    }
                }

            }
        }
    }
}