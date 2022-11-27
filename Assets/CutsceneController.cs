using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private Animator cutsceneCameraAnimator;
    [SerializeField] private GameObject[] cutsceneObjects;
    [SerializeField] private float timeBetweenZoomOutAndIn = 2f;
    [SerializeField] private float timeBeforeUpdateProgress = 0.8f;

    [SerializeField] private GameObject tempHackerText;
    [SerializeField] private TextProgressBar hackerTextProgressBar;

    private float _currentProgress;

    private void Start()
    {
        hackerTextProgressBar.Value = 0.0f;
    }

    public void EnableCutsceneObjects()
    {
        foreach (GameObject obj in cutsceneObjects)
        {
            obj.SetActive(true);
        }
    }

    public void DisableCutsceneObjects()
    {
        foreach (GameObject obj in cutsceneObjects)
        {
            obj.SetActive(false);
        }
    }

    public void SetProgressPercentage(float progressPercentage)
    {
        _currentProgress = progressPercentage;
    }

    private static IEnumerator PlayAndWaitForAnimation(Animator anim, string stateName, Action callback)
    {
        anim.Play(stateName);

        yield return null;

        while (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        callback?.Invoke();
    }

    private void PlayZoomInAnimation(Action callback)
    {
        // TODO: replace hacker text with actual hack progress animation thing
        tempHackerText.SetActive(false);
        StartCoroutine(PlayAndWaitForAnimation(cutsceneCameraAnimator, "Zoom In", () => { callback?.Invoke(); }));
    }

    private void PlayZoomOutAnimation(Action callback)
    {
        tempHackerText.SetActive(false);
        StartCoroutine(PlayAndWaitForAnimation(cutsceneCameraAnimator, "Zoom Out", () =>
        {
            // TODO: replace hacker text with actual hack progress animation thing
            tempHackerText.SetActive(true);

            callback?.Invoke();
        }));
    }

    public void StartTransition(Action callback)
    {
        EnableCutsceneObjects();
        PlayZoomOutAnimation(() =>
        {
            StartCoroutine(Utility.CallbackAfter(timeBeforeUpdateProgress,
                    () => { hackerTextProgressBar.Value = _currentProgress; }
                )
            );
            
            StartCoroutine(Utility.CallbackAfter(timeBetweenZoomOutAndIn, () =>
            {
                PlayZoomInAnimation(() => { callback?.Invoke(); });
            }));
        });
    }
}