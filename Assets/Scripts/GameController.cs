using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool IsMainInstance;
    public int AutoPlayLevel = -1;
    public GameData[] Levels;

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
        while(!sceneLoad.isDone)
        {
            yield return null;
        }

        var minigames = FindObjectsOfType<MinigameController>();
        foreach(var minigame in minigames)
        {
            if (minigame.gameObject.scene.name == minigameScene)
            {
                _currentMinigame = minigame;
                break;
            }
        }

        _currentMinigame.OnEnd.AddListener(() => {
            if(_currentMinigame.State == MinigameState.Success)
            {
                _currentMinigame = null;
                SceneManager.UnloadSceneAsync(minigameScene);
                TransitionCutscene();
            }
            else
            {
                // TODO: Game over
            }
        });

        // increase difficulty after a certain number of minigames
        foreach (var levelsPlayed in _currentLevel.DifficultyUpAfter)
        {
            if (_nextMinigameIndex == levelsPlayed && _currentMinigame.Difficulty < MinigameDifficulty.Hard)
            {
                _currentMinigame.Difficulty++;
            }
        }

        _nextMinigameIndex++;
    }

    void TransitionCutscene()
    {
        // Load while cutscene is playing
        StartCoroutine(LoadNextMinigame());

        // TODO: start cutscene

        // For now I'm just using CallbackAfter to simulate the cutscene playing
        StartCoroutine(Utility.CallbackAfter(3f, () => {
            if (_nextMinigameIndex < _currentLevel.MinigameScenes.Length)
            {
                _currentMinigame.Ready();
            }
            else
            {
                // TODO: End the level
            }
        }));
    }
}
