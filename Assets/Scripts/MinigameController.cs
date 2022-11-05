using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MinigameType
{
    Survival,
    Completion,
}

public enum MinigameState
{
    NotReady,
    Ready,
    Playing,
    Success,
    Failure,
}

public enum MinigameDifficulty
{
    Easy,
    Medium,
    Hard,
}

public class MinigameController : MonoBehaviour
{
    // duration of the objective & controls prompt display
    public const float PromptDuration = 2f;
    
    // the amount of time the player must survive in each difficulty
    public static readonly Dictionary<MinigameDifficulty, float> SurvivalTimes = new Dictionary<MinigameDifficulty, float>
    {
        { MinigameDifficulty.Easy, 5f },
        { MinigameDifficulty.Medium, 10f },
        { MinigameDifficulty.Hard, 15f },
    };

    // the amount of time the player has to complete the objective in each difficulty
    public static readonly Dictionary<MinigameDifficulty, float> CompletionTimes = new Dictionary<MinigameDifficulty, float>
    {
        { MinigameDifficulty.Easy, 20f },
        { MinigameDifficulty.Medium, 15f },
        { MinigameDifficulty.Hard, 10f },
    };

    public MinigameType Type;
    public string ObjectivePrompt;
    public string[] ControlsPrompt;

    public MinigameDifficulty Difficulty { get; set; }
    public UnityEvent OnReady { get; private set; }
    public UnityEvent OnStart { get; private set; }
    public UnityEvent OnEnd { get; private set; }
    public MinigameState State { get; private set; }
    public float TimeRemaining {
        get {
            return _duration - (Time.time - _startTime);
        }
    }

    private float _startTime;
    private float _duration;

    public static MinigameController Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            OnReady = new UnityEvent();
            OnStart = new UnityEvent();
            OnEnd = new UnityEvent();
            State = MinigameState.NotReady;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if(State == MinigameState.Playing && TimeRemaining <= 0f)
        {
            if(Type == MinigameType.Completion)
            {
                Fail();
            }
            else
            {
                Succeed();
            }
        }
    }

    public void Ready()
    {
        State = MinigameState.Ready;
        OnReady.Invoke();

        // TODO: Display objective and controls

        Utility.CallbackAfter(PromptDuration, () =>
        {
            _duration = Type == MinigameType.Survival ? SurvivalTimes[Difficulty] : CompletionTimes[Difficulty];
            _startTime = Time.time;

            State = MinigameState.Playing;
            OnStart.Invoke();
        });
    }

    public void Succeed()
    {
        State = MinigameState.Success;
        OnEnd.Invoke();
    }

    public void Fail()
    {
        State = MinigameState.Failure;
        OnEnd.Invoke();
    }
}
