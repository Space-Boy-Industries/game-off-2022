using UnityEngine;

public abstract class BulletSpawner : MonoBehaviour
{
    public abstract void OnStart();
    public abstract void OnUpdate();

    protected void SpawnBullet(Vector3 position, Vector3 up, float size, float speed, GameObject bulletPrefab)
    {
        var newBullet = SimplePool.Spawn(bulletPrefab, position, Quaternion.identity);
        newBullet.transform.up = up;
        newBullet.transform.localScale = new Vector3(size, size, size);
        var newBulletScript = newBullet.GetComponent<Bullet>();
        newBulletScript.SetStarTime();
        newBulletScript.hitBoxRadius = size/2;
        newBulletScript.speed = speed;
    }
}