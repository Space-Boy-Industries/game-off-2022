using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BulletSpawner : MonoBehaviour
{
    public abstract void OnStart();
    public abstract void OnUpdate();

    protected void SpawnBullet(Vector3 position, Vector3 up, float size, float speed, GameObject bulletPrefab) {
        var newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        newBullet.transform.up = up;
        newBullet.transform.localScale = new Vector3(size, size, size);
        var newBulletScript = newBullet.GetComponent<Bullet>();
        newBulletScript.hitBoxRadius = size/2;
        newBulletScript.speed = speed;
    }
}

public class CircleBulletSpawner : BulletSpawner
{
    public bool spawnBulletsOnStart = false;
    public int numberOfBullets = 10;
    public float bulletSize = 1f;
    public float bulletSpeed = 10f;
    public GameObject bulletPrefab;
    
    public UnityEvent doneSpawningBullets;
    
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
    
    private IEnumerator SpawnBulletCircleRoutine(Vector3 pos, int count, float size, float speed, GameObject prefab)
    {
        for (var i = 0; i < count; i++)
        {
            var degree = (i * 15) % 360;
            SpawnBullet(pos, Quaternion.Euler(0, 0, degree) * Vector3.up, size, speed, prefab);
            yield return new WaitForSeconds(0.075f);
        }
        
        doneSpawningBullets.Invoke();
        doneSpawningBullets.RemoveAllListeners();
        
        Destroy(gameObject);
    }
}