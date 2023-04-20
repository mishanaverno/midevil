using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Units.Groups.Formstions
{
    public class Wall : Line
    {
        public new readonly int id = 3;
        public Wall()
        {
            step = baseStep = 2f;
        }
    }
}