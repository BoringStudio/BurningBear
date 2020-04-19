using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public void DoSpawnStart(GameObject by)
    {
        OnSpawnStart(by);
    }
    public void DoSpawnEnd(GameObject by)
    {
        OnSpawnEnd(by);
    }
    public void DoDespawnStart(GameObject by)
    {
        OnDespawnStart(by);
    }
    public void DoDespawnEnd(GameObject by)
    {
        OnDespawnEnd(by);
    }

    protected virtual void OnSpawnStart(GameObject by) { }
    protected virtual void OnSpawnEnd(GameObject by) { }
    protected virtual void OnDespawnStart(GameObject by) { }
    protected virtual void OnDespawnEnd(GameObject by) { }

}