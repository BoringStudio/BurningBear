using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 20.0f;

    private Rigidbody _rb;
    private Animator _animator;
    private Transform _playerTransform;
    private SpriteRenderer _spriteRenderer;
    private Transform _spriteTransform;
    private Camera _camera;
    private Transform _cameraTransform;

    private Axes _axes = null;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _axes = gameObject.AddComponent<Axes>();

        if (_playerTransform == null)
        {
            _playerTransform = gameObject.GetComponent<Transform>();
        }

        if (_camera == null)
        {
            _camera = gameObject.GetComponentInChildren<Camera>();
        }

        if (_cameraTransform == null)
        {
            _cameraTransform = _camera?.gameObject.transform;
        }

        if (_spriteRenderer == null)
        {
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        if (_spriteTransform == null)
        {
            _spriteTransform = _spriteRenderer?.gameObject.transform;
        }

        if (_animator == null)
        {
            _animator = _spriteRenderer?.gameObject.GetComponent<Animator>();
        }

        if (_rb == null)
        {
            _rb = gameObject.GetComponentInChildren<Rigidbody>();
        }

        Assert.IsNotNull(_spriteRenderer, "[PlayerController]: Sprite renderer is null");
        Assert.IsNotNull(_spriteTransform, "[PlayerController]: Sprite transform is null");
        Assert.IsNotNull(_playerTransform, "[PlayerController]: Player transform is null");
        Assert.IsNotNull(_cameraTransform, "[PlayerController]: Camera transform is null");
        Assert.IsNotNull(_camera, "[PlayerController]: Camera is null");
        Assert.IsNotNull(_axes, "[PlayerController]: Axes is null");
        Assert.IsNotNull(_rb, "[PlayerController]: Rigidbody is null");
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _velocity.x = _axes.Horizontal;
        _velocity.z = _axes.Vertical;

        _velocity *= _maxSpeed;

        _animator.SetFloat("Speed", _velocity.magnitude);
        _animator.SetFloat("Horizontal", _axes.Horizontal);
        _animator.SetFloat("Vertical", _axes.Vertical);


    }

    private void FixedUpdate()
    {
        UpdateMovement();
        Flip();
    }

    private void UpdateMovement()
    {
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    private void Flip()
    {
        if (_axes.Horizontal > 0) {
            TurnToRight();
        } else if (_axes.Horizontal < 0) {
            TurnToLeft();
        }
    }

    private void TurnToLeft()
    {
        _spriteRenderer.flipX = true;
    }

    private void TurnToRight()
    {
        _spriteRenderer.flipX = false;
    }
}
