using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Counters
    {
        private static int unit = 0;
        private static int group = 0;
        private static int side = 0;

        public static int Unit => unit++;
        public static int Group => group++;
        public static int Side => side++;
        public static void Reset()
        {
            unit = 0;
            group = 0;
            side = 0;
        }
    }
}