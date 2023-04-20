using Game;
using Game.DataObjects;
using MainMenu.Test;
using System.Collections.Generic;

namespace Battle
{
    public class BattleMaker : Singletone<BattleMaker>
    {
        public bool withPlayer = true;
        public string location = "Traning";
        public BattleSide offence = new BattleSide(BattleSide.Side.offence);
        public BattleSide deffence = new BattleSide(BattleSide.Side.deffence);
        public List<BattleSide> sides = new();

        public void AddToOffenceSide(GroupData group)
        {
            offence.groupsData.Add(group);
        }
        public void AddToDeffenceSide(GroupData group)
        {
            deffence.groupsData.Add(group);
        }
        // add fractions
        public void Start()
        {
            sides.Add(offence);
            sides.Add(deffence);
            SceneManager.Instance.LoadBattleScene(location);
        }
        // TEst
        public Preset GetPreset()
        {
            Preset pr = new();
            pr.offenceSide = offence.groupsData;
            pr.deffenceSide = deffence.groupsData;
            return pr;
        }

        public void SetPreset(Preset pr)
        {
            offence.groupsData = pr.offenceSide;
            deffence.groupsData = pr.deffenceSide;
        }
    }
}
