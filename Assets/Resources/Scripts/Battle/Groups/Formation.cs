using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Groups
{
    public abstract class Formation
    {
        protected int counter = 1;
        public enum SgtPosition
        {
            front, center, behind
        }
        public enum FormationDirection
        {
            unknown, forward, backward, left, right
        }
        public SgtPosition sgtPosition = SgtPosition.center;
        protected Vector2 sgtInMapPosition = Vector2.zero;
        protected FormationDirection currentDirection;
        public void SetSgtPosition(SgtPosition position)
        {
            sgtPosition = position;
            currentDirection = FormationDirection.unknown;
        }
        abstract public void DistributeUnitsToPosition(List<Unit> units, Unit sgt);
        virtual public void Clear()
        {
            counter = 1;
        }
        public static FormationDirection ConvertSgtDirectionToFormationDirection(float direction)
        {
            if(direction <= 45)
            {
                return FormationDirection.forward;
            }
            else if(direction <= 135)
            {
                return FormationDirection.right;
            }
            else if(direction <= 225)
            {
                return FormationDirection.backward;
            }
            else if(direction <= 315)
            {
                return FormationDirection.left;
            }
            else
            {
                return FormationDirection.forward;
            }
        }
    }
}

