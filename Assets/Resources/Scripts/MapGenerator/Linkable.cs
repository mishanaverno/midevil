using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class Linkable<T> where T : Linkable<T>
    {
        public readonly List<T> links = new();
        public static void Link(T v1, T v2)
        {
            v1.links.Add(v2);
            v2.links.Add(v1);
        }
        public static void Unlink(T v1,T v2)
        {
            v1.links.Remove(v2);
            v2.links.Remove(v1);
        }
    }
}