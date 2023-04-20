using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator.Mesh;
using Game;

namespace MapGenerator
{
    public class Location : Singletone<Location>
    {
        public Vector2 size;
        public  Cell[,] cells;
        public Transform map;
        public readonly List<GameObject> gameObjects = new();
        public readonly Queue<Terrain> terrains = new();
        public readonly List<Zone> zones = new();
        public readonly List<Material> materials = new();

        private List<Triangle> _triangles = new();
        private List<Vertex> _vertices = new();
        private List<Edge> _edges = new();
        private List<Cell> _cells = new();

        public delegate void ForEachCell(Cell cell);
        public void Init(Vector2 size, Transform root)
        {
            map = root;
            this.size = size;
            cells = new Cell[(int)size.y, (int)size.x];
            CreateCells();
            PathFinder.InitGraph(cells);
            
        }

        public void Generate()
        {
            ApplyTerrains();
            BuildZones();
            BuildZoneParts();
            BuildBorders();
            RenderZones();
        }
        private void CreateCells()
        {
            for (int y = 0; y < size.y; y++)
            {
                
                for (int x = 0; x < size.x; x++)
                {
                    // vertexes
                    Vertex c, fr, fl, bl, br;
                    // c
                    c = Cell.Vertex(x, y);
                    c.isCenter = true;
                    _vertices.Add(c);

                    // fl
                    if (x == 0)
                    {
                        fl = Cell.Vertex(x, y, "fl");
                        _vertices.Add(fl);
                    } else
                    {
                        fl = cells[y, x - 1].fr;
                    }
                    // fr
                    fr = Cell.Vertex(x, y, "fr");
                    _vertices.Add(fr);
                    // bl
                    if (x == 0 && y == 0)
                    {
                        bl = Cell.Vertex(x, y, "bl");
                        _vertices.Add(bl);
                    }
                    else if (y == 0)
                    {
                        bl = cells[y, x - 1].br;
                    }
                    else if (x == 0)
                    {

                        bl = cells[y - 1, x].fl;
                    }
                    else
                    {
                        bl = cells[y - 1, x - 1].fr;
                    }
                    // br
                    if (y == 0)
                    {
                        br = Cell.Vertex(x, y, "br");
                        _vertices.Add(br);
                    } else
                    {
                        br = cells[y - 1, x].fr;
                    }
                    //vertex links
                    c.links.AddRange(new List<Vertex>() { fl, fr, bl, br });
                    fr.links.AddRange(new List<Vertex>() { c, fl, br });
                    fl.links.AddRange(new List<Vertex>() { c, fr });
                    br.links.AddRange(new List<Vertex> { c, fr });
                    bl.links.AddRange(new List<Vertex> { c });
                    if (x == 0)
                    {
                        bl.links.Add(fl);
                        fl.links.Add(bl);
                    }
                    if (y == 0)
                    {
                        bl.links.Add(br);
                        br.links.Add(bl);
                    }
                    // instatiate cell
                    cells[y, x] = new Cell(c, fl, fr, bl, br);
                    _cells.Add(cells[y, x]);
                    // cell links
                    if (x > 0)
                    {
                        Cell.Link(cells[y, x], cells[y, x - 1]);
                    }
                    if (y > 0)
                    {
                        Cell.Link(cells[y, x], cells[y - 1, x]);
                    }
                    // triangle links
                    cells[y, x].l.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].f, cells[y, x].b });
                    cells[y, x].r.links.AddRange(new List<Triangle> { cells[y, x].l, cells[y, x].f, cells[y, x].b });
                    cells[y, x].f.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].l, cells[y, x].b });
                    cells[y, x].b.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].f, cells[y, x].l });
                    if (x > 0)
                    {
                        //own
                        cells[y, x].f.links.AddRange(new List<Triangle> { cells[y, x - 1].f, cells[y, x - 1].r });
                        cells[y, x].l.links.AddRange(new List<Triangle> { cells[y, x - 1].f, cells[y, x - 1].r, cells[y, x - 1].b });
                        cells[y, x].b.links.AddRange(new List<Triangle> { cells[y, x - 1].r, cells[y, x - 1].b });
                        //sibling 
                        cells[y, x - 1].f.links.AddRange(new List<Triangle> { cells[y, x].f, cells[y, x].l });
                        cells[y, x - 1].r.links.AddRange(new List<Triangle> { cells[y, x].f, cells[y, x].l, cells[y, x].b });
                        cells[y, x - 1].b.links.AddRange(new List<Triangle> { cells[y, x].b, cells[y, x].l });
                    }
                    if (y > 0)
                    {
                        //own
                        cells[y, x].r.links.AddRange(new List<Triangle> { cells[y - 1, x].f, cells[y - 1, x].r });
                        cells[y, x].b.links.AddRange(new List<Triangle> { cells[y - 1, x].f, cells[y - 1, x].r, cells[y - 1, x].l });
                        cells[y, x].l.links.AddRange(new List<Triangle> { cells[y - 1, x].l, cells[y - 1, x].f });
                        //sibling 
                        cells[y - 1, x].r.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].b });
                        cells[y - 1, x].f.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].l, cells[y, x].b });
                        cells[y - 1, x].l.links.AddRange(new List<Triangle> { cells[y, x].b, cells[y, x].l });
                        if (x + 1 < cells.GetLength(1))
                        {
                            //own
                            cells[y, x].r.links.AddRange(new List<Triangle> { cells[y - 1, x + 1].f, cells[y - 1, x + 1].l });
                            cells[y, x].b.links.AddRange(new List<Triangle> { cells[y - 1, x + 1].f, cells[y - 1, x + 1].l });
                            //siblings
                            cells[y - 1, x + 1].l.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].b });
                            cells[y - 1, x + 1].f.links.AddRange(new List<Triangle> { cells[y, x].r, cells[y, x].b });
                        }
                    }
                    if (x > 0 && y > 0)
                    {
                        //own
                        cells[y, x].b.links.AddRange(new List<Triangle> { cells[y - 1, x - 1].f, cells[y - 1, x - 1].r });
                        cells[y, x].l.links.AddRange(new List<Triangle> { cells[y - 1, x - 1].f, cells[y - 1, x - 1].r });
                        //sibling 
                        cells[y - 1, x - 1].f.links.AddRange(new List<Triangle> { cells[y, x].b, cells[y, x].l });
                        cells[y - 1, x - 1].r.links.AddRange(new List<Triangle> { cells[y, x].b, cells[y, x].l });
                    }
                
                    _triangles.AddRange(cells[y, x].Triangles());
                    _edges.AddRange(cells[y, x].edges);
                }
            }
        }
        public void ApplyTerrains()
        {
            // process vertices
            while (terrains.TryDequeue(out Terrain terrain))
            {
                terrain.Process(_vertices);
            };
            // correct central vertices
            _cells.ForEach(delegate (Cell cell)
            {
                //cell.c.coordinate.y = (cell.fr.coordinate.y + cell.fl.coordinate.y + cell.br.coordinate.y + cell.bl.coordinate.y) / 4;
            });
        }
        public void BuildZones()
        {
            // base zone
            new Zone(new("Dirt", ZoneData.Type.plain)).EnqueueForm(new FormAll()).To(this);
            List<Cell> allCells = new();
            allCells.AddRange(_cells);
            foreach (Zone zone in zones)
            {
                zone.cells = zone.Get(allCells);
                zone.cells.ForEach(delegate (Cell cell)
                {
                    allCells.Remove(cell);
                });
            }
            
        }
        public void BuildZoneParts()
        {
            foreach (Zone zone in zones)
            {
                zone.BuildParts();
            }
        }
        public void BuildBorders()
        {
            foreach (Cell cell in _cells)
            {
                cell.links.ForEach(delegate (Cell link)
                {
                    if (cell.zonePart.zone != link.zonePart.zone)
                    {
                        Vertex vertex = cell.GetTriangle((link.position - cell.position).normalized).Subdivide();
                    }
                });
            }
            foreach (Cell cell in _cells)
            {
                cell.Triangles().ForEach(delegate (Triangle triangle)
                {
                    
                    if (triangle.subTriangles.Count > 0)
                    {
                        triangle.GetSiblingsByEdge(cell.Triangles()).ForEach(delegate (Triangle sibling)
                        {
                            if (sibling.subTriangles.Count > 0)
                            {
                                Vertex cornerVertex = null;
                                sibling.Vertices.ForEach(delegate (Vertex sv)
                                {
                                    triangle.Vertices.ForEach(delegate (Vertex tv)
                                    {
                                        if (tv == sv && tv != cell.c)
                                        {
                                            cornerVertex = tv;
                                        }
                                    });
                                });
                                Vector3 dirToCenter = cell.c.coordinate - cornerVertex.coordinate;
                                cornerVertex.coordinate = cornerVertex.coordinate + dirToCenter * 0.2f;
                                Debug.DrawLine(cell.c.coordinate, cornerVertex.coordinate, Color.red, 100);
                            }
                            
                        });
                    }
                });
            }
        }
        public void RenderZones()
        {
            foreach (Zone zone in zones)
            {
                zone.BuildMesh();
            }
        }
    }
}