using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FocusEditor : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Vector3 positionOffset = new Vector3(3f, 1f);
    public GameObject target;
    
    private float lastScaleFactor = 1f;
    private RectTransform rectTransform; 
    private Camera mainCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        
        if (target != null)
        {
            inputField.text = target.GetComponentInChildren<Lens>().Focus.ToString();
        }
    }
    
    void Update()
    {
        if (target && mainCamera)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
            rectTransform.position = screenPosition;
        }
    }

    public void OnInputChange()
    {
        if (target == null) return;
        
        if (float.TryParse(inputField.text, out float result))
        {
            target.GetComponentInChildren<Lens>().Focus = result;
        }
        else
        {
            target.GetComponentInChildren<Lens>().Focus = 0;
        }
    }
}