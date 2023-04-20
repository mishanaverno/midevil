using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battle.Units.Groups;
using Battle.Units.Groups.Formstions;

namespace Battle
{
    public class GUIMono : MonoBehaviour
    {
        private Transform prepareBattlePanel;
        void Awake()
        {
            prepareBattlePanel = transform.Find("PrepareBattlePanel");
        }
        
        public void StartBattle()
        {
            BattleMono.Instance.StartBattle();
            prepareBattlePanel.gameObject.SetActive(false);
        }
        
        public void InFrontClick()
        {
            BattleMono.Instance.ActiveGroup.SetSgtInFront();
        }
        public void InCenterClick()
        {
            BattleMono.Instance.ActiveGroup.SetSgtInCenter();
        }
        public void InBehindClick()
        {
            BattleMono.Instance.ActiveGroup.SetSgtInBehind();
        }
        public void HeapClick()
        {
            BattleMono.Instance.ActiveGroup.SetFormation(new Heap());
        }
        public void LineClick()
        {
            BattleMono.Instance.ActiveGroup.SetFormation(new Line());
        }
        public void WallClick()
        {
            BattleMono.Instance.ActiveGroup.SetFormation(new Wall());
        }
    }
}
