using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Range(0, 1.0f)]
    public float percentFilled;
    
    [SerializeField] private Image backFill;
    private RectTransform _parent;
    
    private void Start()
    {
        _parent = GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        var targetWidth = _parent.sizeDelta.x * percentFilled;
        // var smoothedWidth = Mathf.Lerp(backFill.rectTransform.sizeDelta.x, targetWidth, 0.03f);
        
        backFill.rectTransform.sizeDelta = new Vector2(targetWidth, backFill.rectTransform.sizeDelta.y);
    }
}
