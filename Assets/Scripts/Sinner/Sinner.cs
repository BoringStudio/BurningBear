using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinner : Spawnable
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Water")
        {
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag != "Water")
        {
            return;
        }
    }

    protected override void OnSpawnStart(GameObject by) { }
    protected override void OnSpawnEnd(GameObject by) { }
    protected override void OnDespawnStart(GameObject by) { }
    protected override void OnDespawnEnd(GameObject by) { }
}
