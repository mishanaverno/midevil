using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using System.Collections.Generic;

namespace Game
{
    public static class Utils
    {
        public static void DestroyChilds(Transform parent)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
        public static List<T> Dedublicate<T>(List<T> input)
        {
            List<T> output = new();
            input.ForEach(delegate (T element)
            {
                if (!output.Contains(element))
                {
                    output.Add(element);
                }
            });
            return output;
        }

        public static T GetJsonData<T>(string path)
        {
            StreamReader reader = new(path, true);
            string jsonString = reader.ReadToEnd();
            JSON jsonObject = JSON.ParseString(jsonString);
            return jsonObject.Deserialize<T>();
        }

        private static readonly Dictionary<string, GameObject> _prefabs = new();

        public static GameObject LoadPrefab(string path)
        {
            if (_prefabs.TryGetValue(path, out GameObject prefab))
            {
                return prefab;
            }
            else
            {
                prefab = Resources.Load<GameObject>("Prefabs/" + path);
                _prefabs.Add(path, prefab);
                return prefab;
            }

        }

        private static readonly Dictionary<string, Material> _materials = new();

        public static Material LoadMaterial(string path)
        {
            if (_materials.TryGetValue(path, out Material material))
            {
                return material;
            }
            else
            {
                material = Resources.Load<Material>("Materials/" + path);
                _materials.Add(path, material);
                return material;
            }
        }

    }
}
