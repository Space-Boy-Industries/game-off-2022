using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScrollingText : MonoBehaviour
{
    [SerializeField] private string text;
    
    private TMP_Text _tmpText;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var lineWidthText = _tmpText.renderedWidth / _tmpText.characterWidthAdjustment;
    }
}
