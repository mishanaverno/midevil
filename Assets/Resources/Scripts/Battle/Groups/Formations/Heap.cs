using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Groups.Formstions
{
    public class Heap : Formation
    {
        public readonly int id = 0;
        protected float radius = 3.5f;
        protected float radius_treshhold = 1.5f;
        protected float row_base_size = 7;
        protected float row_modifier = 1.7f;
        public Heap()
        {

        }

        public override void DistributeUnitsToPosition(List<Unit> units, Unit sgt)
        {
            BattleMono.CursorMono.ClearPoints();
            int counter = 0;
            int rowCounter = 1;
            float rowSize = row_base_size;
            float rowCount = 1;
            if (sgtPosition != SgtPosition.center) {
                units.ForEach(delegate (Unit unit)
                {
                    if (counter >= rowSize)
                    {
                        rowSize *= row_modifier;
                        counter = 0;
                        rowCounter++;
                    }
                    counter++;
                });
                counter = -1;
                rowCounter = 1;
            }
            float sectorSize = 360 / rowSize;
            units.ForEach(delegate (Unit unit)
            {
                if(counter >= rowSize)
                {
                    rowSize *= row_modifier;
                    counter = 0;
                    rowCounter++;
                    sectorSize = 360 / rowSize;
                }
                
                float fromDist, toDist, fromAngle, toAngle;
                if (counter == -1)
                {
                    fromDist = 0;
                    toDist = radius_treshhold;
                    fromAngle = 0;
                    toAngle = 360;
                    counter++;
                } 
                else
                {
                    fromDist = radius * rowCounter - radius_treshhold;
                    toDist = radius * rowCounter + radius_treshhold;
                    fromAngle = counter * sectorSize;
                    toAngle = ++counter * sectorSize;
                }
                float distance = Random.Range(fromDist, toDist);
                float direction = Random.Range(fromAngle, toAngle) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(direction), 0, Mathf.Sin(direction)) * distance;
                if (sgtPosition == SgtPosition.front)
                {
                    offset = new Vector3(offset.x, offset.y, offset.z - (rowCount + 1) * radius);
                }
                else if(sgtPosition == SgtPosition.behind)
                {
                    offset = new Vector3(offset.x, offset.y, offset.z + (rowCount + 1) * radius);
                }
                Vector3 position = sgt.go.transform.TransformPoint(offset);
                Vector3 rotation = sgt.controller.TargetRotation;
                unit.controller.SetTarget(position, rotation, 1f);
                BattleMono.CursorMono.CreatePointInLocal(offset);
            });
        }
    }
}
