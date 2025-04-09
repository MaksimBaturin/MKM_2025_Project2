using UnityEngine;

public class ScaleSaver : MonoBehaviour
{
    public Vector3 initialScale;
    void Start()
    {
        initialScale = transform.localScale;
    }
    
}
