using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class Laser :MonoBehaviour
{
    private List<Vector3> PointList = new List<Vector3>();

    [SerializeField] private LineRenderer RayRender;
    [SerializeField] private GameObject StartPoint;

    public int MaxReflections = 10;
    public float MaxRayLength = 500;
    
    private RaycastHit2D hit;
    void Start()
    {
        RayRender.startWidth = 0.1f;
        RayRender.endWidth = 0.1f;
        RayRender.widthMultiplier = 1f;
    }

    void Update()
    {
        RayRender.widthMultiplier = 1f; // Предотвращаем сужение

        PointList.Clear();
        PointList.Add(StartPoint.transform.position);

        Vector2 dir = transform.TransformDirection(Vector2.up.normalized);
        GetRayPath(StartPoint.transform.position, dir);

        if (PointList.Count >= 2)
        {
            RayRender.positionCount = PointList.Count;
            RayRender.SetPositions(PointList.ToArray());
        }
    }


    private RaycastHit2D[] hits = new RaycastHit2D[1];

    private void GetRayPath(Vector2 startPosition, Vector2 direction, int depth = 0)
    {
        if (depth > MaxReflections) return;

        hit = Physics2D.Raycast(startPosition, direction);
        if (hit.collider != null)
        {
            PointList.Add(hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                Vector2 newDirection = (direction - 2 * (Vector2.Dot(direction, hit.normal)) * hit.normal).normalized;
                GetRayPath(hit.point + newDirection * 0.01f, newDirection, depth + 1);
            }
        }
        else
        {
            PointList.Add(startPosition + direction * MaxRayLength);
        }
    }

}

