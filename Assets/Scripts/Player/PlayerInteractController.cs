using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteractController : MonoBehaviour
{
    public AttachPoint attachPoint;

    [SerializeField] private Transform _mouseOriginInteractionPoint = null;
    [SerializeField] private float _mouseInteractionRadius = 5f;

    [SerializeField] private Player _player = null;
    [SerializeField] private Inferno _inferno = null;
    [SerializeField] private Pot _pot = null;

    [SerializeField] private GameSettings _gameSettings = null;
    [SerializeField] private InputController _inputController = null;
    [SerializeField] private BuildingController _buildingController = null;

    private Camera _camera = null;

    void Awake()
    {
        attachPoint = gameObject.GetComponentInChildren<AttachPoint>();

        _camera = _camera ?? gameObject.GetComponentInChildren<Camera>();

        _mouseOriginInteractionPoint = _mouseOriginInteractionPoint ?? transform;

        Assert.IsNotNull(_player, "[PlayerInteractController]: Player is null");
        Assert.IsNotNull(_inferno, "[PlayerInteractController]: Inferno is null");
        Assert.IsNotNull(_pot, "[PlayerInteractController]: Pot is null");

        Assert.IsNotNull(_gameSettings, "[PlayerInteractController]: Game settings is null");
        Assert.IsNotNull(_inputController, "[PlayerInteractController]: Input controller is null");
        Assert.IsNotNull(_buildingController, "[PlayerInteractController]: Building controller is null");

        Assert.IsNotNull(_camera, "[PlayerInteractController]: Camera is null");
        Assert.IsNotNull(attachPoint, "[PlayerInteractController]: Attach point is null");
    }

    void Update()
    {
        UpdateMouseInteract();
    }

    void UpdateMouseInteract()
    {
        if (_inputController.releaseButtonDown && _player.state == Player.State.Build)
        {
            _buildingController.SetUnit(null);
            _player.state = Player.State.Normal;
        }

        if (_inputController.interactButtonDown)
        {
            Ray ray = _camera.ScreenPointToRay(_inputController.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, _gameSettings.interactableLayer))
            {
                var hittedGameObject = hit.collider.gameObject;
                var hittedTag = hittedGameObject.tag;
                var hitPosition = hit.point;
                var distance = (hitPosition - _mouseOriginInteractionPoint.position).magnitude;

                Debug.Log("Distance to interacted object: " + distance);
                Debug.Log("Hitted game object: " + hittedGameObject);
                Debug.Log("Hitted object tag: " + hittedTag);
                Debug.Log("Hitted point position: " + hitPosition);
                if (distance <= _mouseInteractionRadius)
                {
                    switch (_player.state)
                    {
                        case Player.State.WaitingToAttach:
                            break;
                        case Player.State.Normal:
                            //TryMine(hit);
                            TryAttachObject(hit);
                            //TryUpgradeUnit(hit);
                            break;
                        case Player.State.Attach:
                            TryDeattachObject(hit);
                            TryTossToPot(hit);
                            break;
                        case Player.State.Build:
                            TryBuildUnit(hit);
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

        var attachable = hit.collider.gameObject.GetComponentInParent<Attachable>();
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

        _player.state = Player.State.Normal;
        attachPoint.DeattachObject(newObjectPosition);

        Debug.Log("End try deattach object");
    }

    void TryTossToPot(RaycastHit hit)
    {
        Debug.Log("Start try toss to pot...");

        if (!hit.collider.gameObject.CompareTag(_gameSettings.flameTag))
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
        var building = _buildingController.currentBuilding;
        if (building == null)
        {
            return;
        }

        if (_pot.souls < _buildingController.currentBuilding.cost)
        {
            return;
        }

        Debug.Log("Start try build object...");

        if (!hit.collider.gameObject.CompareTag(_gameSettings.groundTag))
        {
            Debug.Log("Hitted game object not ground");
            return;
        }

        Vector3 point = hit.point;
        point.x = Mathf.Ceil(point.x);
        point.y = 0;
        point.z = Mathf.Ceil(point.z);

        if (!Physics.Raycast(point + Vector3.up * 10, Vector3.down, out var newHit, 11.0f, _gameSettings.interactableLayer))
        {
            return;
        }

        if (!newHit.collider.gameObject.CompareTag(_gameSettings.groundTag))
        {
            Debug.Log("Hitted game object not ground");
            return;
        }

        _pot.TakeSouls(_buildingController.currentBuilding.cost);
        _inferno.Spawn(building, point, null);

        _buildingController.SetUnit(null);

        _player.state = Player.State.Normal;

        Debug.Log("End try build object");
    }

    void TryUpgradeUnit(RaycastHit hit)
    {
    }
}
