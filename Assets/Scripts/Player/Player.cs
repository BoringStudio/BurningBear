using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    public Interactable mouseInteractableObject
    {
        get => _interactController.mouseInteractableObject;
        set => _interactController.mouseInteractableObject = value;
    }
    public Interactable interactableObject
    {
        get => _interactController.interactableObject;
        set => _interactController.interactableObject = value;
    }
    public InteractionPoint interactionPoint {
        get => _interactController.interactionPoint;
    }

    public Vector3 cameraRotation = new Vector3(60, 0, 0);
    public State state = State.Normal;

    private PlayerInteractController _interactController = null;
    private PlayerMovementController _movementController = null;

    private Camera _camera = null;

    public enum State
    {
        Normal,
        Build,
        Grab,
        Upgrade,
    }

    void Awake()
    {
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        _interactController = _interactController ?? gameObject.GetComponentInChildren<PlayerInteractController>();
        _movementController = _movementController ?? gameObject.GetComponentInChildren<PlayerMovementController>();

        Assert.IsNotNull(_camera, "[Player]: Camera is null");
        Assert.IsNotNull(_interactController, "[Player]: Interact controller is null");
        Assert.IsNotNull(_movementController, "[Player]: Movement controller is null");

        transform.GetChild(0).rotation = Quaternion.Euler(cameraRotation);
        _camera.transform.rotation = Quaternion.Euler(cameraRotation);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
