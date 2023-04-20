using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator.Mesh;
using Game;

namespace MapGenerator
{
    public abstract class Terrain : WithForms<Terrain>
    {
        public abstract void Process(List<Vertex> input);
        public void To(Location location)
        {
            location.terrains.Enqueue(this);
        }
    }
    public class Lift : Terrain
    {
        public readonly float value;

        public Lift(float value)
        {
            this.value = value;
        }

        public override void Process(List<Vertex> input)
        {
            Get(input).ForEach(delegate(Vertex vertex) { 
                vertex.coordinate.y = value;
            });
        }
    }
    public class Flat : Terrain
    {
        public enum Operator { min, max, medium };
        public Operator op;

        public Flat(Operator op = Operator.min)
        {
            this.op = op;
        }

        public override void Process(List<Vertex> input)
        {
            List<Vertex> vertices = Get(input);
            float comparer = 0, value = 0;

            switch (op)
            {
                case Operator.min:
                    comparer = float.PositiveInfinity;
                    vertices.ForEach(delegate (Vertex vertex)
                    {
                        if (vertex.coordinate.y < comparer) comparer = vertex.coordinate.y;
                    });
                    value = comparer;
                    break;
                case Operator.max:
                    comparer = float.NegativeInfinity;
                    vertices.ForEach(delegate (Vertex vertex)
                    {
                        if (vertex.coordinate.y > comparer) comparer = vertex.coordinate.y;
                    });
                    value = comparer;
                    break;
                case Operator.medium:
                    vertices.ForEach(delegate (Vertex vertex)
                    {
                        comparer += vertex.coordinate.y;
                    });
                    value = comparer / vertices.Count;
                    break;
            }
            vertices.ForEach(delegate (Vertex vertex) { vertex.coordinate.y = value; });
        }
    }
    public class Sharp : Terrain
    {
        public readonly float value;

        public Sharp(float value = 1)
        {
            this.value = value;
        }

        public override void Process(List<Vertex> input)
        {
            Get(input).ForEach(delegate (Vertex vertex)
            {
                vertex.coordinate.y = Mathf.PerlinNoise(vertex.coordinate.x, vertex.coordinate.z) * value;
            });
        }
    }
}