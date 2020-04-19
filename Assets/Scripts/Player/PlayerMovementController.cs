using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 20.0f;

    private Player _player = null;
    private Rigidbody _rb = null;
    private Animator _animator = null;
    private SpriteRenderer _spriteRenderer = null;

    private InputController _inputController = null;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        //_spriteRenderer = _spriteRenderer ?? gameObject.GetComponentInChildren<SpriteRenderer>();
        _inputController = _inputController ?? gameObject.GetComponent<InputController>();
        _animator = _animator ?? gameObject.GetComponentInChildren<Animator>();
        _player = _player ?? gameObject.GetComponent<Player>();
        _rb = _rb ?? gameObject.GetComponentInChildren<Rigidbody>();

        //Assert.IsNotNull(_spriteRenderer, "[PlayerMovementController]: Sprite renderer is null");
        Assert.IsNotNull(_inputController, "[PlayerMovementController]: Input controller is null");
        Assert.IsNotNull(_animator, "[PlayerMovementController]: Animator is null");
        Assert.IsNotNull(_player, "[PlayerMovementController]: Player is null");
        Assert.IsNotNull(_rb, "[PlayerMovementController]: Rigidbody is null");
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _velocity.x = _inputController.horizontalAxis;
        _velocity.z = _inputController.verticalAxis;

        _velocity = _velocity.normalized * _maxSpeed;

        _animator.SetFloat("Speed", _velocity.magnitude);
        _animator.SetFloat("Vertical", _inputController.verticalAxis);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        Flip();
    }

    private void UpdateMovement()
    {
        if (_velocity.magnitude > 0)
        {
            Debug.Log(_velocity);
        }

        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    private void Flip()
    {
        if (_inputController.horizontalAxis > 0)
        {
            TurnToRight();
        }
        else if (_inputController.horizontalAxis < 0)
        {
            TurnToLeft();
        }
    }

    private void TurnToLeft()
    {
        //_spriteRenderer.flipX = true;
    }

    private void TurnToRight()
    {
        //_spriteRenderer.flipX = false;
    }
}
