using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SinnerSpawner : MonoBehaviour
{
    [SerializeField] private Sinner _sinnerPrefab = null;

    [SerializeField] private Inferno _inferno = null;
    [SerializeField] private SinnerSpawnerManager _spawnerManager = null;

    public bool isActive
    {
        get;
        private set;
    } = false;

    void Awake()
    {
        Assert.IsNotNull(_sinnerPrefab, "[SinnerSpawner]: Sinner prefab is null");

        Assert.IsNotNull(_inferno, "[SinnerSpawner]: Inferno is null");
        Assert.IsNotNull(_spawnerManager, "[SinnerSpawner]: Spawner manager is null");
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
