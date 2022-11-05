using DefaultNamespace;
using UnityEngine;

public enum Direction
{
    Up,
    Forward
}

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask;
    public int damage = 10;
    public float speed = 10;
    public float lifeTime = 3;
    public Direction direction = Direction.Forward;
    public float hitBoxRadius = 0.25f;

    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _startTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Check for collision
        var moveDistance = speed * Time.deltaTime;
        var moveDirection = direction == Direction.Forward ? transform.forward : transform.up;

        var ray = new Ray(transform.position, moveDirection);

        if (Physics.SphereCast(ray, hitBoxRadius, out var hit, moveDistance, collisionMask))
        {
            var shootable = hit.collider.gameObject.GetComponent<IShootable>();
            shootable?.OnShot(this);
            Destroy(gameObject);
        }

        transform.position += moveDirection * moveDistance;
    }
}