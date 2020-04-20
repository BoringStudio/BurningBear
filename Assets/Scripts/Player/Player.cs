using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : Singleton<Player>
{
    public Attachable attachableObject
    {
        get => _interactController.attachPoint.attachedObject;
    }
    public AttachPoint attachPoint
    {
        get => _interactController.attachPoint;
    }

    public Camera playerCamera {
        get => _camera ?? GetComponentInChildren<Camera>();
    }

    public State state = State.Normal;

    private PlayerInteractController _interactController = null;
    private PlayerMovementController _movementController = null;

    private Camera _camera = null;
    private Animator _animator = null;

    public enum State
    {
        Normal,
        Build,
        Attach,
        WaitingToAttach,
        Upgrade,
    }

    void Awake()
    {
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        _animator = _animator ?? gameObject.GetComponentInChildren<Animator>();
        _interactController = _interactController ?? gameObject.GetComponentInChildren<PlayerInteractController>();
        _movementController = _movementController ?? gameObject.GetComponentInChildren<PlayerMovementController>();

        Assert.IsNotNull(_camera, "[Player]: Camera is null");
        Assert.IsNotNull(_animator, "[Player]: Animator is null");
        Assert.IsNotNull(_interactController, "[Player]: Interact controller is null");
        Assert.IsNotNull(_movementController, "[Player]: Movement controller is null");

        _animator.transform.rotation = Quaternion.Euler(GameSettings.Instance.cameraRotation);
        _camera.transform.rotation = Quaternion.Euler(GameSettings.Instance.cameraRotation);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToHand(Attachable target)
    {
        attachPoint.AttachObject(target);
        state = State.Attach;
    }
}
