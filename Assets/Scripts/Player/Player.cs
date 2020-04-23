using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    public Attachable attachableObject
    {
        get => _interactController.attachPoint.attachedObject;
    }
    public AttachPoint attachPoint
    {
        get => _interactController.attachPoint;
    }

    public Camera playerCamera
    {
        get; private set;
    } = null;

    public State state = State.Normal;

    private PlayerInteractController _interactController = null;
    private PlayerMovementController _movementController = null;

    private Animator _animator = null;

    [SerializeField] private GameSettings _gameSettings = null;

    public enum State
    {
        Normal,
        Build,
        Attach,
        WaitingToAttach,
    }

    void Awake()
    {
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        _animator = _animator ?? gameObject.GetComponentInChildren<Animator>();
        _interactController = _interactController ?? gameObject.GetComponentInChildren<PlayerInteractController>();
        _movementController = _movementController ?? gameObject.GetComponentInChildren<PlayerMovementController>();

        Assert.IsNotNull(playerCamera, "[Player]: Camera is null");
        Assert.IsNotNull(_animator, "[Player]: Animator is null");
        Assert.IsNotNull(_interactController, "[Player]: Interact controller is null");
        Assert.IsNotNull(_movementController, "[Player]: Movement controller is null");
        Assert.IsNotNull(_gameSettings, "[Player]: Game settings is null");

        playerCamera.transform.rotation = Quaternion.Euler(_gameSettings.cameraRotation);

        _animator.transform.rotation = Quaternion.Euler(_gameSettings.cameraRotation);
    }

    public void AddToHand(Attachable target)
    {
        attachPoint.AttachObject(target);
        state = State.Attach;
    }

    public void StartBuild()
    {
        state = State.Build;
    }
}
