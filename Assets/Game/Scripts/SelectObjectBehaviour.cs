using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems; // Добавляем это пространство имен

public class SelectObjectBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Vector3 mousePosition;

    [SerializeField] private Canvas UICanvas;
    [SerializeField] private ScaleEditor scaleEditorPrefab;
    [SerializeField] private FocusEditor focusEditorPrefab;
    [SerializeField] private AddObjectsMenu addObjectsMenuPrefab;
    [SerializeField] private LineControl lineControlPrefab;
    
    private ScaleEditor currentScaleEditor; 
    private FocusEditor currentFocusEditor;
    private AddObjectsMenu currentAddObjectsMenu;
    private LineControl currentLineControl;

    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Collider2D targetCollider = Physics2D.OverlapPoint(mousePosition);
            if (targetCollider)
            {
                GameObject newTarget = targetCollider.transform.gameObject;
                
                if (newTarget == target && currentScaleEditor)
                {
                    return;
                }
                
                target = newTarget;
                
                if (currentScaleEditor)
                {
                    Destroy(currentScaleEditor.gameObject);
                }

                if (currentFocusEditor)
                {
                    Destroy(currentFocusEditor.gameObject);
                }

                if (currentLineControl)
                {
                    Destroy(currentLineControl.gameObject);
                }
                
                currentScaleEditor = Instantiate(scaleEditorPrefab, UICanvas.transform);
                currentScaleEditor.target = target;
                
                
                switch (target.tag)
                {
                    case "LensContainer":
                        currentFocusEditor = Instantiate(focusEditorPrefab, UICanvas.transform);
                        currentFocusEditor.target = target;
                        break;
                    case "Player":
                        currentLineControl = Instantiate(lineControlPrefab, UICanvas.transform);
                        currentLineControl.target = target.GetComponent<Laser>();
                        break;
                }
            }
            else
            {
                if (currentAddObjectsMenu)
                {
                    Destroy(currentAddObjectsMenu.gameObject);
                }

                currentAddObjectsMenu = Instantiate(addObjectsMenuPrefab, UICanvas.transform);
                 
            } 
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Collider2D targetCollider = Physics2D.OverlapPoint(mousePosition);
            if (targetCollider == null)
            {
                target = null;
                DeleteGui();
            }
        }    
    }
    
    public void DeleteGui()
    {
        if (currentScaleEditor)
        {
            Destroy(currentScaleEditor.gameObject);
            currentScaleEditor = null;
        }
        if (currentFocusEditor)
        {
            Destroy(currentFocusEditor.gameObject);
            currentFocusEditor = null;
        }
        if (currentAddObjectsMenu)
        {
            Destroy(currentAddObjectsMenu.gameObject);
            currentAddObjectsMenu = null;
        }

        if (currentLineControl)
        {
            Destroy(currentLineControl.gameObject);
            currentLineControl = null;
        }
    }
}