using UnityEngine;

public class SinnerSpawnerManager : MonoBehaviour
{
    [SerializeField] private float _minTimeToSpawn = 20.0f;
    [SerializeField] private float _maxTimeToSpawn = 100.0f;
    [SerializeField] private int _maxActiveSinnerInstances = 10;

    private SinnerSpawner[] _spawners;

    private int _currentActiveSinnerInstances = 0;
    private float _currentTimeToSpawn = 0.0f;
    private float _waitSpawnUntil = -1.0f;
    private bool _needGenerateTimeToSpawn
    {
        get => _waitSpawnUntil < 0.0f;
    }

    void Awake()
    {
        _spawners = GetComponentsInChildren<SinnerSpawner>();
    }

    void Update()
    {
        if (_needGenerateTimeToSpawn)
        {
            _waitSpawnUntil = Random.Range(_minTimeToSpawn, _maxTimeToSpawn);
        }

        if (_currentActiveSinnerInstances <
            Mathf.Min(_maxActiveSinnerInstances, _spawners.Length))
        {
            _currentTimeToSpawn += Time.deltaTime;

            if (_currentTimeToSpawn >= _waitSpawnUntil)
            {
                _currentTimeToSpawn = 0.0f;
                _waitSpawnUntil = -1.0f;
                _currentActiveSinnerInstances += 1;

                SpawnSinner();

                Debug.Log("Spawned sinner");
            }
        }
    }

    private SinnerSpawner GetFirstActive()
    {
        foreach (SinnerSpawner spawner in _spawners)
        {
            if (!spawner.isActive)
            {
                return spawner;
            }
        }

        return null;
    }

    private void SpawnSinner()
    {
        SinnerSpawner spawner = GetFirstActive();
        if (spawner == null)
        {
            Debug.LogError("No not active sinner spawners");
            return;
        }

        spawner.SpawnSinner();
    }

    public void UpdateNumberOfActiveSinner()
    {
        _currentActiveSinnerInstances = 0;
        foreach (SinnerSpawner spawner in _spawners)
        {
            if (spawner.isActive)
            {
                _currentActiveSinnerInstances += 1;
            }
        }
    }
}
