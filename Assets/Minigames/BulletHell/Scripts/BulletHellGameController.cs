using System.Collections;
using UnityEngine;
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
        
        _minigameController.OnReady.AddListener(() =>
        {
            // preload bullet prefabs
            SimplePool.Preload(bulletPrefab, 100, transform);
        });

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
            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, transform);
            _player.GetComponent<SimpleTopDownCharacterController>().onDeath.AddListener(OnPlayerDeath);
        });

        _minigameController.OnEnd.AddListener(() =>
        {
            // get all bulelts
            if (_player)
            {
                _player.GetComponent<SimpleTopDownCharacterController>().onDeath.RemoveListener(OnPlayerDeath);
            }
            
            // destroy all bullets
            var bullets = FindObjectsOfType<Bullet>();
            foreach (var bullet in bullets)
            {
                SimplePool.Despawn(bullet.gameObject);
            }
            SimplePool.DestroyAll();
        });
    }

    // helper function to spawn collider wall
    private void SpawnCollider(Vector3 pos, Vector3 size)
    {
        var go = new GameObject("Collider")
        {
            transform =
            {
                position = pos,
                parent = transform
            }
        };
        
        var newCollider = go.AddComponent<BoxCollider>();
        newCollider.size = size;
    }

    // helper function to spawn circle bullet spawner
    private void SpawnCircleBulletSpawner(Vector3 position, float size, float speed, int count)
    {
        // increment spawner count
        _spawnerCount++;
        
        // spawn bullet spawner
        var circleSpawner = Instantiate(circleSpawnerPrefab, position, Quaternion.identity, transform);
        var circleSpawnerScript = circleSpawner.GetComponent<CircleBulletSpawner>();
        
        // if game over happens, stop spawning bullets
        _minigameController.OnEnd.AddListener(circleSpawnerScript.StopEarly);
        
        // when bullet spawner is finished, remove listener and decrement spawner count
        circleSpawnerScript.doneSpawningBullets.AddListener(() =>
        {
            _minigameController.OnEnd.RemoveListener(circleSpawnerScript.StopEarly);
            _spawnerCount--;
        });
        
        // wait for 1 second to give the players some time to react
        StartCoroutine(Utility.CallbackAfter(0.5f, () =>
        {
            // start spawning bullets
            circleSpawnerScript.StartSpawning(count, size, speed, bulletPrefab);
        }));
    }
    
    // Event callback for when player dies
    private void OnPlayerDeath()
    {
        _minigameController.Fail();
    }

    private void Update()
    {
        if (_minigameController.State != MinigameState.Playing) return;
        if (!_player) return;
        if (_spawnerCount >= maxSpawners) return;

        // random position within the camera bounds. then place position outside the circle
        var position = new Vector3(Random.Range(_minBounds.x, _maxBounds.x), Random.Range(_minBounds.y, _maxBounds.y), 0);
        var size = Random.Range(minBulletSize, maxBulletSize);
        var speed = Random.Range(minBulletSpeed, maxBulletSpeed);
        var count = Random.Range(minSpawnerBulletCount, maxSpawnerBulletCount);
        
        SpawnCircleBulletSpawner(position, size, speed, count);
    }
}
