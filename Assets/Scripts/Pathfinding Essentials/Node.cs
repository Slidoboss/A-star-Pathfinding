using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
   private bool _walkable;
   private Vector2 _worldPosition;
   private int _gridX, _gridY;

   //gcost is how many units away the node is from the start node through the last expolored path.
   //hcost is  how many units away the node is from the end node through the last expolored path.
   public int gCost, hCost;
   public int fCost { get { return gCost + hCost; } }
   public Node parent;

   #region GETTERS
   public Vector2 WorldPosition
   {
      get { return _worldPosition; }
   }
   public bool Walkable
   {
      get { return _walkable; }
   }
   public int GridX
   {
      get { return _gridX; }
   }
   public int GridY
   {
      get { return _gridY; }
   }
   #endregion
   public Node(bool walkable, Vector2 worldPosition, int gridX, int gridY)
   {
      _walkable = walkable;
      _worldPosition = worldPosition;
      _gridX = gridX;
      _gridY = gridY;
   }
}
