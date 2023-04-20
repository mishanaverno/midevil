using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator.Mesh
{
    public class Edge : Linkable<Edge>, IVertices
    {
        public readonly Vertex[] vertices = new Vertex[2];

        public Edge(Vertex vertex1, Vertex vertex2)
        {
            vertices[0] = vertex1;
            vertices[1] = vertex2;
        }

        public Vertex GetAnother(Vertex vertex)
        {
            return vertex == vertices[0] ? vertices[1] : vertices[0];
        }

        public List<Vertex> Vertices()
        {
            return new() { vertices[0], vertices[1] };
        }
        public bool Contains(Vertex vertex)
        {
            return vertices[0] == vertex || vertices[1] == vertex;
        }
    }
}