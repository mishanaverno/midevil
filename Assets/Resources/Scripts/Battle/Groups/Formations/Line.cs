using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;

namespace Battle.Units.Groups.Formstions
{
    public class Line : Formation
    {
        public readonly int id = 1;
        protected float step, baseStep;
        protected int lineLimit = 10;
        protected int lineCounter = 0;
        protected int unitsInLineCounter = 1;
        protected Unit[,] map;
        public int XSize => map.GetLength(1);
        public int YSize => map.GetLength(0);
        public float Step => step;
        public Line()
        {
            step = baseStep = 3f;
            
        }
        public override void DistributeUnitsToPosition(List<Unit> units, Unit sgt)
        {
            BattleMono.CursorMono.ClearPoints();
            FormationDirection newDirection = ConvertSgtDirectionToFormationDirection(BattleMono.Cursor.rotation.eulerAngles.y);
            
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                map = InitMap(units.Count + 1);
                AppendSgt(sgt);
                DistributeUnitsToPositionmap(units);
                
            }
            Vector2 backbonePosition = Vector2.zero;
            switch (sgtPosition)
            {
                case SgtPosition.center:
                    backbonePosition = sgtInMapPosition;
                    break;
                case SgtPosition.front:
                    backbonePosition = IsBackwardOrLeft ? new Vector2(map.GetLength(1) / 2, map.GetLength(0) - 1) : new Vector2(map.GetLength(1) / 2, 0);
                    break;
                case SgtPosition.behind:
                    backbonePosition = !IsBackwardOrLeft ? new Vector2(map.GetLength(1) / 2, map.GetLength(0) - 1) : new Vector2(map.GetLength(1) / 2, 0);
                    break;
            }
            Debug.Log(backbonePosition);
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] != null)
                    {
                        float posX = x - backbonePosition.x, posY = 0;
                       
                        switch (sgtPosition)
                        {
                            case SgtPosition.center:
                                posY = y - backbonePosition.y;
                                break;
                            case SgtPosition.front:
                                posY = y - backbonePosition.y + (baseStep/2) * (IsBackwardOrLeft ? -1 : 1);
                                break;
                            case SgtPosition.behind:
                                posY = y - backbonePosition.y + (baseStep/2) * (IsBackwardOrLeft ? 1 : -1);
                                break;
                        }
                        int xmod = IsBackwardOrLeft ? 1 : -1;
                        int zmod = IsBackwardOrLeft ? 1 : -1;

                        Vector3 offset = new Vector3(posX * step * xmod, 0, posY * step * zmod);
                        Vector3 position = sgt.Transform.TransformPoint(offset);
                        Vector3 rotation = sgt.Transform.rotation.eulerAngles;

                        // to do: optimization
                        BattleMono.CursorMono.CreatePointInLocal(offset);
                        map[y, x].controller.SetTarget(position, rotation);

                    }
                }
            }
        }
        protected Unit[,] InitMap(int unitsCount)
        {
            int x = lineLimit < unitsCount ? lineLimit : unitsCount;
            int y = Mathf.CeilToInt((float) unitsCount / x);
            return new Unit[y, x];
        }
       
        protected void AppendSgt(Unit sgt)
        {
            if (sgtPosition == SgtPosition.center)
            {
                sgtInMapPosition = new Vector2(map.GetLength(1) / 2, map.GetLength(0) / 2);
            }
        }
        protected void DistributeUnitsToPositionmap(List<Unit> units)
        {
            int mod = IsBackwardOrLeft ? 1000 : -1000;
            if (IsVerticalDirection)
            {
                units.Sort(delegate (Unit unit1, Unit unit2)
                {
                    return (int)((unit1.Position.z - unit2.Position.z) * mod);
                });
            }
            else
            {
                units.Sort(delegate (Unit unit1, Unit unit2)
                {
                    return (int)((unit1.Position.x - unit2.Position.x) * mod);
                });
            }
            
            Queue<Unit> unitsQueue = new Queue<Unit>(units);
            int iterator = IsBackwardOrLeft ? -1 : 1;
            for (int y = IsBackwardOrLeft ? map.GetLength(0)-1 : 0; IsBackwardOrLeft ? y >= 0 : y < map.GetLength(0); y+=iterator)
            {
                int freePlaces = map.GetLength(1);
                if (sgtPosition == SgtPosition.center && sgtInMapPosition.y == y)
                {
                    freePlaces--;
                }
                List<Unit> lineUnits = new List<Unit>();
                for (int i = 0; i < freePlaces; i++)
                {
                    Unit unit;
                    if (unitsQueue.TryDequeue(out unit))
                    {
                        lineUnits.Add(unit);
                    }
                }

                if (IsVerticalDirection)
                {
                    lineUnits.Sort(delegate (Unit unit1, Unit unit2)
                    {
                        return (int)(unit2.Position.x * 1000 - unit1.Position.x * 1000);
                    });
                } 
                else
                {
                    lineUnits.Sort(delegate (Unit unit1, Unit unit2)
                    {
                        return (int)(unit1.Position.z * 1000 - unit2.Position.z * 1000);
                    });
                   
                }
                Queue<Unit> lineUnitsQueue = new Queue<Unit>(lineUnits);
                int x = 0;
                int count = lineUnitsQueue.Count;
                if (sgtPosition == SgtPosition.center && sgtInMapPosition.y == y)
                {
                    count++;
                }
                if (count < map.GetLength(1))
                {
                   x = Mathf.CeilToInt((float)(map.GetLength(1) - lineUnitsQueue.Count) / 2);
                }
                while (lineUnitsQueue.Count > 0)
                {
                    if(sgtPosition == SgtPosition.center && sgtInMapPosition == new Vector2(x,y))
                    {
                        x++;
                    }
                    Unit u = lineUnitsQueue.Dequeue();
                    map[y, x] = u;
                    x++;
                }
                
            }
        }
        public void SetLimit(int limit) => this.lineLimit = limit;
        public override void Clear()
        {
            base.Clear();
            lineCounter = 0;
            unitsInLineCounter = 1;
        }
        protected bool IsVerticalDirection => currentDirection == FormationDirection.forward || currentDirection == FormationDirection.backward;
        protected bool IsBackwardOrLeft => currentDirection == FormationDirection.left || currentDirection == FormationDirection.backward;
    }
}
