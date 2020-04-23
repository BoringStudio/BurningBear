using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CoalCoreSpawnHandler))]
public class CoalCore : MonoBehaviour
{
    public bool isEmpty
    {
        get => _coalCapacity <= 0;
    }

    public CoalCoreSpawnHandler spawnHandler { get; private set; }

    [SerializeField] private float _maxProductionProgress = 100.0f;
    [SerializeField] private float _currentProductionProgress = 0.0f;
    [SerializeField] private float _productionRate = 10.0f;
    [SerializeField] private int _coalCapacity = 20;

    [SerializeField] private Coal _coalPrefab;

    [SerializeField] private Player _player;
    [SerializeField] private Inferno _inferno;

    void Awake()
    {
        spawnHandler = spawnHandler ?? GetComponent<CoalCoreSpawnHandler>();

        Assert.IsNotNull(_player, "[CoalCore] Player is null");
        Assert.IsNotNull(_inferno, "[CoalCore] Inferno is null");
        Assert.IsNotNull(spawnHandler, "[CoalCore] Spawn handler is null");
    }

    public bool Mine()
    {
        bool result = false;

        _currentProductionProgress += _productionRate;
        if (_currentProductionProgress >= _maxProductionProgress)
        {
            _currentProductionProgress = 0.0f;
            _coalCapacity -= 1;
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
        _inferno.Spawn(
            _coalPrefab.GetComponent<Spawnable>(),
            _player.attachPoint.transform.position,
            _player.attachPoint.transform);
    }

    private void DespawnCoalCore()
    {
        _inferno.Despawn(spawnHandler);
    }
}
