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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
