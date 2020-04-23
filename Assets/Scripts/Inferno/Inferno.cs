using UnityEngine;

public class Inferno : MonoBehaviour
{
    [SerializeField] private InfernoHand _handPrefab;

    public T Spawn<T>(T prefab, Vector3 position, Transform followTarget) where T : Spawnable
    {
        var entity = Instantiate(prefab, position, Quaternion.identity);
        var hand = Instantiate(_handPrefab, position, Quaternion.identity);
        hand.PlaceEntity(entity, followTarget);

        return entity;
    }

    public void Despawn(Spawnable entity)
    {
        if (entity.despawnStarted || entity.despawned) {
            return;
        }
        
        var position = entity.transform.position;
        var hand = Instantiate(_handPrefab, position, Quaternion.identity);
        hand.RemoveEntity(entity);
    }
}
