using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextProgressBar : MonoBehaviour
{
    [SerializeField] private int maxProgressBarWidth = 29;
    [SerializeField] private bool showPercentage = true;
    [SerializeField] private float timeToReachDesiredValue = 1.0f;
    
    private TMP_Text _text;
    private float _lastValue;
    private float _desiredValue = 0.0f;
    private float _currentValue = 0.0f;
    private float _timeValueSet;
    
    
    public float Value
    {
        get => _desiredValue;
        set
        {
            Debug.Log("Set Value");
            _lastValue = _desiredValue;
            _desiredValue = Mathf.Clamp(value, 0.0f, 1.0f);
            _timeValueSet = Time.time;
        }
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    private void UpdateProgressBar()
    {
        var filledWidth = (int) (_currentValue * maxProgressBarWidth);
        var emptyWidth = maxProgressBarWidth - filledWidth;
        _text.text = '[' + new string('=', filledWidth) + new string('-', emptyWidth) + ']';

        if (showPercentage)
        {
            _text.text += $" {_currentValue * 100:0}%";
        }
    }
    
    private void Update()
    {
        _currentValue = Mathf.Lerp(_lastValue, _desiredValue, (Time.time - _timeValueSet) / timeToReachDesiredValue);
        UpdateProgressBar();
    }
}
