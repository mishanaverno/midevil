using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator.Mesh;
using Game;

namespace MapGenerator
{
    public class Zone : WithForms<Zone>
    {
        public readonly ZoneData data;
        public List<Triangle> triangles = new();
        public List<Vertex> vertices = new();
        public List<Cell> cells = new();
        public List<ZonePart> parts = new();
        // const
        private static readonly int MAX_VERTICES_IN_MESH = 60000;
        public Zone(ZoneData data)
        {
            this.data = data;
        }
        public void To(Location location)
        {
            location.zones.Add(this);
        }
        public void BuildParts()
        {
            List<Cell> allCells = new(cells);
            Queue<Cell> checkQueue = new();

            checkQueue.Enqueue(allCells[0]);
            allCells.RemoveAt(0);
            int vertexCount = 0;
            ZonePart part = new(this);
            while (checkQueue.Count > 0 || allCells.Count > 0)
            {
                Cell cell = checkQueue.Count > 0 ? checkQueue.Dequeue() : allCells[0];
                cell.links.ForEach(delegate (Cell link)
                {
                    int linkIndex = allCells.FindIndex(delegate (Cell c)
                    {
                        return c == link;
                    });
                    if (linkIndex >= 0)
                    {
                        checkQueue.Enqueue(link);
                        allCells.RemoveAt(linkIndex);
                    }
                });
                cell.SetZonePart(part);
                vertexCount += cell.GetVertexCount();
                if (vertexCount > MAX_VERTICES_IN_MESH)
                {
                    parts.Add(part);
                    part = new(this);
                    vertexCount = 0;
                }
            }
            parts.Add(part);
        }
        public void BuildMesh()
        {
            // collect border vertices
            //List<Vertex> borders = new();
            //VertexGraph borderGraph = new();
            //foreach (Vertex vertex in vertices)
            //{
            //    if ((!vertex.isCenter && vertex.links.Count == 8) || (vertex.isCenter && vertex.links.Count == 4)) continue;
            //    borders.Add(vertex);
            //    borderGraph.vertices.Add(new(vertex));
            //    Debug.DrawRay(vertex.coordinate, Vector3.up, Color.black, 100, true);
            //};
            // done, start iterate border vertices
            //float medx = 0, medy = 0;
            //float minv = float.PositiveInfinity;
            //VertexNode start = null;
            //borderGraph.vertices.ForEach(delegate (VertexNode node)
            //{
            //    node.vertex.GetLinked(borders).ForEach(delegate (Vertex link)
            //    {
            //        VertexNode linkNode = borderGraph.vertices.Find(delegate (VertexNode n)
            //        {
            //            return link == n.vertex;
            //        });
            //        node.links.Add(linkNode);
            //    });
            //   
            //    if (node.vertex.coordinate.x + node.vertex.coordinate.z < minv)
            //    {
            //        minv = node.vertex.coordinate.x + node.vertex.coordinate.z;
            //        start = node;
            //    }
            //    medx += node.vertex.coordinate.x;
            //    medy += node.vertex.coordinate.z;
            //});
            //medx /= borders.Count;
            //medy /= borders.Count;
            //Vector2 center = new(medx, medy);
            //minv = 0;
            //VertexNode target = null;
            //Debug.Log(data.material);
            //start.links.ForEach(delegate (VertexNode link)
            //{
            //   Vector2 v2 = link.vertex.Vector2 - center;
            //   float v = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
            //    if (v < 0) v += 360;
            //    if (v > minv)
            //    {
            //        minv = v;
            //        target = link;
            //    }
            //});
            //Vertex prev = null;
            //borderGraph.GetLongestPath(start, target, borderGraph.vertices).ForEach(delegate(Vertex vertex)
            //{
            //    if (prev != null)
            //    {
            //        MakeBorder(prev, vertex);
            //    }
            //    prev = vertex;
            //});

            //
            //Debug.DrawRay(new(medx, 0, medy), Vector3.up * 10, Color.blue, 100, true);
            //Debug.DrawRay(start.vertex.coordinate, Vector3.up, Color.blue, 100, true);
            //Debug.DrawRay(target.vertex.coordinate, Vector3.up, Color.green, 100, true);
            //Debug.DrawLine(new(medx, 0, medy), start.vertex.coordinate, Color.blue, 100, true);
            //

            // collect indexes for triangles
            // convert vertexes to Vectors
            // calculate normals
            // to do UVs
            parts.ForEach(delegate (ZonePart part)
            {
                UnityEngine.Mesh mesh = new();
                List<Vector3> rawVertices = new();
                List<Vector3> normals = new();
                List<Vertex> _vertices = new();
                List<Triangle> _triangles = new();
                List<int> indexes = new();
                List<Vector2> uvs = new();
                List<Vector4> tangents = new();
                int index = 0;
                part.cells.ForEach(delegate (Cell cell)
                {
                    cell.Triangles().ForEach(delegate (Triangle triangle)
                    {
                        float magnitude;
                        //Vector3 normal = Vector3.Cross(triangle.vertices[1].coordinate - triangle.vertices[0].coordinate, triangle.vertices[2].coordinate - triangle.vertices[0].coordinate);
                        //magnitude = normal.magnitude;
                        //if (magnitude > 0) normal /= magnitude;

                        Vector3 thisTrangleNormal = Vector3.Cross(triangle.vertices[1].coordinate - triangle.vertices[0].coordinate, triangle.vertices[2].coordinate - triangle.vertices[0].coordinate);
                        int uvIndex = 0;
                        foreach (Vertex vertex in triangle.Vertices)
                        {
                            //int count = 1;
                            uvs.Add(triangle.uv[uvIndex++]);
                            indexes.Add(index++);
                            rawVertices.Add(vertex.coordinate);
                            Vector3 normal = thisTrangleNormal;
                            //triangle.GetSiblingsByVertex(vertex).ForEach(delegate (Triangle link)
                            //{
                            //    Vector3 linkTrangleNormal = Vector3.Cross(link.vertices[1].coordinate - link.vertices[0].coordinate, link.vertices[2].coordinate - link.vertices[0].coordinate);
                            //    normal += linkTrangleNormal;
                            //    count++;
                            //});
                            magnitude = normal.magnitude;
                            if (magnitude > 0) normal /= magnitude;
                            normals.Add(normal);
                            //Debug.DrawRay(vertex.coordinate, normal * count, Color.red, 100, true);
                            tangents.Add(new Vector4(0, 1, 0, 1));
                        }
                    });
                });
                mesh = new();
                mesh.SetVertices(rawVertices);
                mesh.SetTriangles(indexes, 0);
                mesh.SetNormals(normals);
                mesh.SetUVs(0, uvs);
                mesh.SetTangents(tangents);

                //mesh.RecalculateBounds();
                //mesh.RecalculateNormals();
                //mesh.RecalculateTangents();

                // set mesh and material
               
                part.gameObject.AddComponent<MeshFilter>().mesh = mesh;
                part.gameObject.AddComponent<MeshRenderer>().material = Utils.LoadMaterial("Terrain/" + data.material);
            });
        }
        private void MakeBorder(Vertex vertex1, Vertex vertex2)
        {
            // build triangles
            Vertex vertex1Clone = new(new(vertex1.coordinate.x, vertex1.coordinate.y - 1, vertex1.coordinate.z));
            Vertex vertex2Clone = new(new(vertex2.coordinate.x, vertex2.coordinate.y - 1, vertex2.coordinate.z));
            Triangle triangle1 = new(vertex1, vertex2, vertex1Clone);
            Triangle triangle2 = new(vertex2, vertex2Clone, vertex1Clone);
        }

        private class VertexGraph
        {
            public List<VertexNode> vertices = new();
            public List<Vertex> GetLongestPath(VertexNode current, VertexNode target, List<VertexNode> source)
            {
                int maxPathLength = 0;
                List<Vertex> path = new();
                if (current == target)
                {
                    path.Add(current.vertex);
                    return path;
                }
                List<VertexNode> sourceCopy = new();
                source.ForEach(delegate (VertexNode node)
                {
                    if (node != current) sourceCopy.Add(node);
                });
               
                foreach (VertexNode link in current.links)
                {
                    if (!sourceCopy.Contains(link)) continue;
                    List<Vertex> foundPath = GetLongestPath(link, target, sourceCopy);
                    if (foundPath.Count > maxPathLength)
                    {
                        maxPathLength = foundPath.Count;
                        path = foundPath;
                    }
                }
                sourceCopy.Clear();
                path.Add(current.vertex);
                return path;
            }
        }
        private class VertexNode
        {
            public static int counter = 1;
            public Vertex vertex;
            public List<VertexNode> links = new();
            public int id;

            public VertexNode(Vertex vertex)
            {
                this.vertex = vertex;
                id = counter++;
            }
        }
    }

    // Data object
    public class ZoneData
    {
        public string material;
        public Type type;
        public enum Type { plain = 0, road = 6, rock = 3, forest = 4, swamp = 5};

        public ZoneData() {}

        public ZoneData(string material, Type type)
        {
            this.material = material;
            this.type = type;
        }
    }

    public class ZonePart
    {
        public static int id = 1;
        public List<Cell> cells = new();
        public Zone zone;
        public GameObject gameObject = new();

        public ZonePart (Zone zone)
        {
            this.zone = zone;
            gameObject.name = zone.data.material + id++;
            gameObject.transform.parent = Location.Instance.map.transform;
            Unity.AI.Navigation.NavMeshModifier modifier = gameObject.AddComponent<Unity.AI.Navigation.NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = (int)zone.data.type;
        }
    }

}
