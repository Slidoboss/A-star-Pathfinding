using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> _pathRequestsQueue = new Queue<PathRequest>();
    PathRequest _currentPathRequest;
    static PathRequestManager instance;
    Pathfinding _pathfinding;
    bool _isProcessingPath;

    void Awake()
    {
        instance = this;
        _pathfinding = GetComponent<Pathfinding>();
    }
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance._pathRequestsQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }
    private void TryProcessNext()
    {
        if (!_isProcessingPath && _pathRequestsQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestsQueue.Dequeue();
            _isProcessingPath = true;
            _pathfinding.StartFindPath(_currentPathRequest.pathStart, _currentPathRequest.pathEnd);
        }
    }
    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        _currentPathRequest.callback(path, success);
        _isProcessingPath = false;
        TryProcessNext();

    }
}
