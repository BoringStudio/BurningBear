using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CursorController : Singleton<CursorController>
{
    [SerializeField] private Texture2D _normalCursor;
    [SerializeField] private Texture2D _buildingCursor;
    [SerializeField] private Texture2D _miningCursor;
    [SerializeField] private Texture2D _grabCursor;
    [SerializeField] private Texture2D _upgradeCursor;

    private Player _player = null;
    private GameSettings _gameSettings = null;
    private InputController _inputController = null;

    void Awake()
    {
        _player = _player ?? Player.Instance;
        _gameSettings = _gameSettings ?? GameSettings.Instance;
        _inputController = _inputController ?? InputController.Instance;

        Assert.IsNotNull(_normalCursor, "[CursorController]: Normal cursor texture is null");
        Assert.IsNotNull(_buildingCursor, "[CursorController]: Building cursor texture is null");
        Assert.IsNotNull(_miningCursor, "[CursorController]: Mining cursor texture is null");
        Assert.IsNotNull(_grabCursor, "[CursorController]: Grab cursor texture is null");
        Assert.IsNotNull(_upgradeCursor, "[CursorController]: Upgrade cursor texture is null");

        SetNormalCursor();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = _player.playerCamera.ScreenPointToRay(_inputController.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f, _gameSettings.interactableLayer))
        {
            var hittedGameObject = hit.collider.gameObject;
            var hittedTag = hittedGameObject.tag;

            if (hittedTag == _gameSettings.coalCoreTag)
            {
                SetMiningCursor();
            }
            else if (hittedTag == _gameSettings.attachableTag)
            {
                SetGrabCursor();
            }
            else if (hittedTag == _gameSettings.groundTag && BuildingController.Instance.currentBuilding)
            {
                SetBuildingCursor();
            }
            else if (hittedTag == _gameSettings.unitTag)
            {
                SetUpgradeCursor();
            }
            else
            {
                SetNormalCursor();
            }
        }
    }

    public void SetNormalCursor()
    {
        Cursor.SetCursor(_normalCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetBuildingCursor()
    {
        Cursor.SetCursor(_buildingCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetMiningCursor()
    {
        Cursor.SetCursor(_miningCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetGrabCursor()
    {
        Cursor.SetCursor(_grabCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetUpgradeCursor()
    {
        Cursor.SetCursor(_upgradeCursor, Vector2.zero, CursorMode.Auto);
    }
}
