using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
   public bool displayGridGizmos;
   [SerializeField] private LayerMask _unwalkable;
   [SerializeField] private Vector2 _gridWorldSize;
   [SerializeField] private float _nodeSize;
   private float _halfNodeSize;
   private Node[,] _grid;
   private int _gridSizeX;
   private int _gridSizeY;

   private void Start()
   {
      _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
      _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);
      CreateGrid();
   }

   public int MaxSize
   {
      get { return _gridSizeX * _gridSizeY; }
   }

   private void CreateGrid()
   {
      _grid = new Node[_gridSizeX, _gridSizeY];
      Vector2 worldBottomLeftPos = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.up * _gridWorldSize.y / 2;
      _halfNodeSize = _nodeSize * 0.5f;
      Vector2 halfNodeSize = Vector2.one * _halfNodeSize;

      for (int x = 0; x < _grid.GetLength(0); x++)
      {
         for (int y = 0; y < _grid.GetLength(1); y++)
         {
            Vector2 nodePointInWorld = worldBottomLeftPos + Vector2.right * (x * _nodeSize + _halfNodeSize) + Vector2.up * (y * _nodeSize + _halfNodeSize);
            bool walkable = !Physics2D.OverlapBox(nodePointInWorld, halfNodeSize, 0, _unwalkable);
            _grid[x, y] = new Node(walkable, nodePointInWorld, x, y);
         }
      }
   }

   public List<Node> GetNeighbours(Node node)
   {
      List<Node> neighbours = new();
      for (int x = -1; x <= 1; x++)
      {
         for (int y = -1; y <= 1; y++)
         {
            if (x == 0 && y == 0)
               continue;

            int checkX = node.GridX + x;
            int checkY = node.GridY + y;
            if (checkX >= 0 && checkX < _grid.GetLength(0) && checkY >= 0 && checkY < _grid.GetLength(1))
            {
               neighbours.Add(_grid[checkX, checkY]);
            }
         }
      }
      return neighbours;
   }

   public Node GetNodeFromWorldPoint(Vector3 worldPoint) => NodeFromWorldPoint(worldPoint);
   private Node NodeFromWorldPoint(Vector2 worldPosition)
   {
      float percentX = (worldPosition.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
      float percentY = (worldPosition.y + _gridWorldSize.y / 2) / _gridWorldSize.y;
      percentX = Mathf.Clamp01(percentX);
      percentY = Mathf.Clamp01(percentY);

      int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
      int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
      return _grid[x, y];
   }

   void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(transform.position, _gridWorldSize);

      if (_grid != null && displayGridGizmos)
      {
         foreach (Node node in _grid)
         {
            Gizmos.color = node.Walkable ? Color.white : Color.red;
            Gizmos.DrawCube(node.WorldPosition, Vector2.one * _halfNodeSize * 1.9f);
         }
      }
   }
}
