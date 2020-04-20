using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalCoreSpawnerManager : Singleton<CoalCoreSpawnerManager>
{
    [SerializeField] private float _minTimeToSpawn = 20.0f;
    [SerializeField] private float _maxTimeToSpawn = 100.0f;
    [SerializeField] private int _maxActiveCoalCoreInstances = 10;
    private CoalCoreSpawner[] _spawners;

    private int _currentActiveCoalCoreInstances = 0;
    private float _currentTimeToSpawn = 0.0f;
    private float _waitSpawnUntil = -1.0f;
    private bool _needGenerateTimeToSpawn
    {
        get => _waitSpawnUntil < 0.0f;
    }

    void Awake()
    {
        _spawners = GetComponentsInChildren<CoalCoreSpawner>();
    }

    void Update()
    {
        if (_needGenerateTimeToSpawn)
        {
            _waitSpawnUntil = Random.Range(_minTimeToSpawn, _maxTimeToSpawn);
        }

        if (_currentActiveCoalCoreInstances <
            Mathf.Min(_maxActiveCoalCoreInstances, _spawners.Length))
        {
            _currentTimeToSpawn += Time.deltaTime;

            if (_currentTimeToSpawn >= _waitSpawnUntil)
            {
                _currentTimeToSpawn = 0.0f;
                _waitSpawnUntil = -1.0f;
                _currentActiveCoalCoreInstances += 1;

                SpawnCoalCore();

                Debug.Log("Spawned coal core");
            }
        }
    }

    private CoalCoreSpawner GetFirstActive()
    {
        foreach (CoalCoreSpawner spawner in _spawners)
        {
            if (!spawner.isActive)
            {
                return spawner;
            }
        }

        return null;
    }

    private void SpawnCoalCore()
    {
        CoalCoreSpawner spawner = GetFirstActive();
        if (spawner == null) {
            Debug.LogError("No not active coal core spawners");
            return;
        }

        spawner.SpawnCoalCore();
    }

    public void UpdateNumberOfActiveCoalCore() {
        _currentActiveCoalCoreInstances = 0;
        foreach (CoalCoreSpawner spawner in _spawners)
        {
            if (spawner.isActive)
            {
                _currentActiveCoalCoreInstances += 1;
            }
        }
    }
}
