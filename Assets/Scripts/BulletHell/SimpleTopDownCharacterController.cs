using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
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
    
    // cached components
    private Rigidbody _rigidbody;
    private Camera _mainCamera;
    
    private void Start()
    {
        _input = new InputMap();
        _input.BulletHell.Enable();

        _velocity = new Vector2(0, 0);
        _health = maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // read input
        _moveInput = _input.BulletHell.Move.ReadValue<Vector2>();
        _mousePos = _input.BulletHell.MousePosition.ReadValue<Vector2>();
        _isPrecisionMode = _input.BulletHell.PrecisionMode.ReadValue<float>() > 0.5;
    }

    private void FixedUpdate()
    {
        // apply acceleration
        _velocity += _moveInput * (acceleration * Time.fixedDeltaTime);
        
        // clamp speed
        _velocity = Vector2.ClampMagnitude(
            _velocity,
            _isPrecisionMode ? maxPrecisionSpeed : maxSpeed
        );

        // apply drag
        _velocity *= 0.9f;
        
        // apply velocity
        var pos = transform.position;
        _rigidbody.MovePosition(pos + (Vector3)_velocity * Time.deltaTime);
        
        // rotate player towards mouse        
        // var direction = _mousePos - (Vector2)_mainCamera.WorldToScreenPoint(pos);
        // var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // _rigidbody.MoveRotation(Quaternion.AngleAxis(angle, Vector3.forward));
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