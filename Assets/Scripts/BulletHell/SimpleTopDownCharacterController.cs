using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTopDownCharacterController : MonoBehaviour, IShootable
{
    // player configuration
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float maxPrecisionSpeed = 2f;
    public int maxHealth = 100;
    
    // events
    public UnityEvent onDeath;
    
    // player state
    private Vector2 _velocity;
    private int _health;
    
    // input state
    private InputMap _input;
    private Vector2 _moveInput;
    private Vector2 _mousePos;
    private bool _isPrecisionMode;
    
    private void Start()
    {
        _input = new InputMap();
        _input.BulletHell.Enable();

        _velocity = new Vector2(0, 0);
        _health = maxHealth;
    }

    private void Update()
    {
        _moveInput = _input.BulletHell.Move.ReadValue<Vector2>();
        _mousePos = _input.BulletHell.MousePosition.ReadValue<Vector2>();
        _isPrecisionMode = _input.BulletHell.PrecisionMode.ReadValue<float>() > 0.5;
    }

    private void FixedUpdate()
    {
        _velocity += _moveInput * (acceleration * Time.fixedDeltaTime);
        _velocity = Vector2.ClampMagnitude(
            _velocity,
            _isPrecisionMode ? maxPrecisionSpeed : maxSpeed
        );

        transform.position += (Vector3)_velocity * Time.fixedDeltaTime;

        if (_velocity.magnitude > 0.01f)
        {
            transform.position += (Vector3)_velocity * Time.fixedDeltaTime;
        }

        _velocity *= 0.9f;

        var direction = _mousePos - (Vector2)Camera.main!.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Heal(int amount)
    {
        _health = Mathf.Min(_health + amount, maxHealth);
    }
    
    public void Damage(int amount)
    {
        _health = Mathf.Max(_health - amount, 0);

        if (_health > 0) return;
        
        onDeath.Invoke();
        onDeath.RemoveAllListeners();
        Destroy(gameObject);
    }

    public void OnShot(Bullet bullet)
    {
        Damage(bullet.damage);
    }
}