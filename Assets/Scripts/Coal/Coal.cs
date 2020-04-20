using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Attachable))]
[RequireComponent(typeof(Spawnable))]
public class Coal : MonoBehaviour
{
    public float powerCapacity = 100.0f;

    private CoalAttachHandler _attachHandler = null;
    private CoalSpawnHandler _spawnHandler = null;

    public CoalAttachHandler AsAttachable()
    {
        return _attachHandler ?? (_attachHandler = GetComponent<CoalAttachHandler>());
    }

    public CoalSpawnHandler AsSpawnable()
    {
        return _spawnHandler ?? (_spawnHandler = GetComponent<CoalSpawnHandler>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        _attachHandler = _attachHandler ?? GetComponent<CoalAttachHandler>();
        _spawnHandler = _spawnHandler ?? GetComponent<CoalSpawnHandler>();

        Assert.IsNotNull(_attachHandler, "[Coal]: Attach handler is null");
        Assert.IsNotNull(_spawnHandler, "[Coal]: Spawn handler is null");
    }
}
