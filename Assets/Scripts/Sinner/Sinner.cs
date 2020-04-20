using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Attachable))]
[RequireComponent(typeof(Spawnable))]
public class Sinner : MonoBehaviour
{
    private SinnerAttachHandler _attachHandler = null;
    private SinnerSpawnHandler _spawnHandler = null;

    public SinnerAttachHandler AsAttachable()
    {
        return _attachHandler ?? (_attachHandler = GetComponent<SinnerAttachHandler>());
    }

    public SinnerSpawnHandler AsSpawnable()
    {
        return _spawnHandler ?? (_spawnHandler = GetComponent<SinnerSpawnHandler>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        _attachHandler = _attachHandler ?? GetComponent<SinnerAttachHandler>();
        _spawnHandler = _spawnHandler ?? GetComponent<SinnerSpawnHandler>();

        Assert.IsNotNull(_attachHandler, "[Sinner]: Attach handler is null");
        Assert.IsNotNull(_spawnHandler, "[Sinner]: Spawn handler is null");
    }
}
