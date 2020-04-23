using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D _normalCursor;
    [SerializeField] private Texture2D _buildingCursor;
    [SerializeField] private Texture2D _miningCursor;
    [SerializeField] private Texture2D _grabCursor;
    [SerializeField] private Texture2D _upgradeCursor;

    [SerializeField] private Player _player;
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private InputController _inputController;
    [SerializeField] private BuildingController _buildingController;

    void Awake()
    {
        Assert.IsNotNull(_normalCursor, "[CursorController]: Normal cursor texture is null");
        Assert.IsNotNull(_buildingCursor, "[CursorController]: Building cursor texture is null");
        Assert.IsNotNull(_miningCursor, "[CursorController]: Mining cursor texture is null");
        Assert.IsNotNull(_grabCursor, "[CursorController]: Grab cursor texture is null");
        Assert.IsNotNull(_upgradeCursor, "[CursorController]: Upgrade cursor texture is null");

        Assert.IsNotNull(_player, "[CursorController]: Player is null");
        Assert.IsNotNull(_gameSettings, "[CursorController]: Game settings is null");
        Assert.IsNotNull(_inputController, "[CursorController]: Input controller is null");
        Assert.IsNotNull(_buildingController, "[CursorController]: Building controller is null");

        SetNormalCursor();
    }

    void Update()
    {
        Ray ray = _player.playerCamera.ScreenPointToRay(_inputController.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, _gameSettings.interactableLayer))
        {
            var hittedGameObject = hit.collider.gameObject;
            var hittedTag = hittedGameObject.tag;

            if (hittedTag == _gameSettings.coalCoreTag && _player.state == Player.State.Normal)
            {
                SetMiningCursor();
            }
            else if (hittedTag == _gameSettings.attachableTag && _player.state == Player.State.Normal)
            {
                SetGrabCursor();
            }
            else if (hittedTag == _gameSettings.groundTag && _player.state == Player.State.Build && _buildingController.currentBuilding)
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
