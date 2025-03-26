using UnityEngine;

public class Lens : MonoBehaviour
{
    public float R1 = 10;
    public float R2 = -15;
    public float RefractiveIndex = 1.8f;
    public float Focus;

    [SerializeField] Sprite biconvex;
    [SerializeField] Sprite flatConvex;
    [SerializeField] Sprite convexConcave1;
    
    [SerializeField] Sprite biconcave;
    [SerializeField] Sprite flatConcave;
    [SerializeField] Sprite convexConcave2;

    public Vector2 FocusPoint;

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
        if (R1 == 0 || R2 == 0)
        {
            Debug.LogError("Радиусы кривизны не могут быть нулевыми.");
            return;
        }
        
        Focus = 1 / ((RefractiveIndex - 1) * (1 / R1 - 1 / R2));
        
        UpdateLensSprite();
        
        UpdateFocusPoint();
    }

    private void UpdateLensSprite()
    {
        if (R1 > 0 && R2 > 0)
        {
            spriteRenderer.sprite = biconvex;
        }
        else if (R1 > 0 && R2 == 0)
        {
            spriteRenderer.sprite = flatConvex;
        }
        else if (R1 > 0 && R2 < 0)
        {
            spriteRenderer.sprite = convexConcave1;
        }
        else if (R1 < 0 && R2 < 0)
        {
            spriteRenderer.sprite = biconcave;
        }
        else if (R1 == 0 && R2 < 0)
        {
            spriteRenderer.sprite = flatConcave;
        }
        else if (R1 < 0 && R2 > 0)
        {
            spriteRenderer.sprite = convexConcave2;
        }
    }

    private void UpdateFocusPoint()
    {
        float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        
        float offsetX = Focus * Mathf.Cos(angleInRadians);
        float offsetY = Focus * Mathf.Sin(angleInRadians);
        
        FocusPoint = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FocusPoint, 0.1f);
    }
}