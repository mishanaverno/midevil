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
		{//��������������� ����� ������ ���� �� ����� �� ��������� ����� �� ������� 
			Node startNode = graph[(int)start.x, (int)start.y];
			Node targetNode = graph[(int)target.x, (int)target.y];
			List<Node> openSet = new();// ������ ������ ������� ����� ���������
			List<Node> closeSet = new();// ������ ��� ����������� ������
			startNode.parentNode = null; // ������������� ���������� ��������� ������
			startNode.gCost = 0; //���������� �� ������ 0
			startNode.hCost = GetHeuristicPathLength(startNode, targetNode); // ��������� ���������� �� ��������� ������ ��� ����� �����������
			startNode.RefreshFCost(); // �������� ����� ����������
			openSet.Add(startNode); // ���������� � ������ ��������� �����
			int dirCounter = 0;
	
			while (openSet.Count > 0)
			{ // ���� �������� ������ ���� ������� ������ �� ����� ����
				if (dirCounter >= dirModifier)
				{
					dirCounter = 0;
				}
				dirCounter++;
				Node currentNode = GetMinFCost(openSet); // ��������� ������ � ����������� ����� ���������� �� �������� ����� �� ��������� ������
				if (currentNode == targetNode)
				{ // ���� ����������� ������ � ���� �������� 
					return GetPathFromNode(currentNode);        // �� �������� ���� � ������� �� �������
				}
				openSet.Remove(currentNode); //������� �� ������ ��������� �������� ����������� ������ 
				closeSet.Add(currentNode); //� ���������� �� � ������ ����������
				foreach (Node node in GetNeigbours(currentNode))
				{// �������� ����� �������� � �����������
					if (closeSet.Contains(node)) // ���� ����� ��� ���� � ������ ����������, ���������� �������� � ��������� � ����������
						continue;
					if (node.cell.isWalkable)
					{ // ���� ����� �������� ��� ������������
						float dir = currentNode.dir != GetDirection(node, currentNode) && dirCounter != dirModifier ? 2 : 1 ;
						if (openSet.Contains(node))
						{// ���� ����� ��� ���� � ������ �� ��������
							if (node.gCost > currentNode.gCost + dir)
							{// ���� ���������� �� ������ �� ������ � ������ ������ ��� �� ������
								node.parentNode = currentNode;//������������� �� ������ �� ����������� ������
								node.dir = GetDirection(node, currentNode);
								node.gCost = currentNode.gCost + dir; // ������ ���������� �� ������ ������, ��� ��� ��� ������ 
								node.hCost = GetHeuristicPathLength(node, targetNode);
								node.RefreshFCost(); // ������������� ����� ����
							}
						}
						else
						{// ���� ������ ��� � ������ �� ��������
							node.parentNode = currentNode; // ��������� ������ �� ����������� ������
							node.dir = GetDirection(node, currentNode);
							node.gCost = currentNode.gCost + dir; // ����������� ���������� �� ��������� ����� �� ������ �� 1
							node.hCost = GetHeuristicPathLength(node, targetNode); // ��������� ��������������� ���������� �� ������ �� �������� �����
							node.RefreshFCost(); //������������� ����� ����������
							openSet.Add(node); // � ��������� ������ � ������ �� ��������
						}
					}
				}
			}
			//���� ���� �� ����� ���� �� �������� �����
			return new(); // ���������� ������ ������
		}
		public static Vector2 GetDirection(Node node, Node currentNode)
        {
			return new Vector2(node.cell.c.coordinate.x, node.cell.c.coordinate.z) - new Vector2(currentNode.cell.c.coordinate.x, currentNode.cell.c.coordinate.z);
		}
		private static List<Cell> GetPathFromNode(Node node)
		{// �������������� ���� �� ������� �� ����� � ����� �� ������� � ��� �������  pathfinder, 
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
		{// ����� ��������� �� c����� ������ � ����������� ��������� ������ ����
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
		{// ����� ��������� ���� �� �������� ����� ��������� �����������
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
		{ // ����� ����������� fCost, ������ ��������� ��� ��������� hCost ��� gCost
			fCost = hCost + gCost;
		}
	}
}
