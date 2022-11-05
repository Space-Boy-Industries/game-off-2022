using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MinigameController))]
public class BulletHellGameController : MonoBehaviour
{
    //config
    public int maxSpawners = 3;
    public float minBulletSize = 0.75f;
    public float maxBulletSize = 1.25f;
    public float minBulletSpeed = 9f;
    public float maxBulletSpeed = 15f;
    public int minSpawnerBulletCount = 30;
    public int maxSpawnerBulletCount = 100;
    
    // prefabs and references
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    [FormerlySerializedAs("circleSpawn")] public GameObject circleSpawnerPrefab;

    // state
    private int _spawnerCount;

    // cached dependencies
    private GameObject _player;
    private MinigameController _minigameController;

    // cached camera bounds in world space
    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    private void Start()
    {
        _minigameController = GetComponent<MinigameController>();

        _minigameController.OnStart.AddListener(() =>
        {
            var mainCam = Camera.main;
            if (mainCam == null)
            {
                return;
            }
        
            _minBounds = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            _maxBounds = mainCam.ViewportToWorldPoint(new Vector3(1, 1, 0));
            
            // spawn colliders around camera bounds
            // left
            SpawnCollider(new Vector2(_minBounds.x, 0), new Vector3(0.5f, _maxBounds.y * 2, 1.0f));
            // right
            SpawnCollider(new Vector2(_maxBounds.x, 0), new Vector3(0.5f, _maxBounds.y * 2, 1.0f));
            // top
            SpawnCollider(new Vector2(0, _maxBounds.y), new Vector3(_maxBounds.x * 2, 0.5f, 1.0f));
            // bottom
            SpawnCollider(new Vector2(0, _minBounds.y), new Vector3(_maxBounds.x * 2, 0.5f, 1.0f));
            
            // spawn player
            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            _player.GetComponent<SimpleTopDownCharacterController>().onDeath.AddListener(OnPlayerDeath);
        });

        _minigameController.OnEnd.AddListener(() =>
        {
            _player.GetComponent<SimpleTopDownCharacterController>().onDeath.RemoveListener(OnPlayerDeath);
        });
    }

    private void SpawnCollider(Vector3 pos, Vector3 size)
    {
        var go = new GameObject("Collider")
        {
            transform =
            {
                position = pos
            }
        };
        
        var newCollider = go.AddComponent<BoxCollider>();
        newCollider.size = size;
    }

    private IEnumerator SpawnBulletCircleSpawner(Vector3 pos, int count, float size, float speed)
    {
        // increment spawner count
        _spawnerCount++;
        
        // spawn bullet spawner
        var circleSpawner = Instantiate(circleSpawnerPrefab, pos, Quaternion.identity);
        var circleSpawnerScript = circleSpawner.GetComponent<CircleBulletSpawner>();
        
        // give the players some time to react
        yield return new WaitForSeconds(1.0f);
        
        // start spawning bullets
        circleSpawnerScript.StartSpawning(count, size, speed, bulletPrefab);
        
        // if game over happens, stop spawning bullets
        _minigameController.OnEnd.AddListener(circleSpawnerScript.StopEarly);
        
        // when bullet spawner is finished, remove listener and decrement spawner count
        circleSpawnerScript.doneSpawningBullets.AddListener(() =>
        {
            _minigameController.OnEnd.RemoveListener(circleSpawnerScript.StopEarly);
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
        _minigameController.Fail();
    }

    private void Update()
    {
        if (_minigameController.State != MinigameState.Playing) return;

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
