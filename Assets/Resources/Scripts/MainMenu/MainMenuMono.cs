using Game.DataObjects;
using Game;
using MainMenu.Test;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Leguar.TotalJSON;
using Battle;
using Battle.Units.Equip;

namespace MainMenu
{
    public class MainMenuMono : MonoBehaviour
    {
        private static MainMenuMono _instance;
        private Transform _gui;

        //test
        private Transform _test;
        private Transform _addGroup;
        private string _presetsPath = "Assets/Resources/Data/Test/preset.json";
        private bool _addOffence;
        private Fraction player = new Fraction("OFFENCE SIDE", new Color32(0, 255, 0, 255));
        private Fraction ai = new Fraction("OFFENCE SIDE", new Color32(255, 0, 0, 255));

        public void ShowTestPanel()
        {
            _test.gameObject.SetActive(true);
        }
        public void HideTestPanel()
        {
            _test.gameObject.SetActive(false);
        }
        public void ShowAddGroupPanel(bool isOffence = false)
        {
            _addOffence = isOffence;
            _addGroup.gameObject.SetActive(true);
        }
        public void HideAddGroupPanel()
        {
            _addGroup.gameObject.SetActive(false);
        }
        public void AddGroup()
        {
            GroupData group = new(new UnitData("Sgt", UnitData.Sex.male, new Battle.Units.UnitAttributes(0,100)), BattleMaker.Instance.withPlayer, _addOffence ? player : ai)
            {
                //magic
                armor = "cloth-shirt-1",
                helm = "base",
                shield = "simple-1",
                primary_weapon = "sword-simple-1"
            };

            if (BattleMaker.Instance.withPlayer)
            {
                BattleMaker.Instance.withPlayer = false;
            }
            int unitCount = (int)_addGroup.Find("UnitsCount").GetComponent<Slider>().value;
            for (int i = 0; i < unitCount; i++)
            {
                group.units.Add(new UnitData("Unit", UnitData.Sex.male, new Battle.Units.UnitAttributes(0.5f, 100)));
            }
            if (_addOffence)
            {
                BattleMaker.Instance.offence.groupsData.Add(group);
            }
            else
            {
                BattleMaker.Instance.deffence.groupsData.Add(group);
            }
        }
        public void RefreshGroups()
        {
            GameObject card = Resources.Load<GameObject>("Prefabs/GUI/GroupCard");
            Transform ofg = _test.Find("OffenceGroups/ListContainer/List");
            Transform dfg = _test.Find("DeffenceGroups/ListContainer/List");
            Utils.DestroyChilds(ofg);
            Utils.DestroyChilds(dfg);
            
            //offense 
            BattleMaker.Instance.offence.groupsData.ForEach(delegate (GroupData group)
            {
                card.transform.Find("UnitsCount").GetComponent<TextMeshProUGUI>().SetText(group.units.Count + "/" + group.units.Count);
                Instantiate(card, ofg);
            });
            //deffence
            BattleMaker.Instance.deffence.groupsData.ForEach(delegate (GroupData group)
            {
                card.transform.Find("UnitsCount").GetComponent<TextMeshProUGUI>().SetText(group.units.Count + "/" + group.units.Count);
                Instantiate(card, dfg);
            });
        }
        public void StartBattle()
        {
            BattleMaker.Instance.Start();
        }
        public void SavePreset()
        {
            JSON json = JSON.Serialize(BattleMaker.Instance.GetPreset());
            string jsonString = json.CreateString();
            Debug.Log(json.CreatePrettyString());
            StreamWriter writer = new StreamWriter(_presetsPath, false);
            writer.Write(jsonString);
            writer.Close();
        }
        public void LoadPreset()
        {
            StreamReader reader = new StreamReader(_presetsPath, true);
            string jsonString = reader.ReadToEnd();
            JSON jsonObject = JSON.ParseString(jsonString);
            BattleMaker.Instance.SetPreset(jsonObject.Deserialize<Preset>());
            RefreshGroups();
        }
        // test
        void Awake()
        {
            _instance = this;
            _gui = transform.Find("GUI");
            _test = _gui.Find("Test");
            _addGroup = _gui.Find("AddGroup");
            
        }
        void Start()
        {
        }
    }
}
