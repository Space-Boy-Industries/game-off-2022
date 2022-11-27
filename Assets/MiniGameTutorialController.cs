using TMPro;
using UnityEngine;

public class MiniGameTutorialController : MonoBehaviour
{
    [SerializeField] private TMP_Text miniGameTitleText;
    [SerializeField] private TMP_Text miniGameObjectiveText;
    [SerializeField] private TMP_Text miniGameControlsText;

    public void ShowMiniGameTutorial(bool show)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    public void SetMiniGameData(MinigameController miniGameController)
    {
        SetTitleText(miniGameController.MiniGameName);
        SetObjectiveText(miniGameController.ObjectivePrompt);
        SetControlsText(miniGameController.ControlsPrompt);
    }

    private void SetObjectiveText(string objective)
    {
        miniGameObjectiveText.text = $"Objective:\n{objective}";
    }

    private void SetTitleText(string title)
    {
        miniGameTitleText.text = title;
    }
    
    private void SetControlsText(string[] controls)
    {
        miniGameControlsText.text = $"{string.Join("\n", controls)}";
    }
}
