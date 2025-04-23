using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Laser : MonoBehaviour
{
    private List<Vector3> PointList = new List<Vector3>();
    private List<LineRenderer> debugRenderers = new List<LineRenderer>();
    public bool isLineVisible = true;
    
    [SerializeField] private LineRenderer RayRender;
    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject DebugRendererPrefab; // Префаб с LineRenderer для отладки

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
        
        ClearDebugRenderers();

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

    private void ClearDebugRenderers()
    {
        foreach (var renderer in debugRenderers)
        {
            if (renderer != null)
            {
                Destroy(renderer.gameObject);
            }
        }
        debugRenderers.Clear();
    }
    
    private LineRenderer CreateDebugRenderer(Color color)
    {
        if (DebugRendererPrefab == null)
        {
            GameObject debugObj = new GameObject("DebugRenderer");
            LineRenderer lr = debugObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = color;
            lr.enabled = isLineVisible;
            debugRenderers.Add(lr);
            debugObj.transform.SetParent(this.transform);
            return lr;
        }
        else
        {
            GameObject debugObj = Instantiate(DebugRendererPrefab);
            LineRenderer lr = debugObj.GetComponent<LineRenderer>();
            lr.startColor = color;
            lr.endColor = color;
            debugRenderers.Add(lr);
            return lr;
        }
    }

    private void DrawDebugRay(Vector2 start, Vector2 direction, float length, Color color)
    {
        LineRenderer lr = CreateDebugRenderer(color);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, start + direction * length);
    }

    private void GetRayPath(Vector2 startPosition, Vector2 direction, int depth = 0)
    {
        if (depth > MaxReflections) return;
        
        hit = Physics2D.Raycast(startPosition, direction, MaxRayLength, ~ignoreLayer);

        if (hit.collider)
        {
            PointList.Add(hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                Vector2 newDirection = Vector2.Reflect(direction, hit.normal).normalized;
                GetRayPath(hit.point + newDirection, newDirection, depth + 1);
            }
            else if (hit.collider.CompareTag("Lens"))
            {
                Vector3 hitSide = hit.transform.InverseTransformDirection(hit.normal).normalized;
                Lens lens = hit.collider.GetComponent<Lens>();
                if (hitSide.x < 0)
                {
                    CalculateLensReflection(direction, depth, lens, lens.Focus, lens.FocusPoint);
                }
                else
                {
                    CalculateLensReflection(direction, depth, lens, lens.SecondaryFocus, lens.SecondaryFocusPoint);
                }
            }
        }
        else
        {
            PointList.Add(startPosition + direction * MaxRayLength);
        } 
    }

    private void CalculateLensReflection(Vector2 direction, int depth, Lens lens, float focus, Vector2 focusPoint)
    {
        Vector2 normal = hit.normal;

        if (Mathf.Abs(focus) > 0.1f)
        {
            Vector2 opticAxis = (focusPoint - (Vector2)lens.transform.position).normalized;
            Vector2 focalPlaneNormal = new Vector2(-opticAxis.y, opticAxis.x);
            
            float focusDistance = Vector2.Distance(lens.transform.position, focusPoint);
            float debugRayLength = focusDistance * 1.5f; 

            float distanceFromOpticAxis = Vector2.Dot(direction.normalized, focalPlaneNormal);
            float focusPlaneOffset = distanceFromOpticAxis * focus;
            Vector2 focusPlanePoint;

            Vector2 ray1Start = focusPoint;
            Vector2 ray1Direction = focalPlaneNormal;
            
            Vector2 ray2Start = lens.transform.position;
            Vector2 ray2Direction = direction;

            if (TryGetRayIntersection(ray1Start, ray1Direction, ray2Start, ray2Direction, out Vector2 intersection))
            {
                focusPlanePoint = intersection;
            }
            else
            {
                focusPlanePoint = focusPoint + focusPlaneOffset * focalPlaneNormal;
            }

            // Рисуем отладочные лучи с рассчитанной длиной
            DrawDebugRay(focusPoint, focalPlaneNormal, 10f, Color.magenta); // Фокальная плоскость
            DrawDebugRay(focusPoint, -focalPlaneNormal, 10f, Color.magenta);
            DrawDebugRay(lens.transform.position, opticAxis, focusDistance * 1.5f, Color.magenta); // Главная оптическая ось

            if (lens.Focus > 0) // Собирающая линза
            {
                DrawDebugRay(lens.transform.position, direction, debugRayLength, Color.green);
                Vector2 newDirection = (focusPlanePoint - hit.point).normalized;
                GetRayPath(hit.point + newDirection, newDirection, depth + 1);
            }
            else // Рассеивающая линза
            {
                Vector2 secondaryOpticAxis = direction.normalized;
                Vector2 secondaryFocusPoint;

                if (TryGetRayIntersection(lens.transform.position, direction, focusPoint, focalPlaneNormal, out secondaryFocusPoint))
                {
                    Vector2 newDirection = (hit.point - secondaryFocusPoint).normalized;
                    DrawDebugRay(lens.transform.position, -direction, debugRayLength, Color.green);
                    DrawDebugRay(secondaryFocusPoint, newDirection, debugRayLength, Color.green);
                    GetRayPath(hit.point + newDirection, newDirection, depth + 1);
                }
                else
                {
                    Vector2 newDirection = Vector2.Reflect(direction, normal).normalized;
                    GetRayPath(hit.point + newDirection, newDirection, depth + 1);
                }
            }
        }
        else
        {
            float n1 = 1.0f;
            float n2 = lens.RefractiveIndex;

            if (Vector2.Dot(direction, normal) > 0)
            {
                normal = -normal;
                float temp = n1;
                n1 = n2;
                n2 = temp;
            }

            float n = n1 / n2;
            float cosI = -Vector2.Dot(normal, direction);
            float sinT2 = n * n * (1.0f - cosI * cosI);

            if (sinT2 > 1.0f)
            {
                Vector2 newDirection = Vector2.Reflect(direction, normal).normalized;
                GetRayPath(hit.point + newDirection * 0.1f, newDirection, depth + 1);
            }
            else
            {
                float cosT = Mathf.Sqrt(1.0f - sinT2);
                Vector2 newDirection = n * direction + (n * cosI - cosT) * normal;
                newDirection.Normalize();
                GetRayPath(hit.point + newDirection * 0.1f, newDirection, depth + 1);
            }
        }
    }

    private bool TryGetRayIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2, out Vector2 intersection)
    {
        float a1 = d1.y;
        float b1 = -d1.x;
        float c1 = a1 * p1.x + b1 * p1.y;

        float a2 = d2.y;
        float b2 = -d2.x;
        float c2 = a2 * p2.x + b2 * p2.y;

        float det = a1 * b2 - a2 * b1;

        if (Mathf.Abs(det) < 1e-6)
        {
            intersection = Vector2.zero;
            return false;
        }

        intersection = new Vector2((b2 * c1 - b1 * c2) / det, (a1 * c2 - a2 * c1) / det);
        return true;
    }
}