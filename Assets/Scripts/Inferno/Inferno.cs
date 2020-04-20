using UnityEngine;

public class Inferno : Singleton<MonoBehaviour>
{
    [SerializeField] InfernoHand _handPrefab;

    public void Spawn(Spawnable prefab, Vector3 position)
    {
        var entity = Instantiate(prefab);
        var hand = Instantiate(_handPrefab, position, Quaternion.identity);
        hand.PlaceEntity(entity);
    }

    public void Despawn(Spawnable entity)
    {
        var position = entity.transform.position;
        var hand = Instantiate(_handPrefab, position, Quaternion.identity);
        hand.RemoveEntity(entity);
    }
}
