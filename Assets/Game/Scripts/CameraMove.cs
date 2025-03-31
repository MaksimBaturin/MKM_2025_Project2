using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 2f;
    private Vector3 dragOrigin;
    private bool isDragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!Physics2D.OverlapPoint(mousePos))
            {
                dragOrigin = Input.mousePosition;
                isDragging = true;
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Процесс перетаскивания
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
            Camera.main.transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }
    }
}