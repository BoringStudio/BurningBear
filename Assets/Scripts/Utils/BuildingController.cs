using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    public Unit currentBuilding { get; private set; }

    [SerializeField] private Color _activeButtonColor;
    [SerializeField] private Color _disabledButtonColor;

    [SerializeField] private Pot _pot;
    [SerializeField] private Player _player;

    [SerializeField] private Suck _suckPrefab;
    [SerializeField] private Dragon _dragonPrefab;

    [SerializeField] private Button _suckBuildingButton;
    [SerializeField] private Button _dragonBuildingButton;

    private Image _suckBuildingButtonImage;
    private Image _dragonBuildingButtonImage;

    void Awake()
    {
        Assert.IsNotNull(_player, "[BuildingController]: Player is null");

        Assert.IsNotNull(_suckBuildingButton, "[BuildingController]: Suck building button is null");
        Assert.IsNotNull(_dragonBuildingButton, "[BuildingController]: Dragon building button is null");

        _suckBuildingButton.onClick.AddListener(() => SetUnit(_suckPrefab));
        _dragonBuildingButton.onClick.AddListener(() => SetUnit(_dragonPrefab));

        _suckBuildingButtonImage = _suckBuildingButton.GetComponent<Image>();
        _dragonBuildingButtonImage = _dragonBuildingButton.GetComponent<Image>();

        Assert.IsNotNull(_suckBuildingButtonImage, "[BuildingController]: Suck building button image is null");
        Assert.IsNotNull(_dragonBuildingButtonImage, "[BuildingController]: Dragon building button image is null");
    }

    private void Update()
    {
        SetButtonEnabled(_suckBuildingButton, _suckBuildingButtonImage, _pot.souls >= _suckPrefab.cost);
        SetButtonEnabled(_dragonBuildingButton, _dragonBuildingButtonImage, _pot.souls >= _dragonPrefab.cost);
    }

    private void OnDestroy()
    {
        _suckBuildingButton.onClick.RemoveAllListeners();
        _dragonBuildingButton.onClick.RemoveAllListeners();
    }

    public void SetUnit(Unit unit)
    {
        currentBuilding = unit;

        if (unit)
        {
            _player.StartBuild();
        }
    }

    private void SetButtonEnabled(Button button, Image buttonImage, bool enabled)
    {
        button.interactable = enabled;
        buttonImage.color = enabled ? _activeButtonColor : _disabledButtonColor;
    }
}
