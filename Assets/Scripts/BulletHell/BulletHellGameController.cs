using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletHellGameController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject circleSpawn;
    public float duration = 15f;
    public int maxSpawners = 3;

    public TMP_Text timeText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private GameObject _player;
    private int _spawnerCount;
    private bool _gameOver;
    private float _startTime;
    
    // camera bounds
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

    private void SpawnBullet(Vector3 position, Vector3 up, float size, float speed)
    {
        var newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        newBullet.transform.up = up;
        newBullet.transform.localScale = new Vector3(size, size, size);
        var newBulletScript = newBullet.GetComponent<Bullet>();
        newBulletScript.hitBoxRadius = size/2;
        newBulletScript.speed = speed;
    }

    private IEnumerator SpawnBulletCircleSpawner(Vector3 pos, int count, float size, float speed)
    {
        _spawnerCount++;
        var circleSpawner = Instantiate(circleSpawn, pos, Quaternion.identity);
        var circleSpawnerScript = circleSpawner.GetComponent<CircleBulletSpawner>();
        yield return new WaitForSeconds(1.0f);
        circleSpawnerScript.StartSpawning(count, size, speed, bulletPrefab);
        circleSpawnerScript.doneSpawningBullets.AddListener(() =>
        {
            _spawnerCount--;
        });
    }
    
    private void SpawnFireAtPlayer(Vector3 pos, int count, float size, float speed)
    {
        StartCoroutine(SpawnFireAtPlayerRoutine(pos, count, size, speed));
    }
    
    private IEnumerator SpawnFireAtPlayerRoutine(Vector3 pos, int count, float size, float speed)
    {
        for (var i = 0; i < count; i++)
        {
            var direction = (_player.transform.position - pos).normalized;
            SpawnBullet(pos, direction, size, speed);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnPlayerDeath()
    {
        _gameOver = true;
        gameOverPanel.SetActive(true);
    }

    private void Update()
    {
        if (_gameOver) return;
        
        var timeLeft = duration - (Time.time - _startTime);
        
        if (timeLeft <= 0)
        {
            _gameOver = true;
            winPanel.SetActive(true);
            
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
        var size = Random.Range(0.75f, 1.25f);
        var speed = 10 + Random.Range(1f, 5f);
        
        StartCoroutine(SpawnBulletCircleSpawner(position, Random.Range(30, 100), size, speed));
    }
}
