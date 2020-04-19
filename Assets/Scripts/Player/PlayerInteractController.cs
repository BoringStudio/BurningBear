using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteractController : MonoBehaviour
{
    public Interactable mouseInteractableObject = null;
    public Interactable interactableObject = null;
    public InteractionPoint interactionPoint = null;

    public Transform mouseOriginTransform = null;
    public float mouseInteractionRadius = 5f;

    private Player _player = null;
    private InputController _inputController = null;
    private Camera _camera = null;

    void Awake()
    {
        _player = _player ?? gameObject.GetComponentInChildren<Player>();
        _inputController = _inputController ?? gameObject.GetComponentInChildren<InputController>();
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        interactionPoint = interactionPoint ?? gameObject.GetComponentInChildren<InteractionPoint>();

        Assert.IsNotNull(_player, "[PlayerInteractByMouse]: Player is null");
        Assert.IsNotNull(_inputController, "[PlayerInteractByMouse]: Input controller is null");
        Assert.IsNotNull(_camera, "[PlayerInteractByMouse]: Camera is null");
        Assert.IsNotNull(interactionPoint, "[PlayerInteractByMouse]: Interaction point is null");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseInteract();
    }

    void UpdateMouseInteract()
    {
        if (_inputController.interactButtonDown)
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(_inputController.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var hittedGameObject = hit.collider.gameObject;
                var hittedTag = hittedGameObject.tag;
                var hitPosition = hit.point;
                var distance = (hit.transform.position - mouseOriginTransform.position).magnitude;

                Debug.Log("Distance to interacted object: " + distance);
                if (distance <= mouseInteractionRadius && hittedTag != "Player")
                {
                    Debug.Log("Hitted object tag: " + hittedTag);
                    Debug.Log("Hitted point position: " + hitPosition);

                    mouseInteractableObject = mouseInteractableObject ?? hittedGameObject.GetComponentInChildren<Interactable>();
                    mouseInteractableObject?.OnMouseInteractStart(_player, hitPosition);
                }
            }
        }

        if (_inputController.interactButtonUp)
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(_inputController.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var hitPosition = hit.point;
                mouseInteractableObject?.OnMouseInteractEnd(_player, hitPosition);
            }
        }
    }
}
