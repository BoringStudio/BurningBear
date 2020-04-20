using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CoalCoreSpawnHandler))]
public class CoalCore : MonoBehaviour
{
    [SerializeField] private Coal _coalPrefab;

    private CoalCoreSpawnHandler _spawnHandler = null;

    public float maxProductionProgress = 100.0f;
    public float currentProductionProgress = 0.0f;
    public float productionRate = 10.0f;
    public int coalCapacity = 20;

    public bool isEmpty
    {
        get => coalCapacity <= 0;
    }

    private Inferno _inferno;
    private Player _player;

    // Start is called before the first frame update
    void Awake()
    {
        _inferno = _inferno ?? Inferno.Instance;
        _player = _player ?? Player.Instance;
        _spawnHandler = _spawnHandler ?? GetComponent<CoalCoreSpawnHandler>();

        Assert.IsNotNull(_spawnHandler, "[CoalCore] Spawn handler is null");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public CoalCoreSpawnHandler AsSpawnable() {
        return _spawnHandler ?? GetComponent<CoalCoreSpawnHandler>();
    }

    public bool Mine()
    {
        bool result = false;
        
        currentProductionProgress += productionRate;
        if (currentProductionProgress >= maxProductionProgress)
        {
            currentProductionProgress = 0.0f;
            coalCapacity -= 1;
            SpawnCoal();
            result = true;
        }

        if (isEmpty)
        {
            DespawnCoalCore();
        }

        return result;
    }

    public void SpawnCoal()
    {
        var coal = _inferno.Spawn(
            _coalPrefab.GetComponent<Spawnable>(),
            _player.attachPoint.transform.position,
            _player.attachPoint.transform);
    }

    private void DespawnCoalCore()
    {
        _inferno.Despawn(_spawnHandler);
    }
}
