using Unity.VisualScripting;
using UnityEngine;

public class DragNDropNRotate : MonoBehaviour
{
    private GameObject target;
    
    private Vector3 mousePosition;
    private Vector3 offset;
    [SerializeField] private float RotationSpeed = 3;
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetCollider = Physics2D.OverlapPoint(mousePosition);
            if (targetCollider && Input.GetMouseButtonDown(0))
            {
                target = targetCollider.transform.gameObject;
                offset = target.transform.position - mousePosition;
            }
        }
        if (target)
        {
            target.transform.position = mousePosition + offset;
            
            target.transform.Rotate(Vector3.forward, RotationSpeed * Input.mouseScrollDelta.y);
        }
        
        if (target && Input.GetMouseButtonUp(0))
        {
            target = null;
        }

    }
    
}
