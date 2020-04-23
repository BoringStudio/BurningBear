using UnityEngine;
using UnityEngine.Assertions;

public class CoalCoreSpawner : MonoBehaviour
{
    [SerializeField] private Coal _coalPrefab = null;

    [SerializeField] private Inferno _inferno = null;
    [SerializeField] private CoalCoreSpawnerManager _spawnerManager = null;

    public bool isActive
    {
        get;
        private set;
    } = false;

    void Awake()
    {
        Assert.IsNotNull(_coalPrefab, "[CoalCoreSpawner]: Coal prefab is null");
        Assert.IsNotNull(_inferno, "[CoalCoreSpawner]: Inferno is null");
        Assert.IsNotNull(_spawnerManager, "[CoalCoreSpawner]: Spawn manager is null");
    }

    public void SpawnCoalCore()
    {
        if (!isActive)
        {
            var spawnHandler = _inferno.Spawn(_coalPrefab.AsSpawnable(), transform.position, transform);
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
