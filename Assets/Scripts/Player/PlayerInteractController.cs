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


    private const string flameTag = "Flame";
    private const string groundTag = "Ground";
    private const string sinnerTag = "Sinner";
    private const string wallTag = "Wall";
    private const string waterTag = "Water";

    void Awake()
    {
        _player = _player ?? gameObject.GetComponentInChildren<Player>();
        _inputController = _inputController ?? gameObject.GetComponentInChildren<InputController>();
        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();
        attachPoint = attachPoint ?? gameObject.GetComponentInChildren<AttachPoint>();
        mouseOriginInteractionPoint = mouseOriginInteractionPoint ?? _player?.transform;

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
                        case Player.State.Normal:
                            TryAttachSinner(hit);
                            TryUpgradeUnit(hit);
                            break;
                        case Player.State.Attach:
                            TryDeattachSinner(hit);
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

    void TryAttachSinner(RaycastHit hit)
    {
        Debug.Log("Start try attach sinner...");

        if (hit.collider.gameObject.tag != sinnerTag)
        {
            Debug.Log("Hitted game object not sinner");
            return;
        }

        var sinner = hit.collider.gameObject.GetComponentInChildren<Attachable>();
        if (sinner == null)
        {
            Debug.LogError("Sinner component not found when grub");
            return;
        }

        attachPoint.AttachObject(sinner, attachPoint.transform);
        _player.state = Player.State.Attach;

        Debug.Log("End try attach sinner");
    }

    void TryDeattachSinner(RaycastHit hit)
    {
        Debug.Log("Start try deattach sinner...");

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

        Debug.Log("End try deattach sinner");
    }

    void TryBuildUnit(RaycastHit hit)
    {
    }

    void TryUpgradeUnit(RaycastHit hit)
    {
    }
}
