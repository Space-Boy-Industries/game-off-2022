using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(MinigameController))]
public class ButtonMashingGameController : MonoBehaviour
{
    public int scoreRequirement = 100;
    public GameObject[] enableOnStart;
    public CameraShake cameraShake;

    public Image spacebarImage;

    // state
    private int _score;
    
    // cached components
    public ProgressBar[] progressBars;
    private MinigameController _minigameController;
    private InputMap _inputMap;
    
    private void Start()
    {
        _inputMap = new InputMap();
        _minigameController = GetComponent<MinigameController>();
        
        _minigameController.OnStart.AddListener(OnMinigameStart);
        _minigameController.OnEnd.AddListener(OnMinigameEnd);
    }

    private void OnMinigameStart()
    {
        _inputMap.ButtonMasher.Enable();
        _inputMap.ButtonMasher.MashButton.Enable();
        _inputMap.ButtonMasher.MashButton.performed += OnMashButton;
        
        foreach (var obj in enableOnStart)
        {
            obj.SetActive(true);
        }
    }
    
    private void OnMinigameEnd()
    {
        _inputMap.ButtonMasher.Disable();
        _inputMap.ButtonMasher.MashButton.performed -= OnMashButton;
    }
    
    private void OnMashButton(InputAction.CallbackContext context)
    {
        if (Utility.IsPaused) return;
        
        _score++;
        _score = Mathf.Clamp(_score, 0, scoreRequirement);
        
        cameraShake.shakeDuration += 0.1f;
        cameraShake.shakeDuration = Mathf.Clamp(cameraShake.shakeDuration, 0, 1);

        foreach (var bar in progressBars)
        {
            bar.percentFilled = (float)_score / scoreRequirement;
        }

        if (_score >= scoreRequirement)
        {
            _minigameController.Succeed();
        }
    }

    private void Update()
    {
        if (Utility.IsPaused) return;

        spacebarImage.color = _inputMap.ButtonMasher.MashButton.ReadValue<float>() > 0 ? Color.gray : Color.white;
    }
}
