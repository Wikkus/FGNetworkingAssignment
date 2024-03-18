using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour, IPlayerActions
{
    private PlayerInput _playerInput;
    private Vector2 _moveInput = new();
    private Vector2 _cursorLocation;

    private Transform _shipTransform;
    private Rigidbody2D _rb;

    private Transform _turretPivotTransform;
    private bool _isDead = true;
    private bool _canShoot = true;

    private int _ammoAmount = 0;
    private int _maxAmmo = 10;

    private float _shootCooldown = 0.5f;

    public UnityAction<bool> onFireEvent;
    public int AmmoAmount { get { return _ammoAmount; } set { _ammoAmount = value; } }
    public int MaxAmmo { get { return _maxAmmo; } set { _maxAmmo = value; } }

    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public bool GetCanShoot { get { return _canShoot; }}

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float shipRotationSpeed = 100f;
    [SerializeField] private float turretRotationSpeed = 4f;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (_playerInput == null)
        {
            _playerInput = new();
            _playerInput.Player.SetCallbacks(this);
        }
        _playerInput.Player.Enable();
        _isDead = false;
        _rb = GetComponent<Rigidbody2D>();

        _ammoAmount = _maxAmmo;
        
        _shipTransform = transform;
        _turretPivotTransform = transform.Find("PivotTurret");
        if (_turretPivotTransform == null) Debug.LogError("PivotTurret is not found", gameObject);
    }

    public void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(_isDead) return;
        
        //3. Limited Ammo and 5. Shot Timer
        if (!_canShoot || _ammoAmount <= 0) return;
        if (context.performed)
        {
            _canShoot = false;
            Invoke(nameof(ReadyToShoot), _shootCooldown);
            onFireEvent.Invoke(true);            
            _ammoAmount -= 1;
        }
        else if (context.canceled)
        {
            onFireEvent.Invoke(false);
        }
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_isDead) 
        {
            _moveInput = new();
            return; 
        }
        _moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (_isDead) return;
        _rb.velocity = transform.up * _moveInput.y * movementSpeed;
        _rb.MoveRotation(_rb.rotation + _moveInput.x * -shipRotationSpeed * Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        if (!IsOwner) return;
        if (_isDead) return;
        Vector2 screenToWorldPosition = Camera.main.ScreenToWorldPoint(_cursorLocation);
        Vector2 targetDirection = new Vector2(screenToWorldPosition.x - _turretPivotTransform.position.x, screenToWorldPosition.y - _turretPivotTransform.position.y).normalized;
        Vector2 currentDirection = Vector2.Lerp(_turretPivotTransform.up, targetDirection, Time.deltaTime * turretRotationSpeed);
        _turretPivotTransform.up = currentDirection;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (_isDead) return;
        _cursorLocation = context.ReadValue<Vector2>();
    }

    public void TeleportPlayer(Vector3 location)
    {
        gameObject.GetComponent<NetworkTransform>().Teleport(location, Quaternion.identity, new Vector3(1, 1, 1));
    }

    public void ReadyToShoot()
    {
        _canShoot = true;
    }
}
