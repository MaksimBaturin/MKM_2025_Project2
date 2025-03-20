using UnityEngine;

public class ConvexLens : MonoBehaviour
{
    public float R1 = 10;
    public float R2 = -15;
    public float RefrectiveIndex = 0.8f;
    public float Focus;

    public Vector2 FocusPoint;
    public void Start()
    {
        Focus = 1/((RefrectiveIndex-1)*(1/R1-1/R2));
        
        FocusPoint = new Vector2(transform.position.x + Focus, transform.position.y);
    }
}
