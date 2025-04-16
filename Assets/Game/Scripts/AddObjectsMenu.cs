using UnityEngine;
using UnityEngine.UI;

public class AddObjectsMenu : MonoBehaviour
{
    [SerializeField] private GameObject lens;
    [SerializeField] private GameObject mirror;
    [SerializeField] private GameObject laser;
    [SerializeField] private Vector3 positionOffset;
    private Vector3 screenPosition;
    private RectTransform rectTransform;
    private Camera mainCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //mainCamera = Camera.main;
        mainCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        screenPosition = Input.mousePosition + positionOffset;
        rectTransform.position = screenPosition;
    }

    public void OnMirrorButtonClicked()
    {
        InstantiateAtMousePosition(mirror);
        Destroy(gameObject);
    }

    public void OnLensButtonClicked()
    {
        InstantiateAtMousePosition(lens);
        Destroy(gameObject);
    }
    
    public void OnLaserButtonClicked()
    {
        InstantiateAtMousePosition(laser);
        Destroy(gameObject);
    }

    private void InstantiateAtMousePosition(GameObject prefab)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        Instantiate(prefab, worldPosition, Quaternion.identity);
    }
}