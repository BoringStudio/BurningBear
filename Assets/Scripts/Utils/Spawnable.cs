using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public Transform handAttachmentPoint { get => _handAttachmentPoint; }
    [SerializeField] private Transform _handAttachmentPoint;

    public bool despawned
    {
        get;
        private set;
    } = false;
    public bool despawnStarted
    {
        get;
        private set;
    } = false;

    public void DoSpawnImmediately(GameObject by)
    {
        OnSpawnImmediately(by);
    }
    public void DoSpawnStart(GameObject by)
    {
        OnSpawnStart(by);
    }
    public void DoSpawnEnd(GameObject by)
    {
        OnSpawnEnd(by);
    }
    public void DoDespawnImmediately(GameObject by)
    {
        if (!despawned && !despawnStarted)
        {
            OnDespawnImmediately(by);
        }

        despawnStarted = true;
        despawned = true;
    }
    public void DoDespawnStart(GameObject by)
    {
        if (!despawned && !despawnStarted)
        {
            OnDespawnStart(by);
        }

        despawnStarted = true;
    }
    public void DoDespawnEnd(GameObject by)
    {
        if (!despawned && despawnStarted)
        {
            OnDespawnEnd(by);
        }

        despawned = true;
    }

    protected virtual void OnSpawnImmediately(GameObject by) { }
    protected virtual void OnSpawnStart(GameObject by) { }
    protected virtual void OnSpawnEnd(GameObject by) { }
    protected virtual void OnDespawnImmediately(GameObject by) { }
    protected virtual void OnDespawnStart(GameObject by) { }
    protected virtual void OnDespawnEnd(GameObject by) { }

}