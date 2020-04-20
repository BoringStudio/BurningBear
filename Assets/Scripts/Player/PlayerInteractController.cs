using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteractController : MonoBehaviour
{
    public AttachPoint attachPoint = null;


    public Transform mouseOriginInteractionPoint = null;
    public float mouseInteractionRadius = 5f;


    private Player _player = null;
    private InputController _inputController = null;
    private Camera _camera = null;
    private Inferno _inferno = null;


    private const string flameTag = "Flame";
    private const string groundTag = "Ground";
    private const string attachableTag = "Attachable";
    private const string coalSourceTag = "CoalSource";

    void Awake()
    {
        _inferno = _inferno ?? Inferno.Instance;
        _player = _player ?? Player.Instance;
        _inputController = _inputController ?? gameObject.GetComponentInChildren<InputController>();
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        attachPoint = attachPoint ?? gameObject.GetComponentInChildren<AttachPoint>();
        mouseOriginInteractionPoint = mouseOriginInteractionPoint ?? transform;

        Assert.IsNotNull(_player, "[PlayerInteractController]: Player is null");
        Assert.IsNotNull(_inputController, "[PlayerInteractController]: Input controller is null");
        Assert.IsNotNull(_camera, "[PlayerInteractController]: Camera is null");
        Assert.IsNotNull(attachPoint, "[PlayerInteractController]: Attach point is null");
        Assert.IsNotNull(mouseOriginInteractionPoint, "[PlayerInteractController]: Mouse origin interaction point is null");
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
                var distance = (hitPosition - mouseOriginInteractionPoint.position).magnitude;

                Debug.Log("Distance to interacted object: " + distance);
                Debug.Log("Hitted object tag: " + hittedTag);
                Debug.Log("Hitted point position: " + hitPosition);
                if (distance <= mouseInteractionRadius && hittedTag != "Player")
                {
                    switch (_player.state)
                    {
                        case Player.State.WaitingToAttach:
                            break;
                        case Player.State.Normal:
                            TryMine(hit);
                            TryAttachObject(hit);
                            TryUpgradeUnit(hit);
                            break;
                        case Player.State.Attach:
                            TryDeattachObject(hit);
                            break;
                        case Player.State.Build:
                            TryBuildUnit(hit);
                            break;
                        case Player.State.Upgrade:
                            TryUpgradeUnit(hit);
                            break;
                    }
                }
            }
        }
    }

    void TryMine(RaycastHit hit)
    {
        Debug.Log("Start try mine coal source...");

        if (hit.collider.gameObject.tag != coalSourceTag)
        {
            Debug.Log("Hitted game object not coal source");
            return;
        }

        var coalSource = hit.collider.gameObject.GetComponent<CoalSource>();
        if (coalSource == null)
        {
            Debug.LogError("Coal source component not found when mine");
            return;
        }

        if (coalSource.Mine()) {
            _player.state = Player.State.WaitingToAttach;
        }

        Debug.Log("End try mine coal source");
    }

    void TryAttachObject(RaycastHit hit)
    {
        Debug.Log("Start try attach object...");

        if (hit.collider.gameObject.tag != attachableTag)
        {
            Debug.Log("Hitted game object not attachable");
            return;
        }

        var attachable = hit.collider.gameObject.GetComponent<Attachable>();
        if (attachable == null)
        {
            Debug.LogError("Attachable component not found when attach");
            return;
        }

        attachPoint.AttachObject(attachable);
        _player.state = Player.State.Attach;

        Debug.Log("End try attach object");
    }

    void TryDeattachObject(RaycastHit hit)
    {
        Debug.Log("Start try deattach object...");

        switch (hit.collider.gameObject.tag)
        {
            case groundTag:
            case flameTag:
                var newSinnerPosition = hit.point;

                attachPoint.DeattachObject(newSinnerPosition);
                _player.state = Player.State.Normal;
                break;
            default:
                Debug.Log("Hitted game object not ground or flame");
                return;
        }

        Debug.Log("End try deattach object");
    }

    void TryBuildUnit(RaycastHit hit)
    {
    }

    void TryUpgradeUnit(RaycastHit hit)
    {
    }
}
