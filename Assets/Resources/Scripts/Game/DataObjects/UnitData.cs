using Battle.Units;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.DataObjects
{
    public class UnitData
    {
        public int id;
        public string name;
        // attributes
        public readonly UnitAttributes attributes;

        // equip
        public string armor = "";
        public string helm = "";
        public string shield = "";
        public string primary_weapon = "";


        public enum Sex
        {
            male, female
        }
        public readonly Sex sex;
        public UnitData()
        {

        }
        public UnitData(
            string name,
            Sex sex,
            UnitAttributes attr
        )
        {
            id = Counters.Unit;
            this.name = name;
            this.sex = sex;
            attributes = attr;
        }
    }
}
