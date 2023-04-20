using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator.Mesh
{
    public class Triangle : Linkable<Triangle>, IVertices
    {
        public readonly Vertex[] vertices = new Vertex[3];
        public Cell cell;
        public readonly List<Edge> edges = new();
        public Vector2[] uv= new Vector2[3];
        public readonly List<Triangle> subTriangles = new();
        public Zone Zone { get; private set; }
        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;

            edges.Add(new(vertices[0], vertices[1]));
            edges.Add(new(vertices[1], vertices[2]));
            edges.Add(new(vertices[2], vertices[0]));
        }
        public void SetZone(Zone zone)
        {
            if (Zone == null)
            {
                Zone = zone;
            }
        }
        public List<Vertex> Vertices => new() { vertices[0], vertices[1], vertices[2] };
        public List<Triangle> GetSiblingsByVertexes(List<Triangle> triangles = null)
        {
            List<Triangle> result = new();
            if (triangles == null) triangles = links;
            triangles.ForEach(delegate (Triangle triangle)
            {
                triangle.Vertices.ForEach(delegate (Vertex vertex)
                {
                    if (Vertices.Contains(vertex) && triangle != this)
                    {
                        result.Add(triangle);
                        return;
                    }
                });
            });
            return result;
        }
        public List<Triangle> GetSiblingsByVertex(Vertex findVertex, List<Triangle> triangles = null)
        {
            List<Triangle> result = new();
            if (triangles == null) triangles = links;
            triangles.ForEach(delegate (Triangle triangle)
            {
                triangle.Vertices.ForEach(delegate (Vertex vertex)
                {
                    if (Vertices.Contains(vertex) && findVertex == vertex && triangle != this)
                    {
                        result.Add(triangle);
                        return;
                    }
                });
            });
            return result;
        }
        public List<Triangle> GetSiblingsByEdge(List<Triangle> triangles = null)
        {

            List<Triangle> result = new();
            if (triangles == null)
            {
                triangles = links;
            }
            else
            {
                triangles.Remove(this);
            }
            triangles.ForEach(delegate (Triangle triangle)
            {
                int match = 0;
                triangle.Vertices.ForEach(delegate (Vertex vertex)
                {
                    if (Vertices.Contains(vertex))
                    {
                        match++;
                    }
                    if (match == 2)
                    {
                        match = 0;
                        result.Add(triangle);
                        return;
                    }
                });
            });
            return result;
        }

        List<Vertex> IVertices.Vertices()
        {
            return Vertices;
        }
        public Vertex Subdivide()
        {
            Vertex O = vertices[0];
            Edge E = edges.Find(delegate (Edge edge) { return !edge.Contains(O); });
            Vertex N = new((E.vertices[0].coordinate + E.vertices[1].coordinate) / 2);
            Vector2 DN = (N.Vector2 - O.Vector2).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, DN);
            
            Vertex V1 = E.Vertices().Find(delegate (Vertex vertex)
            {
                Vector2 TV = TurnVertex(angle, (vertex.Vector2 - O.Vector2));
                if (TV.x > 0)
                {
                    return true;
                }
                Debug.Log("TV: " + TV);
                return false;
            });
            Vertex V2 = E.GetAnother(V1);
            Triangle T1 = new(N, V1, O);
            Triangle T2 = new(N, O, V2);

            subTriangles.Add(T1);
            subTriangles.Add(T2);
            return N;
        }
        // to do put in correct place if needed
        public static Vector2 TurnVertex(float angle, Vector2 vector)
        {
            float x = (vector.x * Mathf.Cos(angle * Mathf.Deg2Rad)) + (vector.y * Mathf.Sin(angle * Mathf.Deg2Rad));
            float y = (vector.y * Mathf.Cos(angle * Mathf.Deg2Rad)) - (vector.x * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 result = new Vector2(x, y);
            return result;
        }
        public List<Triangle> GetTriangleForRender()
        {
            List<Triangle> result = new();
            if(subTriangles.Count > 0)
            {
                subTriangles.ForEach(delegate (Triangle subTriangle)
                {
                    result.AddRange(subTriangle.GetTriangleForRender());
                });
            }
            else
            {
                result.Add(this);
            }
            return result;
        }
        
        public int GetVertexCount()
        {
            int subTrianglesVertexCoutn = 0;
            subTriangles.ForEach(delegate (Triangle subTriangle)
            {
                subTrianglesVertexCoutn += subTriangle.GetVertexCount();
            });
            return subTrianglesVertexCoutn + 3;
        }
    }
}