using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PathRequest
{
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Action<Vector2[], bool> callback;

    public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}
