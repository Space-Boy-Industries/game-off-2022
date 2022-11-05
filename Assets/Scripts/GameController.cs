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

    void LoadNextMinigame()
    {
        var minigameScene = _currentLevel.MinigameScenes[_nextMinigameIndex];
        SceneManager.LoadScene(minigameScene, LoadSceneMode.Additive);

        _currentMinigame = FindObjectOfType<MinigameController>();
        _currentMinigame.Init();
        _currentMinigame.OnEnd.AddListener(() => {
            if(_currentMinigame.State == MinigameState.Success)
            {
                SceneManager.UnloadSceneAsync(minigameScene);
                _currentMinigame = null;
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
        // TODO: start cutscene

        // Load while cutscene is playing
        LoadNextMinigame();

        // For now I'm just using CallbackAfter to simulate the cutscene playing
        Utility.CallbackAfter(3f, () => {
            if (_nextMinigameIndex < _currentLevel.MinigameScenes.Length)
            {
                _currentMinigame.Ready();
            }
            else
            {
                // TODO: End the level
            }
        });
    }
}
