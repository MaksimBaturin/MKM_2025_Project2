using UnityEngine;

public class Lens : MonoBehaviour
{
    public float RefractiveIndex = 1.8f;
    public float Focus = 15;
    public float SecondaryFocus;

    [SerializeField] Sprite biconvex;
    
    [SerializeField] Sprite biconcave;
    
    public Vector2 FocusPoint;
    public Vector2 SecondaryFocusPoint;

    [SerializeField] SpriteRenderer spriteRenderer;
    

    private void Start()
    {
        UpdateLens();
    }

    private void Update()
    {
        UpdateLens();
    }

    private void UpdateLens()
    {
        
        SecondaryFocus = -Focus;
        
        UpdateLensSprite();
        
        UpdateFocusPoint();
    }

    private void UpdateLensSprite()
    {
        if (Focus > 0)
        {
            spriteRenderer.sprite = biconvex;
        }
        else if (Focus < 0)
        {
            spriteRenderer.sprite = biconcave;
        }
    }

    private void UpdateFocusPoint()
    {
        float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        
        float offsetX = Focus * Mathf.Cos(angleInRadians);
        float offsetY = Focus * Mathf.Sin(angleInRadians);
        
        FocusPoint = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        SecondaryFocusPoint = new Vector2(transform.position.x - offsetX, transform.position.y - offsetY);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FocusPoint, 0.1f);
    }
}