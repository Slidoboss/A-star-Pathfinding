using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDGridSpawner: MonoBehaviour
{
    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField] private float _nodeSize;
    private float _halfNodeSize;
    private GameObject[,] _grid;
    private GameObject _type;
    private int _gridSizeX;
    private int _gridSizeY;

    private void Start()
    {
        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);
        CreateGrid();
    }

    private void CreateGrid()
    {
        _grid = new GameObject[_gridSizeX, _gridSizeY];
        Vector2 worldBottomLeftPos = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.up * _gridWorldSize.y / 2;
        _halfNodeSize = _nodeSize * 0.5f;
        Vector2 nodeSize = Vector2.one * _nodeSize;//Use this on localScale for tightly packed gameobjects.

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                Vector2 nodePointInWorld = worldBottomLeftPos + Vector2.right * (x * _nodeSize + _halfNodeSize) + Vector2.up * (y * _nodeSize + _halfNodeSize);
                GameObject type = Instantiate(_type, nodePointInWorld,Quaternion.identity);
                //Edit component of GameObject.
                //Assign gameobject to grid array.
                SetObjectAtGridIndex(x, y,type);
            }
        }
    }
    private GameObject GetGameObjectFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
        float percentY = (worldPosition.y + _gridWorldSize.y / 2) / _gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _grid[x, y];
    }
    private void SetObjectAtGridIndex(int x, int y, GameObject value)
    {
        if (x >= 0 && y >= 0 && x < _grid.GetLength(0) && y < _grid.GetLength(1))
            _grid[x, y] = value;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _gridWorldSize);
    }
}
