using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BulletHellGameController : MonoBehaviour
{
    //config
    public float duration = 15f;
    public int maxSpawners = 3;
    public float minBulletSize = 0.75f;
    public float maxBulletSize = 1.25f;
    public float minBulletSpeed = 9f;
    public float maxBulletSpeed = 15f;
    public int minSpawnerBulletCount = 30;
    public int maxSpawnerBulletCount = 100;
    
    // prefabs and references
    public GameObject bulletPrefab;
    public GameObject circleSpawn;
    public TMP_Text timeText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    
    // events
    public UnityEvent onGameOver;

    // state
    private int _spawnerCount;
    private bool _gameOver;
    private float _startTime;
    
    // cached dependencies
    private GameObject _player;

    // cached camera bounds in world space
    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _player.GetComponent<SimpleTopDownCharacterController>().onDeath.AddListener(OnPlayerDeath);
        
        if (Camera.main == null)
        {
            return;
        }
        
        _minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        _maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        _startTime = Time.time;
    }

    private IEnumerator SpawnBulletCircleSpawner(Vector3 pos, int count, float size, float speed)
    {
        // increment spawner count
        _spawnerCount++;
        
        // spawn bullet spawner
        var circleSpawner = Instantiate(circleSpawn, pos, Quaternion.identity);
        var circleSpawnerScript = circleSpawner.GetComponent<CircleBulletSpawner>();
        
        // give the players some time to react
        yield return new WaitForSeconds(1.0f);
        
        // start spawning bullets
        circleSpawnerScript.StartSpawning(count, size, speed, bulletPrefab);
        
        // if game over happens, stop spawning bullets
        onGameOver.AddListener(circleSpawnerScript.StopEarly);
        
        // when bullet spawner is finished, remove listener and decrement spawner count
        circleSpawnerScript.doneSpawningBullets.AddListener(() =>
        {
            onGameOver.RemoveListener(circleSpawnerScript.StopEarly);
            _spawnerCount--;
        });
    }
    
    // private void SpawnFireAtPlayer(Vector3 pos, int count, float size, float speed)
    // {
    //     StartCoroutine(SpawnFireAtPlayerRoutine(pos, count, size, speed));
    // }
    //
    // private IEnumerator SpawnFireAtPlayerRoutine(Vector3 pos, int count, float size, float speed)
    // {
    //     for (var i = 0; i < count; i++)
    //     {
    //         var direction = (_player.transform.position - pos).normalized;
    //         SpawnBullet(pos, direction, size, speed);
    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }

    private void OnPlayerDeath()
    {
        _gameOver = true;
        gameOverPanel.SetActive(true);
        
        onGameOver.Invoke();
    }

    private void Update()
    {
        if (_gameOver) return;
        
        var timeLeft = duration - (Time.time - _startTime);
        
        if (timeLeft <= 0)
        {
            _gameOver = true;
            winPanel.SetActive(true);
            
            onGameOver.Invoke();
            
            // disable player hitbox
            _player.GetComponent<Collider>().enabled = false;
        }
        
        timeText.text = timeLeft.ToString("F2");
        
        if (!_player) return;
        if (_spawnerCount >= maxSpawners) return;
        
        // random chance of spawning a bullet
        if (Random.Range(0, 100) >= 1) return;
        
        // random position within the camera bounds. then place position outside the circle
        var position = new Vector3(Random.Range(_minBounds.x, _maxBounds.x), Random.Range(_minBounds.y, _maxBounds.y), 0);
        var size = Random.Range(minBulletSize, maxBulletSize);
        var speed = Random.Range(minBulletSpeed, maxBulletSpeed);
        
        StartCoroutine(SpawnBulletCircleSpawner(position, Random.Range(minSpawnerBulletCount, maxSpawnerBulletCount), size, speed));
    }
}
