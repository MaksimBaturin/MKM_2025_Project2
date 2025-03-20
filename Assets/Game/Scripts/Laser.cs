using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Laser : MonoBehaviour
{
    private List<Vector3> PointList = new List<Vector3>();

    [SerializeField] private LineRenderer RayRender;
    [SerializeField] private GameObject StartPoint;

    public int MaxReflections = 10;
    public float MaxRayLength = 500;

    private RaycastHit2D hit;
    
    public LayerMask ignoreLayer;

    void Start()
    {
        RayRender.startWidth = 0.1f;
        RayRender.endWidth = 0.1f;
        RayRender.widthMultiplier = 1f;
    }

    void Update()
    {
        RayRender.widthMultiplier = 1f;

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

        // Используем LayerMask для исключения слоя
        hit = Physics2D.Raycast(startPosition, direction, MaxRayLength, ~ignoreLayer);

        if (hit.collider)
        {
            PointList.Add(hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                Vector2 newDirection = (direction - 2 * (Vector2.Dot(direction, hit.normal)) * hit.normal).normalized;
                GetRayPath(hit.point + newDirection, newDirection, depth + 1);
            }
            else if (hit.collider.CompareTag("ConvexLens"))
            {
                ConvexLens lens = hit.collider.GetComponent<ConvexLens>();
                float d = ((Vector3)hit.point - lens.transform.position).magnitude;
                
                float delta = d / lens.Focus;
                
                Vector2 focusDirection = (lens.FocusPoint - hit.point).normalized;
                
                Vector2 newDirection = Quaternion.Euler(0, 0, delta * Mathf.Rad2Deg) * focusDirection;
                
                GetRayPath(hit.point + newDirection, newDirection, depth + 1);
            }
        }
        else
        {
            PointList.Add(startPosition + direction * MaxRayLength);
        }
    }
}