using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator;

namespace Game
{
    public static class PathFinder
    {
		public static Node[,] graph;
		public static void InitGraph(Cell[,] map)
        {
			graph = new Node[map.GetLength(0), map.GetLength(1)];
			for (int y = 0; y < map.GetLength(0); y++)
			{
				for (int x = 0; x < map.GetLength(1); x++)
				{
					Node node = new();
					graph[y, x] = node;
					node.cell = map[y, x];
					if (x > 0)
                    {
						node.links.Add(graph[y, x - 1]);
						graph[y, x - 1].links.Add(node);

					}
					if (y > 0)
                    {
						node.links.Add(graph[y - 1, x]);
						graph[y - 1, x].links.Add(node);
                    }

					
				}
			}
		}
		public static List<Cell> FindPath(Vector2 start, Vector2 target, int dirModifier = 0)
		{//непосредственно метод поиска пути на карте от начальной точки до коненчо 
			Node startNode = graph[(int)start.x, (int)start.y];
			Node targetNode = graph[(int)target.x, (int)target.y];
			List<Node> openSet = new();// список клеток которые нужно проверить
			List<Node> closeSet = new();// список уже проверенных клеток
			startNode.parentNode = null; // инициализация параметров стартовой клетки
			startNode.gCost = 0; //расстояние от начала 0
			startNode.hCost = GetHeuristicPathLength(startNode, targetNode); // получение расстояния до конечнной клетки без учета препятствий
			startNode.RefreshFCost(); // пересчет суммы расстояний
			openSet.Add(startNode); // добавление в список стартовой точки
			int dirCounter = 0;
	
			while (openSet.Count > 0)
			{ // цикл проверки клеток пока окрытый список не будет пуст
				if (dirCounter >= dirModifier)
				{
					dirCounter = 0;
				}
				dirCounter++;
				Node currentNode = GetMinFCost(openSet); // получение клетки с минимальным общим расстоянем до конечной точки из открытого списка
				if (currentNode == targetNode)
				{ // если проверяемая клетка и есть конечная 
					return GetPathFromNode(currentNode);        // то получаем путь и выходим из функции
				}
				openSet.Remove(currentNode); //удаляем из списка ожидающих проверку проверяемую клетку 
				closeSet.Add(currentNode); //и записываем ее в список провереных
				foreach (Node node in GetNeigbours(currentNode))
				{// получаем точки соседние с проверяемой
					if (closeSet.Contains(node)) // если сосед уже есть в списке провереных, пропускаем итерацию и переходим к следующему
						continue;
					if (node.cell.isWalkable)
					{ // если сосед доступен для передвижения
						float dir = currentNode.dir != GetDirection(node, currentNode) && dirCounter != dirModifier ? 2 : 1 ;
						if (openSet.Contains(node))
						{// если сосед уже есть в списке на проверку
							if (node.gCost > currentNode.gCost + dir)
							{// если расстояние от старта до клетки в списке больше чем до соседа
								node.parentNode = currentNode;//устанавливаем ей ссылку на проверяемую клетку
								node.dir = GetDirection(node, currentNode);
								node.gCost = currentNode.gCost + dir; // задаем расстояние от старта соседа, так как оно меньше 
								node.hCost = GetHeuristicPathLength(node, targetNode);
								node.RefreshFCost(); // пересчитываем общий путь
							}
						}
						else
						{// если соседа нет в списке на проверку
							node.parentNode = currentNode; // сохраняем ссылку на проверяемую клетку
							node.dir = GetDirection(node, currentNode);
							node.gCost = currentNode.gCost + dir; // увеличиваем расстояние от стартовой точки до соседа на 1
							node.hCost = GetHeuristicPathLength(node, targetNode); // вычисляем приблизительное расстояние от соседа до конечной точки
							node.RefreshFCost(); //пересчитываем общее расстояние
							openSet.Add(node); // и добавляем соседа в список на проверку
						}
					}
				}
			}
			//если цикл не нашел путь до конечной точки
			return new(); // возвращаем пустой список
		}
		public static Vector2 GetDirection(Node node, Node currentNode)
        {
			return new Vector2(node.cell.c.coordinate.x, node.cell.c.coordinate.z) - new Vector2(currentNode.cell.c.coordinate.x, currentNode.cell.c.coordinate.z);
		}
		private static List<Cell> GetPathFromNode(Node node)
		{// восспроизводит путь по ссылкам от точки к точке из которой в нее перешел  pathfinder, 
			List<Cell> result = new();
			Node currentNode = node;
			while (currentNode != null)
			{
				result.Add(currentNode.cell);
				currentNode = currentNode.parentNode;
			}
			result.Reverse();

			return result;
		}
		private static Node GetMinFCost(List<Node> list)
		{// метод получения из cписка клетки с минимальным значением общего пути
			float min = float.MaxValue;
			Node minNode = null;
			foreach (Node node in list)
			{
				if (node.fCost < min)
				{
					min = node.fCost;
					minNode = node;
				}
			}

			return minNode;
		}
		private static float GetHeuristicPathLength(Node startNode, Node targetNode)
		{// метод получения пути до конечной точки игнорируя препятствия
			return Mathf.Max(
				Mathf.Abs(startNode.cell.c.coordinate.x - targetNode.cell.c.coordinate.x),
				Mathf.Abs(startNode.cell.c.coordinate.z - targetNode.cell.c.coordinate.z),
				Mathf.Abs(startNode.cell.c.coordinate.y - targetNode.cell.c.coordinate.y));
		}
		private static List<Node> GetNeigbours(Node node) => node.links;


	}
	public class Node
    {
		public Cell cell;
		public Node parentNode;
		public Vector2 dir = Vector2.zero;
		public float gCost, hCost, fCost;
		public List<Node> links = new();
		public void RefreshFCost()
		{ // метод вычисляющий fCost, должен вызыватся при изменении hCost или gCost
			fCost = hCost + gCost;
		}
	}
}
