using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalCoreSpawner : MonoBehaviour
{
    [SerializeField] private Coal _spawnablePrefab = null;
    private Inferno _inferno = null;
    private CoalCoreSpawnerManager _spawnerManager = null;

    public bool isActive
    {
        get;
        private set;
    } = false;

    void Awake()
    {
        _inferno = _inferno ?? Inferno.Instance;
        _spawnerManager = _spawnerManager ?? CoalCoreSpawnerManager.Instance;
    }

    void Update()
    {
    }

    public void SpawnCoalCore()
    {
        if (!isActive)
        {
            var spawnHandler = _inferno.Spawn(_spawnablePrefab.AsSpawnable(), transform.position, transform);
            spawnHandler.spawner = this;
            isActive = true;
        }
        else
        {
            Debug.LogError("Try spawn coal core at occupied spawner");
        }
    }

    public void FreeCoalCore()
    {
        isActive = false;
        _spawnerManager.UpdateNumberOfActiveCoalCore();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.png");
    }
}
