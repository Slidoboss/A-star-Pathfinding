using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    float _speed = 10.0f;
    Vector2[] _path;
    private int _targetIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }
    private IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        while (true)
        {
            if (transform.position.ConvertTo<Vector2>() == currentWaypoint)
            {
                _targetIndex++;
                if (_targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentWaypoint = _path[_targetIndex];
            }
            //Adjust and compartmentalize into a Move method.
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
            yield return null;
        }
    }
    public void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.darkBlue;
                Gizmos.DrawCube(_path[i], Vector2.one);
                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
             }    
        }
    }
}
