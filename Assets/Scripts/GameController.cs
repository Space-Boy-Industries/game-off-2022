using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool IsMainInstance;
    public int AutoPlayLevel = -1;
    public GameData[] Levels;
    public Camera transitionCamera;
    
    // TODO: replace this when real art exists
    public GameObject cutsceneObject;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text difficultyText;

    private GameData _currentLevel;
    private MinigameController _currentMinigame;
    private int _nextMinigameIndex;

    public static GameController Instance;

    void Awake()
    {
        if (Instance)
        {
            if (IsMainInstance)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (AutoPlayLevel >= 0)
        {
            StartLevel(AutoPlayLevel);
        }
    }

    void Update()
    {
        if (timerText.IsActive())
        {
            timerText.text = _currentMinigame.TimeRemaining.ToString("00");
        }
    }

    void StartLevel(int index)
    {
        _currentLevel = Levels[index];
        _nextMinigameIndex = 0;
        TransitionCutscene();
    }

    IEnumerator LoadNextMinigame()
    {
        var minigameScene = _currentLevel.MinigameScenes[_nextMinigameIndex];
        var sceneLoad = SceneManager.LoadSceneAsync(minigameScene, LoadSceneMode.Additive);
        while (!sceneLoad.isDone)
        {
            yield return null;
        }

        // disable mini game camera on minigame load
        SetMiniGameCameraActive(false, minigameScene);

        var minigames = FindObjectsOfType<MinigameController>();
        foreach (var minigame in minigames)
        {
            if (minigame.gameObject.scene.name == minigameScene)
            {
                _currentMinigame = minigame;
                break;
            }
        }
        
        _currentMinigame.OnReady.AddListener(() =>
        {
            // disable cutscene camera and placeholder 
            transitionCamera.enabled = false;
            cutsceneObject.SetActive(false);
            
            // enable mini game camera after cutscene
            SetMiniGameCameraActive(true, _currentLevel.MinigameScenes[_nextMinigameIndex]);
        });
        
        _currentMinigame.OnStart.AddListener(() =>
        {
            // enable minigame hud after controls
            SetMiniGameHudActive(true);
        });
        
        _currentMinigame.OnEnd.AddListener(() =>
        {
            // hide timer at end of game
            SetMiniGameHudActive(false);
            
            resultText.gameObject.SetActive(true);
            // display win or lose
            switch (_currentMinigame.State)
            {
                case MinigameState.Failure:
                    resultText.text = "You lose";
                    break;
                case MinigameState.Success:
                    resultText.text = "You win";
                    break;
                case MinigameState.NotReady:
                case MinigameState.Ready:
                case MinigameState.Playing:
                default:
                    resultText.gameObject.SetActive(false);
                    throw new ArgumentOutOfRangeException();
            }
        });

        _currentMinigame.OnDone.AddListener(() =>
        {
            resultText.gameObject.SetActive(false);
            
            if (_currentMinigame.State == MinigameState.Success)
            {
                _currentMinigame = null;
                SceneManager.UnloadSceneAsync(minigameScene);
                _nextMinigameIndex++;
                TransitionCutscene();
            }
            else
            {
                // TODO: Game over
                
                // TODO: replace placeholder game over here
                resultText.text = "Hack Failed";
                resultText.gameObject.SetActive(true);
                
                StartCoroutine(Utility.CallbackAfter(3f, () =>
                {
                    SceneManager.LoadScene("Menu");
                }));
            }
        });

        // increase difficulty after a certain number of minigames
        foreach (var levelsPlayed in _currentLevel.DifficultyUpAfter)
        {
            if (_nextMinigameIndex >= levelsPlayed && _currentMinigame.Difficulty < MinigameDifficulty.Hard)
            {
                _currentMinigame.Difficulty++;
            }
        }
        
        difficultyText.text = _currentMinigame.Difficulty.ToString();
    }

    private void SetMiniGameCameraActive(bool active, string minigameScene)
    {
        var minigameCameras = FindObjectsOfType<Camera>();

        foreach (var camera in minigameCameras)
        {
            if (camera.gameObject.scene.name == minigameScene)
            {
                camera.enabled = active;
            }
        }
    }

    private void SetMiniGameHudActive(bool active)
    {
        timerText.gameObject.SetActive(active);
        difficultyText.transform.parent.gameObject.SetActive(active);
    } 

    void TransitionCutscene()
    {
        if (_nextMinigameIndex < _currentLevel.MinigameScenes.Length)
        {
            SetMiniGameHudActive(false);
            
            // Load while cutscene is playing
            StartCoroutine(LoadNextMinigame());

            // TODO: start cutscene
            transitionCamera.enabled = true;
            cutsceneObject.SetActive(true);

            // For now I'm just using CallbackAfter to simulate the cutscene playing
            StartCoroutine(Utility.CallbackAfter(3f, () =>
                {
                    // start mini game
                    _currentMinigame.Ready();
                })
            );
        }
        else
        {
            // TODO: End the level for (real)
            
            // TODO: replace placeholder level here
            resultText.text = "Hack Complete";
            resultText.gameObject.SetActive(true);
            StartCoroutine(Utility.CallbackAfter(3f, () =>
            {
                SceneManager.LoadScene("Menu");
            }));
        }
    }
}