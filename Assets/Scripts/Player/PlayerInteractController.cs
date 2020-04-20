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
    private GameSettings _gameSettings = null;
    private InputController _inputController = null;
    private Camera _camera = null;
    private Inferno _inferno = null;

    void Awake()
    {
        _inferno = _inferno ?? Inferno.Instance;
        _player = _player ?? Player.Instance;
        _inputController = _inputController ?? InputController.Instance;
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        attachPoint = attachPoint ?? gameObject.GetComponentInChildren<AttachPoint>();
        mouseOriginInteractionPoint = mouseOriginInteractionPoint ?? transform;
        _gameSettings = _gameSettings ?? GameSettings.Instance;

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

            if (Physics.Raycast(ray, out hit, 100.0f, _gameSettings.interactableLayer))
            {
                var hittedGameObject = hit.collider.gameObject;
                var hittedTag = hittedGameObject.tag;
                var hitPosition = hit.point;
                var distance = (hitPosition - mouseOriginInteractionPoint.position).magnitude;

                Debug.Log("Distance to interacted object: " + distance);
                Debug.Log("Hitted game object: " + hittedGameObject);
                Debug.Log("Hitted object tag: " + hittedTag);
                Debug.Log("Hitted point position: " + hitPosition);
                if (distance <= mouseInteractionRadius)
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
                            TryTossToPot(hit);
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
        Debug.Log("Start try mine coal core...");

        if (hit.collider.gameObject.tag != _gameSettings.coalCoreTag)
        {
            Debug.Log("Hitted game object not coal source");
            return;
        }

        var coalCore = hit.collider.gameObject.GetComponent<CoalCore>();
        if (coalCore == null)
        {
            Debug.LogError("Coal core component not found when mine");
            return;
        }

        if (coalCore.Mine())
        {
            _player.state = Player.State.WaitingToAttach;
        }

        Debug.Log("End try mine coal core");
    }

    void TryAttachObject(RaycastHit hit)
    {
        Debug.Log("Start try attach object...");

        if (hit.collider.gameObject.tag != _gameSettings.attachableTag)
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

        if (hit.collider.gameObject.tag != _gameSettings.groundTag)
        {
            Debug.Log("Hitted game object not ground");
            return;
        }

        var newObjectPosition = new Vector3(hit.point.x, 0, hit.point.z);

        attachPoint.DeattachObject(newObjectPosition);
        _player.state = Player.State.Normal;

        Debug.Log("End try deattach object");
    }

    void TryTossToPot(RaycastHit hit)
    {
        Debug.Log("Start try toss to pot...");

        if (hit.collider.gameObject.tag != _gameSettings.flameTag)
        {
            Debug.Log("Hitted game object not flame");
            return;
        }

        Pot pot = hit.collider.gameObject.GetComponent<Pot>();
        if (pot == null)
        {
            Debug.Log("Hitted game object doesnt have pot component");
            return;
        }

        var newObjectPosition = new Vector3(hit.point.x, 0, hit.point.z);
        GameObject objectToDeattach = attachPoint.attachedObject.gameObject;

        attachPoint.DeattachObject(newObjectPosition);
        pot.TossObject(objectToDeattach);
        _player.state = Player.State.Normal;

        Debug.Log("End try toss to pot");
    }

    void TryBuildUnit(RaycastHit hit)
    {
    }

    void TryUpgradeUnit(RaycastHit hit)
    {
    }
}
