using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DisableButtonOnPlatform : DisableObjectOnPlatform
{
    protected override void Disable()
    {
        GetComponent<Button>().interactable = false;
    }
}
