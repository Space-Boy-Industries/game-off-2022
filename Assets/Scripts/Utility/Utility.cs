using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static bool IsPaused => Time.timeScale == 0;
    
    public static IEnumerator CallbackAfter(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }

    public static void TogglePause()
    {
        Time.timeScale = IsPaused ? 1 : 0;
    }
    
    public static void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
