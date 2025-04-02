using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float ZoomSpeed = 1;
    private Vector3 mousePosition;
    
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.LeftControl))
        {
            if (!Physics2D.OverlapPoint(mousePosition))
            {
                var newSize = Camera.main.orthographicSize - Input.mouseScrollDelta.y * ZoomSpeed;
                if (newSize <= 0)
                {
                    Camera.main.orthographicSize = 1;
                }
                else
                {
                    Camera.main.orthographicSize = newSize;
                }
            }
            
        }
    }
}
