using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static IEnumerator CallbackAfter(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
