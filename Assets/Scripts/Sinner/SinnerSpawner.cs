using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinnerSpawner : MonoBehaviour
{
    [SerializeField] private Sinner _sinnerPrefab = null;
    private Inferno _inferno = null;
    private SinnerSpawnerManager _spawnerManager = null;

    public bool isActive
    {
        get;
        private set;
    } = false;

    void Awake()
    {
        _inferno = _inferno ?? Inferno.Instance;
        _spawnerManager = _spawnerManager ?? SinnerSpawnerManager.Instance;
    }

    void Update()
    {
    }

    public void SpawnSinner()
    {
        if (!isActive)
        {
            var spawnHandler = _inferno.Spawn(_sinnerPrefab.AsSpawnable(), transform.position, transform);
            spawnHandler.spawner = this;
            isActive = true;
        }
        else
        {
            Debug.LogError("Try spawn sinner at occupied spawner");
        }
    }

    public void FreeSinner()
    {
        isActive = false;
        _spawnerManager.UpdateNumberOfActiveSinner();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.png");
    }
}
