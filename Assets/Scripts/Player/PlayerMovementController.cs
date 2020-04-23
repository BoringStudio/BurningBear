using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 20.0f;

    [SerializeField] private InputController _inputController = null;

    private Player _player = null;
    private PlayerBodyChanger _bodyChanger = null;

    private Animator _animator = null;
    private Rigidbody _rigidBody = null;

    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _player = _player ?? GetComponent<Player>();
        _bodyChanger = _bodyChanger ?? GetComponentInChildren<PlayerBodyChanger>();

        _animator = _animator ?? GetComponentInChildren<Animator>();
        _rigidBody = _rigidBody ?? GetComponentInChildren<Rigidbody>();

        Assert.IsNotNull(_player, "[PlayerMovementController]: Player is null");
        Assert.IsNotNull(_bodyChanger, "[PlayerMovementController]: Body changer is null");
        Assert.IsNotNull(_inputController, "[PlayerMovementController]: Input controller is null");
        Assert.IsNotNull(_animator, "[PlayerMovementController]: Animator is null");
        Assert.IsNotNull(_rigidBody, "[PlayerMovementController]: Rigidbody is null");
    }

    private void Update()
    {
        _velocity.x = _inputController.horizontalAxis;
        _velocity.z = _inputController.verticalAxis;

        _velocity = _velocity.normalized * _maxSpeed;

        _animator.SetFloat("Speed", _velocity.magnitude);
        _animator.SetFloat("Vertical", _inputController.verticalAxis);
        _animator.SetFloat("HandsLevel", _player.attachPoint.hasAttachedObject ? 1.0f : 0.0f);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateDirection();
    }

    private void UpdateMovement()
    {
        _rigidBody.MovePosition(_rigidBody.position + _velocity * Time.fixedDeltaTime);
        if (_rigidBody.velocity.magnitude < .01)
        {
            _rigidBody.velocity = Vector3.zero;
        }
    }

    private void UpdateDirection()
    {
        if (_inputController.horizontalAxis > 0)
        {
            TurnToRight();
        }
        else if (_inputController.horizontalAxis < 0)
        {
            TurnToLeft();
        }


        if (_inputController.verticalAxis > 0)
        {
            TurnToUp();
        }
        else if (_inputController.verticalAxis <= 0)
        {
            TurnToDown();
        }
    }

    private void TurnToLeft()
    {
        _animator.transform.localScale = new Vector3(
            Mathf.Abs(_animator.transform.localScale.x),
            _animator.transform.localScale.y,
            _animator.transform.localScale.z);
    }

    private void TurnToRight()
    {
        _animator.transform.localScale = new Vector3(
            Mathf.Abs(_animator.transform.localScale.x) * -1,
            _animator.transform.localScale.y,
            _animator.transform.localScale.z);
    }

    private void TurnToUp()
    {
        _bodyChanger.ToUp();
    }

    private void TurnToDown()
    {
        _bodyChanger.ToDown();
    }
}
