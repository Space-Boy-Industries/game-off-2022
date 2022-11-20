using System.Collections;
using UnityEngine;

public class DisableObjectOnPlatform : MonoBehaviour
{
    [SerializeField] private RuntimePlatform[] platforms;

    private void Awake()
    {
        if (((IList)platforms).Contains(Application.platform))
        {
            Disable();
        }
    }

    protected virtual void Disable()
    {
        gameObject.SetActive(false);
    }
}