using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Singletone<T> where T : new()
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}
