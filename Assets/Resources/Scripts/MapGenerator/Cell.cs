using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator.Mesh;

namespace MapGenerator
{
    public class Cell : Linkable<Cell>, IVertices
    {
        // constants
        public readonly static float size = 0.5f;
        // vertexes
        public readonly Vertex c, fl, fr, bl, br;
        // triangles
        public readonly Triangle l, r, f, b;
        //edges
        public readonly List<Edge> edges = new();
        public ZonePart zonePart { get; private set; }

        public readonly Vector2 position;
        public bool isWalkable = true;
        public Cell(Vertex c, Vertex fl, Vertex fr, Vertex bl, Vertex br)
        {
            position = c.Vector2;
            this.c = c;
            this.fl = fl;
            this.fr = fr;
            this.bl = bl;
            this.br = br;
            
            l = new(c, bl, fl);
            l.uv[0] = new(0.5f, 0.5f);
            l.uv[1] = new(0, 0);
            l.uv[2] = new(0, 1);
            f = new(c, fl, fr);
            f.uv[0] = new(0.5f, 0.5f);
            f.uv[1] = new(0, 1);
            f.uv[2] = new(1, 1);
            r = new(c, fr, br);
            r.uv[0] = new(0.5f, 0.5f);
            r.uv[1] = new(1, 1);
            r.uv[2] = new(1, 0);
            b = new(c, br, bl);
            b.uv[0] = new(0.5f, 0.5f);
            b.uv[1] = new(1, 0);
            b.uv[2] = new(0, 0);
        }
        public static Vertex Vertex(float x, float y, string position = "c")
        {
            Vertex v;
            switch (position)
            {
                case "fl":
                    v = new(new(x - size, 0, y + size));
                    break;
                case "fr":
                    v = new(new(x + size, 0, y + size));
                    break;
                case "bl":
                    v = new(new(x - size, 0, y - size));
                    break;
                case "br":
                    v = new(new(x + size, 0, y - size));
                    break;
                default:
                    v = new(new(x, 0, y));
                    break;
            }
            return v;
        }
        public void SetZonePart(ZonePart zonePart)
        {
            this.zonePart = zonePart;
            zonePart.cells.Add(this);
        }
        public List<Vertex> Vertices()
        {
            return new() {c, fl, fr, br, bl};
        }

        public List<Triangle> Triangles() => new List<Triangle> { l, f, r, b };
        public List<Triangle> TrianglesForRender()
        {
            List<Triangle> result = new();
            Triangles().ForEach(delegate (Triangle triangle)
            {
                result.AddRange(triangle.GetTriangleForRender());
            });
            return result;
        }
        public Triangle GetTriangle(Vector2 dir)
        {
            Triangle triangle = null;
            if (dir == Vector2.up)
            {
                triangle = f;
            } 
            else if (dir == Vector2.down)
            {
                triangle = b;
            } 
            else if (dir == Vector2.left)
            {
                triangle = l;
            } 
            else if (dir == Vector2.right)
            {
                triangle = r;
            }
            return triangle;
        }
        public int GetVertexCount()
        {
            return l.GetVertexCount() + r.GetVertexCount() + b.GetVertexCount() + f.GetVertexCount();
        }
     }
}
