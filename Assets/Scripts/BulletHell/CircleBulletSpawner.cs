using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CircleBulletSpawner : BulletSpawner
{
    public bool spawnBulletsOnStart;
    public int numberOfBullets = 10;
    public float bulletSize = 1f;
    public float bulletSpeed = 10f;
    public GameObject bulletPrefab;
    public UnityEvent doneSpawningBullets;

    private bool _stopEarly;
    
    public override void OnStart()
    {
        if (spawnBulletsOnStart)
        {
            StartSpawning(numberOfBullets, bulletSize, bulletSpeed, bulletPrefab);
        }
    }

    public override void OnUpdate()
    {
        
    }

    public void StartSpawning(int count, float size, float speed, GameObject prefab)
    {
        StartCoroutine(SpawnBulletCircleRoutine(transform.position, count, size, speed, prefab));
    }

    public void StopEarly()
    {
        _stopEarly = true;
    }

    private IEnumerator SpawnBulletCircleRoutine(Vector3 pos, int count, float size, float speed, GameObject prefab)
    {
        for (var i = 0; i < count; i++)
        {
            if (_stopEarly) break;
            
            var degree = (i * 15) % 360;
            SpawnBullet(pos, Quaternion.Euler(0, 0, degree) * Vector3.up, size, speed, prefab);
            yield return new WaitForSeconds(0.075f);
        }
        
        doneSpawningBullets.Invoke();
        doneSpawningBullets.RemoveAllListeners();
        
        Destroy(gameObject);
    }
}