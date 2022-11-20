using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool isMainInstance;
    public int autoPlayLevel = -1;
    public GameData[] levels;
    public Camera transitionCamera;
    public int maxLife = 3;

    // TODO: replace this when real art exists
    public GameObject cutsceneObject;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text difficultyText;
    public TMP_Text lifeText;

    private GameData _currentLevel;
    private MinigameController _currentMinigame;
    private int _nextMinigameIndex;
    private int _lifeCount;

    public static GameController Instance;

    private void Awake()
    {
        // handle singleton stuff
        if (Instance)
        {
            if (isMainInstance)
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

    private void Start()
    {
        // initialize state
        _lifeCount = maxLife;
        lifeText.text = _lifeCount.ToString();

        // start first level
        if (autoPlayLevel >= 0)
        {
            StartLevel(autoPlayLevel);
        }
    }

    private void Update()
    {
        // update timer
        if (timerText.IsActive())
        {
            timerText.text = _currentMinigame.TimeRemaining.ToString("00");
        }
    }

    private void StartLevel(int index)
    {
        // initialize level state
        _currentLevel = levels[index];
        _nextMinigameIndex = 0;

        // Start Transition To MiniGame
        TransitionToNextMiniGame();
    }

    private void LoadNextMiniGame(Action<MinigameController> callback)
    {
        // disable the current minigame hud
        SetMiniGameHudActive(false);

        // Load while cutscene is playing
        StartCoroutine(LoadMiniGameRoutine(_nextMinigameIndex,
            (miniGame) =>
            {
                // init mini game

                // setup the mini game to run
                miniGame.OnReady.AddListener(() =>
                {
                    // disable cutscene camera and placeholder 
                    transitionCamera.enabled = false;
                    cutsceneObject.SetActive(false);

                    // enable mini game camera after cutscene
                    SetMiniGameCameraActive(true, _currentLevel.MinigameScenes[_nextMinigameIndex]);
                });

                // start the mini game
                miniGame.OnStart.AddListener(() =>
                {
                    // enable minigame hud after controls
                    SetMiniGameHudActive(true);
                });

                // on end, determine what feedback texts to show
                miniGame.OnEnd.AddListener(() =>
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

                // On done (after feedback text has display, determine how to go to the next state
                miniGame.OnDone.AddListener(() =>
                {
                    resultText.gameObject.SetActive(false);
                    SceneManager.UnloadSceneAsync(_currentLevel.MinigameScenes[_nextMinigameIndex]);

                    // if minigame was won, gp to next mini game
                    if (_currentMinigame.State == MinigameState.Success)
                    {
                        _currentMinigame = null;
                        _nextMinigameIndex++;
                        TransitionToNextMiniGame();
                    }
                    else
                    {
                        // if minigame was lost, subtract a life and determine what state is next
                        // TODO: Game over
                        // TODO: replace placeholder game over here
                        _lifeCount--;
                        lifeText.text = _lifeCount.ToString();

                        // if no lifes remaining, show level fail text and end level after short duration
                        if (_lifeCount == 0)
                        {
                            resultText.text = "Hack Failed";
                            resultText.gameObject.SetActive(true);

                            StartCoroutine(Utility.CallbackAfter(3f, () => { SceneManager.LoadScene("Menu"); }));
                        }
                        else
                        {
                            // if there are lives remaining, display fail minigame text and restart minigame after short duration

                            resultText.text = "Hack Failed, try again.";
                            resultText.gameObject.SetActive(true);

                            StartCoroutine(Utility.CallbackAfter(3f, () =>
                            {
                                _currentMinigame = null;
                                resultText.gameObject.SetActive(false);

                                // Load Minigame without transition because epic
                                LoadNextMiniGame(a => { a.Ready(); });
                            }));
                        }
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

                // set difficult text
                difficultyText.text = _currentMinigame.Difficulty.ToString();
                
                callback?.Invoke(miniGame);
            }
        ));
    }

    private IEnumerator LoadMiniGameRoutine(int index, Action<MinigameController> callback)
    {
        // load scene
        var minigameScene = _currentLevel.MinigameScenes[index];
        var sceneLoad = SceneManager.LoadSceneAsync(minigameScene, LoadSceneMode.Additive);

        // wait till scene has loaded
        while (!sceneLoad.isDone)
        {
            yield return null;
        }

        // disable mini game camera on minigame load and enable them on start
        SetMiniGameCameraActive(false, minigameScene);

        // find the current minigame object in the scene (there could be multiple for a small amount of time)
        var minigames = FindObjectsOfType<MinigameController>();
        foreach (var minigame in minigames)
        {
            if (minigame.gameObject.scene.name != minigameScene) continue;
            _currentMinigame = minigame;
            break;
        }

        // callback
        callback?.Invoke(_currentMinigame);
    }

    private void TransitionToNextMiniGame()
    {
        if (_nextMinigameIndex < _currentLevel.MinigameScenes.Length)
        {
            LoadNextMiniGame(null);

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
            StartCoroutine(Utility.CallbackAfter(3f, () => { SceneManager.LoadScene("Menu"); }));
        }
    }

    private void SetMiniGameCameraActive(bool active, string minigameScene)
    {
        var minigameCameras = FindObjectsOfType<Camera>();

        foreach (var c in minigameCameras)
        {
            if (c.gameObject.scene.name == minigameScene)
            {
                c.enabled = active;
            }
        }
    }

    private void SetMiniGameHudActive(bool active)
    {
        timerText.gameObject.SetActive(active);
        difficultyText.transform.parent.gameObject.SetActive(active);
        lifeText.transform.parent.gameObject.SetActive(active);
    }
}