using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    public Vector3 cameraRotation = new Vector3(60, 0, 0);
    public LayerMask interactableLayer;

    public readonly string groundTag = "Ground";
    public readonly string attachableTag = "Attachable";
    public readonly string coalCoreTag = "CoalCore";
    public readonly string flameTag = "Flame";
    public readonly string unitTag = "Unit";
}
