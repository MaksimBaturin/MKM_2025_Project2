using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraMove : MonoBehaviour
{
    [SerializeField] private float dragSpeedX;
    [SerializeField] private float dragSpeedY;
    private Vector3 dragOrigin;
    private bool isDragging;

    private void Start()
    {
        dragSpeedX = Screen.width / 100f;
        dragSpeedY = Screen.height / 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
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
            Vector3 move = new Vector3(-pos.x * dragSpeedX, -pos.y * dragSpeedY, 0);
            Camera.main.transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }
    }
}