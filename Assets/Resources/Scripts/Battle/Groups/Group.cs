using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle.Units.Groups;
using Game.DataObjects;
using Battle.Units.Equip;
using Battle.Units.Groups.Formstions;
using Game;

namespace Battle.Units
{
    public class Group
    {
        private Formation _formation = new Heap();
        private Unit _sgt;
        private List<Unit> _units = new List<Unit>();
        public readonly GroupData data;
        public Fraction Fraction { get; private set; }
        public readonly BattleSide side;
        public Group(GroupData data, BattleSide side)
        {
            Debug.Log("group created id:" + data.id);
            this.side = side;
            this.data = data;
            Fraction = data.fraction;
            this._sgt = new Unit(
                this,
                data.player ? new PlayerAI() : new SgtAI(),
                data.sgt,
                true
            );
            data.units.ForEach(delegate (UnitData data)
            {
                this._units.Add(new Unit(
                    this,
                    new UnitAI(),
                    data
                ));
            });
        }
        
        public void SetFormation(Formation formation)
        {
            Formation.SgtPosition pos = _formation.sgtPosition;
            formation.sgtPosition = pos;
            _formation = formation;
        }

        public void ChangePosition()
        {
            _formation.DistributeUnitsToPosition(GetUnits, _sgt);
            _units.ForEach(delegate (Unit unit)
            {
                unit.controller.StandTargetPosition();
            });
        }
        public void SetSgtInFront()
        {
            _formation.SetSgtPosition(Formation.SgtPosition.front);
        }
        public void SetSgtInCenter()
        {
            _formation.SetSgtPosition(Formation.SgtPosition.center);
        }
        public void SetSgtInBehind()
        {
            _formation.SetSgtPosition(Formation.SgtPosition.behind);
        }
        public List<Unit> GetUnits => new(_units);
        public List<Unit> Units => _units;
        public int GetUnitCount => _units.Count;
        public Unit Sgt => _sgt;
        public Formation Formation => _formation;
    }
}
