using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Utility.TogglePause();
        }
        
        if (Utility.IsPaused != pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(Utility.IsPaused);

            if (Utility.IsPaused)
            {
                // select first menu button
                pauseMenu.GetComponentInChildren<Button>().Select();
            }
        }
    }

    public void OnExitToMenu()
    {
        Utility.PauseGame(false);
        SceneManager.LoadScene("Menu");
    }

    public void OnExitToDesktop()
    {
        Utility.PauseGame(false);
        Application.Quit();
    }

    public void OnResume()
    {
        Utility.PauseGame(false);
    }
}
