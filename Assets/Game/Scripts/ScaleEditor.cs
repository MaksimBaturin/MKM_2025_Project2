using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleEditor : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Vector3 positionOffset = new Vector3(3f, 1f);
    public float minValue = 0f;
    public float maxValue = 500f; 
    public GameObject target;
    
    private Vector3 initialScale;
    private string ScalePrefKey;
    private float lastScaleFactor = 1f;
    private RectTransform rectTransform;
    private Camera mainCamera;

    void Start()
    {
        ScalePrefKey = target.GetInstanceID()+"Scale";
        initialScale = target.GetComponent<ScaleSaver>().initialScale;
        rectTransform = GetComponent<RectTransform>();
        
        //mainCamera = Camera.main;
        mainCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        
        lastScaleFactor = PlayerPrefs.GetFloat(ScalePrefKey, 1f);
        
        scrollbar.value = Mathf.InverseLerp(minValue, maxValue, lastScaleFactor * 100f);
        
        OnScrollBarValueChanged();
    }

    void Update()
    {
        if (target && mainCamera)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
            rectTransform.position = screenPosition;
        }
    }
    
    public void OnScrollBarValueChanged()
    {  
        float ResultSize = Mathf.Lerp(minValue, maxValue, scrollbar.value);
        resultText.text = ResultSize.ToString("F1");
        lastScaleFactor = ResultSize / 100f;
        ChangeScale(lastScaleFactor);
    }

    private void ChangeScale(float scaleFactor)
    {
        target.transform.localScale = initialScale * scaleFactor;
    }

    public void OnRestartButtonClick()
    {
        scrollbar.value = (100f - minValue) / (maxValue - minValue);
        lastScaleFactor = 1f;
        target.transform.localScale = initialScale;
        PlayerPrefs.SetFloat(ScalePrefKey, lastScaleFactor);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat(ScalePrefKey, lastScaleFactor);
        PlayerPrefs.Save();
    }
}