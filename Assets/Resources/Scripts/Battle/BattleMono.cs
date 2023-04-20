using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battle.Units;
using Battle.Units.Groups.Formstions;
using Battle.Units.Equip;
using Game.DataObjects;
using MainMenu.Test;
using Game;

namespace Battle
{
    public class BattleMono : MonoBehaviour
    {
        private static BattleMono _instance;
        private CameraMono _cameramono;
        private Transform _cursor;
        private CursorMono _cursormono;
        private Transform _gui;
        private Transform _terrain;
        private Transform _units;
        public static BattleMono Instance => _instance;
        public static CameraMono CameraMono => _instance._cameramono;
        public static Transform Cursor => _instance._cursor;
        public static Transform GUI => _instance._gui;
        public static Transform Terrain => _instance._terrain;
        public static CursorMono CursorMono => _instance._cursormono;
        public static Transform Units => _instance._units;
        public static bool BattleStarted => _instance.battleStarted;
        public static Unit Player => _instance._player;
        public List<BattleSide> battleSides = new();
        public List<Group> playerGroups = new();
        public List<Group> AIGroups = new();
        public Group ActiveGroup;
        private Unit _player;
        public bool battleStarted { get; private set; } = false;
        void Awake()
        {
            Counters.Reset();
            _instance = this;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            _cameramono = transform.Find("Main Camera").GetComponent<CameraMono>();
            _cursor = transform.Find("Cursor");
            _cursormono = _cursor.GetComponent<CursorMono>();
            _terrain = transform.Find("Terrain");
            _units = transform.Find("Units");

            // test data
            Debug.Log("GAME START");
            // battle
            battleSides = BattleMaker.Instance.sides;
            battleSides.ForEach(delegate (BattleSide side) {
                side.groupsData.ForEach(delegate (GroupData groupData)
                {
                    Group gr = new(groupData, side);
                    gr.SetFormation(new Heap());
                    gr.Sgt.controller.SetTarget(BattleMono.Cursor.position, BattleMono.Cursor.eulerAngles);
                    gr.Sgt.controller.StandTargetPosition();
                    gr.ChangePosition();
                    if (groupData.player)
                    {
                        ActiveGroup = gr;
                        _player = gr.Sgt;
                        CameraMono.SetTarget(_player.go.transform);
                    }
                    side.groups.Add(gr);
                });
            });
        }
        public List<Group> GetAllEnemyGroups(Group group)
        {
            List<Group> enemyGroups = new();
            battleSides.FindAll(delegate (BattleSide side)
            {
                return side.id != group.side.id;
            }).ForEach(delegate (BattleSide side)
            {
                enemyGroups.AddRange(side.groups);
            });
            return enemyGroups;
        }
        public void StartBattle()
        {
            battleStarted = true;
        }
        public void CheckEnd()
        {
            
        }
        public void End()
        {

        }

    }
}