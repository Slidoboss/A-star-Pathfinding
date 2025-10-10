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
   [SerializeField][Range(0, 100)] private int _blurAmount = 3;
   [SerializeField] private int _obstacleProximityPenalty = 20;
   private LayerMask _walkableMask;
   private float _halfNodeSize;
   private Node[,] _grid;
   private int _gridSizeX;
   private int _gridSizeY;
   private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();

   private int penaltyMin = int.MaxValue;
   private int penaltyMax = int.MinValue;

   [Header("Terrain List")] //This stops one stupid editor error from occuring. If you no believe try remove am.
   public TerrainType[] walkableRegions; //Added this for the weights part


   private void Awake()
   {
      _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
      _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);
      foreach (TerrainType region in walkableRegions)
      {
         _walkableMask.value |= region.terrainMask.value;
         _walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.penalty);
      }
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

            int movementPenalty = 0;
            Collider2D collider = Physics2D.OverlapPoint(nodePointInWorld, _walkableMask);
            if (collider != null)
            {
               _walkableRegionsDictionary.TryGetValue(collider.gameObject.layer, out movementPenalty);
            }
            if (!walkable)
            {
               movementPenalty += _obstacleProximityPenalty;
            }

            _grid[x, y] = new Node(walkable, nodePointInWorld, x, y, movementPenalty);
         }
      }
      BlurPenaltyMap(_blurAmount);
   }

   private void BlurPenaltyMap(int blurSize)
   {
      int kernelSize = blurSize * 2 + 1; //Done to always get odd number.
      int kernelExtents = (kernelSize - 1) / 2;

      int[,] penaltiesHorizontalPass = new int[_gridSizeX, _gridSizeY];
      int[,] penaltiesVerticalPass = new int[_gridSizeX, _gridSizeY];

      for (int y = 0; y < _gridSizeY; y++)
      {
         for (int x = -kernelExtents; x <= kernelExtents; x++)
         {
            int sampleX = Mathf.Clamp(x, 0, kernelExtents);
            penaltiesHorizontalPass[0, y] += _grid[sampleX, y].MovementPenalty;
         }
         for (int x = 1; x < _gridSizeX; x++)
         {
            int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
            int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);
            penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - _grid[removeIndex, y].MovementPenalty + _grid[addIndex, y].MovementPenalty; //The Formula.
         }
      }
      for (int x = 0; x < _gridSizeX; x++)
      {
         for (int y = -kernelExtents; y <= kernelExtents; y++)
         {
            int sampleY = Mathf.Clamp(y, 0, kernelExtents);
            penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
         }

         int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
         _grid[x, 0].MovementPenalty = blurredPenalty;

         for (int y = 1; y < _gridSizeY; y++)
         {
            int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
            int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);
            penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex]; //The Formula.
            blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
            _grid[x, y].MovementPenalty = blurredPenalty;

            #region DEBUG ZONE
            if (blurredPenalty > penaltyMax)
            {
               penaltyMax = blurredPenalty;
            }
            if (blurredPenalty < penaltyMin)
            {
               penaltyMin = blurredPenalty;
            }
            #endregion
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
            Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, node.MovementPenalty));
            Gizmos.color = node.Walkable ? Gizmos.color : Color.red;
            Gizmos.DrawCube(node.WorldPosition, Vector2.one); //* _halfNodeSize * 1.9f);
         }
      }
   }

}
