using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LineControl : MonoBehaviour
{
    [SerializeField] private Vector3 positionOffset = new Vector3(3f, 1f);
    public Laser target;
    [SerializeField] private Toggle toggle;
    private RectTransform rectTransform; 
    private Camera mainCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //mainCamera = Camera.main;
        mainCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        toggle.isOn = target.isLineVisible;
    }
    
    void Update()
    {
        if (target && mainCamera)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
            rectTransform.position = screenPosition;
        }
    }

    public void OnValChanged()
    {
        target.isLineVisible = toggle.isOn;
    }

    
}