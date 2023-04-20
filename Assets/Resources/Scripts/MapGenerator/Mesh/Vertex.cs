using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator.Mesh
{
    public class Vertex : Linkable<Vertex>, IVertices
    { 
        public Vector3 coordinate;
        public bool isCenter = false;
        public Vertex(Vector3 coordinate, bool isCenter = false)
        {
            this.coordinate = coordinate;
            this.isCenter = isCenter;
        }
        public List<Vertex> GetLinkedFromList(List<Vertex> input)
        {
            List<Vertex> output = new();
            links.ForEach(delegate (Vertex linked)
            {
                if (input.Contains(linked))
                {
                    output.Add(linked);
                }
            });
            return output;
        }

        public List<Vertex> Vertices()
        {
            return new() { this };
        }

        public Vector2 Vector2 => new(coordinate.x, coordinate.z);
    }
 
}